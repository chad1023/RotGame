using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour {
	Transform AllSceneObj;
	// Use this for initialization
	void Start () {
		AllSceneObj = GameObject.Find("ALL Scene obj").transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<LightBallControl>())
        {
            other.transform.SetParent(AllSceneObj);
            other.transform.GetComponent<Collider>().enabled = false;
            //other.transform.DOMove(transform.position, 1.5f);
            JObjectPool._InstanceJObjectPool.DelayRecovery(other.gameObject,1.5f);
        }
   
	}
}
