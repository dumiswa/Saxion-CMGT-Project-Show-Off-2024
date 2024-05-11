using UnityEngine;

public class PlayerGroundedState : MovementState
{  
    public override void Enter() => _playerMovement.SetMovementLocked(false);
    public override void Exit() { }
}

