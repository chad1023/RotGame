using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStop : MonoBehaviour {
	public int playtime;
	private float time;
	public ObjectPool objectpool;
	ParticleSystem particlesystem;
	// Use this for initialization
	void Start () {
		particlesystem = GetComponent<ParticleSystem> ();
		if (objectpool == null) {
			objectpool = (ObjectPool)FindObjectOfType (typeof(ObjectPool));
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if (particlesystem.isPlaying) {
			time += Time.deltaTime;
		}
		if (time > playtime) {
			particlesystem.Stop ();
			objectpool.particle_Recovery (particlesystem);
			time = 0;
		}
		
	}
}
