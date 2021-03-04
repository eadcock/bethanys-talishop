using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static Dictionary<string, SaveData> _saveData;

    private static Dictionary<string, SaveData> SaveData => _saveData ?? (_saveData = new Dictionary<string, SaveData>());

    public string CurrentProfile = "Save";

    public SaveData CurrentSaveData
    {
        get
        {
            if(CurrentProfile != null)
            {
                return LoadFromProfile(CurrentProfile);
            }
            
            Debug.LogWarning("CurrentProfile doesn't exist, Data could not be loaded");
            return null;
        }
    }

    public bool RequireProfileInit => !SaveData.ContainsKey(CurrentProfile);

    public void Awake()
    {
        CurrentProfile = "Save";
    }

    public bool SaveToFile(string profile)
    {
        if(SaveData.ContainsKey(profile))
            return SerializationManager.Save(profile, SaveData[profile]);

        return false;
    }

    public bool SaveToFile(string profile, int level)
    {
        SaveToProfile(profile, level);
        return SaveToFile(profile);
    }

    public void SaveToProfile(string profile, int level) 
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

    public SaveData LoadFromProfile(string profile)
    {
        return SaveData.TryGetValue(profile, out SaveData save) ? save : null;
    }

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

    public bool ResetProfile(string profile)
    {
        if (!SaveData.ContainsKey(profile))
            return false;

        SaveData[profile] = new SaveData(1, profile);

        return true;
    }

    public bool DeleteProfile(string profile)
    {
        if (!SaveData.ContainsKey(profile))
            return false;

        SaveData.Remove(profile);

        try
        {
            string dirPath = Application.persistentDataPath + "/saves/" + profile + ".save";
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath);
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
