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

        if (CurrentState != null)   
            CurrentState.Enter();    
        else      
            Debug.LogError("Failed to change state: newState is null");      
    }
}
