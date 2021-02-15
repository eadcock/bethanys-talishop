using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int currentLevel;
    public string saveName;

    public SaveData() : this(0) { }

    public SaveData(int _currentLevel)
    {
        currentLevel = _currentLevel;
        saveName = "Save";
    }

    public SaveData(int _currentLevel, string _saveName)
        : this(_currentLevel)
    {
        saveName = _saveName;
    }
}
