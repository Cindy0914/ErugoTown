using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T _instance;
    public static T Instance { get =>  _instance; }

    public abstract bool dontDestroy { get; set; }

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize() 
    {
        if (_instance == null)
        {
            _instance = this as T;

            if (dontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
