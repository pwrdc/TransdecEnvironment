// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-02-2019
// ***********************************************************************
// <copyright file="BuoyRotation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneEnvironment
{
    /// <summary>
    /// Class BuoyRotation.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BuoyRotation : MonoBehaviour
    {

        /// <summary>
        /// The rotation speed
        /// </summary>
        public float RotationSpeed = 360 / 50;
        /// <summary>
        /// The robot
        /// </summary>
        public GameObject Robot;
        /// <summary>
        /// The bounds
        /// </summary>
        private Bounds bounds;


        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            bounds = Utils.GetComplexBounds(gameObject);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update()
        {
            transform.RotateAround(bounds.center, new Vector3(0, 1, 0), RotationSpeed);
        }
    }
}