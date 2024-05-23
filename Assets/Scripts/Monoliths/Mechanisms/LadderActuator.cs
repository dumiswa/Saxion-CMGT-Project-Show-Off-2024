using Monoliths.Player;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class LadderActuator : Actuator
    {
        [SerializeField]
        private float _maxHeight;
        [SerializeField]
        private float _minHeight;

        private GameObject _caller;
        private Rigidbody _callerRigidbody;

        private bool _isClimbing;

        public override void Interact(GameObject caller)
        {
            _caller = caller;
            _caller.TryGetComponent(out _callerRigidbody);
            Invoke();
        }
        public override void Invoke() => StartClimbing();

        private void Update()
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
            ) - transform.forward * 0.25f;

            _isClimbing = true;
            DataBridge.UpdateData(PlayerMovement.ON_LADDER_DATA_ID, true);
        }

        private void FinishClimbing(bool normal)
        {
            Locked = false;
            if (_callerRigidbody is null)
                return;

            _callerRigidbody.position += (normal? 1f : -1f) * transform.forward * 0.5f;
            _isClimbing = false;
            DataBridge.UpdateData(PlayerMovement.ON_LADDER_DATA_ID, false);
        }

        private void OnDrawGizmos()
        {
            var position1 = new Vector3(transform.position.x, _minHeight, transform.position.z) - transform.forward * 0.25f;
            var position2 = new Vector3(transform.position.x, _maxHeight, transform.position.z) - transform.forward * 0.25f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position1, position2);
            Gizmos.DrawSphere(position1, 0.15f);
            Gizmos.DrawSphere(position2, 0.15f);
        }
    }
}
