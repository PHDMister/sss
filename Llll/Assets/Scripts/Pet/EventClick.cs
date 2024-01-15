using System.Collections.Generic;
using UIFW;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击了：：" + this.name);
        if (!PetSpanManager.Instance().bOpt())
        {
            Debug.Log("非本人空间不能操作！");
            return;
        }
        if (this.gameObject.CompareTag("_TagBox"))
        {
            PetItem petItem = this.gameObject.GetComponent<PetItem>();
            if (petItem != null)
            {
                PetListRecData data = petItem.GetPetBoxData();
                Debug.Log("输出一下ID值： " + data.id + " keep ID : " + data.keep_id);
                if (data != null)
                {
                    if (data.status <= 0)
                    {
                        ToastManager.Instance.ShowPetToast("小宠已经吃饱啦，正在快速长身体哟。", 3f);
                    }
                    else
                    {
                        if (data.status == (int)PetStatus.Hunger)
                        {
                            UIManager.GetInstance().ShowUIForms(FormConst.PETFEEDTIPS_UIFORM);
                            SendMessage("HungryOpt", "Opt", data);
                        }

                        if (data.status == (int)PetStatus.Dangerous)
                        {
                            UIManager.GetInstance().ShowUIForms(FormConst.PETFEEDDANGERTIPS_UIFORM);
                            SendMessage("DangerOpt", "Opt", data);
                        }
                    }
                }
            }
        }
        if (gameObject.CompareTag("_TagPet"))
        {
            PetItem petItem = this.gameObject.GetComponent<PetItem>();
            if (petItem != null)
            {
                PetModelRecData petModelData = petItem.GetPetData();
                PetListRecData petBoxData = petItem.GetPetBoxData();

                if (petModelData != null)
                {
                    if (!petModelData.bAdopted)//未领养状态
                    {
                        PetSpanManager.Instance().LookAtPet(petBoxData.id);
                        UIManager.GetInstance().ShowUIForms(FormConst.CREATEPET_UIFORM);
                        CreatePetRecData _data = PetSpanManager.Instance().GetCreatePetData(petBoxData.id);
                        object[] args = new object[] { _data, petBoxData };
                        SendMessage("CreatePet", "Create", args);
                    }
                    else
                    {
                        if (petModelData.sleep_status == (int)PetSleepStatus.Sleep)
                        {
                            ToastManager.Instance.ShowPetToast("宠物正在休息!", 3f);
                            return;
                        }
                        MessageManager.GetInstance().RequestPetList(() =>
                        {
                            Debug.Log("这里点击了： " + petModelData.train_info != null);
                            PetSpanManager.Instance().LookAtPet(petModelData.id, petModelData.train_info != null);
                            UIManager.GetInstance().ShowUIForms(FormConst.DOGSETDATAPANEL);
                            SendMessage("RefreshDogPanel", "Opt", petModelData.id);
                        });

                    }
                }
            }
        }

        if (gameObject.CompareTag("_TagFeces"))
        {
            PetFecesItem petFecesItem = this.gameObject.GetComponent<PetFecesItem>();
            if (petFecesItem != null)
            {
                PetFeces petFeces = petFecesItem.GetPetFecesData();
                if (petFeces != null)
                {
                    if (PetSpanManager.Instance().bOpt())
                    {
                        List<int> ids = new List<int>() { petFeces.id };
                        MessageManager.GetInstance().RequestClearFeces(ids, (p) =>
                        {
                            PetSpanManager.Instance().clearId = petFeces.id;
                            Transform imgClean = PetSpanManager.Instance().GetFecesCloneTrans(petFeces.id);
                            PetSpanManager.Instance().PlayAddLoveCoinAni(imgClean, p.remain_lovecoin);
                            //ManageMentClass.DataManagerClass.loveCoin = p.remain_lovecoin;
                        });
                    }
                }
            }
        }
    }

    protected void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }

}
