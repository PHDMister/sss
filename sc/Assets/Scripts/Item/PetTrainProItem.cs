using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class PetTrainProItem : BaseUIForm
{
    private GameObject Di_Img;
    private GameObject Toggle_Img;
    private Text Value_Text;
    private Text Gas_text;
    public train trainData;
    public int ID;
    public int Gas_Value;
    private void Awake()
    {
        Di_Img = transform.Find("DI_Img").gameObject;
        Toggle_Img = transform.Find("Toggle_Img").gameObject;
        Value_Text = transform.Find("Value_Text").gameObject.GetComponent<Text>();
        Gas_text = transform.Find("Gas_Text").gameObject.GetComponent<Text>();
        //��ʼѵ��
        RigisterButtonObjectEvent("Image", p =>
        {
            SendMessage("ClickTrain", "Success", this);
        });
    }
    //���ô�ѡ��״̬
    public void SetOpenSelectStateFun()
    {
        Di_Img.gameObject.SetActive(true);
        Toggle_Img.gameObject.SetActive(true);
    }
    //���ùر�ѡ��״̬
    public void SetCloseSelectStateFun()
    {
        Di_Img.gameObject.SetActive(false);
        Toggle_Img.gameObject.SetActive(false);
    }
    //��������
    public void SetDataFun(train train_Data)
    {
        trainData = train_Data;
        Gas_Value = train_Data.price;
        string typeName = "";
        switch (trainData.attribute_type)
        {
            case 1:
                typeName = "���+";
                break;
            case 2:
                typeName = "�ǻ�+";
                break;
            case 3:
                typeName = "����+";
                break;
        }
        Value_Text.text = typeName + trainData.attribute_min + "~" + trainData.attribute_max;
        if (trainData.price > 0)
        {
            Gas_text.text = trainData.price + "";
        }
        else
        {
            Gas_text.text = "���";
        }
    }
}
