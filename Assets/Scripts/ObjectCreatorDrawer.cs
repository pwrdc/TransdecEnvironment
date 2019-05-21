using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ObjectCreator))]
public class ObjectCreatorDrawer : PropertyDrawer
{
    private ObjectCreator objectCreator;
    private RobotAcademy.DataCollection mode;
    private RandomPosition randomPosition;
    private RobotAcademy robotAcademy;

    // The height of a line in the Unity Inspectors
    private const float LineHeight = 17f;
    // The vertical space left below the BroadcastHub UI.
    private const float ExtraSpaceBelow = 10f;
    // The horizontal size of the Control checkbox
    private const int ControlSize = 80;

    /// <summary>
    /// Computes the height of the Drawer depending on the property it is showing
    /// </summary>
    /// <param name="property">The property that is being drawn.</param>
    /// <param name="label">The label of the property being drawn.</param>
    /// <returns>The vertical space needed to draw the property.</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        InitializeObjectCreator(property, label);
        var numLines = objectCreator.targetObjects.Count + 2;
        return (numLines) * LineHeight + ExtraSpaceBelow;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


        /*
        objectCreator.targetObjects = new List<GameObject>();
        objectCreator.targetAnnotations = new List<GameObject>();
        objectCreator.targetIsEnabled = new List<bool>();
        objectCreator.targetModes = new List<RobotAcademy.DataCollection>();
    */
        randomPosition = GameObject.Find("Agent").GetComponent<RandomPosition>();
        robotAcademy = GameObject.Find("Academy").GetComponent<RobotAcademy>();
        InitializeObjectCreator(property, label);
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel++;
        position.height = LineHeight;
        DrawAddButton(position, property);

        EditorGUI.indentLevel++;
        position.y += LineHeight;
        DrawTargetObjects(position, property);

        EditorGUI.EndProperty();
    }

    /// <summary>
    /// Draws the Add and Remove buttons.
    /// </summary>
    /// <param name="position">The position at which to draw.</param>
    private void DrawAddButton(Rect position, SerializedProperty property)
    {
        // This is the rectangle for the Add button
        var addButtonRect = position;
        var buttonContent = new GUIContent(
            "Add New", "Add a new Brain to the Broadcast Hub");
        if (GUI.Button(addButtonRect, buttonContent, EditorStyles.miniButton))
        {
            MarkSceneAsDirty();
            AddNewObject();
        }
    }

    /// <summary>
    /// Draws the Brain and Control checkbox for the brains contained in the BroadCastHub.
    /// </summary>
    /// <param name="brainRect">The Rect to draw the Brains.</param>
    /// <param name="controlRect">The Rect to draw the control checkbox.</param>
    private void DrawTargetObjects(Rect position, SerializedProperty property)
    {   

        int widthOfPart = (int)(position.width / 5);
        int widthObject = (int)(position.width / 5);

        Rect targetIndexRect = position;
        targetIndexRect.width = widthObject / 2;
        targetIndexRect.x = widthOfPart * 0;
        Rect targetEnableRect = position;
        targetEnableRect.width = widthObject / 2;
        targetEnableRect.x = widthOfPart * 0.5f;
        Rect targetObjRect = position;
        targetObjRect.width = widthObject;
        targetObjRect.x = widthOfPart * 1;
        Rect targetAnnotRect = position;
        targetAnnotRect.width = widthObject;
        targetAnnotRect.x = widthOfPart * 2;
        Rect modeTypeRect = position;
        modeTypeRect.width = widthObject;
        modeTypeRect.x = widthOfPart * 3;
        Rect removeButtonRect = position;
        removeButtonRect.width = widthObject;
        removeButtonRect.x = widthOfPart * 4;


        EditorGUI.LabelField(targetObjRect, "Object");
        EditorGUI.LabelField(targetAnnotRect, "Annotation");
        targetIndexRect.y += LineHeight;
        targetEnableRect.y += LineHeight;
        targetObjRect.y += LineHeight;
        targetAnnotRect.y += LineHeight;
        removeButtonRect.y += LineHeight;
        modeTypeRect.y += LineHeight;

        for (var index = 0; index < objectCreator.targetObjects.Count; index++)
        {
            //Debug.Log(objectCreator.targetObjects[index]);
            var targetObject = objectCreator.targetObjects[index];
            var targetAnnotation = objectCreator.targetAnnotations[index];
            var targetIsEnabled = objectCreator.targetIsEnabled[index];
            
            EditorGUI.LabelField(targetIndexRect, index + ":");
            targetIndexRect.y += LineHeight;

            EditorGUI.BeginChangeCheck();
            targetIsEnabled = EditorGUI.Toggle(
                targetEnableRect, "", targetIsEnabled);
            targetEnableRect.y += LineHeight;
            if (EditorGUI.EndChangeCheck())
            {
                robotAcademy.SetFocusedObject(index);
                MarkSceneAsDirty();
            }
            EditorGUI.BeginChangeCheck();
            var targetObj = EditorGUI.ObjectField(
                targetObjRect, targetObject, typeof(GameObject), true) as GameObject;
            //Debug.Log(targetObj);
            targetObjRect.y += LineHeight;
            if (EditorGUI.EndChangeCheck())
            {
                ChangeTargetObject(index, targetObj);
                MarkSceneAsDirty();
            }

            EditorGUI.BeginChangeCheck();
            var targetAnnot = EditorGUI.ObjectField(
                targetAnnotRect, targetAnnotation, typeof(GameObject), true) as GameObject;
            targetAnnotRect.y += LineHeight;
            if (EditorGUI.EndChangeCheck())
            {
                ChangeTargetAnnot(index, targetAnnot);
                MarkSceneAsDirty();
            }

            EditorGUI.BeginChangeCheck();
            objectCreator.targetModes[index] = (RobotAcademy.DataCollection)EditorGUI.EnumPopup(
                modeTypeRect,
                "",
                objectCreator.targetModes[index]);
            if (EditorGUI.EndChangeCheck())
            {
                MarkSceneAsDirty();
                ChangeMode(index, objectCreator.targetModes[index]);
            }
            modeTypeRect.y += LineHeight;


            var buttonContent = new GUIContent("Remove");
            if (GUI.Button(removeButtonRect, buttonContent, EditorStyles.miniButton)) {
                MarkSceneAsDirty();
                RemoveObject(index);
            }
            removeButtonRect.y += LineHeight;
        }
    }

    private void AddNewObject() {
        objectCreator.targetObjects.Add(null);
        objectCreator.targetAnnotations.Add(null);
        objectCreator.targetIsEnabled.Add(false);
        objectCreator.targetModes.Add(RobotAcademy.DataCollection.frontCamera);
        randomPosition.AddNewObject(RobotAcademy.DataCollection.frontCamera);
    }

    private void ChangeTargetObject(int index, GameObject targetObj) {
        objectCreator.targetObjects[index] = targetObj;
    }

    private void ChangeTargetAnnot(int index, GameObject targetAnnot) {
        objectCreator.targetAnnotations[index] = targetAnnot;
    }

    private void RemoveObject(int index) {
        objectCreator.targetObjects.RemoveAt(index);
        objectCreator.targetAnnotations.RemoveAt(index);
        objectCreator.targetModes.RemoveAt(index);
        objectCreator.targetIsEnabled.RemoveAt(index);
        randomPosition.DeleteObject(index);
    }

    private void ChangeMode(int index, RobotAcademy.DataCollection mode) {
        randomPosition.SetMode(index, objectCreator.targetModes[index]);
    }

    private void InitializeObjectCreator(SerializedProperty property, GUIContent label)
    {
        if (objectCreator != null)
        {
            return;
        }
        var target = property.serializedObject.targetObject;
        objectCreator = fieldInfo.GetValue(target) as ObjectCreator;
        if (objectCreator == null)
        {
            objectCreator = new ObjectCreator();
            fieldInfo.SetValue(target, objectCreator);
        }
    }

    /// <summary>
    /// Signals that the property has been modified and requires the scene to be saved for
    /// the changes to persist. Only works when the Editor is not playing.
    /// </summary>
    private static void MarkSceneAsDirty()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
