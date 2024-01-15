using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;

public class EmojiItem : BaseUIForm
{
    public Image m_ImgEmoji;
    public Button m_BtnEmoji;
    private emoji m_EmojiData;

    void Awake()
    {
        m_BtnEmoji.onClick.AddListener(() =>
        {
            KeyValuesUpdate kvs = new KeyValuesUpdate("", m_EmojiData);
            MessageCenter.SendMessage("EmojiClicked", kvs);
        });
    }

    public void SetEmoji(emoji data)
    {
        Sprite sprite = Resources.Load<Sprite>(string.Format("UIRes/UISprite/Emoji/{0}", data.Icon));
        if (m_ImgEmoji != null)
            m_ImgEmoji.sprite = sprite;
        m_EmojiData = data;
    }
}
