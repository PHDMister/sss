using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherSpaceUIItem : MonoBehaviour
{
    //空间名称
    public Text SpaceName_Text;
    //位置数据
    public Text PosData_Text;
    //建筑名称
    public Text HouseName_Text;
    //寄售中提示
    public GameObject ShowBuyTip;
    //寄售中内容
    public Text ShowBuyTip_Text;
    //个人空间按钮
    public Button Space_Btn;
    //进入空间图片提示
    public GameObject IntoImage_Obj;
    //去购买空间图片提示
    public GameObject ToBuyImage_obj;
    //进入空间图文本提示
    public GameObject IntoText_Obj;
    //去购买空间文本提示
    public GameObject ToBuyText_Obj;

    public GameObject ShowBuyTipImage;

    private OtherSpaceData _OtherSpaceData;
    public event Action<OtherSpaceData> OnClickAction;
    private void Awake()
    {
        Space_Btn.onClick.AddListener(OnClickFun);
    }
    /// <summary>
    /// 设置预制体
    /// </summary>
    public void SetPrefabFun(OtherSpaceData otherSpaceData)
    {
        _OtherSpaceData = otherSpaceData;
        SpaceName_Text.text = TextTools.setCutAddString(otherSpaceData.SpaceName, 7, "...");
        PosData_Text.text = otherSpaceData.BuildXYZ;
        HouseName_Text.text = TextTools.setCutAddString(otherSpaceData.BuildName, 7, "...");

        Debug.Log("otherSpaceData.SpaceName：  " + otherSpaceData.SpaceName);
        Debug.Log("otherSpaceData.BuildName：  " + otherSpaceData.BuildName);

        if (otherSpaceData.StatusID == 1)
        {
            ShowBuyTip.SetActive(false);
            IntoImage_Obj.SetActive(true);
            ToBuyImage_obj.SetActive(false);
            IntoText_Obj.SetActive(true);
            ToBuyText_Obj.SetActive(false);
        }
        else
        {
            ShowBuyTip.SetActive(true);
            IntoImage_Obj.SetActive(false);
            ToBuyImage_obj.SetActive(true);
            IntoText_Obj.SetActive(false);
            ToBuyText_Obj.SetActive(true);
            ShowBuyTip_Text.text = "寄售中￥" + otherSpaceData.Price / 100;
        }
    }
    public void OnClickFun()
    {
        OnClickAction?.Invoke(_OtherSpaceData);
    }
    public void AdjustGasUI()
    {
        ShowBuyTip_Text.gameObject.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(ShowBuyTip_Text);
        float height = ShowBuyTipImage.transform.GetComponent<RectTransform>().sizeDelta.y;
        ShowBuyTipImage.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(40f + width, height);
    }
    private void Update()
    {
        AdjustGasUI();
    }

}
