using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private Vector2 _delta;

    [SerializeField] private Camera _cam;

    private bool _isMoving;
    private bool _isRotating;
    private bool _isZooming;

    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomSpeedMouse;
    [SerializeField] private float _zoomSpeedMobile;

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
        if (context.control.device is Mouse)
        {
            float scroll = context.ReadValue<float>();
            ApplyZoom(scroll * _zoomSpeedMouse);
        }
    }

    public void OnTouch0(InputAction.CallbackContext context)
    {
        _touch0 = context.ReadValue<Vector2>();
        TryPinchZoom();
    }

    public void OnTouch1(InputAction.CallbackContext context)
    {
        _touch1 = context.ReadValue<Vector2>();
        TryPinchZoom();
    }

    private void TryPinchZoom()
    {
        if (_touch0 == Vector2.zero || _touch1 == Vector2.zero)
        {
            _prevDistance = 0f;
            _isZooming = false;
            return;
        }

        float _currentDistance = Vector2.Distance(_touch0, _touch1);

        if (!_isZooming)
        {
            _prevDistance = _currentDistance;
            _isZooming = true;
            return;
        }

        float delta = _currentDistance - _prevDistance;
        _prevDistance = _currentDistance;

        ApplyZoom(delta * _zoomSpeedMobile);
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

    private void ApplyZoom(float delta)
    {
        if (Mathf.Approximately(delta, 0f))
            return;

        _cam.orthographicSize -= delta;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);
    }
}
