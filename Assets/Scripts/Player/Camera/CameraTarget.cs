using System;
using UnityEngine;

namespace Monoliths.Player
{
    [Serializable]
    public struct CameraTarget
    {
        public Transform X;
        public Transform Y;
        public Transform Z;

        public CameraTarget(
            Transform x = null,
            Transform y = null,
            Transform z = null
        ){
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
        {
            try
            {
                return new Vector3(X.position.x, Y.position.y, Z.position.z);
            }
            catch
            {
                return Vector3.zero;
            }
        } 
    }
}