using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParentFly : WMonoBehaviour
{
    [SerializeField] protected Transform target;
    public Transform Target => target;
    [SerializeField] protected float moveSpeed = 10f;
    private void Update()
    {
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, this.target.position, this.moveSpeed * Time.deltaTime);
    }
    public virtual void SetNewTarget(Transform newTarget)
    {
        this.target = newTarget;

    }
}
