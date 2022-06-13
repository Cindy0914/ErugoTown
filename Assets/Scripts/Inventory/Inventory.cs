using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    VrPlayer player;
    public Image backImage;
    GameObject slotHolder;
    GameObject slot;
    List<Slot> slotList = new List<Slot>();

    public GameObject inven;


    private void Start()
    {
        TryGetComponent(out player);
    }

    public void Open()
    {

    }

    public void Close()
    {

    }
}
