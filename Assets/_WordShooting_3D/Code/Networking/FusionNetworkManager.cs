using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class FusionNetworkManager : SingletonAbstract<FusionNetworkManager>, INetworkRunnerCallbacks
{

    public NetworkRunner networkRunner;
    public NetworkObject playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    public void ConnectToLobby()
    {
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
        }
        networkRunner.JoinSessionLobby(SessionLobby.Shared, "WS_3D");
    }
    public void ReturnToLobby()
    {
        StartCoroutine(ReturnToLobbyRoutine());
    }

    private IEnumerator ReturnToLobbyRoutine()
    {
        if (networkRunner != null)
        {
            networkRunner.Shutdown(); 
            Destroy(networkRunner);   
            networkRunner = null;     
        }

        yield return null; 
        ConnectToLobby(); 
    }
    public async void Initialize(Lobby lobby)
    {
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
        }
        networkRunner.AddCallbacks(this);
        var scene = SceneRef.FromIndex((int)ScenesManager.SceneOfMyGame.GamePlayMulti);

        if (AuthenticationService.Instance.PlayerId == lobby.HostId)
        {
            Debug.Log("Host start");
            var startGameTask = networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = lobby.Id,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                PlayerCount = lobby.MaxPlayers,
            });
            await startGameTask;
        }
        else
        {
            Debug.Log("Client start");
            var startGameTask = networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = lobby.Id,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
            await startGameTask;
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"-----Player Joined: {player.PlayerId}");
        Vector3 startPos = GetSpawnPosition(player.PlayerId);

        if (runner.IsServer)
        {
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, startPos, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }

    }
    private Vector3 GetSpawnPosition(int playerId)
    {
        Debug.Log("playerId: " + playerId);
        float y = -1;
        float z = -13;
        float x = 0;
        switch (playerId)
        {
            case 1:
                x = 0;
                break;
            case 2:
                x = 2;
                break;
            case 3:
                x = -2;
                break;
            case 4:
                x = 4;
                break;
            default:
                x = 0;
                break;
        }

        return new Vector3(x, y, z);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connect Failed: {reason}");
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
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
