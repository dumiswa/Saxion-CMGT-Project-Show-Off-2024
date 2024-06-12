using UnityEngine;
using System.Linq;

public partial class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private MusicTrack[] _musicTracks;
    [SerializeField] private Sound[] _sounds;
    [SerializeField] private AmbientSound[] _ambientSounds ;

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
        AudioClip clip = Resources.Load<AudioClip>("Audio/Music/" + name);
        if (clip)
        {
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }
    public void PlaySound(string name) 
    {
        Sound sound = _sounds.FirstOrDefault(s => s.name == name);
        if (sound != null && sound.Clip != null)
             _sfxSource.PlayOneShot(sound.Clip, sound.Volume);      
    }
    public void PlayAmbient(string name) 
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/Ambient/" + name);
        if (clip)
        {
            _ambientSource.clip = clip;
            _ambientSource.loop = true;
            _ambientSource.Play();
        }
    }

    public void SetMusicVolume(float volume) => _musicSource.volume = volume;
    public void SetSFXVolume(float volume) => _sfxSource.volume = volume;
    public void SetAmbientVolume(float volume) => _ambientSource.volume = volume;
}
