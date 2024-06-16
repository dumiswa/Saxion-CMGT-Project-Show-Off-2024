using System;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class ObjectMover : Actuator
    {
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

        public override void Invoke()
        {        
            if (!_isMoving)
            {
                CalculateInitialOffset();
                StartMoving();
            }
            else StopMoving();
        }
        public override void Interact(GameObject caller) => Invoke();
       
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
        private void StartMoving() => _isMoving = true;      
        private void StopMoving() => _isMoving = false;

        private void Update()
        {
            if (_isMoving && _player != null)
            {
                Vector3 newPosition = _player.transform.position + _initialOffset;
                transform.position = _player.transform.position + _initialOffset;
            }
        }
    }
}
