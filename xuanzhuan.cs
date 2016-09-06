using UnityEngine;
using System.Collections;

public class xuanzhuan : MonoBehaviour {

    //观察目标   
    public Transform Target;
    //观察距离   
    public float Distance = 18f;
    //旋转速度   
    private float SpeedX = 240;
    private float SpeedY = 120;
    //角度限制   
    private float MinLimitY = 60;
    private float MaxLimitY = 90;

    //旋转角度   
    private float mX = 0.0f;
    private float mY = 0.0f;

    //鼠标缩放距离最值   
    private float MaxDistance = 25f;
    private float MinDistance = 10f;
    //鼠标缩放速率   
    private float ZoomSpeed = 2f;
    //是否启用差值   
    public bool isNeedDamping = true;
    //速度   
    public float Damping = 2.5f;

    //运行平台
    public static string PLATFORM = "PC";

    //触屏
    private Vector2 m_screenpos;
    private float curDist = 0f;
    private float lastDist = 0f;

    void Awake() {
#if UNITY_ANDROID
            print("这里是安卓设备^_^");
        PLATFORM="Android";
#endif

#if UNITY_IPHONE
        print("这里是苹果设备>_<");
        PLATFORM = "IOS";
#endif

#if UNITY_STANDALONE_WIN
        print("我是从Windows的电脑上运行的T_T");
        PLATFORM = "PC";
#endif
    }
    void Start()
    {
        Input.multiTouchEnabled = true;
        mX = transform.eulerAngles.x;
        mY = transform.eulerAngles.y;
        mY = clampAngle(mY, MinLimitY, MaxLimitY);
    }
    void Update()
    {
        if (PLATFORM == "PC")
        {
            if (Input.GetMouseButton(1) && Target != null)
            {
                mX = mX + Input.GetAxis("Mouse X") * SpeedX * 0.02f;
                mY = mY + Input.GetAxis("Mouse Y") * SpeedY * 0.02f;
                mY = clampAngle(mY, MinLimitY, MaxLimitY);
            }
            Distance = Distance - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
        }
        else if (PLATFORM == "Android")
        {
            if (Input.touchCount == 0)
            {
                //return;
            }
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)//开始触屏
                {
                    m_screenpos = Input.touches[0].position;//自定义的二维坐标向量 记录初始触屏位置
                }
                else if (Input.touches[0].phase == TouchPhase.Moved)//手指移动
                {
                    Vector2 movePos = Input.touches[0].deltaPosition;
                    mX = mX + movePos.x * 50 * 0.02f;
                    mY = mY + movePos.y * 50 * 0.02f;
                    mY = clampAngle(mY, MinLimitY, MaxLimitY);
                }
            }
            if (Input.touchCount == 2)
            {
                //记录两个手指的位置         
                Vector2 finger1 = new Vector2();
                Vector2 finger2 = new Vector2();
                //记录两个手指的移动距离 
                Vector2 mov1 = new Vector2();
                Vector2 mov2 = new Vector2();

                for (int i = 0; i < 2; i++)//用循环来实现记录position
                {
                    Touch touch = Input.touches[i];//记录第0个、第1个触屏点的状态
                    if (touch.phase == TouchPhase.Ended) break; //如果手指触屏之后离开就break
                    if (touch.phase == TouchPhase.Moved)// 当手指移动时
                    {
                        float mov = 0; // 用来记录移动增量
                        if (i == 0)
                        {
                            finger1 = touch.position;
                            mov1 = touch.deltaPosition;
                        }
                        else
                        {
                            finger2 = touch.position;
                            mov2 = touch.deltaPosition;
                            curDist = Vector2.Distance(finger1, finger2);

                            if (curDist > lastDist)

                            {
                                mov = Vector2.Distance(mov1, mov2);
                            }
                            else
                            {
                                mov = -Vector2.Distance(mov1, mov2);
                            }
                            Distance = Distance - mov * ZoomSpeed;
                            lastDist = curDist;
                        }
                    }
                }
            }
        }

        Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        print("distance" + Distance);
        Quaternion mRotation = Quaternion.Euler(mY, mX, 0);
        Vector3 mPosition = mRotation * new Vector3(0f, 1f, -Distance) + Target.position;

        //设置相机的角度和位置 
        if(Target != null)
        {
            if (isNeedDamping)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);
                transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
            }
            else
            {
                transform.rotation = mRotation;
                transform.position = mPosition;
            }
        }
}

    private float clampAngle(float angle,float min,float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
