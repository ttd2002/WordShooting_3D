using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : SingletonAbstract<NetworkManager>, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkObject playerPrefab;
    public NetworkObject PlayerPrefab => playerPrefab;
    public static NetworkRunner runner;
    public string _playerName;
    public bool isHost = false;
    [SerializeField] private SessionManager sessionManager;
    [SerializeField] private PlayerManager playerManager;
    public List<SessionInfo> _session = new List<SessionInfo>();
    private int roomCount = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        sessionManager = GetComponent<SessionManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void ConnectToLobby()
    {
        _playerName = FirebaseManager.Instance.userId;
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }
        runner.JoinSessionLobby(SessionLobby.Shared, "WS_3D");
    }

    public async void ConnectToSession(string sessionName)
    {
        this.isHost = false;
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = sessionName,
            PlayerCount = 4,
        });
        UIManagerMainMenu.Instance.SetTitleRoomDetail(sessionName);
    }

    public async void CreateSession()
    {
        this.isHost = true;

        Text inputPwd = UIManagerMainMenu.Instance.Rooms.transform.Find("TextfieldPass").Find("InputFieldPwd").Find("TextPwd").GetComponent<Text>();
        SessionProperty key = inputPwd.text;
        Dictionary<string, SessionProperty> sessionProperty = new Dictionary<string, SessionProperty>();
        sessionProperty.Add("sessionKey", key);
        // int RandomInt = UnityEngine.Random.Range(1, 10);
        string RandomSessionName = "Room#" + this.roomCount++;
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = RandomSessionName,
            SessionProperties = sessionProperty,
            PlayerCount = 4,
        });
        UIManagerMainMenu.Instance.SetTitleRoomDetail(RandomSessionName);
    }

    public async void LeaveRoom()
    {
        if (runner != null)
        {
            await runner.Shutdown(true, ShutdownReason.Ok);
            ConnectToLobby();
            Debug.Log("Player has left the room and returned to the lobby.");
        }
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        _session.Clear();
        _session = sessionList;
        this.sessionManager.RefreshSession(_session);
        Debug.Log("Session List Updated: sessionList count " + _session.Count);
        this.roomCount = _session.Count;

    }

    public void OnStartButtonClicked()
    {
        if (isHost)
        {
            runner.LoadScene(ScenesManager.SceneOfMyGame.GamePlayMulti.ToString());
            Debug.Log("---------Start game----------");
        }
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            SpawnObjects(runner);
        }
    }

    private void SpawnObjects(NetworkRunner runner)
    {
        playerManager.SpawnMultiplePlayers(runner);
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        playerManager.OnPlayerJoined(player);
        Debug.Log("Player joined");
        UIManagerMainMenu.Instance.OpenRoomDetail();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        playerManager.OnPlayerLeft(player);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        if (Input.anyKeyDown && !string.IsNullOrEmpty(Input.inputString))
        {
            data.typedChar = Input.inputString.ToLower()[0];
            data.buttons.Set(NetworkInputData.KEY_TYPED, true);
        }
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    // Implement the other INetworkRunnerCallbacks methods here...

}
