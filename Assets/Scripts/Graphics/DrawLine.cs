using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    VrPlayer player;
    public UI_BrushColor brushColor;

    public GameObject rightHand;
    LineRenderer lineRender;
    List<Vector3> points = new List<Vector3>();

    PoolObject poolObj;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        TryGetComponent(out poolObj);

        Observable.Timer(TimeSpan.FromMinutes(3f))
            .Where(_ => poolObj != null)
            .TakeUntilDestroy(this)
            .Subscribe(_ => ObjectPoolManager.Instance.Despawn(poolObj));
    }

    private void Update()
    {
        if (player.state.Equals(VrPlayer.State.Basic))
        {
            if (brushColor.laserPointer.currentObject == null)
            {
                if (!brushColor.canSketch)
                    return;

                Vector3 position = rightHand.transform.position;

                if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                {
                    var line = ObjectPoolManager.Instance.Spawn("Line");
                    lineRender = line.GetComponent<LineRenderer>();
                    points.Add(position);
                    lineRender.positionCount = 1;
                    lineRender.SetPosition(0, points[0]);

                    lineRender.startColor = brushColor.savedColor;
                    lineRender.endColor = brushColor.savedColor;
                }
                else if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                {
                    Vector3 pos = position;
                    if (Vector3.Distance(pos, points[lineRender.positionCount - 1]) > 0.01f)
                    {
                        points.Add(pos);
                        lineRender.positionCount++;
                        lineRender.SetPosition(lineRender.positionCount - 1, pos);
                    }
                }
                else if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
                    points.Clear();
            }
        }
    }
}
