using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [field: SerializeField] public SaveData saveData { get; private set; }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "saveData.sawit");

    void Start()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        if (File.Exists(SavePath))
        {
            using (FileStream stream = File.Open(SavePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                saveData = formatter.Deserialize(stream) as SaveData;
            }
        }
        else
        {
            saveData = new SaveData();
        }

        GameManager.instance.isFirstTimePlaying = saveData.isFirstTimePlaying;
        GameManager.instance.isFirstTimeShop = saveData.isFirstTimeShop;
        GameManager.instance.isBgmOn = saveData.isBgmOn;
        GameManager.instance.isSfxOn = saveData.isSfxOn;
        GameManager.instance.isVibrationOn = saveData.isVibrationOn;
        GameManager.instance.ApplyAudioSettings();
    }

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        using (FileStream stream = File.Open(SavePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveData);
        }

        Debug.Log("Game saved at " + SavePath);
    }

    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        
    }

    [ContextMenu("cheat: Unlock All Slimes")]  
    public void UnlockAllSlimes()
    {
        
    }

    [ContextMenu("cheat: Unlock All Stages")]
    /*public void UnlockAllStages()
    {
        saveData.unlockedStages = allStages.Count;
        foreach (var stage in allStages)
        {
            stage.isUnlocked = true;
        }
        SaveGame();
    }*/

    [ContextMenu("cheat: Currency")]
    public void CheatCurrency()
    {
        AddCurrency(999999999);
    }

    [ContextMenu("cheat: unlock Next Stage")]
    /*public void UnlockNextStage()
    {
        if (saveData.unlockedStages < allStages.Count)
        {
            saveData.unlockedStages++;
            LoadUnlockedStage();
        }
        else
        {
            Debug.Log("All stages are already unlocked.");
        }
    }*/

    /*void LoadUnlockedStage()
    {
        for(int i = 0; i < saveData.unlockedStages; i++)
        {
            allStages[i].isUnlocked = true;
        }
    }*/

    public void AddCurrency(int amount)
    {
        saveData.currency += amount;
        SaveGame();
    }

    /*public void UnlockSlime(int slimeID)
    {
        if (!saveData.unlockedSlimeIds.Contains(slimeID))
        {
            saveData.unlockedSlimeIds.Add(slimeID);
            SaveGame();

            var slime = allSlimes.FirstOrDefault(s => s.slimeID == slimeID);
            if (slime != null) slime.isUnlocked = true;
        }
    }*/

    public void UnlockStage(int stage)
    {
        if (stage > saveData.unlockedStages)
        {
            saveData.unlockedStages = stage;
            SaveGame();
        }
    }

    /*public List<SlimeData> GetUnlockedSlimes()
    {
        return allSlimes
            .Where(slime => saveData.unlockedSlimeIds.Contains(slime.slimeID))
            .ToList();
    }*/

    public bool IsSlimeUnlocked(int slimeID)
    {
        return saveData.unlockedSlimeIds.Contains(slimeID);
    }

    public void SetFirstTimePlaying(bool value)
    {
        saveData.isFirstTimePlaying = value;
        GameManager.instance.isFirstTimePlaying = value;
        SaveGame();
    }

    public void SetFirstTimeShop(bool value)
    {
        saveData.isFirstTimeShop = value;
        GameManager.instance.isFirstTimeShop = value;
        SaveGame() ;
    }

    public void SaveSettings()
    {
        saveData.isBgmOn = GameManager.instance.isBgmOn;
        saveData.isSfxOn = GameManager.instance.isSfxOn;
        saveData.isVibrationOn = GameManager.instance.isVibrationOn;
        SaveGame();
    }
}