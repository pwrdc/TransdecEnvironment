using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    [System.Serializable]
    public class ObjectCreator
    {
        [SerializeField]
        public List<CameraType> targetCameraModes = new List<CameraType>();
        [SerializeField]
        public List<ObjectType> targetObjectTypes = new List<ObjectType>();
        [SerializeField]
        public List<GameObject> targetObjects = new List<GameObject>();
        [SerializeField]
        public List<GameObject> targetAnnotations = new List<GameObject>();
        [SerializeField]
        public List<bool> targetIsEnabled = new List<bool>();
    }
}