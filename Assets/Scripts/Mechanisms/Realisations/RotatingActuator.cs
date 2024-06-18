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
        protected bool _isRotating = false;
        private int _currentRotationIndex = 0;

        protected virtual void Start()
        {
            _desiredRotation = new GameObject(gameObject.name + "_desiredRotation").transform;
            _desiredRotation.SetParent(transform);
            _desiredRotation.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        protected virtual void Update()
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                _desiredRotation.localRotation, 
                _rotationLerpFactor
            );

            if (Quaternion.Angle(transform.rotation, _desiredRotation.rotation) < _rotationCompleteThreshold)    
                _isRotating = false; 
            
        }

        public override void Invoke()
        {         
            if (_isRotating)
                return;

            _currentRotationIndex++;
            if (_currentRotationIndex >= _rotations.Count)
                _currentRotationIndex = 0;

            _desiredRotation.localEulerAngles = _rotations[_currentRotationIndex];
        }
    }
}