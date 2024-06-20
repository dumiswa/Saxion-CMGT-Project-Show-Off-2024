using System;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace Monoliths.Player
{
    public class PlayerMovement : Monolith
    {
        public const string GLIDING_UNLOCKED_DATA_ID = "GlidingIsUnlocked";
        public const string MOVEMENT_ENABLED_DATA_ID = "MovementIsEnabled";
        public const string SIMULATION_ENABLED_DATA_ID = "RigidbodyIsEnabled";
        public const string ON_LADDER_DATA_ID = "IsClimbingLadder";

        private MovementStateMachine _stateMachine;

        private GameObject _player;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private Transform _playerOrigin;
        private Transform _cameraOrigin;

        private bool _lockMovement = false;
        private bool _lockX = false;
        private bool _lockZ = false;
        private bool _swapZtoY = false;

        public bool IsGlidingUnlocked { get; private set; }
        public bool IsGliding { get; private set; }

        private float _maxSpeed;

        private float _acceleration;
        private Vector2 _accelerationMultiplier;

        private float _climbAcceleration;
        private float _climbMultiplier;

        private float _gravityMultiplier;
        private float _movementMultiplier;

        private float _cutOffSpeed;

        private float _fallStartPosition;
        private float _hardLandedTime;
        private bool _isHardLanding;

        private bool _simulationEnabled;

        public override void Defaults()
        {
            base.Defaults();

            _maxSpeed = 6.6f;
            _acceleration = 0.32f;

            _accelerationMultiplier = Vector2.zero;

            _gravityMultiplier = 1.0f;
            _movementMultiplier = 1.0f;

            _cutOffSpeed = 15.0f;

            _fallStartPosition = 0.0f;
            _hardLandedTime = 0.4f;

            _simulationEnabled = false;

            _climbAcceleration = 4.0f;
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
            _rigidbody ??= _player.AddComponent<Rigidbody>();

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rigidbody.useGravity = false;

            _player.TryGetComponent(out _collider);
            if (_collider is null)
            {
                _collider = _player.AddComponent<CapsuleCollider>();
                _collider.material = new PhysicMaterial("PlayerPhysicMaterial") { dynamicFriction = 1f, staticFriction = 0f };
            }
            _cameraOrigin = Camera.main.transform.parent.parent;

            _animator = _player.transform.Find("Display").GetComponent<Animator>();

            InitializeStates();
            return base.Init();
        }

        public void SetMovementLocked(bool locked = false, bool lockX = false, bool lockZ = false, bool swapZtoY = false) 
        { 
            _lockMovement = locked;
            _lockX = lockX;
            _lockZ = lockZ;
            _swapZtoY = swapZtoY;
        }
        public void SetMovementParams(float gravityMultiplier = 1.0f, float movementMultiplier = 1.0f)
        {
            _gravityMultiplier = gravityMultiplier;
            _movementMultiplier = movementMultiplier;
        }

        private void InitializeStates() 
            => _stateMachine.NextNoExit<PlayerGroundedState>();

        private void Update()
        {
            TrySyncData();
            if (!_simulationEnabled)
                return;

            var isLanded = IsLanded();

            Move();
            Animate(isLanded);

            if (!_swapZtoY)
            {
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
            }

            _stateMachine.Current?.Update();
        }
        private void Animate(bool isLanded)
        {
            _animator.SetBool("IsClimbing", _swapZtoY);
            _animator.SetBool("IsFalling", !isLanded);
            _animator.SetBool("IsGliding", IsGlidingUnlocked && IsGliding);
        }

        public override bool SetActive(bool state)
        {
            StopAnimations();
            return base.SetActive(state);
        }

        private void StopAnimations()
        {
            _animator.SetFloat("Velocity", 0f);
            _animator.SetBool("IsClimbing", false);
            _animator.SetBool("IsFalling", false);
            _animator.SetBool("IsGliding", false);
        }

        private void FixedUpdate()
        {
            if (!_simulationEnabled)
                return;

            Accelerate();
            if(!_swapZtoY)
                ApplyGravity();
        }

        private void TrySyncData()
        {
            try
            {
                var gliding = DataBridge.TryGetData<bool>(GLIDING_UNLOCKED_DATA_ID);
                if (gliding.WasUpdated)
                {
                    IsGlidingUnlocked = gliding.EncodedData;
                    DataBridge.MarkUpdateProcessed<bool>(GLIDING_UNLOCKED_DATA_ID);
                }
                var movementEnabled = DataBridge.TryGetData<bool>(MOVEMENT_ENABLED_DATA_ID);
                if (movementEnabled.WasUpdated)
                {
                    ResetVelocity();
                    SetMovementLocked(!movementEnabled.EncodedData);
                    DataBridge.MarkUpdateProcessed<bool>(MOVEMENT_ENABLED_DATA_ID);
                }
                var simualtionEnabled = DataBridge.TryGetData<bool>(SIMULATION_ENABLED_DATA_ID);
                if (simualtionEnabled.WasUpdated)
                {
                    ResetVelocity();
                    _simulationEnabled = simualtionEnabled.EncodedData;
                    DataBridge.MarkUpdateProcessed<bool>(SIMULATION_ENABLED_DATA_ID);
                }
                var ladderInteracted = DataBridge.TryGetData<bool>(ON_LADDER_DATA_ID);
                if (ladderInteracted.WasUpdated)
                {
                    if (ladderInteracted.EncodedData)
                        _stateMachine.Next<PlayerClimbingState>();
                    else
                        _stateMachine.Next<PlayerGroundedState>();

                    DataBridge.MarkUpdateProcessed<bool>(ON_LADDER_DATA_ID);
                }
                if (!IsActive)
                    base.Init();
            }
            catch (InvalidCastException)
            {
                if (IsActive)
                {
                    IsActive = false;
                    _status = $"Stored data was not of appropriate types";
                }
            }
        }

        private void Accelerate()
        {
            if (!_lockMovement)
            {
                if (_swapZtoY)
                {
                    _climbMultiplier /= 1f + _climbAcceleration;

                    Vector2 direction = Controls.LeftDirectional;

                    _climbMultiplier += _climbAcceleration * 2f * direction.y;
                    _climbMultiplier = Mathf.Clamp(_climbMultiplier, -1f, 1f);
                    _animator.SetFloat("Velocity", _climbMultiplier * 2f);
                }
                else
                {
                    _accelerationMultiplier /= 1f + _acceleration;

                    Vector2 direction = Controls.LeftDirectional;

                    _accelerationMultiplier += _acceleration * 2f * direction;
                    _accelerationMultiplier = new
                    (
                        Mathf.Clamp(_accelerationMultiplier.x, -1f, 1f),
                        Mathf.Clamp(_accelerationMultiplier.y, -1f, 1f)
                    );
                    _animator.SetFloat("Velocity", _accelerationMultiplier.magnitude * 2f);
                }
            }
        }
        private void Move()
        {
            _rigidbody.MovePosition(_rigidbody.position + _maxSpeed * _movementMultiplier * Time.deltaTime * 
            (
                (_lockX? 0 : _accelerationMultiplier.x) * _cameraOrigin.right + 
                (_lockZ? 0 : (_swapZtoY? _climbMultiplier : _accelerationMultiplier.y)) * (_swapZtoY? _cameraOrigin.up : _cameraOrigin.forward)
            ));
        }

        public void SetFriction(float frictionValue)
        {
            if (_collider is null || _collider.material is null)
                return;

            _collider.material.dynamicFriction = frictionValue;
        }

        public void ResetVelocity()
        {
            _accelerationMultiplier = Vector2.zero;
            _climbMultiplier = 0;

            _rigidbody.velocity = Vector3.zero;
        }
        public void ResetPosition()
        {
            ResetVelocity();
            _rigidbody.position = Vector3.zero;
        }

        public void Translate(Vector3 position) 
            => _rigidbody.position = position;
        public Vector3 GetPosition()
             => _rigidbody.position;

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
