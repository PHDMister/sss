using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fight;




public class MoveControllerBase : MonoBehaviour
{
    /// <summary>
    /// 撞击障碍物的状态
    /// </summary>
    public enum CharacterHitState
    {
        Ing, //撞击中
        normoal, //没有撞击状态
    }
    /// <summary>
    ///  冰冻状态
    /// </summary>
    public enum CharacterFrozenState
    {
        Ing, //冰冻状态
        normoal, //没有被冰冻
    }
    /// <summary>
    /// 保护罩状态
    /// </summary>
    public enum CharacterProtectState
    {
        Ing, //被保护中
        normoal, //没有被保护
    }

    /*//角色状态
    public enum CharacterState
    {
        wait,//等待开始
        normale,// 正常
        defense,// 防御
        beAttacked,// 被攻击
        beHit,//被撞击
    }*/
    public enum CharacterRole
    {
        Attacker,// 进攻者
        Runaway,//逃跑者
    }
    public enum BehaviorState
    {
        triChangeTrack,//触发换赛道
        triDefense,// 触发防御
        triAttack,// 触发攻击
    }
    // 速度
    public float moveSpeed;
    //初始位置
    public Vector3 InitPos;

    /// <summary>
    /// 撞击障碍物状态
    /// </summary>
    public CharacterHitState characterHitState;
    /// <summary>
    /// 冰冻状态
    /// </summary>
    public CharacterFrozenState characterFrozenState;
    /// <summary>
    /// 保护罩状态
    /// </summary>
    public CharacterProtectState characterProtectState;


    //角色
    public CharacterRole characterRole;
    //事件
    public BehaviorState behaviorState;

    private Animator animator;
    private Animator DenfensAnimator;

    private string animatorName = "";

    //倒计时
    private float TimeRemaining = 0;
    private bool IsCountDown = false;


    private float aaaaaaa = 0;

    // Start is called before the first frame update
    void Start()
    {
        On_Start();
    }
    // Update is called once per frame
    void Update()
    {
        On_Update();
    }
    public virtual void On_Start()
    {
        transform.gameObject.AddComponent<Rigidbody>();
        Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        transform.gameObject.AddComponent<BoxCollider>();
        BoxCollider boxCollider = transform.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1, 1, 1);
        transform.tag = "Car";

    }
    public virtual void On_Update()
    {
        if (GameController.Instance().GameRuning)
        {
            if (characterHitState != CharacterHitState.Ing && characterFrozenState != CharacterFrozenState.Ing)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitStartDataFun(Vector3 vector3, float speed, CharacterRole characRole)
    {
        InitPos = vector3;
        moveSpeed = speed;
        transform.position = InitPos;
        characterRole = characRole;
        /*    characterHitState = CharacterHitState.normoal;
            characterFrozenState = CharacterFrozenState.normoal;
            characterProtectState = CharacterProtectState.normoal;*/
    }
    //初始化模型
    public void InitStartModelFun()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        if (characterRole == CharacterRole.Attacker)
        {
            GameObject car1 = Instantiate(Resources.Load("Prefabs/Car/car1", typeof(GameObject)) as GameObject, transform);
            car1.transform.localPosition = Vector3.zero;
            animator = car1.transform.GetComponent<Animator>();
            GameObject denfensPre = car1.transform.Find("DefensSkillPrefabs").gameObject;
            DenfensAnimator = denfensPre.transform.GetComponent<Animator>();

        }
        else
        {
            GameObject car2 = Instantiate(Resources.Load("Prefabs/Car/car2", typeof(GameObject)) as GameObject, transform);
            car2.transform.localPosition = Vector3.zero;
            animator = car2.transform.GetComponent<Animator>();
            GameObject denfensPre = car2.transform.Find("DefensSkillPrefabs").gameObject;
            DenfensAnimator = denfensPre.transform.GetComponent<Animator>();
        }
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGameFun()
    {
        characterHitState = CharacterHitState.normoal;
        characterFrozenState = CharacterFrozenState.normoal;
        characterProtectState = CharacterProtectState.normoal;
        animatorName = "";
    }
    /// <summary>
    /// 设置碰撞障碍物的状态
    /// </summary>
    /// <param name="state"></param>
    public void SetHitStateFun(UserInfo userInfo)
    {
        if (userInfo.IsObs)
        {
            SetHitStateDataFun(CharacterHitState.Ing, userInfo);
        }
        else
        {
            SetHitStateDataFun(CharacterHitState.normoal, userInfo);
        }
    }
    private void SetHitStateDataFun(CharacterHitState state, UserInfo userInfo)
    {
        if (characterHitState != state)
        {
            characterHitState = state;
            SetHitStateAnimFun(userInfo);
        }
    }
    /// <summary>
    /// 设置碰撞的动画
    /// </summary>
    private void SetHitStateAnimFun(UserInfo userInfo)
    {
        if (characterHitState == CharacterHitState.Ing)
        {
            Vector3 nowPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            nowPos.z = (userInfo.CurPos.Pos.Z / 100) - 40;
            transform.position = nowPos;

            if (userInfo.ObsPos != null)
            {
                if (GameController.Instance().ObssObjDic.ContainsKey(userInfo.ObsPos.Z))
                {
                    GameController.Instance().ObssObjDic[userInfo.ObsPos.Z].gameObject.SetActive(false);
                }
            }
        }
    }
    /// <summary>
    /// 设置冰冻的状态
    /// </summary>
    /// <param name="state"></param>
    public void SetFrozenStateFun(UserInfo userInfo)
    {
        if (userInfo.IsFreeze)
        {
            SetFrozenStateDataFun(CharacterFrozenState.Ing, userInfo);
        }
        else
        {
            SetFrozenStateDataFun(CharacterFrozenState.normoal, userInfo);
        }
    }
    /// <summary>
    /// 设置冰冻的数据
    /// </summary>
    /// <param name="state"></param>
    /// <param name="userInfo"></param>
    private void SetFrozenStateDataFun(CharacterFrozenState state, UserInfo userInfo)
    {
        if (characterFrozenState != state)
        {
            characterFrozenState = state;
            SetFrozenStateAnimFun(userInfo);
        }
    }
    /// <summary>
    /// 设置冰冻的动画
    /// </summary>
    private void SetFrozenStateAnimFun(UserInfo userInfo)
    {
        if (characterFrozenState == CharacterFrozenState.Ing)
        {
            PlayerSkillAnimationFun("Attack");
            Vector3 nowPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            nowPos.z = (userInfo.CurPos.Pos.Z / 100) - 40;
            transform.position = nowPos;
        }
        else
        {
            PlayerSkillAnimationFun("AttackDown");
        }
    }
    /// <summary>
    /// 设置保护罩的状态
    /// </summary>
    /// <param name="state"></param>
    public void SetProtectStateFun(UserInfo userInfo)
    {
        if (userInfo.IsShield)
        {
            SetProtectStateDataFun(CharacterProtectState.Ing, userInfo);
        }
        else
        {
            SetProtectStateDataFun(CharacterProtectState.normoal, userInfo);
        }
    }
    private void SetProtectStateDataFun(CharacterProtectState state, UserInfo userInfo)
    {
        if (characterProtectState != state)
        {
            characterProtectState = state;
            SetProtectStateAnimFun(userInfo);
        }
    }
    private void SetProtectStateAnimFun(UserInfo userInfo)
    {
        if (characterProtectState == CharacterProtectState.Ing)
        {
            PlayerSkillAnimationFun("Defens");
            if (userInfo.BoxPos != null)
            {
                if (GameController.Instance().BoxObjDic.ContainsKey(userInfo.BoxPos.Z))
                {
                    if (GameController.Instance().BoxObjDic[userInfo.BoxPos.Z].gameObject)
                    {
                        GameController.Instance().BoxObjDic[userInfo.BoxPos.Z].gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            PlayerSkillAnimationFun("DefensDone");
        }
        Vector3 nowPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        nowPos.z = (userInfo.CurPos.Pos.Z / 100) - 40;
        transform.position = nowPos;
    }
    public void PlayerSkillAnimationFun(string animName)
    {
        if (animName == "Defens" || animName == "DefensDone")
        {
            if (DenfensAnimator == null)
            {
                return;
            }
            DenfensAnimator.Play(animName, 0, 0f);
        }
        else
        {
            if (animator == null)
            {
                return;
            }
            animator.Play(animName, 0, 0f);
        }
    }
    /// <summary>
    /// 生成攻击预制体
    /// </summary>
    /// <param name="targetobj"></param>
    public void AttackSkillFun(GameObject targetobj)
    {
        GameObject attackPrefab = Instantiate(Resources.Load("Prefabs/Skill/SnowHit", typeof(GameObject)) as GameObject);
        AttackSkillItem attackSkillItem = attackPrefab.GetComponent<AttackSkillItem>();
        attackSkillItem.SetTargetFun(transform.gameObject, targetobj);
    }
}
