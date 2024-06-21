using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FootStepSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] _stepClips;
    [SerializeField] private AudioClip[] _ladderClimbingClips;
    [SerializeField] private AudioClip[] _bossTakingDmgClips;
    [SerializeField] private AudioClip _damageClip;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioSource _audioSource;

    public enum ActionType
    {
        Walking,
        ClimbingLadder,
    }

    public void PlayStepSounds(ActionType actionType)
    {
        AudioClip[] selectedClips = null;

        switch (actionType)
        {
            case ActionType.Walking:
                selectedClips = _stepClips;
                _audioSource.volume = 0.25f;
                break;
            case ActionType.ClimbingLadder:
                selectedClips = _ladderClimbingClips;
                _audioSource.volume = 0.1f;
                break;
        }
        
        if (selectedClips != null && selectedClips.Length > 0)
        {
            AudioClip stepSound = _stepClips[Random.Range(0, _stepClips.Length)];
            _audioSource.pitch = Random.Range(0.7f, 1.0f);
            _audioSource.PlayOneShot(stepSound);
        }
        
    }

    public void PlayDmgSound()
    {
        _audioSource.PlayOneShot(_damageClip);
    }
    public void PlayDeathSound()
    {
        _audioSource.PlayOneShot(_deathClip);
    }

    public void BossTakesDmg()
    {
        AudioClip dmgSound = _bossTakingDmgClips[Random.Range(0, _bossTakingDmgClips.Length)]; 
        _audioSource.PlayOneShot(dmgSound);
    }
}



