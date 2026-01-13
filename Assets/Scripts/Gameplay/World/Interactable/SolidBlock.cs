using UnityEngine;

public class SolidBlock : Block
{
    public override void OnInteract(Vector3 position)
    {
        // Solid blocks cannot be interacted with
    }
}
