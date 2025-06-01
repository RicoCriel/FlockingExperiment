using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (CharacterController))]
public class PlayerController: MonoBehaviour
{
    [Range(0,10f)]
    [SerializeField] private float _moveSpeed;
    [Range(0,180f)]
    [SerializeField] private float _rotationSpeed;
    [Range(0, 5f)]
    [SerializeField] private float _rotationLerpSpeed;

    [SerializeField] private InputActionAsset _inputAsset;
    private InputAction _moveAction;
    private InputAction _lookAction;

    private CharacterController _controller;
    private Vector2 _inputVector;
    private Vector2 _lookVector;

    [SerializeField] private Vector3 _swimLimits;

    private float _yaw;
    private float _pitch;
    private Vector2 _smoothedLook;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        var actionMap = _inputAsset.FindActionMap("Player"); 
        _moveAction = actionMap.FindAction("Move");
        _lookAction = actionMap.FindAction("Look");

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _lookAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();

        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;
    }

    private void Update()
    {
        _smoothedLook = Vector2.Lerp(_smoothedLook, _lookVector, _rotationLerpSpeed * Time.deltaTime);
        ApplyRotation();
        MoveForward();
        KeepWithinBounds();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookVector = context.ReadValue<Vector2>();
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
        Vector3 forward = transform.forward * _inputVector.y * (_moveSpeed * Time.deltaTime);
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

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(Vector3.zero, _swimLimits * 2f);
    //}
}
