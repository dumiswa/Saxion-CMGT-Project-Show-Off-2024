using Monoliths.Player;

public class MidLevelState : LevelSubState
{
    public override void Enter()
    {
        DataBridge.UpdateData(PlayerMovement.MOVEMENT_ENABLED_DATA_ID, true);
        base.Enter();
    }

    public override void Exit()
    {
        DataBridge.UpdateData(PlayerMovement.MOVEMENT_ENABLED_DATA_ID, false);
        base.Exit();
    }
}
