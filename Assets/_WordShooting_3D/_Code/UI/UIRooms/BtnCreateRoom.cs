using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnCreateRoom : ButtonBase
{
    protected override void OnClick()
    {
        LobbiesManager.Instance.CreateLobby();
    }
}
