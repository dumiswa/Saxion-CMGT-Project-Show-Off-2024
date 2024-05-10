using UnityEngine;

namespace Monoliths.Player
{
    public struct CameraConstraints
    {
        private byte _movementLockInfo;
        public Vector2Int RotationHorizontal;
        public Vector2Int RotationVertical;

        public CameraConstraints(
            Vector2Int rotationHorizontalConstraints, 
            Vector2Int rotationVerticalConstraints, 
            byte initialLockInfo = 0b0000_0000)
        {
            _movementLockInfo = initialLockInfo;
            RotationHorizontal = rotationHorizontalConstraints;
            RotationVertical = rotationVerticalConstraints; 
        }

        public CameraConstraints(bool _ = false)
        {
            _movementLockInfo = 0b0000_0000;
            RotationHorizontal = Vector2Int.zero;
            RotationVertical = Vector2Int.zero;
        }

        public bool IsXAxisLocked() => (_movementLockInfo & 0b0000_0001) != 0;
        public bool IsYAxisLocked() => (_movementLockInfo & 0b0000_0010) != 0;
        public bool IsZAxisLocked() => (_movementLockInfo & 0b0000_0100) != 0;

        public void SetMovementLock(bool x, bool y, bool z)
        {
            SetXAxisLock(x);
            SetYAxisLock(y);
            SetZAxisLock(z);
        }

        public void SetXAxisLock(bool locked)
        {
            if (locked)
                _movementLockInfo |= 0b0000_0001;
            else
                _movementLockInfo &= 0b1111_1110;
        }
        public void SetYAxisLock(bool locked)
        {
            if (locked)
                _movementLockInfo |= 0b0000_0010;
            else
                _movementLockInfo &= 0b1111_1101;
        }
        public void SetZAxisLock(bool locked)
        {
            if (locked)
                _movementLockInfo |= 0b0000_0100;
            else
                _movementLockInfo &= 0b1111_1011;
        }

        public void SetRotationConstraints(Vector2Int horizontal, Vector2Int vertical)
        {
            SetHorizontalRotationConstraints(horizontal);
            SetVerticalRotationConstraints(vertical);
        }

        public void SetVerticalRotationConstraints(Vector2Int vertical) 
            => RotationVertical = vertical;
        public void SetHorizontalRotationConstraints(Vector2Int horizontal) 
            => RotationHorizontal = horizontal;
    }
}