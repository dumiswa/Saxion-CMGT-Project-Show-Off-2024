using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField]
    private float _pitchMultiplier;

    private Transform _cameraPitch;
    private Transform _cameraYaw;

    private Vector3 _initialRotation;

    private void Start()
    {
        _cameraPitch = Camera.main.transform.parent;
        _cameraYaw = _cameraPitch.parent;
        _initialRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(
            Mathf.LerpAngle(_initialRotation.x, _cameraPitch.localRotation.eulerAngles.x, _pitchMultiplier),
            _initialRotation.y + _cameraYaw.localRotation.eulerAngles.y,
            _initialRotation.z);
    }
}