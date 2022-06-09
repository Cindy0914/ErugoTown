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
    [SerializeField, Header("���ϻ��� �ڵ����� ���� ��")]
    int maxAltitude;

    // ���� ���ϻ��� ��ĥ�� �ִ����� ���� ����
    bool canOpenAltitude = false;
    // ���� ���ϻ��� ������ �ִ����� ���� ����
    bool isOpenAltitude = false;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        rigid = player.rigid;
        originDrag = rigid.drag;

        CheckCanFalling();
        Falling();
    }

    // Ư�� Ű�� �����ų�, Ư�� ���� �ٴٶ����� isOpenAltitude�� Ȱ��ȭ
    private void CheckCanFalling()
    {
        CreateBoundary();

        this.UpdateAsObservable()
            .Where(_ => canOpenAltitude)
            .Where(_ => OVRInput.GetDown(OVRInput.Button.Three))
            .Subscribe(_ => CheckInputAltitudeKey());

        // Ư�� ���� ������ Collider ����
        void CreateBoundary()
        {
            GameObject boundaryCube = new GameObject { name = "FallingBoundaryCube" };
            boundaryCube.tag = "BoundaryCube";
            boundaryCube.transform.position = new Vector3(0, maxAltitude, 0);
            BoxCollider boundaryCollider = boundaryCube.AddComponent<BoxCollider>();
            boundaryCollider.size = new Vector3(900, 10, 900);
            boundaryCollider.isTrigger = true;
        }

        // Ư�� Ű�� ��������
        void CheckInputAltitudeKey()
        {
            isOpenAltitude = true;
        }
    }

    // �������� ���϶��� �� ó�� 
    // ����Ʈ Ȱ��ȭ
    // RigidBody.Drag �� ����
    // �ٴ� ���� �˻� 
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

        // ���ϻ��� ����/���� Ÿ�̹�
        this.ObserveEveryValueChanged(_ => isOpenAltitude)
            .Skip(System.TimeSpan.Zero)
            .Subscribe(_ =>
            {
                // ���ϻ��� ������
                if (isOpenAltitude)
                {
                    altitude = ObjectPoolManager.Instance.Spawn("Altitude");
                    altitude.transform.SetParent(transform);
                    altitude.transform.localPosition = Vector3.zero;
                    altitude.transform.localRotation = Quaternion.identity;

                    rigid.drag = dragSpeed;
                }
                // ���ϻ��� ������
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

    // �ٴ� ���� �Ŀ� ���� ó��
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
            // ������ �Ʒ��� �������� ����
            if (rigid.velocity.y < 0)
                isOpenAltitude = true;
        }
    }
}

