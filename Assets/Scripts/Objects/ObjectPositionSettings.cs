using  UnityEngine;
using System.Collections.Generic;

public class ObjectPositionSettings : MonoBehaviour
{
    public GameObject obj => gameObject;
    public Vector3 startPosition = new Vector3(0f, 0f, 0f);
    [Range(0f, 10f)]
    public float xPosRange = 0f;
    [Range(-10f, 0f)]
    public float minusXPosRange = 0f;
    [Range(0f, 10f)]
    public float yPosRange = 0f;
    [Range(-10f, 0f)]
    public float minusYPosRange = 0;
    [Range(0f, 10f)]
    public float zPosRange = 0f;
    [Range(-10f, 0f)]
    public float minusZPosRange = -0f;
    public Vector3 startRotation = new Vector3(0f, 0f, 0f);
    [Range(0f, 30f)]
    public float xAngRange = 0f;
    [Range(0f, 30f)]
    public float yAngRange = 0f;
    [Range(0f, 30f)]
    public float zAngRange = 0f;
    [Range(0f, 360f)]
    public List<float> allowedRotations = new List<float>();

    void Start(){
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }
}