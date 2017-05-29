using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameMain : MonoBehaviour {
	[Header("這邊放東西用的")]
	public GameObject[] EnsPrefabs;
	public Transform[] InsPositions;
	public static ObjectPool objectpool=null;
	public float boss_blood = 100;
	public float char_blood = 10;
	public Text boss_text;
	public Image Boss_img;
	public Text char_text;
	public Image char_img;
	public Text finish_text;

    [Header("DOShake parameter")]
    public GameObject boss;
    public float BossShakeDuration;
    public float BossShakeStrenth;
    [Space]
    public CameraScript _CameraScript;

	int r_Ens,r_Pos;
	float r_Time = 1;//下次產生的時間，預設第一波5秒開始
	float r_Force;
    bool canIns = true;
    bool isEnd = false;

	float init_boss_blood;
	float init_char_blood;

    
    // Use this for initialization
    void Start () {
       
        objectpool = GetComponent<ObjectPool> ();

		init_boss_blood = boss_blood;
		init_char_blood = char_blood;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Reset ();
		}
		if (canIns) {
			StartCoroutine (Ins (r_Time));
		}
		if (boss_blood >= 0) {
			boss_text.text = boss_blood.ToString ();
			Boss_img.fillAmount = (float)(boss_blood / init_boss_blood);
		}
		if (char_blood >= 0) {
			char_text.text = char_blood.ToString ();
			char_img.fillAmount=(float)(char_blood / init_char_blood);
		}
		if (boss_blood == 0 && !isEnd) {
            GameEnd("Boss dead");
		}
		else if (char_blood==0 && !isEnd)
        {
            GameEnd("You dead");
            
        }
        //測試用
        if (Input.GetKeyDown(KeyCode.Z))
        {
            boss.transform.DOShakePosition(BossShakeDuration, BossShakeStrenth);
        }
    }

	IEnumerator Ins(float t)
	{
		canIns = false;
		r_Ens = (int)Random.Range (0, 2);
		r_Pos = (int)Random.Range (3, InsPositions.Length);
		r_Force = Random.Range (20, 40);
		//GameObject temp= (GameObject)Instantiate (EnsPrefabs [r_Ens], InsPositions [r_Pos].position, Quaternion.identity);
		yield return new WaitForSeconds (t);
		GameObject temp= objectpool.Reuse(EnsPrefabs [r_Ens],InsPositions [r_Pos-1].position, Quaternion.identity);
		//temp.GetComponent<EnsContrrol> ().force = r_Force;


		r_Time = Random.Range (1, 2);
		canIns = true;

	}
	public void AttackBoss(int i){
        boss.transform.DOShakePosition(BossShakeDuration, BossShakeStrenth);
        boss_blood -= i;
		if (boss_blood < 0)
			boss_blood = 0;
		
	}
	public void AttackChar(){
		char_blood--;
        _CameraScript.Shake();

        if (char_blood < 0)
			char_blood = 0;
	}
	public void Reset()
	{
		
		boss_blood = init_boss_blood;
		char_blood = init_char_blood;
		objectpool.Reset ();
		r_Time = Random.Range (0, 1);
        canIns = true;
        objectpool.RunAllEns();
        isEnd = false;
        finish_text.gameObject.SetActive(false);

    }
    void GameEnd(string s)
    {
        StopAllCoroutines();
        canIns = false;
        objectpool.StopAllEns();
        isEnd = true;
        finish_text.gameObject.SetActive(true);
        finish_text.text = s;

    }

   
}
