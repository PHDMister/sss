using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherSpaceUIItem : MonoBehaviour
{
    //�ռ�����
    public Text SpaceName_Text;
    //λ������
    public Text PosData_Text;
    //��������
    public Text HouseName_Text;
    //��������ʾ
    public GameObject ShowBuyTip;
    //����������
    public Text ShowBuyTip_Text;
    //���˿ռ䰴ť
    public Button Space_Btn;
    //����ռ�ͼƬ��ʾ
    public GameObject IntoImage_Obj;
    //ȥ����ռ�ͼƬ��ʾ
    public GameObject ToBuyImage_obj;
    //����ռ�ͼ�ı���ʾ
    public GameObject IntoText_Obj;
    //ȥ����ռ��ı���ʾ
    public GameObject ToBuyText_Obj;

    public GameObject ShowBuyTipImage;

    private OtherSpaceData _OtherSpaceData;
    public event Action<OtherSpaceData> OnClickAction;
    private void Awake()
    {
        Space_Btn.onClick.AddListener(OnClickFun);
    }
    /// <summary>
    /// ����Ԥ����
    /// </summary>
    public void SetPrefabFun(OtherSpaceData otherSpaceData)
    {
        _OtherSpaceData = otherSpaceData;
        SpaceName_Text.text = TextTools.setCutAddString(otherSpaceData.SpaceName, 7, "...");
        PosData_Text.text = otherSpaceData.BuildXYZ;
        HouseName_Text.text = TextTools.setCutAddString(otherSpaceData.BuildName, 7, "...");

        Debug.Log("otherSpaceData.SpaceName��  " + otherSpaceData.SpaceName);
        Debug.Log("otherSpaceData.BuildName��  " + otherSpaceData.BuildName);

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
            ShowBuyTip_Text.text = "�����У�" + otherSpaceData.Price / 100;
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
