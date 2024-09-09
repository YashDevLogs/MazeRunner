using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private List<Sound> sounds;  // List of sound scriptable objects
    [SerializeField] private AudioSource audioSource;               

    public void PlaySound(SoundType soundType)
    {
        Sound sound = sounds.Find(s => s.soundType == soundType); 

        if (sound != null && sound.audioClip != null)
        {
            audioSource.PlayOneShot(sound.audioClip, sound.volume);
        }
        else
        {
            Debug.LogWarning("Sound not found or clip missing for: " + soundType);
        }
    }
}
