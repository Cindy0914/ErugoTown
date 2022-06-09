using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DownLoad : UI_RideBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        presenter.downLoadBtn.Add(this);
    }
}
