using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
	public GameObject []ens;
	public ParticleSystem[] particle;
	public int ini_ens=20;
	public int ini_particle=5;
	[SerializeField]
	public Queue<GameObject>[] ens_pool;
	public Queue<ParticleSystem>[] particle_pool;
	public Vector3 pool_location;

    List<GameObject> _Enslist;


    void Awake(){
        _Enslist = new List<GameObject>();
        ens_pool = new Queue<GameObject>[ens.Length];
		particle_pool = new Queue<ParticleSystem>[ens.Length];
		for(int i=0; i<ens.Length;i++) {
			ens_pool [i] = new Queue<GameObject> ();
			particle_pool [i] = new Queue<ParticleSystem> ();

			for (int j = 0; j < ini_ens; j++) {
				GameObject ens_preb = Instantiate (ens[i],pool_location,ens[i].transform.rotation) as GameObject;
				ens_pool[i].Enqueue (ens_preb);
				ens_preb.SetActive (false);
                _Enslist.Add(ens_preb);

            }

			for (int j = 0; j < ini_particle; j++) {
				ParticleSystem particle_preb = Instantiate (particle[i],pool_location,particle[i].transform.rotation) as ParticleSystem;
				particle_preb.Stop ();
				particle_pool[i].Enqueue (particle_preb);
			}
		}


	}

	//function for instatiate ens from pool 
	public GameObject Reuse(GameObject item,Vector3 position,Quaternion rotation){
		int i = 0;
		for (i=0;i<ens.Length;i++)
		{
			if(GameObject.ReferenceEquals(ens[i],item))
			{
				break;
			}
		}
		if (ens_pool[i].Count > 0) {
			GameObject reuse = ens_pool[i].Dequeue ();
			reuse.transform.position = position;
			reuse.transform.rotation = rotation;
			reuse.SetActive (true);
			return reuse;
		} 
		else 
		{
			Debug.Log ("pool empty");
			return null;

		}
	}

	//function for disalbe ens and put into pool for recovery
	public void Recovery(GameObject recovery){
		int i = 0;
		//Debug.Log ("ens:"+ens.Length);
		//Debug.Log ("enspool:"+ens_pool.Length);

		for (i=0;i<ens.Length;i++)
		{
			if(ens[i].name==recovery.name.Replace("(Clone)",""))
			{
				break;
			}
		}
        /*
		//die particle from pool
		if(particle_pool[i].Count>0)
		{
			ParticleSystem p_recover = particle_pool [i].Dequeue ();
			p_recover.transform.position = recovery.transform.position;
			p_recover.transform.rotation = recovery.transform.rotation;
			p_recover.Play ();
		}
        */
		ens_pool[i].Enqueue (recovery);
		recovery.SetActive (false);
	
	}
	public void particle_Reuse(int i,Vector3 position,Quaternion rotation){//i for index
		if(particle_pool[i].Count>0)
		{
			ParticleSystem p_recover = particle_pool [i].Dequeue ();
			p_recover.transform.position = position;
			p_recover.transform.rotation = rotation;
			p_recover.Play ();
		}
	}
	public void particle_Recovery(ParticleSystem recovery){
		for (int i=0;i<ens.Length;i++)
		{
			if(particle[i].name==recovery.name.Replace("(Clone)",""))
			{
				particle_pool [i].Enqueue (recovery);
				break;
			}
		}
	}

	public void Reset(){
		for(int i=0; i<ens.Length;i++) {
			ens_pool [i].Clear ();
			particle_pool [i].Clear ();
			ens_pool [i] = new Queue<GameObject> ();
			particle_pool [i] = new Queue<ParticleSystem> ();

			for (int j = 0; j < ini_ens; j++) {
				GameObject ens_preb = Instantiate (ens[i],pool_location,ens[i].transform.rotation) as GameObject;
				ens_pool[i].Enqueue (ens_preb);
				ens_preb.SetActive (false);
			}

			for (int j = 0; j < ini_particle; j++) {
				ParticleSystem particle_preb = Instantiate (particle[i],pool_location,particle[i].transform.rotation) as ParticleSystem;
				particle_pool[i].Enqueue (particle_preb);
			}
		}
	
	}

	void OnDisabled(){
		for (int i = 0; i < ens.Length; i++) {
			ens_pool [i].Clear ();
			particle_pool [i].Clear ();
		}
	}

    private void Update()
    {
        
    }
    /// <summary>
    //停止敵人移動
    /// </summary>
    public void StopAllEns()
    {
        for (int i = 0; i < _Enslist.Count; i++)
        {
            _Enslist[i].GetComponent<EnsContrrol>().isStop = true;
        }
    }
    public void RunAllEns()
    {
        for (int i = 0; i < _Enslist.Count; i++)
        {
            _Enslist[i].GetComponent<EnsContrrol>().isStop = false;
        }
    }

}
