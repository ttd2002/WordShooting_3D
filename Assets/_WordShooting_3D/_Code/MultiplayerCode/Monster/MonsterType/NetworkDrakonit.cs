using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDrakonit : NetworkMonster
{
    public override void GetHit()
    {
        this.StopAgent();
        monsterAnim.SetTrigger("getHit");
        this.ResumeAgent();
    }
    public override Vector3 GetHitPos()
    {
        return transform.position + new Vector3(0, 1.25f, 0);
    }
}
