using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    public Rigidbody rigid { get; private set; }
    VrPlayerFishing playerFishing;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        playerFishing = ContentsManager.Instance.vrPlayer.playerFishing;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            playerFishing.EndFishing();
        }
    }
}
