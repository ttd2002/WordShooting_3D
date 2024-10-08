using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnOK : ButtonBase
{
    protected override void OnClick()
    {
        Time.timeScale = 1f;
        ScenesManager.Instance.BackToMenuFromGameplay();
    }

}
