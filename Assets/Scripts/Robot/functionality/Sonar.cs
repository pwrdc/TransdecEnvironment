using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Robot
{
    public class Sonar : MonoBehaviour
    {

        [SerializeField]
        private float _viewRadious;
        private string nameTasksObject = "TasksFolder";
        private Dictionary<Transform, int> _objectsInSonarArea;

        public Dictionary<Transform, int> objectsInSonarArea
        {
            get { return _objectsInSonarArea; }
        }

        public float viewRadious
        {
            get { return _viewRadious; }
        }

        private void Start()
        {
            Transform tasks = GameObject.FindWithTag(nameTasksObject).transform;
            _objectsInSonarArea = new Dictionary<Transform, int>();



            for (int ID = 0; ID < tasks.childCount; ID++)
            {
                _objectsInSonarArea.Add(tasks.GetChild(ID), 0);
            }

        }

        void Update()
        {

            for (int i = 0; i < _objectsInSonarArea.Count; i++)
            {

                var element = _objectsInSonarArea.ElementAt(i);


                if (Vector3.Distance(element.Key.position, transform.position) <= _viewRadious)
                {
                    _objectsInSonarArea[element.Key] = 1;
                }
                else
                {
                    _objectsInSonarArea[element.Key] = 0;
                }

            }

        }
    }
}
