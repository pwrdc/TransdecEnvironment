using UnityEngine;
using NaughtyAttributes;

public class Target : MonoBehaviour
{
    public CameraType cameraType;
    public GameObject annotation;
    [ReadOnly]
    public int index;

    private void Start()
    {
        if (annotation == null)
        {
            annotation = gameObject;
        }
    }
}
