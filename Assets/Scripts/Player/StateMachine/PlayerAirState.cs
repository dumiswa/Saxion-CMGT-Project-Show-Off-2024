public class PlayerAirState : MovementState
{
    private const float GLIDING_MOBILITY_MULTIPLIER = 0.5f;
    private const float GLIDING_GRAVITY_MULTIPLIER = 0.025f;
    public override void Enter()
    {
        _playerMovement.SetFriction(0f);

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
    public override void Exit() => _playerMovement.SetFriction(1f);
}

