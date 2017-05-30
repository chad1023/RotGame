using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameMain : MonoBehaviour {
	[Header("這邊放東西用的")]
	private JObjectPool ObjectPool;

	[Header("UI")]
	public Slider durationbar;
	public Slider energybar;

	public Text distancetext;
	public Text totaldistancetext;
	public Text gametext;


	public int totaldistance;
	public int speed;
	public int blood;
	public float energy;
	public bool IsInvincible = false;

	public int energyball_num;
	public int energyball_max;


	[SerializeField]
	private int duration;

	public List<Item_Encountered>encouter_list;
	public List<GameObject> enemy_list;






	void Awake(){
		ObjectPool = JObjectPool._InstanceJObjectPool;

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
		energy += Time.deltaTime;

		UpdateUI ();
		UpdateGame ();
		
    }


	void UpdateUI(){
		durationbar.value = (float)duration/totaldistance;
		energybar.value = energy;
		distancetext.text = duration.ToString()+"(AU)";
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
		energyball_num++;
	//	ObjectPool.GetGameObject ();
		print(i);
	}

	void EnergyShoot(){
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
			print ("enemy" + i + "create");
		}
	}








}
