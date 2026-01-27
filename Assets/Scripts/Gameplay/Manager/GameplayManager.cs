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
    [SerializeField] private LevelData levelData;

    [Header("Environment")]
    public Transform environmentParent;
    public GameObject levelPrefab;
    public Transform startPoint;
    public Block finishPoint;

    private GameManager gameManager;

    [Header("UI")]
    [SerializeField] private Image[] collectiblesIcon;
    public int collectiblesCollected = 0;
    #endregion

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        gameManager = GameManager.instance;
        levelData = gameManager.selectedLevelData;
        levelPrefab = levelData.levelPrefab;
        levelData.ResetLevel();

        SpawnEnvironment();

        finishPoint.gameObject.SetActive(false);


        foreach (Image icon in collectiblesIcon)
        {
            SetAlpha(icon, 0.5f);
        }
    }

    private void SpawnEnvironment()
    {
        Instantiate(levelPrefab, environmentParent);
        playerUnit = Instantiate(playerPrefab, startPoint.position + Vector3.up, startPoint.localRotation, environmentParent).GetComponent<AnimalUnit>();
        Debug.Log("Player Spawned at " + playerUnit.transform.position);
    }

    public SerializedDictionary<Vector3Int, Block> RegisterBlock()
    {
        SerializedDictionary<Vector3Int, Block> blocks = new SerializedDictionary<Vector3Int, Block>();
        List<Block> allBlocks = environmentParent.GetComponentsInChildren<Block>().ToList();

        foreach (Block block in allBlocks)
        {
            Vector3Int pos = block.position;
            if (!blocks.ContainsKey(pos))
            {
                blocks.Add(pos, block);
            }
        }

        return blocks;
    }

    public void OnCollectiblesPicked()
    {
        levelData.CollectiblesCollected();

        if (collectiblesCollected < collectiblesIcon.Length)
        {
            SetAlpha(collectiblesIcon[collectiblesCollected], 1.0f);
            collectiblesCollected++;
        }

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

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
