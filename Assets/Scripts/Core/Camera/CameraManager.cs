using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _cam;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeedMouse;
    [SerializeField] private float _zoomSpeedMobile;
    private float _minZoom = 4f;
    private float _maxZoom = 20f;

    private Vector2 _delta;
    private Vector2 _lookInput;

    private Vector2 _touch0;
    private Vector2 _touch1;

    private bool _isRotating;
    private bool _isPinchZoom;

    private float _lockedXrotation;
    private float _zoomDelta;
    private float _previousPinchDistance;

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

    public void OnRotate(InputAction.CallbackContext context)
    {
        _isRotating = context.started || context.performed;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        Vector2 scroll = context.ReadValue<Vector2>();
        _zoomDelta += scroll.y * _zoomSpeedMouse;
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

    #endregion

    private void LateUpdate()
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (!_isRotating)
            return;

        float yawDelta = _lookInput.x * _rotationSpeed * Time.deltaTime;
        float newY = transform.eulerAngles.y + yawDelta;

        transform.rotation = Quaternion.Euler(_lockedXrotation, newY, 0f);
    }

    private void TryPinchZoom()
    {
        if (_touch0 == Vector2.zero || _touch1 == Vector2.zero)
        {
            _previousPinchDistance = 0f;
            _isPinchZoom = false;
            return;
        }

        float currentDistance = Vector2.Distance(_touch0, _touch1);

        if (!_isPinchZoom)
        {
            _previousPinchDistance = currentDistance;
            _isPinchZoom = true;
            return;
        }

        float delta = currentDistance - _previousPinchDistance;
        _previousPinchDistance = currentDistance;

        _zoomDelta += delta * _zoomSpeedMobile;
    }

    private void HandleZoom()
    {
        if (Mathf.Approximately(_zoomDelta, 0f))
            return;

        _cam.orthographicSize -= _zoomDelta * Time.deltaTime;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);

        _zoomDelta = 0f;
    }
}
