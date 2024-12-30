using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class SingletonNetworkAbstract<T> : WNetworkBehaviour where T : SingletonNetworkAbstract<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = NetworkObject.FindObjectOfType<T>();
                if (instance == null)
                {
                    Debug.LogError($"No {typeof(T).Name} Singleton Instance.");
                }
            }
            return instance;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        if (instance as T != null)
        {
            NetworkObject.Destroy(this.gameObject);
        }
        instance = (T)this;
    }
}

