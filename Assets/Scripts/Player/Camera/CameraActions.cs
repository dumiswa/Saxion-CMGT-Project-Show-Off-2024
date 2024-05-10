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

        private Transform _camera;
        public override bool Init()
        {
            _camera = Camera.main.transform.parent;
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
            Move();
        }

        private void Move()
        {
            _camera.transform.position = Vector3.Lerp
            (
                _camera.transform.position, 
                _target.GetTargetPosition(),
                _interpolationFactor
            );
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
        
        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            Controls.Player.
        }
    }
}