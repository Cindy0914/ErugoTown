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

    [HideInInspector]
    public int bulletCount = 0;

    int thumpDownCount = 0;
    bool isThumpDown = false;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        UpdateObserve_Shot();
        UpdateObserve_MashineGunShot();
        UpdateObserve_ReLoad();
    }

    private void UpdateObserve_Shot()
    {
        this.UpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Shooting))
            .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            .Where(_ => bulletCount < 30)
            .Subscribe(_ =>
            {
                ShotBullet(0.5f, 0.5f, 0.5f);
                bulletCount++;
            });
    }
    private void UpdateObserve_MashineGunShot()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.05f))
            .Where(_ => player.state.Equals(VrPlayer.State.MachineGunShooting))
            .Where(_ => OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            .Where(_ => bulletCount < 75)
            .Subscribe(_ =>
            {
                ShotBullet(0.03f, 0.5f, 0.5f);
                bulletCount++;
            });
    }
    private void UpdateObserve_ReLoad()
    { 
        this.UpdateAsObservable()
            .Where(_ =>
            {
                if (player.state.Equals(VrPlayer.State.Shooting))
                    return (bulletCount >= 30);
                else if (player.state.Equals(VrPlayer.State.MachineGunShooting))
                    return (bulletCount >= 75);
                return false;
            })
            .Select(axis => OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick))
            .Subscribe(axis => ReLoad(axis));
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
        yield return new WaitForSeconds(0.3f);
        isThumpDown = false;
    }
}
