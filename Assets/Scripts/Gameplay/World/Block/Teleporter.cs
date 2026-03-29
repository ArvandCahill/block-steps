using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Teleporter : MonoBehaviour
{
    [Header("Teleport)")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private MoveDirection exitDirection;
    [SerializeField] private float teleportDelay = 1f;
    [SerializeField] private float cooldown = 1f;

    private static HashSet<AnimalUnit> globalCooldown = new HashSet<AnimalUnit>();

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

        Vector3 targetPos = targetPoint.position + GetOffset();

        targetPos = new Vector3(
            Mathf.Round(targetPos.x),
            Mathf.Round(targetPos.y),
            Mathf.Round(targetPos.z)
        );

        player.transform.position = targetPos;

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

    private Vector3 GetOffset()
    {
        return exitDirection switch
        {
            MoveDirection.Forward => Vector3.forward * 2f,
            MoveDirection.Backward => Vector3.back * 2f, 
            MoveDirection.Left => Vector3.left * 2f,
            MoveDirection.Right => Vector3.right * 2f,
            _ => Vector3.zero
        };
    }
}
