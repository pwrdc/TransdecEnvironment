// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-20-2019
// ***********************************************************************
// <copyright file="Collision.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Collision.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class Collision : MonoBehaviour
{

    /// <summary>
    /// The dis
    /// </summary>
    public float dis;

    /// <summary>
    /// The main object
    /// </summary>
    public GameObject mainObject;
    /// <summary>
    /// The colliders
    /// </summary>
    List<Collider> colliders = new List<Collider>();
    /// <summary>
    /// The minimum
    /// </summary>
    Collider min;

    /// <summary>
    /// Updates this instance.
    /// </summary>
    void Update()
    {
        float minDis = 100;
   
        if (colliders.Count != 0)
        {
            foreach (Collider element in colliders)
            {
                
                float distance = Vector3.Distance(element.transform.position, mainObject.transform.position);
                
             
               if (distance< minDis)
                {
                    minDis = distance;
                    min = element;
                }
                dis = minDis;

            }
            colliders.Clear();
        }

    }

    /// <summary>
    /// Called when [trigger enter].
    /// </summary>
    /// <param name="co">The co.</param>
    void OnTriggerEnter(Collider co)
    {
        if (!colliders.Contains(co))
        {
            colliders.Add(co);
        }
    }

    /// <summary>
    /// Gets the distance.
    /// </summary>
    /// <returns>System.Single.</returns>
    public float getDistance()
    {
        return dis;
    }
     
}
