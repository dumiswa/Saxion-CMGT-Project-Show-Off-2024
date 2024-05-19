public abstract class GameState
{
    public delegate void OnEnterState(GameState state);
    public delegate void OnExitState(GameState state);

    public static OnEnterState OnEnter;
    public static OnExitState OnExit;

    public virtual void Enter() => OnEnter?.Invoke(this);
    public virtual void Exit() => OnExit?.Invoke(this);
}

public abstract class GameState<SubStateType> : GameState where SubStateType : GameState
{
    public AbstractStateMachine<SubStateType> SubStateMachine { get; protected set; } = new();
}
