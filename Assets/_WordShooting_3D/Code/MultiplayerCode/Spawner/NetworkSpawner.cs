using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkSpawner : WNetworkBehaviour
{
    [SerializeField] protected Transform holder;
    [SerializeField] protected List<NetworkObject> prefabs;
    [SerializeField] protected List<NetworkObject> poolObjs;
    [SerializeField] private NetworkObject objectPre;

    protected override void LoadComponents()
    {
        this.LoadPrefabs();
        this.LoadHolder();

    }
    protected virtual void LoadHolder()
    {
        if (this.holder != null) return;
        this.holder = transform.Find("Holder");
        Debug.Log(transform.name + ": LoadHolder", gameObject);
    }

    protected virtual void LoadPrefabs()
    {
        if (this.prefabs.Count > 0) return;
        Transform prefabObj = transform.Find("Prefabs");
        foreach (Transform prefab in prefabObj)
        {
            NetworkObject networkObj = prefab.GetComponent<NetworkObject>();
            this.prefabs.Add(networkObj);
        }
        this.HidePrefabs();

        Debug.Log(transform.name + ": LoadPrefab", gameObject);
    }
    protected virtual void HidePrefabs()
    {
        foreach (NetworkObject prefab in this.prefabs)
        {
            prefab.gameObject.SetActive(false);
        }
    }
    public virtual NetworkObject Spawn(string prefabName, Vector3 spawnPos, Quaternion rotation)
    {
        NetworkObject prefab = this.GetPrefabByName(prefabName);
        if (prefab == null) return null;
        NetworkObject newPrefab = this.GetObjectFromPool(prefab, spawnPos, rotation);
        newPrefab.transform.position = spawnPos;
        newPrefab.transform.rotation = rotation;
        RpcSetParent(newPrefab, this.holder.GetComponent<NetworkObject>());

        return newPrefab;
    }
    protected virtual NetworkObject GetObjectFromPool(NetworkObject prefab, Vector3 spawnPos, Quaternion rotation)
    {
        foreach (NetworkObject poolObj in poolObjs)
        {
            if (poolObj.name == prefab.name)
            {
                poolObjs.Remove(poolObj);
                return poolObj;
            }
        }
        NetworkObject newPrefab = Runner.Spawn(objectPre, spawnPos, rotation, Object.InputAuthority);
        newPrefab.name = prefab.name;
        return newPrefab;
    }
    public virtual NetworkObject GetPrefabByName(string prefabName)
    {
        foreach (NetworkObject prefab in this.prefabs)
        {
            if (prefab.name == prefabName) return prefab;
        }
        return null;
    }

    public virtual void Despawn(NetworkObject obj)
    {
        // this.poolObjs.Add(obj);
        // obj.gameObject.SetActive(false);
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
