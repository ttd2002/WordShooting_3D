using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeteoriteDepsawn : Despawn
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
        MeteoriteFly meteoriteFly = transform.parent.GetComponentInChildren<MeteoriteFly>();
        Vector3 despawnPos = meteoriteFly.Target.position;
        if(transform.parent.position == despawnPos || checkCanDespawn) return true;
        return false;
    }
    protected override void DespawnObject()
    {
        Transform reticle = transform.parent.Find("Canvas/Reticle");
        reticle.gameObject.SetActive(false);
        MeteoriteSpawner.Instance.Despawn(transform.parent);
        GunCtrl.Instance.Shooting.ResetTarget();   
    }
    public void SetCanDespawnIsTrue(){
        checkCanDespawn = true;
    }
    
}
