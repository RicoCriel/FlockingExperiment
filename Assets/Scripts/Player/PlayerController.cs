using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public class PlayerController: MonoBehaviour
{
    [Header("Diver movement settings")]
    [Range(0,10f)]
    [SerializeField] private float _moveSpeed;
    [Range(0.1f, 10f)]
    [SerializeField] private float _mouseSensitivity = 1f;
    [Range(0,180f)]
    [SerializeField] private float _rotationSpeed;
    [Range(0, 5f)]
    [SerializeField] private float _rotationLerpSpeed;

    [SerializeField] private InputHandler _inputHandler;

    private CharacterController _controller;
    private Vector2 _inputVector;
    private Vector2 _lookVector;

    [SerializeField] private Vector3 _swimLimits;

    private float _yaw;
    private float _pitch;
    private Vector2 _smoothedLook;
    private bool _isEnabled;

    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = Mathf.Clamp(value, 0f, 10f);
    }

    public float MouseSensitivity
    {
        get => _mouseSensitivity;
        set => _mouseSensitivity = Mathf.Clamp(value, 0.1f, 10f);
    }

    public (float min, float max) MoveSpeedRange => (0f, 10f);
    public (float min, float max) MouseSensitivityRange => (0.1f, 10f);

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputHandler.MovePerformed += OnMove;
        _inputHandler.LookPerformed += OnLook;
    }

    private void OnDisable()
    {
        _inputHandler.MovePerformed -= OnMove;
        _inputHandler.LookPerformed -= OnLook;
    }

    //public void SetMovementSpeed(float value) => MoveSpeed = value;
    public void SetMovementSpeed(float value)
    {
        Debug.Log($"Setting move speed to: {value}");
        MoveSpeed = value;
    }

    public void SetMouseSensitivity(float value) => MouseSensitivity = value;

    public void SetEnabled(bool enabled) => _isEnabled = enabled;

    public void UpdateMovement()
    {
        if (!_isEnabled) return;
        _smoothedLook = Vector2.Lerp(_smoothedLook, _lookVector, _rotationLerpSpeed * Time.deltaTime);
        ApplyRotation();
        MoveForward();
        KeepWithinBounds();
    }

    public void OnMove(Vector2 input)
    {
        _inputVector = input;
    }

    public void OnLook(Vector2 input)
    {
        _lookVector = input * MouseSensitivity;
    }

    private void ApplyRotation()
    {
        _yaw += _smoothedLook.x * _rotationSpeed * Time.deltaTime;
        _pitch -= _smoothedLook.y * _rotationSpeed * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -85f, 85f);

        _controller.transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    private void MoveForward()
    {
        Vector3 forward = transform.forward * _inputVector.y * (MoveSpeed * Time.deltaTime);
        _controller.Move(forward);
    }

    private void KeepWithinBounds()
    {
        Vector3 pos = _controller.transform.position;
        Vector3 bounds = _swimLimits;

        pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
        pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);
        pos.z = Mathf.Clamp(pos.z, -bounds.z, bounds.z);

        _controller.transform.position = pos;
    }
}
