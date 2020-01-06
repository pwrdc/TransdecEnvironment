using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown sceneDropdown;
    [SerializeField]
    private TMP_Dropdown controllDropdown;
    [SerializeField]
    private TMP_Dropdown modeDropdown;

    private string sceneToLoad = "SAUVC";

    private void Start()
    {
        InitializedSettings.Control = RobotControl.player;
        InitializedSettings.IsCollecting = true;
        InitializedSettings.IsMenu = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(sceneToLoad); //Tutaj nazwa sceny do pobrania z ustawien
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void updateMenu()
    {
        sceneToLoad = sceneDropdown.options[sceneDropdown.value].text;
        switch(controllDropdown.value)
        {
            case 0:
                InitializedSettings.Control = RobotControl.player;
                break;
            case 1:
                InitializedSettings.Control = RobotControl.python;
                break;
            case 2:
                InitializedSettings.Control = RobotControl.pythonNoImage;
                break;
            default:
                InitializedSettings.Control = RobotControl.player;
                break;
        }

        switch(modeDropdown.value)
        {
            case 0:
                InitializedSettings.IsCollecting = true;
                break;
            case 1:
                InitializedSettings.IsCollecting = false;
                break;
            default:
                InitializedSettings.IsCollecting = false;
                break;
        }
    }
}
