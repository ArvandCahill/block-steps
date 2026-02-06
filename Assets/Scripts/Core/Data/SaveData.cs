using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int selectedAnimalId = 0;
    public int currency = 0;
    public int unlockedLevels = 1;
    public List<int> unlockedAnimalIds = new List<int>();

    public bool isBgmOn = true;
    public bool isSfxOn = true;

    public bool isFirstTimePlaying = true;
}
