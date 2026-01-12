using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private AnimalUnit unit;
    private Coroutine moveRoutine;
    private PathFinding pathFinding;
    public Iinteractable currentInteractable;   
    public IDraggable currentDraggable;

    private Vector2 inputPos;
    [SerializeField] private InputState inputState = InputState.idle;
    [SerializeField] private float dragThreshold = 10f;

    void Start()
    {
        unit = GetComponent<AnimalUnit>();
        moveRoutine = unit.moveRoutine;
        pathFinding = PathFinding.instance;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Pointer.current == null)
            return;

        switch (inputState)
        {
            case InputState.idle:
                HandleIdle();
                break;
            case InputState.dragging:
                Handleragging();
                break;
            case InputState.pressed:
                HandlePressed();
                break;
        }
    }

    void HandleIdle()
    {
        if (Pointer.current.press.wasPressedThisFrame)
        {
            inputPos = Pointer.current.position.ReadValue();
            inputState = InputState.pressed;
            StartRaycast(inputPos);
        }
    }

    void Handleragging()
    {
        Vector2 currentPos = Pointer.current.position.ReadValue();
        Drag(currentPos);

        if (Pointer.current.press.wasReleasedThisFrame)
        {
            inputState = InputState.idle;
            currentInteractable = null;
        }
    }

    void HandlePressed()
    {
        Vector2 currentPos = Pointer.current.position.ReadValue();

        if (Pointer.current.press.isPressed)
        {
            if (Vector2.Distance(currentPos, inputPos) > dragThreshold)
            {
                inputState = InputState.dragging;
                return;
            }
        }

        if (Pointer.current.press.wasReleasedThisFrame && inputState != InputState.dragging)
        {
            inputState = InputState.idle;
            Click(currentPos);
        }

    }

    void StartRaycast(Vector2 screenPos)
    {
        currentInteractable = null;
        currentDraggable = null;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 1f);

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        hit.transform.TryGetComponent(out currentInteractable);
        hit.transform.TryGetComponent(out currentDraggable);
    }

    void Click(Vector2 screenPos)
    {
        if (currentInteractable == null) return;
        currentInteractable.Interact(currentInteractable.GetPosition());
        PathFinding.instance.Move(unit, currentInteractable.GetPosition());
    }

    void Drag(Vector2 screenPos)
    {
        if(currentDraggable != null) currentDraggable.OnDrag(screenPos);
        else Debug.Log("Camera Movement");
    }
}