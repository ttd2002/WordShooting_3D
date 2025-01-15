using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnRefresh : ButtonBase
{
    protected override void OnClick()
    {
        LobbiesManager.Instance.ShowLobbies();
    }
}
