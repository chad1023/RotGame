﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum GameState
{
	Loading,
	Init,
	Progress,
	Puase,
	Finish
}

public class GameMain : MonoBehaviour {
	[Header("這邊放東西用的")]
	private Clickmanagement clickmanagement;
	public static GameMain _gamemain=null;

	[Header("state")]
	public GameState state=GameState.Init;
	public int gamelevel=0;
	public bool IsInvincible = false;
	public int totaldistance;
	public int speed;
	public int blood;
	public float energy;
	public string shoot;
	public int shootbutton;
	public Vector3 shootpos;
	public AudioSource bgm_manager;
	public Animator animator;
	[SerializeField]
	private int duration;
	public int energyball_num;


	[Header("UI")]
	public Slider durationbar;
	public Image energybar;
	public GameObject start;
	public Text speedtext;
	public Text totaldistancetext;
	public Text gametext;
	public Button[] energypad = new Button [5];
	 
	public Transform[] padpos = new Transform[5];
	[Header("enemy")]
	public List<Item_Encountered>encouter_list;
//	public List<GameObject> enemy_list;




	[Header("unchanged")]
	public int bloodmax;
	public int energyball_max;
	public int enemy_radius;
	public List<GameObject> energyball;
	public LevelManager levelmanager;


	private List<GameObject> clones=new List<GameObject>();

	void Awake(){
		if (_gamemain == null) {
			_gamemain = this;
		} 
		else if (_gamemain != this) {
			Destroy (gameObject);
		}


		clickmanagement = GetComponent<Clickmanagement> ();
		encouter_list.Sort ((x, y) => { return x.e_time.CompareTo(y.e_time); });
		levelmanager = GetComponent<LevelManager> ();


	}

	 public void InitGame(){
		bgm_manager.Stop ();
		LoadLevel ();

		//init state value
		blood = bloodmax;
		duration = 0;
		energy = 0;
		energyball_num = 0;

		//init UI
		durationbar.value = duration;
		energybar.fillAmount = energy;
		foreach (Button pad in energypad)
		{
			foreach (GameObject child in pad.GetComponent<EnergyPad>().enegyball) {
				child.SetActive (false);
			}
			pad.interactable = false;

		}

		//	InvokeRepeating ("EnemyCreate", 0f, 5f);
	
		start.SetActive (false);
		animator.Play ("Go");
	}

	public void StartGame(){
		bgm_manager.Play ();

		state = GameState.Progress;
		totaldistancetext.text = totaldistance.ToString()+"(AU)";



	//	InvokeRepeating ("EnemyCreate", 0f, 5f);
		state = GameState.Progress;
		totaldistancetext.text = totaldistance.ToString()+"(AU)";

		start.SetActive (false);
	}

	public void LoadLevel(){
		LevelData data = levelmanager.getdata (gamelevel);
		
		encouter_list = data.encouter_list;
		//load totaldistance,encounter_list,enemy_list
	}
    
    // Use this for initialization
    void Start () {
		bgm_manager = GetComponent<AudioSource> ();
		animator = GetComponent<Animator> ();



	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.LeftShift)) {
//			if (state == GameState.Progress) {
//				Time.timeScale = 0;
//				state = GameState.Puase;
//			}
//
//			else if (state == GameState.Progress) {
//
//					Time.timeScale = 1;
//					state = GameState.Progress;
//					
//
//			}
//		}


		if (state == GameState.Progress) {
			duration = Mathf.Clamp (duration + (int)(speed * Time.deltaTime), 0, totaldistance);
			if (energyball_num < energyball_max)
				energy = Mathf.Clamp (energy + 0.5f * Time.deltaTime, 0.0f, 1.0f);

			UpdateUI ();
			UpdateGame ();
		} 
		else if (state == GameState.Finish) {
			
		}
	
		if (Input.GetKeyDown (KeyCode.A)) {
			blood = 0;
		}

    }

	void Recovery(){
		foreach (Item_Encountered item in encouter_list) {
			foreach (GameObject enemy in item.type) {
				JObjectPool._InstanceJObjectPool.RecoveryCertainObj (enemy.name);
			}
		}
		foreach (GameObject item in energyball){
			JObjectPool._InstanceJObjectPool.RecoveryCertainObj (item.name);
		}
	
	}

	void UpdateUI(){
		durationbar.value = (float)duration/totaldistance;
		energybar.fillAmount = energy;
		speedtext.text = speed.ToString()+"(AU)";
	}

	void UpdateGame(){
		if (duration == totaldistance) {
			GameFinish ();
			animator.Play ("Through");
		}
		if (blood == 0) {
			GameOver ();	
		}
		if ((energy >= 1)&&(energyball_num<energyball_max)) {
			EnergyCharged ();
			if (energyball_num<energyball_max)
				energy = 0;



				
		}

		if(!IsInvincible)
		{
			foreach (Item_Encountered item in encouter_list)
			{
				if (!item.encoutered) {
					if ((Mathf.Abs(duration - item.e_time) < Time.deltaTime * speed)||(duration>item.e_time)) {
						print ("Encouter " + item.PrefabName);
						//call invoke
						StartCoroutine(EnemyCreate(item));
						item.encoutered = true;
				
					}
				}
			
			}
		}
	}
	void GameFinish(){
		state = GameState.Finish; 
		shoot="";
		shootbutton = 0;
		shootpos = Vector3.zero;
		CancelInvoke ();

	}

	void GameClear(){
		gametext.text = "Clear!";

		Recovery ();
		gamelevel += 1;
		if (gamelevel >= levelmanager.maxlevel) {
			gamelevel -= 1;
		}
		start.SetActive (true);


	}
	void GameOver(){
		gametext.text = "Game Over";
		Recovery ();
		start.SetActive (true);

	}
	void EnergyCharged(){
		Random.seed = System.Guid.NewGuid().GetHashCode();
		int i=Random.Range (0, 5);

		for(int j=0;j<energypad.Length;j++) {
			if (!energypad[j].interactable) {
				energypad[j].interactable = true;
				energypad[j].onClick.AddListener (() => SetClickItem(energyball[i].name,j));
				GameObject enegryball = energypad [j].GetComponent<EnergyPad> ().enegyball[i];
				enegryball.SetActive (true);
				energyball_num++;
				break;
			}
		}
	//	ObjectPool.GetGameObject ();

	}

	public void EnergyShoot(){
		shoot = null;
		energypad [shootbutton].onClick.RemoveAllListeners ();
		energypad [shootbutton].interactable = false;
		foreach (GameObject item in energypad[shootbutton].GetComponent<EnergyPad> ().enegyball) {
			item.SetActive (false);
		}

		if (energyball_num == energyball_max)
			energy = 0;
		energyball_num--;
	}

	public void BloodDecrease(){
		blood--;
	
	}
	public void BloodIncrease(){
		blood++;
	}

	IEnumerator EnemyCreate(Item_Encountered enemy){
		if (!IsInvincible) {
			Random.seed = System.Guid.NewGuid ().GetHashCode ();
			int i = Random.Range (0, enemy.type.Count);

			Vector2 project = (Random.insideUnitCircle).normalized*enemy_radius;
			Vector3 pos = new Vector3 (project.x, project.y, 0);
			GameObject enemy_clone = GetGameObject (enemy.type[i], pos);
			enemy_clone.GetComponent<FlyingObjControl> ().MoveSpeed = 5;
			yield return new WaitForSeconds(5);
			enemy.encoutered = false;
		}
	}
	void SetClickItem(string s,int button_i){
		shoot = s;
		shootbutton = button_i;
		shootpos = padpos [button_i].position;

	}

	GameObject GetGameObject(GameObject prefab,Vector3 pos){
	
		return JObjectPool._InstanceJObjectPool.GetGameObject (prefab.name,pos);
	}








}
