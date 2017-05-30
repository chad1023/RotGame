using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBallControl : MonoBehaviour {
    public GameEnum.Type_Color TypeColor;
    public float SelfLastTime;
    [Range(0,0.8f)]
    public float s;

    public CircleParmeter _CircleParmeter;
    Animator m_Animator;
    float lasttime;
	// Use this for initialization
	void Start () {
        m_Animator = GetComponent<Animator>();
        Init();
    }
	
	// Update is called once per frame
	void Update () {
        if(_CircleParmeter==null){
            _CircleParmeter=transform.GetComponentInParent<CircleParmeter>();

        }

        lasttime += Time.deltaTime;
        s = 0.8f - (lasttime / SelfLastTime) * 0.8f;
        transform.localScale = new Vector3(s, s, s);
        if(lasttime>=SelfLastTime)
        {
            _CircleParmeter.LimitNumber++;
            Init();
            JObjectPool._InstanceJObjectPool.Recovery(this.gameObject,Vector3.zero);
        }
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
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else{
            _CircleParmeter.LimitNumber++;
            Init();
            JObjectPool._InstanceJObjectPool.Recovery(this.gameObject,Vector3.zero);
        }
    }
    public void Init()
    {
        lasttime = 0;
        _CircleParmeter=null;
    }
}
