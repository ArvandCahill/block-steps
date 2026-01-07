using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

        Debug.Log("Scene " + sceneName + " has been loaded!");
    }

    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneAsync(sceneName));

    public void LoadMainMenu()
    {
        gameManager.gameState = GameState.MainMenu;
        LoadScene("Main Menu");
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }
}