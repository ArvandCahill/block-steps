using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Data/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("Animal Properties")]
    public int animalID;
    public string animalName;
    public GameObject animalMesh;

    [Header("Animal Stats")]
    public int health;
    public MovementSpeed movementSpeed;

    [Header("Shop Properties")]
    public int price;
    public bool isUnlockedAtStart;
    public bool isUnlocked;

    public int GetSpeed()
    {
        return movementSpeed switch
        {
            MovementSpeed.Normal => 1,
            MovementSpeed.Fast => 2,
            _ => 1
        };
    }
}
