using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Basically we want this thing to rotate 
 * around it's local Y axis towards currently rendering camera.
 * This could be a shader, but it'd require implementing 
 * LookAt in shader language.
 */
 [ExecuteInEditMode]
public class LightBillboard : MonoBehaviour
{
    // rotation as Euler angles, 
    // the actual transform rotation is updated by the script
    public Vector3 rotation;

    Vector3 GetObserver()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            // in edit mode rotate towards editor camera
            return SceneView.lastActiveSceneView.camera.transform.position;
        }
        else
        {
            return Camera.current.transform.position;
        }
    }

    void OnRenderObject()
    {
        Vector3 observer=GetObserver();
        
        Vector3 onPlane = new Vector3(observer.x, transform.position.y, observer.z);
        Vector3 downDirection = Quaternion.Euler(rotation) * Vector3.down;
        transform.LookAt(onPlane, downDirection);
    }
}
