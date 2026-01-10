using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ResourcesLoader
{
    public static List<AnimalData> LoadAllAnimalData()
    {
        return Resources.LoadAll<AnimalData>("AnimalData").ToList();
    }

    public static List<LevelData> LoadAllLevelData()
    {
        return Resources.LoadAll<LevelData>("LevelData").ToList();
    }
}
