using Monoliths.Player;
using UnityEngine;

public class CameraEvent : OnCollisionActuator
{
    [SerializeField]
    private CameraConstraints _constraints = new(new(-180, 180), new(0, 45), new(-32, -48));

    [Header("Null = Player")]
    [SerializeField]
    private CameraTarget _target;
    private CameraTarget _validatedTarget;

    private Transform _player;

    protected virtual void Start()
    {  
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        ValidateTarget();
    }
    protected virtual void OnValidate() 
        => ValidateTarget();
 
    private void ValidateTarget() =>
        _validatedTarget = new()
        {
            X = _target.X != null ? _target.X : _player,
            Y = _target.Y != null ? _target.Y : _player,
            Z = _target.Z != null ? _target.Z : _player
        };


    public override void Invoke()
    {
        DataBridge.UpdateData(CameraActions.CONSTRAINTS_DATA_ID, _constraints);
        DataBridge.UpdateData(CameraActions.TARGET_DATA_ID, _validatedTarget);
    }
}

