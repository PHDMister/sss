using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferEffectManager : MonoBehaviour
{
    private Transform effectTrans = null;
    private Transform _TransferMgr = null;
    public Transform _TransferGate = null;
    private List<Material> materials = new List<Material>();
    public float lerpDuration = 1f;//生成渐变持续时间
    public static float _timeElapsed = 0f;

    public bool bTransfer = false;//是否传送
    public bool bTransferEnd = false;//是否传送结束
    public bool bTransferSpace = false;//是否传送回空间

    private static TransferEffectManager _instance = null;
    public static TransferEffectManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_TransferEffectManager").AddComponent<TransferEffectManager>();
        }
        return _instance;
    }

    private void Awake()
    {
        _TransferMgr = this.gameObject.transform;
        DontDestroyOnLoad(_TransferMgr);
    }
    public void Init()
    {
        _TransferGate = GameObject.Find("M_chuansongtai01").transform;
        if (_TransferGate != null)
        {
            effectTrans = _TransferGate.transform.Find("hexagon");
            if (effectTrans != null)
            {
                effectTrans.gameObject.SetActive(false);
                materials.Clear();
                materials = InterfaceHelper.FindChildMaterials(effectTrans.transform);
                SetMaterialProperty(0);
            }
        }
    }

    private void Update()
    {
        if (bTransfer)
        {
            if (materials.Count > 0)
            {
                _timeElapsed += Time.deltaTime;
                if (_timeElapsed < lerpDuration)
                {
                    if (effectTrans != null)
                    {
                        if (!effectTrans.gameObject.activeSelf)
                        {
                            effectTrans.gameObject.SetActive(true);
                        }
                    }

                    float value = Mathf.Lerp(0f, 1f, _timeElapsed / lerpDuration);
                    SetMaterialProperty(value);
                }
                else
                {
                    if (effectTrans != null)
                    {
                        if (effectTrans.gameObject.activeSelf)
                        {
                            effectTrans.gameObject.SetActive(false);
                        }
                    }
                    bTransfer = false;
                    _timeElapsed = lerpDuration;
                    SetMaterialProperty(0f);
                    int level = SceneManager.GetActiveScene().buildIndex;
                    SendMessage("TransferEffectEnd", "End", level);
                }
            }
        }

        if (bTransferEnd)
        {
            if (materials.Count > 0)
            {
                _timeElapsed += Time.deltaTime;
                if (_timeElapsed < lerpDuration)
                {
                    if (effectTrans != null)
                    {
                        if (!effectTrans.gameObject.activeSelf)
                        {
                            effectTrans.gameObject.SetActive(true);
                        }
                    }

                    float value = Mathf.Lerp(1f, 0f, _timeElapsed / lerpDuration);
                    SetMaterialProperty(value);
                }
                else
                {
                    if (effectTrans != null)
                    {
                        if (effectTrans.gameObject.activeSelf)
                        {
                            effectTrans.gameObject.SetActive(false);
                        }
                    }
                    bTransferEnd = false;
                    _timeElapsed = lerpDuration;
                    SetMaterialProperty(0f);
                }
            }
        }
    }

    public void SetMaterialProperty(float value)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            material.SetFloat("_DissolveAmount", value);
        }
    }
    protected void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }
}
