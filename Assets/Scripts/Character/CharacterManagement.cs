using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagement : MonoBehaviour {
    public GameObject Character1, Character2;
    public Transform CharacterPos1, CharacterPos2;
    CharacterBase CharacterBase1, CharacterBase2;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetCharactersInit(GameObject C1, GameObject C2)
    {

        Character1 = (GameObject)Instantiate(C1, CharacterPos1.position, Quaternion.identity);
        Character2 = (GameObject)Instantiate(C2, CharacterPos2.position, Quaternion.identity);

        Character1.transform.SetParent(CharacterPos1);
        Character2.transform.SetParent(CharacterPos2);

        CharacterBase1 = Character1.GetComponent<CharacterBase>();
        CharacterBase2 = Character2.GetComponent<CharacterBase>();

        CharacterPos1.GetComponent<TriggerRange>().m_CharacterBase = CharacterBase1;
        CharacterPos2.GetComponent<TriggerRange>().m_CharacterBase = CharacterBase2;
    }
    public void Character1Skill()
    {
        CharacterBase1.DoSkill();
    }
    public void Character2Skill()
    {
        CharacterBase2.DoSkill();
    }
}
