using UnityEngine;

public class EnemyStateMachine
{
    private Enemy _enemy;
    public EnemyState CurrentState { get; private set; }

    public EnemyStateMachine(Enemy enemy) => this._enemy = enemy; 

    public void ChangeState(EnemyState newState)
    {
        if (CurrentState != null)      
            CurrentState.Exit();       
        CurrentState = newState;
        CurrentState.Enter();
    }
}
