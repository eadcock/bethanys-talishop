using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int currentLevel;
    public string saveName;
    public Options options;
    public int currentDialogue;

    public SaveData() : this(0) { }

    public SaveData(int _currentLevel)
    {
        currentLevel = _currentLevel;
        saveName = "Save";
        options = OptionsManager.options;
    }

    public SaveData(int _currentLevel, string _saveName) : this(_currentLevel) => saveName = _saveName;

    public SaveData(string _saveName, Options _options) : this(1, _saveName, _options) { }

    public SaveData(string _saveName, int _currentDialogue) : this(1, _saveName) => currentDialogue = _currentDialogue;

    public SaveData(int _currentLevel, string _saveName, Options _options) : this(_currentLevel, _saveName) => options = _options;

    public SaveData(int _currentLevel, string _saveName, Options _options, int _currentDialogue) : this(_currentLevel, _saveName, _options) => currentDialogue = _currentDialogue;

    public void Deconstruct(out int level, out string name, out Options ops, out int dialogue) => (level, name, ops, dialogue) = (currentLevel, saveName, options, currentDialogue);
}
