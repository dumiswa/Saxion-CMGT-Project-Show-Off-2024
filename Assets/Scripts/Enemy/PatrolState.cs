using System;
using UnityEngine;

public class PatrolState : EnemyState
{
    private Transform[] _waypoints;
    private int _waypointIndex = 0;

    public PatrolState(WalkingEnemy enemy, Transform[] waypoints) : base(enemy)
        => this._waypoints = waypoints;
   

    public override void Enter()
    {
        try
        {
            _enemy.transform.position = _waypoints[_waypointIndex].position;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during PatrolState.Enter: " + ex.ToString());
        }
    }
    public override void Execute()
    {
        if (Vector3.Distance(_enemy.transform.position, _waypoints[_waypointIndex].position) < 0.01f)      
            _waypointIndex = (_waypointIndex + 1) % _waypoints.Length;
               
        _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _waypoints[_waypointIndex].position, _enemy.MovementSpeed * Time.deltaTime);   
    }
    public override void Exit() {}
}
