using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class Escape : MonoBehaviour
{
    [Scene]
    public string mainMenuScene;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
