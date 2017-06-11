using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class blackhole : MonoBehaviour {
    Transform AllSceneObj;
	// Use this for initialization
	void Start () {
        AllSceneObj = GameObject.Find("ALL Scene obj").transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LightBallControl>())
        {
            other.transform.SetParent(AllSceneObj);
            other.transform.GetComponent<Collider>().enabled = false;
            other.transform.DOMove(transform.position, 1.5f);
            JObjectPool._InstanceJObjectPool.Recovery(other.gameObject);
        }
    }
}
