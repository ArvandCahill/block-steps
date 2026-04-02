using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DraggableBlock : Block, IDraggable
{
    [Header("Drag Settings")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask obstacleMask;

    private Camera cam;

    private bool dragging;
    private Vector3 dragStartWorld;
    private Vector3 dragStartBlockPos;
    private Vector3 initialBlockPos;
    private Vector3 dragAxis;

    private Transform passenger;
    private Vector3 lastPosition;

    protected void Awake()
    {
        cam = Camera.main;
        initialBlockPos = transform.position;
        lastPosition = transform.position;
    }

    public void OnDrag(Vector3 screenPos)
    {
        if (!dragging)
            BeginDrag(screenPos);

        UpdateDrag(screenPos);
    }

    private void BeginDrag(Vector3 screenPos)
    {
        PathFinding.instance?.UnregisterBlock(this);

        dragging = true;
        isWalkable = false;

        dragStartBlockPos = transform.position;
        dragStartWorld = ScreenToWorldOnPlane(screenPos, dragStartBlockPos.y);

        dragAxis = Vector3.zero;
    }

    private void UpdateDrag(Vector3 screenPos)
    {
        Vector3 worldPos = ScreenToWorldOnPlane(screenPos, dragStartBlockPos.y);
        Vector3 delta = worldPos - dragStartWorld;

        if (dragAxis == Vector3.zero)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
                dragAxis = delta.x >= 0 ? Vector3.right : Vector3.left;
            else
                dragAxis = delta.z >= 0 ? Vector3.forward : Vector3.back;
        }

        Vector3 rawTarget =
            dragStartBlockPos + dragAxis * Vector3.Dot(delta, dragAxis);

        rawTarget = ClampToInitial(rawTarget);

        Vector3 nextPos = Vector3.MoveTowards(
            transform.position,
            rawTarget,
            moveSpeed * Time.deltaTime
        );

        Vector3 move = nextPos - transform.position;
        if (move.sqrMagnitude < 0.0001f)
            return;

        if (IsBlocked(move))
            return;

        ApplyMovement(nextPos);
    }


    private void ApplyMovement(Vector3 targetPos)
    {
        Vector3 delta = targetPos - transform.position;

        transform.position = targetPos;

        if (passenger != null)
            passenger.position += delta;

        lastPosition = transform.position;
    }

    public void OnDragEnd()
    {
        dragging = false;
        isWalkable = true;

        Vector3 snapped = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
        );

        ApplyMovement(snapped);

        PathFinding.instance?.RegisterBlock(this);
    }

    private bool IsBlocked(Vector3 move)
    {
        return Physics.BoxCast(
            transform.position,
            Vector3.one * 0.45f,
            move.normalized,
            Quaternion.identity,
            move.magnitude,
            obstacleMask
        );
    }

    private Vector3 ClampToInitial(Vector3 target)
    {
        return new Vector3(
            Mathf.Clamp(target.x, initialBlockPos.x - maxDistance, initialBlockPos.x + maxDistance),
            transform.position.y,
            Mathf.Clamp(target.z, initialBlockPos.z - maxDistance, initialBlockPos.z + maxDistance)
        );
    }

    private Vector3 ScreenToWorldOnPlane(Vector3 screenPos, float y)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0, y, 0));
        plane.Raycast(ray, out float enter);
        return ray.GetPoint(enter);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag("Player"))
            return;

        passenger = other.transform;
        other.transform.SetParent(transform, true);
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.collider.CompareTag("Player"))

        passenger = null;
        other.transform.SetParent(null, true);
    }

    public override void OnInteract(Vector3 position) { }
}
