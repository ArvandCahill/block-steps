using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Teleporter : MonoBehaviour
{
    [Header("Teleporter")]
    [SerializeField] private Teleporter targetPortal;

    [Header("Exit Points)")]
    [SerializeField] private Transform exitForward;
    [SerializeField] private Transform exitBackward;
    [SerializeField] private Transform exitLeft;
    [SerializeField] private Transform exitRight;

    [Header("Settings")]
    [SerializeField] private MoveDirection exitDirection;
    [SerializeField] private float teleportDelay = 1f;
    [SerializeField] private float cooldown = 1f;

    private static HashSet<AnimalUnit> globalCooldown = new();

    private AnimalUnit player;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<AnimalUnit>();

        if(player == null)
            return;

        if (globalCooldown.Contains(player))
            return;

        Debug.Log("Player enter portal");

        StartCoroutine(Teleport(player));
    }

    private IEnumerator Teleport(AnimalUnit player)
    {
        globalCooldown.Add(player);

        yield return new WaitForSeconds(teleportDelay);

        if (player.moveRoutine != null)
        {
            StopCoroutine(player.moveRoutine);
            player.moveRoutine = null;
        }

        PathFinding.instance.EnableMarker(player, Vector3.zero, false);

        Transform exitPoint = targetPortal.GetExitPoint(exitDirection);

        if (exitPoint == null)
        {
            Debug.LogError("Invalid exit direction for teleporter.");
            yield break;
        }

        player.transform.position = Snap(exitPoint.position);
        player.visualRoot.rotation = exitPoint.rotation;

        yield return new WaitForSeconds(cooldown);

        globalCooldown.Remove(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<AnimalUnit>();

        if (player == null)
            return;

        globalCooldown.Remove(player);
    }

    private Transform GetExitPoint(MoveDirection direction)
    {
        return direction switch
        {
            MoveDirection.Forward => exitForward,
            MoveDirection.Backward => exitBackward,
            MoveDirection.Left => exitLeft,
            MoveDirection.Right => exitRight,
            _ => null
        };
    }

    private Vector3 Snap(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y),
            Mathf.Round(pos.z)
        );
    }
}
