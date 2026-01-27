using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _cam;
    [SerializeField] private CinemachineCamera _virtualCam;
    [SerializeField] private Transform _cameraRig;
    private Interactable _currentInteractable;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 120f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeedMouse = 1f;
    [SerializeField] private float _zoomSpeedMobile = 0.01f;

    private float _minZoom = 30f;
    private float _maxZoom = 75f;

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
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, _minZoom, _maxZoom);

        inputAction = InputManager.instance.inputAction;
    }

    private void OnEnable()
    {
        inputAction.Camera.Enable();

        inputAction.Camera.Zoom.performed += OnZoom;
    }

    private void OnDisable()
    {
        inputAction.Camera.Zoom.performed -= OnZoom;

        inputAction.Camera.Disable();
    }

    private void LateUpdate()
    {
        StartRaycast();
        HandleZoom();
        HandlePinchZoom();
    }


    private void StartRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    _currentInteractable.FadeOut();
                }
                return;
            }
        }

        if (_currentInteractable != null)
        {
            _currentInteractable.FadeIn();
            _currentInteractable = null;
        }
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse)
        {
            float scroll = context.ReadValue<float>();
            _zoomDelta += scroll * _zoomSpeedMouse;
        }
    }

    private void HandlePinchZoom()
    {
        if (Touchscreen.current == null)
            return;

        if (Touchscreen.current.touches.Count < 2)
        {
            _isPinchZoom = false;
            return;
        }

        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];

        if (!touch0.isInProgress || !touch1.isInProgress)
        {
            _isPinchZoom = false;
            return;
        }

        Vector2 pos0 = touch0.position.ReadValue();
        Vector2 pos1 = touch1.position.ReadValue();

        float currentDistance = Vector2.Distance(pos0, pos1);

        if (!_isPinchZoom)
        {
            _previousPinchDistance = currentDistance;
            _isPinchZoom = true;
            return;
        }

        float delta = currentDistance - _previousPinchDistance;
        _previousPinchDistance = currentDistance;

        _zoomDelta += delta * _zoomSpeedMobile * Time.deltaTime;
    }

    private void HandleZoom()
    {
        if (Mathf.Approximately(_zoomDelta, 0f))
            return;

        _cam.fieldOfView -= _zoomDelta;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, _minZoom, _maxZoom);

        _zoomDelta = 0f;
    }

    public void SetCameraTarget(Transform target)
    {
        _virtualCam.Follow = target;
    }
}
