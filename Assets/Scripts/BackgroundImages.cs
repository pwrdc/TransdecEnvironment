using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImages : MonoBehaviour
{
    [ResetParameter] public bool isBackgroundImage = false;
    public int numberOfPictures;
    public GameObject frontCameraBackground = null;
    public GameObject bottomCameraBackground = null;
    public int frequencyOfImageBackground = 10;

    private int numOfImageToDisplay = 0;
    private int numOfDisplayedImage = 0;

    [ResetParameter] private CameraType activatedCameraType;

    private string[] fileNames;

    private bool isBackgroundAvailable = false;

    private void ApplyResetParameters(){
        this.activatedCameraType =  (CameraType)RobotAcademy.Instance.GetResetParameter("FocusedCamera");
        this.isBackgroundImage = RobotAcademy.Instance.IsResetParameterTrue("EnableBackgroundImage");
    }

    void OnDataCollection(){
        if (isBackgroundImage)
            SetNewBackground();
    }

    void OnApplicationQuit()
    {
        EnableBackgroundImage(false);
    }

    void LoadImages(){
        try
        {
            string path = Application.dataPath + @"\Resources\backgroundImages";
            string[] files = System.IO.Directory.GetFiles(path, "*.jpg");
            fileNames = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string[] words = files[i].Split('\\');
                string word = words[words.Length - 1];
                fileNames[i] = word.Split('.')[0];
            }
            isBackgroundAvailable = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not find background images: " + ex.Message);
        }
    }

    void Start()
    {
        RobotAcademy.Instance.onResetParametersChanged+=ApplyResetParameters;
        RobotAgent.Instance.OnDataCollection+=OnDataCollection;
        LoadImages();
    }

    public void EnableBackgroundImage(bool enable)
    {
        Utils.ActivateEnvironmentMeshRenderer(!enable);
        frontCameraBackground.SetActive(enable);
        bottomCameraBackground.SetActive(enable);
    }

    void Update(){
        EnableBackgroundImage(isBackgroundImage);
    }

    public void SetNewBackground()
    {
        if (isBackgroundAvailable)
        {
            if (activatedCameraType==CameraType.frontCamera)
            {
                SetNewMaterial(frontCameraBackground, numOfImageToDisplay);
            }
            else if (activatedCameraType==CameraType.bottomCamera)
            {
                SetNewMaterial(bottomCameraBackground, numOfImageToDisplay);
            }

            numOfDisplayedImage++;
            if (numOfDisplayedImage % frequencyOfImageBackground == 0)
            {
                ChangeImage();
            }
        }
    }

    void SetNewMaterial(GameObject background, int imageNum)
    {
        Material material = new Material(Shader.Find("Standard"));
        Texture2D backgroundImage = Resources.Load("backgroundImages/" + fileNames[imageNum], typeof(Texture2D)) as Texture2D;
        material.SetTexture("_MainTex", backgroundImage);
        background.GetComponent<MeshRenderer>().material = material;
    }

    void ChangeImage()
    {
        numOfImageToDisplay++;
        if (numOfImageToDisplay > GetNumOfImages())
            numOfImageToDisplay = 1;
    }

    public int GetNumOfImages() { return fileNames.Length; }
}
