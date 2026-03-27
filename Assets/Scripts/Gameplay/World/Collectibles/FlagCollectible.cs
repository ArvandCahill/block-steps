using UnityEngine;

public class FlagCollectible : Collectible
{
    public override void OnCollected()
    {
        GameEvents.TriggerLevelFinished(true);
    }
}
