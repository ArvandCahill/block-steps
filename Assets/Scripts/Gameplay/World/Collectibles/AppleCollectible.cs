public class AppleCollectible : Collectible
{
    private bool collected;

    private void Awake()
    {
        GameplayManager.instance.appleCollectibles.Add(this);
    }

    public override void OnCollected()
    {
        if (collected)
            return;

        collected = true;

        GameEvents.TriggerCollectiblePicked();

        gameObject.SetActive(false);
    }
}
