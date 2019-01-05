using System;
using UnityEngine;

public class RandomInit : MonoBehaviour {

    [Serializable]
    public class ObjectPosition {
        public GameObject obj;
        public Vector3 start_position = new Vector3(0f, 0f, 0f);
        [Range(0f, 10f)]
        public float x_pos_range = 1f;
        [Range(0f, 10f)]
        public float y_pos_range = 1f;
        [Range(0f, 10f)]
        public float z_pos_range = 1f;
        public Vector3 start_rotation = new Vector3(0f, 0f, 0f);
        [Range(0f, 30f)]
        public float x_ang_range = 5f;
        [Range(0f, 30f)]
        public float y_ang_range = 5f;
        [Range(0f, 30f)]
        public float z_ang_range = 5f;
        [Range(0f, 360f)]
        public float[] allowed_rotations;
    }
    public System.Random rnd = new System.Random();
    public ObjectPosition[] objects;

    void Start () {
        for (int i = 0; i < this.objects.Length; i++) {
            // if object position and rotation are not default save them for randomizer
            if (objects[i].obj.transform.position != new Vector3(0f, 0f, 0f)) objects[i].start_position = objects[i].obj.transform.position;
            if (objects[i].obj.transform.eulerAngles != new Vector3(0f, 0f, 0f)) objects[i].start_rotation = objects[i].obj.transform.eulerAngles;
        }
        PutAll();
	}

    public void PutAll() {
        // select one of the quarters
        int quarter = rnd.Next(0, 4);
        // divide into binary for particular quarter selection
        int x_coef = 2 * (quarter % 2) - 1;
        int z_coef = 2 * (quarter / 2 % 2) - 1;
        for (int i = 0; i < this.objects.Length; i++)
        {
            // adjust positions according to values stored in the object (or set in inspector); take coefs into consideration
            float x_pos = x_coef * objects[i].start_position.x + (2 * (float)rnd.NextDouble() - 1) * objects[i].x_pos_range;
            float y_pos = objects[i].start_position.y + (2 * (float)rnd.NextDouble() - 1) * objects[i].y_pos_range;
            float z_pos = z_coef * objects[i].start_position.z + (2 * (float)rnd.NextDouble() - 1) * objects[i].z_pos_range;
            // transform position
            objects[i].obj.transform.position = new Vector3(x_pos, y_pos, z_pos);
            // adjust rotations according to values stored in object (or set in inspector)
            float x_rot = objects[i].start_rotation.x + (2 * (float)rnd.NextDouble() - 1) * objects[i].x_ang_range;
            float y_rot = objects[i].start_rotation.y + (2 * (float)rnd.NextDouble() - 1) * objects[i].y_ang_range;
            // if object has several possible y rotations, pick one randomly
            if (objects[i].allowed_rotations.Length != 0) y_rot = -x_coef * z_coef * objects[i].allowed_rotations[rnd.Next(0, objects[i].allowed_rotations.Length)];
            // if on the other side, rotate 180
            if (z_coef == -1) y_rot += 180f;
            float z_rot = objects[i].start_rotation.z + (2 * (float)rnd.NextDouble() - 1) * objects[i].z_ang_range;
            // transform rotation
            objects[i].obj.transform.eulerAngles = new Vector3(x_rot, y_rot, z_rot);
        }
    }
	
	void Update () {
		
	}
}
