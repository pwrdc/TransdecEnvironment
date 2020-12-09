using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text.RegularExpressions;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown controlDropdown = null;
    public TMP_Dropdown modeDropdown = null;
    public TMP_Dropdown sceneDropdown = null;
    public TMP_Dropdown targetsDropdown = null;
    public Targets targets;
    public string[] scenes;

    void Start()
    {
        // if the scene with main menu was loaded use settings
        Settings.Used = true;
        Settings.Load();
        InitializeDropdowns();
    }

    void OnDisable() => Settings.Save();
    void OnDestroy() => Settings.Save();
    void OnApplicationQuit() => Settings.Save();

    string ProcessIdentifier(string identifier)
    {
        // https://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters/4489046
        Regex r = new Regex(@"
                 (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
        // split on capital letters
        string result = r.Replace(identifier, " ");
        // capitalize the first word
        result = char.ToUpper(result[0]) + result.Substring(1);
        return result;
    }

    void InitializeDropdowns()
    {
        // for each dropdown:
        // add options names
        // transfer current selection from settings
        // and add change event listener

        modeDropdown.AddOptions(Enum.GetNames(typeof(SimulationMode)).Select(ProcessIdentifier).ToList());
        modeDropdown.value = (int)Settings.Mode;
        modeDropdown.onValueChanged.AddListener(value => Settings.Mode = (SimulationMode)value);

        controlDropdown.AddOptions(Enum.GetNames(typeof(RobotControl)).Select(ProcessIdentifier).ToList());
        controlDropdown.value = (int)Settings.Control;
        controlDropdown.onValueChanged.AddListener(value => Settings.Control = (RobotControl)value);
        
        List<string> targetsFolderNames = targets.targetsFolders.Select(folder => ProcessIdentifier(folder.name)).ToList();
        targetsDropdown.AddOptions(targetsFolderNames);
        targetsDropdown.value = Settings.TargetsFolderIndex;
        targetsDropdown.onValueChanged.AddListener(value => Settings.TargetsFolderIndex = value);

        sceneDropdown.AddOptions(scenes.Select(ProcessIdentifier).ToList());
        sceneDropdown.value = Settings.SceneIndex;
        sceneDropdown.onValueChanged.AddListener(value => Settings.SceneIndex = value);
    }

    // Functions called from UI buttons

    public void PlayGame()
    {
        SceneManager.LoadScene(scenes[Settings.SceneIndex]);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
