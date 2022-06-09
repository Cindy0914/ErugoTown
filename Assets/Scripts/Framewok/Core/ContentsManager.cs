using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsManager : Singleton<ContentsManager>
{
    public override bool dontDestroy { get; set; } = false;

    public GameObject player;
    [HideInInspector]
    public VrPlayer vrPlayer;
    Camera mainCamera;

    public VrCamera vrCamera;

    protected override void Initialize()
    {
        base.Initialize();
        player.TryGetComponent(out vrPlayer);
        mainCamera = Camera.main;
    }

    public GameObject GetPlayer() 
    {
/*        if (player == null)
            player = ObjectPoolManager.Instance.GetPoolObject("Player").gameObject;*/
        return player; 
    }

    public Camera GetCamera() { return mainCamera; }
    public void SetMainCamera(Camera camera) 
    {
        mainCamera.tag = "Untagged";
        camera.tag = "MainCamera";
        mainCamera = camera; 
    }
}
