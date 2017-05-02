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
    //紀錄手指觸碰位置
    private Vector2 m_screenPos = new Vector2();

    public GameObject Circle;
    public float RotateSpeed;

    bool isTouch = false;
    Jtouch m_Jtouch;
    void Update()
    {

#if UNITY_EDITOR || UNITY_STANDALONE
        MouseInput();   // 滑鼠偵測
#elif UNITY_ANDROID
		MobileInput();  // 觸碰偵測
#endif
        if (isTouch)
        {
            if (m_Jtouch.x_Distance != 0)
                Circle.transform.Rotate(0, 0, m_Jtouch.x_Distance * RotateSpeed * Time.deltaTime);
            if (m_Jtouch.y_Distance != 0)
                Circle.transform.Rotate(0, 0, -m_Jtouch.y_Distance * RotateSpeed * Time.deltaTime);
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
        /*
        else if (Input.touchCount > 1)
        {

            //記錄兩個手指位置
            Vector2 finger1 = new Vector2();
            Vector2 finger2 = new Vector2();

            //記錄兩個手指移動距離
            Vector2 move1 = new Vector2();
            Vector2 move2 = new Vector2();

            //是否是小於2點觸碰
            for (int i = 0; i < 2; i++)
            {
                UnityEngine.Touch touch = UnityEngine.Input.touches[i];

                if (touch.phase == TouchPhase.Ended)
                    break;

                if (touch.phase == TouchPhase.Moved)
                {
                    //每次都重置
                    float move = 0;

                    //觸碰一點
                    if (i == 0)
                    {
                        finger1 = touch.position;
                        move1 = touch.deltaPosition;
                        //另一點
                    }
                    else
                    {
                        finger2 = touch.position;
                        move2 = touch.deltaPosition;

                        //取最大X
                        if (finger1.x > finger2.x)
                        {
                            move = move1.x;
                        }
                        else
                        {
                            move = move2.x;
                        }

                        //取最大Y，並與取出的X累加
                        if (finger1.y > finger2.y)
                        {
                            move += move1.y;
                        }
                        else
                        {
                            move += move2.y;
                        }

                        //當兩指距離越遠，Z位置加的越多，相反之
                        Camera.main.transform.Translate(0, 0, move * Time.deltaTime);
                    }
                }
            }//end for
        }//end else if 
        */
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
