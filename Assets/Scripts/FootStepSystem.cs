using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FootStepSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] _stepClips;
    [SerializeField] private AudioClip[] _ladderClimbingClips;
    [SerializeField] private AudioClip _damageClip;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioSource _playerAudioSource;

    public enum ActionType
    {
        Walking,
        ClimbingLadder
    }

    public void PlayStepSounds(ActionType actionType)
    {
        AudioClip[] selectedClips = null;

        switch (actionType)
        {
            case ActionType.Walking:
                selectedClips = _stepClips;
                _playerAudioSource.volume = 0.25f;
                break;
            case ActionType.ClimbingLadder:
                selectedClips = _ladderClimbingClips;
                _playerAudioSource.volume = 0.1f;
                break;
        }
        
        if (selectedClips != null || selectedClips.Length == 0)
        {
            AudioClip stepSound = _stepClips[Random.Range(0, _stepClips.Length)];
            _playerAudioSource.pitch = Random.Range(0.7f, 1.0f);
            _playerAudioSource.PlayOneShot(stepSound);
        }
        
    }

    public void PlayDmgSound()
    {
        _playerAudioSource.PlayOneShot(_damageClip);
    }
    public void PlayDeathSound()
    {
        _playerAudioSource.PlayOneShot(_deathClip);
    }
}



