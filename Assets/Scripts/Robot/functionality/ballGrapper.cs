using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Robot.functionality;


namespace Robot.functionality
{
    public class BallGrapper : MonoBehaviour
    {

        [SerializeField]
        private LayerMask mask;
        [SerializeField]
        private Vector3 offSet;
        [SerializeField]
        private float range = 0.5f;
        private State state;
        private GameObject ball;

        private void Start()
        {
            state = State.CANT_HOLD;
        }

        private void FixedUpdate()
        {
            if (state != State.IS_HOLDING)
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
                    state = State.CAN_HOLD;
                }
                else
                {
                    state = State.CANT_HOLD;
                }
            }


            if (Input.GetKeyDown(KeyCode.Tab))
            {

                if (state == State.IS_HOLDING)
                {
                    state = State.CANT_HOLD;
                }

                if (state == State.CAN_HOLD)
                {
                    state = State.IS_HOLDING;
                }
            }

            if (state == State.IS_HOLDING)
            {
                ball.transform.position = offSet + transform.position;
            }
        }
    }
}