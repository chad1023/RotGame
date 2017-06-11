using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllAnimator : MonoBehaviour {
	private Animator m_Animator;
	private GameMain m_GameMain;
	// Use this for initialization
	void Start () {
		m_Animator=GetComponent<Animator>();
		m_GameMain=GameObject.Find("GameMain").GetComponent<GameMain>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void initgame()
	{
		m_GameMain.InitGame();
	}
	public void PlayGo()
	{
		m_GameMain.start.SetActive(false);
		m_Animator.Play("Go");

	}
}
