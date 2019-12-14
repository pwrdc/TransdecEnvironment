// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-22-2019
// ***********************************************************************
// <copyright file="ObjectCreatorDrawer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Objects
{
    /// <summary>
    /// Class ObjectCreator.
    /// </summary>
    [System.Serializable]
    public class ObjectCreator
    {
        /// <summary>
        /// The target camera modes
        /// </summary>
        [SerializeField]
        public List<CameraType> targetCameraModes = new List<CameraType>();
        /// <summary>
        /// The target object types
        /// </summary>
        [SerializeField]
        public List<ObjectType> targetObjectTypes = new List<ObjectType>();
        /// <summary>
        /// The target objects
        /// </summary>
        [SerializeField]
        public List<GameObject> targetObjects = new List<GameObject>();
        /// <summary>
        /// The target annotations
        /// </summary>
        [SerializeField]
        public List<GameObject> targetAnnotations = new List<GameObject>();
        /// <summary>
        /// The target is enabled
        /// </summary>
        [SerializeField]
        public List<bool> targetIsEnabled = new List<bool>();
    }
#if UNITY_EDITOR
    /// <summary>
    /// Class ObjectCreatorDrawer.
    /// Implements the <see cref="UnityEditor.PropertyDrawer" />
    /// </summary>
    /// <seealso cref="UnityEditor.PropertyDrawer" />
    [CustomPropertyDrawer(typeof(ObjectCreator))]
    public class ObjectCreatorDrawer : PropertyDrawer
    {
        /// <summary>
        /// The object creator
        /// </summary>
        private ObjectCreator objectCreator;
        /// <summary>
        /// The mode
        /// </summary>
        private CameraType mode;
        /// <summary>
        /// The object spawn configuration
        /// </summary>
        private ObjectSpawnConfiguration objectSpawnConfiguration;
        /// <summary>
        /// The robot academy
        /// </summary>
        private RobotAcademy robotAcademy;

        /// <summary>
        /// Gets the object creator.
        /// </summary>
        /// <value>The object creator.</value>
        private ObjectCreator ObjectCreator { get { return objectCreator; } }

        // The height of a line in the Unity Inspectors
        /// <summary>
        /// The line height
        /// </summary>
        private const float LineHeight = 17f;
        // The vertical space left below the BroadcastHub UI.
        /// <summary>
        /// The extra space below
        /// </summary>
        private const float ExtraSpaceBelow = 10f;
        // The horizontal size of the Control checkbox
        /// <summary>
        /// The control size
        /// </summary>
        private const int ControlSize = 80;
        /// <summary>
        /// The specific identifier
        /// </summary>
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

        /// <summary>
        /// Override this method to make your own GUI for the property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (specificId == -1)
                specificId = objectCreator.targetObjects.Count;

            /*
            objectCreator.targetObjects = new List<GameObject>();
            objectCreator.targetAnnotations = new List<GameObject>();
            objectCreator.targetIsEnabled = new List<bool>();
            objectCreator.targetCameraModes = new List<RobotAcademy.DataCollection>();
        */
            if (robotAcademy == null && objectSpawnConfiguration == null)
            {
                robotAcademy = GameObject.Find("Academy").GetComponent<RobotAcademy>();
                objectSpawnConfiguration = GameObject.Find("Objects").GetComponent<ObjectSpawnConfiguration>();
            }
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
        /// <param name="property">The property.</param>
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
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
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
                objectCreator.targetCameraModes[index] = (CameraType)EditorGUI.EnumPopup(
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
                objectCreator.targetObjectTypes[index] = (ObjectType)EditorGUI.EnumPopup(
                    modeObjectTypeRect,
                    "",
                    objectCreator.targetObjectTypes[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    MarkSceneAsDirty();
                    ChangeObjectMode(index, objectCreator.targetObjectTypes[index]);
                }

                //Remove button
                var buttonContent = new GUIContent("Remove");
                if (GUI.Button(removeButtonRect, buttonContent, EditorStyles.miniButton))
                {
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

        /// <summary>
        /// Adds the new object.
        /// </summary>
        private void AddNewObject()
        {
            objectCreator.targetObjects.Add(null);
            objectCreator.targetAnnotations.Add(null);
            objectCreator.targetIsEnabled.Add(false);
            objectCreator.targetCameraModes.Add(CameraType.frontCamera);
            objectCreator.targetObjectTypes.Add(ObjectType.Small);
            objectSpawnConfiguration.AddNewObject(CameraType.frontCamera, ObjectType.Small, null);
        }

        /// <summary>
        /// Inserts the new object.
        /// </summary>
        /// <param name="index">The index.</param>
        private void InsertNewObject(int index)
        {
            if (index > objectCreator.targetObjects.Count)
                index = objectCreator.targetObjects.Count;

            objectCreator.targetObjects.Insert(index, null);
            objectCreator.targetAnnotations.Insert(index, null);
            objectCreator.targetIsEnabled.Insert(index, false);
            objectCreator.targetCameraModes.Insert(index, CameraType.frontCamera);
            objectCreator.targetObjectTypes.Insert(index, ObjectType.Small);
            objectSpawnConfiguration.InsertNewObject(index, CameraType.frontCamera, ObjectType.Small);
        }

        /// <summary>
        /// Changes the target object.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="targetObj">The target object.</param>
        private void ChangeTargetObject(int index, GameObject targetObj)
        {
            objectCreator.targetObjects[index] = targetObj;
            objectSpawnConfiguration.SetObjectTarget(index, targetObj);
        }

        /// <summary>
        /// Changes the target annot.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="targetAnnot">The target annot.</param>
        private void ChangeTargetAnnot(int index, GameObject targetAnnot)
        {
            objectCreator.targetAnnotations[index] = targetAnnot;
            objectSpawnConfiguration.SetObjectTargetAnnotation(index, targetAnnot);
        }

        /// <summary>
        /// Removes the object.
        /// </summary>
        /// <param name="index">The index.</param>
        private void RemoveObject(int index)
        {
            objectCreator.targetObjects.RemoveAt(index);
            objectCreator.targetAnnotations.RemoveAt(index);
            objectCreator.targetCameraModes.RemoveAt(index);
            objectCreator.targetObjectTypes.RemoveAt(index);
            objectCreator.targetIsEnabled.RemoveAt(index);
            objectSpawnConfiguration.DeleteObject(index);
        }

        /// <summary>
        /// Changes the camera mode.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="mode">The mode.</param>
        private void ChangeCameraMode(int index, CameraType mode)
        {
            objectSpawnConfiguration.SetCameraMode(index, objectCreator.targetCameraModes[index]);
        }

        /// <summary>
        /// Changes the object mode.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="type">The type.</param>
        private void ChangeObjectMode(int index, ObjectType type)
        {
            objectSpawnConfiguration.SetObjectType(index, objectCreator.targetObjectTypes[index]);
        }

        /// <summary>
        /// Initializes the object creator.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
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
#endif
}