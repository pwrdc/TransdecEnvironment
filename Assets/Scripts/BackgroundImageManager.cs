// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-22-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-28-2019
// ***********************************************************************
// <copyright file="BackgroundImageManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates background for view
/// </summary>
[System.Serializable]
public class BackgroundSettings
{
    /// <summary>
    /// Background image
    /// </summary>
    public bool isBackgroundImage = false;
    /// <summary>
    /// Front camera plane where background image is placed
    /// </summary>
    public GameObject frontCameraBackground = null;
    /// <summary>
    /// Bottom camera plane where background image is placed
    /// </summary>
    public GameObject bottomCameraBackground = null;
    /// <summary>
    /// How frequently is image modified
    /// </summary>
    public int frequencyOfImageBackground = 10;
    /// <summary>
    /// Number of total different image to display (can loop)
    /// </summary>
    public int numOfImageToDisplay = 5;
}

/// <summary>
/// Manager of background image
/// Loads images from Res/backgroundimages/
/// </summary>
public class BackgroundImageManager : MonoBehaviour
{
    /// <summary>
    /// The number of pictures
    /// </summary>
    private int numberOfPictures;
    /// <summary>
    /// The front camera background
    /// </summary>
    private GameObject frontCameraBackground = null;
    /// <summary>
    /// The bottom camera background
    /// </summary>
    private GameObject bottomCameraBackground = null;
    /// <summary>
    /// The frequency of image background
    /// </summary>
    private int frequencyOfImageBackground = 10;

    /// <summary>
    /// The number of image to display
    /// </summary>
    private int numOfImageToDisplay = 0;
    /// <summary>
    /// The number of displayed image
    /// </summary>
    private int numOfDisplayedImage = 0;

    /// <summary>
    /// The activated camera type
    /// </summary>
    private CameraType activatedCameraType;

    /// <summary>
    /// The file names
    /// </summary>
    private string[] fileNames;

    /// <summary>
    /// The is background available
    /// </summary>
    private bool isBackgroundAvailable = false;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake()
    {
        RobotAgent.Instance.OnDataBackgroundUpdate += UpdateData;    
    }

    //Setting up background images
    /// <summary>
    /// Starts this instance.
    /// </summary>
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

    /// <summary>
    /// Sets background image enable property
    /// </summary>
    /// <param name="enable">if set to <c>true</c> [enable].</param>
    public void EnableBackgroundImage(bool enable)
    {
        Utils.ActivateEnvironmentMeshRenderer(!enable);
        frontCameraBackground.SetActive(enable);
        bottomCameraBackground.SetActive(enable);
    }

    /// <summary>
    /// Sets new background
    /// Called 1 times per update
    /// </summary>
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

    //Set new background image on gameobject
    /// <summary>
    /// Sets the new material.
    /// </summary>
    /// <param name="background">The background.</param>
    /// <param name="imageNum">The image number.</param>
    void SetNewMaterial(GameObject background, int imageNum)
    {
        Material material = new Material(Shader.Find("Standard"));
        Texture2D backgroundImage = Resources.Load("backgroundImages/" + fileNames[imageNum], typeof(Texture2D)) as Texture2D;
        material.SetTexture("_MainTex", backgroundImage);
        background.GetComponent<MeshRenderer>().material = material;
    }

    /// <summary>
    /// Changes the image.
    /// </summary>
    void ChangeImage()
    {
        numOfImageToDisplay++;
        if (numOfImageToDisplay > GetNumOfImages())
            numOfImageToDisplay = 1;
    }

    /// <summary>
    /// Update background information data,
    /// This method is called from agent
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void UpdateData(BackgroundSettings settings)
    {
        this.bottomCameraBackground = settings.bottomCameraBackground;
        this.frontCameraBackground = settings.frontCameraBackground;
        this.numberOfPictures = settings.numOfImageToDisplay;
        this.frequencyOfImageBackground = settings.frequencyOfImageBackground;
    }

    /// <summary>
    /// Update target settings
    /// called from agent
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void UpdateData(TargetSettings settings)
    {
        this.activatedCameraType = settings.cameraType;
    }

    /// <summary>
    /// Returns number of different images in folder
    /// </summary>
    /// <returns>num of images in folder</returns>
    public int GetNumOfImages() { return fileNames.Length; }
}
