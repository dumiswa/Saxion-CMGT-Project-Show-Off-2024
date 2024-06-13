using UnityEngine;

[CreateAssetMenu(fileName = "New Ambient Sound", menuName = "Audio/Ambient Sound")]
public class AmbientSound : ScriptableObject
{
    public string Name;
    public AudioClip Clip;
    public bool Loop = true;
}

