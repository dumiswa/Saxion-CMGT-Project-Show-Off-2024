using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float MovementSpeed;
    public EnemyStateMachine StateMachine { get; private set; }

    protected virtual void Awake() => StateMachine = new EnemyStateMachine(this);
    protected virtual void Start() => InitializeStates();
    protected abstract void InitializeStates();
    protected virtual void Update()
    {
        if (StateMachine.CurrentState == null)       
            Debug.Log("No current state active");       
        else   
            StateMachine.CurrentState.Execute();
    }
    protected virtual void Patrol() {}
    public void ChangeState(EnemyState newState) {}
}
