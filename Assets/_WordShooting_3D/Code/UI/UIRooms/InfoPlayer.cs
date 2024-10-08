using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPlayer : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(PlayerNameChanged))]
    public string PlayerName { get; set; }
    void PlayerNameChanged()
    {
        UpdatePlayerNameUI(PlayerName);
    }
    [SerializeField] private TextMeshProUGUI playerNameLabel;

    void Start()
    {
        if (Object.HasStateAuthority)
        {
            PlayerName = NetworkManager.Instance._playerName;
        }
    }
    private void UpdatePlayerNameUI(string name)
    {
        // PlayerName = name;
        playerNameLabel.text = name;
    }
}
