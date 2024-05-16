using Monoliths.Player;
using Monoliths;
using System;

public class MovementStateMachine : AbstractStateMachine<MovementState>
{
    private PlayerMovement _owner;
    public MovementStateMachine(PlayerMovement owner) 
        => _owner = owner;

    public override void NextNoExit(Type stateType)
    {
        Previous = Current;
        Current = (MovementState)StateRegistrar.Get(stateType);
        Current.SetPlayerMovement(_owner);
        Current.Enter();
    }
    public override void NextNoExit<State>()
    {
        Previous = Current;
        Current = (MovementState)StateRegistrar.Get(typeof(State));
        Current.SetPlayerMovement(_owner);
        Current.Enter();
    }
}