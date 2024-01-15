using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;
public class CameraUIItem : BaseUIForm
{
    private Button ClickBtn;
    private int Index;
    private Image Icon_Image;
    private Text FurnitureName_Text;
    private GameObject ChooseImge_Obj;
    private void Awake()
    {
        ClickBtn = transform.GetComponent<Button>();
        ClickBtn.onClick.AddListener(OnClickListenFun);
        Icon_Image = transform.Find("Icon_Img").GetComponent<Image>();
        FurnitureName_Text = transform.Find("FurnitureName_Text").GetComponent<Text>();
        ChooseImge_Obj = transform.Find("Choose_Image").gameObject;
        ChooseImge_Obj.SetActive(false);
    }
    void OnClickListenFun()
    {
        SendMessage("CamerUIMessage", "data", Index);
    }
    public void SetCameraUIFun(int index)
    {
        Index = index;
        Debug.Log("这里的值L    : " + index);
        if (RoomFurnitureCtrl.Instance().CameraValueDataDic.ContainsKey(index))
        {
            CameraValueData valueData = RoomFurnitureCtrl.Instance().CameraValueDataDic[index];
            FurnitureName_Text.text = valueData.CameraName;
            Debug.Log("输出内容： " + valueData.CameraName + "   镜头名字： " + valueData.IconName);
            Sprite texture = InterfaceHelper.GetIconFun(valueData.IconName);
            if (texture != null)
            {
                Icon_Image.sprite = texture;
            }
            else
            {
                Debug.Log("没有找到，为空");
            }
        }
    }
}
