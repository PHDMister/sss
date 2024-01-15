using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UIFW;

public class NoticeController : ISingleton
{
    private GameObject m_NoticePanelObj;
    private Text m_TxtMsg;//跑马灯text.
    private Image m_ImgMask;//跑马灯显示长度
    private Queue<string> m_MsgQueue = new Queue<string>();//灯队列.
    private bool isScrolling = false;//判断当前text中的跑马灯是否跑完.
    private Tweener tweener;
    private void Start()
    {

    }

    public void Init()
    {
        m_MsgQueue = new Queue<string>();
    }
    /// <summary>
    /// 添加跑马灯信息.
    /// </summary>
    /// <param name="msg"></param>
    public void AddMessage(string msg)
    {
        if (m_NoticePanelObj == null)
        {
            m_NoticePanelObj = ResourcesMgr.GetInstance().LoadAsset("UIRes/UIPrefabs/NoticePanel", true);
        }

        m_NoticePanelObj.SetActive(true);
        Transform parentTrans = UIManager.GetInstance().GetTopLayerTrans();
        if (parentTrans != null)
        {
            m_NoticePanelObj.transform.SetParent(parentTrans);
            m_NoticePanelObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -179f);
        }
        m_TxtMsg = UnityHelper.FindTheChildNode(m_NoticePanelObj, "txt_Msg").GetComponent<Text>();
        m_ImgMask = UnityHelper.FindTheChildNode(m_NoticePanelObj, "Mask").GetComponent<Image>();
        if (m_TxtMsg == null)
            return;
        m_MsgQueue.Enqueue(msg);
        if (isScrolling) return;
        Scrolling();
    }

    public void Scrolling()
    {
        float beginX = 354;
        float leftX = -354;
        while (m_MsgQueue.Count > 0)
        {
            float duration = 10f;
            float speed = 100f;
            int loop = 1;
            string msg = m_MsgQueue.Dequeue();
            m_TxtMsg.text = msg;
            float txtWidth = m_TxtMsg.preferredWidth;//文本自身的长度.
            Vector3 pos = m_TxtMsg.rectTransform.localPosition;
            float width = m_ImgMask.GetComponent<RectTransform>().rect.width;
            float distance = width + txtWidth;//beginX - leftX + txtWidth;
            duration = distance / speed;
            isScrolling = true;
            while (loop-- > 0)
            {
                m_TxtMsg.rectTransform.localPosition = new Vector3(beginX, pos.y, pos.z);
                tweener = m_TxtMsg.rectTransform.DOLocalMoveX(-distance, duration / 2).SetEase(Ease.Linear).OnComplete(() =>
                {
                    isScrolling = false;
                    if (m_NoticePanelObj != null)
                    {
                        GameObject.Destroy(m_NoticePanelObj);
                    }
                    if (tweener != null)
                    {
                        tweener.Kill();
                    }
                });
            }
        }
    }
}