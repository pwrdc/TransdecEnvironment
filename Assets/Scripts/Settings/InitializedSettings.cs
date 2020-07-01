using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitializedSettings
{
    public static RobotControl Control { get; set; }
    public static bool IsCollecting { get; set; }
    public static bool IsMenu { get; set; } = false;
    public static int targetsFolderIndex { get; set; } = 0;
}
