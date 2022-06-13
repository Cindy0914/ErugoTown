using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using UnityEngine;

public class CustomDistanceGrabber : MonoBehaviour
{
    // GrabObject마다 잡을수 있는 손이 정해져있음
    // 따라서 오브젝트의 태그를 이용해 상호작용손을 나눠줬음
    // 만약 양손 모두 잡을수있는 오브젝트라면 양쪽모두 멤버로 들고있어야함
    public enum LeftInteractionObjectTag
    {
        Throw,
        Food,

    }

    public enum RightInteractionObjectTag
    {
        Gun,
        Throw,
        Food,

    }

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
    }

    private void Update()
    {
        switch (controller)
        {
            case OVRInput.Controller.LTouch:
                if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
                    FindInteracteObject(typeof(LeftInteractionObjectTag));
                if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
                    EndGrab();
                break;
            case OVRInput.Controller.RTouch:
                if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
                    FindInteracteObject(typeof(LeftInteractionObjectTag));
                if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
                    EndGrab();
                break;
        }
    }

    private void FindInteracteObject(System.Type type)
    {
        RaycastHit hit;
        if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit, maxGrabDistance, 1 << (int)targetLayer))
        {
            currentGrabbable = hit.collider.gameObject;

            var interacteObject = System.Enum.GetValues(type);
            foreach (var obj in interacteObject)
            {
                if (obj.ToString().Equals(currentGrabbable.tag))
                {
                    grabbable = currentGrabbable.GetComponent<CustomGrabbable>();
                    BeginGrab();
                    break;
                }
            }
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
