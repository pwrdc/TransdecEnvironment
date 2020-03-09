using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSettings : MonoBehaviour
{
    private static TargetSettings mInstance;
    public static TargetSettings Instance => 
        mInstance == null ? (mInstance = FindObjectOfType<TargetSettings>()) : mInstance;

    [ResetParameter] public GameObject target;
    [ResetParameter] public GameObject targetAnnotation;
    [ResetParameter] public int targetIndex;
    [ResetParameter] public CameraType cameraType;
    [ResetParameter] public Vector3 targetOffset = Vector3.zero;
    [ResetParameter] public bool drawBox = false;

    void Awake(){
        target = RobotAcademy.Instance.objectCreator.targetObjects[targetIndex];
        targetAnnotation = RobotAcademy.Instance.objectCreator.targetAnnotations[targetIndex];
    }
    private void ApplyResetParameters(){
        cameraType = (CameraType)RobotAcademy.Instance.resetParameters["FocusedCamera"];
        targetIndex = (int)RobotAcademy.Instance.resetParameters["FocusedObject"];
        target = RobotAcademy.Instance.objectCreator.targetObjects[targetIndex];
        targetAnnotation = RobotAcademy.Instance.objectCreator.targetAnnotations[targetIndex];
        drawBox = RobotAcademy.Instance.IsResetParameterTrue("CollectData");
    }

    void Update(){
        target.SetActive(RobotAgent.Instance.agentSettings.positiveExamples);
    }
}
