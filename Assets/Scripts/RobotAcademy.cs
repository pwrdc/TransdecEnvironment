using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Control {
    learning, player
}

public class RobotAcademy : Academy { 

    [Header("Controller settings")]
    public Control control;
    public Brain learningBrain;
    public Brain playerBrain;

    RobotAgent robot;

    public override void InitializeAcademy() {
        robot = GameObject.Find("Robot").GetComponent<RobotAgent>();
        if (control == Control.learning) {
            robot.GiveBrain(learningBrain);
            broadcastHub.broadcastingBrains.Add(learningBrain);
            broadcastHub.SetControlled(learningBrain, true);
        }
        else {
            robot.GiveBrain(playerBrain);
            broadcastHub.broadcastingBrains.Add(playerBrain);
        }
    }
}
