using UnityEngine;
using System.Collections.Generic;

public class CollectiblesPuzzleChecker : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private List<GameObject> portals;

    private int collectedCount;
    private int totalCount;

    private void Awake()
    {
        var all = GetComponentsInChildren<Collectible>(true);

        totalCount = all.Length;

        Debug.Log($"[Checker] Total collectibles found: {totalCount}");
    }

    private void OnEnable()
    {
        GameEvents.OnCollectiblePicked += OnCollectiblesPicked;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectiblePicked -= OnCollectiblesPicked;
    }

    private void Start()
    {
        foreach (var portal in portals)
        {
            if (portal != null)
                portal.SetActive(false);
        }

        Debug.Log("[Checker] Portals disabled at start");
    }

    private void OnCollectiblesPicked()
    {
        collectedCount++;

        Debug.Log($"[Checker] Collected: {collectedCount}/{totalCount}");

        if (collectedCount >= totalCount)
        {
            ActivatePortals();
        }
    }

    private void ActivatePortals()
    {
        foreach (var portal in portals)
        {
            if (portal != null)
                portal.SetActive(true);
        }

        Debug.Log("[Checker] All portals activated!");
    }
}