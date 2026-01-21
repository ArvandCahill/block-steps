using UnityEngine;
public class Collectibles : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;

        GameplayManager.instance.OnCollectiblesPicked();
        gameObject.SetActive(false);
    }
}
