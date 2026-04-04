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

        GameManager.instance.audioManager.PlayRandomSFX("Collectibles1", "Collectibles2");
        GameEvents.TriggerCollectiblePicked();

        gameObject.SetActive(false);
    }
}
