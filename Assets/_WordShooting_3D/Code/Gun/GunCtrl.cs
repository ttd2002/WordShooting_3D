using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunCtrl : SingletonAbstract<GunCtrl>
{
    [SerializeField] protected Transform firePoint;
    public Transform FirePoint => firePoint;
    [SerializeField] protected LookAtTarget lookAtTarget;
    public LookAtTarget LookAtTarget => lookAtTarget;

    [SerializeField] protected Shooting shooting;
    public Shooting Shooting => shooting;


    void Update()
    {
        this.GetTargetToShoot();
        this.ShootingToTarget();
        if (Input.GetKeyDown(KeyCode.Tab)) this.LookAtTarget.SwitchToNextTarget();
    }

    private void GetTargetToShoot()
    {
        this.lookAtTarget.FillTargetObjects();
        this.lookAtTarget.RemoveTargetObject();
        Transform targetObject = this.GetTarget();
        if (targetObject == null) return;
        Transform reticle = targetObject.Find("Canvas/Reticle");
        reticle.gameObject.SetActive(true);
        this.lookAtTarget.LookTarget(targetObject);
    }
    private void ShootingToTarget()
    {
        this.shooting.CheckKeyInput(this.GetTarget());
    }
    private Transform GetTarget()
    {
        return this.lookAtTarget.GetFirstTargetObject();
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadFirePoint();
        this.LoadLookAtTarget();
        this.LoadShooting();
    }

    protected virtual void LoadFirePoint()
    {
        if (this.firePoint != null) return;
        this.firePoint = transform.Find("Model").Find("TopSide").Find("FirePoint");
        Debug.Log(transform.name + ": LoadFirePoint", gameObject);
    }
    protected virtual void LoadLookAtTarget()
    {
        if (this.lookAtTarget != null) return;
        this.lookAtTarget = transform.GetComponentInChildren<LookAtTarget>();
        Debug.Log(transform.name + ": LoadLookAtTarget", gameObject);
    }
    protected virtual void LoadShooting()
    {
        if (this.shooting != null) return;
        this.shooting = transform.GetComponentInChildren<Shooting>();
        Debug.Log(transform.name + ": LoadShooting", gameObject);
    }
}
