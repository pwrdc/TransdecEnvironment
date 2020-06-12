using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesGenerator : MonoBehaviour
{
    public Placeable robot;
    public Transform targetsFolder;
    public Transform noiseFolder;
    Placeable[] targets;
    Placeable[] noise;
    Placer placer;
    
    public FloatRange cameraRange=new FloatRange(3, 10);
    public Vector3 cameraRotationRange = new Vector3(20, 20, 20);

    bool targetAlwaysVisible;

    // suppress variable is never assigned warning
    #pragma warning disable 0649
    [ResetParameter] bool enableNoise;
    [ResetParameter] bool collectData;
    [ResetParameter] int focusedObject;
    [ResetParameter] bool setFocusedObjectInCenter;
    [ResetParameter("Positive")] bool positiveExamples;
    [ResetParameter] CameraType focusedCamera;
    #pragma warning restore 0649


    void Start()
    {
        ResetParameterAttribute.InitializeAll(this);
        placer = GetComponent<Placer>();
        RobotAgent.Instance.OnReset.AddListener(OnReset);
        RobotAgent.Instance.OnDataCollection.AddListener(OnDataCollection);
        targets = targetsFolder.GetComponentsInChildren<Placeable>();
        noise = noiseFolder.GetComponentsInChildren<Placeable>();
    }

    void RandomizeRobotRotation(Placeable target)
    {
        // the closer the camera is to the object the more random rotation is allowed
        float distance = Vector3.Distance(target.transform.position, robot.transform.position);
        float rangeMultiplier = 1 - cameraRange.ReverseLerp(distance);
        Vector3 multipliedRange = cameraRotationRange * rangeMultiplier;
        Quaternion rotation =
            Quaternion.Euler(
                Random.Range(-multipliedRange.x, multipliedRange.x),
                Random.Range(-multipliedRange.y, multipliedRange.y),
                Random.Range(-multipliedRange.z, multipliedRange.z));
        robot.transform.rotation*= rotation;
    }

    void RotateRobot(Placeable targetPlaceable, Target target)
    {
        Quaternion rotation=Quaternion.LookRotation(targetPlaceable.transform.position - robot.transform.position);
        if(RobotAgent.Instance.focusedCamera == CameraType.bottomCamera || target.cameraType==CameraType.bottomCamera)
        {
            // rotate the robot 90 degrees up
            rotation *= Quaternion.Euler(-90, 0, 0);
        }
        robot.transform.rotation = rotation;
        if (!setFocusedObjectInCenter)
            RandomizeRobotRotation(targetPlaceable);
    }

    private void OnDataCollection()
    {
        if (RobotAgent.Instance.agentSettings.randomizeTargetObjectPositionOnEachStep)
            OnReset();
    }

    private class Bounds
    {
        Vector3 min;
        Vector3 max;

        public Bounds() { }

        public void ExpandToContain(Vector3 vector)
        {
            if (vector.x < min.x) min.x = vector.x;
            if (vector.y < min.y) min.y = vector.y;
            if (vector.z < min.z) min.z = vector.z;
            if (vector.x > max.x) max.x = vector.x;
            if (vector.y > max.y) max.y = vector.y;
            if (vector.z > max.z) max.z = vector.z;
        }

        public bool Contains(Vector3 vector)
        {
            return
                vector.x >= min.x
             && vector.y >= min.y
             && vector.z >= min.z
             && vector.x <= max.x
             && vector.y <= max.y
             && vector.z <= max.z;
        }
    }

    bool ObscuresView(Placeable placeable, Vector3 target)
    {
        if (!placeable.canObscureView || !targetAlwaysVisible)
            return false;
        Vector3 camera = RobotAgent.Instance.transform.position;

        Vector3 placeablePosition = placeable.transform.position;

        // create the smallest cuboid containing camera and target and check if placeable lies within
        Bounds bounds = new Bounds();
        bounds.ExpandToContain(camera);
        bounds.ExpandToContain(target);
        if (!bounds.Contains(placeablePosition))
            return false;

        // distance(P, l)=|AP x u|/|u| where A is a point on the line and u is line's direction 
        Vector3 AP = placeablePosition - target;
        Vector3 u = target - camera;
        float distance = Vector3.Cross(AP, u).magnitude / u.magnitude;

        return distance < placeable.radius;
    }

    // calls action until it returns true or it is called tries times
    // returns true if action succeded
    bool Try(int tries, System.Func<bool> action)
    {
        while (tries > 0)
        {
            if (action()) return true;
            else tries--;
        }
        return false;
    }

    void GenerateForDataCollection()
    {
        placer.Clear();
        Placeable target = targets[focusedObject];
        foreach (var otherTarget in targets)
        {
            if(otherTarget!=target)
                otherTarget.gameObject.SetActive(false);
        }
        // try putting target 10 times and every time target is placed try placing camera near it 10 times
        if (Try(10, () =>
            placer.Place(target)
            && Try(10, () => placer.PlaceNear(robot, target, cameraRange))
            ))
        {
            // successfully placed both of them
            RotateRobot(target, Target.AtIndex(focusedObject));
            if (enableNoise)
                placer.PlaceAll(noise, (Placeable placeable)=>ObscuresView(placeable, target.transform.position));
            if (collectData && !positiveExamples)
                target.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Despite trying 100 times, placing robot and target failed, perhaps the placing area or scenes generator is not set up incorrectly.");
        }
    }

    void GenerateForFreeMovement()
    {
        placer.Clear();
        placer.PlaceAll(targets);
        placer.Place(robot);
        if(enableNoise)
            placer.PlaceAll(noise);
    }

    void OnReset()
    {
        if (collectData)
        {
            GenerateForDataCollection();
        } else
        {
            GenerateForFreeMovement();
        }
    }
}
