using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] Transform targetFollow;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] private TextMeshProUGUI namePlayerText;
    [Networked]
    [OnChangedRender(nameof(OnNameChanged))]
    public NetworkString<_16> playerName { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        namePlayerText.text = playerName.ToString();
        if (HasInputAuthority)
        {
            this.virtualCamera = Transform.FindAnyObjectByType<CinemachineVirtualCamera>();
            this.virtualCamera.Follow = this.targetFollow;
            RPC_SetPlayerName(ConnectDatabase.Instance.userName);
        }
        Runner.SetPlayerObject(Object.InputAuthority, Object);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerName(string name)
    {
        playerName = name;
    }
    private void OnNameChanged()
    {
        Debug.Log("Updating name to: " + playerName);
        namePlayerText.text = playerName.ToString();
    }


}
