using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public GameObject bullet;
    public bool shoot;

    void Update()
    {
        if (shoot == true)
        {
            GameObject new_bullet = (GameObject)Instantiate(bullet);
            shoot = false;
        }
    }
}
