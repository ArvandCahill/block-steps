using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class AutoMoveBlock : Block
{
    [Header("Movement")]
    [SerializeField] private MoveDirection moveDirection;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Behavior")]
    [SerializeField] private float toggleCooldown = 2f;
    [SerializeField] private float enterDelay = 2f;
    [SerializeField] private bool lockYAxis = true;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 currentTarget;
    private Vector3 playerOffset;

    private AnimalUnit unitOnBlock;
    private MoveState state = MoveState.AtStart;

    public bool isMoving;
    public bool canToggle = true;

    private void Awake()
    {
        startPosition = transform.position;
        targetPosition = CalculateTargetPosition();
        currentTarget = startPosition;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        Vector3 newPos = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            moveSpeed * Time.deltaTime
        );

        ApplyMovement(newPos);
        Debug.Log("Player is Moving");

        if (Vector3.Distance(newPos, currentTarget) < 0.01f)
        {
            Vector3 snapped = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
        );

            isMoving = false;

            if (unitOnBlock != null)
                unitOnBlock.stopMovement = false;

            ApplyMovement(snapped);
            PathFinding.instance?.RegisterBlock(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag("Player"))
            return;

        unitOnBlock = other.collider.GetComponent<AnimalUnit>();
        

        if (unitOnBlock == null)
        {
            return;
        }

        playerOffset = other.transform.position - transform.position;

        other.transform.SetParent(transform, true);
        Debug.Log("Player inside autoMove parents");

        if (!canToggle || isMoving)
            return;

        unitOnBlock.stopMovement = true;
        PathFinding.instance.EnableMarker(unitOnBlock, Vector3.zero, false, false);

        StartCoroutine(DelayedMove());
    }

    private IEnumerator DelayedMove()
    {
        yield return new WaitForSeconds(enterDelay);
        ToggleMove();
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.collider.CompareTag("Player"))
            return;

        if (isMoving)
            return;

        unitOnBlock = null;

        other.transform.SetParent(null, true);
        Debug.Log("Player exit AutoMove parent");

        StartCoroutine(ToggleCooldownRoutine());
    }

    private void OnCollisionStay(Collision other)
    {
        if (!other.collider.CompareTag("Player"))
            return;

        if (other.transform.parent != transform)
            other.transform.SetParent(transform, true);
    }

    private void ApplyMovement(Vector3 targetPos)
    {
        transform.position = targetPos;
    }

    private void ToggleMove()
    {
        canToggle = false;

        state = state == MoveState.AtStart
            ? MoveState.AtTarget
            : MoveState.AtStart;

        currentTarget = state == MoveState.AtTarget
            ? targetPosition
            : startPosition;

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

    public override void OnInteract(Vector3 position)
    {
        
    }
}
