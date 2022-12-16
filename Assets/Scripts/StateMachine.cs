using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MANU,
    GAME,
    GAME_ENDED
}

public static class StateMachine
{
    // The current state of the state machine
    public static GameState currentState = GameState.MANU;
    public static event Action<GameState> stateExitEvent;
    public static event Action<GameState> stateEnterEvent;


    // Change to a new state
    public static void ChangeState(GameState nextState)
    {
        // Exit the current state
        stateExitEvent?.Invoke(currentState);

        // Set the current state to the new state
        currentState = nextState;

        // Enter the new state
        stateEnterEvent?.Invoke(currentState);
    }
}
/*
// An example state
public class ExampleState : IState
{
    // A reference to the state machine that this state belongs to
    private StateMachine stateMachine;

    public ExampleState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Entering Example State");
    }

    public void Exit()
    {
        Debug.Log("Exiting Example State");
    }
}
*/
