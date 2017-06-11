using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {
	
	public int maxlevel;
	public List<LevelData> Leveldata_list;

	// Use this for initialization
	void Start () {
		maxlevel = Leveldata_list.Count;
	}

	public LevelData getdata(int i){
		return Leveldata_list [i];
	}
	// Update is called once per frame
	void Update () {

	}
}
