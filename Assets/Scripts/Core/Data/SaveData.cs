using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int selectedAnimalId = 0;
    public int currency;
    public List<int> unlockedAnimalIds = new List<int>();
    public int unlockedStages = 1;

    public bool isBgmOn = true;
    public bool isSfxOn = true;

    public bool isFirstTimePlaying = true;
}
