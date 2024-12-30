using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinLobbyButton : ButtonBase
{
    public bool needPassword;
    public string lobbyId;
    protected override void OnClick()
    {
        this.JoinLobbyButtonPressed();
    }
    public void JoinLobbyButtonPressed()
    {
        LobbiesManager.Instance.JoinLobby(lobbyId, needPassword);
    }
}