using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public struct KeyedClip
{
    public AudioClip clip;
    public string key;
}

[CustomEditor(typeof(KeyedClip))]
public class KeyedClipEditor : Editor
{
    SerializedProperty clip;
    SerializedProperty key;

    private void OnEnable()
    {
        clip = serializedObject.FindProperty("clip");
        key = serializedObject.FindProperty("key");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Key"));
        key.stringValue = EditorGUILayout.TextField(key.stringValue, GUILayout.Width(10.0f));
        EditorGUILayout.ObjectField(clip, new GUIContent("Clip"));
        EditorGUILayout.EndHorizontal();

        base.DrawDefaultInspector();
        GUIUtility.ExitGUI();
    }
}
