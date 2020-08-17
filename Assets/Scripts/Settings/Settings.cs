using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SimulationMode
{
    FreeSwim,
    DataCollection
}

public static class Settings
{
    public static RobotControl Control { get; set; }
    public static SimulationMode Mode { get; set; }
    public static int TargetsFolderIndex { get; set; } = 0;
    public static int SceneIndex { get; set; } = 0;

    // if true the settings should be used in the simulation
    public static bool Used { get; set; } = false;

    /// <summary>
    /// Saves settings to the disc.
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.SetInt("Control", (int)Control);
        PlayerPrefs.SetInt("Mode", (int)Mode);
        PlayerPrefs.SetInt("TargetsFolderIndex", TargetsFolderIndex);
        PlayerPrefs.SetInt("SceneIndex", SceneIndex);
    }

    /// <summary>
    /// Loads settings from the disc.
    /// </summary>
    public static void Load()
    {
        Control = (RobotControl)PlayerPrefs.GetInt("Control");
        Mode = (SimulationMode)PlayerPrefs.GetInt("Mode");
        TargetsFolderIndex = PlayerPrefs.GetInt("TargetsFolderIndex");
        SceneIndex = PlayerPrefs.GetInt("SceneIndex");
    }
}
