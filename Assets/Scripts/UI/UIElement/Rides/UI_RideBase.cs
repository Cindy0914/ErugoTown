using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RideBase : MonoBehaviour
{
    public Button button;
    public Transform inTrans;
    public Transform outTrans;
    protected RidesPresenter presenter;

    protected virtual void OnEnable()
    {
        presenter = GameObject.FindObjectOfType<RidesPresenter>();
    }
}
