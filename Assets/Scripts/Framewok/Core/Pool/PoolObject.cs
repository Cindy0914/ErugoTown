using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

using KeyType = System.String;

[DisallowMultipleComponent]
public class PoolObject : MonoBehaviour
{
    public KeyType key;

    private Transform root;
    [HideInInspector]
    public Transform Root 
    {
        get { return root; }
        set
        {
            root = value;
            transform.SetParent(root);
        }
    }

    protected virtual void Start()
    {
        key = gameObject.name;
    }

    public PoolObject Clone()
    {
        GameObject go = Object.Instantiate(gameObject);
        go.name = gameObject.name;
        if (!go.TryGetComponent(out PoolObject po))
            po = go.AddComponent<PoolObject>();
        go.SetActive(false);

        return po;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public virtual void Action() { }
}
