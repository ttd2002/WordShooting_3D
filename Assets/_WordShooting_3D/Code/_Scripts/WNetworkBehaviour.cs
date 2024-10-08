using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class WNetworkBehaviour : NetworkBehaviour
{
    protected virtual void Reset()
    {
        this.LoadComponents();
        this.ResetValue();

    }
    protected virtual void Awake()
    {
        this.LoadComponents();
    }
    protected virtual void LoadComponents()
    {
        //for override
    }
    protected virtual void ResetValue()
    {
        //for override
    }
    protected virtual void Start()
    {
        //for override
    }
}
