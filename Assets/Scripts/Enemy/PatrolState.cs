using System;
using UnityEngine;

public class PatrolState : EnemyState
{
    private Transform[] _waypoints;
    private int _waypointIndex = 0;
    private float _chaseRange;
    private Transform _playerTransform;
    private Transform _lastWaypointTransform;

    public PatrolState(WalkingEnemy enemy, Transform[] waypoints, Transform playerTransform, float chaseRange, int waypointIndex = 0) : base(enemy)
    {
        _waypoints = waypoints;  
        _playerTransform = playerTransform;
        _chaseRange = chaseRange;
        _waypointIndex = waypointIndex;
    }

    private void MoveToNextWaypoint() 
        => _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _lastWaypointTransform.position, _enemy.MovementSpeed * Time.deltaTime);

    public override void Enter()
    {
        _lastWaypointTransform = _waypoints[_waypointIndex].transform;
        MoveToNextWaypoint();
    }
    public override void Execute()
    {
        if (Vector3.Distance(_enemy.transform.position, _playerTransform.position) <= _chaseRange)
        {
            _enemy.StateMachine.ChangeState(new ChaseState(_enemy, _playerTransform, _chaseRange, 10.0f, _waypointIndex));
            return;
        }
        if (Vector3.Distance(_enemy.transform.position, _waypoints[_waypointIndex].position) < 0.01f)       
            _waypointIndex = (_waypointIndex + 1) % _waypoints.Length;
        
        _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _waypoints[_waypointIndex].position, _enemy.MovementSpeed * Time.deltaTime);   
    }
    public override void Exit() {}
}
