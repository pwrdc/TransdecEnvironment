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
    private int specificId = -1;

    /// <summary>
    /// Computes the height of the Drawer depending on the property it is showing
    /// </summary>
    /// <param name="property">The property that is being drawn.</param>
    /// <param name="label">The label of the property being drawn.</param>
    /// <returns>The vertical space needed to draw the property.</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        InitializeObjectCreator(property, label);
        var numLines = objectCreator.targetObjects.Count * 2 + 3;
        return (numLines) * LineHeight + ExtraSpaceBelow;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if(specificId == -1)
            specificId = objectCreator.targetObjects.Count; 

        /*
        objectCreator.targetObjects = new List<GameObject>();
        objectCreator.targetAnnotations = new List<GameObject>();
        objectCreator.targetIsEnabled = new List<bool>();
        objectCreator.targetCameraModes = new List<RobotAcademy.DataCollection>();
    */
        randomPosition = GameObject.Find("Agent").GetComponent<RandomPosition>();
        robotAcademy = GameObject.Find("Academy").GetComponent<RobotAcademy>();
        InitializeObjectCreator(property, label);
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel++;
        position.height = LineHeight;
        DrawAddButton(position, property);

        EditorGUI.indentLevel++;
        position.y += LineHeight * 2;
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
            "Add New", "Add new object");
        if (GUI.Button(addButtonRect, buttonContent, EditorStyles.miniButton))
        {
            MarkSceneAsDirty();
            AddNewObject();
        }

        // This is the rectangle for the Add button
        EditorGUI.LabelField(new Rect(position.x, position.y + LineHeight, position.width / 4, position.height), "specific id:");
        var specificIdFieldRect = new Rect(position.x + position.width / 4, position.y + LineHeight, position.width / 4, position.height);
        
        specificId = EditorGUI.IntField(specificIdFieldRect, specificId);

        var addSpecificIdButtonRect = new Rect(position.x + position.width / 2, position.y + LineHeight, position.width / 2, position.height);
        var buttonSpecificIdContent = new GUIContent(
            "Add with specific id", "Specify id with int on left side");
        if (GUI.Button(addSpecificIdButtonRect, buttonSpecificIdContent, EditorStyles.miniButton))
        {
            MarkSceneAsDirty();
            InsertNewObject(specificId);
        }
    }

    /// <summary>
    /// Draws the Brain and Control checkbox for the brains contained in the BroadCastHub.
    /// </summary>
    /// <param name="brainRect">The Rect to draw the Brains.</param>
    /// <param name="controlRect">The Rect to draw the control checkbox.</param>
    private void DrawTargetObjects(Rect position, SerializedProperty property)
    {   

        int widthOfPartRow1 = (int)(position.width / 4);
        int widthOfPartRow2 = (int)(position.width / 3);

        int heightOfRow = (int)(2 * LineHeight);

        Rect targetIndexRect = position;
        targetIndexRect.width = widthOfPartRow1 / 2;
        targetIndexRect.x = widthOfPartRow1 * 0;
        Rect targetEnableRect = position;
        targetEnableRect.width = widthOfPartRow1 / 2;
        targetEnableRect.x = widthOfPartRow1 * 0.5f;
        Rect targetObjRect = position;
        targetObjRect.width = widthOfPartRow1;
        targetObjRect.x = widthOfPartRow1 * 1;
        Rect targetAnnotRect = position;
        targetAnnotRect.width = widthOfPartRow1;
        targetAnnotRect.x = widthOfPartRow1 * 2;
        Rect modeCameraTypeRect = position;
        modeCameraTypeRect.width = widthOfPartRow1;
        modeCameraTypeRect.x = widthOfPartRow1 * 3;
        Rect modeObjectTypeRect = position;
        modeObjectTypeRect.width = widthOfPartRow2;
        modeObjectTypeRect.x = widthOfPartRow2 * 1;
        Rect removeButtonRect = position;
        removeButtonRect.width = widthOfPartRow2;
        removeButtonRect.x = widthOfPartRow2 * 2;


        EditorGUI.LabelField(targetObjRect, "Object");
        EditorGUI.LabelField(targetAnnotRect, "Annotation");
        targetIndexRect.y += LineHeight;
        targetEnableRect.y += LineHeight;
        targetObjRect.y += LineHeight;
        targetAnnotRect.y += LineHeight;
        modeCameraTypeRect.y += LineHeight;

        removeButtonRect.y += LineHeight * 2;
        modeObjectTypeRect.y += LineHeight * 2;

        for (int index = 0; index < objectCreator.targetObjects.Count; index++)
        {
            var targetObject = objectCreator.targetObjects[index];
            var targetAnnotation = objectCreator.targetAnnotations[index];
            var targetIsEnabled = objectCreator.targetIsEnabled[index];
            
            //Index of object
            EditorGUI.LabelField(targetIndexRect, index + ":");

            //Toggle of activity object
            EditorGUI.BeginChangeCheck();
            targetIsEnabled = EditorGUI.Toggle(
                targetEnableRect, "", targetIsEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                robotAcademy.SetFocusedObject(index);
                MarkSceneAsDirty();
            }

            //TargetObject
            EditorGUI.BeginChangeCheck();
            var targetObj = EditorGUI.ObjectField(
                targetObjRect, targetObject, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                ChangeTargetObject(index, targetObj);
                MarkSceneAsDirty();
            }

            //TargetAnnotation
            EditorGUI.BeginChangeCheck();
            var targetAnnot = EditorGUI.ObjectField(
                targetAnnotRect, targetAnnotation, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                ChangeTargetAnnot(index, targetAnnot);
                MarkSceneAsDirty();
            }

            //ObjectCamera Enum
            EditorGUI.BeginChangeCheck();
            objectCreator.targetCameraModes[index] = (RobotAcademy.DataCollection)EditorGUI.EnumPopup(
                modeCameraTypeRect,
                "",
                objectCreator.targetCameraModes[index]);
            if (EditorGUI.EndChangeCheck())
            {
                MarkSceneAsDirty();
                ChangeCameraMode(index, objectCreator.targetCameraModes[index]);
            }

            //ObjectType Enum
            EditorGUI.BeginChangeCheck();
            objectCreator.targetObjectModes[index] = (RobotAcademy.ObjectType)EditorGUI.EnumPopup(
                modeObjectTypeRect,
                "",
                objectCreator.targetObjectModes[index]);
            if (EditorGUI.EndChangeCheck())
            {
                MarkSceneAsDirty();
                ChangeObjectMode(index, objectCreator.targetObjectModes[index]);
            }

            //Remove button
            var buttonContent = new GUIContent("Remove");
            if (GUI.Button(removeButtonRect, buttonContent, EditorStyles.miniButton)) {
                MarkSceneAsDirty();
                RemoveObject(index);
            }

            targetEnableRect.y += heightOfRow;
            targetObjRect.y += heightOfRow;
            targetAnnotRect.y += heightOfRow;
            modeCameraTypeRect.y += heightOfRow;
            modeObjectTypeRect.y += heightOfRow;
            removeButtonRect.y += heightOfRow;
            targetIndexRect.y += heightOfRow;
        }
    }

    private void AddNewObject() {
        objectCreator.targetObjects.Add(null);
        objectCreator.targetAnnotations.Add(null);
        objectCreator.targetIsEnabled.Add(false);
        objectCreator.targetCameraModes.Add(RobotAcademy.DataCollection.frontCamera);
        objectCreator.targetObjectModes.Add(RobotAcademy.ObjectType.Small);
        randomPosition.AddNewObject(RobotAcademy.DataCollection.frontCamera, RobotAcademy.ObjectType.Small);
    }

    private void InsertNewObject(int index) {
        if(index > objectCreator.targetObjects.Count)
            index = objectCreator.targetObjects.Count;
            
        objectCreator.targetObjects.Insert(index, null);
        objectCreator.targetAnnotations.Insert(index, null);
        objectCreator.targetIsEnabled.Insert(index, false);
        objectCreator.targetCameraModes.Insert(index, RobotAcademy.DataCollection.frontCamera);
        objectCreator.targetObjectModes.Insert(index, RobotAcademy.ObjectType.Small);
        randomPosition.InsertNewObject(index, RobotAcademy.DataCollection.frontCamera, RobotAcademy.ObjectType.Small);
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
        objectCreator.targetCameraModes.RemoveAt(index);        
        objectCreator.targetObjectModes.RemoveAt(index);
        objectCreator.targetIsEnabled.RemoveAt(index);
        randomPosition.DeleteObject(index);
    }

    private void ChangeCameraMode(int index, RobotAcademy.DataCollection mode) {
        randomPosition.SetCameraMode(index, objectCreator.targetCameraModes[index]);
    }

    private void ChangeObjectMode(int index, RobotAcademy.ObjectType type) {
        randomPosition.SetObjectMode(index, objectCreator.targetObjectModes[index]);
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
