using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Properties")]
    public GameObject levelPrefab;
    public Sprite levelImage;
    public int levelNumber;
    public string levelName;

    [Header("Collectibles")]
    [Range(1, 5)] public int maxCollectibles;

    [Header("Environment")]
    public bool isNightMode;
    public SkyboxType skyboxType;
}
