using System;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;


public class InputHandler: MonoBehaviour
{
    public event Action<Vector2> MovePerformed;
    public event Action<Vector2> LookPerformed;
    public event Action UnlockPerformed; 

    [SerializeField] private InputActionAsset _inputAsset;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _unLockAction;

    private List<InputAction>_inputActions;

    private void Awake()
    {
        _inputActions = new List<InputAction>();
        var actionMap = _inputAsset.FindActionMap("Player");
        actionMap.Enable();
        _moveAction = actionMap.FindAction("Move");
        _lookAction = actionMap.FindAction("Look");
        _unLockAction = actionMap.FindAction("Unlock");

        _inputActions.Add(_moveAction);
        _inputActions.Add(_lookAction);
        _inputActions.Add(_unLockAction);

        _moveAction.performed += ctx => MovePerformed?.Invoke(ctx.ReadValue<Vector2>());
        _lookAction.performed += ctx => LookPerformed?.Invoke(ctx.ReadValue<Vector2>());
        _unLockAction.performed += ctx => UnlockPerformed?.Invoke();

        _moveAction.canceled += ctx => MovePerformed?.Invoke(Vector2.zero);
        _lookAction.canceled += ctx => LookPerformed?.Invoke(Vector2.zero);
    }

    private void OnEnable()
    {
        _inputAsset.Enable();
        foreach(var action in _inputActions)
        {
            action.Enable();
        }
    }
    private void OnDisable()
    {
        _inputAsset.Disable();

        foreach (var action in _inputActions)
        {
            action.Disable();
        }
        _inputActions.Clear();
    }

    public void EnableGameplayInput()
    {
        _moveAction.Enable();
        _lookAction.Enable();
    }

    public void DisableGameplayInput()
    {
        _moveAction.Disable();
        _lookAction.Disable();
    }
}
