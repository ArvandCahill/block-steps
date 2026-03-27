using System;
using UnityEngine;

[Serializable]
public class LevelProgress 
{
    public int collectiblesCollected = 0;
    public bool isUnlocked = false;
    public bool isCompleted = false;

    public void AddCollectiblesCollected(LevelData levelData)
    {
        if (collectiblesCollected == levelData.maxCollectibles) return;

        collectiblesCollected++;
    }

    public void MarkAsCompleted()
    {
        isCompleted = true;
        Debug.Log("Level marked as completed");
    }
}