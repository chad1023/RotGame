using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour {
	public Vector3 m_Direction;
	public float speed;
	public float Att;
	public GameObject Effect;
	public GameEnum.Type_Color Color;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (m_Direction.normalized * speed);
	}
	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		EnsContrrol tempEnemyBase = other.GetComponent<EnsContrrol>();
		if (tempEnemyBase) 
		{
			GameObject tempEffect = (GameObject)Instantiate (Effect, transform.position, Quaternion.identity);
			if(tempEnemyBase.ColorType==Color)
				tempEnemyBase.Hit (Att * 2);
			else
				tempEnemyBase.Hit (Att);
			Destroy (this.gameObject);
			Destroy (tempEffect, 1f);
		}
	}
	public void SetValue(Vector3 D,float F ,GameEnum.Type_Color C)
	{
		m_Direction = D;
		Att = F;
		Color = C;
	}

}
