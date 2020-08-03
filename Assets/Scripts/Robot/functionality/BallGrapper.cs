using UnityEngine;


namespace Robot.Functionality
{
    public class BallGrapper : MonoBehaviour
    {

        [SerializeField]
        private LayerMask mask=0;
        [SerializeField]
        private Vector3 offSet=Vector3.zero;
        [SerializeField]
        private float range = 0.5f;
        private StateGrapper state;
        [SerializeField]
        private GameObject ball;

        private void Start()
        {
            state = StateGrapper.CANT_HOLD;
        }

        public void Grab()
        {
            var actualState = GetState();

            if (actualState == StateGrapper.IS_HOLDING)
            {
                actualState = StateGrapper.CANT_HOLD;
            }

            if (actualState == StateGrapper.CAN_HOLD)
            {
                actualState = StateGrapper.IS_HOLDING;
            }

            if (actualState == StateGrapper.IS_HOLDING)
            {
                var degree = Mathf.Deg2Rad * transform.parent.rotation.eulerAngles.y;
                Vector3 pos = new Vector3(Mathf.Sin(degree) * offSet.x, offSet.y, Mathf.Cos(degree) * offSet.z) + transform.parent.position;
                ball.transform.position = pos;
            }
        }

        public StateGrapper GetState() 
        {
            Logic();
            return state;
        }

        private void Logic() 
        {
            
            if (state != StateGrapper.IS_HOLDING)
            {

                Collider[] coll = Physics.OverlapSphere(transform.position, range, mask);

                float distance = range;
                foreach (Collider i in coll)
                {
                    if (Vector3.Distance(i.gameObject.transform.position, transform.position) < distance)
                    {
                        distance = Vector3.Distance(i.gameObject.transform.position, transform.position);
                        ball = i.gameObject;
                    }

                }


                if (distance < range)
                {
                    state = StateGrapper.CAN_HOLD;
                }
                else
                {
                    state = StateGrapper.CANT_HOLD;
                }
            }
        }
    }
}