using UnityEngine;

internal class PlayState : BaseState
{
    private readonly StateMachine _stateMachine;
    private readonly PlayerController _playerController;
    private readonly PanelController _panelController;
    private readonly InputHandler _inputHandler;

    public PlayState(StateMachine stateMachine, StateFactory factory)
    {
        _stateMachine = stateMachine;
        _playerController = Object.FindFirstObjectByType<PlayerController>();
        _panelController = Object.FindFirstObjectByType<PanelController>();
        _inputHandler = Object.FindFirstObjectByType<InputHandler>();
    }

    public override void Enter()
    {
        _panelController.HideIcons();
        _panelController.HideControlDiverPanel();
        _panelController.ShowUnlockPanel();
        _inputHandler.EnableGameplayInput();
        _playerController.SetEnabled(true);
        _inputHandler.UnlockPerformed += OnUnlockPerformed;
    }
    public override void Update()
    {
        _playerController.UpdateMovement();
    }

    public override void Exit()
    {
        _panelController.ShowIcons();
        _panelController.HideUnlockPanel();
        _panelController.ShowControlDiverPanel();
        _playerController.SetEnabled(false);
        _inputHandler.DisableGameplayInput();
        _inputHandler.UnlockPerformed -= OnUnlockPerformed;
    }

    private void OnUnlockPerformed()
    {
        _stateMachine.TransitionTo(_stateMachine.Factory.Menu());
    }

    
}
