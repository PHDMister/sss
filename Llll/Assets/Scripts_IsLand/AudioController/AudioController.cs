using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    protected AudioSource audioSource;


    public void Init()
    {

    }

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        MessageCenter.AddMsgListener("AudioController_Enable", OnEnableHandler);
    }

    protected void Start()
    {
        if (AudioMgr.Ins == null) return;
        if (AudioMgr.Ins.isPlaying) audioSource.Play();
        else audioSource.Stop();
    }


    protected void OnDestroy()
    {
        MessageCenter.RemoveMsgListener("AudioController_Enable", OnEnableHandler);
    }

    private void OnEnableHandler(KeyValuesUpdate kv)
    {
        if (kv.Key == "open") audioSource.Play();
        else if (kv.Key == "close") audioSource.Stop();
    }
}
