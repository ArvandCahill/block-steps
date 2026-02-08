using UnityEngine;

public class FinishBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        GameManager.instance.saveManager.UnlockNextStage();
        StartCoroutine(GameEvents.TriggerPlayerFinished(true, GameplayManager.instance.collectiblesCollected));
    }
}
