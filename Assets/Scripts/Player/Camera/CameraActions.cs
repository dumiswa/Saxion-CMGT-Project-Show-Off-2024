using System;
using UnityEngine;

namespace Monoliths.Player
{
    public class CameraActions : Monolith
    {
        public const string CONSTRAINTS_DATA_ID = "CameraConstraintsData";
        public const string TARGET_DATA_ID = "CameraTargetData";
        public const string SEQUENCE_DATA_ID = "CameraSequenceData";

        private CameraConstraints _constraints;
        private CameraTarget _target;
        private CameraSequence _sequence;

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
                IsActive = false;
                return false;
            }

            var initialTarget = GameObject.FindGameObjectWithTag("Player").transform;

            _constraints = new CameraConstraints(new(-180,180), new(0,45));
            _target = new CameraTarget(initialTarget);
            _sequence = new CameraSequence();

            DataBridge.UpdateData(CONSTRAINTS_DATA_ID, _constraints);
            DataBridge.UpdateData(TARGET_DATA_ID, _target);
            DataBridge.UpdateData(SEQUENCE_DATA_ID, _sequence);
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
            Vector3 destination;
            if (!_sequence.IsPlaying)
                destination = _target.GetTargetPosition();
            else
                destination = _sequence.CurrentTargetDestination;

            _cameraOrigin.transform.position = Vector3.Lerp
            (
                _cameraOrigin.transform.position,
                destination,
                _interpolationFactor
            );
        }

        private void Rotate()
        {
            if (!_sequence.IsPlaying)
            {
                var direction = Controls.RightDirectional;
                var target = _target.GetTargetPosition();

                _cameraPitch.RotateAround(target, _cameraOrigin.right, direction.y * _rotationSpeed * Time.deltaTime);
                _cameraOrigin.RotateAround(target, Vector3.up, -direction.x * _rotationSpeed * Time.deltaTime);

                var pitch = _cameraPitch.localEulerAngles.x;
                ClampAngle(ref pitch, _constraints.RotationVertical.x, _constraints.RotationVertical.y);
                _cameraPitch.localEulerAngles = new(Mathf.LerpAngle(_cameraPitch.localEulerAngles.x, pitch, _interpolationFactor), 0, 0);

                var yaw = _cameraOrigin.localEulerAngles.y;
                ClampAngle(ref yaw, _constraints.RotationHorizontal.x, _constraints.RotationHorizontal.y);
                _cameraOrigin.localEulerAngles = new(0, Mathf.LerpAngle(_cameraOrigin.localEulerAngles.y, yaw, _interpolationFactor), 0);
            }
            else
            {
                _cameraPitch.localEulerAngles = new(Mathf.LerpAngle(_cameraPitch.localEulerAngles.x, 
                    _sequence.CurrentRotationDestination.x, _interpolationFactor * 0.4f), 0, 0);
                _cameraOrigin.localEulerAngles = new(0, Mathf.LerpAngle(_cameraOrigin.localEulerAngles.y, 
                    _sequence.CurrentRotationDestination.y, _interpolationFactor * 0.4f), 0);
            }

            void ClampAngle(ref float angle, int min, int max)
            {
                if (min == max)
                {
                    angle = min;
                    return;
                }

                if (min < 0)
                    min += 360;
                if (max < 0)
                    max += 360;

                if(min < max)
                    angle = Mathf.Clamp(angle, min, max);
                else if(angle > max && angle < min)
                {
                    angle = AngularDistance(angle, min) < AngularDistance(angle, max) ? min : max;
                }
            }
            float AngularDistance(float from, float to)
            {
                float distance = Mathf.Abs(to - from) % 360f;
                return distance > 180f ? 360f - distance : distance;
            }
        }

        private void TrySyncData()
        {
            try
            {
                var constraints = DataBridge.TryGetData<CameraConstraints>(CONSTRAINTS_DATA_ID);
                var target = DataBridge.TryGetData<CameraTarget>(TARGET_DATA_ID);
                var sequence = DataBridge.TryGetData<CameraSequence>(SEQUENCE_DATA_ID);

                if (constraints.WasUpdated)
                {
                    if (constraints != Data<CameraConstraints>.Empty)
                    {
                        if (!IsActive) base.Init();
                        _constraints = constraints.EncodedData;
                        DataBridge.MarkUpdateProcessed<CameraConstraints>(CONSTRAINTS_DATA_ID);
                    }
                    else if (IsActive)
                    {
                        IsActive = false;
                        _status = $"Couldn't get \"{CONSTRAINTS_DATA_ID}\" data packet";
                    }
                }
                if (target.WasUpdated)
                {
                    if (target != Data<CameraTarget>.Empty)
                    {
                        if (!IsActive) base.Init();
                        _target = target.EncodedData;
                        DataBridge.MarkUpdateProcessed<CameraTarget>(TARGET_DATA_ID);
                    }
                    else if (IsActive)
                    {
                        IsActive = false;
                        _status = $"Couldn't get \"{TARGET_DATA_ID}\" data packet";
                    }
                }
                if (sequence.WasUpdated)
                {
                    if (sequence != Data<CameraSequence>.Empty)
                    {
                        if (!IsActive) base.Init();
                        _sequence = sequence.EncodedData;
                        DataBridge.MarkUpdateProcessed<CameraSequence>(SEQUENCE_DATA_ID);
                    }
                    else if (IsActive)
                    {
                        IsActive = false;
                        _status = $"Couldn't get \"{SEQUENCE_DATA_ID}\" data packet";
                    }
                }
            }
            catch (InvalidCastException)
            {
                if (IsActive)
                {
                    IsActive = false;
                    _status = $"Stored data was not of appropriate types";
                }
            }
        }
    }
}