using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterBase : MonoBehaviour {
    [Header("基本數值")]
    public float Att;
    public float Hp;
    public int AttNum;
    public GameEnum.Type_Color m_Color;
    public float AttFrequency;
    public GameObject AttEffect;

	[SerializeField]
    protected bool canAttack = true;
	public List<EnsContrrol> m_EnemyList;

	// Use this for initialization
	protected virtual void Start ()
    {
		m_EnemyList = new List<EnsContrrol> ();	
	}

    // Update is called once per frame
    protected virtual void Update ()
    {
		if (canAttack && m_EnemyList.Count!=0)
        {
            canAttack = false;
            StartCoroutine(DoOneAttack());
        }
    }
    /// <summary>
    //施放技能
    /// </summary>
    public virtual void DoSkill()
    {
    }
    /// <summary>
    //攻擊
    /// </summary>
    protected virtual void DoAttack()
    {
		if (m_EnemyList.Count == 0) 
		{
			StopCoroutine (DoOneAttack ());
			canAttack = true;
			return;
		}
            
        float num = Mathf.Min(AttNum, m_EnemyList.Count);

//		Debug.Log (num);
        for (int i = 0; i < num; i++)
        {
            //m_EnemyList[i].Hit(Att);
			Vector3 tempDir = m_EnemyList [i].transform.position - transform.position;
			AttackBase tempAtt = Instantiate (AttEffect, transform.position, Quaternion.identity).GetComponent<AttackBase> ();
			tempAtt.SetValue (tempDir, Att , m_Color);
		}
    }
    IEnumerator DoOneAttack()
    {
        DoAttack();

        yield return new WaitForSeconds(AttFrequency);
//		Debug.Log ("altfre");
		canAttack = true;
    }

	public void AddEnemy(EnsContrrol E)
    {
		if(!CheckInList(E))
        	m_EnemyList.Add(E);
    }
	public void RemoveEnemy(EnsContrrol E)
    {
		if(CheckInList(E))
			m_EnemyList.Remove(E);
    }
	public void DebugList(string s)
	{
		 Debug.Log (s + " " + this.gameObject.name + " " + m_EnemyList.Count);

	}
	public bool CheckInList(EnsContrrol E){
		for (int i = 0; i < m_EnemyList.Count; i++) {
			if (m_EnemyList [i] == E)
				return true;
		}
		return false;
	
	}
}
