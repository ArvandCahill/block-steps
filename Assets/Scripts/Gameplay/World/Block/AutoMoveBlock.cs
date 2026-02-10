using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AutoMoveBlock : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private MoveDirection moveDirection;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Behavior")]
    [SerializeField] private float toggleCooldown = 2f;
    [SerializeField] private bool lockYAxis = true;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 currentTarget;

    private AnimalUnit unitOnBlock;
    private MoveState state = MoveState.AtStart;

    private bool isMoving;
    private bool canToggle = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        startPosition = transform.position;
        targetPosition = CalculateTargetPosition();
        currentTarget = startPosition;
    }

    private void FixedUpdate()
    {
        if (!isMoving)
            return;


        Vector3 newPos = Vector3.MoveTowards(rb.position, currentTarget, moveSpeed * Time.deltaTime);

        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, currentTarget) < 0.01f)
        {
            isMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        unitOnBlock = other.GetComponent<AnimalUnit>();

        if (!canToggle || isMoving)
            return;

        AutoMove();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        unitOnBlock = null;
        StartCoroutine(ToggleCooldownRoutine());
    }

    private void AutoMove()
    {
        canToggle = false;

        state = state == MoveState.AtStart ? MoveState.AtTarget : MoveState.AtStart;

        currentTarget = state == MoveState.AtTarget ? targetPosition : startPosition;

        isMoving = true;
    }

    IEnumerator ToggleCooldownRoutine()
    {
        yield return new WaitForSeconds(toggleCooldown);
        canToggle = true;
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 offset = Vector3.zero;

        switch (moveDirection)
        {
            case MoveDirection.Forward:
                offset = Vector3.forward;
                break;
            case MoveDirection.Backward:
                offset = Vector3.back;
                break;
            case MoveDirection.Left:
                offset = Vector3.left;
                break;
            case MoveDirection.Right:
                offset = Vector3.right;
                break;
        }

        Vector3 targetPos = startPosition + offset * moveDistance;

        if (lockYAxis)
            targetPos.y = startPosition.y;

        return targetPos;
    }
}
