using UnityEngine;

[CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Sound")]
public class Sound : ScriptableObject
{
    public string Name;
    public AudioClip Clip;
    public float Volume = 1.0f;
    public float Pitch = 1.0f;
}
