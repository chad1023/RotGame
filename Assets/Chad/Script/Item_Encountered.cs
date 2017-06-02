using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
