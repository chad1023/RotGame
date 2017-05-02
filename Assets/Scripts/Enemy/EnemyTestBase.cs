using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestBase : MonoBehaviour {
    public float Hp;
    /// <summary>
    //被哪個角色的範圍所偵測到
    /// </summary>
    public CharacterBase _CharacterBase;
	public bool isDie = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Hp <= 0 && !isDie)
        {
            isDie = true;
            DoDie();
        }

	}

    void DoDie() {
        //_CharacterBase.RemoveEnemy(this);
        //Destroy(this.gameObject);
    }
    public void Hit(float dam)
    {
        Hp -= dam;
    }
}
