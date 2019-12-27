using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void Robosub()
    {
        Debug.Log("ROBOSUB");
        SceneManager.LoadScene("RoboSub2020"); //Tutaj nazwa sceny do pobrania z ustawien
    }

    public void ERL()
    {
        SceneManager.LoadScene("ERL"); //Tutaj nazwa sceny do pobrania z ustawien
    }

    public void SAUVC()
    {
        SceneManager.LoadScene("SAUVC"); //Tutaj nazwa sceny do pobrania z ustawien
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
