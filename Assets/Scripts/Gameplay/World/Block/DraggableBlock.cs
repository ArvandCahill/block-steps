using UnityEngine;
using System.Collections;
using static UnityEngine.UI.Image;

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
    private Vector3 velocity;

    private Transform passenger;
    private Vector3 lastPosition;

    private Coroutine snapRoutine;
    

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
        if (snapRoutine != null)
        {
            StopCoroutine(snapRoutine);
            snapRoutine = null;
        }
        PathFinding.instance?.UnregisterBlock(this);

        dragging = true;
        isWalkable = false;

        dragStartBlockPos = transform.position;
        dragStartWorld = ScreenToWorldOnPlane(screenPos, dragStartBlockPos.y);
        SetBlockBelowWalkability(true);

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

        Vector3 nextPos = Vector3.SmoothDamp(
            transform.position,
            rawTarget,
            ref velocity,
            0.5f
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

        snapRoutine = StartCoroutine(Snap(snapped));
    }

    private IEnumerator Snap(Vector3 target)
    {
        Vector3 velocity = Vector3.zero;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                target,
                ref velocity,
                0.3f 
            );

            yield return null;
        }

        transform.position = target;

        PathFinding.instance?.RegisterBlock(this);
        SetBlockBelowWalkability(false);
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
        Debug.Log("Collision detected");
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.down) > 0.1f)
            {
                Debug.DrawRay(contact.point, contact.normal * 200f, Color.green, 2f);
                Debug.Log("Passenger detected");
                passenger = other.transform;
                other.transform.SetParent(transform, true);
                return;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            passenger = null;
            other.transform.SetParent(null);
        }

    }

    private void SetBlockBelowWalkability(bool walkable)
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 3f))
        {
            if (hit.collider.gameObject != gameObject)
            {
                Block blockBelow = hit.collider.GetComponent<Block>();
                blockBelow.isWalkable = walkable;
            }
        }
    }

    public override void OnInteract(Vector3 position) { }
}
