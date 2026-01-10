using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject levelPrefab;
    public int levelNumber;
    public bool isLocked;
    public bool isNightMode;
}
