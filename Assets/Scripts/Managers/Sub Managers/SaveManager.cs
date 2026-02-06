using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;

public class SaveManager : MonoBehaviour
{
    [field: SerializeField] public SaveData saveData { get; private set; }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "saveData.sawit");

    private GameManager GameManager => GameManager.instance;

    private List<AnimalData> AllAnimalData => GameManager.allAnimalData;

    private List<LevelData> AllLevelData => GameManager.allLevelData;


    public void LoadSaveData()
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

        GameManager.isBgmOn = saveData.isBgmOn;
        GameManager.isSfxOn = saveData.isSfxOn;
        GameManager.Currency = saveData.currency;
        GameManager.isFirstTimePlaying = saveData.isFirstTimePlaying;

        LoadUnlockedAnimals();
        LoadUnlockedStage();
    }

    void LoadUnlockedAnimals()
    {
        foreach (var animal in AllAnimalData)
        {
            animal.CheckMilestone(GameManager.Currency);

            if (animal.animalID == saveData.selectedAnimalId)
            {
                GameManager.SetSelectedAnimal(animal);
            }
        }
    }

    /*public void UnlockAnimal(int animalID)
    {
        if (!saveData.unlockedAnimalIds.Contains(animalID))
        {
            saveData.unlockedAnimalIds.Add(animalID);
            SaveGame(saveData);

            var animal = AllAnimalData.FirstOrDefault(s => s.animalID == animalID);
            if (animal != null) animal.isUnlocked = true;
        }
    }*/

    public void SetSelectedAnimalId(int animalID)
    {
        saveData.selectedAnimalId = animalID;
        SaveGame(saveData);
    }

    [ContextMenu("Save Game")]
    public void SaveGame(SaveData saveData)
    {
        using (FileStream stream = File.Open(SavePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveData);
        }

        Debug.Log("Game saved at " + SavePath);
    }

    [ContextMenu("Reset Save")]
    private void ResetSaveData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save data reset.");
        }
        else
        {
            Debug.Log("No save data found to reset.");
        }
    }

    [ContextMenu("cheat: Unlock All Animals")]
    public void UnlockAllAnimals()
    {

    }

    [ContextMenu("cheat: Unlock All Level")]
    public void UnlockAllLevels()
    {
        saveData.unlockedLevels = AllLevelData.Count;
        LoadUnlockedStage();
    }

    [ContextMenu("cheat: Currency")]
    public void CheatCurrency()
    {
        AddCurrency(999999999);
    }

    [ContextMenu("cheat: unlock Next Level")]
    public void UnlockNextStage()
    {
        if (saveData.unlockedLevels < AllLevelData.Count)
        {
            saveData.unlockedLevels++;
            LoadUnlockedStage();
        }
        else
        {
            Debug.Log("All stages are already unlocked.");
        }
    }

    void LoadUnlockedStage()
    {
        for(int i = 0; i < saveData.unlockedLevels; i++)
        {
            AllLevelData[i].isUnlocked = true;
        }
    }

    public void AddCurrency(int amount)
    {
        saveData.currency += amount;
        SaveGame(saveData);
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

    public void UnlockLevel(int stage)
    {
        if (stage > saveData.unlockedLevels)
        {
            saveData.unlockedLevels = stage;
            SaveGame(saveData);
        }
    }

    /*public List<SlimeData> GetUnlockedSlimes()
    {
        return allSlimes
            .Where(slime => saveData.unlockedSlimeIds.Contains(slime.slimeID))
            .ToList();
    }*/

    public bool IsAnimalUnlocked(int slimeID)
    {
        return saveData.unlockedAnimalIds.Contains(slimeID);
    }

    public void SetFirstTimePlaying(bool value)
    {
        saveData.isFirstTimePlaying = value;
        GameManager.instance.isFirstTimePlaying = value;
        SaveGame(saveData);
    }

    public void SaveSettings()
    {
        saveData.isBgmOn = GameManager.instance.isBgmOn;
        saveData.isSfxOn = GameManager.instance.isSfxOn;
        SaveGame(saveData);
    }

}