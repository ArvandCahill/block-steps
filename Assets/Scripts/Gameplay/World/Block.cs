using UnityEngine;

public class Block : MonoBehaviour 
{
    public Vector3Int position;
    public BlockType type;
    public bool isSolid;
    public bool isWalkable;

    private void OnValidate()
    {
        position = Vector3Int.RoundToInt(transform.position);
    }
}
