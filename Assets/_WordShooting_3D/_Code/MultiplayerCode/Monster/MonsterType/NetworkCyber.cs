using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCyber : NetworkMonster
{

    public override Vector3 GetHitPos()
    {
        return transform.position + new Vector3(0.2f, 1.25f, 0);
    }
}
