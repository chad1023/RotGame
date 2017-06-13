using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class blackhole : MonoBehaviour {
    Transform AllSceneObj;
	public Transform image;
//	Rigidbody rigidbody;
//	public float speed;
//	Tweener tweener;
//	Tweener tweener_drop;
//	// Use this for initialization
	void Start () {
        AllSceneObj = GameObject.Find("ALL Scene obj").transform;
//		rigidbody = GetComponent<Rigidbody> ();

		Tweener tweener = image.DORotate (new Vector3 (0, 0, -360), 1f,RotateMode.LocalAxisAdd);
//
////		Tweener tweener = rigidbody.DORotate (new Vector3 (0, 0, 360), 1f,RotateMode.FastBeyond360);
		tweener.SetLoops (-1);
		tweener.SetEase(Ease.Linear);
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
            other.transform.DOMove(transform.position, 0.5f);
            JObjectPool._InstanceJObjectPool.DelayRecovery(other.gameObject,0.5f);

        }


    }
}
