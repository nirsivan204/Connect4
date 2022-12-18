using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractManager : MonoBehaviour,IStateMachineClient
{
    public abstract void OnEnterState(GameState state);

    public abstract void OnExitState(GameState state);

    private void OnValidate()
    {
        ValidateParams();
    }

    protected virtual void OnEnable()
    {
        OnEnterState(StateMachine.currentState);
        StateMachine.StateEnterEvent += OnEnterState;
        StateMachine.StateExitEvent += OnExitState;
    }

    protected virtual void OnDisable()
    {
        StateMachine.StateEnterEvent -= OnEnterState;
        StateMachine.StateExitEvent -= OnExitState;
    }

    protected abstract void ValidateParams();

}
