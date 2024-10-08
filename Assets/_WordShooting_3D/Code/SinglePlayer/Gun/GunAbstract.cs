using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunAbstract : WMonoBehaviour
{
    [SerializeField] protected GunCtrl gunCtrl;
    public GunCtrl GunCtrl { get => gunCtrl; }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGunCtrl();
    }
    protected virtual void LoadGunCtrl()
    {
        if (this.gunCtrl != null) return;
        this.gunCtrl = transform.parent.GetComponent<GunCtrl>();
        Debug.Log(transform.name + ": LoadGunCtrl", gameObject);
    }
}
