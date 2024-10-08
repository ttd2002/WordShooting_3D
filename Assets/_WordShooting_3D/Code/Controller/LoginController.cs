using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public event Action<bool, PlayerInfo, string> OnSignedIn;
    public event Action<string> OnSignedOut;
    private PlayerInfo playerInfo;
    async void Awake()
    {
        await UnityServices.InitializeAsync();

        PlayerAccountService.Instance.SignedIn += SignedInWithUnity;
        PlayerAccountService.Instance.SignedOut += OnUserSignedOut;
    }

    private async void SignedInWithUnity()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;

            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Already signed in.");
                return;
            }

            await SignInWithUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    public void InitSignOut()
    {
        try
        {
            AuthenticationService.Instance.SignOut(true);

            Debug.Log("Signed Out");

            OnSignedOut?.Invoke("Signed Out Completed");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn successfully.");

            playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();

            string name = await AuthenticationService.Instance.GetPlayerNameAsync(false);

            bool firstTime = string.IsNullOrEmpty(name);

            if (firstTime)
            {
                Debug.Log("First time log in");
            }
            else
            {
                Debug.Log("Not first time log in");
            }

            OnSignedIn?.Invoke(firstTime, playerInfo, name);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnUserSignedOut()
    {
        Debug.Log("-- Signed Out");

        PlayerPrefs.DeleteKey("user_auth_token");
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignedInWithUnity;
        PlayerAccountService.Instance.SignedOut -= OnUserSignedOut;
    }
}
