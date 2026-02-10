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

    public SaveManager saveManager { get; private set; }
    public SceneLoader SceneLoader { get; private set; }

    [Header("Game Enumerators")]
    [SerializeField] private GameState gameState;

    [Header("Game Settings")]
    [SerializeField] private bool isBgmOn = true;
    [SerializeField] private bool isSfxOn = true;

    [Header("Save Settings")]
    [SerializeField] private int currency = 0;
    [SerializeField] private List<int> unlockedUnit = new();
    [SerializeField] private int unlockedStages = 1;
    public bool isFirstTimePlaying;

    [Header("Resource")]
    [SerializeField] private AnimalData selectedAnimalData;
    [SerializeField] private LevelData selectedLevelData;
    public List<AnimalData> allAnimalData;
    public List<LevelData> allLevelData;

    [Header("UI")]
    [SerializeField] private CanvasGroup panel;

    [HideInInspector] public bool isAnimating;
    private int sceneCount = 0;

    private void Awake()
    {
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

        LoadResource();
        SceneLoader = GetComponent<SceneLoader>();
        saveManager = GetComponent<SaveManager>();
        saveManager.LoadSaveData();
        admobInit.Init();
    }

    #region Properties
    public int Currency
    {
        get { return currency; } 
        set 
        {
            currency = value; 
            GameEvents.TriggerCurrencyValueChanged(currency);
        }
    }

    public bool IsBgmOn
    {
        get { return isBgmOn; }
        set 
        { 
            if (value == isBgmOn) return;

            isBgmOn = value; 
            audioManager?.SetBgmActive(IsBgmOn);
        }
    }

    public bool IsSfxOn
    {
        get { return isSfxOn; }
        set 
        { 
            if (value == isSfxOn) return;

            isSfxOn = value; 
            audioManager?.SetSfxActive(IsSfxOn);
        }
    }

    #endregion

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
        audioManager?.Initialize(IsBgmOn, IsSfxOn);

        uiManager?.HideAllPopups();

        ApplyAudioSettings();
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

    public void ApplyAudioSettings()
    {
        audioManager?.SetBgmActive(IsBgmOn);
        audioManager?.SetSfxActive(IsSfxOn);
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

            if (nextIndex + 1 > unlockedStages)
            {
                unlockedStages = nextIndex + 1;
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