using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestClick : MonoBehaviour {
	public GameObject Char1;
	public GameObject Char2;
	public GameObject Circle;
	public int Num;
    public Text text_num;
	bool isClick =false;
	int n;
    GameObject tempC;
	// Use this for initialization
	void Start () {
        text_num.text = "" + Num;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnMouseDown(){
		if (Num > 0 && isClick) {
            
			isClick = false;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D rayhit=Physics2D.Raycast (ray.origin,ray.direction,10);
            if (n == 1)
                tempC = (GameObject)Instantiate(Char1, rayhit.point, Quaternion.identity, Circle.transform);
            else if (n == 2)
                tempC = (GameObject)Instantiate(Char2, rayhit.point, Quaternion.identity, Circle.transform);
			Num--;
            print(Vector3.Distance(transform.position, rayhit.point));
            tempC.transform.localScale = new Vector3(1, 1, 1) *
                (0.4f + Vector3.Distance(transform.position, rayhit.point) / 2.5f * 0.6f);
            text_num.text = "" + Num;
            print(rayhit.point);
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
