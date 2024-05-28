using Monoliths;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// 
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float MovementSpeed;
    public EnemyStateMachine StateMachine { get; private set; }

    protected virtual void Awake() => StateMachine = new EnemyStateMachine(this);
    protected virtual void Start() => InitializeStates();
    protected abstract void InitializeStates();
    protected virtual void Update() => StateMachine.CurrentState?.Execute();
    protected virtual void Patrol() {}
    public void ChangeState(EnemyState newState) {}
}
/// <summary>
/// 
/// </summary>
public class EnemyStateMachine
{
    private Enemy _enemy;
    public EnemyState CurrentState { get; private set; }

    public EnemyStateMachine(Enemy enemy) => this._enemy = enemy;
    public void ChangeState(EnemyState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
/// <summary>
/// 
/// </summary>
public abstract class EnemyState
{
    protected Enemy _enemy;

    public EnemyState(Enemy enemy) => this._enemy = enemy;
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
/// <summary>
///
/// </summary>
public class WalkingEnemy : Enemy
{
    public Transform[] waypoints;
    private int _waypointIndex = 0;

    protected override void InitializeStates()
    {
        if (waypoints.Length > 0) 
            ChangeState(new PatrolState(this, waypoints));
    }
}
/// <summary>
/// 
/// </summary>
public class PatrolState : EnemyState
{
    private Transform[] _waypoints;
    private int _waypointIndex = 0;

    public PatrolState(WalkingEnemy enemy, Transform[] waypoints) : base(enemy)
    {
        this._waypoints = waypoints;
    }

    public override void Enter()
    {
        _enemy.transform.position = _waypoints[_waypointIndex].position;
    }
    public override void Execute()
    {
        if (Vector3.Distance(_enemy.transform.position, _waypoints[_waypointIndex].position) < 0.01f)
            _waypointIndex = (_waypointIndex + 1) % _waypoints.Length;
        _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _waypoints[_waypointIndex].position, _enemy.MovementSpeed * Time.deltaTime);   
    }
    public override void Exit() {}
}
