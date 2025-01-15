using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : WMonoBehaviour
{
    public void Open(){
        gameObject.SetActive(true);
    }
    public void Close(){
        gameObject.SetActive(false);
    }
}
