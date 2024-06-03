using Monoliths.Player;
using UnityEngine;

public class Projectile : OnCollisionActuator
{
    [SerializeField]
    private float _speed = 0.5f;
    [SerializeField]
    private float _maxDistance = -1f;
    [SerializeField]
    private float _maxLifeTime = -1f;

    private float _distanceCounter;
    private float _timeCounter;

    private Vector3 _initialPosition;

    private void Start() 
        => _initialPosition = transform.position;
    
    private void Update()
    {
        transform.position -= _speed * Time.deltaTime * transform.forward;
        
        if(_maxDistance >= 0)
        {
            _distanceCounter = Vector3.Distance(_initialPosition, transform.position);
            if(_distanceCounter >= _maxDistance)
                Destroy(gameObject);
        }
        if (_maxLifeTime >= 0)
        {
            _timeCounter += Time.deltaTime;
            if (_timeCounter >= _maxLifeTime)
                Destroy(gameObject);
        }
    }
    public override void Invoke()
    {
        base.Invoke();
        DataBridge.UpdateData
        (
            PlayerResources.CURRENT_LIVES_DATA_ID, 
            (byte)(DataBridge.TryGetData<byte>(PlayerResources.CURRENT_LIVES_DATA_ID).EncodedData - 1)
        );
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.DrawSphere(transform.position - transform.forward * _maxDistance, 0.15f);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * _maxDistance);
    }
}