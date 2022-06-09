using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrPlayerGameInteraction : MonoBehaviour
{
    VrPlayer player;
    Transform ridesTrans = null;

    public Transform RidesTrans
    {
        get => ridesTrans;
        set
        {
            ridesTrans = value;

            transform.SetParent(ridesTrans);

            player.mainText.SetText("내리시려면 [Y]를 누르세요.", Color.black);
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
    }

}
