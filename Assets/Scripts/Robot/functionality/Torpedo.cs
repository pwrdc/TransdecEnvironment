using UnityEngine;


namespace Robot.Functionality
{
    public class Torpedo : MonoBehaviour
    {
        [SerializeField]
        private float range = 10f;
        [SerializeField]
        private Transform fpsPosition;
        [SerializeField]
        private LayerMask mask;

        private bool _isHit = false;

        public bool IsHit() { return _isHit; }

        public void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsPosition.transform.position, fpsPosition.transform.forward, out hit, range, mask))
            {
                _isHit = true;
            }
            _isHit = false;
        }
    }
}

