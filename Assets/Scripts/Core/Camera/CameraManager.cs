using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private Vector2 _delta;

    [SerializeField] private Camera _cam;

    private bool _isMoving;
    private bool _isRotating;

    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _zoomSpeed;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _minZoom = 4f;
    private float _maxZoom = 20f;

    private float _lockedXrotation;

    private Vector2 _touch0;
    private Vector2 _touch1;

    private float _prevDistance;

    private void Awake()
    {
        _lockedXrotation = transform.rotation.eulerAngles.x;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);
    }

    #region Input Callback

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        _isMoving = context.started || context.performed;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        _isRotating = context.started || context.performed;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        float zoomDelta = context.ReadValue <float>();
        ApplyZoom(zoomDelta);
    }

    #endregion

    private void LateUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (!_isMoving)
            return;

        Vector3 right = transform.right * _moveInput.x;
        Vector3 up = transform.up * _moveInput.y;

        Vector3 movement = (right + up) * _movementSpeed * Time.deltaTime;
        transform.position += movement;
    }

    private void HandleRotation()
    {
        if (!_isRotating)
            return;

        float yawDelta = _lookInput.x * _rotationSpeed * Time.deltaTime;
        float newY = transform.eulerAngles.y + yawDelta;

        transform.rotation = Quaternion.Euler(_lockedXrotation, newY, 0f);
    }

    private void ApplyZoom(float _zoomDelta)
    {
        if (Mathf.Approximately(_zoomDelta, 0f))
            return;

        _cam.orthographicSize -= _zoomDelta * _zoomSpeed * Time.deltaTime;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);
    }
}
