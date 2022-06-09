using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Car))]
public class VehicleController : MonoBehaviour
{
    VrPlayer player;
    Car car;
    Rigidbody rigid;

    [SerializeField]
    WheelCollider[] wheelColliders;

    [SerializeField]
    GameObject[] wheelMeshs = new GameObject[4];

    [SerializeField]
    GameObject handle;

    [SerializeField]
    float motorTorque;
    [SerializeField]
    float steerAngle;

    [SerializeField]
    float downForce;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        TryGetComponent(out car);

        TryGetComponent(out rigid);
        rigid.centerOfMass = new Vector3(0, -1, 0);

        this.FixedUpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Car))
            .Select(axis => OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick))
            .Subscribe(axis =>
            {
                Vehicle(axis);
            });

        this.UpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Car))
            .Subscribe(_ =>
            {
                ContentsManager.Instance.vrCamera.gameObject.transform.rotation = car.inTrans.rotation;
            });

        // 하차
        this.UpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Car))
            .Where(_ => OVRInput.GetDown(OVRInput.Button.One))
            .Subscribe(_ =>
            {
                Fade.Instance.FadeInOut(() =>
                {
                    player.transform.SetParent(null);
                    player.transform.position = car.outTrans.position;
                    player.transform.rotation = car.outTrans.rotation;
                    //car.rigid.velocity = Vector3.zero;

                    for (int i = 0; i < 4; i++)
                    {
                        wheelColliders[i].motorTorque = 0;
                    }

                    player.canMove = true;
                    player.state = VrPlayer.State.Basic;

                    player.col.enabled = true;
                    player.rigid.useGravity = true;
                    player.rigid.isKinematic = false;
                    ContentsManager.Instance.vrCamera.target = player.gameObject;
                    ContentsManager.Instance.vrCamera.cameraDeltaPos = Vector3.up;

                    player.l_laser.SetActive(true);
                    player.r_laser.SetActive(true);
                });
            });
    }

    // 비히클 기본 이동
    private void Vehicle(Vector2 axis)
    {
        rigid.AddForce(-transform.up * downForce * rigid.velocity.magnitude);

        for (int i = 0; i < 4; i++)
        {
            Quaternion q;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out q);
            wheelMeshs[i].transform.SetPositionAndRotation(pos, q);
        }

        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = axis.x * steerAngle;
        if (handle != null)
            handle.transform.localRotation = Quaternion.Euler(0, 0, handle.transform.localRotation.eulerAngles.z + axis.x);

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = axis.y * ((motorTorque * 100f) * 0.4f);
        }
    }

    // 승차
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Fade.Instance.FadeInOut(() =>
            {
                player.transform.SetParent(car.inTrans);
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.identity;

                player.canMove = false;
                player.state = VrPlayer.State.Car;

                player.col.enabled = false;
                player.rigid.useGravity = false;
                player.rigid.isKinematic = true;

                player.transform.SetParent(car.inTrans);
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.identity;
                // ContentsManager.Instance.vrCamera.cameraDeltaPos = Vector3.down;

                player.l_laser.SetActive(false);
                player.r_laser.SetActive(false);
            });
        }
    }
}
