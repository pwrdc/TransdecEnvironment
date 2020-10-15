using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class Accelerometer : MonoBehaviour
    {
        new Rigidbody rigidbody;
        Vector3 lastVelocity;
        public Vector3 acceleration;
        Vector3 lastAngularVelocity;
        public Vector3 angularAcceleration;
        Quaternion startRotation;
        public Vector3 rotation;

        void Start()
        {
            rigidbody = GetComponentInParent<Rigidbody>();
            lastVelocity = transform.InverseTransformDirection(rigidbody.velocity);
            lastAngularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity);
            startRotation = rigidbody.rotation;
        }

        void Update()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
            acceleration = (localVelocity - lastVelocity) / Time.fixedDeltaTime;
            Vector3 localAngularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity);
            angularAcceleration = (localAngularVelocity - lastAngularVelocity) / Time.fixedDeltaTime;
            lastVelocity = localVelocity;
            lastAngularVelocity = localAngularVelocity;
            // source: https://forum.unity.com/threads/get-the-difference-between-two-quaternions-and-add-it-to-another-quaternion.513187/
            // current=start*difference
            // difference=current/start
            // difference=current*(1/start)
            rotation = (rigidbody.rotation * Quaternion.Inverse(startRotation)).eulerAngles.Select(NormalizeRotation);
        }

        // turns an angle from range (0, 360) to (-180, 180)
        float SignedAngleFromZero(float x)
        {
            if (x > 180)
            {
                return -(360 + x);
            }
            else
            {
                return x;
            }
        }

        private float NormalizeRotation(float current)
        {
            float result = current;
            if (result < 0)
                result += 360;
            result = (result + 180) % 360 - 180;
            return result;
        }
    }
}