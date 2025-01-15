using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAbstract : MonoBehaviour
{
    [SerializeField] private DemonHandImpact demonHandImpact;
    [SerializeField] private DemonWeaponImpact demonWeaponImpact;

    public void DealDamage()
    {
        this.demonHandImpact.DealDamage();
    }
    public void Purch2()
    {
        this.demonWeaponImpact.Purch2();
    }
    public void Purch3()
    {
        this.demonWeaponImpact.Purch3();
    }
}
