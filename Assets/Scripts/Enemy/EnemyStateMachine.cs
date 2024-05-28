using UnityEngine;

public class EnemyStateMachine
{
    private Enemy _enemy;
    public EnemyState CurrentState { get; private set; }

    public EnemyStateMachine(Enemy enemy)
    {
        this._enemy = enemy;
        Debug.Log("State machine created for: " + _enemy.gameObject.name);
    }

    public void ChangeState(EnemyState newState)
    {
        if (CurrentState != null)
        {
            Debug.Log("Exiting State: " + CurrentState.GetType().Name);
            CurrentState.Exit();
        }
        CurrentState = newState;
        Debug.Log("Changing to New State: " + CurrentState.GetType().Name);
        CurrentState.Enter();
    }
}
