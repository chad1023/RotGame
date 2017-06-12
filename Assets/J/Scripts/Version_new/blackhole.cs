using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class blackhole : MonoBehaviour {
    Transform AllSceneObj;
	// Use this for initialization
	void Start () {
        AllSceneObj = GameObject.Find("ALL Scene obj").transform;
//		Tweener tweener = transform.DORotate (new Vector3 (0, 0, 360), 5f);
//		tweener.SetUpdate (true);
//		tweener.SetEase(Ease.Linear);
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}


    void OnTriggerEnter(Collider other)
    {
		print (other.gameObject.name);
		if (other.GetComponent<LightBallControl>())
        {
            other.transform.SetParent(AllSceneObj);
            other.transform.GetComponent<Collider>().enabled = false;
            other.transform.DOMove(transform.position, 1.5f);
            JObjectPool._InstanceJObjectPool.Recovery(other.gameObject);
        }
    }
}
