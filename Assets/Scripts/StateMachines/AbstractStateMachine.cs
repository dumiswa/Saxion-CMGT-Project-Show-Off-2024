using Monoliths;
using System;

public class AbstractStateMachine<StateType> where StateType : GameState
{
    public StateType Current { get; protected set; }
    public StateType Previous { get; protected set; }

    public virtual void Next(Type stateType)
    {
        Current.Exit();
        NextNoExit(stateType);
    }
    public virtual void NextNoExit(Type stateType)
    {
        Previous = Current;
        Current = (StateType)StateRegistrar.Get(stateType);
        Current.Enter();
    }

    public virtual void Next<State>() where State : StateType
    {
        Current.Exit();
        NextNoExit<State>();
    }
    public virtual void NextNoExit<State>()
    {
        Previous = Current;
        Current = (StateType)StateRegistrar.Get(typeof(State));
        Current.Enter();
    }

    public void Return() => Next(Previous.GetType());

    public bool CurrentIs<State>() => Current is not null && Current.GetType().IsAssignableFrom(typeof(State));
    public bool CurrentIs(Type stateType) => Current is not null && Current.GetType().IsAssignableFrom(stateType);
}
