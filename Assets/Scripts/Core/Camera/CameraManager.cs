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
    private float _xRotation;
    private float _zoomDelta;
    private float _minZoom = 4f;
    private float _maxZoom = 20f;

    private void Awake()
    {
        _xRotation = transform.rotation.eulerAngles.x;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _delta = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _isMoving = context.started || context.performed;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        _isRotating = context.started || context.performed;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        _zoomDelta = context.ReadValue<float>();
    }

    private void LateUpdate()
    {
        if (_isMoving)
        {   
            var pos = transform.right * (_delta.x * -_movementSpeed);
            pos += transform.up * (_delta.y * -_movementSpeed);

            transform.position += Time.deltaTime * pos;
        }

        if (_isRotating)
        {
            transform.Rotate(new Vector3(_xRotation, _delta.x * _rotationSpeed, 0.0f));
            transform.rotation = Quaternion.Euler(_xRotation, transform.rotation.eulerAngles.y, 0.0f);
        }

        if (_zoomDelta != 0)
        {
            _cam.orthographicSize -= _zoomDelta * _zoomSpeed * Time.deltaTime;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);

            _zoomDelta = 0f;
        }
    }
}
