using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitializedSettings
{
    private static RobotControl control;
    private static bool isCollecting;

    public static RobotControl Control { get => control; set => control = value; }
    public static bool IsCollecting { get => isCollecting; set => isCollecting = value; }
}
