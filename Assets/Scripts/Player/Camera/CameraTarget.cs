using UnityEngine;

namespace Monoliths.Player
{
    public struct CameraTarget
    {
        public Transform X;
        public Transform Y; 
        public Transform Z;

        public CameraTarget(
            Transform x = null, 
            Transform y = null, 
            Transform z = null)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public CameraTarget(Transform target = null)
        {
            X = target;
            Y = target;
            Z = target;
        }

        public Vector3 GetTargetPosition() 
            => new Vector3(X.position.x, Y.position.y, Z.position.z);
    }
}