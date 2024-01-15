using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class FishClubItem : BaseUIForm
{
    public Image m_IconImg;
    public Text m_TextName;
    public Button m_BtnItem;
    public GameObject m_SelectedGo;

    private int itemIndex;
    private Color selectedColor = new Color(0, 255, 240);
    
    void Awake()
    {
        m_BtnItem.onClick.AddListener(OnClickItemHandler);
    }

    public void SetFishInfo(int index,island_fish cfg)
    {
        itemIndex = index;
        m_TextName.text = cfg.fish_name;
    }

    public void SetSelectState(bool isSelected)
    {
        m_SelectedGo.SetActive(isSelected);
        m_TextName.color = isSelected ? selectedColor : Color.white;
    }
    
    public void SetIcon(string icon,Material material)
    {
        Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("Fish", icon);
        m_IconImg.sprite = sprite;
        m_IconImg.material = material;
    }
    
    void OnClickItemHandler()
    {
        SendMessage("FishClubItemMsg", "Click" , itemIndex);
    }
    
}