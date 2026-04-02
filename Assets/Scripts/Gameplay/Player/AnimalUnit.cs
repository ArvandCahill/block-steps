using Unity.Behavior;
using UnityEngine;

public class AnimalUnit : MonoBehaviour
{
    [Header("Core")]
    public AnimalData animalData;
    public Rigidbody rb;
    public MeshFilter Mesh;
    public Transform visualRoot;
    public Animator animator;
    public Coroutine moveRoutine;
    public bool isPlayer = false;

    [Header("Stats")]   
    public int Health;
    public float movementSpeed;

    [Header("AI")]
    [SerializeField] private BehaviorGraphAgent behaviorGraphAgent;
    public GameObject alertIcon;

    [Header("Movement")]
    public bool stopMovement { get; set; } = false;
    public bool isMoving => moveRoutine != null;

    [Header("Platform")]
    private Vector3 externalDelta;

    private void Start()
    {
        Init(animalData);
    }

    public void Init(AnimalData animalData)
    {
        this.animalData = animalData;
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
        GetComponentInChildren<Collider>().enabled = false;
        StopAllCoroutines();
        InputManager.instance.DisableAllMap();
        GameplayManager.instance.DisableAI();
        stopMovement = true;
        animator.SetTrigger("die");
        GameEvents.TriggerLevelFinished(false);
        enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void EnableAI(bool enabled)
    {
        behaviorGraphAgent.enabled = enabled;
    }

    public void AddExternalDelta(Vector3 delta)
    {
        externalDelta += delta; 
    }

    private void FixedUpdate()
    {
        if (externalDelta == Vector3.zero)
            return;

        rb.MovePosition(rb.position + externalDelta);
        externalDelta = Vector3.zero;
    }
}
