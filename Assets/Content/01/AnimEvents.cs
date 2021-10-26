using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    void PlaySound()
    {
      AudioSource SoundPlayer = GetComponent<AudioSource>();
      AudioClip clip = SoundPlayer.clip;
      SoundPlayer.PlayOneShot(clip);
    }
    
}
