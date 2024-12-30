using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
public class NetworkGun : WNetworkBehaviour
{
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected NetworkLookAtTarget networkLookAtTarget;
    [SerializeField] protected NetworkShooting networkShooting;

    public Transform FirePoint => firePoint;
    public NetworkLookAtTarget NetworkLookAtTarget => networkLookAtTarget;
    public NetworkShooting NetworkShooting => networkShooting;
    public NetworkObject currentTarget;
    public override void FixedUpdateNetwork()
    {
        currentTarget = NetworkMeteoriteSpawner.Instance.currentTarget;
        this.RPC_SetCurrentTarget(currentTarget);
        this.GetTargetToShoot(currentTarget);
        this.networkShooting.CheckKeyInput(currentTarget);
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.F1))
        {
            RPC_ToggleAutoTyping();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ToggleAutoTyping()
    {
        NetworkShooting.isAutoTyping = !NetworkShooting.isAutoTyping;

        if (NetworkShooting.isAutoTyping)
        {
            NetworkShooting.StartAutoTyping();
        }
        else
        {
            NetworkShooting.StopAutoTyping();
        }
    }
    [Rpc]
    public void RPC_SetCurrentTarget(NetworkObject target)
    {
        this.currentTarget = target;
        this.GetTargetToShoot(target);

    }
    private void GetTargetToShoot(NetworkObject targetObject)
    {
        if (targetObject == null) return;
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
