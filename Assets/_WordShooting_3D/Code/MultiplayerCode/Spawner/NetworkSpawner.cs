using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkSpawner : WNetworkBehaviour
{
    [SerializeField] protected Transform holder;
    [SerializeField] protected NetworkObject objectPre;

    protected override void LoadComponents()
    {
        this.LoadHolder();

    }
    protected virtual void LoadHolder()
    {
        if (this.holder != null) return;
        this.holder = transform.Find("Holder");
        Debug.Log(transform.name + ": LoadHolder", gameObject);
    }
    public virtual NetworkObject Spawn(Vector3 spawnPos, Quaternion rotation,Action<NetworkRunner, NetworkObject> onSpawnedCallback = null)
    {
        NetworkObject newPrefab = Runner.Spawn(objectPre, spawnPos, rotation, Object.InputAuthority);
        RpcSetParent(newPrefab, this.holder.GetComponent<NetworkObject>());
        onSpawnedCallback?.Invoke(Runner, newPrefab);
        return newPrefab;
    }
    public virtual void Despawn(NetworkObject obj)
    {
        Runner.Despawn(obj);
    }
    public virtual Transform GetHolder()
    {
        return this.holder;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSetParent(NetworkObject obj, NetworkObject parent)
    {
        obj.name = objectPre.name;
        obj.transform.SetParent(parent.transform, true);
    }

}
