using UnityEngine;
using Unity.Behavior;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameplayManager : MonoBehaviour
{
    #region Variables
    public static GameplayManager instance { get; private set; }

    [Header("Core")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public AnimalUnit playerUnit;
    [SerializeField] private AIController aiController;
    [SerializeField] private CameraManager cameraManager;   
    [SerializeField] public LevelData levelData;

    [Header("Environment")]
    [SerializeField] private Transform environmentParent;
    public Transform startPoint;
    public Block finishPoint;
    [SerializeField] private Material daySkyMat;
    [SerializeField] private Material nightSkyMat;

    [Header("Lighting")]
    [SerializeField] private Light directionalLight;
    [SerializeField] Volume postProcessVolume;    

    [Header("Prefabs")]
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private GameObject flag;

    private GameManager gameManager;
    public bool isPaused = false;

    public int collectiblesCollected = 0;
    #endregion

    private void OnEnable()
    {
        GameEvents.OnCollectiblePicked += OnCollectiblesPicked;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectiblePicked -= OnCollectiblesPicked;
    }

    void Awake()
    {
        if (instance == null) instance = this;

        gameManager = GameManager.instance;
        levelData = gameManager.GetSelectedLevel();
        levelPrefab = levelData?.levelPrefab;
        levelData?.ResetLevel();
        SpawnEnvironment();
    }

    void Start()
    {
        cameraManager.SetCameraTarget(playerUnit.transform);
        finishPoint.gameObject.SetActive(false);
    }

    private void SpawnEnvironment()
    {
        if (levelData != null) Instantiate(levelPrefab, environmentParent);
        playerUnit = Instantiate(playerPrefab, startPoint.position + Vector3.up, startPoint.localRotation, environmentParent).GetComponent<AnimalUnit>();
        playerUnit.Init(gameManager.GetSelectedAnimal());
        Instantiate(flag, finishPoint.transform.position + Vector3.up, Quaternion.identity, finishPoint.transform);
        ChangeSkybox();
        SetLightIntensity();
        SetExposure();
        Debug.Log("Player Spawned at " + playerUnit.transform.position);
    }

    public void OnCollectiblesPicked()
    {
        levelData.CollectiblesCollected();
        collectiblesCollected++;

        if (levelData.IsFinish())
        {
            finishPoint.gameObject.SetActive(true);
            Debug.Log("Finish Point Activated");
        }
    }

    void ChangeSkybox()
    {
        if (!levelData.isNightMode)
        {
            RenderSettings.skybox = daySkyMat;
        }
        else
        {
            RenderSettings.skybox = nightSkyMat;
        }
    }

    void SetLightIntensity()
    {
        if (!levelData.isNightMode)
        {
            directionalLight.intensity = 1f;
        }
        else
        {
            directionalLight.intensity = 0.2f;
        }
    }

    void SetExposure()
    {
        if (postProcessVolume.profile.TryGet(out ColorAdjustments exposure))
        {
            float targetValue = levelData.isNightMode ? -2f : 0f;

            exposure.postExposure.value = targetValue;
            Debug.Log("Exposure set to " + exposure.postExposure.value);
        }
        else
        {
            Debug.LogWarning("ColorAdjustments not found in Volume!");
        }
    }

    public void NextLevel()
    {
        GameManager.instance.LoadNextLevel();
    }

    public void DisableAI()
    {
        foreach (BehaviorGraphAgent agent in aiController.agents)
        {
            agent.enabled = false;
        }
    }
}