using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace ObjectCollection
{
	[System.Serializable]
	public class ObjectCreator
	{
		[SerializeField] 
		public List<RobotAcademy.DataCollection> targetCameraModes = new List<RobotAcademy.DataCollection>();
		[SerializeField] 
		public List<RobotAcademy.ObjectType> targetObjectModes = new List<RobotAcademy.ObjectType>();
		[SerializeField] 
		public List<GameObject> targetObjects = new List<GameObject>();
		[SerializeField] 
		public List<GameObject> targetAnnotations = new List<GameObject>();
		[SerializeField] 
		public List<bool> targetIsEnabled = new List<bool>();
	}
}