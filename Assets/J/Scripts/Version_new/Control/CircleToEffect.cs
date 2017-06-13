using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleToEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnMouseDown()
    {

        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit2D rayhit=Physics2D.Raycast (ray.origin,ray.direction,100);
        GameObject tempclick=JObjectPool._InstanceJObjectPool.GetGameObject("ClickEffect",rayhit.point);
		StartCoroutine(delay(tempclick));
	}
	IEnumerator delay(GameObject g){
		yield return new WaitForSeconds(2f);
		JObjectPool._InstanceJObjectPool.Recovery(g);
	}
}
