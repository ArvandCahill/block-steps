using UnityEngine;

public class AnimalUnit : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private AnimalData animalData;
    public Rigidbody rb;
    public PlayerController PlayerController;
    public MeshFilter Mesh;
    public Transform visualRoot;
    public Animator animator;
    public Coroutine moveRoutine;
    public bool isPlayer = false;

    [Header("Stats")]   
    public int Health;
    public float movementSpeed;

    public bool stopMovement = false;

    public bool isMoving => moveRoutine != null;

    void Start()
    {
        Init();
    }

    void Init()
    {
        Mesh.mesh = animalData.animalMesh;
        Health = animalData.health;
        movementSpeed = animalData.GetSpeed();
        animator.SetFloat("speed", movementSpeed);
    }

    public Quaternion GetRotationFromDirection(Vector3Int dir)
    {
        if (dir == Vector3Int.forward)
            return Quaternion.Euler(0, 0, 0);

        if (dir == Vector3Int.back)
            return Quaternion.Euler(0, 180, 0);

        if (dir == Vector3Int.right)
            return Quaternion.Euler(0, 90, 0);

        if (dir == Vector3Int.left)
            return Quaternion.Euler(0, -90, 0);

        return visualRoot.transform.rotation;
    }

    public void Die()
    {
        stopMovement = true;
        animator.SetTrigger("die");
        GameplayManager.instance.DisableAI();
        GameEvents.TriggerPlayerFinished(false, GameplayManager.instance.collectiblesCollected);
        enabled = false;
    }
}
