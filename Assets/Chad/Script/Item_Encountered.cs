using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Item_Encountered{
	public GameObject Prefab;
	public string PrefabName
	{
		get
		{
			return Prefab.name;
		}
	}

	public int e_time;
	public Vector3 pos;
	public bool encoutered=false;
	public Image icon;
}
