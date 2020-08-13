using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydrophone : MonoBehaviour
{
    Transform pinger;

    public void SetFrequency(float frequency)
    {
        pinger = Pinger.FindPinger(frequency)?.transform;
    }
    
    public float GetAngle()
    {
        if (pinger == null)
            return float.PositiveInfinity;
        Vector3 toTarget = pinger.position - transform.position;
        Vector3 robotDirection = transform.forward;
        toTarget.y = robotDirection.y = 0;// we are only intereseted in the angle on the XZ plane
        return Vector3.Angle(toTarget, robotDirection);
    }
}
