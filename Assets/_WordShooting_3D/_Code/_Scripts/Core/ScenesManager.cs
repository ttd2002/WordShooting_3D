using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : SingletonAbstract<ScenesManager>
{

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public enum SceneOfMyGame
    {
        MainMenu,
        GamePlay_Ver2,
        GamePlayMulti_Ver2
    }

    public void LoadScene(SceneOfMyGame scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public void LoadNewGame()
    {
        SceneManager.LoadScene(SceneOfMyGame.GamePlay_Ver2.ToString());
    }
    public void BackToMenuFromGameplay()
    {
        StartCoroutine(LoadSceneAsync(SceneOfMyGame.MainMenu.ToString()));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        UIGameplayManager.Instance.LoadingUI.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
        LobbiesManager.Instance.ReloadComponent();
        UIManagerMainMenu.Instance.OpenMainMenu();
        Time.timeScale = 1f;
    }

    public async void BackToMenuFromMultiGameplay()
    {
        await LoadSceneFromMultiAsync(SceneOfMyGame.MainMenu.ToString());
    }

    private async Task LoadSceneFromMultiAsync(string sceneName)
    {
        UIGameplayManager.Instance.LoadingUI.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            await Task.Yield();
        }

        FusionNetworkManager.Instance.ReturnToLobby();
        if (LobbiesManager.Instance.isLeader)
        {
            await LobbiesManager.Instance.ResetRoom(); 

        }
        LobbiesManager.Instance.ReloadComponent();
        LobbiesManager.Instance.UpdateLobbyInfo();
        UIManagerMainMenu.Instance.ClosePanel();
        UIManagerMainMenu.Instance.OpenRoomDetail();
        Time.timeScale = 1f;
    }



}
