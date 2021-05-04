using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomPuzzleGenerator))]
public class RandomPuzzleGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RandomPuzzleGenerator gen = (RandomPuzzleGenerator)target;
        if (GUILayout.Button("Create new talisman"))
        {
            gen.grid.Clear();
            gen.CreateRandomTalisman(RandomPuzzleGenerator.GenStyle.PureRandom);
        }

        if(GUILayout.Button("Solve current"))
        {
            gen.grid.SolveCurrent();
        }
    }
}
