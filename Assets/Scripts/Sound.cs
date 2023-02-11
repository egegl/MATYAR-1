using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField] public AudioClip Clip { get; private set; }
    [field:SerializeField, Range(0f, 1f)] public float Volume { get; private set; }
    [field:SerializeField, Range(.1f, 3f)] public float Pitch { get; private set; }
    
    [HideInInspector]
    public AudioSource source;

    public Sound(string name, AudioClip clip)
    {
        Name = name;
        Clip = clip;
    }
}
