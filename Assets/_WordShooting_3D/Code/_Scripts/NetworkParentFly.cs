using System.Collections;
using UnityEngine;
using Fusion;

public abstract class NetworkParentFly : WNetworkBehaviour
{
    protected Transform targetTrans { get; set; }
    public Transform TargetTrans => targetTrans;
    [SerializeField] protected float moveSpeed = 10f;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetTargetPosition(Vector3 targetPosition)
    {
        if (targetTrans != null)
        {
            targetTrans.position = targetPosition;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // if (Runner.IsServer)
        // {
        if (targetTrans == null)
        {
            Runner.Despawn(transform.parent.GetComponent<NetworkObject>());
            return;
        }
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, this.targetTrans.position, this.moveSpeed * Runner.DeltaTime);

        // }
    }
    public virtual void SetNewTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            targetTrans = newTarget;
            if (Object.HasStateAuthority)
            {
                RPC_SetTargetPosition(newTarget.position);
            }
        }
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTargetTrans();
    }
    protected virtual void LoadTargetTrans()
    {
        if (this.targetTrans != null) return;
        this.targetTrans = FindAnyObjectByType<MeteoriteDespawnPoints>().GetRandom();
        Debug.Log(transform.name + ": LoadTargetTrans", gameObject);
    }

}
