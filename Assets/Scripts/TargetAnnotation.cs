using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TargetAnnotation : MonoBehaviour
{
    public GameObject target;
    public int numberofpictures;
    public float margin = 10.0f;
    public bool drawBox = false;
    public Texture2D background;
    public Camera cam = null;
    public GameObject frontCameraBackground = null;
    public GameObject bottomCameraBackground = null;

    public bool activate = false;
    public bool activateBackground = false;
    public RobotAcademy.DataCollection activatedMode;


    private float[] boxCoord = new float[4];
    private GUIStyle style = null;
    private Vector3[] pts = new Vector3[8];

    private int numOfImageToDisplay = 0;

    private string[] fileNames;

    public void OnGUI()
    {
        if (activate) {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box);
                style.normal.background = background;
            }
            Bounds bounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(target);
            // obiekt jest niewidoczny
            if (cam.WorldToScreenPoint(bounds.center).z < 0) return;
            // 8 wspolrzednych 
            pts[0] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[1] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[2] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[3] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[4] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[5] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[6] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[7] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            // odpowiednie wspolrzedne na ekranie
            for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;
            // maksymalne i minimalne wartosci
            Vector3 min = pts[0];
            Vector3 max = pts[0];
            for (int i = 1; i < pts.Length; i++)
            {
                min = Vector3.Min(min, pts[i]);
                max = Vector3.Max(max, pts[i]);
            }
            boxCoord[0] = min.x;
            boxCoord[1] = min.y;
            boxCoord[2] = max.x;
            boxCoord[3] = max.y;
            // prostokat
            if (drawBox)
            {
                Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
                r.xMin -= margin;
                r.xMax += margin;
                r.yMin -= margin;
                r.yMax += margin;
                // 'box'
                GUI.Box(r, "", style);
            }
            if(activateBackground)
            {
                if(RobotAcademy.DataCollection.frontCamera == activatedMode) {
                    SetNewMaterial(frontCameraBackground, numOfImageToDisplay);
                }
                else if(RobotAcademy.DataCollection.bottomCamera == activatedMode) {
                    SetNewMaterial(bottomCameraBackground, numOfImageToDisplay);
                }
            }

        }
    }

    void SetNewMaterial(GameObject background, int imageNum) {
        Material material = new Material(Shader.Find("Standard"));
        Texture2D backgroundImage = Resources.Load("backgroundImages/"+fileNames[imageNum], typeof(Texture2D)) as Texture2D;
        material.SetTexture("_MainTex", backgroundImage);
        background.GetComponent<MeshRenderer>().material = material;
    }

    void Awake() {
        string path = @"D:\Github\TransdecEnvironment_9\Assets\Resources\backgroundImages";
        string[] files = System.IO.Directory.GetFiles(path, "*.jpg");
        fileNames = new string[files.Length];

        for(int i = 0; i < files.Length; i++) {
            string[] words = files[i].Split('\\');
            string word = words[words.Length - 1];
            fileNames[i] = word.Split('.')[0];
        }
    }

    public int getNumberOfBackgroundImages() {
        return fileNames.Length;
    }

    public void ChangeImageToDisplay(int numberOfImage) {
        numOfImageToDisplay = numberOfImage;
    }

    public float[] GetBoundingBox() {
        return boxCoord;
    }
}