using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Monolith
{
    private GameObject _player;
    private Rigidbody _rigidbody;
    private PlayerProfile _controls;
    private Vector2 _currentMovementInput;

    public override bool Init()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player is null)
        {
            MonolithStatus = "Couldn't Find Player";
            return false;
        }
        _player.TryGetComponent(out _rigidbody);
        if(_rigidbody is null)
        {
            _rigidbody = _player.AddComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        _controls = new PlayerProfile();
        _controls.PlayerMovemenMap.Move.performed += OnMovementInput;
        _controls.PlayerMovemenMap.Move.canceled += OnMovementInputCanceled;
        _controls.PlayerMovemenMap.Move.Enable();

        return base.Init();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
    }

    private void OnMovementInputCanceled(InputAction.CallbackContext context)
    {
        _currentMovementInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(_currentMovementInput.x, 0, _currentMovementInput.y) * Time.fixedDeltaTime * 5.0f;
        _rigidbody.MovePosition(_rigidbody.position + move);
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Update()
    {
    }
}
