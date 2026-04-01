using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Sub-Managers")]
    [field: SerializeField] public AudioManager audioManager { get; private set; }
    [field: SerializeField] public UIManager uiManager { get; private set; }

    [Header ("Ads")]
    [field: SerializeField] public AdmobInit admobInit { get; private set; }

    public SceneLoader SceneLoader { get; private set; }

    [Header("Game Enumerators")]
    [SerializeField] private GameState gameState;

    [Header("Resource")]
    [SerializeField] private AnimalData selectedAnimalData;
    [SerializeField] private LevelData selectedLevelData;
    public List<AnimalData> allAnimalData;
    public List<LevelData> allLevelData;

    [Header("UI")]
    [SerializeField] private CanvasGroup panel;

    [HideInInspector] public bool isAnimating;
    private int sceneCount = 0;

    private SaveManager saveManager => SaveManager.instance;

    private void Awake()
    {
        LoadResource();

        Application.targetFrameRate = 60;

  
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        /*if (admobInit != null && admobInit.isAdActive)
        {
            admobInit.Init();
            admobInit.RequestBanner();
        }*/

        SceneLoader = GetComponent<SceneLoader>();
        admobInit.Init();
    }

   #region Getters and Setters
    public void SetSelectedAnimal(AnimalData animalData)
    {
        selectedAnimalData = animalData;
        saveManager.SetSelectedAnimalId(animalData.animalID);
    }

    public AnimalData GetSelectedAnimal() => selectedAnimalData;

    public void SetSelectedLevel(LevelData levelData) => selectedLevelData = levelData;

    public LevelData GetSelectedLevel() => selectedLevelData;

    public void SetGameState(GameState newGameState) => gameState = newGameState;

    public GameState GetGameState() => gameState;

    #endregion

    void Start()
    {
        saveManager.LoadSaveData();
        audioManager?.Initialize(saveManager.IsBgmOn, saveManager. IsSfxOn);

        uiManager?.HideAllPopups();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                audioManager.PlaySFX("Tap");
            }
        }
    }

    public void ResetSceneCount() => sceneCount = 0;

    private void LoadResource()
    {
        allAnimalData = ResourcesLoader.LoadAllAnimalData();
        allLevelData = ResourcesLoader.LoadAllLevelData();
    }

    public void LoadNextLevel()
    {
        if (selectedLevelData == null)
        {
            Debug.LogWarning("SelectedLevelData is null, can't load next level.");
            return;
        }

        int currentIndex = allLevelData.IndexOf(selectedLevelData);

        if (currentIndex < 0)
        {
            Debug.LogError("Current level not found in allLevelData list.");
            return;
        }

        int nextIndex = currentIndex + 1;

        if (nextIndex < allLevelData.Count)
        {
            LevelData nextLevel = allLevelData[nextIndex];

            SetSelectedLevel(nextLevel);

            if (nextIndex + 1 > saveManager.UnlockedLevels)
            {
                saveManager.UnlockedLevels = nextIndex + 1;
            }

            SceneLoader.RestartScene();
        }
        else
        {
            LoadingScreen(true);
            SceneLoader.LoadScene("MainMenu"); 
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        sceneCount++;

        uiManager?.HideAllPopups();

        if (sceneName == "MainMenu" && sceneCount > 0)
            admobInit.ShowInterstitial();
    }

    public void LoadingScreen(bool enable)
    {
        if (enable)
        {
            panel.gameObject.SetActive(enable);
            panel.DOFade(1, 0f);
        }

        else
        {
            panel.DOFade(0, 1f).onComplete = (() =>
            {
                panel.gameObject.SetActive(enable);
            });
        }
    }
}