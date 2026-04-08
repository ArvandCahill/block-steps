using UnityEngine;
using Unity.Behavior;
using System.Collections.Generic;
using System.Linq;

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

        Debug.Log("Night Mode: " + levelData.isNightMode);
        if (levelData.isNightMode) RandomizeCollectibles();
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

    private void FinishLevel(bool isWinning)
    {
        string sfxName = isWinning ? "Win" : "Lose";

       
        gameManager.audioManager.PlaySFX(sfxName);
        isLevelFinished = true;

        Reward();

        if (levelData.isNightMode) return;
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

    #region Randomizer

    private void RandomizeCollectibles()
    {
        Debug.Log("=== RANDOMIZE START ===");

        List<Vector3Int> walkables = PathFinding.instance.GetAllWalkablePositions();
        Debug.Log("Total Walkables: " + walkables.Count);

        List<Vector3Int> chosenPositions = new();

        walkables = walkables.OrderBy(x => Random.value).ToList();

        int minGridDistance = 8;
        int minDistanceFromStart = 4;
        int minDistanceFromFinish = 4;

        Vector3Int startPos = PathFinding.instance.GetPlayerPosition(startPoint.position);
        Vector3Int finishPos = finishPoint.GetPosition();

        Debug.Log("Start Pos: " + startPos);
        Debug.Log("Finish Pos: " + finishPos);

        int skippedStart = 0;
        int skippedFinish = 0;
        int skippedTooClose = 0;

        foreach (var pos in walkables)
        {
            int distToStart =
                Mathf.Abs(pos.x - startPos.x) +
                Mathf.Abs(pos.y - startPos.y) +
                Mathf.Abs(pos.z - startPos.z);

            if (distToStart < minDistanceFromStart)
            {
                skippedStart++;
                continue;
            }

            int distToFinish =
                Mathf.Abs(pos.x - finishPos.x) +
                Mathf.Abs(pos.y - finishPos.y) +
                Mathf.Abs(pos.z - finishPos.z);

            if (distToFinish < minDistanceFromFinish)
            {
                skippedFinish++;
                continue;
            }

            bool tooClose = false;

            foreach (var chosen in chosenPositions)
            {
                int dist =
                    Mathf.Abs(pos.x - chosen.x) +
                    Mathf.Abs(pos.y - chosen.y) +
                    Mathf.Abs(pos.z - chosen.z);

                if (dist < minGridDistance)
                {
                    tooClose = true;
                    skippedTooClose++;
                    break;
                }
            }

            if (tooClose) continue;

            chosenPositions.Add(pos);
            Debug.Log("Chosen Pos: " + pos);

            if (chosenPositions.Count >= appleCollectibles.Count)
                break;
        }

        Debug.Log("Skipped (Start): " + skippedStart);
        Debug.Log("Skipped (Finish): " + skippedFinish);
        Debug.Log("Skipped (Too Close): " + skippedTooClose);
        Debug.Log("Final Chosen Count: " + chosenPositions.Count);
        Debug.Log("Apple Count Needed: " + appleCollectibles.Count);

        for (int i = 0; i < appleCollectibles.Count; i++)
        {
            if (i >= chosenPositions.Count)
            {
                Debug.LogWarning("Not enough valid positions for all apples!");
                break;
            }

            Vector3Int gridPos = chosenPositions[i];
            Vector3 worldPos = new Vector3(gridPos.x, gridPos.y + 1, gridPos.z);

            appleCollectibles[i].transform.parent.position = worldPos;

            Debug.Log($"Apple {i} placed at {worldPos}");
        }

        Debug.Log("=== RANDOMIZE END ===");
    }

    #endregion
}