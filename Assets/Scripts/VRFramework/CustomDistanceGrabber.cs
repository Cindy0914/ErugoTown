using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using UnityEngine;

public class CustomDistanceGrabber : MonoBehaviour
{
    /// <summary>
    /// 오브젝트 인식 최대거리
    /// </summary>
    [SerializeField]
    float maxGrabDistance;
    /// <summary>
    /// 오브젝트를 당기는 시간
    /// </summary>
    [SerializeField, Range(0f, 2f)]
    float pullDuration;
    [SerializeField]
    Define.Layer targetLayer = Define.Layer.Grabbable;
    public OVRInput.Controller controller;
    [SerializeField]
    Transform gripTrans;
    LineRenderer laser;

    [SerializeField]
    Ease ease = Ease.Linear;

    [HideInInspector]
    public GameObject currentGrabbable;
    CustomGrabbable grabbable;

    public float throwSpeed;

    private void Start()
    {
        laser = gripTrans.gameObject.GetComponentInChildren<LineRenderer>();

        laser.SetPosition(1, new Vector3(0, 0, maxGrabDistance));
        laser.transform.Find("Point").transform.localPosition = new Vector3(0, 0, maxGrabDistance);

        switch (controller)
        {
            case OVRInput.Controller.LTouch:
                this.UpdateAsObservable()
                    .Where(_ => OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
                    .Subscribe(_ =>
                    {
                        FindGrabbableObject();
                    });

                this.UpdateAsObservable()
                    .Where(_ => OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
                    .Subscribe(_ =>
                    {
                        EndGrab();
                    });
                break;

            case OVRInput.Controller.RTouch:
                this.UpdateAsObservable()
                    .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
                    .Subscribe(_ =>
                    {
                        FindGrabbableObject();
                    });

                this.UpdateAsObservable()
                    .Where(_ => OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
                    .Subscribe(_ =>
                    {
                        EndGrab();
                    });
                break;
        }
    }

    private void FindGrabbableObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit, maxGrabDistance, 1 << (int)targetLayer))
        {
            currentGrabbable = hit.collider.gameObject;
            grabbable = currentGrabbable.GetComponent<CustomGrabbable>();
            BeginGrab();
        }
    }

    private void BeginGrab()
    {
        StartCoroutine(Util.Util.VibrateController(0.2f, 0.2f, 0.2f, controller));

        grabbable.Rigid.isKinematic = true;
        currentGrabbable.transform.SetParent(gripTrans);
        currentGrabbable.transform.DOLocalMove(Vector3.zero, pullDuration).SetEase(ease);
        currentGrabbable.transform.DOLocalRotate(Vector3.zero, pullDuration);
    }

    private void Grabbing()
    {

    }

    private void EndGrab()
    {
        if (currentGrabbable != null)
        {
            currentGrabbable.transform.SetParent(null);
            ThrowObject();
            grabbable.Rigid.isKinematic = grabbable.originKinematic;
        }
        grabbable = null;
        currentGrabbable = null;
    }

    private void ThrowObject()
    {
        var velocity = OVRInput.GetLocalControllerVelocity(controller);
        grabbable.Rigid.velocity = velocity * throwSpeed;
        grabbable.Rigid.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller) * throwSpeed;
    }
}
