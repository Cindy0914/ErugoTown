using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Bullet : PoolObject
{
    Rigidbody rigid;
    public float bulletSpeed = 30;

    protected override void Start()
    {
        TryGetComponent(out rigid);
    }

    public override void Action()
    {
        if (rigid == null)
            TryGetComponent(out rigid);

        rigid.velocity = Vector3.zero;
        rigid.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);

        Observable.Timer(TimeSpan.FromSeconds(1f))
            .Subscribe(_ => ObjectPoolManager.Instance.Despawn(this));
    }

}
