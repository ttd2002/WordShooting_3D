using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopUpDespawn : DespawnByTime
{
    protected override void ResetValue()
    {
        this.delay = 1.5f;
    }
    protected override void DespawnObject()
    {
        ScorePopUpSpawner.Instance.Despawn(transform.parent);
    }
}
