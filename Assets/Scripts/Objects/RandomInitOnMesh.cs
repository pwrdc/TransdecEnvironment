using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class RandomInitOnMesh : MonoBehaviour, IRandomInit
    {
        private Renderer mesh;
        private RaycastHit hit;
        private Vector3 position;
        private int layerMask = 1 << 10;
        [SerializeField]
        private List<GameObject> objectsToPut = new List<GameObject>();

        Vector3 GetRandomPosition(GameObject target)
        {
            float randomX = Utils.GetRandom(mesh.bounds.min.x, mesh.bounds.max.x);
            float randomZ = Utils.GetRandom(mesh.bounds.min.z, mesh.bounds.max.z);
            float y_pos = 0;
            if (Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
            {
                y_pos = -hit.distance + 2f;
            }
            return new Vector3(randomX, target.transform.position.y, randomZ);
        }

        bool IsCorrectPosition()
        {
            if (!Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
                return false;
            return true;
        }

        Vector3 GetPosition(GameObject target)
        {
            do
            {
                position = GetRandomPosition(target);
            } while (!IsCorrectPosition());

            return position;
        }

        public void PutTarget(GameObject target)
        {
            position = GetPosition(target);
            target.transform.position = position;
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