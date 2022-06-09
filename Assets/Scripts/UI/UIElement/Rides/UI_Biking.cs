using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Biking : UI_RideBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        presenter.bikingBtn.Add(this);
    }
}
