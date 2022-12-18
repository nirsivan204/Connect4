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
    public static event Action<GameState> StateExitEvent;
    public static event Action<GameState> StateEnterEvent;
    private static GameState? _stateInQueue = null;
    private static bool _isChangingState = false;


    /// <summary>
    /// Change to a new state atomically. While changing to a new state, another state-change can be requested before the previous was ended. 
    /// This implementation will allow the second state change, but under 2 conditions:
    /// 1. The first change ended completely (All listeners of both exit and enter event finished running)
    /// 2. During a change, only one additional state change can be saved to be right after that in the queue. When the second change will start, the user can request another one.  
    /// 
    /// This is a defensive programming mathodology.
    /// This state machine supports moving from one state to another, and it can't allow registering 2 different next-states from the same state.
    /// 
    ///
    /// </summary>
    private static void ChangeState(GameState nextState)
    {
        _isChangingState = true;
        // Exit the current state
        StateExitEvent?.Invoke(currentState);

        // Set the current state to the new state
        currentState = nextState;

        // Enter the new state
        StateEnterEvent?.Invoke(currentState);
        _isChangingState = false;
        //If queue not empty
        if (_stateInQueue != null)
        {
            // Save in a temporary var
            GameState newState = (GameState)_stateInQueue;
            // Empty the queue
            _stateInQueue = null;
            // Call next change
            ChangeState(newState);
        }
    }

    public static void SetNextState(GameState nextState)
    {
        //If not during a state change
        if (_isChangingState)
        {
            //If user requested a different state from the one he already requested, it is not allowed
            if (_stateInQueue != null && _stateInQueue != nextState)
            {
                throw new Exception("Two different states were registered to be the next state, this is not allowed");
            }
            //Save next state in queue
            _stateInQueue = nextState;
            return;
        }
        else
        {
            ChangeState(nextState);
        }
    }

}
