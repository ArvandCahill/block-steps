using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [HideInInspector] public int selectedAnimalId = 0;

    [Header("Player Progress")]
    public int currency = 0;
    public int unlockedLevels = 0;
    public List<LevelProgress> levelProgress = new List<LevelProgress>();
    public List<int> unlockedAnimalIds = new List<int>();

    [Header("Settings")]
    public bool isBgmOn = true;
    public bool isSfxOn = true;

    public bool isFirstTimePlaying = true;
    public bool isFirstTimeNightMode = true;
}
