using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
public class NetworkGun : WNetworkBehaviour
{
    [SerializeField] protected Transform firePoint;
    public Transform FirePoint => firePoint;
    [SerializeField] protected NetworkLookAtTarget networkLookAtTarget;
    public NetworkLookAtTarget NetworkLookAtTarget => networkLookAtTarget;
    [SerializeField] protected NetworkShooting networkShooting;
    public NetworkShooting NetworkShooting => networkShooting;
    public NetworkObject currentTarget;
    public override void FixedUpdateNetwork()
    {
        NetworkObject target = NetworkMeteoriteSpawner.Instance.GetFirstActiveObject();
        this.RPC_SetCurrentTarget(target);
        this.networkShooting.CheckKeyInput(currentTarget);
      
    }
    [Rpc]
    public void RPC_SetCurrentTarget(NetworkObject target)
    {
        // Cập nhật currentTarget cho tất cả các client
        this.currentTarget = target;
        this.GetTargetToShoot(target);

    }
    private void GetTargetToShoot(NetworkObject targetObject)
    {
        if (targetObject == null) return;
        Transform reticle = targetObject.transform.Find("Canvas/Reticle");
        reticle.gameObject.SetActive(true);
        this.networkLookAtTarget.LookTarget(targetObject.transform);
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadFirePoint();
        this.LoadLookAtTarget();
        this.LoadNetworkShooting();
    }

    protected virtual void LoadFirePoint()
    {
        if (this.firePoint != null) return;
        this.firePoint = transform.Find("Model").Find("TopSide").Find("FirePoint");
        Debug.Log(transform.name + ": LoadFirePoint", gameObject);
    }
    protected virtual void LoadLookAtTarget()
    {
        if (this.networkLookAtTarget != null) return;
        this.networkLookAtTarget = transform.GetComponentInChildren<NetworkLookAtTarget>();
        Debug.Log(transform.name + ": LoadLookAtTarget", gameObject);
    }
    protected virtual void LoadNetworkShooting()
    {
        if (this.networkShooting != null) return;
        this.networkShooting = transform.GetComponentInChildren<NetworkShooting>();
        Debug.Log(transform.name + ": LoadNetworkShooting", gameObject);
    }
}
