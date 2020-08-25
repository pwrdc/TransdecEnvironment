using UnityEngine;

namespace Robot.Functionality
{
    public class Torpedo : MonoBehaviour
    {
        public float force = 100;
        public float timeToDestroyAfterHit=1f;

        [HideInInspector]
        public System.Action OnHit;
        [HideInInspector]
        public LayerMask mask = 0;

        void Start()
        {
            // add initial force to the torpedo
            GetComponent<Rigidbody>().AddRelativeForce(0, force, 0, ForceMode.Impulse);
        }

        void OnCollisionEnter(UnityEngine.Collision collision)
        {
            // check against mask
            if ((mask & 1<<collision.gameObject.layer) != 0)
            {
                // this action is set by the object instantiating torpedos
                OnHit?.Invoke();
            }
            Destroy(gameObject, timeToDestroyAfterHit);
        }
    }
}