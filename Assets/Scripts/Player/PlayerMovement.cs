using System;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace Monoliths.Player
{
    public class PlayerMovement : Monolith
    {
        private const string GLIDING_UNLOCKED_DATA_ID = "GlidingIsUnlocked";

        private MovementStateMachine _stateMachine;

        private GameObject _player;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private Transform _playerOrigin;
        private Transform _cameraOrigin;

        private bool _lockMovement = false;

        public bool IsGlidingUnlocked { get; private set; }
        public bool IsGliding { get; private set; }

        private float _maxSpeed;
        private float _acceleration;

        private Vector2 _accelerationMultiplier;

        private float _gravityMultiplier;
        private float _movementMultiplier;

        private float _cutOffSpeed;

        private float _fallStartPosition;
        private float _hardLandedTime;
        private bool _isHardLanding;

        private void Defaults()
        {
            _maxSpeed = 3.6f;
            _acceleration = 0.32f;

            _accelerationMultiplier = Vector2.zero;

            _gravityMultiplier = 1.0f;
            _movementMultiplier = 1.0f;

            _cutOffSpeed = 15.0f;

            _fallStartPosition = 0.0f;
            _hardLandedTime = 0.4f;
        }

        public override bool Init()
        {
            _stateMachine = new(this);
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player is null)
            {
                _status = "Couldn't Find Player";
                return false;
            }
            _playerOrigin = _player.transform.Find("Origin");
            _player.TryGetComponent(out _rigidbody);
            if (_rigidbody is null)
            {
                _rigidbody = _player.AddComponent<Rigidbody>();
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            _rigidbody.useGravity = false;
            _player.TryGetComponent(out _collider);
            if (_collider is null)
            {
                _collider = _player.AddComponent<CapsuleCollider>();
                _collider.material = new PhysicMaterial("PlayerPhysicMaterial") { dynamicFriction = 1f, staticFriction = 0f };
            }
            _cameraOrigin = Camera.main.transform.parent.parent;

            InitializeStates();
            return base.Init();
        }
        public void SetMovementLocked(bool locked) => _lockMovement = locked;
        public void SetMovementParams(float gravityMultiplier = 1.0f, float movementMultiplier = 1.0f)
        {
            _gravityMultiplier = gravityMultiplier;
            _movementMultiplier = movementMultiplier;
        }

        private void InitializeStates() => _stateMachine.NextNoExit<PlayerGroundedState>();

        private void Update()
        {           
            TrySyncData();
            var isLanded = IsLanded();

            Move();

            if (isLanded && !_stateMachine.CurrentIs<PlayerGroundedState>() && !_isHardLanding)
            {
                if (_fallStartPosition - _rigidbody.position.y > 2f)
                    MonolithMaster.Instance.RunCoPlayerLoop(OnHardLanded());
                else             
                    _stateMachine.Next<PlayerGroundedState>();                                    
            }
            else if (!isLanded && !_stateMachine.CurrentIs<PlayerAirState>())
            {
                _fallStartPosition = _rigidbody.position.y;
                _stateMachine.Next<PlayerAirState>();
            }

            _stateMachine.Current?.Update();
        }
        private void FixedUpdate()
        {
            Accelerate();
            ApplyGravity();
        }

        private void TrySyncData()
        {
            try
            {
                var constraints = DataBridge.TryGetData<bool>(GLIDING_UNLOCKED_DATA_ID);
                if (constraints.WasUpdated)
                {
                    if (!_isActive) 
                        base.Init();

                    IsGlidingUnlocked = constraints.EncodedData;
                    DataBridge.MarkUpdateProcessed<bool>(GLIDING_UNLOCKED_DATA_ID);
                }
            }
            catch (InvalidCastException)
            {
                if (_isActive)
                {
                    _isActive = false;
                    _status = $"Stored data was not of appropriate types";
                }
            }
        }

        public void SetFriction(float frictionValue)
        {
            if (_collider != null && _collider.material != null) 
            {
                _collider.material.dynamicFriction = frictionValue;
            }
        }

        private void Accelerate()
        {
            _accelerationMultiplier /= 1f + _acceleration;
            if (!_lockMovement)
            {
                Vector2 direction = Controls.LeftDirectional;

                _accelerationMultiplier += _acceleration * 2f * direction;
                _accelerationMultiplier = new
                (
                    Mathf.Clamp(_accelerationMultiplier.x, -1f, 1f),
                    Mathf.Clamp(_accelerationMultiplier.y, -1f, 1f)
                );
            };
        }
        private void Move()
        {
            _rigidbody.MovePosition(_rigidbody.position + _maxSpeed * _movementMultiplier * Time.deltaTime * 
            (
                _accelerationMultiplier.x * _cameraOrigin.right + 
                _accelerationMultiplier.y * _cameraOrigin.forward
            ));
        }

        private void ApplyGravity()
        {
            if (_rigidbody.velocity.y <= -_cutOffSpeed)
                return;

            _rigidbody.AddForce(Vector3.down * (25f * _gravityMultiplier), ForceMode.Acceleration);
        }

        private bool IsLanded() 
            => Physics.OverlapSphere(_playerOrigin.transform.position, 0.5125f)
                      .Where(result => result.gameObject.layer == LayerMask.NameToLayer("Ground"))
                      .ToArray().Length > 0;
        private IEnumerator OnHardLanded()
        {
            _isHardLanding = true;
            yield return new WaitForSeconds(_hardLandedTime);
            _isHardLanding = false;

            _stateMachine.Next<PlayerGroundedState>();
        }

        private void OnGlideButtonPressed() => IsGliding = true;
        private void OnGlideButtonReleased() => IsGliding = false;

        private void OnEnable()
        {
            Defaults();

            Controls.Profile.Map.FirstContextualButton.started += ctx => OnGlideButtonPressed();
            Controls.Profile.Map.FirstContextualButton.canceled += ctx => OnGlideButtonReleased();
        }
        private void OnDisable()
        {
            Defaults();

            Controls.Profile.Map.FirstContextualButton.started -= ctx => OnGlideButtonPressed();
            Controls.Profile.Map.FirstContextualButton.performed -= ctx => OnGlideButtonReleased();
        }
    }
}
