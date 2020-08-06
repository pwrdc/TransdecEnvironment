using UnityEngine;

public class BallGrapper : MonoBehaviour
{
    public LayerMask mask=0;
    public Vector3 offSet=Vector3.zero;
    public float range = 0.5f;
    public enum State
    {
        Holding, ObjectInReach, NoObjectsInReach
    }
    State state= State.NoObjectsInReach;
    Transform objectInReach;
    public float holdingTimeot = 0.5f;
    bool grabHeld = false;

    public void Grab()
    {
        grabHeld = true;
        UpdateReachingState();
        if (state == State.NoObjectsInReach)
        {
            EventsLogger.Log("Attempted to grab something.");
        }
        if (state == State.ObjectInReach)
        {
            state = State.Holding;
            EventsLogger.Log("Started holding an object.");
        }
    }

    public State GetState() 
    {
        UpdateReachingState();
        return state;
    }

    private void Update()
    {
        if (state == State.Holding)
        {
            if (grabHeld)
                objectInReach.position = Vector3.Lerp(objectInReach.position, transform.position + transform.TransformDirection(offSet), 0.5f);
            else
            {
                state = State.NoObjectsInReach;
                EventsLogger.Log("Stopped holding an object.");
            }
            grabHeld = false;
        }
    }

    // Finds closest object in range
    Transform FindObjectToHold()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, mask);
        float closestDistance = range;
        Collider closest = null;
        foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(collider.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closest = collider;
                closestDistance = distance;
            }
        }
        return closest?.transform;
    }

    private void UpdateReachingState() 
    {
        if (state == State.Holding)
            return;
        objectInReach = FindObjectToHold();
        if(objectInReach!=null)
            state = State.ObjectInReach;
        else
            state = State.NoObjectsInReach;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}