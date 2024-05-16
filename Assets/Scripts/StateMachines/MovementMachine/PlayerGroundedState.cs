using UnityEngine;

public class PlayerGroundedState : MovementState
{
    public override void Enter()
    {
        _playerMovement.SetFriction(1f);
        _playerMovement.SetMovementLocked(false);
    }
    public override void Exit() => _playerMovement.SetFriction(0f);
}

