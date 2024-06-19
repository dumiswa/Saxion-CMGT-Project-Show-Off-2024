using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] _stepClips;
    [SerializeField] private AudioSource _playerAudioSource;

    public void PlayStepSounds()
    {
        if (_stepClips.Length == 0) return;
    
        AudioClip stepSound = _stepClips[Random.Range(0, _stepClips.Length)];
        _playerAudioSource.pitch = Random.Range(0.7f, 1.0f);
        _playerAudioSource.PlayOneShot(stepSound);
    }
}



