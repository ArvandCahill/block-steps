using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Properties")]
    public GameObject levelPrefab;
    public Sprite levelImage;
    public int levelNumber;
    public string levelName;

    [Header("Level Data")]
    [Range(1, 5)] public int maxCollectibles;
    public bool isUnlocked;
    public int collectiblesCollected;
    public bool isLevelFinished = false;

    [Header("Environment")]
    public bool isNightMode;
    public SkyboxType skyboxType;

    public void ResetLevel()
    {
        collectiblesCollected = 0;
    }

    public void CollectiblesCollected()
    {
        collectiblesCollected++;
        Debug.Log("Collectibles : " + collectiblesCollected);
    }

    public bool IsFinish()
    {
        bool isFinish = collectiblesCollected >= maxCollectibles;
        isLevelFinished = isFinish;
        return isFinish;
    }
}
