using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Monoliths.Player
{
    public class CameraSequence
    {
        public bool IsPlaying { get; private set; }

        private bool _isPlayingRotation, _isPlayingTarget, _isPlayingAdds;

        private readonly List<(float delay, Vector2Int rotation)> _rotationSequence = new();
        private readonly List<(float delay, Vector3 target)> _targetSequence = new();
        private readonly List<(float delay, Vector2 interpolationAndFov)> _addsSequence = new();

        public Vector3 CurrentTargetDestination { get; private set; }
        public Vector2Int CurrentRotationDestination { get; private set; }
        public Vector2 CurrentAddsDestination { get; private set; }

        public (float delay, Vector2Int rotation) GetNextRotation()
        {
            if (_targetSequence.Count == 0)
                return (-1, Vector2Int.zero);

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
        public (float delay, Vector2 adds) GetNextAdds()
        {
            if (_addsSequence.Count == 0)
                return (-1, Vector2.zero);

            var result = _addsSequence[0];
            _addsSequence.RemoveAt(0);
            return result;
        }

        public void Add(Vector2Int rotation, float delay) 
            => _rotationSequence.Add((delay, rotation));
        public void Add(Vector3 target, float delay)
            => _targetSequence.Add((delay, target));
        public void Add(Vector2 adds, float delay)
            => _addsSequence.Add((delay, adds));

        public void Play() 
        {
            if (IsPlaying)
                return;

            IsPlaying = true;
            MonolithMaster.Instance.StartCoroutine(SequenceRotation());
            MonolithMaster.Instance.StartCoroutine(SequenceTarget());
            MonolithMaster.Instance.StartCoroutine(SequenceAdds());
        }
        public void Stop() => IsPlaying = false;
        
        private IEnumerator SequenceRotation()
        {
            (float delay, Vector2Int rotation) = GetNextRotation();

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
            if (!_isPlayingTarget && !_isPlayingAdds)
                IsPlaying = false;
        }

        private IEnumerator SequenceAdds()
        {
            (float delay, Vector2 adds) = GetNextAdds();

            if (delay == -1)
                yield break;

            CurrentAddsDestination = adds;

            _isPlayingRotation = true;
            while (IsPlaying)
            {
                yield return new WaitForSeconds(delay);
                (delay, adds) = GetNextAdds();
                if (delay == -1)
                {
                    _isPlayingAdds = false;
                    break;
                }
                CurrentAddsDestination = adds;
            }
            if (!_isPlayingTarget && !_isPlayingRotation)
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
            if (!_isPlayingRotation && !_isPlayingAdds)
                IsPlaying = false;
        }
    }
}