using Monoliths.Player;
using System;
using System.Collections;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class RotatingIsland : RotatingActuator
    {
        [Serializable]
        public class Positions 
        {
            public Vector3 position;
            public float delay;
        }

        [Serializable]
        public class Rotations
        {
            public Vector2Int rotation;
            public float delay;
        }

        [Serializable]
        public class Adds
        {
            public Vector2 interpolationAndFov;
            public float delay;
        }

        [SerializeField]
        private Positions[] _cameraPositions;
        [SerializeField]
        private Rotations[] _cameraRotations;
        [SerializeField]
        private Adds[] _cameraInterpolationAndFOV;

        private Animator _animator;

        protected override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
        }

        public override void Invoke()
        {
            if (Locked)
                return;

            base.Invoke();

            _animator.SetTrigger("Rotation");

            var cameraSequence = DataBridge.TryGetData<CameraSequence>(CameraActions.SEQUENCE_DATA_ID).EncodedData;

            foreach (var position in _cameraPositions)
                cameraSequence.Add(position.position, position.delay);

            foreach (var rotation in _cameraRotations)
                cameraSequence.Add(rotation.rotation, rotation.delay);

            foreach (var adds in _cameraInterpolationAndFOV)
                cameraSequence.Add(adds.interpolationAndFov, adds.delay);

            cameraSequence.Play();
            StartCoroutine(Rotation());
        }

        private IEnumerator Rotation()
        {
            MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(false);
            MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(false);

            Locked = true;
            
            float a = 0;
            float b = 0;
            float c = 0;

            foreach (var i in _cameraPositions)
                a += i.delay;
            foreach (var i in _cameraRotations)
                b += i.delay;
            foreach (var i in _cameraInterpolationAndFOV)
                c += i.delay;
             
            float t = Mathf.Max(a, Mathf.Max(b,c));
            yield return new WaitForSeconds(t);

            MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(true);
            MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(true);
            Locked = false;
        }
    }
}