using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAbstract : SingletonAbstract<UIAbstract>
{
    protected bool isOpen = false;

    protected override void Start()
    {
        base.Start();
        this.Close();
    }
    public virtual void Toggle()
    {
        this.isOpen = !this.isOpen;
        if (this.isOpen) this.Open();
        else this.Close();
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
        this.isOpen = true;
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
        this.isOpen = false;

    }
}
