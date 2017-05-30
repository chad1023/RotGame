using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBallControl : MonoBehaviour {
    public GameEnum.Type_Color TypeColor;
    public float SelfLastTime;
    [Range(0,0.8f)]
    public float s;
    Animator m_Animator;
    float lasttime;
	// Use this for initialization
	void Start () {
        m_Animator = GetComponent<Animator>();
        Init();
    }
	
	// Update is called once per frame
	void Update () {
        lasttime += Time.deltaTime;
        s = 0.8f - (lasttime / SelfLastTime) * 0.8f;
        transform.localScale = new Vector3(s, s, s);

    }
    void OnTriggerEnter(Collider other)
    {
        FlyingObjControl _FlyingObjControl = other.GetComponent<FlyingObjControl>();
        if (_FlyingObjControl)
        {
            bool isSame = _FlyingObjControl.TypeColor == TypeColor;
            _FlyingObjControl.GetHit(isSame);
            GetHit(isSame);
        }
    }
    void GetHit(bool b)
    {
        if (b)
        {
            lasttime = 0;
        }
    }
    public void Init()
    {
        lasttime = 0;
    }
}
