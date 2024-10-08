using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParentFly : WMonoBehaviour
{
    [SerializeField] protected Transform targetTrans;
    public Transform TargetTrans => targetTrans;
    [SerializeField] protected float moveSpeed = 7f;
    protected virtual void Update()
    {
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, this.targetTrans.position, this.moveSpeed * Time.deltaTime);
    }
    public virtual void SetNewTarget(Transform newTarget)
    {
        this.targetTrans = newTarget;

    }
}
