using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [HideInInspector]
    public Image icon;
    Text amountText;

    SDItem itemInfo = new SDItem();

    private void Start()
    {
        TryGetComponent(out icon);
        transform.GetChild(0).TryGetComponent(out amountText);
    }

    public void RefreshSlot()
    {
        icon.sprite = ResourceManager.Instance.Load<Sprite>(itemInfo.spritePath);
        amountText.text = $"X{itemInfo.amount}";
    }
}
