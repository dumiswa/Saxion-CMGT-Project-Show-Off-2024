using Monoliths.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Monoliths.Mechanisms
{
    public class MovableLadder : TwoBActuator
    {
        [SerializeField]
        private float _maxHeight;
        [SerializeField]
        private float _minHeight;

        [Space(5)]
        [SerializeField]
        private bool _fwdOffset = true;

        private GameObject _caller;
        private Rigidbody _callerRigidbody;

        private bool _isClimbing;

        private GameObject _player;

        [SerializeField]
        private float _distanceThreshold = 0.1f;

        private Vector3 _initialOffset;
        private bool _isMoving = false;

        private void Start()
        {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player");
        }

        public void Invoke2()
        {
            if (Locked || _isClimbing)
                return;

            CalculateInitialOffset();
            StartMoving();
        }
        public override void Interact2(GameObject caller) => Invoke2();

        private void CalculateInitialOffset()
        {
            Vector3 flatPlayerPosition = new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z);
            Vector3 flatOjectPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            Vector3 directionToPlayer = (flatPlayerPosition - flatOjectPosition).normalized;
            float currentDistance = Vector3.Distance(flatOjectPosition, flatPlayerPosition);

            if (currentDistance <= _distanceThreshold)
            {
                transform.position = new Vector3(
                    flatPlayerPosition.x - directionToPlayer.x * (_distanceThreshold + 0.05f),
                    transform.position.y,
                    flatPlayerPosition.z - directionToPlayer.z * (_distanceThreshold + 0.05f));
            }
              
            _initialOffset = transform.position - flatPlayerPosition;
        }
        private void StartMoving() 
        {
            MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(false);
            _isMoving = true;
            Controls.Profile.Map.SecondContextualButton.started += StopMoving;
        }
        protected virtual void StopMoving(InputAction.CallbackContext ctx)
        {
            MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(true);
            _isMoving = false;
            Controls.Profile.Map.SecondContextualButton.started -= StopMoving;
        }

        private void Update()
        {
            if (_isMoving && _player != null)
            {
                Vector3 newPosition = _player.transform.position + _initialOffset;
                transform.position = _player.transform.position + _initialOffset;
            }

            ClimbingUpdate();
        }

        public override void Interact(GameObject caller)
        {
            if (Locked || _isMoving)
                return;

            _caller = caller;
            _caller.TryGetComponent(out _callerRigidbody);
            Invoke();
        }
        public override void Invoke() => StartClimbing();

        private void ClimbingUpdate()
        {
            if (!_isClimbing)
                return;

            if (_callerRigidbody.position.y >= _maxHeight)
                FinishClimbing(true);

            else if (_callerRigidbody.position.y <= _minHeight)
                FinishClimbing(false);
        }

        private void StartClimbing()
        {
            Locked = true;
            if (_callerRigidbody is null)
                return;

            _callerRigidbody.position = new Vector3
            (
                transform.position.x,
                _callerRigidbody.position.y + 0.15f,
                transform.position.z
            ) - (_fwdOffset ? 1f : -1f) * transform.forward * 0.25f;

            _isClimbing = true;
            DataBridge.UpdateData(PlayerMovement.ON_LADDER_DATA_ID, true);
        }
        private void FinishClimbing(bool normal)
        {
            Locked = false;
            if (_callerRigidbody is null)
                return;

            _callerRigidbody.position += (_fwdOffset ? 1f : -1f) * (normal ? 1f : -1f) * 0.5f * transform.forward;
            _isClimbing = false;
            DataBridge.UpdateData(PlayerMovement.ON_LADDER_DATA_ID, false);
        }


        private void OnDrawGizmos()
        {
            var position1 = new Vector3(transform.position.x, _minHeight, transform.position.z)
                - (_fwdOffset ? 1 : -1) * 0.25f * transform.forward;

            var position2 = new Vector3(transform.position.x, _maxHeight, transform.position.z)
                - (_fwdOffset ? 1 : -1) * 0.25f * transform.forward;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position1, position2);
            Gizmos.DrawSphere(position1, 0.15f);
            Gizmos.DrawSphere(position2, 0.15f);
        }
    }
}
