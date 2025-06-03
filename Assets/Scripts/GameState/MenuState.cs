
using UnityEngine;

internal class MenuState: BaseState
{
    private readonly CursorController _cursorController;
    private readonly PanelController _panelController;
    private readonly StateMachine _stateMachine;
    private readonly InputHandler _inputHandler;

    public MenuState(StateMachine stateMachine, StateFactory factory)
    {
        _stateMachine = stateMachine;
        _cursorController = new CursorController();
        _panelController = Object.FindFirstObjectByType<PanelController>();
        _inputHandler = Object.FindFirstObjectByType<InputHandler>();
    }

    public override void Enter()
    {
        _panelController.HideUnlockPanel();
        _panelController.ShowControlDiverPanel();
        _cursorController.ShowCursor();
        _inputHandler.UnlockPerformed += OnUnlockPerformed;
    }

    //The state doesnt need an Update()

    public override void Exit()
    {
        _panelController.HideControlDiverPanel();
        _cursorController.HideCursor();
        _inputHandler.UnlockPerformed -= OnUnlockPerformed;
    }

    private void OnUnlockPerformed()
    {
        _stateMachine.TransitionTo(_stateMachine.Factory.Play());
    }


}
