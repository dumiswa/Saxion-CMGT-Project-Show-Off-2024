using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float MovementSpeed;
    public EnemyStateMachine StateMachine { get; private set; }

    protected virtual void Awake() => StateMachine = new EnemyStateMachine(this);
    protected virtual void Start() => InitializeStates();
    protected virtual void Update() => StateMachine.CurrentState?.Execute();
    protected abstract void InitializeStates();

}
