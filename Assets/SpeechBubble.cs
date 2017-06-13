using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		Time.timeScale = 0;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			
			Time.timeScale = 1;
			Destroy (gameObject);
		}
		
	}
}
