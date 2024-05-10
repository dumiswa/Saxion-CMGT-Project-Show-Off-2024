using System;
using UnityEngine;
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

        private bool _lockMovement = false;

        public bool IsGlidingUnlocked { get; private set; }
        public bool IsGliding { get; private set; }

        private float _speed = 5.0f;

        private float _gravityMultiplier = 1.0f;
        private float _movementMultiplier = 1.0f;

        private float _cutOffSpeed = 15f;

        public override bool Init()
        {
            DataBridge.UpdateData(GLIDING_UNLOCKED_DATA_ID, true); //DEBUG

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

            if (!_lockMovement) 
                Move();

            if (isLanded && !_stateMachine.CurrentIs<PlayerGroundedState>())
                _stateMachine.Next<PlayerGroundedState>();

            else if(!isLanded && !_stateMachine.CurrentIs<PlayerAirState>())
                _stateMachine.Next<PlayerAirState>();

            _stateMachine.Current?.Update();
        }
        private void FixedUpdate()
        {
            if (_rigidbody.velocity.y <= -_cutOffSpeed)
                return;
            
            _rigidbody.AddForce(Vector3.down * (25f * _gravityMultiplier), ForceMode.Acceleration);
        }

        private void TrySyncData()
        {
            try
            {
                var constraints = DataBridge.TryGetData<bool>(GLIDING_UNLOCKED_DATA_ID);

                if (constraints.WasUpdated)
                {
                    if (constraints != Data<bool>.Empty)
                    {
                        if (!_isActive) base.Init();
                        IsGlidingUnlocked = constraints.EncodedData;
                        DataBridge.MarkUpdateProcessed<bool>(GLIDING_UNLOCKED_DATA_ID);
                    }
                    else if (_isActive)
                    {
                        _isActive = false;
                        _status = $"Couldn't get \"{GLIDING_UNLOCKED_DATA_ID}\" data packet";
                    }
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

        private void Move()
        {
            Vector2 direction = Controls.Movement;
            Vector3 move = _speed * _movementMultiplier * Time.deltaTime * new Vector3(direction.x, 0, direction.y);
            _rigidbody.MovePosition(_rigidbody.position + move);
        }

        private bool IsLanded()
        {
            foreach (var result in Physics.OverlapSphere(_playerOrigin.transform.position, 0.25f))
            {
                if(result.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    return true;
            }
            return false;
        }
       
        private void OnGlideButtonPressed() => IsGliding = true;
        private void OnGlideButtonReleased() => IsGliding = false;

        private void OnEnable()
        {
            Controls.Player.PlayerMovementMap.Jump.started += ctx => OnGlideButtonPressed();
            Controls.Player.PlayerMovementMap.Jump.canceled += ctx => OnGlideButtonReleased();
        }
        private void OnDisable()
        {
            Controls.Player.PlayerMovementMap.Jump.started -= ctx => OnGlideButtonPressed();
            Controls.Player.PlayerMovementMap.Jump.performed -= ctx => OnGlideButtonReleased();
        }
    }
}
