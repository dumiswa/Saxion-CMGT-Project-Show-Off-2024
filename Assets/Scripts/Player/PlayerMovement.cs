using UnityEngine;

namespace Monoliths.Player
{
    public class PlayerMovement : Monolith
    {
        private GameObject _player;
        private Rigidbody _rigidbody;
        private Collider _collider;

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

            return base.Init();
        }

        private void Update()
        {
            Vector2 movementInput = Controls.Movement;
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y) * Time.deltaTime * 5.0f;
            _rigidbody.MovePosition(_rigidbody.position + move);
        }
    }
}

