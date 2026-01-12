using UnityEngine;

public abstract class Block : Interactable
{
    public Vector3Int position;
    public BlockType type;
    public bool isSolid;
    public bool isWalkable;

    protected void OnValidate()
    {
        position = Vector3Int.RoundToInt(transform.position);
    }

    public override void Interact(Vector3 position)
    {
        OnInteract(position);
    }

    public abstract void OnInteract(Vector3 position);
}