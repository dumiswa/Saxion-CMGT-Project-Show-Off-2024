using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class RotatingActuator : Actuator
    {
        [Header("Rotation Parameters")]
        [SerializeField]
        private List<Vector3> _rotations;
        [Space(5)]
        [SerializeField]
        private float _rotationLerpFactor = 0.25f;
        [SerializeField]
        private float _rotationCompleteThreshold = 0.1f;

        private Transform _desiredRotation;
        private bool _isRotating = false;
        private int _currentRotationIndex = 0;

        private void Start()
        {
            _desiredRotation = new GameObject(gameObject.name + "_desiredRotation").transform;
            _desiredRotation.SetParent(transform);
            _desiredRotation.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void Update()
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                _desiredRotation.localRotation, 
                _rotationLerpFactor
            );

            //_isRotating = transform.rotation != _desiredRotation.localRotation;
            if (Quaternion.Angle(transform.rotation, _desiredRotation.rotation) < _rotationCompleteThreshold)    
                _isRotating = false; 
            
        }

        public override void Invoke()
        {         
            if (_isRotating)
            {
                Debug.Log("Is rotating: " + _isRotating);
                return;
            }

            Debug.Log("Rotating...");
            _currentRotationIndex++;
            Debug.Log("Rotation index is: " + _currentRotationIndex + "out of" + _rotations.Count + "rotations");
            if (_currentRotationIndex >= _rotations.Count)
                _currentRotationIndex = 0;

            _desiredRotation.localEulerAngles = _rotations[_currentRotationIndex];
            Debug.Log("Desired rotation is" + _rotations[_currentRotationIndex]);
        }
    }
}