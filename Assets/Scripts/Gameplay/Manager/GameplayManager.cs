using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

public class GameplayManager : MonoBehaviour
{
    #region Variables
    public static GameplayManager instance { get; private set; }

    [Header("Core")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerUnit playerUnit;
    [SerializeField] private PlayerController playerController;

    [Header("Environment")]
    public Transform environmentParent;
    public GameObject levelPrefab;
    public Vector3 startPoint;
    public Vector3 finishPoint;

    [Header("References")]
    private GameManager gameManager;
    #endregion

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        gameManager = GameManager.instance;

        SpawnEnvironment();
    }

    private void SpawnEnvironment()
    {
        playerUnit = Instantiate(playerPrefab, startPoint + Vector3.up, Quaternion.identity, environmentParent).GetComponent<PlayerUnit>();
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

    public List<Vector3Int> GetAllBlockPos()
    {
        return null;
    }

}
