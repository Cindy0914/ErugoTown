using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class VrCamera : MonoBehaviour
{
    VrPlayer player;
    public float rotSpeed = 100f;

    public Vector3 cameraDeltaPos;


    public GameObject target;

    float xRot = 0;
    float yRot = 0;

    private void Start()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");


        transform.Translate(new Vector3(hor, 0, ver));

        transform.position = transform.position + 
            new Vector3(hor, 0, ver);


        player = ContentsManager.Instance.vrPlayer;
        // Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

        yRot = transform.rotation.eulerAngles.x;
        xRot = transform.rotation.eulerAngles.y;

        this.LateUpdateAsObservable().Subscribe(_ => transform.position = (cameraDeltaPos + target.transform.position));
    }

    private void UpdateObservable()
    {
        // this.UpdateAsObservable().Where(_ => !player.mode.Equals(VrPlayer.Mode.DontMove)).Skip(TimeSpan.FromSeconds(1)).Subscribe(_ => PcRotation());
    }

    private void PcRotation()
    {
        xRot += Input.GetAxisRaw("Mouse X") * rotSpeed * Time.deltaTime;
        yRot += Input.GetAxisRaw("Mouse Y") * rotSpeed * Time.deltaTime;

        yRot = Mathf.Clamp(yRot, -60, 60);

        transform.rotation = Quaternion.Euler(-yRot, xRot, 0);
    }
}
