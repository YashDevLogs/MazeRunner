using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "ScriptableObjects/Sound")]
public class Sound : ScriptableObject
{
    public SoundType soundType;      // Enum type for the sound
    public AudioClip audioClip;      // Reference to the audio clip
    public float volume = 1f;        // Volume of the sound
}
