using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class ObjectMover : Actuator
    {
        private GameObject _player;

        private Vector3 _initialPlayerPosition;
        private Vector3 _initialObjectPosition;
        private bool _isMoving = false;

        private void Start()
        {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player");
        }

        public override void Invoke()
        {        
            if (!_isMoving) 
                StartMoving();
            else
                StopMoving();
        }
        public override void Interact(GameObject caller) => Invoke();
        
        private void StartMoving()
        {
           _initialPlayerPosition = _player.transform.position;
           _initialObjectPosition = transform.position;

           _isMoving = true;
        }
        private void StopMoving() => _isMoving = false;

        private void Update()
        {
            if (_isMoving && _player != null)
            {
                Vector3 playerMovement = _player.transform.position - _initialPlayerPosition;
                transform.position = _initialObjectPosition + playerMovement;
            }
        }
    }
}
