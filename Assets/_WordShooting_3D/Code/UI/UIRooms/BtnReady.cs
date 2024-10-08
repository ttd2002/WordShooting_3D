using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BtnReady : ButtonBase
{
    public PlayerRef playerRef;
    protected override void OnClick()
    {
        if (transform.Find("Background").GetComponentInChildren<Text>().text == "Ready")
        {
            // NetworkManager.Instance.OnReadyButtonClicked(playerRef);
        }
        else
        {
            NetworkManager.Instance.OnStartButtonClicked();

        }
    }
}
