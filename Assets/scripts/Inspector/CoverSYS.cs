using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CoverGenerator))]
public class CoverSYS : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CoverGenerator generator = (CoverGenerator)target;
        if (GUILayout.Button("Generate Covers"))
        {
            generator.GenerateCovers();
        }
    }
}
