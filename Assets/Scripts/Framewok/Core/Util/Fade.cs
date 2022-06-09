using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Fade : Singleton<Fade>
{
    public override bool dontDestroy { get; set; } = false;

    public Image fadeImg;

    protected override void Initialize()
    {
        base.Initialize();
        FadeInOut();
    }

    public void FadeInOut(Action fadeInCompleteAction = null)
    {
        fadeImg.DOFade(1.0f, 2f).OnComplete(() =>
        {
            fadeInCompleteAction?.Invoke();
            fadeImg.DOFade(0.0f, 2f);
        });
    }

}
