using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    VrPlayer player;
    public GameObject processingObj;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera") && !processingObj.activeSelf)
        {
            processingObj.SetActive(true);
            player.state = VrPlayer.State.Swimming;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera") && processingObj.activeSelf)
        {
            processingObj.SetActive(false);
            player.state = VrPlayer.State.Basic;
        }
    }
}
