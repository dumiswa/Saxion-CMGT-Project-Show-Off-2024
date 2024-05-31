using UnityEngine;

public class WalkingEnemy : Enemy
{
    public Vector3[] Waypoints;
    [HideInInspector]
    public bool IsBeingEdited;

    public float ChaseRange = 1.5f;

    protected override void InitializeStates()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (Waypoints.Length > 0)    
            StateMachine.ChangeState(new PatrolState(this, Waypoints, playerTransform, ChaseRange));    
        else        
            Debug.LogError("No waypoints set for PatrolState.");
        
    }

    private void OnDrawGizmos()
    {
        if (IsBeingEdited)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.grey;

        if (Waypoints.Length <= 1)
            return;

        for (int i = 0; i < Waypoints.Length; i++)
        {
            Gizmos.DrawSphere(Waypoints[i], 0.15f);
            Gizmos.DrawSphere(Waypoints[i < Waypoints.Length - 1 ? i + 1 : 0], 0.15f);
            Gizmos.DrawLine(Waypoints[i], Waypoints[i < Waypoints.Length - 1? i + 1 : 0]);
        }
    }
}
