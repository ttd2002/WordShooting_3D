using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplayManager : SingletonAbstract<UIGameplayManager>
{
    [SerializeField] private GameObject loadingUI;
    public GameObject LoadingUI => loadingUI;
    [SerializeField] public NetworkScore networkScore;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadLoadingUI();
    }
    protected virtual void LoadLoadingUI()
    {
        if (this.loadingUI != null) return;
        this.loadingUI = transform.parent.Find("UIMiddle").Find("LoadingCircle").gameObject;
        Debug.Log(transform.name + ": LoadLoadingUI", gameObject);
    }

}
