using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonSpawner<T> : Spawner where T : SingletonSpawner<T>
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
        if (instance as T != null) Debug.LogError("Only 1 " + typeof(T).Name + " allow to exist");
        instance = (T)this;
    }

}
