using System.Collections;
using System.Collections.Generic;
using UniRx;
using DG.Tweening;
using UnityEngine;

public class SkyBoxBlendController : MonoBehaviour
{
    SkyboxBlender skyboxBlender;
    [SerializeField]
    Light directionalLight;


    private void Start()
    {
        skyboxBlender = GetComponent<SkyboxBlender>();
        StartCoroutine(nameof(StartSkyBoxBlend));

        this.ObserveEveryValueChanged(_ => skyboxBlender.CurrentIndex)
            .Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                if (value == 0)
                    DOTween.To(() => directionalLight.intensity, value => directionalLight.intensity = value, 1f, 2f);
                else if (value == 1)
                    DOTween.To(() => directionalLight.intensity, value => directionalLight.intensity = value, 0.5f, 2f);
            });
    }

    IEnumerator StartSkyBoxBlend()
    {
        yield return new WaitForSeconds(skyboxBlender.timeToWait);
        skyboxBlender.SkyboxBlend();
    }
}
