using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

public class PlayerController : MonoBehaviour
{
    PlayerUnit playerUnit;
    Coroutine moveRoutine;
    public float rotateSpeed = 45f;

    void Start()
    {
        playerUnit = GetComponent<PlayerUnit>();
    }

    void Update()
    {
        if (Pointer.current == null)
            return;

        if (!Pointer.current.press.wasPressedThisFrame)
            return;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        HandleInput(screenPos);
    }

    void HandleInput(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Vector3Int targetPos = Vector3Int.RoundToInt(hit.transform.position);

        List<Vector3Int> path = PathFinding.instance.FindPath(GetPlayerPosition(), targetPos);

        if (path == null || path.Count == 0)
        {
            Debug.Log("Path not found");
            return;
        }

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPoint(path));
    }

    IEnumerator MoveToPoint(List<Vector3Int> path)
    {
        playerUnit.animator.SetBool("isMoving", true);
        foreach (var p in path)
        {
            Vector3 target = new Vector3(p.x, 1, p.z);
            Vector3Int dir = p - GetPlayerPosition();
            Quaternion lookRot = GetRotationFromDirection(dir);

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    5f * Time.deltaTime
                );

                playerUnit.visualRoot.transform.rotation = Quaternion.Slerp(
                    playerUnit.visualRoot.transform.rotation,
                    lookRot,
                    rotateSpeed * Time.deltaTime
                );
                yield return null;
            }

        }
        playerUnit.animator.SetBool("isMoving", false);
    }

    Vector3Int GetPlayerPosition()
    {
        Vector3 pos = transform.position;
        return new Vector3Int(
            Mathf.RoundToInt(pos.x),
            0,
            Mathf.RoundToInt(pos.z)
        );
    }

    Quaternion GetRotationFromDirection(Vector3Int dir)
    {
        if (dir == Vector3Int.forward)
            return Quaternion.Euler(0, 0, 0);

        if (dir == Vector3Int.back)    
            return Quaternion.Euler(0, 180, 0);

        if (dir == Vector3Int.right)     
            return Quaternion.Euler(0, 90, 0);

        if (dir == Vector3Int.left)       
            return Quaternion.Euler(0, -90, 0);

        return playerUnit.visualRoot.transform.rotation;
    }

}
