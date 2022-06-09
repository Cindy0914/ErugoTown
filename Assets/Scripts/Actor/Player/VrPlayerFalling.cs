using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class VrPlayerFalling : MonoBehaviour
{
    VrPlayer player;
    Rigidbody rigid;

    PoolObject altitude;

    [SerializeField]
    float dragSpeed;
    float originDrag;
    [SerializeField, Header("낙하산이 자동으로 펴질 고도")]
    int maxAltitude;

    // 현재 낙하산을 펼칠수 있는지에 대한 여부
    bool canOpenAltitude = false;
    // 현재 낙하산이 펼쳐져 있는지에 대한 여부
    bool isOpenAltitude = false;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        rigid = player.rigid;
        originDrag = rigid.drag;

        CheckCanFalling();
        Falling();
    }

    // 특정 키를 누르거나, 특정 고도에 다다랐을때 isOpenAltitude를 활성화
    private void CheckCanFalling()
    {
        CreateBoundary();

        this.UpdateAsObservable()
            .Where(_ => canOpenAltitude)
            .Where(_ => OVRInput.GetDown(OVRInput.Button.Three))
            .Subscribe(_ => CheckInputAltitudeKey());

        // 특정 고도를 측정할 Collider 생성
        void CreateBoundary()
        {
            GameObject boundaryCube = new GameObject { name = "FallingBoundaryCube" };
            boundaryCube.tag = "BoundaryCube";
            boundaryCube.transform.position = new Vector3(0, maxAltitude, 0);
            BoxCollider boundaryCollider = boundaryCube.AddComponent<BoxCollider>();
            boundaryCollider.size = new Vector3(900, 10, 900);
            boundaryCollider.isTrigger = true;
        }

        // 특정 키를 눌렀을때
        void CheckInputAltitudeKey()
        {
            isOpenAltitude = true;
        }
    }

    // 떨어지는 중일때의 후 처리 
    // 이펙트 활성화
    // RigidBody.Drag 값 조절
    // 바닥 착지 검사 
    private void Falling()
    {
        this.UpdateAsObservable()
            .Where(_ => !player.state.Equals(VrPlayer.State.Falling))
            .Where(_ => rigid.velocity.y < -10)
            .Subscribe(_ =>
            {
                player.state = VrPlayer.State.Falling;
                canOpenAltitude = true;
            });

        // 낙하산이 펴질/접힐 타이밍
        this.ObserveEveryValueChanged(_ => isOpenAltitude)
            .Skip(System.TimeSpan.Zero)
            .Subscribe(_ =>
            {
                // 낙하산이 펴질때
                if (isOpenAltitude)
                {
                    altitude = ObjectPoolManager.Instance.Spawn("Altitude");
                    altitude.transform.SetParent(transform);
                    altitude.transform.localPosition = Vector3.zero;
                    altitude.transform.localRotation = Quaternion.identity;

                    rigid.drag = dragSpeed;
                }
                // 낙하산이 접힐때
                else
                {
                    EndFalling();
                }
            });

        this.UpdateAsObservable()
            .Where(_ => isOpenAltitude)
            .Where(_ => Physics.Raycast(transform.position, Vector3.down, 2f, 1 << LayerMask.NameToLayer("Ground")))
            .Subscribe(_ =>
            {
                canOpenAltitude = false;
                isOpenAltitude = false;
            });
    }

    // 바닥 착지 후에 대한 처리
    private void EndFalling()
    {
        if (isOpenAltitude)
            return;

        player.state = VrPlayer.State.Basic;

        ObjectPoolManager.Instance.Despawn(altitude);
        rigid.drag = originDrag;
        canOpenAltitude = false;
        isOpenAltitude = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BoundaryCube"))
        {
            canOpenAltitude = true;
            // 위에서 아래로 떨어지는 판정
            if (rigid.velocity.y < 0)
                isOpenAltitude = true;
        }
    }
}

