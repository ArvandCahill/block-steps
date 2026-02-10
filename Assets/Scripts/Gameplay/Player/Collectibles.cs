using UnityEngine;
public class Collectibles : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectibleParticles;
    [SerializeField] private GameObject mesh;

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;

        if (mesh == null)
        {
            gameObject.SetActive(false);
        }

        collectibleParticles.Play();
        GameEvents.TriggerCollectiblePicked();
        mesh.SetActive(false);
    }
}
