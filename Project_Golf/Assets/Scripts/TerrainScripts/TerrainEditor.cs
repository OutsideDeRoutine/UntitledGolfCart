using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainFitter))]
public class TerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainFitter myScript = (TerrainFitter)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.DoYourThing();
        }
    }
}