using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioSource> _audioSources = new();

    [SerializeField] private List<MusicTrack> _musicTracks = new();
    [SerializeField] private List<Sound> _sounds = new();
    [SerializeField] private List<AmbientSound> _ambientSounds = new();

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _ambientSource;

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
}
