using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MANU,
    GAME,
    GAME_ENDED,
    PAUSE,
    RESTART,
}
/// <summary>
///       ------------Pause  Restart <----
///       |            ^|     ^|          \
///       |            ||     ||           \
///       v            |v     |v            \
/// Main Manu ----->     Game     ------>  Game_ended
///     ^                                   |
///     |                                   |
///      -----------------------------------
/// 
/// 
/// </summary>

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
