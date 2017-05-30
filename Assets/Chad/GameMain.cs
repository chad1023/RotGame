using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameMain : MonoBehaviour {
	[Header("這邊放東西用的")]
	private Clickmanagement clickmanagement;

	[Header("UI")]
	public Slider durationbar;
	public Image energybar;

	public Text speedtext;
	public Text totaldistancetext;
	public Text gametext;
	public Button[] energypad = new Button [5];
	 


	public int totaldistance;
	public int speed;
	public int blood;
	public float energy;
	public bool IsInvincible = false;

	public string shoot;
	public int shootbutton;

	public int energyball_num;
	public int energyball_max;
	public int enemy_radius;


	[SerializeField]
	private int duration;

	public List<Item_Encountered>encouter_list;
	public List<GameObject> enemy_list;
	public List<GameObject> energyball;






	void Awake(){
		clickmanagement = GetComponent<Clickmanagement> ();
		encouter_list.Sort ((x, y) => { return x.e_time.CompareTo(y.e_time); });

	}

    
    // Use this for initialization
    void Start () {

		totaldistancetext.text = totaldistance.ToString()+"(AU)";
		InvokeRepeating ("EnemyCreate",0f, 2f);


	}
	
	// Update is called once per frame
	void Update () {

		duration= Mathf.Clamp (duration+(int)(speed * Time.deltaTime), 0, totaldistance);
		if(energyball_num<energyball_max)
			energy =Mathf.Clamp(energy+0.5f*Time.deltaTime,0.0f,1.0f);


		UpdateUI ();
		UpdateGame ();
		
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
				if (duration == item.e_time) {
					print ("Encouter "+item.PrefabName);
				
				}
			
			}
		}
	}
	void GameClear(){
		gametext.text = "Clear!";
	}
	void GameOver(){
		gametext.text = "Game Over";
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

		print(i);
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

	void BloodDecrease(){
		blood--;
	
	}
	void BloodIncrease(){
		blood++;
	}

	void EnemyCreate(){
		if (!IsInvincible) {
			Random.seed = System.Guid.NewGuid ().GetHashCode ();
			int i = Random.Range (0, enemy_list.Count);
			Vector2 project = Random.insideUnitCircle*enemy_radius;
			Vector3 pos = new Vector3 (project.x, project.y, 0);
			GameObject enemy = GetGameObject (enemy_list [i], pos);
			enemy.GetComponent<FlyingObjControl> ().MoveSpeed = 5;
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
