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
    public GameState gameState;

    [Header("Game Settings")]
    public bool isBgmOn = true;
    public bool isSfxOn = true;
    public bool isVibrationOn = true;

    [Header("Save Settings")]
    public bool isFirstTimePlaying;
    public bool isFirstTimeShop;

    [Header("Resource")]
    

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

    //*public void PauseGame() => SetGameState(GameState.Pause);*//*

    public void SetGameState(GameState newGameState) => gameState = newGameState;

    public void ResetSceneCount() => sceneCount = 0;

    private void LoadResource()
    {
        
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

    public static void Vibrate(long milliseconds = 100, int amplitude = 255)
    {
        if (!instance.isVibrationOn) return;
#if UNITY_Android && !UNITY_EDITOR
        int sdkVersion = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (vibrator != null)
        {
            if (sdkVersion >= 26)
            {
                AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", vibrationEffect);
            }
            else
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
#endif
    }
}