using System;
using UnityEngine;

public class RandomInit : MonoBehaviour {

    public bool drawTransdecQuarter = false;

    [HideInInspector]
    public System.Random rnd;

    [Serializable]
    public class ObjectPosition {
        public GameObject obj;
        public Vector3 startPosition = new Vector3(0f, 0f, 0f);
        [Range(0f, 10f)]
        public float xPosRange = 1f;
        [Range(0f, 10f)]
        public float minusXPosRange = 1f;
        [Range(0f, 10f)]
        public float yPosRange = 1f;
        [Range(0f, 10f)]
        public float minusYPosRange = 1f;
        [Range(0f, 10f)]
        public float zPosRange = 1f;
        [Range(0f, 10f)]
        public float minusZPosRange = 1f;
        public Vector3 startRotation = new Vector3(0f, 0f, 0f);
        [Range(0f, 30f)]
        public float xAngRange = 5f;
        [Range(0f, 30f)]
        public float yAngRange = 5f;
        [Range(0f, 30f)]
        public float zAngRange = 5f;
        [Range(0f, 360f)]
        public float[] allowedRotations;
    }
    public ObjectPosition[] objects;

    void Start () {
        for (int i = 0; i < this.objects.Length; i++) {
            // if object position and rotation are not default save them for randomizer
            if (objects[i].obj.transform.position != new Vector3(0f, 0f, 0f)) objects[i].startPosition = objects[i].obj.transform.position;
            if (objects[i].obj.transform.eulerAngles != new Vector3(0f, 0f, 0f)) objects[i].startRotation = objects[i].obj.transform.eulerAngles;
        }
	}
    float getRandom(float min, float max)
    {
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    public void PutAll() {
        int quarter, xCoef, zCoef;
        if (drawTransdecQuarter) {
            // select one of the quarters
            quarter = rnd.Next(0, 4);
            // divide into binary for particular quarter selection
            xCoef = 2 * (quarter % 2) - 1;
            zCoef = 2 * (quarter / 2 % 2) - 1;
        }
        else {
            xCoef = 1;
            zCoef = 1;
        }
        for (int i = 0; i < this.objects.Length; i++)
        {
            // adjust positions according to values stored in the object (or set in inspector); take coefs into consideration
            float xPos = xCoef * objects[i].startPosition.x + getRandom(objects[i].minusXPosRange, objects[i].xPosRange);
            float yPos = objects[i].startPosition.y + getRandom(objects[i].minusYPosRange, objects[i].yPosRange);
            float zPos = zCoef * objects[i].startPosition.z + getRandom(objects[i].minusZPosRange, objects[i].zPosRange);
            // transform position
            objects[i].obj.transform.position = new Vector3(xPos, yPos, zPos);
            // adjust rotations according to values stored in object (or set in inspector)
            float xRot = objects[i].startRotation.x + getRandom(-objects[i].xAngRange, objects[i].xAngRange);
            float yRot = objects[i].startRotation.y + getRandom(-objects[i].yAngRange, objects[i].yAngRange);
            // if object has several possible y rotations, pick one randomly
            if (objects[i].allowedRotations.Length != 0) yRot += -xCoef * zCoef * objects[i].allowedRotations[rnd.Next(0, objects[i].allowedRotations.Length)];
            // if on the other side, rotate 180
            if (zCoef == -1) yRot += 180f;
            float zRot = objects[i].startRotation.z + getRandom(-objects[i].zAngRange, objects[i].zAngRange);
            // transform rotation
            objects[i].obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
        }
    }
}
