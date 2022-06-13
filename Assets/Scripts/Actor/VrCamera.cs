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

    private void Start()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(hor, 0, ver));

        transform.position = transform.position + 
            new Vector3(hor, 0, ver);

        player = ContentsManager.Instance.vrPlayer;
    }

    private void LateUpdate()
    {
        transform.position = (cameraDeltaPos + target.transform.position);
    }
}
