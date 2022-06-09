using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickkableObject : MonoBehaviour
{
    Rigidbody rigid;

    private void Start()
    {
        TryGetComponent(out rigid);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Stickkable"))
        {
            rigid.velocity = Vector3.zero;
            rigid.isKinematic = true;
        }
    }
}
