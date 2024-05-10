using Monoliths.Player;

public class PlayerWalkingState : GameState
{
    private PlayerMovement _playerMovement;

    public PlayerWalkingState(PlayerMovement playerMovement) =>  _playerMovement = playerMovement;
    
    public override void Enter() => _playerMovement.SetMovementEnabled(true);
    public override void Exit() => _playerMovement.SetMovementEnabled(false);
}

public class PlayerJumpingState : GameState
{
    private PlayerJumpingState _playerMovement;

    public PlayerJumpingState(PlayerMovement playerMovement) => _playerMovement = playerMovement;
    public override void Enter() => _playerMovement.PerformJump();

    public override void Exit() { }
}

public class PlayerFallingState : GameState
{
    private PlayerMovement _playerMovement;

    public PlayerFallingState(PlayerMovement playerMovement) => _playerMovement = playerMovement;

    public override void Enter() => _playerMovement.SetMovementEnabled(false);
    public override void Exit() => _playerMovement.SetMovementEnabled(true);
}

