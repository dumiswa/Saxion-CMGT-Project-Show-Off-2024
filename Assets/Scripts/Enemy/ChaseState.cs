using UnityEngine;

public class ChaseState : EnemyState
{
    private Transform _playerTransform;
    private float _loseInterestRange;
    private int _lastWaypointIndex;
    private float _chaseRange;

    public ChaseState (Enemy enemy, Transform playerTransform, float chaseRange, float loseInterestRange, int lastWaypointIndex) : base(enemy)
    {
        _playerTransform = playerTransform;
        _chaseRange = chaseRange;
        _loseInterestRange = loseInterestRange;
        _lastWaypointIndex = lastWaypointIndex;
    }

    public override void Enter() {}
    public override void Execute()
    {
        _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _playerTransform.position, _enemy.MovementSpeed * Time.deltaTime);

        if(_enemy is WalkingEnemy walkingEnemy)
        {
            Transform lastWaypointTransform = walkingEnemy.Waypoints[_lastWaypointIndex];
            float distanceToLastWaypoint = Vector3.Distance(_enemy.transform.position, lastWaypointTransform.position);

            if(distanceToLastWaypoint > _loseInterestRange)
            {

                _enemy.StateMachine.ChangeState(new PatrolState(walkingEnemy, walkingEnemy.Waypoints, _playerTransform, _chaseRange, _lastWaypointIndex));
            }      
        }   
    }
    public override void Exit() {}
}
