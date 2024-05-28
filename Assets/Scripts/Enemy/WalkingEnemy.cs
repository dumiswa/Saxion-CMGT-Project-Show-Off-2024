using UnityEngine;

public class WalkingEnemy : Enemy
{
    public Transform[] waypoints;

    protected override void InitializeStates()
    {
        if (waypoints.Length > 0)
        {
            Debug.Log("Initializing patrol state");
            ChangeState(new PatrolState(this, waypoints));
        }     
        else Debug.Log("No waypoints set");
    }
}
