using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class Invizibilo : Actuator
    {
        public const string INVIZIBILO_ACCOMPANING = "CurrentInvizibiloCompanion";
        private enum State
        {
            Inactive,
            Active,
        }

        [SerializeField]
        private State _state;
        private GameObject _caller;

        [Space(5)]
        [Header("PARAMETERS")]
        [SerializeField]
        private float _followingRadius;
        [SerializeField]
        private float _maxSpeed;
        [SerializeField]
        private float _accelerationMultiplier;
        
        private Vector2 _accelerationBuffer;
        private Vector3 _direction;

        private void Start()
        {
            DataBridge.UpdateData<Invizibilo>(INVIZIBILO_ACCOMPANING, null);
            _state = State.Inactive;
        }
        public override void Interact(GameObject caller)
        {
            if (Locked)
                return;

            _caller = caller;
            Invoke();
        }
        public override void Invoke()
        {
            if (_state == State.Inactive)
            {
                Locked = true;
                _state = State.Active;

                DataBridge.UpdateData(INVIZIBILO_ACCOMPANING, this);
            }
        }
        private void Update()
        {
            if(_state == State.Active)
            {
                _direction = Vector3.Distance(_caller.transform.position, transform.position) > _followingRadius? 
                            (_caller.transform.position - transform.position).normalized : 
                            Vector3.zero;
            }

            Move();
        }

        private void Move()
        {
            _accelerationBuffer += _accelerationMultiplier * Time.deltaTime * (
                new Vector2(_direction.x, _direction.z));
            _accelerationBuffer /= Mathf.Clamp(0.5f * Time.deltaTime, 0.001f, 5f);

            _accelerationBuffer = new Vector2
            (
                Mathf.Clamp(Mathf.Abs(_accelerationBuffer.x), 0, _maxSpeed), 
                Mathf.Clamp(Mathf.Abs(_accelerationBuffer.y), 0, _maxSpeed)
            );
            transform.position += new Vector3
            (
                _direction.x * _accelerationBuffer.x, 
                0, 
                _direction.z * _accelerationBuffer.y
            ) * Time.deltaTime;
        }

        public void FinalizeAct()
        {
            _state = State.Inactive;
            DataBridge.UpdateData<Invizibilo>(INVIZIBILO_ACCOMPANING, null);
            Destroy(gameObject);
        }
    }
}
