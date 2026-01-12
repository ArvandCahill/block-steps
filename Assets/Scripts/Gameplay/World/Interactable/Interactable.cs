using UnityEngine;

public abstract class Interactable : MonoBehaviour, Iinteractable
{
    public abstract void Interact(Vector3 position);

    public Vector3Int GetPosition()
    {
        Vector3 pos = transform.position;
        return new Vector3Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z)
        );
    }
}
