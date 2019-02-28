using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RobotControl {
    player, python
}

public enum DataCollection {
    gate, path
}

public class RobotAcademy : Academy { 

    [Header("Controller settings")]
    public RobotControl control;
    public Brain learningBrain;
    public Brain playerBrain;

    [Header("Data collection settings")]
    public DataCollection mode;
    public GameObject gateTargetObject;
    public GameObject pathTargetObject;

    RobotAgent robot;

    void OnValidate() {
        if (control == RobotControl.player) {
            robot = GameObject.Find("Robot").GetComponent<RobotAgent>();
            robot.GiveBrain(playerBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(playerBrain);
            
        }
        else {
            robot.GiveBrain(learningBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(learningBrain);
            broadcastHub.SetControlled(learningBrain, true);
        }
    }

    public override void InitializeAcademy() {
        if (control == RobotControl.player) {
            robot.agentParameters.maxStep = 0;
        }
    }
}
