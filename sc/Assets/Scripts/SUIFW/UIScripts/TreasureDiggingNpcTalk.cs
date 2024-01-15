using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class TreasureDiggingNpcTalk : BaseUIForm
{
    private Camera npcCamera = null;
    public Text m_TextTalk;
    public Button m_BtnNext;

    public float delay = 0.001f;
    private string firstText = "欢迎来到寻宝乐园！我是寻宝使者——“一粒喵”！\n寻宝乐园里有无数的宝藏在等你去挖掘哦！";
    private string secondText = "";
    private string currentText;
    private bool bFirstTalkEnd = false;
    private bool bFirstSkip = false;
    private bool bSecondTalkEnd = false;
    private bool bSecondSkip = false;
    private int m_ClickNum = 0;
    private Coroutine m_CoFirst;
    private Coroutine m_CoSecond;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //注册进入主城的事件
        if (m_BtnNext != null)
            m_BtnNext.onClick.AddListener(OnClickNext);

        RigisterButtonObjectEvent("BtnBg",
              P =>
              {
                  OnClickNext();
              }
          );
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);

        var joystickMgrObj = JoystickManager.Instance().GetJoystickObj();
        if (joystickMgrObj != null)
        {
            joystickMgrObj.gameObject.SetActive(false);
        }
        ResetData();

        if (m_BtnNext != null)
        {
            if (m_BtnNext.gameObject.activeSelf)
            {
                m_BtnNext.gameObject.SetActive(false);
            }
        }

        //treasure_ticket ticketCfg = ManageMentClass.DataManagerClass.GetTreasureTicketTable(1);
        //if (ticketCfg != null)
        //{
        //    secondText = string.Format("消耗【{0}】*1就可以兑换寻宝入场券*1\n有了它就可以进入寻宝乐园挖掘宝藏啦！", ticketCfg.collection_name);
        //}
        secondText = string.Format("消耗指定藏品就可以兑换挖宝券\n有了它就可以进入寻宝乐园挖掘宝藏啦！");
        LookAtNpc();
        m_CoFirst = StartCoroutine(ShowTextFirst());
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        var joystickMgrObj = JoystickManager.Instance().GetJoystickObj();
        if (joystickMgrObj != null)
        {
            joystickMgrObj.gameObject.SetActive(true);
        }
        ResetData();
    }

    private void ResetData()
    {
        bFirstTalkEnd = false;
        bFirstSkip = false;
        bSecondTalkEnd = false;
        bSecondSkip = false;
        m_ClickNum = 0;
    }

    private void LookAtNpc()
    {
        if (npcCamera == null)
        {
            npcCamera = new GameObject("npcCamera").AddComponent<Camera>();
        }
        GameObject npcObj = GameObject.FindGameObjectWithTag("_TagNpc");
        if (npcObj != null)
        {
            //npcCamera.transform.position = new Vector3(npcObj.transform.position.x, npcObj.transform.position.y, npcObj.transform.position.z);
            npcCamera.transform.position = new Vector3(10.813f, 2.307f, -2.21f);
            npcCamera.transform.rotation = Quaternion.Euler(13.236f, 41.916f, 0.843f);
            npcCamera.cullingMask = (1 << 0) + (1 << 14);
        }
    }
    private void CancelLookAtNpc()
    {
        if (npcCamera != null)
        {
            DestroyImmediate(npcCamera.gameObject);
        }
    }

    IEnumerator ShowTextFirst()
    {
        bFirstTalkEnd = false;
        bSecondTalkEnd = false;

        for (int i = 0; i < firstText.Length; i++)
        {
            currentText = firstText.Substring(0, i);
            if (m_TextTalk != null)
                m_TextTalk.text = currentText;
            yield return new WaitForSeconds(delay);
            //yield return new WaitForEndOfFrame();
        }
        if (m_BtnNext != null)
        {
            if (!m_BtnNext.gameObject.activeSelf)
            {
                m_BtnNext.gameObject.SetActive(true);
            }
        }

        bFirstTalkEnd = true;
    }

    IEnumerator ShowTextSecond()
    {
        bSecondTalkEnd = false;
        for (int i = 0; i < secondText.Length; i++)
        {
            currentText = secondText.Substring(0, i);
            if (m_TextTalk != null)
                m_TextTalk.text = currentText;
            yield return new WaitForSeconds(delay);
            //yield return new WaitForEndOfFrame();
        }
        if (m_BtnNext != null)
        {
            if (!m_BtnNext.gameObject.activeSelf)
            {
                m_BtnNext.gameObject.SetActive(true);
            }
        }
        bSecondTalkEnd = true;
    }

    private void OnClickNext()
    {
        m_ClickNum++;
        if (!bFirstTalkEnd)
        {
            if (m_ClickNum > 0)
            {
                if (!bFirstSkip)
                {
                    StopCoroutine(m_CoFirst);
                    m_TextTalk.text = firstText;
                    if (!m_BtnNext.gameObject.activeSelf)
                    {
                        m_BtnNext.gameObject.SetActive(true);
                    }
                    bFirstTalkEnd = true;
                    bFirstSkip = true;
                    return;
                }
            }
        }

        if (bFirstTalkEnd)
        {
            if (!bSecondSkip && !bSecondTalkEnd)
            {
                if (m_ClickNum <= 2)
                {
                    if (m_BtnNext != null)
                    {
                        if (m_BtnNext.gameObject.activeSelf)
                        {
                            m_BtnNext.gameObject.SetActive(false);
                        }
                    }
                    m_CoSecond = StartCoroutine(ShowTextSecond());
                    return;
                }
            }
        }

        if (!bSecondTalkEnd)
        {
            if (m_ClickNum > 2)
            {
                if (!bSecondSkip)
                {
                    StopCoroutine(m_CoSecond);
                    m_TextTalk.text = secondText;
                    if (!m_BtnNext.gameObject.activeSelf)
                    {
                        m_BtnNext.gameObject.SetActive(true);
                    }
                    bSecondTalkEnd = true;
                    bSecondSkip = true;
                    return;
                }
            }
        }


        if (bSecondTalkEnd)
        {
            CancelLookAtNpc();
            CloseUIForm();
            OpenUIForm(FormConst.TREASUREDIGGINGMAINMENU);
            OpenUIForm(FormConst.TREASUREDIGGINGTICKETEXCHANGE);
        }
    }
}
