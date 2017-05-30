using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickmanagement : MonoBehaviour {

    public string PrefabName;
    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ClickLight(string s)
    {
        PrefabName=s;
    }
}
