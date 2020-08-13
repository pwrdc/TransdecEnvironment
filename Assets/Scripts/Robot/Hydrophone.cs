using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydrophone : MonoBehaviour
{
    List<Pinger> pingersOnFrequency;

    public void SetFrequency(float frequency)
    {
        pingersOnFrequency = Pinger.FindPingers(frequency);
    }
    
    public float GetAngle()
    {
        Transform pinger = null;
        if(pingersOnFrequency!=null)
            pinger=Utils.FindClosest(pingersOnFrequency, transform.position)?.transform;
        if (pinger == null)
            return float.PositiveInfinity;
        Vector3 toTarget = pinger.position - transform.position;
        Vector3 robotDirection = transform.forward;
        toTarget.y = robotDirection.y = 0;// we are only intereseted in the angle on the XZ plane
        return Vector3.Angle(toTarget, robotDirection);
    }
}
