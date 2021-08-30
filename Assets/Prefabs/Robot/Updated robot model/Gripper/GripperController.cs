using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripperController : MonoBehaviour
{

    public Rigidbody steeringPoint;
    [Header("Hinges")]
    public List<HingeJoint> hinges = new List<HingeJoint>();
    public float motorSpeed;
    public float motorForce;
    public float deafultLimit_min;
    public float deafultLimit_max;
    private JointLimits deafultLim = new JointLimits();

    private Dictionary<HingeJoint, Rigidbody> rbs = new Dictionary<HingeJoint, Rigidbody>();
    private JointMotor closing = new JointMotor();
    private JointMotor opening = new JointMotor();


    [Header("Brakes")]
    public int brakeDelay;
    private bool activeBrakes;


    private int idleTicks;
    private bool isClosing;


    void Start()
    {

        //Set up joints and joint limits
        deafultLim.min = deafultLimit_min;
        deafultLim.max = deafultLimit_max;
        foreach(HingeJoint joint in hinges)
        {
            joint.useSpring = false;
            joint.useLimits = true;
            joint.limits = deafultLim;
        }


        //Set up motors
        closing.targetVelocity = -motorSpeed;
        closing.force = motorForce;
        opening.targetVelocity = motorSpeed;
        opening.force = motorForce;
        foreach (HingeJoint joint in hinges)
        {
            joint.useMotor = true;
            joint.motor = opening;
            isClosing = false;
        }


        //Set up brakes
        foreach (HingeJoint joint in hinges)
        {
            rbs.Add(joint, joint.gameObject.GetComponent<Rigidbody>());
        }
        idleTicks = 0;
        activeBrakes = false;

    }


    private void Update()
    {

        //Process input
        if (Input.GetMouseButtonDown(0))
            foreach (HingeJoint joint in hinges)
            {
                joint.motor = closing;
                isClosing = true;
                idleTicks = 0;
                activeBrakes = false;
                EvaluateBrakes(activeBrakes);
            }
        if (Input.GetMouseButtonDown(1))
            foreach (HingeJoint joint in hinges)
            {
                joint.motor = opening;
                isClosing = false;
                idleTicks = 0;
                activeBrakes = false;
                EvaluateBrakes(activeBrakes);
            }


    }


    void FixedUpdate()
    {
    

        //Check brakes
        foreach (HingeJoint joint in hinges)
        {
            if (rbs.TryGetValue(joint, out Rigidbody rb))
            {
                if ( (transform.InverseTransformVector((rb.angularVelocity - steeringPoint.angularVelocity)).sqrMagnitude < 0.01f))
                {
                    idleTicks++;
                }
            }
        }

        if ((idleTicks > brakeDelay * 3) && !activeBrakes)
            EvaluateBrakes(true);

    }


    void EvaluateBrakes(bool brakesActive)
    {


        if (brakesActive)
        {
            //Check for gripped object
            Debug.DrawLine(transform.position, transform.position + 0.5f * transform.TransformDirection(Vector3.down), Color.green, 0.1f);
            if (isClosing && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 0.5f, 1))
            {
                Debug.Log("Brake engaged");
                //Drastically limit hinge joints' movement
                foreach (HingeJoint joint in hinges)
                {
                    var lim = new JointLimits();
                    lim.min = joint.transform.localEulerAngles.z - 360f + 88f;
                    lim.max = joint.transform.localEulerAngles.z - 360f + 90f;
                    joint.limits = lim;
                    activeBrakes = true;
                    StartCoroutine(CheckGrip());
                }
            }
        }
        else
            foreach (HingeJoint joint in hinges) { joint.limits = deafultLim; }

    }




    IEnumerator CheckGrip()
    {
        while(activeBrakes)
        {
            if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 0.5f, 1))
                break;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

}
