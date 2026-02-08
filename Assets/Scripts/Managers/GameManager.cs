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

    //[Header ("Ads")]
    //[field: SerializeField] public AdmobInit admobInit { get; private set; }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        sceneCount++;

        uiManager?.HideAllPopups();
        //if (sceneName == "MainMenu" || sceneName == "ModeSelector") admobInit.DestroyBanner();
        //else admobInit.RequestBanner();
        //StartCoroutine(admobInit.ShowInterstitalWithDelay(sceneCount));
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