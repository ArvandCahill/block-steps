using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameplayUI : MonoBehaviour
{
    private GameplayManager GameplayManager => GameplayManager.instance;
    private InputManager InputManager => InputManager.instance;
    private GameState currentState;

    [Header("Collectibles")]
    [SerializeField] private Transform appleImageParent;
    [SerializeField] private List<Image> appleImages = new();
    [SerializeField] private Image applePrefab;

    private int collectedIndex = 0;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button nextLevelButton;

    private void OnEnable()
    {
        GameEvents.OnCollectiblePicked += UpdateCollectiblesUI;
        GameEvents.OnPlayerFinished += OnPlayerFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectiblePicked -= UpdateCollectiblesUI;
        GameEvents.OnPlayerFinished -= OnPlayerFinished;
    }

    private void Start()
    {
        currentState = GameManager.instance.GetGameState();
        InstantiateAppleImages();
        HideResultUI();
    }

    private void InstantiateAppleImages()
    {
        for (int i = 0; i < GameplayManager.levelData.maxCollectibles; i++)
        {
            var img = Instantiate(applePrefab, appleImageParent);
            appleImages.Add(img);
            SetAppleColor(i, false);
        }
    }

    private void SetAppleColor(int index, bool isCollected)
    {
        if (index < 0 || index >= appleImages.Count) return;
        appleImages[index].color = isCollected ? Color.white : Color.gray;
    }

    public void UpdateCollectiblesUI()
    {
        SetAppleColor(collectedIndex, true);
        collectedIndex++;
    }

    public void ShowPause()
    {
        pauseButton.gameObject.SetActive(false);
        currentState = GameState.Paused;
        InputManager.DisableAllMap();

        Time.timeScale = 0f;

        ShowResultUI(
            pause: false,
            home: true,
            cont: true,
            replay: true,
            next: false
        );
    }

    public void ShowWin()
    {
        currentState = GameState.Win;

        InputManager.DisableAllMap();

        ShowResultUI(
            pause: false,
            home: true,
            cont: false,
            replay: true,
            next: true
        );
    }

    public void ShowLose()
    {
        currentState = GameState.Lose;

        InputManager.DisableAllMap();

        ShowResultUI(
            pause: false,
            home: true,
            cont: false,
            replay: true,
            next: false
        );
    }

    public void ContinueGame()
    {
        if (currentState != GameState.Paused) return;

        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1f;
        currentState = GameState.Playing;
        InputManager.EnableAllMap();

        HideResultUI();
    }

    private void ShowResultUI(bool pause, bool home, bool cont, bool replay, bool next)
    {
        resultPanel.SetActive(true);

        pauseButton.gameObject.SetActive(pause);
        homeButton.gameObject.SetActive(home);
        continueButton.gameObject.SetActive(cont);
        replayButton.gameObject.SetActive(replay);
        nextLevelButton?.gameObject.SetActive(next);
    }

    private void HideResultUI()
    {
        resultPanel.SetActive(false);

        homeButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        nextLevelButton?.gameObject.SetActive(false);
    }

    private void OnPlayerFinished(bool isWin)
    {
        if (isWin) ShowWin();
        else ShowLose();
    }
}
