// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-02-2019
// ***********************************************************************
// <copyright file="LightManager.cs" company="">
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
    /// Class LightManager.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class LightManager : MonoBehaviour
    {
        /// <summary>
        /// The light
        /// </summary>
        private new Light light;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Start()
        {
            light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        }

        /// <summary>
        /// Initializes the light.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="intensitivity">The intensitivity.</param>
        public void initializeLight(float angle, float intensitivity)
        {
            light.intensity = intensitivity;
            light.transform.rotation = Quaternion.Euler(angle, -90, 0);
        }
    }
}