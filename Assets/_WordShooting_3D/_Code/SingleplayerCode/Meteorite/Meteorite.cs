using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : WMonoBehaviour
{
    public bool isTarger;
    [SerializeField] private MeteoriteReticle meteoriteReticle;
    public MeteoriteReticle MeteoriteReticle => meteoriteReticle;
    protected override void Awake()
    {
        base.Awake();
        this.isTarger = false;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        this.isTarger = false;
    }
    void Update()
    {
        if (this.isTarger)
        {
            this.meteoriteReticle.Open();
        }
        else
        {
            this.meteoriteReticle.Close();

        }
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadMeteoriteReticle();

    }
    protected virtual void LoadMeteoriteReticle()
    {
        if (this.meteoriteReticle != null) return;
        this.meteoriteReticle = transform.Find("Canvas").Find("Reticle").GetComponent<MeteoriteReticle>();
        Debug.Log(transform.name + ": LoadMeteoriteReticle", gameObject);
    }
}
