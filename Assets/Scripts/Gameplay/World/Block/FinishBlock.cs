using DG.Tweening.Core.Easing;
using UnityEngine;

public class FinishBlock
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        SaveManager.instance.UnlockNextStage();
        GameEvents.TriggerLevelFinished(true);
    }
}
