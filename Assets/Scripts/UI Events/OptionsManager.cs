using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GoBackTo
{
    Pause,
    Title
}

public class OptionsManager : MonoBehaviour
{
    public static Options options = new Options(false);

    public static void UpdateSkipDialgue(bool val)
    {
        options.skipDialogue = val;
        
        SyncOptions();
    }

    public void DisplayOptions()
    {
        foreach(Transform t in transform)
        {
            switch (t.gameObject.name)
            {
                case "SkipDialogue":
                    t.gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(options.skipDialogue);
                    break;
            }

            t.gameObject.SetActive(true);
        }
    }

    public void HideOptions()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }

        SyncOptions();
        GameMaster.Instance.Save.SaveToProfile(GameMaster.Instance.Save.CurrentProfile, options);
    }

    public static void SyncOptions()
    {
        if(GameMaster.Instance.Save?.CurrentSaveData != null)
            GameMaster.Instance.Save.CurrentSaveData.options = options;
    }

    public static void DeleteSave() => GameMaster.Instance.Save.DeleteProfile(GameMaster.Instance.Save.CurrentProfile);

    public void Start()
    {
        
    }

    public void UpdatePanel()
    {
        foreach(Transform t in gameObject.transform)
        {
            switch(t.gameObject.name)
            {
                case "SkipDialogue":
                    t.gameObject.GetComponent<Toggle>().isOn = options.skipDialogue;
                    break;
            }
        }
    }
}
