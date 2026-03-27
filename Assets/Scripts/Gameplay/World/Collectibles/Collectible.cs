using UnityEngine;
public abstract class Collectible : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectibleParticles;

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;
        if(collectibleParticles != null) collectibleParticles.Play();
        gameObject.SetActive(false);
        OnCollected();
    }

    public abstract void OnCollected();
}
