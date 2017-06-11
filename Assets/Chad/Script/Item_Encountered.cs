using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Item_Encountered{
	
	public string PrefabName;
	public Vector3 pos;
	public int e_time;
	public bool encoutered=false;
	public List <GameObject> type;
}
[System.Serializable]
public class planet
{
	public GameObject prefab;
	public Vector3 pos;
	public int e_time;
	public bool encoutered=false;
}
