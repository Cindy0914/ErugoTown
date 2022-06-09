using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class VrPlayerFishing : MonoBehaviour
{
    VrPlayer player;
    GameObject r_Hand;
    [SerializeField]
    Bobber bobber;

    [SerializeField]
    GameObject[] fishs;

    enum FishingState
    {
        Start,
        End
    }

    FishingState state = FishingState.Start;

    public bool canFishing { get; set; } = false;

    // ����Ⱑ ��������
    [HideInInspector]
    public bool canCatch = false;
    // ���� ����������
    bool isFishing = false;
    bool IsFishing
    {
        get => isFishing;
        set
        {
            if (value && !isFishing)
            {
                StopCoroutine(nameof(CalculateCatchProbability));
                StartCoroutine(nameof(CalculateCatchProbability));
            }

            isFishing = value;
        }
    }

    // �ܰ迡 ���� �ð�
    const float timeOverLevel = 5;
    // �ܰ迡 ���� �����ð� ������
    float minTolFluctuationWidth = -3;
    float maxTolFluctuationWidth = 3;

    // �ð��� ���� ���� ����Ȯ��
    /// <summary>
    /// 1/16 -> 1/8 -> 1/4 -> 1/2
    /// </summary>
    float probabilityOverTime = 6.25f;
    float originprobabilityOverTime = 6.25f;

    float successRandValue = 10000;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        r_Hand = player.r_grabber.gameObject;

        /// ��ȹ Ȯ���� ���� �Ǿ�����
        this.ObserveEveryValueChanged(value => value.probabilityOverTime)
            .Skip(1)
            .Subscribe(_ =>
            {
                WhetherSuccess();
            });

        this.ObserveEveryValueChanged(value => value.canCatch)
            .Where(_ => canCatch == true)
            .Subscribe(_ =>
            {
                StartCoroutine(Util.Util.ViberateRepeat(5, 0.03f, 0.5f, 0.8f));
            });

        // ���ÿ� ���� ��������
        UpdateObserve_FishingButtonDownUp();
    }

    private void UpdateObserve_FishingButtonDownUp()
    {
        float fishingStartZPos = 0f;
        float fishingEndZPos = 0f;

        this.UpdateAsObservable()
           .Where(_ => player.state.Equals(VrPlayer.State.Fishing))
           .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
           .Subscribe(_ =>
           {
               switch (state)
               {
                   // ���� ����
                   case FishingState.Start:

                       player.canMove = false;
                       IsFishing = true;
                       canCatch = false;

                       fishingStartZPos = r_Hand.transform.localPosition.z;
                       successRandValue = UnityEngine.Random.Range(0f, 100f);
                       break;
               }
           });

        this.UpdateAsObservable()
           .Where(_ => player.state.Equals(VrPlayer.State.Fishing))
           .Where(_ => OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
           .Subscribe(_ =>
           {
               switch (state)
               {
                   case FishingState.Start:
                       bobber.rigid.isKinematic = false;
                       fishingEndZPos = r_Hand.transform.localPosition.z;

                       // ���˴� ������
                       bobber.rigid.AddForce((Camera.main.transform.forward + Vector3.up) * (10 + (Mathf.Abs(fishingEndZPos - fishingStartZPos) * 50)), ForceMode.Impulse);
                       state = FishingState.End;
                       break;
                   case FishingState.End:
                       bobber.rigid.isKinematic = true;
                       fishingStartZPos = 0;
                       fishingEndZPos = 0;
                       EndFishing();

                       if (canCatch)
                       {
                           // ���� ����
                           var randNum = UnityEngine.Random.Range(0, fishs.Length);
                           var fish = ObjectPoolManager.Instance.Spawn(fishs[randNum].name);
                           var fishRigid = fish.GetComponent<Rigidbody>();
                           fishRigid.isKinematic = true;
                           fish.transform.position = bobber.transform.position;
                           fish.transform.rotation = Quaternion.identity;
                           fish.gameObject.transform.SetParent(bobber.gameObject.transform);

                           Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                           .Subscribe(_ =>
                           {
                               fish.gameObject.transform.SetParent(null);
                               fishRigid.isKinematic = false;
                           });

                           player.mainText.SetText($"[{fish.name}]��(��) ���ҽ��ϴ�!", Color.yellow);
                       }
                       break;
               }
           });
    }

    private void WhetherSuccess()
    {
        if (successRandValue <= probabilityOverTime)
        {
            // Success
            var fishShadowClone = ObjectPoolManager.Instance.Spawn("FishShadow");
            fishShadowClone.transform.position = bobber.transform.position;
            fishShadowClone.transform.rotation = Quaternion.identity;

            fishShadowClone.GetComponent<FishShadow>().RotateFish();

            StopCoroutine(nameof(CalculateCatchProbability));
        }
        else
        {
            // Fail


            // ���� ����
            if (probabilityOverTime >= 50f)
            {
                player.mainText.SetText($"�̷�..��ó�� ����Ⱑ ��������..", Color.gray);
                FailedFishing();
            }
        }
    }

    private void FailedFishing()
    {
        EndFishing();
        // TODO 
    }

    public void EndFishing()
    {
        player.canMove = true;
        bobber.rigid.isKinematic = true;
        bobber.transform.DOLocalMove(Vector3.forward * 19, 0.3f).SetEase(Ease.OutQuad);
        // ���� ��
        IsFishing = false;
        probabilityOverTime = originprobabilityOverTime;
        state = FishingState.Start;
    }

    private IEnumerator CalculateCatchProbability()
    {
        var waitTime = new WaitForSeconds(CalculateTOL());
        int intervalCount = 0;

        while (intervalCount < 4)
        {
            yield return waitTime;
            probabilityOverTime *= 2;
            intervalCount++;
        }
    }

    // �������� ������ Time Over Level�� ����
    private float CalculateTOL()
    {
        var randTol = UnityEngine.Random.Range(minTolFluctuationWidth, maxTolFluctuationWidth);
        var totalTol = randTol + timeOverLevel;
        return totalTol;
    }
}
