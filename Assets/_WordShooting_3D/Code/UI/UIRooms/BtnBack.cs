using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnBack : ButtonBase
{
    protected override void OnClick()
    {
        LobbiesManager.Instance.LeaveLobby();
    }
}
