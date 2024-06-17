using System.Collections;
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
        [Space(5)]
        [Header("PARAMETERS")]
        [SerializeField]
        private float _followingRadius;
        [SerializeField]
        private float _maxSpeed;
        [SerializeField]
        private float _accelerationMultiplier;

        [Space(5)]
        [Header("Activity code animator")]
        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        [Range(0, 1)]
        private float _changeStep;
        [SerializeField]
        [Range(3, 16)]
        private float _visibleDistance;

        private Vector2 _accelerationBuffer;
        private Vector3 _direction;

        private GameObject _caller;
        private Animator _animator;
        private static Transform _player;

        private bool _isVisible;
        private bool _finalized;

        private void Start()
        {
            DataBridge.UpdateData<Invizibilo>(INVIZIBILO_ACCOMPANING, null);

            _animator = transform.Find("Display").GetComponent<Animator>();
            _player ??= GameObject.FindGameObjectWithTag("Player").transform;
            _state = State.Inactive;
        }
        public override void Interact(GameObject caller)
        {
            if (_finalized)
                return;

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
            AnimateVisibility();
            if (_finalized)
                return;

            var data = DataBridge.TryGetData<Invizibilo>(INVIZIBILO_ACCOMPANING).EncodedData;
            if (data == this)
                _isVisible = true;
            else if (data == null)
            {
                if (Vector3.Distance(_player.position, transform.position) < _visibleDistance &&
                    Mathf.Abs(_player.position.y - transform.position.y) < _visibleDistance / 4)
                    _isVisible = true;
                else
                    _isVisible = false;
            }
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
            if (_finalized)
                return;

            _accelerationBuffer += _accelerationMultiplier * Time.deltaTime * 
                new Vector2(_direction.x, _direction.z);

            if(_direction == Vector3.zero)
            {
                _accelerationBuffer = Vector2.zero;
            }

            var buffer = new Vector2
            (
                Mathf.Clamp(Mathf.Abs(_accelerationBuffer.x), 0, _maxSpeed), 
                Mathf.Clamp(Mathf.Abs(_accelerationBuffer.y), 0, _maxSpeed)
            );
            transform.position += new Vector3
            (
                _direction.x * buffer.x, 
                0, 
                _direction.z * buffer.y
            ) * Time.deltaTime;

            _animator.SetFloat("Velocity", buffer.magnitude * 2f);
        }

        private void AnimateVisibility()
        {
            var color = _renderer.material.GetColor("_Color");
            var opacity = color.a;
            if (_isVisible)
            {
                if (opacity < 1)
                    opacity += _changeStep * Time.deltaTime;
            }
            else if (opacity > 0) 
                opacity -= _changeStep * Time.deltaTime;

            _renderer.material.SetColor("_Color", 
                new Color(color.r, color.g, color.b, opacity));
        }

        public void FinalizeAct() => StartCoroutine(Finalisation());

        private IEnumerator Finalisation()
        {
            _finalized = true;
            _animator.SetBool("IsEating", true);
            _state = State.Inactive;
            _isVisible = false;
            _changeStep = 2f;
            DataBridge.UpdateData<Invizibilo>(INVIZIBILO_ACCOMPANING, null);
            yield return new WaitForSeconds(1.4f);
            Destroy(gameObject);
        }
    }
}
