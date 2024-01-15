using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.U2D;
using Treasure;

public class RoomItem : BaseUIForm
{
    public Button m_BtnItem;
    public Button m_BtnEnter;
    public Text m_NameText;
    public Text m_NumText;
    public Transform m_FullTrans;
    public Transform m_CurTrans;
    private Room m_RoomData;
    private Text curText;
    // Start is called before the first frame update
    void Awake()
    {
        //m_BtnItem.onClick.AddListener(() =>
        //{
        //    if (m_RoomData.UserList.Count >= 6)
        //    {
        //        ToastManager.Instance.ShowNewToast("该房间人数已满，无法加入", 2f);
        //    }
        //});

        m_BtnEnter.onClick.AddListener(() =>
        {
            if (m_RoomData == null)
                return;
            MessageManager.GetInstance().SendJoinRoom(m_RoomData.RoomId);
        });

        curText = m_CurTrans.GetComponent<Text>();
    }

    public void SetRoomName(string name)
    {
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.RainbowBeach)
            m_NameText.text = string.Format("彩虹沙滩·{0}", name);
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.ShenMiHaiWan)
            m_NameText.text = string.Format("神秘海湾·{0}", name);
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.HaiDiXingKong)
            m_NameText.text = string.Format("海底星空·{0}", name);
    }

    public void SetRoomNum(int num)
    {
        m_NumText.text = string.Format("{0}/6", num);
    }

    public void SetFullState(bool bFull)
    {
        m_FullTrans.gameObject.SetActive(bFull);
        m_BtnEnter.gameObject.SetActive(!bFull);
        //m_NameText.color = bFull ? new Color(252 / 255f, 75 / 255f, 75 / 255f) : new Color(24f / 255f, 173f / 255f, 206f / 255f);
        curText.text = bFull ? GetFullText() : GetCurText();
        curText.color = bFull ? new Color(252 / 255f, 75 / 255f, 75 / 255f) : new Color(49 / 255f, 157 / 255f, 79 / 255f);
    }

    public string GetCurText()
    {
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.RainbowBeach)
            return "· 当前沙滩";
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.ShenMiHaiWan)
            return "· 当前海湾";
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.HaiDiXingKong)
            return "· 当前海底";
        return "· 当前沙滩";
    }
    public string GetFullText()
    {
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.RainbowBeach)
            return "· 沙滩人满";
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.ShenMiHaiWan)
            return "· 海湾人满";
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.HaiDiXingKong)
            return "· 海底人满";
        return "· 沙滩人满";
    }


    public void SetSelfRoomState(bool bSelf)
    {
        m_CurTrans.gameObject.SetActive(bSelf);
        if (bSelf)
        {
            if (m_FullTrans.gameObject.activeSelf)
            {
                m_FullTrans.gameObject.SetActive(false);
            }
        }
    }

    public void SetRoomData(Room data)
    {
        m_RoomData = data;
    }

    public void SetEnterVisiable(bool bVisiable)
    {
        m_BtnEnter.gameObject.SetActive(bVisiable);
    }
}
