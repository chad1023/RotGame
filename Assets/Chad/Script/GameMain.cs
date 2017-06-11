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
	public int gamelevel=0;
	public bool IsInvincible = false;
	public int totaldistance;
	public int speed;
	public int blood;
	public float energy;
	public string shoot;
	public int shootbutton;

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
	public Image marker;
	public Canvas canvas;
	public SpriteRenderer planet_go;
	public SpriteRenderer planet_arrive;
	 

	[Header("enemy")]
	public List<Item_Encountered>enemy_list;
	public List<planet>planet_list;
	public List<GameObject> tem_list;



	[Header("unchanged")]
	public int bloodmax;
	public int energyball_max;
	public int enemy_radius;
	public List<GameObject> energyball;
	public LevelManager levelmanager;
	public RectTransform markerstart;
	public RectTransform markerend;

	private List<GameObject> clones=new List<GameObject>();

	void Awake(){
		if (_gamemain == null) {
			_gamemain = this;
		} 
		else if (_gamemain != this) {
			Destroy (gameObject);
		}


		clickmanagement = GetComponent<Clickmanagement> ();


		levelmanager = GetComponent<LevelManager> ();


	}

	 public void InitGame(){
		

		bgm_manager.Stop ();
		foreach (GameObject item in tem_list) {
			Destroy (item);
		}
		tem_list.Clear ();
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
		
		enemy_list = data.enemy_list;
		enemy_list.Sort ((x, y) => { return x.e_time.CompareTo(y.e_time); });
		planet_go.sprite = data.planet_go;
		planet_arrive.sprite = data.planet_arrive;


		planet_list = data.planet_list;
		planet_list.Sort ((x, y) => { return x.e_time.CompareTo(y.e_time); });


		totaldistance = data.totaldistance;

		foreach (planet item in planet_list) {
			float rate = item.e_time/(float)totaldistance;
		
			Image marker_clone =(Image)Instantiate (marker, markerstart.transform);
		
			marker_clone.transform.position = rate*(markerend.transform.position-markerstart.transform.position)+markerstart.transform.position;
			tem_list.Add (marker_clone.gameObject);

		}



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
		foreach (Item_Encountered item in enemy_list) {
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
		foreach (planet item in planet_list) {
			if (!item.encoutered) {
				if(Mathf.Abs(duration - item.e_time) < Time.deltaTime * speed){
					print ("Encouter planet: " + item.prefab.name);
					//call invoke
					GameObject tem = (GameObject)Instantiate(item.prefab,item.pos,Quaternion.identity);
					item.encoutered = true;
					tem_list.Add (tem);
				}
				
			}
		
		}

		if(!IsInvincible)
		{
			foreach (Item_Encountered item in enemy_list)
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
			float t = Random.Range (0, 1f);
			Vector2 project = (Random.insideUnitCircle).normalized*enemy_radius;
			Vector3 pos = new Vector3 (project.x, project.y, 0);
			GameObject enemy_clone = GetGameObject (enemy.type[i], pos);
			enemy_clone.GetComponent<FlyingObjControl> ().MoveSpeed = 5;
			yield return new WaitForSeconds(enemy.period-t);
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
