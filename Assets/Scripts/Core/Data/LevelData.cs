using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject levelPrefab;
    public int levelNumber;

    public int collectiblesCollected;
    public int maxCollectibles;

    public bool isLocked;
    public bool isNightMode;

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
        return collectiblesCollected >= maxCollectibles;
    }
}
