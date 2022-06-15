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

        CreateBoundary();
        ValueChangeByOpenAltitude();
    }

    private void Update()
    {

        // Ư�� ������ Ű�� �Է¹޾� ���ϻ��� ��ħ
        if (canOpenAltitude && OVRInput.GetDown(OVRInput.Button.Three))
            isOpenAltitude = true;

        if(!player.state.Equals(VrPlayer.State.Falling) && rigid.velocity.y < -10)
        {
            player.state = VrPlayer.State.Falling;
            canOpenAltitude = true;
        }

        if(isOpenAltitude)
        {
            // �ٴڰ��� �Ÿ� üũ
            if(Physics.Raycast(transform.position, Vector3.down, 2f, 1 << LayerMask.NameToLayer("Ground")))
            {
                canOpenAltitude = false;
                isOpenAltitude = false;
            }
        }

    }

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

    // �������� ���϶��� �� ó�� 
    // ����Ʈ Ȱ��ȭ
    // RigidBody.Drag �� ����
    // �ٴ� ���� �˻� 
    private void ValueChangeByOpenAltitude()
    {
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

