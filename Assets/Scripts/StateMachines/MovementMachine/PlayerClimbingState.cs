using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingState : MovementState
{
    public override void Enter()
    {
        _playerMovement.SetMovementLocked(true);
        _playerMovement.EnableVerticalMovement(); 
    }

    public override void Update()
    {
        _playerMovement.HandleVerticalInput();
    }

    public override void Exit() 
    {
        _playerMovement.SetMovementLocked(false);
        _playerMovement.DisableVerticalMovement();
    }
}
