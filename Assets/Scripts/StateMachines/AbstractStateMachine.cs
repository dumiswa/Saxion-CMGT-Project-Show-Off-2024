using System;

public class AbstractStateMachine<StateType> where StateType : GameState
{
    public StateType Current  { get; private set; }
    public StateType Previous { get; private set; }

    public void Next(Type stateType)
    {
        Current.Exit();
        NextNoExit(stateType);
    }
    public void NextNoExit(Type stateType)
    {
        Previous = Current;
        Current = (StateType)StateRegistrar.Get(stateType);
        Current.Enter();
    }

    public void Next<State>() where State : StateType
    {
        Current.Exit();
        NextNoExit<State>();
    }
    public void NextNoExit<State>()
    {
        Previous = Current;
        Current = (StateType)StateRegistrar.Get(typeof(State));
        Current.Enter();
    }

    public void Return() => Next(Previous.GetType());

    public bool CurrentIs<State>() => Current is not null && Current.GetType().IsAssignableFrom(typeof(State));
    public bool CurrentIs(Type stateType) => Current is not null && Current.GetType().IsAssignableFrom(stateType);
}
