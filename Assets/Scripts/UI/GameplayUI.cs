using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

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
    [SerializeField] private CanvasGroup resultCanvasGroup;
    [SerializeField] private TextMeshProUGUI resultTitleText;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button nextLevelButton;

    [Header("Animations")]
    [SerializeField] private float panelAnimDuration = 0.3f;

    private void OnEnable()
    {
        GameEvents.OnCollectiblePicked += UpdateCollectiblesUI;
        GameEvents.OnLevelFinished += OnPlayerFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectiblePicked -= UpdateCollectiblesUI;
        GameEvents.OnLevelFinished -= OnPlayerFinished;
    }

    private void Start()
    {
        currentState = GameManager.instance.GetGameState();
        InstantiateAppleImages();
        HideResultUI();
    }

    #region Collectibles
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
        if (collectedIndex >= appleImages.Count) return;

        var img = appleImages[collectedIndex];
        SetAppleColor(collectedIndex, true);

        img.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 6, 0.5f);

        collectedIndex++;
    }
    #endregion

    #region States
    public void ShowPause()
    {
        currentState = GameState.Paused;
        pauseButton.gameObject.SetActive(false);
        InputManager.DisableAllMap();
        Time.timeScale = 0f;

        SetResultText("PAUSED");

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

        SetResultText("YOU WIN!");

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

        SetResultText("YOU'RE CAPTURED");

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
    #endregion

    #region Result UI
    private void SetResultText(string text)
    {
        resultTitleText.text = text;
        resultTitleText.alpha = 0f;

        resultTitleText
            .DOFade(1f, 0.25f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    private void ShowResultUI(bool pause, bool home, bool cont, bool replay, bool next)
    {
        resultPanel.SetActive(true);

        resultPanel.transform.localScale = Vector3.zero;
        resultCanvasGroup.alpha = 0f;

        resultPanel.transform
            .DOScale(Vector3.one, panelAnimDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        resultCanvasGroup
            .DOFade(1f, panelAnimDuration)
            .SetUpdate(true);

        pauseButton.gameObject.SetActive(pause);
        homeButton.gameObject.SetActive(home);
        continueButton.gameObject.SetActive(cont);
        replayButton.gameObject.SetActive(replay);
        nextLevelButton?.gameObject.SetActive(next);

        AnimateButtons();
    }

    private void AnimateButtons()
    {
        float delay = 0.05f;
        AnimateButton(homeButton, delay);
        AnimateButton(continueButton, delay + 0.05f);
        AnimateButton(replayButton, delay + 0.1f);
        AnimateButton(nextLevelButton, delay + 0.15f);
    }

    private void AnimateButton(Button btn, float delay)
    {
        if (btn == null || !btn.gameObject.activeSelf) return;

        btn.transform.localScale = Vector3.zero;
        btn.transform
            .DOScale(Vector3.one, 0.25f)
            .SetDelay(delay)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void HideResultUI()
    {
        resultPanel.SetActive(false);

        homeButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        nextLevelButton?.gameObject.SetActive(false);
    }
    #endregion

    private void OnPlayerFinished(bool isWin)
    {
        if (isWin) ShowWin();
        else ShowLose();
    }
}
