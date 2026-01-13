using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _cameraRig;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 120f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeedMouse = 1f;
    [SerializeField] private float _zoomSpeedMobile = 0.01f;

    private float _minZoom = 4f;
    private float _maxZoom = 20f;

    private PlayerInputAction inputAction;

    private Vector2 _lookInput;
    private bool _isRotating;

    private float _zoomDelta;

    private Vector2 _touch0;
    private Vector2 _touch1;
    private bool _isPinchZoom;
    private float _previousPinchDistance;

    private float _lockedXrotation;

    private void Awake()
    {
        _lockedXrotation = transform.rotation.eulerAngles.x;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);

        inputAction = InputManager.instance.inputAction;
    }

    private void OnEnable()
    {
        inputAction.Camera.Enable();

        inputAction.Camera.Look.performed += OnLook;
        inputAction.Camera.Look.canceled += OnLook;

        inputAction.Camera.Rotate.started += OnRotate;
        inputAction.Camera.Rotate.canceled += OnRotate;

        inputAction.Camera.Zoom.performed += OnZoom;
    }

    private void OnDisable()
    {
        inputAction.Camera.Look.performed -= OnLook;
        inputAction.Camera.Look.canceled -= OnLook;

        inputAction.Camera.Rotate.started -= OnRotate;
        inputAction.Camera.Rotate.canceled -= OnRotate;

        inputAction.Camera.Zoom.performed -= OnZoom;

        inputAction.Camera.Disable();
    }

    private void LateUpdate()
    {
        HandleRotation();
        HandleZoom();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        _isRotating = context.started;
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse)
        {
            float scroll = context.ReadValue<float>();
            _zoomDelta += scroll * _zoomSpeedMouse;
        }
    }

    private void HandleRotation()
    {
        if (!_isRotating)
            return;

        float yawDelta = _lookInput.x * _rotationSpeed * Time.deltaTime;    
        float newY = _cameraRig.eulerAngles.y + yawDelta;

        _cameraRig.rotation = Quaternion.Euler(0f, newY, 0f);
    }

    private void HandleZoom()
    {
        if (Mathf.Approximately(_zoomDelta, 0f))
            return;

        _cam.orthographicSize -= _zoomDelta;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);

        _zoomDelta = 0f;
    }
}
