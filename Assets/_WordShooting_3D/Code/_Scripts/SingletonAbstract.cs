using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonAbstract<T> : WMonoBehaviour where T : SingletonAbstract<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindObjectOfType<T>();
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
            Object.Destroy(this.gameObject);
        }
        instance = (T)this;
    }
}



