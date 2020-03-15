using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class RandomInitOnMesh : MonoBehaviour
    {
        public Renderer mesh;
        private int layerMask = 1 << 10;
        [SerializeField]
        private List<GameObject> objectsToPut = new List<GameObject>();

        Vector3 GetPosition()
        {
            Vector3 position = new Vector3();
            position.x=Utils.GetRandom(mesh.bounds.min.x, mesh.bounds.max.x);
            position.z=Utils.GetRandom(mesh.bounds.min.z, mesh.bounds.max.z);
            position.y=mesh.bounds.max.y;
            RaycastHit hit;
            if (Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
            {
                position.y = -hit.distance + 2f;
            }
            return position;
        }

        public void PutTarget(GameObject target)
        {
            target.transform.position = GetPosition();
        }

        public void PutAll()
        {
            foreach (var obj in objectsToPut)
            {
                PutTarget(obj);
            }
        }
    }
}