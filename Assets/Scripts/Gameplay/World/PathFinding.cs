using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using System.Collections;

public class PathFinding : MonoBehaviour
{
    public static PathFinding instance { get; private set; }

    [SerializedDictionary("Position", "Game Object")]
    public SerializedDictionary<Vector3Int, Block> blocks = new SerializedDictionary<Vector3Int, Block>();

    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private AnimationClip moveAnim;
    [SerializeField] private Transform marker;

    static readonly Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.left,
        Vector3Int.right,
        Vector3Int.forward,
        Vector3Int.back,
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

        foreach (var dir in directions)
        {
            Vector3Int sameLevel = position + dir;
            if (IsPositionWalkable(sameLevel))
            {
                neighbors.Add(sameLevel);
                continue;
            }

            Vector3Int upStep = position + dir + Vector3Int.up;
            if (IsPositionWalkable(upStep))
            {
                neighbors.Add(upStep);
                continue;
            }

            Vector3Int downStep = position + dir + Vector3Int.down;
            if (IsPositionWalkable(downStep))
            {
                neighbors.Add(downStep);
            }
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
            if (!unit.isPlayer)
            {
                blocks[Vector3Int.RoundToInt(unit.transform.position) + Vector3Int.down].isWalkable = true;

                if (path[i] == path[^1]) blocks[path[i]].isWalkable = false;
            }

            unit.animator.SetBool("isMoving", true);
            Vector3 start = unit.transform.position;
            Vector3 target = new Vector3(path[i].x, path[i].y + 1, path[i].z);
            Vector3Int dir = path[i] - path[i - 1];
            Quaternion lookRot = unit.GetRotationFromDirection(new Vector3Int(dir.x, 0, dir.z));

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / stepDuration;

                unit.rb.MovePosition(Vector3.Lerp(start, target, t));

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

        EnableMarker(unit, Vector3.zero, false);
        unit.moveRoutine = null;
    }

    public Coroutine StartMoveRoutine(AnimalUnit unit, List<Vector3Int> path)
    {
        return StartCoroutine(MoveToPoint(unit, path));
    }

    public void Move(AnimalUnit unit, Vector3Int targetPos)
    {
        if (unit.gameObject.transform.position == new Vector3(targetPos.x, targetPos.y + 1, targetPos.z)) return;

        if (unit.isMoving)
        {
            StopMove(unit);
            EnableMarker(unit, targetPos, false);
            return;
        }

        List<Vector3Int> path = FindPath(GetPlayerPosition(unit.transform.position), targetPos);

        if (path == null || path.Count == 0)
        {
            Debug.Log("Path not found");
            return;
        }

        unit.stopMovement = false;
        EnableMarker(unit, targetPos, true);
        unit.moveRoutine = StartMoveRoutine(unit, path);
    }

    void StopMove(AnimalUnit unit)
    {
        unit.stopMovement = true;
    }

    public void EnableMarker(AnimalUnit unit, Vector3 position, bool enable)
    {
        if (!unit.isPlayer) return;
        marker.gameObject.SetActive(enable);
        marker.position = position + Vector3.up;
    }

    public Vector3Int GetPlayerPosition(Vector3 pos)
    {
        return new Vector3Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(0),
            Mathf.RoundToInt(pos.z)
        );
    }
}