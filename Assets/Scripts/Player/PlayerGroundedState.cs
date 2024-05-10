using Monoliths.Player;
using UnityEngine;

public class MovementState : GameState
{
    protected PlayerMovement _playerMovement;

    public void SetPlayerMovement(PlayerMovement playerMovement) => _playerMovement = playerMovement;
    public virtual void Update(){}
    public override void Enter(){}
    public override void Exit(){}
}

public class PlayerGroundedState : MovementState
{  
    public override void Enter() => _playerMovement.SetMovementLocked(false);
    public override void Exit() { }
}

public class PlayerAirState : MovementState
{
    private const float GLIDING_MOBILITY_MULTIPLIER = 0.5f;
    private const float GLIDING_GRAVITY_MULTIPLIER = 0.1f;
    public override void Enter()
    {
        if (_playerMovement.IsGlidingUnlocked)
        {
            if (_playerMovement.IsGliding)
                _playerMovement.SetMovementParams(
                    GLIDING_GRAVITY_MULTIPLIER, 
                    GLIDING_MOBILITY_MULTIPLIER);
        }
        else _playerMovement.SetMovementLocked(true);
    }
    public override void Update()
    {
        if (!_playerMovement.IsGlidingUnlocked)
            return;

        if (_playerMovement.IsGliding)
            _playerMovement.SetMovementParams(
                GLIDING_GRAVITY_MULTIPLIER,
                GLIDING_MOBILITY_MULTIPLIER);
        else
            _playerMovement.SetMovementParams(1.0f, 1.0f);
    }
    public override void Exit() { }
}

