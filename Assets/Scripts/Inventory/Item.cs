using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
    // �������� ItemInfo�� ����־�� �Ѵ�.
    // 3D2D ��� ��� �־�� �ϴ°�? O
    // �ߺ� ������ ������ �ΰ�
    // ��� Add �Ұ��ΰ�

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


    // ������ ȹ��
    // ��ȣ�ۿ�Ű�� �̿��� �������� �۾����鼭 �㸮�뿡 ��ġ�̵�

}
