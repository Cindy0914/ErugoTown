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

        this.UpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Basic))
            .Where(_ => OVRInput.GetDown(OVRInput.Button.Four))
            .Subscribe(_ =>
            {
                inven.SetActive(true);
            });


        this.UpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Basic))
            .Where(_ => OVRInput.GetUp(OVRInput.Button.Four))
            .Subscribe(_ =>
            {
                inven.SetActive(false);
            });


        /*        slotHolder = backImage.transform.GetChild(0).gameObject;
                for (int i = 0; i < 30;i++)
                {
                    slot = ObjectPoolManager.Instance.Spawn("Slot").gameObject;
                    slot.transform.SetParent(slotHolder.transform);
                    slot.transform.localScale = Vector3.one;
                    slot.transform.localPosition = new Vector3(0, 0, 0);
                    slotList.Add(slot.GetComponent<Slot>());
                }

                Close();*/
    }

    public void Open()
    {
        /*        backImage.DOFade(1, 0);

                foreach(var slot in slotList)
                {
                    slot.icon.DOFade(1, 0);
                }*/
        backImage.gameObject.SetActive(true);
    }

    public void Close()
    {
        /*backImage.DOFade(0, 0);

        foreach (var slot in slotList)
        {
            slot.icon.DOFade(0, 0);
        }*/
        backImage.gameObject.SetActive(false);
    }
}
