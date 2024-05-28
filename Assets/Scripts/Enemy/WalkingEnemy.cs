using UnityEngine;

public class WalkingEnemy : Enemy
{
    public Transform[] waypoints;

    protected override void InitializeStates()
    {
        if (waypoints.Length > 0)
            StateMachine.ChangeState(new PatrolState(this, waypoints));
    }
}
