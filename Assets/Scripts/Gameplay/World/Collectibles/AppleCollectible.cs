public class AppleCollectible : Collectible
{
    private void Start()
    {
        GameplayManager.instance.appleCollectibles.Add(this);
    }

    public override void OnCollected()
    {
        GameEvents.TriggerCollectiblePicked();
    }
}
