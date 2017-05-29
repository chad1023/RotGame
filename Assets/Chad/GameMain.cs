using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameMain : MonoBehaviour {
	[Header("這邊放東西用的")]
	public JObjectPool ObjectPool;

    
    // Use this for initialization
    void Start () {
		if(ObjectPool==null){
			
			ObjectPool = GameObject.FindObjectOfType<JObjectPool> ();
			if (!ObjectPool) {
				Debug.LogError ("Couldn't find the ObjectPool!!");
			}
		}


	}
	
	// Update is called once per frame
	void Update () {
		
    }


}
