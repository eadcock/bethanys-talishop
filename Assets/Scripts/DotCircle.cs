using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 
[CustomEditor(typeof(DotCircle))]
[ExecuteInEditMode]
public class DotCircle : MonoBehaviour
{
    public int diameter;
    
    public void ChangeSize()
    {
        float radius = diameter / 2.0f;
        //Loops through children. 0:Right 1:Top 2:Left 3:Bottom
        for(int i = 0; i < 4; i++)
        {
            Transform dotChild = transform.GetChild(i);
            if (dotChild.gameObject.activeSelf)
            {
                if(dotChild.gameObject.activeSelf == false)
                {
                    dotChild.gameObject.SetActive(true);

                }
                float angle = i * Mathf.PI * .5f;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                x = Mathf.Round(x * 2) / 2;
                y = Mathf.Round(y * 2) / 2;
                dotChild.localPosition = new Vector3(x, y);
            }
            else
            {
                if (dotChild.gameObject.activeSelf == true)
                {
                    dotChild.gameObject.SetActive(false);

                }
            }
            
        }
    }
}

// Custom Editor the "old" way by modifying the script variables directly.
// No handling of multi-object editing, undo, and Prefab overrides!
[CustomEditor(typeof(DotCircle))]
public class DotCircleEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DotCircle dotCircle = (DotCircle)target;

        dotCircle.diameter = EditorGUILayout.IntSlider("Diameter", dotCircle.diameter, 1, 10);
        ProgressBar(dotCircle.diameter / 10.0f, "Diameter");

        dotCircle.ChangeSize();
    }

    // Custom GUILayout progress bar.
    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }

}
#endif