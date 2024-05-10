using UnityEngine;

namespace Monoliths.Player
{
    public class PlayerMovement : Monolith
    {
        private GameObject _player;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private bool _enableMovement = true;

        public override bool Init()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player is null)
            {
                _status = "Couldn't Find Player";
                return false;
            }
            _player.TryGetComponent(out _rigidbody);
            if (_rigidbody is null)
            {
                _rigidbody = _player.AddComponent<Rigidbody>();
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            _player.TryGetComponent(out _collider);
            if(_collider is null)
            {
                _collider = _player.AddComponent<CapsuleCollider>();
                _collider.material = new PhysicMaterial("PlayerPhysicMaterial") { dynamicFriction = 1f, staticFriction = 0f };
            }

            InitializeStates();
            return base.Init();
        }

        private void InitializeStates() => GameStateMachine.Instance.Next(new PlayerWalkingState(this));

        public void SetMovementEnabled(bool enabled) => _enableMovement = enabled;

        public void PerformJump()
        {
            if (_enableMovement) 
            {
                _rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
                GameStateMachine.Instance.Next(new PlayerFallingState(this));   
            }
        }

        private void Update()
        {
            if (_enableMovement) 
            {
                Vector2 movementInput = Controls.Movement;
                Vector3 move = new Vector3(movementInput.x, 0, movementInput.y) * Time.deltaTime * 5.0f;
                _rigidbody.MovePosition(_rigidbody.position + move);
            }       

            if (Controls.JumpPressed && GameStateMachine.Instance.CurrentIs<PlayerWalkingState>())
            {
                GameStateMachine.Instance.Next(new PlayerJumpingState(this));
            }

            if (!IsTouchingGround() && GameStateMachine.Instance.Current is PlayerWalkingState) 
            {
                GameStateMachine.Instance.Next(new PlayerFallingState(this));
            }
        }

        private bool IsTouchingGround()
        {
            return Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hit, 1.1f);
        }
    }
}

