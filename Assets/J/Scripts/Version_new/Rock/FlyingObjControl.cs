using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjControl : MonoBehaviour {
    public GameEnum.Type_Color TypeColor;
    public float MoveSpeed;

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
}
