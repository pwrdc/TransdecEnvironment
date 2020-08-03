using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to read object placing related options from Academy
/// and generate scenes according to them using Placer.
/// </summary>
[RequireComponent(typeof(Placer))]
public class ScenesGenerator : MonoBehaviour
{
    public Placeable robot;
    public Transform targetsFolder;
    public Transform noiseFolder;
    public VisibilityChecker visibilityChecker;
    Placeable[] targets;
    Placeable[] noise;
    Placer placer;
    
    public FloatRange cameraRange=new FloatRange(3, 10);
    public Vector3 cameraRotationRange = new Vector3(20, 20, 20);

    public bool targetAlwaysVisible;

    [ResetParameter] bool enableNoise=false;
    [ResetParameter] bool collectData=false;
    [ResetParameter] int focusedObject=0;
    [ResetParameter] bool setFocusedObjectInCenter=false;
    [ResetParameter("Positive")] bool positiveExamples=false;
    [ResetParameter] CameraType focusedCamera=CameraType.frontCamera;


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
        if(focusedCamera == CameraType.bottomCamera || target.cameraType==CameraType.bottomCamera)
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

    void GenerateForDataCollection()
    {
        placer.Clear();
        // disable all targets that aren't focused
        Placeable target = targets[focusedObject];
        foreach (var otherTarget in targets)
        {
            if(otherTarget!=target)
                otherTarget.gameObject.SetActive(false);
        }
        // try putting target 10 times and every time target is placed try placing camera near it 10 times
        if (Utils.Try(10, () =>
            placer.Place(target)
            && Utils.Try(10, () => placer.PlaceNear(robot, target, cameraRange))
            ))
        {
            // successfully placed both of them
            RotateRobot(target, Targets.Focused);
            if (enableNoise)
            {
                if (targetAlwaysVisible)
                    placer.PlaceAll(noise, (Placeable placeable) => visibilityChecker.ObscuresView(placeable, target));
                else
                    placer.PlaceAll(noise);
            }
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
