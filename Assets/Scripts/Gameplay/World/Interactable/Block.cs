using UnityEngine;

public abstract class Block : Interactable
{
    [SerializeField] private Vector3Int position;
    public BlockType type;
    public bool isWalkable;

    [TextArea] public string debugInfo;

    protected void OnEnable()
    {
        position = base.GetPosition();
        isWalkable = !IsBlockedAbove();

        if (type == BlockType.Start) GameplayManager.instance.startPoint = transform;
        if (type == BlockType.Finish) GameplayManager.instance.finishPoint = this;
        if (type == BlockType.Decoration) isWalkable = false;
    }

    public override void Interact(Vector3 position)
    {
        OnInteract(position);
    }

    public abstract void OnInteract(Vector3 position);

    protected bool IsBlockedAbove()
    {
        RaycastHit hit;

        Vector3 center = transform.position + Vector3.up * 0.5f;
        Vector3 halfExtents = Vector3.one * 0.3f; 
        Vector3 direction = Vector3.up;
        float distance = 0.6f;

        bool isBlocked = Physics.BoxCast(
            center,
            halfExtents,
            direction,
            out hit,
            Quaternion.identity,
            distance
        );

        Debug.DrawRay(
            center,
            direction * distance,
            isBlocked ? Color.red : Color.green,
            5f
        );

        if (isBlocked)
            debugInfo = hit.collider.name;
        else
            debugInfo = "Clear";

        return isBlocked;
    }

}