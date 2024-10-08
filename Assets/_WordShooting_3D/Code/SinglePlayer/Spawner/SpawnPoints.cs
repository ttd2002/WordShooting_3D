using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnPoints : WMonoBehaviour
{
    [SerializeField] protected List<Transform> points;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPoints();
    }

    protected virtual void LoadPoints()
    {
        if (this.points.Count > 0) return;
        foreach (Transform point in transform)
        {
            this.points.Add(point);
        }
        Debug.Log(transform.name + ": LoadPoints", gameObject);
    }
    public virtual Transform GetRandom()
    {
        int ran = Random.Range(0, this.points.Count);
        return this.points[ran];
    }
}
