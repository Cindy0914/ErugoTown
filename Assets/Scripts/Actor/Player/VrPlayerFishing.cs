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

    // 물고기가 물었는지
    [HideInInspector]
    public bool canCatch = false;
    // 현재 낚시중인지
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

    // 단계에 따른 시간
    const float timeOverLevel = 5;
    // 단계에 따른 랜덤시간 변동폭
    float minTolFluctuationWidth = -3;
    float maxTolFluctuationWidth = 3;

    // 시간에 따른 낚시 성공확률
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

        /// 포획 확률이 변동 되었을때
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

        // 낚시에 대한 상태조절
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
                   // 낚시 시작
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

                       // 낚싯대 던지기
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
                           // 낚시 성공
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

                           player.mainText.SetText($"[{fish.name}]을(를) 낚았습니다!", Color.yellow);
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


            // 낚시 실패
            if (probabilityOverTime >= 50f)
            {
                player.mainText.SetText($"이런..근처에 물고기가 없나봐요..", Color.gray);
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
        // 낚시 끝
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

    // 랜덤값을 포함한 Time Over Level을 연산
    private float CalculateTOL()
    {
        var randTol = UnityEngine.Random.Range(minTolFluctuationWidth, maxTolFluctuationWidth);
        var totalTol = randTol + timeOverLevel;
        return totalTol;
    }
}
