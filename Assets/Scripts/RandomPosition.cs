using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;
    public GameObject toAnnotate = null;
    public GameObject noiseFolder = null;
    public int numberOfNoiseToGenerate = 5;
    public bool setFocusedObjectInCenter = false;
    private List<GameObject> otherObjs = new List<GameObject>();

    [SerializeField]
    public List<Settings> options = new List<Settings>();

    [SerializeField]
    public int enabledOption;

    [System.Serializable]
    public class Settings {
        public RobotAcademy.DataCollection mode;
        public float minPhi = -180.0f;
        public float maxPhi = 180.0f;
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        [Range(30.0f, 90.0f)]
        public float cameraFov = 40.0f;
        public float waterLevel = 11.0f;
        [Range(0.0f, 11.0f)]
        public float maxDepth = 4.0f;
        [Range(0.0f, 30.0f)]
        public float xAngRange = 15f;
        [Range(0.0f, 30.0f)]
        public float yAngRange = 15f;
        [Range(0.0f, 30.0f)]
        public float zAngRange = 15f;
        //other objects parameters
        [Range(0.0f, 10.0f)]
        public float othMinRadius = 0.0f;
        [Range(0.0f, 10.0f)]
        public float othMaxRadius = 4.0f;
        [Range(0.0f, 11.0f)]
        public float othMaxDepth = 8.0f;
        [Range(0.0f, 90.0f)]
        public float othXAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othYAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othZAngRange = 90f;

        public Settings(RobotAcademy.DataCollection mode) { this.mode = mode; }
    }

    [HideInInspector]
    public Bounds targetBounds;

    float GetRandom(float min, float max)
    {
        System.Random rnd = transform.gameObject.GetComponent<RandomInit>().GetRandomizer();
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    public void Start() {
    	foreach (Transform child in noiseFolder.transform) {
    		otherObjs.Add(child.gameObject);
        }
    }

    public void GetNewPos()
    {
        Vector3 newPos = targetBounds.center;
        float xRot = GetRandom(-options[enabledOption].xAngRange, options[enabledOption].xAngRange);
        float yRot = GetRandom(-options[enabledOption].yAngRange, options[enabledOption].yAngRange);
        float zRot = GetRandom(-options[enabledOption].zAngRange, options[enabledOption].zAngRange);
        float r;

        newPos.y = GetRandom(options[enabledOption].maxDepth, options[enabledOption].waterLevel);


        if(options[enabledOption].mode.Equals(RobotAcademy.DataCollection.bottomCamera)) {
        	if(setFocusedObjectInCenter)
        		r = 0;
        	else
            	r = GetRandom(0, ((newPos.y - targetBounds.center.y) * (options[enabledOption].cameraFov/100)) );
        }
        else
            r = GetRandom(options[enabledOption].minRadius, options[enabledOption].maxRadius);

        float theta = GetRandom(options[enabledOption].minPhi, options[enabledOption].maxPhi) - 90;
        theta = theta * 3.14f / 180; 

        newPos.x += r * Mathf.Cos(theta);        
        newPos.z += r * Mathf.Sin(theta);

        agent.transform.position = newPos;
        if(options[enabledOption].mode.Equals(RobotAcademy.DataCollection.bottomCamera)) {
            agent.transform.eulerAngles = new Vector3(0, GetRandom(0, 360), 0);
        }
        else {
        	Debug.Log(setFocusedObjectInCenter);
            agent.transform.LookAt(transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(toAnnotate).center);
            if(!setFocusedObjectInCenter)
            	agent.transform.eulerAngles = new Vector3(agent.transform.eulerAngles.x + xRot, agent.transform.eulerAngles.y + yRot, agent.transform.eulerAngles.z + zRot);
        }
    }

    public void GetOthNewPos(GameObject obj)
    {
        Vector3 newPos = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(toAnnotate).center;

        float xRot = GetRandom(-options[enabledOption].othXAngRange, options[enabledOption].othXAngRange);
        float yRot = GetRandom(-options[enabledOption].othYAngRange, options[enabledOption].othYAngRange);
        float zRot = GetRandom(-options[enabledOption].othZAngRange, options[enabledOption].othZAngRange);
        float r = GetRandom(options[enabledOption].othMinRadius, options[enabledOption].othMaxRadius);
        float theta = GetRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = GetRandom(options[enabledOption].othMaxDepth, options[enabledOption].waterLevel - 0.2f);
        newPos.z += r * Mathf.Sin(theta);
        obj.transform.position = newPos;
        obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
    }

    public List<GameObject> GetRandomObjects(List<GameObject> objects, int amount) {
    	int rand;
    	List<GameObject> objToChose = new List<GameObject>();
    	foreach(GameObject obj in objects) objToChose.Add(obj);
    	while(objToChose.Count > amount) {
    		rand = (int)(GetRandom(0, objToChose.Count));
    		objToChose.RemoveAt(rand);
		}
		return objToChose;
    }

    public void DrawPositions(bool addNoise, bool randomQuarter, bool randomPosition) {
        
        if(options[enabledOption].mode.Equals(RobotAcademy.DataCollection.bottomCamera))
            transform.GetComponent<RandomInitOnMesh>().PutAll(target);
        else 
            transform.GetComponent<RandomInit>().PutAll(randomQuarter, randomPosition, false);

        targetBounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(toAnnotate);

        GetNewPos();
        if (addNoise)
        {
        	List<GameObject> objToChose = GetRandomObjects(otherObjs, numberOfNoiseToGenerate);
            foreach (GameObject obj in objToChose) GetOthNewPos(obj);
            foreach (GameObject obj in otherObjs) obj.SetActive(false);
            foreach (GameObject obj in objToChose) obj.SetActive(true);
        }
        
    }

    public void DeleteObject(int index) {
    	options.RemoveAt(index);
    }

    public void SetCameraMode(int index, RobotAcademy.DataCollection mode) {
    	options[index].mode = mode;
    }

    public void SetObjectMode(int index, RobotAcademy.ObjectType type) {
        if(type == RobotAcademy.ObjectType.Big) {
            options[index].minRadius = 3;
            options[index].maxRadius = 8;
            options[index].waterLevel = 11;
            options[index].maxDepth = 4;
        }
        else if(type == RobotAcademy.ObjectType.Small) {
            options[index].minRadius = 1.5f;
            options[index].maxRadius = 3;
            options[index].waterLevel = 9;
            options[index].maxDepth = 7.5f;
        }
        else if(type == RobotAcademy.ObjectType.OnBottom) {
            options[index].minRadius = 1.5f;
            options[index].maxRadius = 4;
            options[index].waterLevel = 11;
            options[index].maxDepth = 9f;
        }
    }

    public void AddNewObject(RobotAcademy.DataCollection mode, RobotAcademy.ObjectType type) {
    	options.Add(new Settings(mode));     
        SetObjectMode(options.Count - 1, type);       
    }

    public void InsertNewObject(int index, RobotAcademy.DataCollection mode, RobotAcademy.ObjectType type) {
        options.Insert(index, new Settings(mode));     
        SetObjectMode(index, type);       
    }

    public void ActivateOption(int index) {
    	enabledOption = index;
    }
}
