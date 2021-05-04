using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Options
{
    public bool skipDialogue;
    public AudioSettings audioSettings;

    public Options(bool skip, AudioSettings audioSettings)
    {
        skipDialogue = skip;
        this.audioSettings = audioSettings;
    }

    public Options(Options option)
    {
        skipDialogue = option.skipDialogue;
        audioSettings = option.audioSettings;
    }

    public void Deconstruct(out bool skip, out AudioSettings audio) => (skip, audio) = (skipDialogue, audioSettings);
}
