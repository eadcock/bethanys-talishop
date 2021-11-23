using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static Dictionary<string, SaveData> _saveData;

    private static Dictionary<string, SaveData> SaveData => _saveData ??= new Dictionary<string, SaveData>();

    public string CurrentProfile = "Save";

    public SaveData CurrentSaveData
    {
        get
        {
            if(CurrentProfile != null)
            {
                return LoadFromProfile(CurrentProfile);
            }
            
            Debug.LogWarning($"CurrentProfile ({CurrentProfile}) doesn't exist, Data could not be loaded");
            return null;
        }
    }

    public bool RequireProfileInit => !SaveData.ContainsKey(CurrentProfile);

    public void Awake()
    {
        CurrentProfile = "Save";
    }

    /// <summary>
    /// Saves a profile to its appropriate save file
    /// </summary>
    /// <param name="profile"></param>
    /// <returns>If the save operation was successful</returns>
    public bool SaveToFile(string profile)
    {
        if(SaveData.ContainsKey(profile))
            return SerializationManager.Save(profile, SaveData[profile]);

        return false;
    }

    /// <summary>
    /// First, locally saves the level data to the profile.
    /// Then, saves the profile to its appropriate save file.
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="level"></param>
    /// <returns>Whether the save operation was successful</returns>
    public bool SaveToFile(string profile, int level)
    {
        SaveLevelToProfile(profile, level);
        return SaveToFile(profile);
    }

    /// <summary>
    /// First, locally saves the Options data to the profile.
    /// The, saves the profile to its appropriate save file.
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="op"></param>
    /// <returns>Whether the save operation was successful</returns>
    public bool SaveToFile(string profile, Options op)
    {
        SaveToProfile(profile, op);
        return SaveToFile(profile);
    }

    /// <summary>
    /// Locally saves the level data to the desired profile.
    /// If the profile doesn't exist, creates it.
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="level"></param>
    public void SaveLevelToProfile(string profile, int level) 
    { 
        if(SaveData.ContainsKey(profile))
        {
            SaveData[profile].currentLevel = level;
        } 
        else
        {
            SaveData.Add(profile, new SaveData(level, profile));
        }
    }

    /// <summary>
    /// Locally saves the Options data to the desired profile.
    /// If the profile doesn't exist, creates it.
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="level"></param>
    public void SaveToProfile(string profile, Options options)
    {
        if (SaveData.ContainsKey(profile))
            SaveData[profile].options = options;
        else
            SaveData.Add(profile, new SaveData(profile, options));
    }

    /// <summary>
    /// Locally saves the current line of dialogue to the desired profile.
    /// If the profile doesn't exist, creates it
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="currentDialogue"></param>
    public void SaveDialogueToProfile(string profile, int currentDialogue)
    {
        if (SaveData.ContainsKey(profile))
            SaveData[profile].currentDialogue = currentDialogue;
        else
            SaveData.Add(profile, new SaveData(profile, currentDialogue));
    }

    /// <summary>
    /// Retrieves the current saved data for a given profile
    /// </summary>
    /// <param name="profile"></param>
    /// <returns>Object representing the saved data of the profile</returns>
    public SaveData LoadFromProfile(string profile)
    {
        return SaveData.TryGetValue(profile, out SaveData save) ? save : new SaveData();
    }

    /// <summary>
    /// Loads the profile's save file and formats the information into a SaveData object
    /// </summary>
    /// <param name="profile"></param>
    /// <returns>The profile's formatted save data</returns>
    public SaveData LoadFile(string profile)
    {
        var result = SerializationManager.Load(Application.persistentDataPath + "/saves/" + profile + ".save");
        if(result is null)
        {
            return null;
        } 
        else
        {
            SaveData data = (SaveData)result;
            if (!SaveData.ContainsKey(profile))
                SaveData.Add(profile, data);
            else
                SaveData[profile] = data;

            return data;
        }
    }

    /// <summary>
    /// Clears the save for a given profile
    /// </summary>
    /// <param name="profile"></param>
    /// <returns></returns>
    public bool ResetProfile(string profile)
    {
        if (!SaveData.ContainsKey(profile))
            return false;

        SaveData[profile] = new SaveData(1, profile);

        return true;
    }

    /// <summary>
    /// Deletes all save data for a given profile. Also deletes all reference to the profile
    /// </summary>
    /// <param name="profile"></param>
    /// <returns>Whether deletion of the profile was successful</returns>
    public bool DeleteProfile(string profile)
    {
        Debug.Log(profile);
        Debug.Log(SaveData.Count);
        if (SaveData.ContainsKey(profile))
            SaveData.Remove(profile);

        try
        {
            string dirPath = Application.persistentDataPath + "/saves/" + profile + ".save";
            if (File.Exists(dirPath))
            {
                File.Delete(dirPath);
                Debug.Log("Deleted!");
            }
            else
            {
                Debug.Log("Doesn't exist! " + dirPath);
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
