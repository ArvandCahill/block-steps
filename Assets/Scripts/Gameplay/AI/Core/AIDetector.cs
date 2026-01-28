using UnityEngine;

public class AIDetector : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Vector3 detectorSize;
    [SerializeField] private float detectionRadius = 5f;
    
    public AnimalUnit UpdateRangeDetector()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, playerMask);

        if (colliders.Length > 0)
        {
            return colliders[0].gameObject.GetComponentInParent<AnimalUnit>();
        }

        return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AnimalUnit player = collision.collider.GetComponentInParent<AnimalUnit>();

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < 1f)
        {
            player.Die();
        }
    }
}