using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool isBgmOn = true;
    public bool isSfxOn = true;

    [Header("Save Settings")]
    public int currency = 0;
    private List<int> unlockedUnit = new();
    private int unlockedStages = 1;
    public bool isFirstTimePlaying;

    [Header("Resource")]
    [SerializeField] private AnimalData selectedAnimalData;
    [SerializeField] private LevelData selectedLevelData;
    public List<AnimalData> allAnimalData;
    public List<LevelData> allLevelData;
    
    [HideInInspector] public bool isAnimating;
    private int sceneCount = 0;

    private void Awake()
    {
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
        saveManager = GetComponent<SaveManager>();
        LoadResource();
        LoadGame(saveManager.LoadSaveData());
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        audioManager?.Initialize();

        uiManager?.HideAllPopups();

        ApplyAudioSettings();
    }

    public void ApplyAudioSettings()
    {
        audioManager?.SetBgmActive(isBgmOn);
        audioManager?.SetSfxActive(isSfxOn);
    }

    #region Getters and Setters
    public void SetSelectedAnimal(AnimalData animalData) => selectedAnimalData = animalData;

    public AnimalData GetSelectedAnimal() => selectedAnimalData;

    public void SetSelectedLevel(LevelData levelData) => selectedLevelData = levelData;

    public LevelData GetSelectedLevel() => selectedLevelData;

    public void SetGameState(GameState newGameState) => gameState = newGameState;
    #endregion

    public void ResetSceneCount() => sceneCount = 0;

    private void LoadResource()
    {
        allAnimalData = ResourcesLoader.LoadAllAnimalData();
        allLevelData = ResourcesLoader.LoadAllLevelData();
    }

    private void LoadGame(SaveData saveData)
    {
        isBgmOn = saveData.isBgmOn;
        isSfxOn = saveData.isSfxOn;
        currency = saveData.currency;
        unlockedStages = saveData.unlockedStages;
        isFirstTimePlaying = saveData.isFirstTimePlaying;
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
}