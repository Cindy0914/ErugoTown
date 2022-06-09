using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleAnimation : MonoBehaviour
{
    public Vector3 endValue;
    public float duration;
    public Ease ease = Ease.Linear;

    Vector3 originLocalVec;

    public bool canMovePosition = true;
    public bool canRotate = false;

    private void Start()
    {
        originLocalVec = transform.localPosition;
        //transform.localPosition += endValue;
        if (canMovePosition)
            Invoke(nameof(PlayAnimateByDirectionForLocal), UnityEngine.Random.Range(0f, 2.0f));
        if (canRotate)
            Invoke(nameof(PlayAnimateByRotate), UnityEngine.Random.Range(0f, 2.0f));

    }

    private void PlayAnimateByDirectionForLocal()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.SetEase(ease);
        sequence.Append(transform.DOLocalMove(originLocalVec + endValue, duration));
        sequence.Append(transform.DOLocalMove(originLocalVec, duration));
        sequence.SetLoops(-1);
        sequence.Play();
        //sequence.Restart(false);
    }

    private void PlayAnimateByRotate()
    {
        transform.DOLocalRotate(endValue, duration).SetEase(ease)
            .SetLoops(-1,LoopType.Incremental);
    }
}
