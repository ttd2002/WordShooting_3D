using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using WebSocketSharp;

public class LoginController : MonoBehaviour
{
    private WebSocket _ws;
    private FirebaseAuth _auth;
    private string _clientId;
    private bool _isLogin = false;
    private string _googleClientId = "457644578552-ge0cdfu5959f59455cj1lbrge8fsqfe2.apps.googleusercontent.com";
    private string _googleClientSecret = "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";
    private string _redirectUri = "http://localhost:3000";

    void Update()
    {
        MainThreadInvoker.Update();
        if (!_isLogin) return;
        UIManagerMainMenu.Instance.OpenMainMenu();
        _isLogin = false;
    }

    void Start()
    {
        InitializeFirebase();
        _clientId = Guid.NewGuid().ToString();
        this._ws = new WebSocket("ws://localhost:3000");
        this._ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connected.");
            this._ws.Send(_clientId.ToString());
        };
        this._ws.OnMessage += OnMessageReceived;
        this._ws.Connect();
    }

    private async void OnMessageReceived(object sender, MessageEventArgs e)
    {
        string authCode = e.Data;
        await ExchangeCodeForTokenHttpClient(authCode);
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                this._auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    public void LoginWithGoogle()
    {
        string oauthUrl = $"https://accounts.google.com/o/oauth2/auth?" +
                          $"client_id={this._googleClientId}&" +
                          "response_type=code&" +
                          "scope=email%20profile&" +
                          $"redirect_uri={this._redirectUri}&" +
                          $"state={Uri.EscapeDataString(_clientId)}";
        Debug.Log("oauthUrl: " + oauthUrl);
        Application.OpenURL(oauthUrl);
    }

    public async Task CompleteLogin(User user)
    {
        string userKey = user.email.Replace("@", "_").Replace(".", "_");
        ConnectDatabase.Instance.userName = user.name;
        ConnectDatabase.Instance.userId = userKey;
        await UserController.Instance.SaveUserDataAsync(user);
        _isLogin = true;
        string userName = user.email.Split('@')[0];
        MainThreadInvoker.RunOnMainThread(async () =>
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(userName, "Test123.");
            }
            catch (RequestFailedException ex)
            {
                if (ex.Message.Contains("Invalid username or password"))
                {
                    await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(userName, "Test123.");
                }
                else
                {
                    Debug.LogError($"Error during authentication: {ex.Message}");
                }
            }
        });
    }

    public async Task ExchangeCodeForTokenHttpClient(string authCode)
    {
        string tokenEndpoint = "https://oauth2.googleapis.com/token";
        var client = new HttpClient();
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("client_id", this._googleClientId),
            new KeyValuePair<string, string>("client_secret", this._googleClientSecret),
            new KeyValuePair<string, string>("redirect_uri", this._redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
        });

        try
        {
            var response = await client.PostAsync(tokenEndpoint, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonUtility.FromJson<GoogleTokenResponse>(responseString);

                var playerInfo = await client.GetStringAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={tokenResponse.id_token}");
                var playerInfoToken = JsonUtility.FromJson<User>(playerInfo);
                MainThreadInvoker.RunOnMainThread(async () =>
                {
                    await CompleteLogin(playerInfoToken);
                });
            }
            else
            {
                Debug.LogError("Error exchanging code for token: " + response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("HttpClient Error: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        if (_ws != null && _ws.IsAlive)
        {
            _ws.Close();
        }
    }
}

[Serializable]
public class GoogleTokenResponse
{
    public string access_token;
    public string expires_in;
    public string token_type;
    public string refresh_token;
    public string id_token;
}
