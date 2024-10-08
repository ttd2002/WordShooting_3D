using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private LoginController loginController;
    private void OnEnable()
    {
        loginButton.onClick.AddListener(LoginButtonPressed);
        logoutButton.onClick.AddListener(LogOutButtonPressed);
        loginController.OnSignedIn += LoginController_OnsignedIn;
        loginController.OnSignedOut += LoginController_OnsignedOut;
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(LoginButtonPressed);
        logoutButton.onClick.RemoveListener(LogOutButtonPressed);
        loginController.OnSignedIn -= LoginController_OnsignedIn;
    }

    private async void LoginButtonPressed()
    {
        await loginController.InitSignIn();
    }
    private void LogOutButtonPressed()
    {
        loginController.InitSignOut();
    }
    private void LoginController_OnsignedIn(bool firstTime, PlayerInfo playerInfo, string playerName)
    {
        Debug.Log("Player Name: " + playerName + " | Player ID: " + playerInfo.Id);
        UIManagerMainMenu.Instance.OpenMainMenu();
        FirebaseManager.Instance.userId = playerInfo.Id;
    }
    private void LoginController_OnsignedOut(string response)
    {
        Debug.Log(response);
    }
}
