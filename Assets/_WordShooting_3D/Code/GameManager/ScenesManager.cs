using System.Collections;
using System.Collections.Generic;
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
        GamePlay,
        GamePlayMulti
    }

    public void LoadScene(SceneOfMyGame scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public void LoadNewGame()
    {
        SceneManager.LoadScene(SceneOfMyGame.GamePlay.ToString());
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
        UIManagerMainMenu.Instance.OpenMainMenu();
        Time.timeScale = 1f;
    }


}
