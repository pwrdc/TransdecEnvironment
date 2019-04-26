using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotAcademy : Academy { 
    public enum RobotControl {
        player, python, pad
    }

    public enum DataCollection {
        gate, path
    }

    public enum CameraID {
        frontCamera = 0, bottomCamera = 1
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
    public DataCollection mode;
    public GameObject gateTargetObject;
    public GameObject pathTargetObject;

    [Header("Debug settings - use carefully!")]
    public bool forceDataCollection = false;
    public bool forceNoise = false;
    public bool forceNegativeExamples = false;

    RobotAgent robot;

    void OnValidate() {
        robot = GameObject.Find("Robot").GetComponent<RobotAgent>();
        robot.mode = mode;
        robot.gateTargetObject = gateTargetObject;
        robot.pathTargetObject = pathTargetObject;
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
        if (resetParameters["AgentMaxSteps"] > 0)
            robot.targetReset = true;
        else
            robot.targetReset = false;
        if (resetParameters["CollectData"] == 1 || forceDataCollection) {
            robot.sendRelativeData = true;
            robot.dataCollection = true;
        }
        else {
            robot.sendRelativeData = false;
            robot.dataCollection = false;
        }
        if (resetParameters["Positive"] == 0 || forceNegativeExamples)
            robot.positiveExamples = false;
        else
            robot.positiveExamples = true;

        if (mode == DataCollection.path)
            resetParameters["FocusedCamera"] = 1;
        else
            resetParameters["FocusedCamera"] = 0;

        robot.isCurrentEnabled = (resetParameters["WaterCurrent"] == 1 ? true : false);
        robot.randomQuarter = randomQuarter;
        robot.randomPosition = randomPosition;
        robot.randomOrientation = randomOrientation;
    }

    public override void AcademyReset() {
        if (resetParameters["CollectData"] == 1 || forceDataCollection)
        {
            robot.sendRelativeData = true;
            robot.dataCollection = true;
            robot.mode = mode;
            robot.gateTargetObject = gateTargetObject;
            robot.pathTargetObject = pathTargetObject;
            if (resetParameters["EnableNoise"] == 1 || forceNoise)
                robot.addNoise = true;
            else
                robot.addNoise = false;
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

    }

    public override void InitializeAcademy() {
        robot.agentParameters.maxStep = (int)(resetParameters["AgentMaxSteps"]);
        if (resetParameters["CollectData"] == 1 || forceDataCollection) {
            if (resetParameters["EnableNoise"] == 1 || forceNoise)
                robot.addNoise = true;
            else
                robot.addNoise = false;
        }
        robot.focusedCamera = (int)resetParameters["FocusedCamera"];
    }
}
