using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using DG.Tweening;

public class VrPlayerLoco : MonoBehaviour
{
    VrPlayer player;
    public GameObject centerCamera;
    public float moveSpeed;
    public float jumpSpeed;
    public float flySpeed;
    public float swimSpeed;

    Rigidbody rigid;
    GameObject cameraRig;

    bool canSnapRotate = false;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        rigid = player.rigid;
        cameraRig = ContentsManager.Instance.vrCamera.gameObject;

        // -------- UpdateAsObservable -------
        UpdateObservable_Jump();
        UpdateObservable_VrRotation();

        // -------- FixedUpdateAsObservable --------
        FixedUpdateObserve_VrMovement();
        // FixedUpdateObserve_FlyAxis();
        // FixedUpdateObserve_FlyAction();
        FixedUpdateObserve_Swimming();
    }

    private void FixedUpdateObserve_VrMovement()
    {
        this.FixedUpdateAsObservable() // VR 이동 키 입력
            .Where(_ => player.canMove)
            .Select(_ => OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick))
            .Where(dir => dir.magnitude > 0.1f)
            .Skip(TimeSpan.FromSeconds(1))
            .Subscribe(dir =>
            {
                if (!rigid.useGravity)
                    rigid.useGravity = true;

                dir = (dir.magnitude >= 1) ? dir.normalized : dir;

                Movement(dir, moveSpeed);
            });
    }

    private void UpdateObservable_VrRotation()
    {
        this.FixedUpdateAsObservable()
            .Where(_ => player.canMove)
            .Where(_ => !player.state.Equals(VrPlayer.State.Swimming))
            .Select(_ => Quaternion.LookRotation(centerCamera.transform.forward))
            .Skip(TimeSpan.FromSeconds(1))
            .Subscribe(cameraRot =>
            {
                cameraRot.x = 0;
                cameraRot.z = 0;

                transform.rotation = cameraRot;
            });
    }

    private void UpdateObservable_Jump()
    {
        this.UpdateAsObservable()
            .Where(_ => player.canMove)
            .Where(_ => OVRInput.GetDown(OVRInput.Button.Three))
            .Where(_ => Physics.Raycast(transform.position + (Vector3.down * 0.5f), Vector3.down, 1f, 1 << LayerMask.NameToLayer("Ground")))
            .Subscribe(_ =>
            {
                rigid.velocity = Vector3.zero;
                rigid.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            });
    }

    /*    private void FixedUpdateObserve_FlyAxis()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => player.canMove)
                .Select(_ => OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick))
                .Subscribe(axis =>
                {
                    float yAxis = axis.y;
                    _rigid.velocity = new Vector3(_rigid.velocity.x, yAxis * flySpeed, _rigid.velocity.z);
                });
        }

        private void FixedUpdateObserve_FlyAction()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => player.canMove)
                .Where(_ => OVRInput.Get(OVRInput.Button.Three))
                .Subscribe(_ =>
                {
                    _rigid.velocity = new Vector3(_rigid.velocity.x, flySpeed, _rigid.velocity.z);
                });
        }*/

    private void FixedUpdateObserve_Swimming()
    {
        this.FixedUpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Swimming))
            .Select(axis => OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick))
            .Where(dir => dir.magnitude > 0.1f)
            .Subscribe(dir =>
            {
                rigid.useGravity = false;
                rigid.velocity = Vector3.zero;

                Quaternion q = Quaternion.LookRotation(centerCamera.transform.forward);
                q.z = 0;
                transform.rotation = q;

                dir = (dir.magnitude >= 1) ? dir.normalized : dir;
                Movement(dir, swimSpeed);
            });
    }

    private void Movement(Vector2 dir, float speed)
    {
        var hor = dir.x;
        var ver = dir.y;

        // transform.Translate(new Vector3(hor, 0, ver), Space.Self);

        var rigidDir = transform.TransformDirection(new Vector3(hor, 0, ver));
        rigidDir.Normalize();

        var moveOffset = rigidDir * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveOffset);

    }
}
