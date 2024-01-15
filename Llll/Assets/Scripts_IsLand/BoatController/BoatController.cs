using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExtInterface
{
    void SetAnimator(string trigger, float speed = 1);
}


public class BoatController : MonoBehaviour, IExtInterface
{
    public PlayerItem Player;
    protected Vector3 offsetY = new Vector3(0, -0.1f, 0);
    protected string BindAnim = "SitDown";
    protected Transform bindPoint;
    protected CharacterController characterController;

    protected GameObject Splash;
    protected GameObject Splash1;

    protected GameObject RippleParticles01;
    protected GameObject RippleParticles02;

    //数据备份
    protected float backRadius = 0;
    protected Vector3 backCenter = Vector3.zero;
    protected float backStepOffset = 0;
    //表现需要的设置数据
    protected float newRadius = 1.3f;
    protected Vector3 newCenter = new Vector3(0, 1.8f, 0);
    protected float newStepOffset = 0.02f;



    protected void Awake()
    {
        Splash = transform.Find("Splash").gameObject;
        Splash1 = transform.Find("Splash(1)").gameObject;
        RippleParticles01 = transform.Find("RippleParticles01").gameObject;
        RippleParticles02 = transform.Find("RippleParticles02").gameObject;

        RippleParticles01.SetActive(false);
        RippleParticles02.SetActive(false);
        Splash.SetActive(false);
        Splash1.SetActive(false);
    }

    protected IEnumerator Start()
    {
        yield return null;
        PlayerItem selfPlayerItem = CharacterManager.Instance().GetPlayerItem();
        bool isActive = Player == selfPlayerItem;
        RippleParticles01.SetActive(isActive);
        RippleParticles02.SetActive(isActive);
    }

    protected void OnEnable()
    {
        if (Player)
        {
            Player.SelfAnimator.ResetTrigger("Idle");
            Player.SelfAnimator.Play(BindAnim, 0, 1);
        }
    }

    protected void LateUpdate()
    {
        if (Player)
        {
            this.transform.position = Player.transform.position + offsetY;
            this.transform.rotation = Player.transform.rotation;
        }
    }

    protected void OnDestroy()
    {
        UnBind();
    }

    public void BindPlayer(PlayerItem player)
    {
        Player = player;
        Player.ExtAnimInterface = this;
        characterController = Player.GetComponent<CharacterController>();
        backRadius = characterController.radius;
        backCenter = characterController.center;
        backStepOffset = characterController.stepOffset;

        characterController.radius = newRadius;
        characterController.center = newCenter;
        characterController.stepOffset = newStepOffset;

        player.SelfAnimator.ResetTrigger("Idle");
        player.SelfAnimator.Play(BindAnim, 0, 1);
    }

    public void UnBind(string anim = "")
    {
        if (Player != null)
        {
            Player.ExtAnimInterface = null;
            if (!string.IsNullOrEmpty(anim)) Player.SetAnimator(anim);
            Player = null;
        }
        if (characterController != null)
        {
            characterController.radius = backRadius;
            characterController.center = backCenter;
            characterController.stepOffset = backStepOffset;
            characterController = null;
        }
    }

    public void PlayEffect()
    {
        //显示特效
        if (Splash.activeSelf == false)
            Splash.SetActive(true);
        if (Splash1.activeSelf == false)
            Splash1.SetActive(true);
    }

    public void StopEffect()
    {
        //关闭特效
        if (Splash.activeSelf)
            Splash.SetActive(false);
        if (Splash1.activeSelf)
            Splash1.SetActive(false);
    }


    #region 接口函数
    public void SetAnimator(string trigger, float speed = 1)
    {

    }
    #endregion
}
