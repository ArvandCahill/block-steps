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

        gameManager.LoadingScreen(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            yield return null;
        }

        Enum.TryParse(sceneName, true, out GameState gameState);
        gameManager.SetGameState(gameState);
        gameManager.LoadingScreen(false);
        isLoading = false;
        Debug.Log("Scene " + sceneName + " has been loaded!");
    }

    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneAsync(sceneName));

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
        Time.timeScale = 1f;
    }
}