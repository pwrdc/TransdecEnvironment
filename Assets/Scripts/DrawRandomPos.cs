using System;
using UnityEngine;

public class RandomCylinderInit : MonoBehaviour {

    [Serializable]
    public class ObjectPosition {
        public GameObject obj; //Robot
        public CapsuleCollider target_capsule_collider; //Spawn only on edges of collider
        public GameObject look_at_object; //Robot will look at object
        [HideInInspector]
        public Vector3 start_position = new Vector3(0f, 0f, 0f);
    	[HideInInspector]
        public int direction;
    	[HideInInspector]
        public float radius;
    	[HideInInspector]
        public float height;
    }

    private enum Direction {X, Y, Z};
    
    public ObjectPosition[] objects;

    void Start () {
        for (int i = 0; i < this.objects.Length; i++) {
            objects[i].start_position = objects[i].target_capsule_collider.transform.position;
            objects[i].direction = objects[i].target_capsule_collider.direction;
            
            Debug.Log(objects[i].start_position);
            Debug.Log(objects[i].target_capsule_collider.bounds.size);
            
            
            if (objects[i].direction == (int)Direction.X) {
            	objects[i].height = objects[i].target_capsule_collider.bounds.size.x - objects[i].target_capsule_collider.bounds.size.y;
            	objects[i].radius = objects[i].target_capsule_collider.bounds.size.y / 2;
            }
            else if (objects[i].direction == (int)Direction.Y) {
            	objects[i].height = objects[i].target_capsule_collider.bounds.size.y - objects[i].target_capsule_collider.bounds.size.z;
            	objects[i].radius = objects[i].target_capsule_collider.bounds.size.z / 2;
            }
            else {
            	objects[i].height = objects[i].target_capsule_collider.bounds.size.z - objects[i].target_capsule_collider.bounds.size.x;
            	objects[i].radius = objects[i].target_capsule_collider.bounds.size.x / 2;
            }
        	
        }
        PutAll();
	}

    void PutAll() {
        System.Random rnd = new System.Random();
        for (int i = 0; i < this.objects.Length; i++) {
            double deegres = rnd.NextDouble() * 360;
            double phi = Math.PI * deegres / 180.0;
            float height = (float)(rnd.NextDouble() - 0.5);

            if (objects[i].direction == (int)Direction.X)
            	CalculateDirectionX(i, phi, height);
        	else if (objects[i].direction == (int)Direction.Y)
            	CalculateDirectionY(i, phi, height);
        	else
            	CalculateDirectionZ(i, phi, height);

            Debug.Log("Phi: " + phi);

            objects[i].obj.transform.LookAt(objects[i].look_at_object.transform);
            objects[i].obj.transform.eulerAngles = new Vector3(0, objects[i].obj.transform.rotation.eulerAngles.y, 0);
        }
    }

    void CalculateDirectionX(int index, double phi, float height) {
    	float z_pos = objects[index].start_position.z + objects[index].radius * (float)(Math.Sin(phi));
        float y_pos = objects[index].start_position.y + objects[index].radius * (float)(Math.Cos(phi));
        float x_pos = objects[index].start_position.x + objects[index].height * height;

        objects[index].obj.transform.position = new Vector3(x_pos, y_pos, z_pos);
    }

    void CalculateDirectionY(int index, double phi, float height) {
    	float x_pos = objects[index].start_position.x + objects[index].radius * (float)(Math.Cos(phi));
        float z_pos = objects[index].start_position.z + objects[index].radius * (float)(Math.Sin(phi));
        float y_pos = objects[index].start_position.y + objects[index].height * height;

        objects[index].obj.transform.position = new Vector3(x_pos, y_pos, z_pos);
    }

    void CalculateDirectionZ(int index, double phi, float height) {
    	float x_pos = objects[index].start_position.x + objects[index].radius * (float)(Math.Cos(phi));
        float y_pos = objects[index].start_position.y + objects[index].radius * (float)(Math.Sin(phi));
        float z_pos = objects[index].start_position.z + objects[index].height * height;

        objects[index].obj.transform.position = new Vector3(x_pos, y_pos, z_pos);
    }
	
	void Update () {
		
	}
}
