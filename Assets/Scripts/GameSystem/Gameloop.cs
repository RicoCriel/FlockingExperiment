using System;
using UnityEngine;

public class Gameloop : MonoBehaviour
{
    private StateMachine _stateMachine;
    private PropSpawner _propSpawner;
    public event Action OnSimulationStarted;

    private void Awake()
    {
        _stateMachine = new StateMachine();
        _propSpawner = FindFirstObjectByType<PropSpawner>();
        if(_propSpawner != null)
        {
            OnSimulationStarted += _propSpawner.SpawnProps;
        }
    }

    private void OnEnable()
    {
        OnSimulationStarted?.Invoke();
    }

    private void OnDisable()
    {
        OnSimulationStarted -= _propSpawner.SpawnProps;
    }

    private void Update()
    {
        _stateMachine.Update();
    }
}
