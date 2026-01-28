using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneLoader : MonoBehaviour
{
    GameManager gameManager;
    private bool isLoading = false;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (isLoading) yield break;
        isLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            yield return null;
        }

        /*gameManager.gameState = Enum.TryParse()*/

        Debug.Log("Scene " + sceneName + " has been loaded!");
    }

    public void LoadScene(string sceneName)
    {
        Enum.TryParse(sceneName, true, out gameManager.gameState);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        gameManager.gameState = GameState.MainMenu;
        LoadScene("Main Menu");
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
        Time.timeScale = 1f;
    }
}