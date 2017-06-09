using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//手勢定義


public class Jtouch
{
	public GameEnum.Direction Direction ;
    public float x_Distance ;//正的話是向左滑
    public float y_Distance ;//正的話是向下滑

}
public class CircleController_2 : MonoBehaviour
{
    [Header("圓環設定")]
    public GameObject Circle;
    public GameObject[] InnerCircle;
    public float CircleSelfRotateSpeed;
    [Space]
    [Header("操控設定")]
    public float RotateSpeed;
    public float RotateLastTime;
    public bool RotateLimit = true;



    //Private patameter 
    private float touchlasttime;
    //紀錄手指觸碰位置
    private Vector2 m_screenPos = new Vector2();
    bool isTouch = false;
    Jtouch m_Jtouch;
    void Update()
    {
		MouseInput(); 
//#if UNITY_EDITOR || UNITY_STANDALONE
//        MouseInput();   // 滑鼠偵測
//#elif UNITY_ANDROID
//		MobileInput();  // 觸碰偵測
//#endif
        if (isTouch)
        {
            if(RotateLimit)
                touchlasttime += Time.deltaTime;
            if (touchlasttime <= RotateLastTime)
            {
                if (m_Jtouch.x_Distance != 0)
                    Circle.transform.Rotate(0, 0, m_Jtouch.x_Distance * RotateSpeed * Time.deltaTime);
                if (m_Jtouch.y_Distance != 0)
                    Circle.transform.Rotate(0, 0, -m_Jtouch.y_Distance * RotateSpeed * Time.deltaTime);
            }
            else
                isTouch = false;
        }

    }

/// <summary>
/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
/// </summary>
    void FixedUpdate()
    {
        if(!isTouch){
            for (int i = 0; i < InnerCircle.Length; i++)
            {
                if(i % 2== 0)
                {
                    InnerCircle[i].transform.Rotate(0,0,CircleSelfRotateSpeed*Time.deltaTime);
                }
                else
                    InnerCircle[i].transform.Rotate(0,0,-CircleSelfRotateSpeed*Time.deltaTime);

            }

        }
    }
    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            isTouch = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_Jtouch = new Jtouch();
            isTouch = false;
            touchlasttime = 0;
        //    Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //    gDefine.Direction mDirection = HandDirection(m_screenPos, pos);
        //    Debug.Log("mDirection: " + mDirection.ToString());
        }
        if (Input.GetMouseButton(0)) {
            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            m_Jtouch = HandDirection(m_screenPos, pos);
            m_screenPos = pos;
        }
    }
    
    void MobileInput()
    {
        if (Input.touchCount <= 0)
            return;

        //1個手指觸碰螢幕
        if (Input.touchCount == 1)
        {

            //開始觸碰
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                Debug.Log("Began");
                //紀錄觸碰位置
                m_screenPos = Input.touches[0].position;

                //手指移動
            }
            else if (Input.touches[0].phase == TouchPhase.Moved)
            {
                Debug.Log("Moved");
                //移動攝影機
                //Camera.main.transform.Translate (new Vector3 (-Input.touches [0].deltaPosition.x * Time.deltaTime, -Input.touches [0].deltaPosition.y * Time.deltaTime, 0));
            }


            //手指離開螢幕
            if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Debug.Log("Ended");
                Vector2 pos = Input.touches[0].position;

                //gDefine.Direction mDirection = HandDirection(m_screenPos, pos);
                //Debug.Log("mDirection: " + mDirection.ToString());
            }
            //攝影機縮放，如果1個手指以上觸碰螢幕
        }
        
    }//end void

    /*gDefine.Direction*/
    Jtouch HandDirection(Vector2 StartPos, Vector2 EndPos)
    {
        //gDefine.Direction mDirection;
        Jtouch mJtouch = new Jtouch();

		//print (StartPos+" "+Screen.width);
        //手指水平移動
        if (Mathf.Abs(StartPos.x - EndPos.x) > Mathf.Abs(StartPos.y - EndPos.y))
        {
            if (StartPos.x > EndPos.x)
            {
                //手指向左滑動
                //mDirection = gDefine.Direction.Left;
				mJtouch.Direction = GameEnum.Direction.Left;
                
            }
            else
            {
                //手指向右滑動
                //mDirection = gDefine.Direction.Right;
				mJtouch.Direction = GameEnum.Direction.Right;
            }
			if (StartPos.y > Screen.height/2)
				mJtouch.x_Distance = StartPos.x - EndPos.x;
			
			else
				mJtouch.x_Distance = (StartPos.x - EndPos.x) * -1;
			

            mJtouch.y_Distance = 0;
        }
        else
        {
            if (m_screenPos.y > EndPos.y)
            {
                //手指向下滑動
                //mDirection = gDefine.Direction.Down;
				mJtouch.Direction = GameEnum.Direction.Down;
            }
            else
            {
                //手指向上滑動
                //mDirection = gDefine.Direction.Up;
				mJtouch.Direction = GameEnum.Direction.Up;
            }
            mJtouch.x_Distance = 0;
			if (StartPos.x > Screen.width / 2)
				mJtouch.y_Distance = StartPos.y - EndPos.y;
			else
				mJtouch.y_Distance = (StartPos.y - EndPos.y) * -1;
        }
        return mJtouch;
        //return mDirection;
    }
}
