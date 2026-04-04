using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    private AnimalUnit unit;

    private void Awake()
    {
        unit = GetComponentInParent<AnimalUnit>();
    }

    public void PlayFootstep()
    {
        if (unit.isPlayer && !unit.isShopAi)
            GameManager.instance.audioManager.PlayRandomSFX("Footstep1", "Footstep2");
            
    }
}
