using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioSource> _audioSources;

    [SerializeField] private MusicTrack[] musicTracks;
    [SerializeField] private Sound[] sounds;
    [SerializeField] private AmbientSound[] ambientSounds;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

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
        var track = musicTracks.FirstOrDefault(m => m.Name == name);
        if (track != null && track.Clip != null)
        {
            musicSource.clip = track.Clip;
            musicSource.loop = track.Loop;
            musicSource.Play();
        }
    }

    public void PlayMainMenuMusic()
        =>PlayMusic("MainMenuMT");
    public void PlayLevelMusic(string levelName)
        =>PlayMusic(levelName);

    public void PlaySound(string name)
    {
        var sound = sounds.FirstOrDefault(s => s.Name == name);
        if (sound != null && sound.Clip != null)
            sfxSource.PlayOneShot(sound.Clip, sound.Volume);
    }

    public void PlayAmbient(string name)
    {
        var ambient = ambientSounds.FirstOrDefault(a => a.Name == name);
        if (ambient != null && ambient.Clip != null)
        {
            ambientSource.clip = ambient.Clip;
            ambientSource.loop = ambient.Loop;
            ambientSource.Play();
        }
    }

    public void StopMusic() => musicSource.Stop();
    
     public void Stop(string name)
     {
        if (musicSource.isPlaying && musicSource.clip != null && musicSource.clip.name == name)     
            musicSource.Stop();
     }

    public void SetMusicVolume(float volume) => musicSource.volume = volume;
    public void SetSFXVolume(float volume) => sfxSource.volume = volume;
    public void SetAmbientVolume(float volume) => ambientSource.volume = volume;

    public bool IsPlaying(string soundName)
    {
        return _audioSources[soundName].isPlaying;
    }
}
