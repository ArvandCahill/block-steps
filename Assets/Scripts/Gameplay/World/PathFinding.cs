using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using System.Collections;
using Unity.Burst;

[BurstCompile]
public class PathFinding : MonoBehaviour
{
    public static PathFinding instance { get; private set; }

    [SerializedDictionary("Position", "Game Object")]
    public SerializedDictionary<Vector3Int, Block> blocks = new SerializedDictionary<Vector3Int, Block>();

    static readonly Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.left,
        Vector3Int.right,
        Vector3Int.forward,
        Vector3Int.back
    };

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        blocks = GameplayManager.instance.RegisterBlock();
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0) 
        {
            Vector3Int pos = queue.Dequeue();
            if (pos == target)
                break;

            foreach (var neighbor in GetNeighborBlocks(pos))
            {
                if (cameFrom.ContainsKey(neighbor))
                    continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = pos;
            }
        }

        if (!cameFrom.ContainsKey(target))
            return null;

        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int p = target;

        while (p != start)
        {
            path.Add(p);
            p = cameFrom[p];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    List<Vector3Int> GetNeighborBlocks(Vector3Int position)
    {
        List<Vector3Int> neighbors = new();

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = position + dir;

            if (IsPositionWalkable(neighborPos))
                neighbors.Add(neighborPos);
        }

        return neighbors;
    }

    bool IsPositionWalkable(Vector3Int position)
    {
        if (blocks.ContainsKey(position))
        {
            Block block = blocks[position];
            return block.isWalkable;
        }
        return false;
    }

}
