using UnityEngine;

namespace Robot.Functionality
{
    public class Torpedo : MonoBehaviour
    {
        [SerializeField]
        private float range = 10f;
        [SerializeField]
        private Transform fpsPosition = null;
        [SerializeField]
        private LayerMask mask = 0;

        public bool lastTorpedoHit { get; private set; } = false;

        public void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsPosition.transform.position, fpsPosition.transform.forward, out hit, range, mask))
            {
                EventsLogger.Log("Torpedo hit an object marked as torpedo target.");
                lastTorpedoHit = true;
            }
            else
            {
                EventsLogger.Log("Torpedo missed.");
                lastTorpedoHit = false;
            }
        }
    }
}