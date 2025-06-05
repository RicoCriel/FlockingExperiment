using UnityEngine;

internal class MenuState: BaseState
{
    private readonly CursorController _cursorController;
    private readonly ToolTipController _toolTipController;
    private readonly PanelController _panelController;
    private readonly StateMachine _stateMachine;
    private readonly InputHandler _inputHandler;

    public MenuState(StateMachine stateMachine, StateFactory factory)
    {
        _stateMachine = stateMachine;
        _cursorController = new CursorController();
        _panelController = Object.FindFirstObjectByType<PanelController>();
        _toolTipController = Object.FindFirstObjectByType<ToolTipController>();
        _inputHandler = Object.FindFirstObjectByType<InputHandler>();
    }

    public override void Enter()
    {
        _toolTipController.SetEnabled(true);

        _panelController.HideUnlockPanel();
        _panelController.ShowControlDiverPanel();
        _cursorController.ShowCursor();

        AssignToolTipsToButtons();
        _inputHandler.UnlockPerformed += OnUnlockPerformed;
    }

    public override void Update()
    {
        _toolTipController.UpdatePosition();
    }

    public override void Exit()
    {
        _toolTipController.SetEnabled(false);
        _toolTipController.HideToolTips();

        _panelController.HideControlDiverPanel();
        _panelController.HideActiveSettingPanels();

        _cursorController.HideCursor();
        _inputHandler.UnlockPerformed -= OnUnlockPerformed;
    }

    private void OnUnlockPerformed()
    {
        _stateMachine.TransitionTo(_stateMachine.Factory.Play());
    }

    private void AssignToolTipsToButtons()
    {
        var buttons = Object.FindObjectsOfType<ButtonView>();
        foreach (var button in buttons)
        {
            button.Initialize(_toolTipController);
        }
    }


}
