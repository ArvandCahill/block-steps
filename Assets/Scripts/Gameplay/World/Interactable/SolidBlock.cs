using UnityEngine;

public class SolidBlock : Block, IDraggable
{
    public override void OnInteract(Vector3 position)
    {
        // Solid blocks cannot be interacted with
    }

    public void OnDrag(Vector3 position)
    {
        gameObject.transform.position = position;
    }
}
