using UIFW;
using UnityEngine.UI;

public class UIBagItem : BaseUIForm
{
    private Text txtName;

    private Image imgIcon;
    private void Awake()
    {
        txtName = transform.Find("txtName").GetComponent<Text>();
        imgIcon = transform.Find("imgIcon").GetComponent<Image>();
    }

    public void SetData(BagItem item)
    {
        txtName.text = item.Name + "x" + item.Count;
        
        SetIcon(imgIcon, "ShellIcon", item.Icon);
    }
}
