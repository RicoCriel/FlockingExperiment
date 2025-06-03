internal abstract class BaseState : IState
{
    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}

internal class StateFactory
{
    private readonly StateMachine _context;

    public StateFactory(StateMachine context) => _context = context;

    public MenuState Menu() => new MenuState(_context, this);
    public PlayState Play() => new PlayState(_context, this);
}

internal class StateMachine
{
    private IState _currentState;
    public readonly StateFactory Factory;

    public StateMachine()
    {
        Factory = new StateFactory(this);
        _currentState = Factory.Menu(); 
        _currentState.Enter();
    }

    public void TransitionTo(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void Update() => _currentState?.Update();
}


