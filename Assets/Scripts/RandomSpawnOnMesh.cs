using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnOnMesh : MonoBehaviour
{
	public Renderer renderer;
	public GameObject target;
	private	RaycastHit hit;
	private Vector3 position;

	private List<GameObject> paths = new List<GameObject>();

    [Range(120.0f, 135.0f)]
	public float rotation_min = 130;
    [Range(135.0f, 150.0f)]
	public float rotation_max = 140;

	Vector3 get_random_position(int layerMask = 1 << 10) {
		float randomX = Random.Range(renderer.bounds.min.x, renderer.bounds.max.x);
		float randomZ = Random.Range(renderer.bounds.min.z, renderer.bounds.max.z);
		float y_pos = 0; 
		if (Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask)) {
		    y_pos = -hit.distance + 2f;
		}
		return new Vector3(randomX, renderer.bounds.max.y, randomZ);
	}

	bool is_correct_position(int layerMask = 1 << 10) {
		if (!Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
		    return false;
		return true;
	}

	Vector3 get_position() {
    	int layerMask = 1 << 10;
		do {
			position = get_random_position();
		} while(!is_correct_position());

		return position;
	}



	public void spawn_path() {
    	position = get_position();
		target.transform.position = position;
	

        float rotation = Random.Range(0, 360);
        target.transform.eulerAngles = new Vector3(0, rotation, 0);
    }

    public void rotate_path() {
    	float rotation = Random.Range(rotation_min, rotation_max);
    	paths[1].transform.localRotation = Quaternion.Euler(0, rotation, 0);
    }

    void Awake() {
    	foreach (Transform child in target.transform)
        {
            paths.Add(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    	position = get_position();
		target.transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}