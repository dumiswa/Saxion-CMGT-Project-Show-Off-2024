using System;
using UnityEngine;

namespace Monoliths.Player
{
    public class CameraActions : Monolith
    {
        public const string CONSTRAINTS_DATA_ID = "CameraConstraintsData";
        public const string TARGET_DATA_ID = "CameraTargetData";

        private CameraConstraints _constraints;
        private CameraTarget _target;

        private float _interpolationFactor = 0.03f;
        private float _rotationSpeed = 78f;

        private Transform _cameraOrigin;
        private Transform _cameraPitch;

        public override bool Init()
        {
            _cameraOrigin = Camera.main.transform.parent.parent;
            _cameraPitch  = Camera.main.transform.parent;
            if (_cameraOrigin is null)
            {
                _status = "Couldn't Find Camera";
                _isActive = false;
                return false;
            }

            var initialTarget = GameObject.FindGameObjectWithTag("Player").transform;

            _constraints = new CameraConstraints(new(-180,180), new(-45,45));
            _target = new CameraTarget(initialTarget);

            DataBridge.UpdateData(CONSTRAINTS_DATA_ID, _constraints);
            DataBridge.UpdateData(TARGET_DATA_ID, _target);
            return base.Init();
        }

        private void LateUpdate()
        {
            TrySyncData();
            Rotate();
            Move();
        }

        private void Move()
        {
            _cameraOrigin.transform.position = Vector3.Lerp
            (
                _cameraOrigin.transform.position, 
                _target.GetTargetPosition(),
                _interpolationFactor
            );
        }

        private void Rotate()
        {
            var direction = Controls.RightDirectional;
            var target = _target.GetTargetPosition();

            var horizontalMin = Mathf.Min(_constraints.RotationHorizontal.x, _constraints.RotationHorizontal.y);
            var horizontalMax = Mathf.Max(_constraints.RotationHorizontal.x, _constraints.RotationHorizontal.y);
            var verticalMin = Mathf.Min(_constraints.RotationVertical.x, _constraints.RotationVertical.y);
            var verticalMax = Mathf.Max(_constraints.RotationVertical.x, _constraints.RotationVertical.y);

            _cameraPitch.RotateAround(target, _cameraOrigin.right, direction.y * _rotationSpeed * Time.deltaTime);
            _cameraOrigin.RotateAround(target, Vector3.up, -direction.x * _rotationSpeed * Time.deltaTime);

            var pitch = _cameraPitch.localEulerAngles.x;
            pitch = pitch <= 180? Mathf.Clamp(pitch, 0, verticalMax) : Mathf.Clamp(pitch, 360 + verticalMin, 360);
            _cameraPitch.localEulerAngles = new(pitch, 0, 0);

            var yaw = _cameraOrigin.localEulerAngles.y;
            yaw = yaw <= 180 ? Mathf.Clamp(yaw, 0, horizontalMax) : Mathf.Clamp(yaw, 360 + horizontalMin, 360);
            _cameraOrigin.localEulerAngles = new(0, yaw, 0);
        }

        private void TrySyncData()
        {
            try
            {
                var constraints = DataBridge.TryGetData<CameraConstraints>(CONSTRAINTS_DATA_ID);
                var target = DataBridge.TryGetData<CameraTarget>(TARGET_DATA_ID);

                if (constraints.WasUpdated)
                {
                    if (constraints != Data<CameraConstraints>.Empty)
                    {
                        if (!_isActive) base.Init();
                        _constraints = constraints.EncodedData;
                        DataBridge.MarkUpdateProcessed<CameraConstraints>(CONSTRAINTS_DATA_ID);
                    }
                    else if (_isActive)
                    {
                        _isActive = false;
                        _status = $"Couldn't get \"{CONSTRAINTS_DATA_ID}\" data packet";
                    }
                }
                if (target.WasUpdated)
                {
                    if (target != Data<CameraTarget>.Empty)
                    {
                        if (!_isActive) base.Init();
                        _target = target.EncodedData;
                        DataBridge.MarkUpdateProcessed<CameraTarget>(TARGET_DATA_ID);
                    }
                    else if (_isActive)
                    {
                        _isActive = false;
                        _status = $"Couldn't get \"{TARGET_DATA_ID}\" data packet";
                    }
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