using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using quiet;
using System.Linq;

public enum VolumeSlider
{
    Master,
    Music,
    SoundFX,
}

[System.Serializable]
public struct NamedClip
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{

    AudioSource musicAS;
    [SerializeField]
    AudioClip musicClip;
    AudioSource soundFxAS;

    AudioSettings settings;

    [SerializeField]
    public NamedClip[] library;

    public Lookup<string, AudioClip> lookupLibrary;

    public bool IsMusicPlaying => musicAS.isPlaying && musicAS.volume > 0;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameMaster.Instance.Save.CurrentSaveData.options.audioSettings;

        musicAS = gameObject.AddComponent<AudioSource>();
        musicAS.clip = musicClip;
        musicAS.loop = true;

        soundFxAS = gameObject.AddComponent<AudioSource>();
        soundFxAS.loop = false;

        UpdateVolume();
        musicAS.Play();

        lookupLibrary = (Lookup<string, AudioClip>)library.ToLookup(c => c.name, c => c.clip);
    }

    public void PlayMusic()
    {
        musicAS.Play();
    }

    public void PauseMusic()
    {
        musicAS.Pause();
    }

    public void MuteMusic()
    {
        settings.rawMusicVolume = new Float01(0);
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        musicAS.volume = settings.ScaledMusicVolume;
        soundFxAS.volume = settings.ScaledSoundFxVolume;
    }

    public void SetVolume(VolumeSlider v, Float01 volume)
    {
        switch(v)
        {
            case VolumeSlider.Master:
                settings.masterVolume = volume;
                break;
            case VolumeSlider.Music:
                settings.rawMusicVolume = volume;
                break;
            case VolumeSlider.SoundFX:
                settings.rawSoundFxVolume = volume;
                break;
        }

        UpdateVolume();
    }

    public void PlaySoundFX(AudioClip clip)
    {
        soundFxAS.PlayOneShot(clip, settings.ScaledSoundFxVolume);
    }

    public void PlaySoundFX(string clip)
    {
        if (lookupLibrary[clip].Any())
            soundFxAS.PlayOneShot(lookupLibrary[clip].First(), settings.ScaledSoundFxVolume);
    }

    public void PlayMusic(string clip)
    {
        if (lookupLibrary[clip].Any())
            musicAS.clip = lookupLibrary[clip].First();
    }
}
