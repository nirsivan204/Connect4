
public interface IStateMachineClient
{
    void OnEnterState(GameState state);
    void OnExitState(GameState state);
}
