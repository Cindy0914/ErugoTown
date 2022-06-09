using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System;

public class TextFade : MonoBehaviour
{
    Text text;

    private void Start()
    {
        TryGetComponent(out text);
        Observable.Interval(TimeSpan.FromSeconds(1f))
            .Subscribe(_ =>
            {
                text.DOColor(new Color(0, 0, 0, 0.5f), 1f).OnComplete(() =>
                {
                    text.DOColor(new Color(0, 0, 0, 0f), 1f);
                });
            });
    }
}
