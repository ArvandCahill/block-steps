using UnityEngine;
public class Collectibles : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;

        GameEvents.TriggerCollectiblePicked();
        gameObject.SetActive(false);
    }
}
