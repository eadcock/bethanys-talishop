using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DevCheats))]
public class DevCheatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DevCheats dev = (DevCheats)target;
        if(dev.enabled)
        {
            if (GUILayout.Button("Unlock All"))
            {
                GameMaster.Instance.Save.CurrentSaveData.currentLevel = GameMaster.NUM_PUZZLES;
            }
        }
    }
}
