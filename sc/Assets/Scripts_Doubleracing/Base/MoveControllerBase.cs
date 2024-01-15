using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fight;




public class MoveControllerBase : MonoBehaviour
{
    /// <summary>
    /// ײ���ϰ����״̬
    /// </summary>
    public enum CharacterHitState
    {
        Ing, //ײ����
        normoal, //û��ײ��״̬
    }
    /// <summary>
    ///  ����״̬
    /// </summary>
    public enum CharacterFrozenState
    {
        Ing, //����״̬
        normoal, //û�б�����
    }
    /// <summary>
    /// ������״̬
    /// </summary>
    public enum CharacterProtectState
    {
        Ing, //��������
        normoal, //û�б�����
    }

    /*//��ɫ״̬
    public enum CharacterState
    {
        wait,//�ȴ���ʼ
        normale,// ����
        defense,// ����
        beAttacked,// ������
        beHit,//��ײ��
    }*/
    public enum CharacterRole
    {
        Attacker,// ������
        Runaway,//������
    }
    public enum BehaviorState
    {
        triChangeTrack,//����������
        triDefense,// ��������
        triAttack,// ��������
    }
    // �ٶ�
    public float moveSpeed;
    //��ʼλ��
    public Vector3 InitPos;

    /// <summary>
    /// ײ���ϰ���״̬
    /// </summary>
    public CharacterHitState characterHitState;
    /// <summary>
    /// ����״̬
    /// </summary>
    public CharacterFrozenState characterFrozenState;
    /// <summary>
    /// ������״̬
    /// </summary>
    public CharacterProtectState characterProtectState;


    //��ɫ
    public CharacterRole characterRole;
    //�¼�
    public BehaviorState behaviorState;

    private Animator animator;
    private Animator DenfensAnimator;

    private string animatorName = "";

    //����ʱ
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
    /// ��ʼ������
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
    //��ʼ��ģ��
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
    /// ��ʼ��Ϸ
    /// </summary>
    public void StartGameFun()
    {
        characterHitState = CharacterHitState.normoal;
        characterFrozenState = CharacterFrozenState.normoal;
        characterProtectState = CharacterProtectState.normoal;
        animatorName = "";
    }
    /// <summary>
    /// ������ײ�ϰ����״̬
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
    /// ������ײ�Ķ���
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
    /// ���ñ�����״̬
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
    /// ���ñ���������
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
    /// ���ñ����Ķ���
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
    /// ���ñ����ֵ�״̬
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
    /// ���ɹ���Ԥ����
    /// </summary>
    /// <param name="targetobj"></param>
    public void AttackSkillFun(GameObject targetobj)
    {
        GameObject attackPrefab = Instantiate(Resources.Load("Prefabs/Skill/SnowHit", typeof(GameObject)) as GameObject);
        AttackSkillItem attackSkillItem = attackPrefab.GetComponent<AttackSkillItem>();
        attackSkillItem.SetTargetFun(transform.gameObject, targetobj);
    }
}
