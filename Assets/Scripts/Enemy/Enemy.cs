using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float MovementSpeed = 4.0f;
    public EnemyStateMachine StateMachine { get; private set; }

    protected virtual void Awake() => StateMachine = new EnemyStateMachine(this);
    protected virtual void Start() => InitializeStates();
    protected virtual void Update()
    {
        if (StateMachine.CurrentState != null)   
            StateMachine.CurrentState.Execute();    
        else       
            Debug.LogError("No active state during Update.");
        
    }
    protected abstract void InitializeStates();
}
