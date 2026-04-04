using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;

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

    [Header("Scale Effect")]
    [SerializeField] private float shrinkDuration = 0.5f;
    [SerializeField] private float growDuration = 0.5f;
    [SerializeField] private float minScale = 0.1f;

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

        Transform playerTransform = player.transform;

        Tween shrinkTween = playerTransform
            .DOScale(minScale, shrinkDuration)
            .SetEase(Ease.InBack);

        yield return shrinkTween.WaitForCompletion();

        yield return new WaitForSeconds(teleportDelay);

        if (player.moveRoutine != null)
        {
            StopCoroutine(player.moveRoutine);
            player.moveRoutine = null;
        }

        PathFinding.instance.EnableMarker(player, Vector3.zero, false, false);

        Transform exitPoint = targetPortal.GetExitPoint(exitDirection);

        if (exitPoint == null)
        {
            Debug.LogError("Invalid exit direction for teleporter.");
            yield break;
        }

        playerTransform.position = Snap(exitPoint.position);
        player.visualRoot.rotation = exitPoint.rotation;

        playerTransform.localScale = Vector3.one * minScale;

        Tween growTween = playerTransform
            .DOScale(Vector3.one, growDuration)
            .SetEase(Ease.OutBack);

        yield return growTween.WaitForCompletion();

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
