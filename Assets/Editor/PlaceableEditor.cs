using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Placeable))]
public class PlaceableEditor : Editor
{
    SerializedProperty debugModeProperty;
    SerializedProperty probingVectorProperty;

    void OnEnable()
    {
        debugModeProperty = serializedObject.FindProperty("debugMode");
        probingVectorProperty = serializedObject.FindProperty("probingVector");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (debugModeProperty.boolValue)
        {
            EditorGUILayout.HelpBox("There is a white line showing the probing vector and yellow line showing what is the allowed distance in this direction.", MessageType.Info);
            EditorGUILayout.PropertyField(probingVectorProperty, new GUIContent("Probing Vector"));
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}