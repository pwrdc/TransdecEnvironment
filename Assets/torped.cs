using UnityEngine;

public class torped : MonoBehaviour
{
    public float range = 10f;
    public Transform fpsPos;



    // Update is called once per frame
    void FixedUpdate()
    {
        shoot();
            
    }



    private bool shoot() {
        if (Input.GetKeyDown(KeyCode.X))
        {

            RaycastHit hit;
            if (Physics.Raycast(fpsPos.transform.position, fpsPos.transform.forward, out hit, range))
            {
                return true;
            }
        }
        return false;
    }
}

