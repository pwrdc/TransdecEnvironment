using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ObjectCollection;


public class RobotAcademy : Academy { 
    public enum RobotControl {
        player, python, pad
    }

    public enum DataCollection {
        frontCamera, bottomCamera
    }

    public enum ObjectType {
        Big, Small, OnBottom
    }

    [Header("Controller settings")]
    public RobotControl control;
    public Brain learningBrain;
    public Brain playerBrain;
    public Brain padBrain;

    [Header("Start position settings")]
    public bool randomQuarter = true;
    public bool randomPosition = true;
    public bool randomOrientation = true;


    [Header("Data collection settings")]
    [SerializeField] 
    public ObjectCreator objectCreator = new ObjectCreator(); 

    [Header("Debug settings - use carefully!")]
    public bool forceDataCollection = false;
    public bool forceNoise = false;
    public bool forceNegativeExamples = false;



    RobotAgent robot;
    DataCollection mode;
    GameObject targetObject;
    GameObject targetAnnotation;

    void OnValidate() {
        robot = GameObject.Find("Robot").GetComponent<RobotAgent>();

        BrainControl();

        if (resetParameters["AgentMaxSteps"] > 0)
            robot.targetReset = true;
        else
            robot.targetReset = false;
        if (resetParameters["CollectData"] == 1 || forceDataCollection) {
            robot.sendRelativeData = true;
            robot.dataCollection = true;
            SetFocusedObject((int)resetParameters["FocusedObject"]);

            if(resetParameters["EnableBackgroundImage"] == 0)
                robot.isBackgroundImage = false;
            else
                robot.isBackgroundImage = true;
        }
        else {
            robot.sendRelativeData = false;
            robot.dataCollection = false;
        }
        if (resetParameters["Positive"] == 0 || forceNegativeExamples)
            robot.positiveExamples = false;
        else
            robot.positiveExamples = true;

        if (mode == DataCollection.bottomCamera)
            resetParameters["FocusedCamera"] = 1;
        else
            resetParameters["FocusedCamera"] = 0;

        if(resetParameters["WaterCurrent"] == 0)
            robot.isCurrentEnabled = false;
        else
            robot.isCurrentEnabled = true;


        robot.randomQuarter = randomQuarter;
        robot.randomPosition = randomPosition;
        robot.randomOrientation = randomOrientation;        
    }

    public override void AcademyReset() {
        if (resetParameters["CollectData"] == 1 || forceDataCollection)
        {
            robot.sendRelativeData = true;
            robot.dataCollection = true;
            if(resetParameters["EnableBackgroundImage"] == 0)
                robot.isBackgroundImage = false;
            else
                robot.isBackgroundImage = true;

            if (resetParameters["EnableNoise"] == 1 || forceNoise) {
                robot.addNoise = true;
                robot.noise.SetActive(true);
            }
            else {
                robot.addNoise = false;
                robot.noise.SetActive(false);
            }

            SetFocusedObject((int)resetParameters["FocusedObject"]);
        }
        else
        {
            robot.sendRelativeData = false;
            robot.dataCollection = false;
        }
        if (resetParameters["Positive"] == 0 || forceNegativeExamples)
            robot.positiveExamples = false;
        else 
            robot.positiveExamples = true;

        if (mode == DataCollection.bottomCamera)
            resetParameters["FocusedCamera"] = 1;
        else
            resetParameters["FocusedCamera"] = 0;

        if(resetParameters["WaterCurrent"] == 0)
            robot.isCurrentEnabled = false;
        else
            robot.isCurrentEnabled = true;
    }

    public override void InitializeAcademy() {        
        robot = GameObject.Find("Robot").GetComponent<RobotAgent>();
        robot.agentParameters.maxStep = (int)(resetParameters["AgentMaxSteps"]);
        if (resetParameters["CollectData"] == 1 || forceDataCollection) {
            if(resetParameters["EnableBackgroundImage"] == 0)
                robot.isBackgroundImage = false;
            else
                robot.isBackgroundImage = true;

            if (resetParameters["EnableNoise"] == 1 || forceNoise)
                robot.addNoise = true;
            else
                robot.addNoise = false;
        }
    }

    public void SetFocusedObject(int index) {
        for(int i = 0; i < objectCreator.targetObjects.Count; i++) {
            objectCreator.targetIsEnabled[i] = false;
        }
        objectCreator.targetIsEnabled[index] = true;


        mode = objectCreator.targetCameraModes[index];
        targetObject = objectCreator.targetObjects[index];
        targetAnnotation = objectCreator.targetAnnotations[index];

        robot.target = targetObject;
        robot.targetAnnotation = targetAnnotation;
        robot.targetMode = mode;
        robot.targetIndex = index;

        resetParameters["FocusedObject"] = index;

        robot.SetAgent();
    }

    private void BrainControl() {
        if (control == RobotControl.player) {
            robot.GiveBrain(playerBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(playerBrain);
            robot.collectObservations = true;
            robot.targetReset = true;
        }
        else if (control == RobotControl.pad) {
            robot.GiveBrain(padBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(padBrain);
            broadcastHub.SetControlled(padBrain, true);
            robot.collectObservations = false;
            robot.targetReset = false;
        }
        else {
            robot.GiveBrain(learningBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(learningBrain);
            broadcastHub.SetControlled(learningBrain, true);
            robot.collectObservations = true;
            robot.targetReset = false;
        }
    }
}
