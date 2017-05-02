using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRange : MonoBehaviour {
    /// <summary>
    //擁有這個偵測範圍的角色
    /// </summary>
    public CharacterBase m_CharacterBase;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D other)
    {
		EnsContrrol tempEnemyBase = other.GetComponent<EnsContrrol>();
		if (tempEnemyBase != null && !tempEnemyBase.isDie /*&& tempEnemyBase._CharacterBase!=m_CharacterBase*/)
		{	
			tempEnemyBase.AddChar (m_CharacterBase);
			tempEnemyBase._CharacterBase = m_CharacterBase;
            m_CharacterBase.AddEnemy(tempEnemyBase);
            tempEnemyBase._CharacterBase = m_CharacterBase;
			m_CharacterBase.DebugList ("Enter");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
		EnsContrrol tempEnemyBase = other.GetComponent<EnsContrrol>();
		if (tempEnemyBase != null /*&& !tempEnemyBase.isDie*/ )
        {
			tempEnemyBase.RemoveChar (m_CharacterBase);
            m_CharacterBase.RemoveEnemy(tempEnemyBase);
            //tempEnemyBase._CharacterBase = null;
			m_CharacterBase.DebugList ("Exit");
        }
    }
}
