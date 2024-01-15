using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static bool IsBgmOpen = true;
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
        if (IsBgmOpen) audioSource.Play();
        else audioSource.Stop();
    }


    protected void OnDestroy()
    {
        MessageCenter.RemoveMsgListener("AudioController_Enable", OnEnableHandler);
    }

    private void OnEnableHandler(KeyValuesUpdate kv)
    {
        if (kv.Key == "open")
        {
            audioSource.Play();
            IsBgmOpen = true;
        }
        else
        {
            audioSource.Stop();
            IsBgmOpen = false;
        }
    }
}
