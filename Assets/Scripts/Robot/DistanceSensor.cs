using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class DistanceSensor : MonoBehaviour
    {
        public float offset = 0.0f;
        public Vector3 direction = Vector3.forward;

        Vector3 TransformedDirection => transform.TransformDirection(direction);
        Vector3 RayStart => transform.position+ TransformedDirection*offset;

        public float GetDistance()
        {
            RaycastHit hit;
            if (Physics.Raycast(RayStart, TransformedDirection, out hit, Mathf.Infinity))
            {
                return hit.distance;
            }
            return float.PositiveInfinity;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(RayStart, TransformedDirection * GetDistance());
        }
    }
}