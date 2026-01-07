using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int currency;
    public List<int> unlockedSlimeIds = new List<int>();
    public int unlockedStages = 1;

    public bool isBgmOn = true;
    public bool isSfxOn = true;
    public bool isVibrationOn = true;

    public bool isFirstTimePlaying = true;
    public bool isFirstTimeShop = true;
}
