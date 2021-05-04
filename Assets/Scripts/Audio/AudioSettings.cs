using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;

[System.Serializable]
public class AudioSettings 
{
    public Float01 masterVolume;
    public Float01 rawMusicVolume;
    public Float01 rawSoundFxVolume;

    public Float01 ScaledMusicVolume => (Float01)Math.Map(rawMusicVolume, 0.0f, 1.0f, 0.0f, masterVolume);
    public Float01 ScaledSoundFxVolume => (Float01)Math.Map(rawSoundFxVolume, 0.0f, 1.0f, 0.0f, masterVolume);

    public AudioSettings()
    {
        masterVolume = rawMusicVolume = rawSoundFxVolume = (Float01)0.5f;
    }
}
