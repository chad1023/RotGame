using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotIns : MonoBehaviour {
	public Transform SpotPosition;
	public float SpotSpeed;

	private float offsetPos;

	private string Name="光點";
	private float randonScale;
	private float randonTime;
	private bool canSpawn=true;
	// Use this for initialization
	void Start () {
		randonTime=Random.Range(1,1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if(canSpawn)
			StartCoroutine(Spawn());
	}
	IEnumerator Spawn(){
		canSpawn=false;
		offsetPos=Random.Range(-2.8f,2.8f);
		Vector3 pos=SpotPosition.position;
		pos.x+=offsetPos;
		GameObject tempSpot;
		tempSpot=JObjectPool._InstanceJObjectPool.GetGameObject(Name,pos);
		tempSpot.GetComponent<SpotFly>().speed=SpotSpeed;
		yield return new WaitForSeconds(randonTime);
		randonTime=Random.Range(5,10.0f);
		canSpawn=true;
	}
}
