using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using System.Collections;

public class PathFinding : MonoBehaviour
{
    public static PathFinding instance { get; private set; }

    [SerializedDictionary("Position", "Game Object")]
    public SerializedDictionary<Vector3Int, Block> blocks = new SerializedDictionary<Vector3Int, Block>();

    [SerializeField] public float rotateSpeed = 10f;
    [SerializeField] public AnimationClip moveAnim;

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

    IEnumerator MoveToPoint(AnimalUnit unit, List<Vector3Int> path)
    {
        float stepDuration = moveAnim.length / unit.movementSpeed;

        for (int i = 1; i < path.Count; i++)
        {
            unit.animator.SetBool("isMoving", true);
            Vector3 start = unit.transform.position;
            Vector3 target = new Vector3(path[i].x, 1, path[i].z);
            Vector3Int dir = path[i] - path[i - 1];
            Quaternion lookRot = unit.GetRotationFromDirection(dir);

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / stepDuration;

                unit.transform.position = Vector3.Lerp(start, target, t);

                unit.visualRoot.rotation = Quaternion.Slerp(
                    unit.visualRoot.rotation,
                    lookRot,
                    rotateSpeed * Time.deltaTime
                );

                yield return null;
            }

            unit.animator.SetBool("isMoving", false);
            yield return new WaitForSeconds(0.1f);

            if (unit.stopMovement) break;
        }

        unit.moveRoutine = null;
    }

    public Coroutine StartMoveRoutine(AnimalUnit unit, List<Vector3Int> path)
    {
        return StartCoroutine(MoveToPoint(unit, path));
    }

    public void Move(AnimalUnit unit, Vector3Int targetPos)
    {
        if (unit.isMoving)
        {
            StopMove(unit);
            return;
        }

        List<Vector3Int> path = FindPath(unit.GetPlayerPosition(), targetPos);

        if (path == null || path.Count == 0)
        {
            Debug.Log("Path not found");
            return;
        }

        unit.stopMovement = false;
        unit.moveRoutine = StartMoveRoutine(unit, path);
    }

    void StopMove(AnimalUnit unit)
    {
        unit.stopMovement = true;
    }
}