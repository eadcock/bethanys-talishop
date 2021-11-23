using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using quiet;

public enum GoBackTo
{
    Pause,
    Title
}

public class OptionsManager : MonoBehaviour
{
    public static Options options = new Options(false, new AudioSettings());

    public static void UpdateSkipDialgue(bool val)
    {
        options.skipDialogue = val;
        GameMaster.Instance.Audio.PlaySoundFX("UIClick");

        SyncOptions();
    }

    public static void MasterVolumeOnChange(float value)
    {
        GameMaster.Instance.Audio.SetVolume(VolumeSlider.Master, (Float01)value);
    }

    public static void MusicVolumeOnChange(float value)
    {
        GameMaster.Instance.Audio.SetVolume(VolumeSlider.Music, (Float01)value);
    }

    public static void SoundFxVolumeOnChange(float value)
    {
        GameMaster.Instance.Audio.SetVolume(VolumeSlider.SoundFX, (Float01)value);
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
                case "MasterVolumeSlider":
                    t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(options.audioSettings.masterVolume);
                    break;
                case "MusicVolumeSlider":
                    t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(options.audioSettings.rawMusicVolume);
                    break;
                case "SoundFXVolumeSlider":
                    t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(options.audioSettings.rawSoundFxVolume);
                    break;
            }

            t.gameObject.SetActive(true);
        }
    }

    public void HideOptions()
    {
        if(transform.GetChild(0).gameObject.activeInHierarchy)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }

            SyncOptions();
            GameMaster.Instance.Save.SaveToFile(GameMaster.Instance.Save.CurrentProfile, options);
        }
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
                    t.gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(options.skipDialogue);
                    break;
                case "MasterVolumeSlider":
                    t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(options.audioSettings.masterVolume);
                    break;
                case "MusicVolumeSlider":
                    t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(options.audioSettings.rawMusicVolume);
                    break;
                case "SoundFXVolumeSlider":
                    t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(options.audioSettings.rawSoundFxVolume);
                    break;
            }
        }
    }
}
