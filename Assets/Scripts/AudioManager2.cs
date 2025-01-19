using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager2 : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip backgroundMusic;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip jumpSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
          if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    public void StopCurrentAudio()
    {
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Stopping current audio.");
        }
    }

    public void PlayWinSound()
    {
        if (audioSource == null)
        {
        Debug.LogError("AudioSource is null! Ensure an AudioSource is attached to " + gameObject.name);
        return;
        }
        
        if (winSound != null)
        {
            StopCurrentAudio();
            audioSource.clip = winSound;
            audioSource.loop = false;
            audioSource.Play();
        }
        else
        {
        Debug.LogError("Win sound is not assigned!");
        }
    }

    public void PlayLoseSound()
    {
        if (loseSound != null)
        {
            audioSource.clip = loseSound;
            audioSource.loop = false;  
            audioSource.Play();
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

}
