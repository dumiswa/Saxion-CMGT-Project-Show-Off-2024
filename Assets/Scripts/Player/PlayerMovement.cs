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

        private Transform _castPoint1;
        private Transform _castPoint2;
        private Transform _castPoint3;
        private Transform _castPoint4;

        private Vector2 _directionLock1;
        private Vector2 _directionLock2;
        private Vector2 _directionLock3;
        private Vector2 _directionLock4;

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
            _playerOrigin = _player.transform.Find("Origin");
            _player.TryGetComponent(out _rigidbody);
            _player.TryGetComponent(out _collider);
            if (_collider is null)
            {
                _collider = _player.AddComponent<CapsuleCollider>();
                _collider.material = new PhysicMaterial("PlayerPhysicMaterial") { dynamicFriction = 1f, staticFriction = 0f };
            }
            _cameraOrigin = Camera.main.transform.parent.parent;

            _animator = _player.transform.Find("Display").GetComponent<Animator>();

            var casts = _player.transform.Find("ColliderCasts");
            _castPoint1 = casts.GetChild(0);
            _castPoint2 = casts.GetChild(1);
            _castPoint3 = casts.GetChild(2);
            _castPoint4 = casts.GetChild(3);

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

            DefineDirectionLocks();

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

        private void DefineDirectionLocks()
        {
            var solids1 = Physics.OverlapSphere(_castPoint1.position, 0.2f)
                                .Where(result => !result.isTrigger);
            var solids2 = Physics.OverlapSphere(_castPoint2.position, 0.2f)
                                .Where(result => !result.isTrigger);
            var solids3 = Physics.OverlapSphere(_castPoint3.position, 0.2f)
                                .Where(result => !result.isTrigger);
            var solids4 = Physics.OverlapSphere(_castPoint4.position, 0.2f)
                                .Where(result => !result.isTrigger);


            _directionLock1 = solids1.Any() ? _castPoint1.forward : Vector2.zero;
            _directionLock2 = solids2.Any() ? _castPoint2.forward : Vector2.zero;
            _directionLock3 = solids3.Any() ? _castPoint3.forward : Vector2.zero;
            _directionLock4 = solids4.Any() ? _castPoint4.forward : Vector2.zero;
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
                    Vector2 direction = Controls.LeftDirectional;
                    Vector2 newAccelerationMultiplier = _accelerationMultiplier;

                    if (_directionLock1 != Vector2.zero && Vector2.Dot(direction, _directionLock1) > 0)
                    {
                        newAccelerationMultiplier.x = Mathf.Min(newAccelerationMultiplier.x, 0);
                        newAccelerationMultiplier.y = Mathf.Min(newAccelerationMultiplier.y, 0);
                    }
                    if (_directionLock2 != Vector2.zero && Vector2.Dot(direction, _directionLock2) > 0)
                    {
                        newAccelerationMultiplier.x = Mathf.Min(newAccelerationMultiplier.x, 0);
                        newAccelerationMultiplier.y = Mathf.Min(newAccelerationMultiplier.y, 0);
                    }
                    if (_directionLock3 != Vector2.zero && Vector2.Dot(direction, _directionLock3) > 0)
                    {
                        newAccelerationMultiplier.x = Mathf.Min(newAccelerationMultiplier.x, 0);
                        newAccelerationMultiplier.y = Mathf.Min(newAccelerationMultiplier.y, 0);
                    }
                    if (_directionLock4 != Vector2.zero && Vector2.Dot(direction, _directionLock4) > 0)
                    {
                        newAccelerationMultiplier.x = Mathf.Min(newAccelerationMultiplier.x, 0);
                        newAccelerationMultiplier.y = Mathf.Min(newAccelerationMultiplier.y, 0);
                    }

                    _accelerationMultiplier = newAccelerationMultiplier / (1f + _acceleration);
                    _accelerationMultiplier += _acceleration * 2f * direction;

                    _accelerationMultiplier = new Vector2(
                        Mathf.Clamp(_accelerationMultiplier.x, -1f, 1f),
                        Mathf.Clamp(_accelerationMultiplier.y, -1f, 1f)
                    );

                    Debug.Log(_accelerationMultiplier);

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
