public abstract class GameState
{
    public abstract void Enter();
    public abstract void Exit();
}

public abstract class GameState<SubStateType> : GameState where SubStateType : GameState
{
    public AbstractStateMachine<SubStateType> SubStateMachine { get; protected set; }

    public delegate void OnEnterState(GameState state);
    public delegate void OnExitState(GameState state);

    public static OnEnterState OnEnter;
    public static OnExitState OnExit;
}