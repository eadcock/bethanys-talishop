using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Options
{
    public bool skipDialogue;

    public Options(bool skip)
    {
        skipDialogue = skip;
    }

    public Options(Options option)
    {
        skipDialogue = option.skipDialogue;
    }
}
