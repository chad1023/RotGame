using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    float x, y;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            //x = input.getaxis("mouse x");
            //y = input.getaxis("mouse y") ;
            //print("" + x + " , " + y);
        }
        if (Input.GetMouseButtonDown(0)) {
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");
            print("" + x + " , " + y);
        }
        if (Input.GetMouseButtonUp(1)) {
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");
            print("" + x + " , " + y);
        }
    }
}
