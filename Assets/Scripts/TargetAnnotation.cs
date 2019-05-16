﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TargetAnnotation : MonoBehaviour
{
    public GameObject target;
    Canvas bck;
    GameObject backgroudObject;
    RawImage randomImage;
    Texture2D planeImage;
    public int numberofpictures;
    public float margin = 10.0f;
    public bool drawBox = false;
    public bool addPlane = false;
    public  Texture2D background;
    public Camera cam = null;

    public bool activate = false;

    private float[] boxCoord = new float[4];
    private GUIStyle style = null;
    private Vector3[] pts = new Vector3[8];

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
            if(addPlane)
            {

                var dist = Vector3.Distance(target.transform.position, cam.transform.position);
                backgroudObject = new GameObject();
                backgroudObject.AddComponent<Canvas>();
                backgroudObject.AddComponent<RawImage>();

                bck = backgroudObject.GetComponent<Canvas>();
                bck.renderMode = RenderMode.ScreenSpaceCamera;
                bck.worldCamera = cam;
                bck.planeDistance = dist+1.5f;
                randomImage = backgroudObject.GetComponent<RawImage>();
                int randomNumber = (int)Random.Range(0, numberofpictures);

                string path = randomNumber.ToString();

                planeImage = Resources.Load<Texture2D>(path);
                randomImage.texture = planeImage;
                
            
                randomImage.uvRect = new Rect(0, 0, 1, 1);
                randomImage.transform.SetParent(bck.transform);


               
            }

        }
        /*WWW www = new WWW("http://gyanendushekhar.com/wp-content/uploads/2017/07/SampleImage.png");
        while (!www.isDone)
            yield return null;
        Debug.Log(www.texture.name);*/
       

    }

    public float[] GetBoundingBox() {
        return boxCoord;
    }
}
