using UnityEngine;


namespace Robot.functionality
{
    public class torped : MonoBehaviour
    {
        [SerializeField]
        private float range = 10f;
        [SerializeField]
        private Transform fpsPosition;

        void FixedUpdate()
        {
            shoot();
        }

        private bool shoot()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit hit;
                if (Physics.Raycast(fpsPosition.transform.position, fpsPosition.transform.forward, out hit, range))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

