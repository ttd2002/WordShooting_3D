using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesManager : SingletonAbstract<LobbiesManager>
{
    [Header("Lobby list")]
    [SerializeField] private GameObject lobbyListParent;
    [SerializeField] private GameObject joinedLobbyStartButton;
    [SerializeField] private Transform lobbyContentParent;
    [SerializeField] private Transform lobbyItemPrefab;

    [Header("Joined lobby")]
    [SerializeField] private GameObject joinedLobbyParent;
    [SerializeField] private Transform playerItemPrefab;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private Text joinedLobbyNameText;

    [Header("Password protection")]
    [SerializeField] private GameObject inputPasswordParent;
    [SerializeField] private Button inputPasswordButton;
    [SerializeField] private Text createPassword;
    [SerializeField] private Text joinPassword;
    [SerializeField] private TextMeshProUGUI errorText;

    public bool isLeader = false;
    private Player playerData;
    private string playerName;
    private string joinedLobbyId;
    private string playerId;
    private CancellationTokenSource lobbyCancellationTokenSource;
    private CancellationTokenSource heartbeatCancellationTokenSource;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    protected override async void Start()
    {
        var options = new InitializationOptions();
        options.SetProfile("Player" + DateTime.UtcNow.Ticks);
        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () =>
        {
            playerId = AuthenticationService.Instance.PlayerId;
        };
    }

    public async void CreateLobby()
    {
        CreateProfile();
        this.isLeader = true;
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            IsPrivate = false,
            Player = playerData,
            Data = new Dictionary<string, DataObject> { { "StartGame", new DataObject(DataObject.VisibilityOptions.Public, "false") } }
        };

        if (!string.IsNullOrEmpty(createPassword.text))
        {
            options.Password = createPassword.text;
        }

        int randomInt = UnityEngine.Random.Range(1, 10);
        string randomLobbyName = "Room#" + randomInt;

        try
        {
            Lobby createdLobby = await LobbyService.Instance.CreateLobbyAsync(randomLobbyName, 4, options);
            lobbyListParent.SetActive(false);
            joinedLobbyParent.SetActive(true);
            joinedLobbyId = createdLobby.Id;
            UpdateLobbyInfo();

            heartbeatCancellationTokenSource = new CancellationTokenSource();
            await LobbyHeartbeat(createdLobby, heartbeatCancellationTokenSource.Token);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Create Lobby Error: {e}");
        }
    }

    public async void JoinLobby(string lobbyID, bool needPassword)
    {
        CreateProfile();
        var joinOptions = new JoinLobbyByIdOptions { Player = playerData };

        if (needPassword)
        {
            joinOptions.Password = await InputPassword();
        }

        try
        {
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, joinOptions);
            joinedLobbyId = joinedLobby.Id;
            lobbyListParent.SetActive(false);
            joinedLobbyParent.SetActive(true);
            UpdateLobbyInfo();

            heartbeatCancellationTokenSource = new CancellationTokenSource();
            await LobbyHeartbeat(joinedLobby, heartbeatCancellationTokenSource.Token);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Join Lobby Error: {e}");
        }
    }

    public async void LeaveLobby()
    {
        if (string.IsNullOrEmpty(joinedLobbyId))
        {
            Debug.LogWarning("You are not currently in a lobby.");
            return;
        }

        try
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(joinedLobbyId, playerId);
            Debug.Log("Successfully left the lobby: " + joinedLobbyId);

            joinedLobbyId = string.Empty;
            lobbyCancellationTokenSource?.Cancel();
            heartbeatCancellationTokenSource?.Cancel();
            ExitLobbyCreationButton();
        }
        catch (Exception e)
        {
            Debug.LogError("Error leaving lobby: " + e.Message);
        }
    }

    private async Task<string> InputPassword()
    {
        bool waiting = true;
        inputPasswordParent.SetActive(true);

        inputPasswordButton.onClick.AddListener(() => waiting = false);

        while (waiting)
        {
            await Task.Yield();
        }

        inputPasswordParent.SetActive(false);
        return joinPassword.text;
    }

    public void ExitLobbyCreationButton()
    {
        joinedLobbyParent.SetActive(false);
        lobbyListParent.SetActive(true);
        ShowLobbies();
    }

    public async void ShowLobbies()
    {
        lobbyCancellationTokenSource?.Cancel();
        lobbyCancellationTokenSource = new CancellationTokenSource();

        while (Application.isPlaying && lobbyListParent.activeInHierarchy && !lobbyCancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                foreach (Transform t in lobbyContentParent)
                {
                    Destroy(t.gameObject);
                }

                foreach (Lobby lobby in queryResponse.Results)
                {
                    Transform newLobbyItem = Instantiate(lobbyItemPrefab, lobbyContentParent);
                    newLobbyItem.GetComponent<JoinLobbyButton>().lobbyId = lobby.Id;
                    newLobbyItem.GetComponent<JoinLobbyButton>().needPassword = lobby.HasPassword;
                    newLobbyItem.Find("Background").GetChild(0).GetComponent<Text>().text = lobby.Name;
                    newLobbyItem.Find("Background").GetChild(1).GetComponent<Text>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;

                    newLobbyItem.Find("Background").GetChild(2).gameObject.SetActive(lobby.HasPassword);
                }

                await Task.Delay(2000, lobbyCancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Debug.LogError("Error showing lobbies: " + e.Message);
            }
        }
    }
    public async Task ResetRoom()
    {
        var updates = new Dictionary<string, DataObject> { { "StartGame", new DataObject(DataObject.VisibilityOptions.Public, "false") }, { "GameReady", new DataObject(DataObject.VisibilityOptions.Public, "false") } };
        await Lobbies.Instance.UpdateLobbyAsync(this.joinedLobbyId, new UpdateLobbyOptions { Data = updates });
    }
    public async void UpdateLobbyInfo()
    {
        lobbyCancellationTokenSource?.Cancel();
        lobbyCancellationTokenSource = new CancellationTokenSource();

        while (Application.isPlaying && !string.IsNullOrEmpty(joinedLobbyId) && !lobbyCancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId);

                if (lobby == null || lobby.Data["StartGame"].Value == "true")
                {
                    if (AuthenticationService.Instance.PlayerId == lobby.HostId)
                    {
                        errorText.gameObject.SetActive(true);
                        errorText.text = "Starting game...";
                        FusionNetworkManager.Instance.Initialize(lobby);
                        await Task.Delay(1000);
                        var updates = new Dictionary<string, DataObject> { { "GameReady", new DataObject(DataObject.VisibilityOptions.Public, "true") } };
                        await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions { Data = updates });
                        return;
                    }
                    else if (lobby.Data["GameReady"].Value == "true")
                    {
                        errorText.gameObject.SetActive(true);
                        errorText.text = "Starting game...";
                        FusionNetworkManager.Instance.Initialize(lobby);
                        return;
                    }
                }

                joinedLobbyStartButton.SetActive(AuthenticationService.Instance.PlayerId == lobby.HostId);
                joinedLobbyNameText.text = lobby.Name;

                foreach (Transform t in playerListParent)
                {
                    Destroy(t.gameObject);
                }

                for (int i = 0; i < lobby.Players.Count; i++)
                {
                    Player player = lobby.Players[i];
                    Transform newPlayerItem = Instantiate(playerItemPrefab, playerListParent);
                    newPlayerItem.Find("Background").GetChild(0).GetComponent<Text>().text = (i + 1) + "P";
                    newPlayerItem.Find("Background").GetChild(1).GetComponent<Text>().text = "Name: " + player.Data["Name"].Value;
                    newPlayerItem.Find("Background").GetChild(2).gameObject.SetActive(lobby.HostId == player.Id);
                }

                await Task.Delay(2000, lobbyCancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Debug.LogError("Error updating lobby info: " + e.Message);
            }
        }
    }

    private async Task LobbyHeartbeat(Lobby lobby, CancellationToken token)
    {
        while (!token.IsCancellationRequested && !string.IsNullOrEmpty(joinedLobbyId))
        {
            try
            {
                if (playerId == lobby.HostId)
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
                }
            }
            catch (LobbyServiceException e)
            {

                Debug.LogError("Error sending heartbeat: " + e.Message);
                break;
            }
            await Task.Delay(15000, token);
        }
    }
    public void CreateProfile()
    {
        playerName = ConnectDatabase.Instance.userName ?? "Player_" + UnityEngine.Random.Range(1000, 9999);
        playerId = AuthenticationService.Instance.PlayerId;
        PlayerDataObject playerDataObjectName = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        playerData = new Player(id: playerId, data: new Dictionary<string, PlayerDataObject> { { "Name", playerDataObjectName } });
    }


    public async void StartGame()
    {
        if (string.IsNullOrEmpty(joinedLobbyId))
        {
            Debug.LogError("Not in a lobby");
            return;
        }

        try
        {
            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId);

            if (AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                var updates = new Dictionary<string, DataObject> { { "StartGame", new DataObject(DataObject.VisibilityOptions.Public, "true") } };
                await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions { Data = updates });
            }
        }
        catch (LobbyServiceException e)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Start fail...";
            Debug.LogError("Error starting game: " + e.Message);
        }
    }

    public void ReloadComponent()
    {
        this.LoadLobbyListParent();
        this.LoadJoinedLobbyStartButton();
        this.LoadLobbyContentParent();
        this.LoadJoinedLobbyParent();
        this.LoadPlayerListParent();
        this.LoadJoinedLobbyNameText();
        this.LoadInputPasswordParent();
        this.LoadInputPasswordButton();
        this.LoadJoinPassword();
        this.LoadCreatePassword();
        this.LoadErrorText();
    }
    protected virtual void LoadLobbyListParent()
    {
        if (this.lobbyListParent != null) return;
        this.lobbyListParent = Transform.FindFirstObjectByType<Canvas>().transform.Find("Rooms").gameObject;
        Debug.Log(transform.name + ": LoadLobbyListParent", gameObject);
    }
    protected virtual void LoadJoinedLobbyStartButton()
    {
        if (this.joinedLobbyStartButton != null) return;
        this.joinedLobbyStartButton = Transform.FindFirstObjectByType<Canvas>().transform.Find("RoomDetail/VerticalGroup/HorizontalGroup/StartButton").gameObject;
        Debug.Log(transform.name + ": LoadJoinedLobbyStartButton", gameObject);
    }
    protected virtual void LoadLobbyContentParent()
    {
        if (this.lobbyContentParent != null) return;
        this.lobbyContentParent = Transform.FindFirstObjectByType<Canvas>().transform.Find("Rooms/Vertical Group/Grid/Scroll View/Viewport/Content").transform;
        Debug.Log(transform.name + ": LoadLobbyContentParent", gameObject);
    }
    protected virtual void LoadJoinedLobbyParent()
    {
        if (this.joinedLobbyParent != null) return;
        this.joinedLobbyParent = Transform.FindFirstObjectByType<Canvas>().transform.Find("RoomDetail").gameObject;
        Debug.Log(transform.name + ": LoadJoinedLobbyParent", gameObject);
    }
    protected virtual void LoadPlayerListParent()
    {
        if (this.playerListParent != null) return;
        this.playerListParent = Transform.FindFirstObjectByType<Canvas>().transform.Find("RoomDetail/VerticalGroup/Grid/Scroll View/Viewport/Content").transform;
        Debug.Log(transform.name + ": LoadPlayerListParent", gameObject);
    }
    protected virtual void LoadJoinedLobbyNameText()
    {
        if (this.joinedLobbyNameText != null) return;
        this.joinedLobbyNameText = Transform.FindFirstObjectByType<Canvas>().transform.Find("RoomDetail/RoomDetailTitle/TitleLabel").transform.GetComponent<Text>();
        Debug.Log(transform.name + ": LoadJoinedLobbyNameText", gameObject);
    }

    protected virtual void LoadInputPasswordParent()
    {
        if (this.inputPasswordParent != null) return;
        this.inputPasswordParent = Transform.FindFirstObjectByType<Canvas>().transform.Find("Rooms/TextfieldPassJoin").gameObject;
        Debug.Log(transform.name + ": LoadInputPasswordParent", gameObject);
    }
    protected virtual void LoadInputPasswordButton()
    {
        if (this.inputPasswordButton != null) return;
        this.inputPasswordButton = Transform.FindFirstObjectByType<Canvas>().transform.Find("Rooms/TextfieldPassJoin/Button").transform.GetComponent<Button>();
        Debug.Log(transform.name + ": LoadInputPasswordButton", gameObject);
    }
    protected virtual void LoadCreatePassword()
    {
        if (this.createPassword != null) return;
        this.createPassword = Transform.FindFirstObjectByType<Canvas>().transform.Find("Rooms/TextfieldPass/InputFieldPwd/TextPwd").transform.GetComponent<Text>();
        Debug.Log(transform.name + ": LoadCreatePassword", gameObject);
    }
    protected virtual void LoadJoinPassword()
    {
        if (this.joinPassword != null) return;
        this.joinPassword = Transform.FindFirstObjectByType<Canvas>().transform.Find("Rooms/TextfieldPassJoin/InputFieldPwd/TextPwdJoin").transform.GetComponent<Text>();
        Debug.Log(transform.name + ": LoadJoinPassword", gameObject);
    }
    protected virtual void LoadErrorText()
    {
        if (this.errorText != null) return;
        this.errorText = Transform.FindFirstObjectByType<Canvas>().transform.Find("RoomDetail/infoText").transform.GetComponent<TextMeshProUGUI>();
        Debug.Log(transform.name + ": LoadErrorText", gameObject);
    }


}
