using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// After setting target name using ChangeTarget
/// the class will find a target closed to the agent with this name
/// and set it as FocusedObject in the academy.
/// </summary>
public class TargetSelector : MonoBehaviour
{
    List<Target> targetsWithName;

    public void ChangeTarget(string targetName)
    {
        targetsWithName = Targets.FindTargetsByName(targetName);
        if(targetsWithName.Count==0)
            Debug.LogWarning($"The target named \"{targetName}\" doesn't exist in current targets folder.");
    }
    
    public void SwitchToClosestTarget()
    {
        if (targetsWithName == null)
            throw new InvalidOperationException("Call to SwitchToClosestTarget before setting the target name using ChangeTarget in TargetsSelector");
        Target closestTarget = Utils.FindClosest(targetsWithName, transform.position);
        if(closestTarget!=null)
            RobotAcademy.Instance.SetResetParameter("FocusedObject", closestTarget.index);
    }
}
