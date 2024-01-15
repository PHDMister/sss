using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json.Linq;

public class PetTrainFinishPanel : BaseUIForm
{
    public Text GasValue_Text;
    private int remainingTime;
    PetModelRecData petModelRecData;
    private int ExpendGasValue;

    private bool isClickBtn = false;

    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        ReceiveMessage("PetTrainFinishPanelTime", p =>
        {
            petModelRecData = p.Values as PetModelRecData;
            remainingTime = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
            SetPanelDataFun();
        });
        RigisterButtonObjectEvent("PetCloseTrainArrifm_Btn", p =>
        {
            if (!isClickBtn)
            {
                return;
            }
            isClickBtn = false;
            Debug.Log("remaining: " + remainingTime);
            if (remainingTime > 0)
            {
                if (ExpendGasValue > ManageMentClass.DataManagerClass.gas_Amount)
                {
                    //gas不足
                    OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
                }
                else
                {
                    //关闭面板，且向服务发送数据
                    MessageManager.GetInstance().RequestPetTrainFinish(petModelRecData.train_info.train_record_id, (JObject jo) =>
                    {
                        //这里接收数据
                        Debug.Log("走到这里来了");

                        string jsonData = jo["data"]["train_result"].ToString();
                        Debug.Log("jsonData: " + jsonData);
                        petModelRecData.train_result = JsonUntity.FromJSON<TrainResult>(jsonData);
                        CalcTrainDataFun(jo);
                        //打开奖励面板
                        Debug.Log("结束训练");
                        SendMessage("ClosePetTrainMessage", "Success", null);
                        //关闭此面板
                        CloseUIForm();
                        SendMessage("RefreshPetTrainDataUIMessage", "Success", petModelRecData);
                    });
                }
            }
        });
        RigisterButtonObjectEvent("PetCloseTrainCancel_Btn", p =>
        {
            CloseUIForm();
        });
    }

    public override void Display()
    {
        base.Display();
        isClickBtn = true;
    }

    /// <summary>
    /// 设置面板的gas
    /// </summary>
    void SetPanelDataFun()
    {
        int minute = remainingTime / 60;
        if (remainingTime % 60 > 0)
        {
            minute += 1;
        }
        train trainData = ManageMentClass.DataManagerClass.GetTrainTableFun(petModelRecData.train_info.train_id);
        ExpendGasValue = minute * trainData.complete_cost;
        GasValue_Text.text = ExpendGasValue + "";
    }
    /// <summary>
    /// 处理数据
    /// </summary>
    /// <param name="jo"></param>
    void CalcTrainDataFun(JObject jo)
    {
        ManageMentClass.DataManagerClass.gas_Amount = (int)jo["data"]["remain_gas"];
        SendMessage("UpdataGasValue", "data", null);
        var rewardValue = jo["data"]["train_result"]["reward"];
        foreach (var item in rewardValue)
        {
            switch (item["name"].ToString())
            {
                case "成长值":
                    petModelRecData.lo_exp += (int)item["val"];
                    break;
                case "体格":
                    CalcTrainDataAttributeTyeFun(1, (int)item["val"]);
                    break;
                case "智慧":
                    CalcTrainDataAttributeTyeFun(2, (int)item["val"]);
                    break;
                case "幸运":
                    CalcTrainDataAttributeTyeFun(3, (int)item["val"]);
                    break;
            }
        }

    }
    /// <summary>
    /// 根据种类加值
    /// </summary>
    void CalcTrainDataAttributeTyeFun(int attributeTypeID, int addValue)
    {
        for (int i = 0; i < petModelRecData.pet_attribute.Count; i++)
        {
            if (petModelRecData.pet_attribute[i].attribute_type == attributeTypeID)
            {
                petModelRecData.pet_attribute[i].cur_val += addValue;
            }
        }
    }
}
