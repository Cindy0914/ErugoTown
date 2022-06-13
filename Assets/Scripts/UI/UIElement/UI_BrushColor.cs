using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UI_BrushColor : MonoBehaviour
{
    public RectTransform colorWheel;
    public Texture2D colorTexture;
    public Laser.LaserPointer laserPointer;
    public Image selectColorImg;

    [HideInInspector]
    public Color savedColor;

    public bool canSketch = false;

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => laserPointer.currentObject == this.gameObject)
            .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            .Subscribe(_ =>
            {
                selectColorImg.gameObject.SetActive(true);
                canSketch = true;
            });

        this.UpdateAsObservable()
            .Where(_ => laserPointer.currentObject == this.gameObject)
            .Where(_ => OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            .Subscribe(_ =>
            {
                GetColorFromWheel();
                selectColorImg.color = savedColor;
            });

        this.UpdateAsObservable()
            .Where(_ => laserPointer.currentObject == this.gameObject)
            .Where(_ => OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
            .Subscribe(_ => selectColorImg.gameObject.SetActive(false));
    }

    private void Update()
    {
        if(laserPointer.currentObject.Equals(this.gameObject))
        {
            if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                selectColorImg.gameObject.SetActive(true);
                canSketch = true;
            }
            if(OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                GetColorFromWheel();
                selectColorImg.color = savedColor;
            }
            if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
                selectColorImg.gameObject.SetActive(false);
        }
    }

    public void GetColorFromWheel()
    {
        var localPos = colorWheel.transform.InverseTransformPoint(laserPointer.hit.point);
        var toImg = new Vector2(localPos.x / colorWheel.sizeDelta.x + 0.5f, localPos.y / colorWheel.sizeDelta.y + 0.5f);
        Color sampledColor = Color.black;
        if (toImg.x < 1.0 && toImg.x > 0.0f && toImg.y < 1.0 && toImg.y > 0.0f)
        {
            int Upos = Mathf.RoundToInt(toImg.x * colorTexture.width);
            int Vpos = Mathf.RoundToInt(toImg.y * colorTexture.height);
            sampledColor = colorTexture.GetPixel(Upos, Vpos);
        }
        savedColor = new Color(sampledColor.r, sampledColor.g, sampledColor.b, 1);
    }
}
