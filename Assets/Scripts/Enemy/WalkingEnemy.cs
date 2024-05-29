using UnityEngine;

public class WalkingEnemy : Enemy
{
    public Transform[] Waypoints;
    public float ChaseRange = 1.5f;

    protected override void InitializeStates()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (Waypoints.Length > 0)    
            StateMachine.ChangeState(new PatrolState(this, Waypoints, playerTransform, ChaseRange));    
        else        
            Debug.LogError("No waypoints set for PatrolState.");
        
    }
}
