public class PlayerGroundedState : MovementState
{
    public override void Enter()
    {
        AudioManager.Instance.PlaySound("MainCharacterLanding");
        _playerMovement.SetFriction(1f);
        _playerMovement.SetMovementLocked(false);
    }
    public override void Exit() => _playerMovement.SetFriction(0f);
}

