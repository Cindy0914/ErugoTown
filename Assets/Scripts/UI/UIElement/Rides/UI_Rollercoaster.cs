using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Rollercoaster : UI_RideBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        presenter.rollercoasterBtn.Add(this);
    }
}
