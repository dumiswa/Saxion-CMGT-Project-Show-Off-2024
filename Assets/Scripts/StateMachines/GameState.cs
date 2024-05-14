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
    public AbstractStateMachine<SubStateType> SubStateMachine { get; protected set; }
}

public class MenuState : GameState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class IntroState : GameState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class LevelSelectionState : GameState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class LevelState : GameState<LevelSubState>
{
    public void FinalizeLevel()
    {
        if (!SubStateMachine.CurrentIs<MidLevelState>())
            return;

        SubStateMachine.Next<LevelFinishState>();
    }
    public override void Enter()
    {
        //INIT
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public abstract class LevelSubState : GameState {}
public class LevelStartState : LevelSubState
{
    public override void Enter()
    {
        //LOCK
        //PLAY ANIM
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class MidLevelState : LevelSubState
{
    public override void Enter()
    {
        //UNLOCK
        base.Enter();
    }

    public override void Exit()
    {
        //LOCK
        base.Exit();
    }
}

public class LevelFinishState : LevelSubState
{
    public override void Enter()
    {
        //SAVE
        //PLAY ANIM
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
