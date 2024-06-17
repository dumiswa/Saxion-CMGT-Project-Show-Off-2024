using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class MovingActuator : Actuator
    {
        [SerializeField]
        private Vector3[] _positions;
        [SerializeField]
        private float _factor = 0.03f;

        private byte _index = 0;
        private Vector3 _desiredPosition;
        private void Start() 
            => _desiredPosition = transform.localPosition;   

        public override void Invoke()
        {
            _desiredPosition = _positions[(_index++)%_positions.Length];
        }

        private void Update() => transform.localPosition = Vector3.Lerp(transform.localPosition, _desiredPosition, _factor);
    }
}