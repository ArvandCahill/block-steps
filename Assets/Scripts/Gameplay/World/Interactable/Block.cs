using UnityEngine;

public abstract class Block : Interactable
{
    public Vector3Int position;
    public BlockType type;
    public bool isWalkable;

    protected void OnEnable()
    {
        position = Vector3Int.RoundToInt(transform.position);
        isWalkable = !IsBlockedAbove();

/*        if (!isWalkable || type == BlockType.Decoration) GetComponent<BoxCollider>().enabled = false;*/
    }

    public override void Interact(Vector3 position)
    {
        OnInteract(position);
    }

    public abstract void OnInteract(Vector3 position);

    protected bool IsBlockedAbove()
    {
        return Physics.Raycast(transform.position, Vector3.up, 1.1f);
    }
}