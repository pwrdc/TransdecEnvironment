// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-21-2019
// ***********************************************************************
// <copyright file="DepthSensor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    /// <summary>
    /// Class DepthSensor.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class DepthSensor : MonoBehaviour
    {
        /// <summary>
        /// The water surface
        /// </summary>
        private GameObject waterSurface;
        /// <summary>
        /// The offset
        /// </summary>
        public float offset = 0.0f;

        /// <summary>
        /// The rbody
        /// </summary>
        private Rigidbody rbody;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Initializes the specified water surface.
        /// </summary>
        /// <param name="waterSurface">The water surface.</param>
        public void Init(GameObject waterSurface)
        {
            this.waterSurface = waterSurface;
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <returns>System.Single.</returns>
        public float GetDepth()
        {
            float depth;
            float d = waterSurface.transform.position.y - rbody.position.y + offset;
            if (d >= 0)
                depth = d;
            else
                depth = 0f;
            return depth;
        }
    }
}