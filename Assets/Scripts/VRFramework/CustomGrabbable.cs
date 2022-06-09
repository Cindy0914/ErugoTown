using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrabbable : MonoBehaviour
{
    Rigidbody rigid;
    public Rigidbody Rigid { get => rigid; }

    [HideInInspector]
    public bool originKinematic;

    private void Start()
    {
        TryGetComponent(out rigid);
        originKinematic = rigid.isKinematic;
    }
}
