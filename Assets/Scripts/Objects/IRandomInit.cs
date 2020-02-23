using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    interface IRandomInit
    {
        void PutAll();
        void PutTarget(GameObject target);

        void UpdateData(ObjectConfigurationSettings settings);
    }
}