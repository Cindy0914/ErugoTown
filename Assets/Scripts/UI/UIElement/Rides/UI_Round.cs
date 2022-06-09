using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Round : UI_RideBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        presenter.roundBtn.Add(this);
    }
}
