using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleParmeter : MonoBehaviour {
    public Clickmanagement clickmanagement;
    public int LimitNumber;
    public float Radius;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnMouseDown()
    {
        // get the ray point
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayhit = Physics2D.Raycast(ray.origin, ray.direction, 10);
        // change it as vector3
        Vector3 modifyvecter = new Vector3(rayhit.point.x, rayhit.point.y, 0);
        Vector3.Normalize(modifyvecter);
        // modify its magnitude to Radius
        //modifyvecter *= Radius;

        GameObject InsLight= JObjectPool._InstanceJObjectPool.GetGameObject(clickmanagement.PrefabName, modifyvecter);
        InsLight.transform.localScale=new Vector3(0.8f,0.8f,0.8f);
        InsLight.transform.SetParent(this.transform);
        clickmanagement.PrefabName = null;
        print(transform.gameObject.name);
    }

    
}
