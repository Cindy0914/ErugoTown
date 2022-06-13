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

    bool groundCheck = false;

    // 입력상태
    bool isMove = false;
    bool isJump = false;
    bool isRotate = false;
    bool isSwim = false;

    // 키값
    Vector2 input;
    Quaternion cameraRot;
    Quaternion swimCameraRot;


    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        rigid = player.rigid;
        cameraRig = ContentsManager.Instance.vrCamera.gameObject;
    }

    private void Update()
    {
        input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        input = (input.magnitude >= 1) ? input.normalized : input;

        if (player.canMove)
        {
            if (isSwim)
                isSwim = false;

            // 이동
            {
                isMove = (input.magnitude > 0.1f);

                if (isMove)
                {
                    if (!rigid.useGravity)
                        rigid.useGravity = true;
                }
            }

            // 카메라에 따른 회전
            {
                cameraRot = Quaternion.LookRotation(centerCamera.transform.forward);
                cameraRot.x = 0;
                cameraRot.z = 0;
                isRotate = true;
            }

            // 점프
            {
                if (OVRInput.GetDown(OVRInput.Button.Three) && groundCheck)
                {
                    isJump = true;
                }
            }
        }
        else
        {
            isMove = false;
            isRotate = false;
            isJump = false;

            // 수영
            if (player.state.Equals(VrPlayer.State.Swimming))
            {
                isSwim = true;
                rigid.useGravity = false;
                rigid.velocity = Vector3.zero;

                swimCameraRot = Quaternion.LookRotation(centerCamera.transform.forward);
                swimCameraRot.z = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMove)
            Movement(input, moveSpeed);

        if (isRotate)
            transform.rotation = cameraRot;


        groundCheck =
            Physics.Raycast(transform.position + (Vector3.down * 0.5f), Vector3.down, 1f, 1 << LayerMask.NameToLayer("Ground"));
        if (isJump)
        {
            rigid.velocity = Vector3.zero;
            rigid.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        if (isSwim)
        {
            transform.rotation = swimCameraRot;
            Movement(input, swimSpeed);
        }

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
