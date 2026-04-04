using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private AnimalUnit unit;
    private Coroutine moveRoutine;
    private PathFinding pathFinding;

    public Iinteractable currentInteractable;   
    public IDraggable currentDraggable;

    private Vector2 startPos;
    private Vector2 currentPos;

    [SerializeField] private InputState inputState = InputState.idle;
    [SerializeField] private float dragThreshold = 10f;

    private InputManager inputManager;

    private void OnEnable()
    {
        inputManager = InputManager.instance;

        inputManager.inputAction.Player.Enable();
        inputManager.inputAction.Player.Press.started += OnPressStarted;
        inputManager.inputAction.Player.Drag.performed += OnDrag;
        inputManager.inputAction.Player.Press.canceled += OnPressReleased;

        GameEvents.OnLevelFinished += disable;
    }

    private void OnDisable()
    {
        inputManager.inputAction.Player.Press.started -= OnPressStarted;
        inputManager.inputAction.Player.Drag.performed -= OnDrag;
        inputManager.inputAction.Player.Press.canceled -= OnPressReleased;

        inputManager.inputAction.Player.Disable();
        inputManager.inputAction.Camera.Disable();
    }

    void Start()
    {
        unit = GetComponent<AnimalUnit>();
        moveRoutine = unit.moveRoutine;
        pathFinding = PathFinding.instance;
        mainCamera = Camera.main;
    }

    void OnPressStarted(InputAction.CallbackContext context)
    {
        currentInteractable = null;
        currentDraggable = null;

        startPos = inputManager.inputAction.Player.Drag.ReadValue<Vector2>();
        if (currentDraggable != null) inputManager.cameraMap.Disable();
        StartRaycast(startPos);

        inputState = InputState.pressed;
    }

    void OnDrag(InputAction.CallbackContext context)
    {
        currentPos = context.ReadValue<Vector2>();

        if (inputState == InputState.pressed && Vector2.Distance(currentPos, startPos) > dragThreshold)
        {
            inputState = InputState.dragging;
        }

        if(inputState == InputState.dragging) Drag(currentPos);
    }

    void OnPressReleased(InputAction.CallbackContext context)
    {
        currentPos = context.ReadValue<Vector2>();

        if (inputState == InputState.dragging)
        {
            currentDraggable?.OnDragEnd();
            inputState = InputState.idle;
            inputManager.cameraMap.Enable();
            return;
        }

        if (currentInteractable != null)
        {
            Click(currentPos);
        }

        inputState = InputState.idle;
    }

    void StartRaycast(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        hit.transform.TryGetComponent(out currentInteractable);
        hit.transform.TryGetComponent(out currentDraggable);
        /*Debug.Log("Hit: " + hit.transform.name);*/
    }

    void Click(Vector2 screenPos)
    {
        if (currentInteractable == null) return;
        currentInteractable.Interact(currentInteractable.GetPosition());
        pathFinding.Move(unit, currentInteractable.GetPosition());
    }

    void Drag(Vector2 screenPos)
    {
        currentDraggable?.OnDrag(screenPos);
    }

    void disable(bool isWinning)
    {
        /*enabled = false;*/
    }
}