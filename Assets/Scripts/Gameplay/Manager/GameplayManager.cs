using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine.UI;
using Unity.Behavior;

public class GameplayManager : MonoBehaviour
{
    #region Variables
    public static GameplayManager instance { get; private set; }

    [Header("Core")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public AnimalUnit playerUnit;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AIController aiController;
    [SerializeField] private CameraManager cameraManager;   
    [SerializeField] public LevelData levelData;

    [Header("Environment")]
    [SerializeField] private Transform environmentParent;
    [SerializeField] private GameObject levelPrefab;
    public Transform startPoint;
    public Block finishPoint;

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

    public void DisableAI()
    {
        foreach (BehaviorGraphAgent agent in aiController.agents)
        {
            agent.enabled = false;
        }
    }

    public List<Vector3Int> GetAllBlockPos()
    {
        return null;
    }

    public void Pause()
    {
        if (isPaused)
        {
            InputManager.instance.playerMap.Enable();
            Time.timeScale = 1.0f;
            isPaused = false;
        }

        else
        {
            InputManager.instance.playerMap.Disable();
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}
