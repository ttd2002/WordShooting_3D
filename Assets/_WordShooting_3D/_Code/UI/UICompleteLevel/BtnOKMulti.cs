using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnOKMulti : ButtonBase
{
    protected override void OnClick()
    {
        ScenesManager.Instance.BackToMenuFromMultiGameplay();
    }
}
