using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform inTrans;
    public Transform outTrans;

    [HideInInspector]
    public Rigidbody rigid;

    private void Start()
    {
        TryGetComponent(out rigid);
    }
}
