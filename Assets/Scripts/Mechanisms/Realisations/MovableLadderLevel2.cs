using UnityEngine;
using UnityEngine.InputSystem;

namespace Monoliths.Mechanisms
{
    public class MovableLadderLevel2 : MovableLadder
    {
        [SerializeField]
        private Transform _a;
        [SerializeField]
        private Transform _b;
        [SerializeField]
        private float _xPositionPredicate;
        protected override void StopMoving(InputAction.CallbackContext ctx)
        {
            base.StopMoving(ctx);
            transform.SetParent(transform.position.x < _xPositionPredicate? _a: _b);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(new Vector3(_xPositionPredicate, transform.position.y, transform.position.z), 0.5f);
        }
    }
}
