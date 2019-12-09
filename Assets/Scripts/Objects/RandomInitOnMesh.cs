// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="RandomInitOnMesh.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Random init of objects on mesh
    /// </summary>
    public class RandomInitOnMesh : MonoBehaviour, IRandomInit
    {
        /// <summary>
        /// The mesh
        /// </summary>
        private Renderer mesh;
        /// <summary>
        /// The hit
        /// </summary>
        private RaycastHit hit;
        /// <summary>
        /// The position
        /// </summary>
        private Vector3 position;
        /// <summary>
        /// The layer mask
        /// </summary>
        private int layerMask = 1 << 10; //layer mask - POOL

        /// <summary>
        /// The objects to put
        /// </summary>
        [SerializeField]
        private List<GameObject> objectsToPut = new List<GameObject>();
        /// <summary>
        /// The object configuration settings
        /// </summary>
        private ObjectConfigurationSettings objectConfigurationSettings;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
        }

        /// <summary>
        /// Gets the random position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Vector3.</returns>
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

        /// <summary>
        /// Determines whether [is correct position].
        /// </summary>
        /// <returns><c>true</c> if [is correct position]; otherwise, <c>false</c>.</returns>
        bool IsCorrectPosition()
        {
            if (!Physics.Raycast(position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
                return false;
            return true;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Vector3.</returns>
        Vector3 GetPosition(GameObject target)
        {
            do
            {
                position = GetRandomPosition(target);
            } while (!IsCorrectPosition());

            return position;
        }

        /// <summary>
        /// Puts the target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void PutTarget(GameObject target)
        {
            position = GetPosition(target);
            target.transform.position = position;
        }

        /// <summary>
        /// Puts all.
        /// </summary>
        public void PutAll()
        {
            foreach(var obj in objectsToPut)
            {
                PutTarget(obj);
            }
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(ObjectConfigurationSettings settings)
        {
            objectConfigurationSettings = settings;
        }
    }
}