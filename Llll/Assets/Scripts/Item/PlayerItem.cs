using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    public IExtInterface ExtAnimInterface;
    public IExtInterface OtherExtAExtInterface;
    private CharacterController playerCharacter;
    private Animator PlayerAnimator;
    public Animator SelfAnimator => PlayerAnimator;
    private string lastTrigger;
    public string LastTrigger => lastTrigger;

    private ulong userID = 0;
    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        playerCharacter = transform.GetComponent<CharacterController>();
        PlayerAnimator = transform.GetComponent<Animator>();
        FindSubModel();
    }
    //设置动画
    public void SetAnimator(string trigger, float speed = 1)
    {
        if (ExtAnimInterface != null)
        {
            ExtAnimInterface.SetAnimator(trigger, speed);
            return;
        }

        if (!string.IsNullOrEmpty(lastTrigger))
        {
            PlayerAnimator.ResetTrigger(lastTrigger);
        }
        PlayerAnimator.SetTrigger(trigger);
        lastTrigger = trigger;
        PlayerAnimator.speed = Mathf.Max(1, speed);

        OtherExtAExtInterface?.SetAnimator(trigger, speed);
    }
    public void SetAnimatorSpeed(float speed = 1)
    {
        PlayerAnimator.speed = speed;
    }
    /// <summary>
    /// 设置人物移动
    /// </summary>
    /// <param name="speed"></param>
    public void SetPlayerMove(float moveSpeed)
    {
        playerCharacter.SimpleMove(transform.forward * moveSpeed);
    }
    /// <summary>
    /// 设置人物移动
    /// </summary>
    /// <param name="speed"></param>
    public void SetPlayerMove(Vector3 vec, float moveSpeed)
    {
        playerCharacter.SimpleMove(vec * moveSpeed);
    }
    /// <summary>
    /// 立即设置旋转 
    /// </summary>
    /// <param name="tmpDir"></param>
    public void SetPlayerRotation(Vector2 tmpDir)
    {
        var tmp_HeroTagetDir = new Vector3(tmpDir.x, 0, tmpDir.y) - transform.forward;
        if (tmp_HeroTagetDir.sqrMagnitude > 0.1f)
        {
            Quaternion tmp_DirQuaternion = Quaternion.LookRotation(tmp_HeroTagetDir);
            transform.rotation = tmp_DirQuaternion;
        }
    }
    /// <summary>
    /// 立即设置旋转 
    /// </summary>
    /// <param name="tmpDir"></param>
    public void SetPlayerRotation(Vector3 euler)
    {
        transform.rotation = Quaternion.Euler(euler);
    }

    public void SetPlayerLookRotation(Vector3 pos)
    {
        transform.rotation = Quaternion.LookRotation(pos);
    }

    /// <summary>
    /// 摇杆设置人物旋转
    /// </summary>
    /// <param name="tmpDir"></param>
    public void SetPlayerRotation(Vector2 tmpDir, float RotateRate)
    {
        var tmp_HeroTagetDir = new Vector3(tmpDir.x, 0, tmpDir.y) - transform.forward;
        if (tmp_HeroTagetDir.sqrMagnitude > 0.1f)
        {
            Quaternion tmp_DirQuaternion = Quaternion.LookRotation(tmp_HeroTagetDir);
            var tmp_SmoothQuaternion = Quaternion.Slerp(transform.rotation, tmp_DirQuaternion, RotateRate * Time.deltaTime);
            transform.rotation = tmp_SmoothQuaternion;
        }
    }
    /// <summary>
    /// 同步移动
    /// </summary>
    /// <param name="joydeltaPos"></param>
    /// <param name="maxDis"></param>
    public void SyncPlayerMove(float joydeltaPos, float nowDis, int state)
    {
        BaseNetView roomSyncNet = WebSocketAgent.Ins.NetView as BaseNetView;
        roomSyncNet?.SyncSelfMove(joydeltaPos, transform.position, nowDis, state);
    }

    public void SetPlayerLocalAngle(Vector3 V3)
    {
        transform.localEulerAngles = V3;
    }

    public void PlayAnimation(string aniName)
    {
        if (!string.IsNullOrEmpty(aniName))
        {
            PlayerAnimator.Play(aniName);
        }
    }

    public bool IsHaveState(string stateName)
    {
        int stateid = Animator.StringToHash(stateName);
        bool hasAction = PlayerAnimator.HasState(0, stateid);
        return hasAction;

    }

    public bool IsPlaying(string animName)
    {
        if (lastTrigger == animName) return true;
        AnimatorStateInfo animInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(0);
        return animInfo.IsName(animName);
    }

    public string GetCurAnimName()
    {
        AnimatorClipInfo clipInfo = PlayerAnimator.GetCurrentAnimatorClipInfo(0)[0];
        return clipInfo.clip.name;
    }

    public float GetCurAnimLength(string animName)
    {
        return PlayerAnimator.GetAnimLength(animName);
    }

    public AnimationClip GetAnimClip(string animName)
    {
        return PlayerAnimator.GetAnimationClip(animName);
    }

    public CharacterController GetPlayerCharacter()
    {
        return playerCharacter;
    }
    public void SetUserID(ulong _userId)
    {
        userID = _userId;
    }
    public ulong GetUserID()
    {
        return userID;
    }


    #region SubModel
    protected Dictionary<string, SubModel> subModelDic = new Dictionary<string, SubModel>();
    public virtual void FindSubModel()
    {
        if (HasTransNode("_Gold", out var node1)) subModelDic["_Gold"] = new SubModel(node1, "_Gold");
        if (HasTransNode("_Silver", out var node2)) subModelDic["_Silver"] = new SubModel(node2, "_Silver");
        if (HasTransNode("_Iron", out var node3)) subModelDic["_Iron"] = new SubModel(node3, "_Iron");
        if (HasTransNode("NormalRod", out var node4)) subModelDic["NormalRod"] = new SubModel(node4, "NormalRod");
        if (HasTransNode("SuperRod", out var node5)) subModelDic["SuperRod"] = new SubModel(node5, "SuperRod");
        if (HasTransNode("DNormalDiving", out var node6)) subModelDic["DNormalDiving"] = new SubModel(node6, "DNormalDiving");
        if (HasTransNode("DSuperDiving", out var node7)) subModelDic["DSuperDiving"] = new SubModel(node7, "DSuperDiving");
    }
    public bool HasTransNode(string nodeName, out Transform tNode)
    {
        tNode = null;
        foreach (Transform node in transform)
        {
            if (node.name == nodeName || node.name.Contains(nodeName))
            {
                tNode = node;
                return true;
            }
        }
        return false;
    }
    public SubModel GetSubModel(string key)
    {
        if (subModelDic.TryGetValue(key, out var subModel))
        {
            return subModel;
        }
        return null;
    }
    #endregion


}


//子模型
public class SubModel
{
    public string key;
    public Transform Node;
    public Animator Animator;
    protected Renderer[] enableRenderer;
    protected GameObject OtherModel;
    protected string lastTrigger = "";
    public SubModel(Transform go, string key)
    {
        this.key = key;
        Node = go;
        Node.gameObject.SetActive(true);
        Animator = Node.GetComponent<Animator>();
        enableRenderer = Node.GetComponentsInChildren<Renderer>(true);
        SetActive(false);
    }
    public void Play(string animName, int layer = 0, float normalizeTime = 0)
    {
        Animator.Play(animName, layer, normalizeTime);
    }
    public void AddModel(string nodeName, GameObject model)
    {
        OtherModel = model;
        Transform trans = FindChildNode(Node.transform, nodeName);
        if (trans)
        {
            model.transform.SetParent(trans);
            model.transform.localPosition = Vector3.zero;
            model.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.Log("1111111111 not  find  nodeName=" + nodeName);
        }
    }

    Transform FindChildNode(Transform parent, string childNodeName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childNodeName)
            {
                return child;
            }
            Transform result = FindChildNode(child, childNodeName);
            if (result != null)
            {
                return result;
            }
        }
        // 未找到子节点
        return null;
    }

    public void RemoveModel()
    {
        if (OtherModel)
        {
            OtherModel.transform.parent = null;
            GameObject.Destroy(OtherModel);
            OtherModel = null;
        }
    }
    public void SetTrigger(string trigger, float speed = 1)
    {
        if (trigger != lastTrigger && !string.IsNullOrEmpty(lastTrigger))
        {
            Animator.ResetTrigger(lastTrigger);
        }
        Animator.SetTrigger(trigger);
        Animator.speed = speed;
        lastTrigger = trigger;
    }
    public void SetActive(bool enable)
    {
        foreach (var renderer in enableRenderer)
        {
            renderer.gameObject.SetActive(enable);
        }
        Animator.enabled = enable;
    }
}