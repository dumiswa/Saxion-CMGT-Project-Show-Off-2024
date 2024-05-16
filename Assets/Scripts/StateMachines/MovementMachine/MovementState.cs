using Monoliths.Player;

public class MovementState : GameState
{
    protected PlayerMovement _playerMovement;

    public void SetPlayerMovement(PlayerMovement playerMovement) => _playerMovement = playerMovement;
    public virtual void Update(){}
    public override void Enter(){}
    public override void Exit(){}
}

