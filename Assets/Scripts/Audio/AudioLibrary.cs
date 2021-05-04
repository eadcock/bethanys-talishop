using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;

[Serializable]
public class AudioLibrary 
{
    private Dictionary<string, AudioClip> clips;

    [SerializeField]
    public KeyedClip clip;

    public AudioLibrary()
    {
        
    }


}

[CustomEditor(typeof(AudioLibrary))]
public class AudioLibraryEditor : Editor
{
    SerializedProperty clip;

    public void OnEnable()
    {
        clip = serializedObject.FindProperty("clip");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(clip, new GUIContent("Keyed Clip"));
    }
}
