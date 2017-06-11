using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class RocketControl : MonoBehaviour {

	[Header("Rocket shake parameter")]
    public float RocketShakeDuration;
    public float RocketShakeStrenth;

	[Space]
	private string exploname="ExploEffect";
	private GameMain gamemain;
	AudioSource exploaudio;
	// Use this for initialization
	void Start () {
		exploaudio = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        FlyingObjControl _FlyingObjControl = other.GetComponent<FlyingObjControl>();
        if (_FlyingObjControl) { 
			_FlyingObjControl.DestoryByHit();
			JObjectPool._InstanceJObjectPool.Recovery(_FlyingObjControl.gameObject);
			GameMain._gamemain.BloodDecrease ();
			exploaudio.Play ();

		}
    }

}
