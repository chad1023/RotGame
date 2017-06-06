using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("Destroyitself", 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Destroyifself(){
		JObjectPool._InstanceJObjectPool.Recovery (gameObject, Vector3.zero);
	}
}
