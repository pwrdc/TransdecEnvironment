// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-22-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-22-2019
// ***********************************************************************
// <copyright file="IRandomInit.cs" company="">
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
    /// Interface IRandomInit
    /// </summary>
    interface IRandomInit
    {
        /// <summary>
        /// Puts all.
        /// </summary>
        void PutAll();
        /// <summary>
        /// Puts the target.
        /// </summary>
        /// <param name="target">The target.</param>
        void PutTarget(GameObject target);

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void UpdateData(ObjectConfigurationSettings settings);
    }
}