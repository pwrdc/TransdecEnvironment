using UnityEngine;

public class RecordingMode : MonoBehaviour
{
    [HelpBox("If enabled reduces frame rate and decreases simulation speed. \n"
            +"Used to enable video recording on low-end PCs in lower frame rate. \n"
            +"The result video recording can be later sped up to Result Frame Rate in a video editing sofware.")]
    public int recordingFrameRate=10;
    public int resultFrameRate = 30;
    int savedVSyncCount;

    void Update()
    {
        // VSync must be disabled
        savedVSyncCount = QualitySettings.vSyncCount;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = recordingFrameRate;
        Time.timeScale = (float)recordingFrameRate/resultFrameRate;
    }

    void OnDisable()
    {
        QualitySettings.vSyncCount= savedVSyncCount;
        Application.targetFrameRate=-1;
        Time.timeScale = 1;
    }
}
