using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class VrPlayerGrab : MonoBehaviour
{
    VrPlayer player;
    CustomDistanceGrabber l_grabber;
    CustomDistanceGrabber r_grabber;

    [HideInInspector]
    public float originThrowSpeed;
    public float objectThrowSpeed;

    public GameObject gunInfoCanvas;
    GameObject currentGrabObject;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        l_grabber = player.l_grabber;
        r_grabber = player.r_grabber;

        originThrowSpeed = l_grabber.throwSpeed;
        CheckNullOfGrabbedObjectByValueChange();
    }

    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            GrabbableInteraction();
    }

    private void GrabbableInteraction()
    {
        currentGrabObject = GetCurrentGrabbedObject();

        if (currentGrabObject != null)
        {
            l_grabber.throwSpeed = originThrowSpeed;
            r_grabber.throwSpeed = originThrowSpeed;

            player.l_laser.SetActive(false);
            player.r_laser.SetActive(false);

            switch (currentGrabObject.gameObject.tag)
            {
                case "Gun":
                    if (player.playerShooting.muzzle == null)
                    {
                        player.playerShooting.muzzle = currentGrabObject.gameObject.transform.Find("Muzzle");
                        player.state = VrPlayer.State.Shooting;
                        gunInfoCanvas.SetActive(true);
                    }
                    break;
                case "MachineGun":
                    if (player.playerShooting.muzzle == null)
                    {
                        player.playerShooting.muzzle = currentGrabObject.gameObject.transform.Find("Muzzle");
                        player.state = VrPlayer.State.Shooting;
                        gunInfoCanvas.SetActive(true);
                    }
                    break;
                case "Throw":
                    // grabbed.grabbedRigidbody.isKinematic = false;
                    l_grabber.throwSpeed = objectThrowSpeed;
                    r_grabber.throwSpeed = objectThrowSpeed;
                    break;
                case "Food":
                    player.state = VrPlayer.State.Eating;
                    break;
                case "FishingRod":
                    player.state = VrPlayer.State.Fishing;
                    break;
            }
        }
    }

    private void CheckNullOfGrabbedObjectByValueChange()
    {
        r_grabber.ObserveEveryValueChanged(value => value.currentGrabbable)
            .Skip(System.TimeSpan.Zero)
            .Where(value => value == null)
            .Subscribe(_ =>
            {
                EndGrab();
            });
    }

    private GameObject GetCurrentGrabbedObject()
    {
        if (l_grabber.currentGrabbable != null)
            return l_grabber.currentGrabbable;
        else if (r_grabber.currentGrabbable != null)
            return r_grabber.currentGrabbable;
        else return null;
    }

    private void EndGrab()
    {
        player.l_laser.SetActive(true);
        player.r_laser.SetActive(true);
        player.playerShooting.muzzle = null;
        player.state = VrPlayer.State.Basic;
        gunInfoCanvas.SetActive(false);
    }
}
