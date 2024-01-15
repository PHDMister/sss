using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    public static AudioMgr Ins;

    public bool isPlaying = true;
    public AudioSource bgm;
    public AudioSource effect;
    public AudioSource effect2;

    protected AudioSource sound;

    protected void Awake()
    {
        Ins = this;
        DontDestroyOnLoad(this.gameObject);
        this.gameObject.name = "[AudioManager]";
    }

    protected void Start()
    {
        isPlaying = true;
        if (bgm)
        {
            bgm.playOnAwake = true;
            bgm.loop = true;
            bgm.Play();
        }

        if (effect)
        {
            effect.playOnAwake = false;
            effect.loop = true;
            effect.Stop();
        }

        if (effect2)
        {
            effect.playOnAwake = false;
            effect.loop = true;
            effect.Stop();
        }

        UpdateAudioSource();
    }

    protected void OnDestroy()
    {
        Ins = null;
    }

    public void UpdateAudioSource()
    {
        effect.Stop();
        effect2.Stop();

        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.RainbowBeach)
            sound = effect;
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.ShenMiHaiWan)
            sound = effect;
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.HaiDiXingKong)
            sound = effect2;

        if (isPlaying) sound.Play();
    }
    public void SetMute(bool val)
    {
        isPlaying = !val;
        if (val == true)
        {
            bgm.Stop();
            sound.Stop();
            MessageCenter.SendMessage("AudioController_Enable", "close", null);
        }
        else
        {
            bgm.Play();
            sound.Play();
            MessageCenter.SendMessage("AudioController_Enable", "open", null);
        }
    }
    public void PlaySound()
    {
        isPlaying = true;
        SetMute(false);
        bgm.Play();
        effect.Play();
        MessageCenter.SendMessage("AudioController_Enable", "open", null);
    }

    public void StopBgm()
    {
        bgm.Stop();
    }
    public void StopEffect()
    {
        sound.Stop();
    }
}
