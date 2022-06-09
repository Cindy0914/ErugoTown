using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 아이템이 ItemInfo를 들고있어야 한다.
    // 3D2D 모두 들고 있어야 하는가? O
    // 중복 가능한 아이템 인가
    // 어떻게 Add 할것인가

    [SerializeField]
    int index;

    SDItem itemInfo = new SDItem();

    public Sprite icon { get; set; }

    private void Start()
    {
        itemInfo = DataManager.Instance.SD.sdItem.Where(value =>
        {
            return value.index == index;
        }).SingleOrDefault();
    }


    // 아이템 획득
    // 상호작용키를 이용해 스케일이 작아지면서 허리쯤에 위치이동

}
