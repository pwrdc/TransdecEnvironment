using UnityEngine;
using System;

namespace Robot.Functionality
{
    public class TorpedoLauncher : MonoBehaviour
    {
        public LayerMask mask = 0;

        public Torpedo torpedo;
        public Vector3 offset;
        public float rate=0.5f;

        float nextShoot = 0f;

        public bool lastTorpedoHit { get; private set; } = false;
        public bool ready => Time.time>=nextShoot;

        public void Shoot()
        {
            if (ready)
            {
                EventsLogger.Log("Torpedo shot.");
                InitializeTorpedo(Instantiate(torpedo, transform.TransformPoint(offset), transform.rotation* torpedo.transform.rotation));
                lastTorpedoHit = false;
                nextShoot = Time.time + rate;
            }
        }

        void InitializeTorpedo(Torpedo torpedo)
        {
            torpedo.OnHit = TorpedoHit;
            torpedo.mask = mask;
        }

        // this method is called by torpedos when they hit a torpedo target
        public void TorpedoHit()
        {
            EventsLogger.Log("Torpedo hit an object marked as torpedo target.");
            lastTorpedoHit = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(offset), 0.1f);
        }
    }
}