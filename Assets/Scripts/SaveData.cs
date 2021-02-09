using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            if(_current == null)
            {
                _current = new SaveData();
            }
            return _current;
        }
        set
        {
            _current = value;
        }
    }

    public int currentLevel;
    public const string SaveName = "Save";

    public SaveData()
    {
        Debug.Log("creating new save data");
        currentLevel = 0;
    }
}
