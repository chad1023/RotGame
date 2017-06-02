using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjControl : MonoBehaviour {
    public GameEnum.Type_Color TypeColor;
    public float MoveSpeed;


    private string exploname="ExploEffect";
    private bool isExplo=false;
    private bool isFly=true;
    private Collider m_Collider;
	private GameObject Target;
    private Vector3 Dir;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
		Target=GameObject.Find("Rocket");
        m_Collider=GetComponent<Collider>();
        m_Rigidbody=GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		Dir = Target.transform.position - transform.position;
        
	}

    
    void FixedUpdate()
    {
        if (isFly)
            m_Rigidbody.velocity = Dir.normalized * MoveSpeed * 0.1f;
        else
            m_Rigidbody.velocity = Vector3.zero;
    }
    public void GetHit(bool b)
    {
        if (b)
            DisappearSelf();
        else
            DestroySelf();

        // call the staic JObjectPool instance ,then recovery this
        JObjectPool._InstanceJObjectPool.Recovery(this.gameObject, Vector3.zero);
    }

    
    /// <summary>
    /// if Color is the same ,then disappear-self ,go play the Disappear animation
    /// </summary>
    void DisappearSelf()
    {

        
    }

    /// <summary>
    /// if Color is not the same ,then destroy-self ,go play the Destroy animation
    /// </summary>
    void DestroySelf()
    {

    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        isExplo=false;   
    }
    /// <summary>
    /// HIt and destory itself than
    /// </summary>
    public void DestoryByHit()
    {
        if(!isExplo){
            isExplo=true;
            GameObject tempExplo= JObjectPool._InstanceJObjectPool.GetGameObject(exploname,transform.position);
			JObjectPool._InstanceJObjectPool.DelayRecovery(tempExplo,1.5f);
        }

    }
}
