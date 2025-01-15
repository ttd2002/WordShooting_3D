using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberAbstract : MonoBehaviour
{
    [SerializeField] private CyberWeaponImpact cyberWeaponImpact;

    public void DealDamage()
    {
        this.cyberWeaponImpact.DealDamage();
    }
}
