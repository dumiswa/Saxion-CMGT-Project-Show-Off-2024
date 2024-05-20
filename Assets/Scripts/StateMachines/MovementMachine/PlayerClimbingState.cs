using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingState : MovementState
{
    public override void Enter()
    {
        _playerMovement.IsClimbing = true;
        _playerMovement.SetMovementLocked(true);
        _playerMovement.EnableVerticalMovement();
    }

    public override void Exit() 
    {
        _playerMovement.IsClimbing = false;
        _playerMovement.SetMovementLocked(false);
        _playerMovement.DisableVerticalMovement();
    }
}
