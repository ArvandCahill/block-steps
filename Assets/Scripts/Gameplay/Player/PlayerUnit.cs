using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private AnimalData animalData;
    public PlayerController PlayerController;
    public GameObject Mesh;
    public Transform visualRoot;
    public Animator animator;

    [Header("Stats")]   
    public int Health;
    public int movementSpeed;

    public bool isMoving;

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    void Init()
    {
        Mesh = animalData.animalMesh;
        Health = animalData.health;
        movementSpeed = animalData.GetSpeed();
    }
}
