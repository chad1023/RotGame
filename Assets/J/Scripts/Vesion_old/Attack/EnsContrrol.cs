using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnsContrrol : MonoBehaviour {
	public GameEnum.Type_Color ColorType;
	public GameObject Target;
	public float force;
	public float blood=10;
	float iniblood;
	/// <summary>
	//被哪個角色的範圍所偵測到
	/// </summary>
	public CharacterBase _CharacterBase;
	public bool isDie = false;
    /// <summary>
    //被哪些角色的範圍所偵測到
    /// </summary>
    public List<CharacterBase> m_ChList;
    /// <summary>
    //是否暫停
    /// </summary>
    public bool isStop = false;

	GameMain gamemain;
	ParticleSystem explosion;
	TextMesh bloodtext;
	Vector2 Dir;
	Rigidbody2D m_Rigidbody2D;
	// Use this for initialization
	void Start () {
		Target = GameObject.Find ("Center");
		m_Rigidbody2D = GetComponent<Rigidbody2D> ();
		bloodtext = GetComponentInChildren<TextMesh> ();
		gamemain = GameObject.FindObjectOfType<GameMain> ();
		iniblood = blood;
		Reset ();
//		explosion = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		Dir = Target.transform.position - transform.position;
        //m_Rigidbody2D.AddForce (Dir.normalized * force * Time.deltaTime, ForceMode2D.Force);

        if (!isStop)
            m_Rigidbody2D.velocity = Dir.normalized * force * 0.1f;
        else
            m_Rigidbody2D.velocity = Vector2.zero;

        bloodtext.text = blood.ToString ();
		if (blood == 0 && !isDie) {
			DoDie ();

		}

	}


	//Attack Target
	void OnCollisionEnter2D(Collision2D other){
		if (other.collider.tag == "Player") {
			DoDie ();
			//Debug.Log (other.gameObject.name);

		}
	}
	void DoDie() {
		isDie = true;
		foreach (var item in m_ChList) {
			item.RemoveEnemy (this);
		}
		//Debug.Log ("die");

		Reset ();


		//Destroy(this.gameObject);
	}
	public void Reset()
	{
		m_ChList = new List<CharacterBase> ();
		blood = iniblood;
		isDie = false;
	}
	public void Hit(float dam)
	{
		blood -= dam;
		if (blood < 0) {
			blood = 0;
		}
	}
	public void AddChar(CharacterBase C)
	{
		if(!CheckInList(C))
			m_ChList.Add(C);
	}
	public void RemoveChar(CharacterBase C)
	{
		if(CheckInList(C))
			m_ChList.Remove(C);
	}
	public bool CheckInList(CharacterBase E){
		for (int i = 0; i < m_ChList.Count; i++) {
			if (m_ChList [i] == E)
				return true;
		}
		return false;

	}
}
