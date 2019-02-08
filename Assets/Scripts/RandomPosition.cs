using System.Collections;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;
    public GameObject[] other_objs;
    public bool enable_camera_rotation = false;
    [Range(0.0f, 10.0f)]
    public float min_radius = 3.0f;
    [Range(0.0f, 10.0f)]
    public float max_radius = 10.0f;
    public float water_level = 11.0f;
    [Range(0.0f, 11.0f)]
    public float max_depth = 4.0f;
    [Range(0.0f, 30.0f)]
    public float x_ang_range = 5f;
    [Range(0.0f, 30.0f)]
    public float y_ang_range = 5f;
    [Range(0.0f, 30.0f)]
    public float z_ang_range = 5f;
    public bool random_quarter = false;
    //other objects parameters
    public bool oth_objects = true;
    [Range(0.0f, 10.0f)]
    public float oth_min_radius = 0.0f;
    [Range(0.0f, 10.0f)]
    public float oth_max_radius = 4.0f;
    [Range(0.0f, 11.0f)]
    public float oth_max_depth = 8.0f;
    [Range(0.0f, 90.0f)]
    public float oth_x_ang_range = 90f;
    [Range(0.0f, 90.0f)]
    public float oth_y_ang_range = 90f;
    [Range(0.0f, 90.0f)]
    public float oth_z_ang_range = 90f;

    Bounds get_bounds(GameObject target) {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for(int i = 0; i < renderers.Length; i++) {
            if(renderers[i] != renderers[0]) bounds.Encapsulate(renderers[i].bounds);
        } 

        return bounds;
    }

    float get_random(float min, float max)
    {
        System.Random rnd = GameObject.Find("Academy").GetComponent<RandomInit>().rnd;
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    public void get_new_pos()
    {
        Bounds bounds = get_bounds(target);

        Vector3 new_pos = bounds.center;
        float x_rot = get_random(-x_ang_range, x_ang_range);
        float y_rot = get_random(-y_ang_range, y_ang_range);
        float z_rot = get_random(-z_ang_range, z_ang_range);
        float r = get_random(min_radius, max_radius);
        float theta = get_random(0, 2 * Mathf.PI);
        new_pos.x += r * Mathf.Cos(theta);
        new_pos.y = get_random(max_depth, water_level);
        new_pos.z += r * Mathf.Sin(theta);
        agent.transform.position = new_pos;
        agent.transform.LookAt(bounds.center);
        if(!enable_camera_rotation) agent.transform.eulerAngles = new Vector3(x_rot, agent.transform.eulerAngles.y + y_rot, z_rot);
    }

    public void get_oth_new_pos(GameObject obj)
    {
        Bounds bounds = get_bounds(target);

        Vector3 new_pos = bounds.center;
        float x_rot = get_random(-oth_x_ang_range, oth_x_ang_range);
        float y_rot = get_random(-oth_y_ang_range, oth_y_ang_range);
        float z_rot = get_random(-oth_z_ang_range, oth_z_ang_range);
        float r = get_random(oth_min_radius, oth_max_radius);
        float theta = get_random(0, 2 * Mathf.PI);
        new_pos.x += r * Mathf.Cos(theta);
        new_pos.y = get_random(oth_max_depth, water_level);
        new_pos.z += r * Mathf.Sin(theta);
        obj.transform.position = new_pos;
        obj.transform.LookAt(bounds.center);
        obj.transform.eulerAngles = new Vector3(x_rot, obj.transform.eulerAngles.y + y_rot, z_rot);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (random_quarter)
        {
            GameObject.Find("Academy").GetComponent<RandomInit>().PutAll();
        }
        get_new_pos();
        if(oth_objects)
        {
            foreach (GameObject obj in other_objs) get_oth_new_pos(obj);
        }
    }
    
}