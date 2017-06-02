using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour {
	private string exploname="ExploEffect";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        FlyingObjControl _FlyingObjControl = other.GetComponent<FlyingObjControl>();
        if (_FlyingObjControl) { 
			JObjectPool._InstanceJObjectPool.Recovery(_FlyingObjControl.gameObject);
			GameObject tempExplo= JObjectPool._InstanceJObjectPool.GetGameObject(exploname,other.transform.position);
			JObjectPool._InstanceJObjectPool.DelayRecovery(tempExplo,1.5f);
		}
    }

}
