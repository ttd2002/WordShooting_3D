using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkMeteoriteDepsawn : Despawn
{
    [SerializeField] protected bool checkCanDespawn;
    protected override void OnEnable()
    {
        base.OnEnable();
        this.ResetCheck();
    }
    protected virtual void ResetCheck()
    {
        checkCanDespawn = false;
    }
    protected override bool CanDespawn()
    {
        NetworkMeteoriteFly meteoriteFly = transform.parent.GetComponentInChildren<NetworkMeteoriteFly>();
        Vector3 despawnPos = meteoriteFly.TargetTrans.position;
        if (transform.parent.position == despawnPos || checkCanDespawn) return true;
        return false;
    }
    protected override void DespawnObject()
    {
        NetworkMeteoriteSpawner.Instance.OnTextCompleted();
        ShootingManager.ResetAllTargets();
    }
    public void SetCanDespawnIsTrue()
    {
        checkCanDespawn = true;
    }
}
