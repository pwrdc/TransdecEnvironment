using UnityEditor;
using UnityEngine;

// For new Range subtypes whose generic arguments are displayed as one line in the inspector
// add a new CustomPropertyDrawer attribute with this type below.
[CustomPropertyDrawer(typeof(FloatRange))]
[CustomPropertyDrawer(typeof(IntRange))]
public class RangetDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The 4 comes from extra spacing between the fields (2px each)
        return EditorGUIUtility.singleLineHeight * 3 + 4;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);

        EditorGUI.indentLevel++;

        Rect minRect = new Rect(position.x, position.y + 18, position.width, 16);
        EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"));
        Rect maxRect = new Rect(position.x, position.y + 36, position.width, 16);
        EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"));

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}