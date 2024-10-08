using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{

    public List<PlayerInfoPre> playerList = new List<PlayerInfoPre>();
    private Dictionary<PlayerRef, bool> playerReadyStatus = new Dictionary<PlayerRef, bool>();
    public List<Transform> spawnedPlayerInfos = new List<Transform>();
    public void OnPlayerJoined(PlayerRef player)
    {
        playerList.Add(new PlayerInfoPre(player.PlayerId.ToString(), player));
        playerReadyStatus[player] = false; // Player is not ready by default
        if (NetworkManager.Instance.isHost)
        {
            Text btnLabel = UIManagerMainMenu.Instance.RoomDetail.transform.Find("VerticalGroup").Find("HorizontalGroup").Find("StartButton").Find("Background").GetComponentInChildren<Text>();
            btnLabel.text = "Start";
            Button btnStart = UIManagerMainMenu.Instance.RoomDetail.transform.Find("VerticalGroup").Find("HorizontalGroup").Find("StartButton").GetComponent<Button>();
            // btnStart.interactable = false;
        }
        else
        {
            BtnReady btnReady = UIManagerMainMenu.Instance.RoomDetail.transform.Find("VerticalGroup").Find("HorizontalGroup").Find("StartButton").GetComponent<BtnReady>();
            btnReady.playerRef = player;
        }

        UpdatePlayerInfoUI();
    }

    public void OnPlayerLeft(PlayerRef player)
    {
        Debug.Log("Player left: " + player.ToString());
        if (playerReadyStatus.ContainsKey(player))
        {
            playerReadyStatus.Remove(player);
        }
        playerList.RemoveAll(p => p.playerRef == player);
        UpdatePlayerInfoUI();
    }
    private void UpdatePlayerInfoUI()
    {
        Transform infoPlayer = InfoPlayerSpawner.Instance.Spawn(InfoPlayerSpawner.infoPlayer, Vector3.zero, Quaternion.identity);
        infoPlayer.gameObject.SetActive(true);
        infoPlayer.transform.localScale = new Vector3(1, 1, 1);

        spawnedPlayerInfos.Add(infoPlayer);
        int playerIndex = spawnedPlayerInfos.Count;

        Text PlayerId = infoPlayer.transform.Find("Background").Find("PlayerId").GetComponent<Text>();
        PlayerId.text = playerIndex + "P";

        Text playerName = infoPlayer.transform.Find("Background").Find("PlayerName").GetComponent<Text>();
        playerName.text = "Name: Player" + playerIndex;

        switch (playerIndex)
        {
            case 2:
                playerName.color = Color.green;
                break;
            case 3:
                playerName.color = Color.blue;
                break;
            case 4:
                playerName.color = Color.red;
                break;
            default:
                playerName.color = Color.white;
                break;
        }
    }
    // public void RefreshAllPlayerUI()
    // {
    //     for (int i = 0; i < spawnedPlayerInfos.Count; i++)
    //     {
    //         Transform infoPlayer = spawnedPlayerInfos[i];
    //         UpdatePlayerInfoUI(infoPlayer, i + 1, runner.LocalPlayer.PlayerId);
    //         PlayerRef player = playerList[i].playerRef;
    //         if (playerReadyStatus.ContainsKey(player))
    //         {
    //             Text readyStatus = infoPlayer.transform.Find("Background").Find("ReadyStatus").GetComponent<Text>();
    //             readyStatus.text = playerReadyStatus[player] ? "Ready" : "Not Ready";
    //         }
    //     }
    // }

    public void OnReadyButtonClicked(PlayerRef player)
    {
        if (playerReadyStatus.ContainsKey(player))
        {
            playerReadyStatus[player] = true;
        }
        else
        {
            playerReadyStatus.Add(player, true);
        }
        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        bool allReady = true;
        foreach (var status in playerReadyStatus.Values)
        {
            if (!status)
            {
                allReady = false;
                break;
            }
        }
        if (allReady && NetworkManager.Instance.isHost)
        {
            EnableStartButton();
        }
    }

    private void EnableStartButton()
    {
        Button btnStart = UIManagerMainMenu.Instance.RoomDetail.transform.Find("VerticalGroup/HorizontalGroup/StartButton").GetComponent<Button>();
        btnStart.interactable = true;
    }

    public void SpawnMultiplePlayers(NetworkRunner runner)
    {
        Vector3 startPos = new Vector3(0, -1, -13);
        foreach (PlayerInfoPre playerInfo in playerList)
        {
            NetworkObject playerObject = runner.Spawn(NetworkManager.Instance.PlayerPrefab, startPos, Quaternion.identity, playerInfo.playerRef);
            TextMeshProUGUI playerNameText = playerObject.transform.Find("Model/Canvas").GetComponentInChildren<TextMeshProUGUI>();
            playerNameText.text = playerInfo.playerName;
            startPos += new Vector3(2, 0, 0);  
        }
    }

}

[System.Serializable]
public class PlayerInfoPre
{
    public string playerName;
    public PlayerRef playerRef;

    public PlayerInfoPre(string name, PlayerRef playerRef)
    {
        playerName = name;
        this.playerRef = playerRef;
    }
}
