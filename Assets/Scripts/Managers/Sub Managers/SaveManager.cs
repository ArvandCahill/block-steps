using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; private set; }

    [field: SerializeField] public SaveData saveData { get; private set; }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "saveData.sawit");

    private GameManager GameManager => GameManager.instance;

    private List<AnimalData> AllAnimalData => GameManager.allAnimalData;

    private List<LevelData> AllLevelData => GameManager.allLevelData;

    #region Properties
    public int Currency
    {
        get { return saveData.currency; }
        set
        {
            saveData.currency = value;
            SaveGame();
        }
    }

    public int UnlockedLevels
    {
        get { return saveData.unlockedLevels; }
        set
        {
            if (saveData.unlockedLevels >= value) return; 
            saveData.unlockedLevels = value;
            Debug.Log("Unlocked levels updated: " + saveData.unlockedLevels);
            SaveGame();
        }
    }

    public bool IsBgmOn
    {
        get { return saveData.isBgmOn; }
        set
        {
            if (value == saveData.isBgmOn) return;

            saveData.isBgmOn = value;
            GameManager.audioManager?.SetBgmActive(IsBgmOn);
            SaveGame();
        }
    }

    public bool IsSfxOn
    {
        get { return saveData.isSfxOn; }
        set
        {
            if (value == saveData.isSfxOn) return;

            saveData.isSfxOn = value;
            GameManager.audioManager?.SetSfxActive(IsSfxOn);
            SaveGame();
        }
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

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

            foreach (var level in AllLevelData)
            {
                saveData.levelProgress.Add(new LevelProgress());
            }
        }

        LoadUnlockedAnimals();
    }

    void LoadUnlockedAnimals()
    {
        foreach (var animal in AllAnimalData)
        {
            animal.CheckMilestone(Currency);

            if (animal.animalID == saveData.selectedAnimalId)
            {
                GameManager.SetSelectedAnimal(animal);
            }
        }
    }

    public void SetSelectedAnimalId(int animalID)
    {
        saveData.selectedAnimalId = animalID;
        SaveGame();
    }

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        using (FileStream stream = File.Open(SavePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveData);
        }

        /*Debug.Log("Game saved at " + SavePath);*/
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

    [ContextMenu("cheat: Unlock All Level")]
    public void UnlockAllLevels()
    {
        saveData.unlockedLevels = AllLevelData.Count;
        SaveGame();
    }

    [ContextMenu("cheat: Currency")]
    public void CheatCurrency()
    {
        AddCurrency(99);
    }

    [ContextMenu("cheat: unlock Next Level")]
    public void UnlockNextStage()
    {
        if (saveData.unlockedLevels < AllLevelData.Count)
        {
            saveData.unlockedLevels++;
        }
        else
        {
            Debug.Log("All stages are already unlocked.");
        }
    }

    public void AddCurrency(int amount)
    {
        Debug.Log("Adding " + amount + " currency.");
        Currency += amount;
        SaveGame();
    }

    public bool IsAnimalUnlocked(int slimeID)
    {
        return saveData.unlockedAnimalIds.Contains(slimeID);
    }

    public void SetFirstTimePlaying(bool value)
    {
        saveData.isFirstTimePlaying = value;
        SaveGame();
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= saveData.unlockedLevels;
    }

    public LevelProgress GetLevelProgress(int levelNumber)
    {
        return saveData.levelProgress[levelNumber];
    }
}