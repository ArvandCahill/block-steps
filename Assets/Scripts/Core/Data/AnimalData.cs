using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Data/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("Animal Properties")]
    public int animalID;
    public string animalName;
    public Sprite animalImage;
    public Mesh animalMesh;

    [Header("Animal Stats")]
    public int health;
    public MovementSpeed movementSpeed;

    [Header("Shop Properties")]
    public int price;
    public bool isUnlockedAtStart;
    private bool isUnlocked;

    public float GetSpeed()
    {
        return movementSpeed switch
        {
            MovementSpeed.Slow => 0.8f,
            MovementSpeed.Normal => 1.2f,
            MovementSpeed.Fast => 1.5f,
            _ => 1
        };
    }

    public bool CheckMilestone(int currency)
    {
        isUnlocked = currency >= price;
        return isUnlocked;
    }
}
