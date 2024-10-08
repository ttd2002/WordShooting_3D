using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnPlaySingleMode : ButtonBase
{
    protected override void OnClick()
    {
        Time.timeScale = 1;
        ScenesManager.Instance.LoadNewGame();
    }
}
