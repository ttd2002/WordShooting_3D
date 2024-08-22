using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBase : WMonoBehaviour
{
    [Header("Base text")]
    [SerializeField] protected TextMeshProUGUI text;

    protected virtual void Update()
    {
        //for overide
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadText();
    }
    protected virtual void LoadText()
    {
        if (this.text != null) return;
        this.text = GetComponent<TextMeshProUGUI>();
        Debug.Log(transform.name + ": LoadText", gameObject);
    }
    
}
