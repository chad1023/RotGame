using System.Collections;
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
	public static int gamelevel;
	public bool IsInvincible = false;
	public int totaldistance;
	public int speed;
	public int blood;
	public float energy;
	public string shoot;
	public int shootbutton;
	public AudioSource bgm_manager;
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
	 
	[Header("enemy")]
	public List<Item_Encountered>encouter_list;
	public List<GameObject> enemy_list;




	[Header("unchanged")]
	public int bloodmax;
	public int energyball_max;
	public int enemy_radius;
	public List<GameObject> energyball;

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


	}

	public void InitGame(){
		bgm_manager.Play ();

		foreach (GameObject clone in clones) {
			Destroy (clone);
		}
		clones.Clear ();

		blood = bloodmax;
		duration = 0;
		energy = 0;
		energyball_num = 0;
		foreach (Button pad in energypad)
		{
			foreach (GameObject child in pad.GetComponent<EnergyPad>().enegyball) {
				child.SetActive (false);
			}
			pad.interactable = false;
		
		}

		InvokeRepeating ("EnemyCreate", 0f, 5f);
		state = GameState.Progress;
		totaldistancetext.text = totaldistance.ToString()+"(AU)";

		start.SetActive (false);
	}

	public void LoadLevel(){

		//load totaldistance,encounter_list,enemy_list
	}
    
    // Use this for initialization
    void Start () {
		bgm_manager = GetComponent<AudioSource> ();




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
		foreach (GameObject item in enemy_list) {
			JObjectPool._InstanceJObjectPool.RecoveryCertainObj (item.name);
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
			GameClear ();
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
		CancelInvoke ();
		Recovery ();
	
	}

	void GameClear(){
		GameFinish ();
		gametext.text = "Clear!";
	
		start.SetActive (true);


	}
	void GameOver(){
		GameFinish ();
		gametext.text = "Game Over";

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
			GameObject enemy_clone = GetGameObject (enemy.Prefab, pos);
			enemy_clone.GetComponent<FlyingObjControl> ().MoveSpeed = 5;
			yield return new WaitForSeconds(2);
			enemy.encoutered = false;
		}
	}
	void SetClickItem(string s,int button_i){
		shoot = s;
		shootbutton = button_i;

	}

	GameObject GetGameObject(GameObject prefab,Vector3 pos){
	
		return JObjectPool._InstanceJObjectPool.GetGameObject (prefab.name,pos);
	}








}
