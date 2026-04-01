public class AppleCollectible : Collectible
{
    private bool collected;

    public override void OnCollected()
    {
        if (collected)
            return;

        collected = true;

        GameEvents.TriggerCollectiblePicked();

        gameObject.SetActive(false);
    }
}
