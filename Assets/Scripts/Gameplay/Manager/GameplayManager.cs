using UnityEngine;
using Unity.Behavior;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{
    #region Variables
    public static GameplayManager instance { get; private set; }

    [SerializeField] private SceneEnvironment sceneEnvironment;

    [Header("Core")]
    [SerializeField] public AnimalUnit playerUnit;
    [SerializeField] private AIController aiController;
    [SerializeField] private CameraManager cameraManager;   
    [SerializeField] public LevelData levelData;

    [Header("Environment")]
    [SerializeField] private Transform environmentParent;
    public List<Collectible> appleCollectibles = new();
    public Transform startPoint;
    public Block finishPoint; 

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private GameObject flag;

    public bool isPaused = false;

    private LevelProgress levelProgress;
    private int collectiblesCollected = 0;
    private int initialCollectiblesProgress;
    private bool isLevelFinished = false;
    #endregion

    private GameManager gameManager => GameManager.instance;
    private SaveManager saveManager => SaveManager.instance;

    private void OnEnable()
    {
        GameEvents.OnCollectiblePicked += OnCollectiblesPicked;
        GameEvents.OnLevelFinished += FinishLevel;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectiblePicked -= OnCollectiblesPicked;
        GameEvents.OnLevelFinished -= FinishLevel;
    }

    void Awake()
    {
        if (instance == null) instance = this;

        levelData = gameManager.GetSelectedLevel();
        levelPrefab = levelData?.levelPrefab;
        SpawnEnvironment();
        
    }

    void Start()
    {
        sceneEnvironment.Init(levelData);
        levelProgress = saveManager.GetLevelProgress(levelData.levelNumber);
        initialCollectiblesProgress = levelProgress.collectiblesCollected;
        finishPoint.gameObject.SetActive(false);
        cameraManager.SetCameraTarget(playerUnit.transform);
    }

    private void SpawnEnvironment()
    {
        if (levelData != null) Instantiate(levelPrefab, environmentParent);
        playerUnit = Instantiate(playerPrefab, startPoint.position + Vector3.up, startPoint.localRotation, environmentParent).GetComponent<AnimalUnit>();
        playerUnit.Init(gameManager.GetSelectedAnimal());
        Instantiate(flag, finishPoint.transform.position + Vector3.up, Quaternion.identity, finishPoint.transform);
        Debug.Log("Player Spawned at " + playerUnit.transform.position);
    }

    public void OnCollectiblesPicked()
    {
        collectiblesCollected++;

        if (collectiblesCollected == levelData.maxCollectibles)
        {
            finishPoint.gameObject.SetActive(true);
            Debug.Log("Finish Point Activated");
        }

        if (levelProgress.collectiblesCollected != levelData.maxCollectibles)
        {
            levelProgress.AddCollectiblesCollected(levelData);
            return;
        }
    }

    public void ResetCollectiblesCollected()
    {
        if (isLevelFinished) return;

        levelProgress.collectiblesCollected = initialCollectiblesProgress;
        saveManager.SaveGame();
    }

    private void FinishLevel(bool isWinning)
    {
        isLevelFinished = true;
        Reward();

        if (isWinning && saveManager.UnlockedLevels == levelData.levelNumber)
        {
            saveManager.UnlockedLevels += 2; 
            levelProgress.MarkAsCompleted();
        }
    }

    private void Reward()
    {
        int reward = collectiblesCollected - initialCollectiblesProgress;
        Debug.Log("Reward: " + reward);
        saveManager.AddCurrency(reward);
    }

    public void NextLevel()
    {
        GameManager.instance.LoadNextLevel();
    }

    [ContextMenu("Disable AI")]
    public void DisableAI()
    {
        foreach (BehaviorGraphAgent agent in aiController.agents)
        {
            agent.enabled = false;

            if (agent.TryGetComponent<AnimalUnit>(out var unit)) 
            { 
                unit.stopMovement = true;
            }
        }
    }

    [ContextMenu("Collect All Apples")]
    public void CollectApples()
    {
        foreach (Collectible apple in appleCollectibles)
        {
            apple.OnCollected();
            apple.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Teleport to Finish Point")]
    public void TeleportToFinish()
    {
        playerUnit.transform.position = finishPoint.transform.position + Vector3.up;
    }
}