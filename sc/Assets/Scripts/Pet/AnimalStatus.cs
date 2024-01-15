using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalStatus : MonoBehaviour
{
    Animator anim;
    //每次转速
    int sky = -300;

    //上次移动是否完成
    bool StateMotion = true;
    //上次旋转是否完成
    bool AngleMotion = true;

    //是否休息
    bool WhetherRest = false;
    //休息间隔
    public float timeInterval = 20f;
    float timerInterval = 0f;
    //休息时间
    public float timeRest = 10f;
    float timerRest = 0f;
    bool bRest = false;
    //行走速度
    public float MoveSpeed = 3.0f;
    //奔跑速度
    public float RunSpeed = 6.0f;

    //确定移动次数
    int Distance;
    //确定移动状态：1/走 0/跑
    int c = 1;

    PetItem petItem = null;

    bool bInLeftArea = false;
    bool bInRightArea = false;
    bool bInTopArea = false;
    bool bInBottomArea = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        petItem = GetComponent<PetItem>();


        RandomTimeIntervalFun();
        RandomTimeRestFun();


        /*        public void RandomTimeIntervalFun()
                {
                    timeInterval = Random.Range(10f, 20f);
                }

                public void RandomTimeRestFun()
                {
                    timeRest = Random.Range(5f, 10f);
                }*/
    }

    // Update is called once per frame
    void Update()
    {
        //锁定时处于待机动画不移动
        if (PetSpanManager.Instance().bLookAting)
        {
            StopAnimation();
            anim.SetBool("ToRest", true);
            return;
        }

        if (petItem == null)
            return;
        PetModelRecData petModelRecData = petItem.GetPetData();
        if (petModelRecData == null)
            return;

        //盒子变宠物时起名状态时不移动
        bool bBox = PetSpanManager.Instance().IsCreateNameBox(petItem);
        if (bBox)
        {
            StopAnimation();
            anim.SetBool("ToRest", true);
            return;
        }

        //状态是否良好
        int key = PetSpanManager.Instance().GetPetModelPos(petModelRecData.id);
        ManageMentClass.DataManagerClass.petModelRecData.TryGetValue(key, out petModelRecData);
        if (petModelRecData == null)
            return;
        bool bHealth = PetSpanManager.Instance().IsHealth(petModelRecData);
        if (!bHealth)
            return;

        //训练中
        bool bTrain = PetSpanManager.Instance().IsTraining(petModelRecData.id);
        if (bTrain)
        {
            StopAnimation();
            return;
        }
        //睡眠中
        bool bSleep = petModelRecData.sleep_status == (int)PetSleepStatus.Sleep;
        if (bSleep)
        {
            StopAnimation();
            PetSpanManager.Instance().PlayPetAni(petModelRecData.id, (int)PetStateAnimationType.Sleep);
            return;
        }

        if (!PetSpanManager.Instance().bInAidStations())
        {
            bool bEnterObstacle;
            PetSpanManager.Instance().dicEnterObstacle.TryGetValue(petModelRecData.id, out bEnterObstacle);
            if (bEnterObstacle)
            {
                Vector3 m = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 turnForward = Vector3.RotateTowards(transform.forward, -m, sky * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(turnForward);

                //transform.rotation = Quaternion.Slerp(transform.rotation,
                //                               Quaternion.LookRotation(new Vector3(Vector3.zero.x - transform.position.x, 0, Vector3.zero.z - transform.position.z)),
                //                                sky * Time.deltaTime);
                StateMotion = true;
                AngleMotion = true;
                PetSpanManager.Instance().dicEnterObstacle[petModelRecData.id] = false;
            }
        }

        //确定转头方向
        int d = Random.Range(0, 2);

        float Statu = c == 1 ? MoveSpeed : RunSpeed;
        if (StateMotion)
        {
            Distance = Random.Range(20, 30);
            StateMotion = false;
        }

        //生成随机数(0-15);确定旋转次数。
        int RotateAngle = Random.Range(0, 3);
        if (!WhetherRest)
        {
            if (AngleMotion)
            {
                if (d == 1)
                {
                    sky = -sky;
                }
                AngleMotion = false;
                for (int i = 0; i < RotateAngle; i++)
                {
                    transform.Rotate(0, sky * Time.deltaTime, 0, Space.Self);
                }
            }

            if ((Distance--) != 0)
            {
                if (c == 1)
                {
                    StopAnimation();
                    anim.SetBool("ToWalk", true);
                }
                else
                {
                    StopAnimation();
                    anim.SetBool("ToWalk", true);
                }
                transform.Translate(0, 0, Statu * Time.deltaTime, Space.Self);
            }
            else
            {
                StateMotion = true;
                AngleMotion = true;
                //c = Random.Range(0, 2);
            }
        }
        else
        {
            StopAnimation();
            anim.SetBool("ToRest", true);
        }

        if (timerInterval < timeInterval && !WhetherRest)
        {
            timerInterval += Time.deltaTime;
        }

        if (timerInterval >= timeInterval && !WhetherRest)
        {
            if (!WhetherRest)
                WhetherRest = true;
            timerInterval = 0f;
            bRest = true;
            RandomTimeIntervalFun();
        }

        if (bRest)
        {
            timerRest += Time.deltaTime;
            if (timerRest >= timeRest)
            {
                if (WhetherRest)
                    WhetherRest = false;
                timerRest = 0f;
                bRest = false;
                RandomTimeRestFun();
            }
        }

        if (gameObject.transform.position.x > ChangeRoomManager.Instance().roomAreaData.leftBoundary)//超出左区域
        {

            gameObject.transform.position = new Vector3(ChangeRoomManager.Instance().roomAreaData.leftBoundary - 0.1f, gameObject.transform.position.y, gameObject.transform.position.z);
            Vector3 m = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 turnForward = Vector3.RotateTowards(transform.forward, -m, sky * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(turnForward);

            //transform.rotation = Quaternion.Slerp(transform.rotation,
            //                   Quaternion.LookRotation(new Vector3(Vector3.zero.x - transform.position.x, 0, Vector3.zero.z - transform.position.z)),
            //                    sky * Time.deltaTime);
            StateMotion = true;
            AngleMotion = true;

            bInLeftArea = true;
        }
        else
        {
            bInLeftArea = false;
        }

        if (gameObject.transform.position.x < ChangeRoomManager.Instance().roomAreaData.rightBoundary)//超出右区域
        {
            gameObject.transform.position = new Vector3(ChangeRoomManager.Instance().roomAreaData.rightBoundary + 0.1f, gameObject.transform.position.y, gameObject.transform.position.z);
            Vector3 m = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 turnForward = Vector3.RotateTowards(transform.forward, -m, sky * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(turnForward);

            //transform.rotation = Quaternion.Slerp(transform.rotation,
            //                   Quaternion.LookRotation(new Vector3(Vector3.zero.x - transform.position.x, 0, Vector3.zero.z - transform.position.z)),
            //                    sky * Time.deltaTime);
            StateMotion = true;
            AngleMotion = true;

            bInRightArea = true;
        }
        else
        {
            bInRightArea = false;
        }

        if (gameObject.transform.position.z < ChangeRoomManager.Instance().roomAreaData.topBoundary)//超出上区域
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, ChangeRoomManager.Instance().roomAreaData.topBoundary + 0.1f);
            Vector3 m = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 turnForward = Vector3.RotateTowards(transform.forward, -m, sky * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(turnForward);

            //transform.rotation = Quaternion.Slerp(transform.rotation,
            //                   Quaternion.LookRotation(new Vector3(Vector3.zero.x - transform.position.x, 0, Vector3.zero.z - transform.position.z)),
            //                    sky * Time.deltaTime);

            StateMotion = true;
            AngleMotion = true;

            bInTopArea = true;
        }
        else
        {
            bInTopArea = false;
        }

        if (gameObject.transform.position.z > ChangeRoomManager.Instance().roomAreaData.bottomBoundary)//超出下区域
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, ChangeRoomManager.Instance().roomAreaData.bottomBoundary - 0.1f);
            Vector3 m = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 turnForward = Vector3.RotateTowards(transform.forward, -m, sky * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(turnForward);

            //transform.rotation = Quaternion.Slerp(transform.rotation,
            //                   Quaternion.LookRotation(new Vector3(0, 0, -transform.position.z)),
            //                    sky * Time.deltaTime);
            StateMotion = true;
            AngleMotion = true;

            bInBottomArea = true;
        }
        else
        {
            bInBottomArea = false;
        }
    }
    /// <summary>
    /// 停止之前的动画行为
    /// </summary>
    void StopAnimation()
    {
        anim.SetBool("ToRest", false);
        anim.SetBool("ToWalk", false);
    }

    public void SetWhetherRest(bool bRest)
    {
        WhetherRest = bRest;
    }

    public bool GetWhetherRest()
    {
        return WhetherRest;
    }

    public void RandomTimeIntervalFun()
    {
        timeInterval = Random.Range(10f, 20f);
    }

    public void RandomTimeRestFun()
    {
        timeRest = Random.Range(5f, 10f);
    }


}