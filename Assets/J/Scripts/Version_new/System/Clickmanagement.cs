using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickmanagement : MonoBehaviour {
    public static Clickmanagement _InstanceClickmanagement = null;


    public string ClickName;
    // Use this for initialization
    void Start () {

        //Check if there is already an instance of Clickmanagement
        if (_InstanceClickmanagement == null)
            _InstanceClickmanagement = this;
        //If _InstanceClickmanagement already exists ,Destroy this 
        //this enforces our singleton pattern so there can only be one instance of Clickmanagement.
        else if (_InstanceClickmanagement != this)
            Destroy(this.gameObject);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
