using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State for robot 
enum State { 
     IS_HOLDING, CAN_HOLD, CANT_HOLD
}


public class ballGrapper : MonoBehaviour
{

    
    
    public Vector3 offSet;//in what place should the item be carried?
    public float range = 0.5f;//catch range
    private State state;
    public LayerMask mask;//Layer for objects which can be hold
    //if you want object to be hold it must contain collider and layer "ball"


    private GameObject ball;//oject which is the nearest of robot


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


        if (Input.GetKeyDown(KeyCode.Tab)) {

            if (state == State.IS_HOLDING) {
                state = State.CANT_HOLD;
              
            }

            if (state == State.CAN_HOLD) {
                state = State.IS_HOLDING;
             
            }
        }


       


        
    }



    private void Update()
    {
        if (state == State.IS_HOLDING)
        {
            moveObject();
        }
    }




    private void moveObject() {
        ball.transform.position = offSet + transform.position;
    }

    
}
