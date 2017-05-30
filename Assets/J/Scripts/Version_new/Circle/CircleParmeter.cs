using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleParmeter : MonoBehaviour {

    public int LimitNumber;
    public float Radius;

    string PrefabName;
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
        modifyvecter *= Radius;

        JObjectPool._InstanceJObjectPool.GetGameObject(PrefabName, modifyvecter);
        PrefabName = null;
    }

    
}
