using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomInitOnMesh : MonoBehaviour
{
	public Renderer mesh;
	
	private	GameObject target;
	private	RaycastHit hit;
	private Vector3 position;

	private List<GameObject> paths = new List<GameObject>();

    [Range(110.0f, 135.0f)]
	public float rotation_min = 130;
    [Range(135.0f, 170.0f)]
	public float rotation_max = 140;


    public float GetRandom(float min, float max)
    {
        System.Random rnd = transform.gameObject.GetComponent<RandomInit>().GetRandomizer();
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

	Vector3 GetRandomPosition(int layerMask = 1 << 10) {
		float randomX = GetRandom(mesh.bounds.min.x, mesh.bounds.max.x);
		float randomZ = GetRandom(mesh.bounds.min.z, mesh.bounds.max.z);
		float y_pos = 0; 
		if (Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask)) {
		    y_pos = -hit.distance + 2f;
		}
		return new Vector3(randomX, target.transform.position.y, randomZ);
	}

	bool IsCorrectPosition(int layerMask = 1 << 10) {
		if (!Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
		    return false;
		return true;
	}

	Vector3 GetPosition() {
		do {
			position = GetRandomPosition();
		} while(!IsCorrectPosition());

		return position;
	}


	public void PutAll(GameObject target) {
		this.target = target;
		/*
		foreach (Transform child in target.transform) {
            target.Add(child.gameObject);
        }

        //Rotate angle of path
		float rotation = GetRandom(rotation_min, rotation_max);
    	paths[0].transform.localRotation = Quaternion.Euler(0, rotation, 0);
    	*/
    	//Get Random position for path
    	position = GetPosition();
		target.transform.position = position;
		
		//Rotate path
        //rotation = GetRandom(0, 360);
        //target.transform.eulerAngles = new Vector3(0, rotation, 0);
        
    }

}