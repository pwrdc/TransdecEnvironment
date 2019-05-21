using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectCreator
{
	[SerializeField] 
	public List<RobotAcademy.DataCollection> targetModes = new List<RobotAcademy.DataCollection>();
	[SerializeField] 
	public List<GameObject> targetObjects = new List<GameObject>();
	[SerializeField] 
	public List<GameObject> targetAnnotations = new List<GameObject>();
	[SerializeField] 
	public List<bool> targetIsEnabled = new List<bool>();
}