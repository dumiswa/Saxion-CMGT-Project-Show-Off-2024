using System;
using UnityEngine;

namespace Monoliths.Player
{
    public struct CameraConstraints
    {
        private byte _movementLockInfo;
        public Vector2Int _rotationHorizontalConstraints;
        public Vector2Int _rotationVerticalConstraints;

        public CameraConstraints(
            Vector2Int rotationHorizontalConstraints, 
            Vector2Int rotationVerticalConstraints, 
            byte initialLockInfo = 0b0000_0000)
        {
            _movementLockInfo = initialLockInfo;
            _rotationHorizontalConstraints = rotationHorizontalConstraints;
            _rotationVerticalConstraints = rotationVerticalConstraints; 
        }

        public CameraConstraints(bool _ = false)
        {
            _movementLockInfo = 0b0000_0000;
            _rotationHorizontalConstraints = Vector2Int.zero;
            _rotationVerticalConstraints = Vector2Int.zero;
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
            {
                _movementLockInfo |= 0b0000_0001;
            }
            else
            {
                _movementLockInfo &= 0b1111_1110;
            }
        }
        public void SetYAxisLock(bool locked)
        {
            if (locked)
            {
                _movementLockInfo |= 0b0000_0010;
            }
            else
            {
                _movementLockInfo &= 0b1111_1101;
            }
        }
        public void SetZAxisLock(bool locked)
        {
            if (locked)
            {
                _movementLockInfo |= 0b0000_0100;
            }
            else
            {
                _movementLockInfo &= 0b1111_1011;
            }
        }

        public void SetRotationConstraints(Vector2Int horizontal, Vector2Int vertical)
        {
            SetHorizontalRotationConstraints(horizontal);
            SetVerticalRotationConstraints(vertical);
        }

        public void SetVerticalRotationConstraints(Vector2Int vertical) 
            => _rotationVerticalConstraints = vertical;
        public void SetHorizontalRotationConstraints(Vector2Int horizontal) 
            => _rotationHorizontalConstraints = horizontal;
    }
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
    }

    public class CameraActions : Monolith
    {
        public const string CONSTRAINTS_DATA_ID = "CameraConstraintsData";
        public const string TARGET_DATA_ID = "CameraTargetData";

        private CameraConstraints _constraints;
        private CameraTarget _target;

        private Camera _camera;
        public override bool Init()
        {
            _camera = Camera.main;
            if (_camera is null)
            {
                _status = "Couldn't Find Camera";
                _isActive = false;
                return false;
            }

            var initialTarget = GameObject.FindGameObjectWithTag("Player").transform;

            _constraints = new CameraConstraints(new(-180,180), new(-90,90));
            _target = new CameraTarget(initialTarget);

            DataBridge.UpdateData(CONSTRAINTS_DATA_ID, _constraints);
            DataBridge.UpdateData(TARGET_DATA_ID, _target);
            return base.Init();
        }

        private void LateUpdate()
        {
            TrySyncData();
        }
        private void TrySyncData()
        {
            try
            {
                var constraints = DataBridge.TryGetData<CameraConstraints>(CONSTRAINTS_DATA_ID);
                var target = DataBridge.TryGetData<CameraTarget>(TARGET_DATA_ID);

                if (constraints != Data<CameraConstraints>.Empty)
                {
                    if (!_isActive) base.Init();
                    _constraints = constraints.EncodedData;
                    DataBridge.MarkDataClean<CameraConstraints>(CONSTRAINTS_DATA_ID);
                }
                else if (_isActive)
                {
                    _isActive = false;
                    _status = $"Couldn't get \"{CONSTRAINTS_DATA_ID}\" data packet";
                }

                if (target != Data<CameraTarget>.Empty)
                {
                    if (!_isActive) base.Init();
                    _target = target.EncodedData;
                    DataBridge.MarkDataClean<CameraTarget>(TARGET_DATA_ID);
                }
                else if (_isActive)
                {
                    _isActive = false;
                    _status = $"Couldn't get \"{TARGET_DATA_ID}\" data packet";
                }
            }
            catch (InvalidCastException)
            {
                if (_isActive)
                {
                    _isActive = false;
                    _status = $"Stored data was not of appropriate types";
                }
            }
        }
    }
}