using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Monoliths.Player
{
    public class CameraSequence
    {
        public bool IsPlaying { get; private set; }

        private bool _isPlayingRotation, _isPlayingTarget;

        private readonly List<(float delay, Vector2 rotation)> _rotationSequence = new();
        private readonly List<(float delay, Vector3 target)> _targetSequence = new();

        public Vector3 CurrentTargetDestination { get; private set; }
        public Vector2 CurrentRotationDestination { get; private set; }

        public (float delay, Vector2 rotation) GetNextRotation()
        {
            if (_targetSequence.Count == 0)
                return (-1, Vector2.zero);

            var result = _rotationSequence[0];
            _rotationSequence.RemoveAt(0);
            return result;
        }

        public (float delay, Vector3 target) GetNextTarget()
        {
            if (_targetSequence.Count == 0)
                return (-1, Vector3.zero);

            var result = _targetSequence[0];
            _targetSequence.RemoveAt(0);
            return result;
        }

        public void Add(Vector2 rotation, float delay) 
            => _rotationSequence.Add((delay, rotation));

        public void Add(Vector3 target, float delay)
            => _targetSequence.Add((delay, target));

        public void Play() 
        {
            if (IsPlaying)
                return;

            IsPlaying = true;
            MonolithMaster.Instance.StartCoroutine(SequenceRotation());
            MonolithMaster.Instance.StartCoroutine(SequenceTarget());
        }
        public void Stop() => IsPlaying = false;
        
        private IEnumerator SequenceRotation()
        {
            (float delay, Vector2 rotation) = GetNextRotation();

            if (delay == -1)
                yield break;

            CurrentRotationDestination = rotation;

            _isPlayingRotation = true;
            while (IsPlaying)
            {
                yield return new WaitForSeconds(delay);
                (delay, rotation) = GetNextRotation();
                if (delay == -1)
                {
                    _isPlayingRotation = false;
                    break;
                }
                CurrentRotationDestination = rotation;
            }
            if (!_isPlayingTarget)
                IsPlaying = false;
        }

        private IEnumerator SequenceTarget()
        {
            (float delay, Vector3 target) = GetNextTarget();

            if(delay == -1)
                yield break;

            CurrentTargetDestination = target;

            _isPlayingTarget = true;
            while (IsPlaying)
            {
                yield return new WaitForSeconds(delay);
                (delay, target) = GetNextTarget();
                if (delay == -1)
                {
                    _isPlayingTarget = false;
                    break;
                }
                CurrentTargetDestination = target;
            }
            if (!_isPlayingRotation)
                IsPlaying = false;
        }
    }
}