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
    public bool setObjectAlwaysVisible = true; //when adding noise, make sure that noise won't override object
    public bool addCompetitionObjectsAsNoise = true;
    private List<GameObject> otherObjs = new List<GameObject>();
    private List<MeshRenderer[]> otherObjsMesh = new List<MeshRenderer[]>();

    private float radiusOfGeneratedObject = 0;
    private Vector3 positionOfGeneratedObject = new Vector3();

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

    Vector2 calculateEquationOf2DLine(Vector2 pos1, Vector2 pos2) {
        float a = (pos1.y - pos2.y) / (pos1.x - pos2.x);
        float b = pos1.y - pos1.x * a;
        return new Vector2(a, b);
    }

    Vector2[] calculateEquationOf3DLine(Vector3 pos1, Vector3 pos2) {
        Vector2 xyLine = calculateEquationOf2DLine(new Vector2(pos1.x, pos1.y), new Vector2(pos2.x, pos2.y));
        Vector2 xzLine = calculateEquationOf2DLine(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        
        return new Vector2[] {xyLine, xzLine};
    }

    bool isPointInObject(Vector3 point, Vector2[] lineEquationMin, Vector2[] lineEquationMax) {
        if(lineEquationMin[0].x * point.x + lineEquationMin[0].y < point.y &&
            lineEquationMin[1].x * point.x + lineEquationMin[1].y < point.z &&
            lineEquationMax[0].x * point.x + lineEquationMax[0].y > point.y &&
            lineEquationMax[1].x * point.x + lineEquationMax[1].y > point.z) {
            return true;
        }
        return false;
    }

    GameObject getRaycastHit() {
        int layerMask = (1 << 9) | (1 << 11);
        Debug.Log(layerMask);
        float dist = Vector3.Distance(toAnnotate.transform.position, agent.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, toAnnotate.transform.position - agent.transform.position, out hit, dist, layerMask))
            return hit.transform.gameObject;
        return null;
    }

    bool isOverridingObject(GameObject obj) {
        //Return true if object is overriding target, otherwise return false
        Bounds objBounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(obj);

        Vector3[] boxCoordOfTarget = GetBoxCoord(targetBounds);        
        Vector3[] boxCoordOfObject = GetBoxCoord(objBounds);

        Vector3 positionOfObject = obj.transform.position;
        //first equation is XY second is XZ
        Vector2[] lineEquationMin = calculateEquationOf3DLine(boxCoordOfTarget[0], agent.transform.position); 
        Vector2[] lineEquationMax = calculateEquationOf3DLine(boxCoordOfTarget[1], agent.transform.position);

        //if object is 
        if(isPointInObject(boxCoordOfObject[0], lineEquationMin, lineEquationMax) ||
            isPointInObject(boxCoordOfObject[1], lineEquationMin, lineEquationMax) ||
            isPointInObject(positionOfObject, lineEquationMin, lineEquationMax)) {
            return true;
        }
        return false;
    }

    float GetRandom(float min, float max)
    {
        System.Random rnd = transform.gameObject.GetComponent<RandomInit>().GetRandomizer();
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }   

    Vector3[] GetBoxCoord(Bounds targetBounds) {      
        Vector3[] boxCoord = new Vector3[2];
        Vector3[] pts = new Vector3[8];

        pts[0] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[1] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        pts[2] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[3] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        pts[4] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[5] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        pts[6] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[7] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        
        Vector3 min = pts[0];
        Vector3 max = pts[0];

        for (int i = 1; i < pts.Length; i++)
        {
            min = Vector3.Min(min, pts[i]);
            max = Vector3.Max(max, pts[i]);
        }

        boxCoord[0] = new Vector3(min.x, min.y, min.z);
        boxCoord[1] = new Vector3(max.x, max.y, max.z);

        return boxCoord;
    }

    public void GetNoise() {
        otherObjs = new List<GameObject>();
        otherObjsMesh = new List<MeshRenderer[]>();
    	foreach (Transform child in noiseFolder.transform) {
    		otherObjs.Add(child.gameObject);
            otherObjsMesh.Add(child.gameObject.GetComponentsInChildren<MeshRenderer>());
        }

        if(addCompetitionObjectsAsNoise) {
            foreach(Transform child in this.transform) {
                if(child.gameObject != agent && child.gameObject != target && child.gameObject.tag != target.tag) {
                    otherObjs.Add(child.gameObject);
                    otherObjsMesh.Add(child.gameObject.GetComponentsInChildren<MeshRenderer>());
                }
            }
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
        else {
            r = GetRandom(options[enabledOption].minRadius, options[enabledOption].maxRadius);
        }


        float theta = GetRandom(options[enabledOption].minPhi, options[enabledOption].maxPhi) - 90;
        theta = theta * 3.14f / 180; 

        newPos.x += r * Mathf.Cos(theta);        
        newPos.z += r * Mathf.Sin(theta);

        radiusOfGeneratedObject = r;
        positionOfGeneratedObject = newPos;

        agent.transform.position = newPos;
        if(options[enabledOption].mode.Equals(RobotAcademy.DataCollection.bottomCamera)) {
            agent.transform.eulerAngles = new Vector3(0, GetRandom(0, 360), 0);
        }
        else {
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

        if(setObjectAlwaysVisible) {
            if(isOverridingObject(obj)) {                
                r = GetRandom(radiusOfGeneratedObject, options[enabledOption].othMaxRadius);
                newPos.x += r * Mathf.Cos(theta);
                newPos.y = GetRandom(radiusOfGeneratedObject, options[enabledOption].waterLevel - 0.2f);
                newPos.z += r * Mathf.Sin(theta);
            }  
        }

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

            if(setObjectAlwaysVisible) {
                GameObject obj = getRaycastHit();
                while(obj != null && obj != target && obj != toAnnotate) {
                    obj.SetActive(false);
                    obj = getRaycastHit();
                }
            }
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


    public void ChangeObjectColor(GameObject obj, int maxHueChange, int maxSaturationChange, int maxValueChange) {
        MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer mesh in meshRenderers) {
            Material[] materials = mesh.materials;
            foreach(Material material in materials) {
                Color color = material.color;

                
            }
        }
    }
}
