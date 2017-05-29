using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjControl : MonoBehaviour {
    public GameEnum.Type_Color TypeColor;

    Animator m_Animator;
    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
		
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
