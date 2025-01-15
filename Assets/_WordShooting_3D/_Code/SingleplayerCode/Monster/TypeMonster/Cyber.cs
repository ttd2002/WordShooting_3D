using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cyber : Monster
{
    public override Vector3 GetHitPos()
    {
        return transform.position + new Vector3(0.2f, 1.25f, 0);
    }
}
