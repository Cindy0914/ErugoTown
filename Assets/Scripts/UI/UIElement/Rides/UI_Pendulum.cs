using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Pendulum : UI_RideBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        presenter.pendulumBtn.Add(this);
    }
}
