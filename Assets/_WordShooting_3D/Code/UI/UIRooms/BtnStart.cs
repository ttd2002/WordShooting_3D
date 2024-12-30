using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BtnStart : ButtonBase
{
    protected override void OnClick()
    {
        LobbiesManager.Instance.StartGame();
    }
}
