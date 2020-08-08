using UnityEngine;
using UnityEngine.SceneManagement;

public class Escape : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
