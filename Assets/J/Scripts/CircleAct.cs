using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAct : MonoBehaviour {
    public GameObject CircleAttackRange;
	[Header("開關的頻率")]
    public float f_Time = 0.5f;
	[Header("持續的時間")]
     public float l_Time = 0.2f;
    float f_time;
    float l_time;
	// Use this for initialization
	void Start () {
        f_time = 0;	
	}
	
	// Update is called once per frame
	void Update () {
        f_time += Time.deltaTime;
        l_time += Time.deltaTime;

        if (f_time >= f_Time) {
            f_time = 0;
            l_time = 0;
            CircleAttackRange.SetActive(true);
        }
        if (l_time >= l_Time)
        {
            CircleAttackRange.SetActive(false);
        }
    
    }
    
    
}
