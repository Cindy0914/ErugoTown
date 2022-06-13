using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class VrPlayerShooting : MonoBehaviour
{
    VrPlayer player;
    [HideInInspector]
    public Transform muzzle;

    public int bulletCount { get; set; }

    const int MAX_GUNBULLETCOUNT = 30;
    const int MAX_MACHINEGUNBULLETCOUNT = 75;

    int thumpDownCount = 0;
    bool isThumpDown = false;
    WaitForSeconds thumpDownDelay = new WaitForSeconds(0.3f);

    GameObject currentGrabObject;
    CustomDistanceGrabber r_grabber;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        r_grabber = player.r_grabber;
        MashineGunShot();
    }

    private void Update()
    {
        if(player.state.Equals(VrPlayer.State.Shooting))
        {
            var input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            switch (r_grabber.currentGrabbable.tag)
            {
                case "Gun":
                    if(bulletCount >= MAX_GUNBULLETCOUNT)
                    {
                        ReLoad(input);
                        return;
                    }
                    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        ShotBullet(0.5f, 0.5f, 0.5f);
                        bulletCount++;
                    }
                    break;
                case "MachineGun":
                    if (bulletCount >= MAX_MACHINEGUNBULLETCOUNT)
                        ReLoad(input);
                    break;
            }
        }
    }

    private void MashineGunShot()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.05f))
            .Where(_ => r_grabber.currentGrabbable.CompareTag("MachineGun"))
            .Where(_ => OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            .Where(_ => bulletCount < MAX_MACHINEGUNBULLETCOUNT)
            .Subscribe(_ =>
            {
                ShotBullet(0.03f, 0.5f, 0.5f);
                bulletCount++;
            });
    }

    private void ShotBullet(float waitTime, float frequency, float amplitude)
    {
        StopAllCoroutines();
        StartCoroutine(Util.Util.VibrateController(waitTime, frequency, amplitude));
        var bullet = ObjectPoolManager.Instance.Spawn("Bullet");
        bullet.transform.position = muzzle.position;
        bullet.transform.rotation = Quaternion.LookRotation(muzzle.forward);
        bullet.Action();
    }

    private void ReLoad(Vector2 axis)
    {
        var y = axis.y;

        if(y < 0 && !isThumpDown)
        {
            thumpDownCount++;
            isThumpDown = true;
            StartCoroutine(ThumpDownDelayRefresh());
        }

        if(thumpDownCount >= 2)
        {
            thumpDownCount = 0;
            bulletCount = 0;
            isThumpDown = false;
        } 
    }

    private IEnumerator ThumpDownDelayRefresh()
    {
        yield return thumpDownDelay;
        isThumpDown = false;
    }
}
