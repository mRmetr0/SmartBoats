using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PopulationManager))]
public class PopulationManagerScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        
        if (GUILayout.Button("Generate Plants"))
        {
            (target as GenerationManager)?.GenerateBoxes();
        }
        if (GUILayout.Button("Generate Herb/Carn/Omni"))
        {
            (target as GenerationManager)?.GenerateObjects();
        }
    }
}
