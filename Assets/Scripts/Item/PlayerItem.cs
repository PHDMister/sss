using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    private CharacterController playerCharacter;
    private Animator PlayerAnimator;
    private string lastTrigger;
    public string LastTrigger => lastTrigger;
    private Transform weaTrans;
    private Animator WeaAnimator;
    private ulong userID = 0;
    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        playerCharacter = transform.GetComponent<CharacterController>();
        PlayerAnimator = transform.GetComponent<Animator>();
        weaTrans = transform.Find("Weapon");
        if (weaTrans) WeaAnimator = weaTrans.GetComponent<Animator>();
    }
    //设置动画
    public void SetAnimator(string trigger, float speed = 1)
    {
        if (!string.IsNullOrEmpty(lastTrigger))
        {
            PlayerAnimator.ResetTrigger(lastTrigger);
            if (weaTrans && lastTrigger.StartsWith("W_"))
                WeaAnimator.ResetTrigger(lastTrigger);
        }
        PlayerAnimator.SetTrigger(trigger);
        lastTrigger = trigger;
        PlayerAnimator.speed = Mathf.Max(1, speed);

        //挖宝的锄头
        if (weaTrans)
        {
            if (trigger.StartsWith("W_"))
            {
                if (!weaTrans.gameObject.activeSelf) weaTrans.gameObject.SetActive(true);
                WeaAnimator.SetTrigger(trigger);
                WeaAnimator.speed = Mathf.Max(1, speed);
            }
            else if (weaTrans.gameObject.activeSelf)
            {
                weaTrans.gameObject.SetActive(false);
            }
        }
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
}
