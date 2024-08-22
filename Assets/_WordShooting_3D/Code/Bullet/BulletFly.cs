using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : ParentFly
{
    [SerializeField] protected float arcRange = 2.5f;

    protected override void Start()
    {
        iTween.PunchPosition(transform.parent.gameObject, new Vector3(Random.Range(-arcRange, arcRange), Random.Range(-arcRange, arcRange), 0), Random.Range(0.5f, 2));
    }
}
