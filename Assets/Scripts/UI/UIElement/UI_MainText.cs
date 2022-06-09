using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainText : MonoBehaviour
{
    [SerializeField]
    Text text;
    Transform textTrans;

    private void Start()
    {
        textTrans = text.GetComponent<Transform>();
    }

    public void SetText(string _text,Color color)
    {
        text.gameObject.SetActive(true);
        textTrans.localPosition = new Vector3(0,-0.4f,0);
        text.color = color;
        text.text = _text;
        textTrans.DOLocalMove(new Vector3(0, 0.4f, 0), 2f).SetEase(Ease.Linear);
        text.DOFade(0, 4f).OnComplete(() => text.gameObject.SetActive(false));
    }
}
