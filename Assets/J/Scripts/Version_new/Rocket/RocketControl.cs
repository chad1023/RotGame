using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        FlyingObjControl _FlyingObjControl = other.GetComponent<FlyingObjControl>();
        if (_FlyingObjControl) { }
    }

}
