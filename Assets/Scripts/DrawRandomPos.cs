using System;
using UnityEngine;

public class DrawRandomPos : MonoBehaviour {

    [Serializable]
    public class ObjectPosition {
        public GameObject obj; //Robot
        public CapsuleCollider targetCapsuleCollider; //Spawn only on edges of collider
        public GameObject lookAtObject; //Robot will look at object
        [HideInInspector]
        public Vector3 startPosition = new Vector3(0f, 0f, 0f);
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
            objects[i].startPosition = objects[i].targetCapsuleCollider.transform.position;
            objects[i].direction = objects[i].targetCapsuleCollider.direction;
            
            Debug.Log(objects[i].startPosition);
            Debug.Log(objects[i].targetCapsuleCollider.bounds.size);
            
            
            if (objects[i].direction == (int)Direction.X) {
            	objects[i].height = objects[i].targetCapsuleCollider.bounds.size.x - objects[i].targetCapsuleCollider.bounds.size.y;
            	objects[i].radius = objects[i].targetCapsuleCollider.bounds.size.y / 2;
            }
            else if (objects[i].direction == (int)Direction.Y) {
            	objects[i].height = objects[i].targetCapsuleCollider.bounds.size.y - objects[i].targetCapsuleCollider.bounds.size.z;
            	objects[i].radius = objects[i].targetCapsuleCollider.bounds.size.z / 2;
            }
            else {
            	objects[i].height = objects[i].targetCapsuleCollider.bounds.size.z - objects[i].targetCapsuleCollider.bounds.size.x;
            	objects[i].radius = objects[i].targetCapsuleCollider.bounds.size.x / 2;
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

            objects[i].obj.transform.LookAt(objects[i].lookAtObject.transform);
            objects[i].obj.transform.eulerAngles = new Vector3(0, objects[i].obj.transform.rotation.eulerAngles.y, 0);
        }
    }

    void CalculateDirectionX(int index, double phi, float height) {
    	float zPos = objects[index].startPosition.z + objects[index].radius * (float)(Math.Sin(phi));
        float yPos = objects[index].startPosition.y + objects[index].radius * (float)(Math.Cos(phi));
        float xPos = objects[index].startPosition.x + objects[index].height * height;

        objects[index].obj.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void CalculateDirectionY(int index, double phi, float height) {
    	float xPos = objects[index].startPosition.x + objects[index].radius * (float)(Math.Cos(phi));
        float zPos = objects[index].startPosition.z + objects[index].radius * (float)(Math.Sin(phi));
        float yPos = objects[index].startPosition.y + objects[index].height * height;

        objects[index].obj.transform.position = new Vector3(xPos, yPos, zPos);
    }

    void CalculateDirectionZ(int index, double phi, float height) {
    	float xPos = objects[index].startPosition.x + objects[index].radius * (float)(Math.Cos(phi));
        float yPos = objects[index].startPosition.y + objects[index].radius * (float)(Math.Sin(phi));
        float zPos = objects[index].startPosition.z + objects[index].height * height;

        objects[index].obj.transform.position = new Vector3(xPos, yPos, zPos);
    }
	
	void Update () {
		
	}
}
