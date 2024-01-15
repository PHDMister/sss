using SuperScrollView;
using UIFW;
using UnityEngine.UI;

public class ClothingDefault:BaseUIForm
{
    public Button m_BtnIcon;
    public Image m_SelectImg;
    void Awake()
    {
        SetItemSelectState(false);
        m_BtnIcon.onClick.AddListener(() =>
        {
            SendMessage("ReceiveClothingItemClick","", index);
        });
                
        ReceiveMessage("ReceiveClothingItemClick", (p) =>
        {
            var itemIndex = (int)p.Values;
            SetItemSelectState(index == itemIndex);
        });
    }
    
    private void SetItemSelectState(bool bSelect)
    {
        if (m_SelectImg == null)
            return;
        m_SelectImg.gameObject.SetActive(bSelect);
    }

    private int index;
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetData()
    {
        SetItemSelectState(index == ManageMentClass.DataManagerClass.ClothingSelectedIndex);
    }
}
