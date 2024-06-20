using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioSource> _audioSources = new();

    [SerializeField] private List<MusicTrack> _musicTracks = new();
    [SerializeField] private List<Sound> _sounds = new();
    [SerializeField] private List<AmbientSound> _ambientSounds = new();

    [SerializeField] private List<Sound> _amebaSouinds = new();

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _ambientSource;

    [Header("Music Group")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup level0MusicGroup;
    [SerializeField] private AudioMixerGroup level1MusicGroup;
    [SerializeField] private AudioMixerGroup level2MusicGroup;
    [SerializeField] private AudioMixerGroup level3MusicGroup;

    [Header("Ambient Group")]
    [SerializeField] private AudioMixerGroup ambienceGroup;
    [SerializeField] private AudioMixerGroup level0AmbienceGroup;
    [SerializeField] private AudioMixerGroup level1AmbienceGroup;
    [SerializeField] private AudioMixerGroup level2AmbienceGroup;
    [SerializeField] private AudioMixerGroup level3AmbienceGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlayMusic(string name)
    {
        var track = _musicTracks.FirstOrDefault(m => m?.Name == name);
        if (track != null && track.Clip != null)
        {
            _musicSource.clip = track.Clip;
            _musicSource.loop = track.Loop;
            SetMusicVolume(track.Volume);
            _musicSource.Play();
        }
    }

    public void PlayMainMenuMusic()
        =>PlayMusic("MainMenuMT");
    public void PlayLevelMusic(string levelName)
        =>PlayMusic(levelName);



    public void PlaySound(string name)
    {
        var sound = _sounds.FirstOrDefault(s => s.Name == name);
        if (sound != null && sound.Clip != null)
        {
            SetSFXVolume(sound.Volume);
            _sfxSource.PlayOneShot(sound.Clip, sound.Volume);
        }
            
    }
    public void PlayAmbient(string name)
    {
        var ambient = _ambientSounds.FirstOrDefault(a => a.Name == name);
        if (ambient != null && ambient.Clip != null)
        {
            _ambientSource.clip = ambient.Clip;
            _ambientSource.loop = ambient.Loop;
            SetAmbientVolume(ambient.Volume);
            _ambientSource.Play();
        }
    }

    public void StopMusic() => _musicSource.Stop();
    public void StopAmbient() => _ambientSource.Stop();
    public void Stop(string name)
    {
        try
        {
            if (_musicSource.clip != null && _musicSource.isPlaying && _musicSource.clip.name == name)
                _musicSource.Stop();
        }
        catch (UnassignedReferenceException)
        {
            Debug.Log($"Unassigned \"{name}\" In Audio Manager.");
        }
    }

    public void SetMusicVolume(float volume) => _musicSource.volume = volume;
    public void SetSFXVolume(float volume) => _sfxSource.volume = volume;
    public void SetAmbientVolume(float volume) => _ambientSource.volume = volume;

    public bool IsPlaying(string soundName) 
        => _audioSources.ContainsKey(soundName) && _audioSources[soundName].isPlaying;

    public void SetMusicGroup(string levelName)
    {
        switch (levelName)
        {
            case "Tutorial":
                _musicSource.outputAudioMixerGroup = level0MusicGroup;
                _ambientSource.outputAudioMixerGroup = level0AmbienceGroup;
                break;
            case "Level 1":
                _musicSource.outputAudioMixerGroup = level1MusicGroup;
                _ambientSource.outputAudioMixerGroup = level1AmbienceGroup;
                break;
            case "Level 2":
                _musicSource.outputAudioMixerGroup = level2MusicGroup;
                _ambientSource.outputAudioMixerGroup = level2AmbienceGroup;
                break;
            case "Level 3":
                _musicSource.outputAudioMixerGroup = level3MusicGroup;
                _ambientSource.outputAudioMixerGroup = level3AmbienceGroup;
                break;
            default:
                _musicSource.outputAudioMixerGroup = musicGroup;
                _ambientSource.outputAudioMixerGroup = ambienceGroup;
                break;
        }
    }
    public void SetDefaultMusicGroup()
    {
        _musicSource.outputAudioMixerGroup = musicGroup;
        _ambientSource.outputAudioMixerGroup = ambienceGroup;
    }
}
