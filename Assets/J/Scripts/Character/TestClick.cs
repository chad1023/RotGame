using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClick : MonoBehaviour {
	public GameObject Char1;
	public GameObject Char2;
	public GameObject Circle;
	public int Num;
	bool isClick =false;
	int n;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnMouseDown(){
		if (Num > 0 && isClick) {
			isClick = false;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D rayhit=Physics2D.Raycast (ray.origin,ray.direction,10);
			if(n==1)
				Instantiate (Char1, rayhit.point, Quaternion.identity,Circle.transform);
			else if (n==2)
				Instantiate (Char2, rayhit.point, Quaternion.identity,Circle.transform);
			Num--;
		}
	}
	public void Click1(){
		isClick = true;
		n = 1;
	}
	public void Click2(){
		isClick = true;
		n = 2;
	}
}
