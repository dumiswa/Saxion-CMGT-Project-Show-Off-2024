public abstract class EnemyState
{
    protected Enemy _enemy;

    public EnemyState(Enemy enemy) => this._enemy = enemy;
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
