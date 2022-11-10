using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Mixers")]
    [SerializeField] AudioMixer masterMixer;

    [Header("Parameters")]
    public float masterVolume = 1f;

    [Header("Sounds")]
    public AudioSource musicSource, sfxSource;

    public void SetVolume(float volume)
    {
        masterVolume = volume;
        masterMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.Log("No SFX clip found!");
            return;
        }
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == null)
        {
            Debug.Log("No music found!");
            return;
        }
        musicSource.clip = music;
        musicSource.Play();
    }

    public void NullMusic()
    {
        musicSource.Stop();
    }
}
