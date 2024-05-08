using UnityEngine;

public class PlayerMovement : Monolith
{
    private GameObject _player;
    private Rigidbody _rigidbody;

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

        return base.Init();
    }

    private void Update()
    {
    }
}
