using UnityEngine;
public class ShootingEnemy : Enemy
{
    [SerializeField]
    private Vector3 _shootingDirection;
    [SerializeField]
    [Range(0.25f, 4f)]
    private float _shootingFrequency;
    [SerializeField]
    private Projectile _projectilePrefab;

    protected override void InitializeStates()
    {
        StateMachine.ChangeState(new ShootingState(this, 
            _shootingDirection.normalized,
            _shootingFrequency, 
            _projectilePrefab
            )
        );
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + _shootingDirection.normalized);
        Gizmos.DrawSphere(transform.position + _shootingDirection.normalized, 0.15f);
    }
}

public class ShootingState : EnemyState
{
    private Vector3 _shootingDirection;
    private float _shootingFrequency;

    private Projectile _projectilePrefab;
    private AudioSource _audioSource;
    private AudioClip _shootingClip;

    private float _counter;

    public ShootingState(ShootingEnemy enemy, Vector3 direction , float frequency, Projectile prefab) : base(enemy)
    {
        _shootingDirection = direction;
        _shootingFrequency = frequency;
        _projectilePrefab = prefab;
    }

    public override void Enter() => _counter = 0;

    public override void Execute()
    {
        _counter+= Time.deltaTime;
        if(_counter > _shootingFrequency)
        {
            _counter = 0;
            Spawn();
        }
    }

    private void Spawn()
    {
        var instance = Object.Instantiate(_projectilePrefab, _enemy.transform.position, Quaternion.identity);
        instance.transform.forward = -_shootingDirection;
    }

    public override void Exit(){ }
}
