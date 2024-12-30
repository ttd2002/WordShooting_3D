using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkDespawnByTime : WNetworkBehaviour
{
    [SerializeField] protected float delay = 3f;
    [SerializeField] protected float timer = 0f;
    void Update()
    {
        this.Despawning();
    }
    void OnEnable()
    {
        this.ResetTimer();
    }
    protected virtual void ResetTimer()
    {
        this.timer = 0;
    }
    protected bool CanDespawn()
    {
        this.timer += Time.deltaTime;
        if (this.timer > this.delay) return true;
        return false;
    }

    protected virtual void Despawning()
    {
        if (!this.CanDespawn()) return;
        this.DespawnObject();
    }
    protected virtual void DespawnObject()
    {
        Runner.Despawn(transform.parent.transform.GetComponent<NetworkObject>());
    }
}
