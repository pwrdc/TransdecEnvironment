using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundSettings
{
    public bool isBackgroundImage = false;
    public GameObject frontCameraBackground = null;
    public GameObject bottomCameraBackground = null;
    public int frequencyOfImageBackground = 10;
    public int numOfImageToDisplay = 5;
}

public class BackgroundImageManager : MonoBehaviour
{
    private int numberOfPictures;
    private GameObject frontCameraBackground = null;
    private GameObject bottomCameraBackground = null;
    private int frequencyOfImageBackground = 10;

    private int numOfImageToDisplay = 0;
    private int numOfDisplayedImage = 0;

    private CameraType activatedCameraType;

    private string[] fileNames;

    private bool isBackgroundAvailable = false;

    void Awake()
    {
        RobotAgent.Instance.OnDataBackgroundUpdate += UpdateData;
    }

    void Start()
    {
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

    public void EnableBackgroundImage(bool enable)
    {
        Utils.ActivateEnvironmentMeshRenderer(!enable);
        frontCameraBackground.SetActive(enable);
        bottomCameraBackground.SetActive(enable);
    }

    public void SetNewBackground()
    {
        if (isBackgroundAvailable)
        {
            if (CameraType.frontCamera == activatedCameraType)
            {
                SetNewMaterial(frontCameraBackground, numOfImageToDisplay);
            }
            else if (CameraType.bottomCamera == activatedCameraType)
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

    public void UpdateData(BackgroundSettings settings)
    {
        this.bottomCameraBackground = settings.bottomCameraBackground;
        this.frontCameraBackground = settings.frontCameraBackground;
        this.numberOfPictures = settings.numOfImageToDisplay;
        this.frequencyOfImageBackground = settings.frequencyOfImageBackground;
    }

    public void UpdateData(TargetSettings settings)
    {
        this.activatedCameraType = settings.cameraType;
    }

    public int GetNumOfImages() { return fileNames.Length; }
}
