using UnityEngine;

[CreateAssetMenu(fileName = "New Music Track", menuName = "Audio/Music Track")]
public class MusicTrack : ScriptableObject
{
    public string Name;
    public AudioClip Clip;
    public bool Loop = true;
}
