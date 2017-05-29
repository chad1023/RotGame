using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testpool : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			GameObject t = JObjectPool._InstanceJObjectPool.GetGameObject ("Cube", transform.position, transform.rotation);
		
		}
	}
}
