using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrakonitAbstract : MonoBehaviour
{
    [SerializeField] private DrakonitRHandImpact drakonitRHandImpact;
    [SerializeField] private DrakonitLHandImpact drakonitLHandImpact;

    public void LHand()
    {
        this.drakonitLHandImpact.DealDamage();
    }
    public void RHand()
    {
        this.drakonitRHandImpact.DealDamage();
    }
}
