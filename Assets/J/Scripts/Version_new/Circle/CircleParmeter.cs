using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleParmeter : MonoBehaviour {
    public GameMain gamemain;
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
		if(LimitNumber>0 && gamemain.shoot!=""){
            // get the ray point
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D rayhit = Physics2D.Raycast(ray.origin, ray.direction, 10);
            // change it as vector3
            Vector3 modifyvecter = new Vector3(rayhit.point.x, rayhit.point.y, 0);
            modifyvecter.Normalize();
            // modify its magnitude to Radius
            modifyvecter *= Radius;

			GameObject InsLight= JObjectPool._InstanceJObjectPool.GetGameObject(gamemain.shoot, modifyvecter);
            InsLight.transform.localScale=new Vector3(0.8f,0.8f,0.8f);
            InsLight.transform.SetParent(this.transform);
            
			gamemain.EnergyShoot ();
            
			LimitNumber--;
        }
    }

    
}
