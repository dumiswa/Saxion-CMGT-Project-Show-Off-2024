public class PlayerClimbingState : MovementState
{
    public override void Enter()
    {
        _playerMovement.ResetVelocity();
        _playerMovement.SetMovementLocked(
            locked: false, 
            lockX: true, 
            lockZ: false, 
            swapZtoY: true);
    }

    public override void Exit() 
    {
        _playerMovement.SetMovementLocked(locked: false);
    }
}
