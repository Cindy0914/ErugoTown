using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UI_GunInfo : MonoBehaviour
{
    public VrPlayer player;
    public Image gunImage;
    public Sprite[] gunSprites;
    public Text bulletCountText;

    int bulletCount = 0;

    private void Start()
    {
        this.ObserveEveryValueChanged(_ => player.state)
            .TakeUntilDestroy(this)
            .Subscribe(_ =>
            {
                player.playerShooting.bulletCount = 0;
                switch (player.state)
                {
                    case VrPlayer.State.Shooting:
                        gunImage.sprite = gunSprites[0];
                        bulletCount = 30;
                        break;
                    case VrPlayer.State.MachineGunShooting:
                        gunImage.sprite = gunSprites[1];
                        bulletCount = 75;
                        break;
                }
            });

        this.ObserveEveryValueChanged(_ => player.playerShooting?.bulletCount)
            .TakeUntilDestroy(this)
            .Subscribe(_ =>
            {
                if (player.playerShooting.bulletCount < bulletCount)
                    bulletCountText.text = $"BulletCount : {bulletCount - player.playerShooting?.bulletCount}";
                else
                    bulletCountText.text = "Please. ReLoad";
            });
    }
}
