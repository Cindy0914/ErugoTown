using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public enum AnimParam
    {
        Flex,
        Pinch
    }

    [SerializeField]
    Animator anim;

    CustomDistanceGrabber grabber;

    private void Start()
    {
        TryGetComponent(out grabber);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                HandAnimByInputValue();
            });
    }

    private void HandAnimByInputValue()
    {
        anim.SetFloat(AnimParam.Flex.ToString(), OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, grabber.controller));
        anim.SetFloat(AnimParam.Pinch.ToString(), OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, grabber.controller));
    }
}
