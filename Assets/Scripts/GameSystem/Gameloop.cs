using UnityEngine;

public class Gameloop : MonoBehaviour
{
    private StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = new StateMachine();
    }

    private void Update()
    {
        _stateMachine.Update();
    }
}
