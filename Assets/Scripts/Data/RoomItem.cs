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
    }

    public void SetRoomName(string name)
    {
        m_NameText.text = string.Format("寻宝空间·{0}", name);
    }

    public void SetRoomNum(int num)
    {
        m_NumText.text = string.Format("{0}/6", num);
    }

    public void SetFullState(bool bFull)
    {
        m_FullTrans.gameObject.SetActive(bFull);
        m_BtnEnter.gameObject.SetActive(!bFull);
        m_NameText.color = bFull ? new Color(255f / 255f, 32f / 255f, 95f / 255f) : new Color(63 / 255f, 100f / 255f, 230f / 255f);
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
