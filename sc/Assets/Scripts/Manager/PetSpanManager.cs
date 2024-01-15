using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;
using System.Linq;

public enum PetStatus
{
    Normal = 0,
    Hunger = 1,
    Dangerous = 2,
    Died = 3,
    InEvolution = 4,//进化中进度条满的状态
    Evolved = 5,//已进化(已起名)
}

public enum PetType
{
    PetBox = 1,
    PetCubsMale = 2,
    PetCubsFemale = 3,
    PetMale = 4,
    PetFemale = 5,
}


public enum PetHealthType
{
    Satiation = 0,//饱食
    Clean = 1,//清洁
    Mood = 2,//心情
    Healthy = 3,//健康
}

public enum PetSleepStatus
{
    Normal = 1,//正常
    Sleep = 2,//睡眠
}

public class PetSpanManager : MonoBehaviour
{

    private Transform _TransPetSpanMgr = null;
    private static PetSpanManager _instance = null;
    private GameObject boxObj = null;
    private GameObject boxUIObj = null;
    private GameObject petObj = null;
    private GameObject petUIObj = null;
    public Dictionary<int, GameObject> dicBoxObj = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> dicBoxUIObj = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> dicPetObj = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> dicPetUIObj = new Dictionary<int, GameObject>();
    public Dictionary<int, CreatePetRecData> createPetRecData = new Dictionary<int, CreatePetRecData>();
    private float timer = 0f;
    private Camera petCamera = null;
    private GameObject joystickMgrObj = null;
    public bool bLookAting = false;
    public int selectPetId = 0;
    public LoveCoinRecData loveCoinRecData = null;
    public FecesRecData fecesRecData = null;
    public Dictionary<int, GameObject> dicFecesObj = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> dicFecesUI = new Dictionary<int, GameObject>();
    public GameObject fecesObj = null;
    public GameObject fecesUIObj = null;
    public GameObject particleObj = null;
    public bool bClear = false;
    public int clearId = 0;
    public GameObject selectPetObj = null;
    public GameObject injectedObj = null;
    public GameObject feedObj = null;
    public Dictionary<int, bool> dicEnterObstacle = new Dictionary<int, bool>();
    public Dictionary<int, bool> dicTrain = new Dictionary<int, bool>();
    public bool bBigPet = false;

    //狗的预览背景
    public GameObject dogBgObj = null;
    private Vector3 dogLastPos;

    public static PetSpanManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_PetSpanManager").AddComponent<PetSpanManager>();
        }
        return _instance;
    }

    private void Awake()
    {
        _TransPetSpanMgr = this.gameObject.transform;
    }

    private void Start()
    {

    }

    public void CloneBox(int pos, PetListRecData data)
    {
        if (data == null)
            return;
        string petModel = "";
        pet petConfig = ManageMentClass.DataManagerClass.GetPetTableFun(data.pet_type);
        if (petConfig != null)
        {
            petModel = petConfig.pet_model;
        }
        if (data.status != (int)PetStatus.Died)//死亡状态不展示箱子
        {
            bool bEvolve = (data.lo_exp == data.exp);//是否可变宠物
            if (!bEvolve)
            {
                GameObject obj;
                if (!dicBoxObj.ContainsKey(pos))
                {
                    string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, petModel);
                    obj = ResourcesMgr.GetInstance().LoadAsset(path, true);
                    obj.name = petModel;
                    dicBoxObj[pos] = obj;
                }
                else
                {
                    dicBoxObj.TryGetValue(pos, out obj);
                }

                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                }
                Debug.Log("输出一下pos： " + pos);
                obj.transform.position = ChangeRoomManager.Instance().listSpanPos[pos - 1];
                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                obj.transform.localScale = Vector3.one;
                obj.transform.SetParent(_TransPetSpanMgr);
                boxObj = obj;
                PetItem petItem = obj.GetComponent<PetItem>();
                if (petItem != null)
                {
                    if (data != null)
                    {
                        petItem.SetPetBoxData(data);
                    }
                }
                CloneBoxHud(pos);
                SetBoxData(data);
            }
            else
            {
                MessageManager.GetInstance().RequestCreatePetInfo(data.id, p =>
                {
                    if (p == null)
                        return;
                    createPetRecData[data.id] = p;
                    PetModelRecData petModelRecData = new PetModelRecData();
                    petModelRecData.pet_type = p.pet_type;
                    petModelRecData.bAdopted = false;
                    petModelRecData.id = 0;
                    petModelRecData.pet_box_id = data.id;
                    ClonePetModel(pos, petModelRecData, data);
                });
            }
        }
    }

    public void CloneBoxHud(int pos)
    {
        GameObject obj;
        if (!dicBoxUIObj.ContainsKey(pos))
        {
            obj = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_BOXUI, true);
            obj.name = "BoxUI";
            dicBoxUIObj[pos] = obj;
        }
        else
        {
            dicBoxUIObj.TryGetValue(pos, out obj);
        }


        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }

        UIFollowWorldObject uiFollowWorldObject = obj.transform.Find("UIRoot").GetComponent<UIFollowWorldObject>();
        if (uiFollowWorldObject != null)
        {
            Transform target = UnityHelper.FindTheChildNode(boxObj, "UIRoot");
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                //canvas.sortingOrder = -1;
                //obj.transform.localRotation = Quaternion.Euler(0, 180f, 0);
                //obj.transform.localScale = Vector3.one;
                //uiFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());

                canvas.renderMode = RenderMode.WorldSpace;
                obj.transform.position = target.position;


                //打开训练面板
                if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
                {
                    //
                    obj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                }
                else
                {
                    obj.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                }
                canvas.worldCamera = Camera.main;
                uiFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());
            }
        }
        boxUIObj = obj;
    }

    public void ClonePetHud(int pos)
    {
        Transform target = UnityHelper.FindTheChildNode(petObj, "UIRoot");
        GameObject obj;
        if (!dicPetUIObj.ContainsKey(pos))
        {
            obj = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_PETUI, true);
            obj.name = "PetUI";
            obj.transform.position = target.position;

            if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
            {
                obj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
            }
            else
            {
                obj.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
            }
            //obj.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            //obj.transform.localScale = Vector3.one;
            dicPetUIObj[pos] = obj;
        }
        else
        {
            dicPetUIObj.TryGetValue(pos, out obj);
        }

        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }

        UIFollowWorldObject uiFollowWorldObject = obj.transform.Find("UIRoot").GetComponent<UIFollowWorldObject>();
        if (uiFollowWorldObject != null)
        {
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                //canvas.sortingOrder = -1;
                //uiFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());
                if (bInAidStations())
                {
                    canvas.sortingOrder = 1;
                }
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
                uiFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());
            }
        }
        petUIObj = obj;
    }

    public void SetBoxData(PetListRecData data = null)
    {
        if (boxUIObj == null)
            return;

        Transform btnHungry = UnityHelper.FindTheChildNode(boxUIObj, "BtnHungry");
        Transform btnDanger = UnityHelper.FindTheChildNode(boxUIObj, "BtnDanger");

        btnHungry.GetComponent<Button>().enabled = bOpt();//ManageMentClass.DataManagerClass.is_Owner;
        btnDanger.GetComponent<Button>().enabled = bOpt();//ManageMentClass.DataManagerClass.is_Owner;

        EventTriggerListener.Get(btnHungry.gameObject).onClick = p =>
        {
            Debug.Log("OnClickHungry");
            if (bOpt())
            {
                UIManager.GetInstance().ShowUIForms(FormConst.PETFEEDTIPS_UIFORM);
                SendMessage("HungryOpt", "Opt", data);
            }
        };

        EventTriggerListener.Get(btnDanger.gameObject).onClick = p =>
        {
            Debug.Log("OnClickDanger");
            if (bOpt())
            {
                UIManager.GetInstance().ShowUIForms(FormConst.PETFEEDDANGERTIPS_UIFORM);
                SendMessage("DangerOpt", "Opt", data);
            }
        };

        if (data == null)
            return;
        switch (data.status)
        {
            case (int)PetStatus.Normal://正常
                if (btnHungry != null)
                    btnHungry.gameObject.SetActive(false);
                if (btnDanger != null)
                    btnDanger.gameObject.SetActive(false);
                break;
            case (int)PetStatus.Hunger://饥饿
                if (btnHungry != null)
                    btnHungry.gameObject.SetActive(true);
                if (btnDanger != null)
                    btnDanger.gameObject.SetActive(false);
                break;
            case (int)PetStatus.Dangerous://濒危
                if (btnHungry != null)
                    btnHungry.gameObject.SetActive(false);
                if (btnDanger != null)
                    btnDanger.gameObject.SetActive(true);
                break;
            case (int)PetStatus.Died://死亡
                if (btnHungry != null)
                    btnHungry.gameObject.SetActive(false);
                if (btnDanger != null)
                    btnDanger.gameObject.SetActive(false);
                break;
        }

        Image m_ImgSlider = UnityHelper.FindTheChildNode(boxUIObj, "Slider").GetComponent<Image>();
        if (m_ImgSlider != null)
        {
            if (data.lo_exp == 1)
            {
                /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/PetHUD");
                  Sprite sprite = atlas.GetSprite("Rectangle30_1");*/
                m_ImgSlider.sprite = ManageMentClass.ResourceControllerClass.ResLoadPetHUDByPathNameFun("Rectangle30_1");
                m_ImgSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(26f, 26f);
            }
            else
            {
                /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/PetHUD");
                 Sprite sprite = atlas.GetSprite("Rectangle30");*/
                m_ImgSlider.sprite = ManageMentClass.ResourceControllerClass.ResLoadPetHUDByPathNameFun("Rectangle30");
                float value = (float)data.lo_exp / (float)data.exp;
                m_ImgSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(212f * value, 26f);
            }
            //m_ImgSlider.fillAmount = (float)data.lo_exp / (float)data.exp;
        }
        Text m_TextSlider = UnityHelper.FindTheChildNode(boxUIObj, "TextProgress").GetComponent<Text>();
        if (m_TextSlider != null)
        {
            m_TextSlider.text = string.Format("成长进度 {0}/{1}", data.lo_exp, data.exp);
        }
    }

    public void SetPetUIData(PetModelRecData data, PetListRecData boxData = null)
    {
        if (petUIObj == null)
            return;

        Transform btnStatus = UnityHelper.FindTheChildNode(petUIObj, "BtnStatus");
        Transform btnAdopt = UnityHelper.FindTheChildNode(petUIObj, "BtnAdopt");
        Transform nameTrans = UnityHelper.FindTheChildNode(petUIObj, "ImgName");
        Text nameText = UnityHelper.FindTheChildNode(petUIObj, "TextName").GetComponent<Text>();
        Image statusImg = UnityHelper.FindTheChildNode(petUIObj, "ImgStatus").GetComponent<Image>();

        btnStatus.gameObject.SetActive(data.bAdopted);
        btnAdopt.gameObject.SetActive(!data.bAdopted);
        nameTrans.gameObject.SetActive(data.bAdopted);

        btnStatus.GetComponent<Button>().enabled = bOpt();//ManageMentClass.DataManagerClass.is_Owner;
        btnAdopt.GetComponent<Button>().enabled = bOpt();//ManageMentClass.DataManagerClass.is_Owner;

        bool bReceive = IsReceiveLoveCoin(data.id);
        if (bReceive)//已领取爱心币
        {
            EventTriggerListener.Get(btnStatus.gameObject).onClick = p =>
            {
                if (bOpt())
                {

                    MessageManager.GetInstance().RequestPetList(() =>
                    {
                        PetSpanManager.Instance().LookAtPet(data.id);
                        Debug.Log("    这里打开了： " + data.train_info != null);
                        UIManager.GetInstance().ShowUIForms(FormConst.DOGSETDATAPANEL, data.train_info != null);
                        SendMessage("RefreshDogPanel", "Opt", data.id);
                    });


                }
            };
        }
        else
        {
            EventTriggerListener.Get(btnStatus.gameObject).onClick = p =>
            {
                if (bOpt())
                {
                    MessageManager.GetInstance().RequestLoveCoinReceive(data.id, (p) =>
                     {
                         PlayLoveCoinAni(data.id, p.remain_lovecoin);
                         RemovePetLoveCoinData(data.id);
                         UpdatePetStatus(data.id);
                         //ManageMentClass.DataManagerClass.loveCoin = p.remain_lovecoin;
                         //SendMessage("UpdateLoveCoin", "Value", null);
                     });
                }
            };
        }


        EventTriggerListener.Get(btnAdopt.gameObject).onClick = p =>
        {
            if (bOpt())
            {
                if (!data.bAdopted)
                {
                    PetSpanManager.Instance().LookAtPet(boxData.id);
                }
                else
                {
                    PetSpanManager.Instance().LookAtPet(data.id);
                }
                UIManager.GetInstance().ShowUIForms(FormConst.CREATEPET_UIFORM);
                CreatePetRecData _data = GetCreatePetData(boxData.id);
                object[] args = new object[] { _data, boxData };
                SendMessage("CreatePet", "Create", args);
            }
        };

        if (data == null || !data.bAdopted)
            return;
        nameText.text = data.pet_name;
        Dictionary<int, bool> healthStatusDic = new Dictionary<int, bool>();
        for (int i = 0; i < data.pet_condition.Count; i++)
        {
            int curValue = data.pet_condition[i].cur_val;
            int conditionType = data.pet_condition[i].condition_type;

            petcondition petconditionTab = ManageMentClass.DataManagerClass.GetPetConditionTable(data.pet_type, conditionType);
            int goodMin = petconditionTab.good_min;
            int gootMax = petconditionTab.good_max;
            int poorMin = petconditionTab.poor_min;
            int poorMax = petconditionTab.poor_max;
            int badMin = petconditionTab.bad_min;
            int badMax = petconditionTab.bad_max;

            bool bGood = curValue >= goodMin;
            bool bPoor = curValue >= poorMin && curValue <= poorMax;
            bool bbad = curValue >= badMin && curValue <= badMax;

            if (!healthStatusDic.ContainsKey(conditionType))
            {
                healthStatusDic.Add(conditionType, bGood);
            }
        }
        //是否全部状态都健康
        bool bHealth = true;
        foreach (var status in healthStatusDic)
        {
            if (!status.Value)
            {
                bHealth = false;
                break;
            }
        }

        btnStatus.gameObject.SetActive((data.bAdopted && !bHealth) || !bReceive);
        int[] SortPriority = new int[4] { (int)PetHealthType.Healthy, (int)PetHealthType.Satiation, (int)PetHealthType.Clean, (int)PetHealthType.Mood };
        int curStatus = 0;
        for (int i = 0; i < SortPriority.Length; i++)
        {
            if (healthStatusDic.ContainsKey(SortPriority[i]))
            {
                bool bNormal;
                healthStatusDic.TryGetValue(SortPriority[i], out bNormal);
                if (!bNormal)
                {
                    curStatus = SortPriority[i];
                    break;
                }
            }
        }

        string spriteName = "";
        switch (curStatus)
        {
            case (int)PetHealthType.Satiation:
                spriteName = string.Format("{0}", "icon-WW=EIS2");
                break;
            case (int)PetHealthType.Clean:
                spriteName = string.Format("{0}", "icon-SHUAZ2");
                break;
            case (int)PetHealthType.Mood:
                spriteName = string.Format("{0}", "icon-WANJU2");
                break;
            case (int)PetHealthType.Healthy:
                spriteName = string.Format("{0}", "icon-YAO2");
                break;
        }

        if (curStatus == (int)PetHealthType.Clean)
        {
            SetDirtyTexture(data.id);
        }
        else
        {
            SetNormalTexture(data.id);
        }

        if (!bReceive)
        {
            /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
             Sprite sprite = atlas.GetSprite("aixin");*/
            statusImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun("aixin");
        }
        else
        {
            /*     SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/PetHUD");
                 Sprite sprite = atlas.GetSprite(spriteName);*/
            statusImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadPetHUDByPathNameFun(spriteName);
        }

        GameObject petObj = GetCurPetObj(data.id);
        Animator animator = petObj.GetComponent<Animator>();

        //训练中
        bool bTrain = PetSpanManager.Instance().IsTraining(data.id);
        //睡眠中
        bool bSleep = data.sleep_status == (int)PetSleepStatus.Sleep;

        //低落状态
        if (!bHealth)
        {
            if (petObj != null)
            {
                if (animator != null)
                {
                    SetTrigger(animator, "Low");
                }
            }
        }
        else if (bTrain)
        {
            PetSpanManager.Instance().SetBeginTrainPetRigidbody(data.id);
            PetSpanManager.Instance().PlayPetAni(data.id, (int)PetStateAnimationType.Train);
        }
        else if (bSleep)
        {
            PetSpanManager.Instance().PlayPetAni(data.id, (int)PetStateAnimationType.Sleep);
        }
        else
        {
            if (petObj != null)
            {
                if (animator != null)
                {
                    SetTrigger(animator, "Idle");
                }
            }
        }

        //if (!string.IsNullOrEmpty(data.pet_name))
        //{
        //    RectTransform textRect = UnityHelper.FindTheChildNode(petUIObj, "TextName").GetComponent<RectTransform>();
        //    RectTransform imgRect = UnityHelper.FindTheChildNode(petUIObj, "ImgName").GetComponent<RectTransform>();

        //    if (textRect != null)
        //    {
        //        LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);
        //        HorizontalLayoutGroup horizontalLayoutGroup = imgRect.GetComponent<HorizontalLayoutGroup>();
        //        float width = textRect.rect.width + horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right;
        //        Vector3 pos1 = btnStatus.GetComponent<RectTransform>().anchoredPosition;
        //        Vector3 pos2 = btnAdopt.GetComponent<RectTransform>().anchoredPosition;
        //        btnStatus.GetComponent<RectTransform>().anchoredPosition = new Vector3(140f - (1 - width / 175f) * 140f + width / 2, pos1.y, pos1.z);
        //        btnAdopt.GetComponent<RectTransform>().anchoredPosition = new Vector3(140f - (1 - width / 175f) * 140f + width / 2, pos2.y, pos2.z);
        //    }
        //}

    }

    public void OnClickDanger()
    {

    }

    private void Update()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 2)
        {
            LoopRequestPetList();
        }
    }

    public void LoopPlayAni()
    {
        InvokeRepeating("PlayBoxAnimation", 5f, 5f);
    }
    public void PlayBoxAnimation()
    {
        if (dicBoxObj == null)
            return;
        foreach (var box in dicBoxObj)
        {
            if (box.Value.activeSelf)
            {
                Animator animator = box.Value.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play("Take 001", 0, 0f);
                }
            }
        }
    }

    public void Hide()
    {
        if (dicBoxObj != null)
        {
            foreach (var box in dicBoxObj)
            {
                if (box.Value.activeSelf)
                {
                    box.Value.SetActive(false);
                }
            }
        }

        if (dicPetObj != null)
        {
            foreach (var pet in dicPetObj)
            {
                if (pet.Value.activeSelf)
                {
                    pet.Value.SetActive(false);
                }
            }
        }

        if (dicBoxUIObj != null)
        {
            foreach (var boxUI in dicBoxUIObj)
            {
                if (boxUI.Value.activeSelf)
                {
                    boxUI.Value.SetActive(false);
                }
            }
        }

        if (dicPetUIObj != null)
        {
            foreach (var petUI in dicPetUIObj)
            {
                if (petUI.Value.activeSelf)
                {
                    petUI.Value.SetActive(false);
                }
            }
        }
    }

    public void Clear()
    {
        if (dicBoxObj != null)
        {
            foreach (var box in dicBoxObj)
            {
                DestroyImmediate(box.Value);
            }
            dicBoxObj.Clear();
        }

        if (dicPetObj != null)
        {
            foreach (var pet in dicPetObj)
            {
                DestroyImmediate(pet.Value);
            }
            dicPetObj.Clear();
        }

        if (dicBoxUIObj != null)
        {
            foreach (var boxUI in dicBoxUIObj)
            {
                DestroyImmediate(boxUI.Value);
            }
            dicBoxUIObj.Clear();
        }

        if (dicPetUIObj != null)
        {
            foreach (var petUI in dicPetUIObj)
            {
                DestroyImmediate(petUI.Value);
            }
            dicPetUIObj.Clear();
        }

        if (dicFecesObj != null)
        {
            foreach (var item in dicFecesObj)
            {
                DestroyImmediate(item.Value);
            }
            dicFecesObj.Clear();
        }

        if (dicFecesUI != null)
        {
            foreach (var item in dicFecesUI)
            {
                DestroyImmediate(item.Value);
            }
            dicFecesUI.Clear();
        }
    }
    protected void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }

    /// <summary>
    /// 进化后Id
    /// </summary>
    /// <param name="petId"></param>
    public bool IsEvolved(int petId)
    {
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.pet_box_id == petId)
            {
                return true;
            }
        }
        return false;
    }

    public PetModelRecData GetPetModelData(int petId, int pet_box_id = 0)
    {
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.id == petId)
            {
                return item.Value;
            }
        }
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.pet_box_id == pet_box_id)
            {
                return item.Value;
            }
        }
        return null;
    }

    public int GetPetModelPos(int petId)
    {
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.id == petId)
            {
                return item.Key;
            }
        }
        return 0;
    }

    public PetModelRecData GetPetModelDataByBoxId(int boxId)
    {
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.pet_box_id == boxId)
            {
                return item.Value;
            }
        }
        return null;
    }

    public int GetPetModelPos(PetModelRecData petModelRecData)
    {
        int petPos = 0;
        if (petModelRecData != null)
        {
            foreach (var box in ManageMentClass.DataManagerClass.petListDataDic)
            {
                if (box.Value.id == petModelRecData.pet_box_id)
                {
                    petPos = box.Key;
                    break;
                }
            }
        }
        return petPos;
    }

    public void ClonePetModel(int pos, PetModelRecData data, PetListRecData boxData = null)
    {
        if (data == null)
            return;
        string petModel = "";
        pet petConfig = ManageMentClass.DataManagerClass.GetPetTableFun(data.pet_type);
        if (petConfig != null)
        {
            petModel = petConfig.pet_model;
        }
        GameObject obj;
        Debug.Log(" 序号内容： " + pos);
        if (!dicPetObj.ContainsKey(pos))
        {
            string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, petModel);
            obj = ResourcesMgr.GetInstance().LoadAsset(path, true);
            obj.name = petModel;
            obj.transform.position = ChangeRoomManager.Instance().listSpanPos[pos - 1];
            obj.transform.localRotation = Quaternion.Euler(0, 180, 0);
            obj.transform.SetParent(_TransPetSpanMgr);
            dicPetObj[pos] = obj;
        }
        else
        {
            if (!bBigPet)
            {
                dicPetObj.TryGetValue(pos, out obj);
            }
            else
            {
                string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, petModel);
                obj = ResourcesMgr.GetInstance().LoadAsset(path, true);
                obj.name = petModel;
                obj.transform.position = ChangeRoomManager.Instance().listSpanPos[pos - 1];
                obj.transform.localRotation = Quaternion.Euler(0, 180, 0);
                obj.transform.SetParent(_TransPetSpanMgr);
                dicPetObj[pos] = obj;
            }
        }

        petObj = obj;
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }

        PetItem petItem = obj.GetComponent<PetItem>();
        if (petItem != null)
        {
            if (data != null)
            {
                petItem.SetPetData(data);
            }
            if (boxData != null)
            {
                petItem.SetPetBoxData(boxData);
            }
        }

        ClonePetHud(pos);
        SetPetUIData(data, boxData);
    }

    public void UpdatePet()
    {
        Hide();
        //宠物盒
        bool bDied = false;
        foreach (var box in ManageMentClass.DataManagerClass.petListDataDic)
        {
            if (box.Value.status < (int)PetStatus.Evolved)
            {
                if (box.Value.status == (int)PetStatus.Died)
                {
                    bDied = true;
                }
                CloneBox(box.Key, box.Value);
            }
            else
            {
                if (box.Value.status != (int)PetStatus.Evolved)//已进化不做处理
                {
                    PetModelRecData petModelRecData = GetPetModelDataByBoxId(box.Value.id);
                    if (petModelRecData != null)
                    {
                        ClonePetModel(box.Key, petModelRecData);
                    }
                }
            }
        }
        if (bDied)
        {
            UIManager.GetInstance().ShowUIForms(FormConst.PETFEEDDIETIPS_UIFORM);
        }
        LoopPlayAni();

        //宠物
        bool bRecycle = false;
        string petName = "";
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.status == (int)PetStatus.Died)
            {
                bRecycle = true;
                petName = item.Value.pet_name;
            }
            else
            {
                if (item.Value.status != (int)PetStatus.Evolved)
                {
                    ClonePetModel(item.Key, item.Value);
                }
            }
        }
        if (bRecycle)
        {
            UIManager.GetInstance().ShowUIForms(FormConst.PETFEEDDIETIPS_UIFORM);
            SendMessage("RecyclePet", "petdied", petName);
        }

        //聚焦宠物时隐藏头顶信息
        if (bLookAting)
        {
            if (selectPetId > 0)
            {
                SetSelectPetVisible(selectPetId, true);
            }
            SetHeadUIVisable(false);
        }
    }

    public CreatePetRecData GetCreatePetData(int petId)
    {
        CreatePetRecData data;
        createPetRecData.TryGetValue(petId, out data);
        return data;
    }

    public void LoopRequestPetList()
    {
        if (ManageMentClass.DataManagerClass.petModelRecData.Count <= 0)
            return;
        int maxConsumeTime = 60;//默认60s刷一次
        //foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        //{
        //    for (int j = 0; j < item.Value.pet_condition.Count; j++)
        //    {
        //        pet_consume petConsume = ManageMentClass.DataManagerClass.GetPetConsumeTable(item.Value.pet_type, item.Value.pet_condition[j].condition_type);
        //        if (petConsume != null)
        //        {
        //            int consumeTime;
        //            int.TryParse(petConsume.consume_time, out consumeTime);
        //            if (consumeTime > maxConsumeTime)
        //            {
        //                maxConsumeTime = consumeTime;
        //            }
        //        }
        //    }
        //}
        if (timer < maxConsumeTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            RequestPetList();
            RequestPetFecesList();
        }
    }

    public void RequestPetList()
    {
        MessageManager.GetInstance().RequestPetList(() =>
        {
            MessageManager.GetInstance().RequestLoveCoinList((p) =>
            {
                loveCoinRecData = p;
                if (loveCoinRecData != null)
                {
                    ManageMentClass.DataManagerClass.loveCoin = loveCoinRecData.remain_lovecoin;
                }
                PetSpanManager.Instance().UpdatePet();
            });

            SendMessage("LoopUpdatePetData", "Opt", null);
        });
    }

    public void RequestPetFecesList()
    {
        MessageManager.GetInstance().RequestFeces((p) =>
        {
            fecesRecData = p;
            if (fecesRecData == null || fecesRecData.list == null)
                return;
            UpdateFeces(fecesRecData);
        });
    }

    public void HideFeces()
    {
        if (dicFecesObj != null)
        {
            foreach (var item in dicFecesObj)
            {
                if (item.Value.activeSelf)
                {
                    item.Value.SetActive(false);
                }
            }
        }

        if (dicFecesUI != null)
        {
            foreach (var item in dicFecesUI)
            {
                if (item.Value.activeSelf)
                {
                    item.Value.SetActive(false);
                }
            }
        }
    }

    public void UpdateFeces(FecesRecData fecesRecData)
    {
        HideFeces();
        for (int i = 0; i < fecesRecData.list.Count; i++)
        {
            if (i < 10)//防止越界
            {
                CloneFeces(i, fecesRecData.list[i]);
            }
        }
    }
    public void LookAtPet(int petId, bool isActive = false)
    {
        SetSelectPetVisible(petId, true);
        SetHeadUIVisable(false);
        bLookAting = true;
        selectPetId = petId;

        if (petCamera == null)
        {
            petCamera = new GameObject("PetCamera").AddComponent<Camera>();
        }
        petCamera.GetUniversalAdditionalCameraData().SetRenderer(0);

        if (selectPetObj != null)
        {
            selectPetObj.transform.localRotation = Quaternion.Euler(0f, 170f, 0f);
            /*  if (!bInAidStations())
              {
                  selectPetObj.transform.localRotation = Quaternion.Euler(0f, 170f, 0f);
              }
              else
              {
                  selectPetObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
              }*/
            dogLastPos = new Vector3(selectPetObj.transform.localPosition.x, selectPetObj.transform.localPosition.y, selectPetObj.transform.localPosition.z);

            Debug.Log("是否打开了：  " + isActive);

            selectPetObj.transform.localPosition = new Vector3(0f, 0f, -2f);

            SetDogBgObjActiveFun(true);
            ChangeRoomManager.Instance().SetDogRoomBgTextureFun(ChangeRoomManager.Instance().GetNowRoomID());
            Debug.Log("输出一下具体内容值： 宠物");
            if (isActive)
            {
                petCamera.transform.position = new Vector3(selectPetObj.transform.position.x + 0.22f, selectPetObj.transform.position.y + 0.4f, selectPetObj.transform.position.z - 1.5f);

            }
            else
            {
                petCamera.transform.position = new Vector3(selectPetObj.transform.position.x + 0.22f, selectPetObj.transform.position.y + 0.4f, selectPetObj.transform.position.z - 1f);

            }
            petCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            petCamera.cullingMask = /*(1 << 0) +*/ (1 << 11);

            PetItem petItem = selectPetObj.GetComponent<PetItem>();
            Animator animator = selectPetObj.GetComponent<Animator>();
            if (petItem != null)
            {
                PetModelRecData petModelRecData = petItem.GetPetData();
                int key = GetPetModelPos(petModelRecData.id);
                ManageMentClass.DataManagerClass.petModelRecData.TryGetValue(key, out petModelRecData);
                if (petModelRecData != null)
                {
                    bool bHealth = PetSpanManager.Instance().IsHealth(petModelRecData);
                    //训练中
                    bool bTrain = PetSpanManager.Instance().IsTraining(petModelRecData.id);
                    //睡眠中
                    bool bSleep = petModelRecData.sleep_status == (int)PetSleepStatus.Sleep;
                    if (!bHealth)
                    {
                        if (animator != null)
                        {
                            SetTrigger(animator, "Low");
                        }
                    }
                    else if (bTrain)
                    {
                        PetSpanManager.Instance().SetBeginTrainPetRigidbody(petModelRecData.id);
                        PetSpanManager.Instance().PlayPetAni(petModelRecData.id, (int)PetStateAnimationType.Train);
                    }
                    else if (bSleep)
                    {
                        PetSpanManager.Instance().PlayPetAni(petModelRecData.id, (int)PetStateAnimationType.Sleep);
                    }
                    else
                    {
                        if (animator != null)
                        {
                            SetTrigger(animator, "Idle");
                        }
                    }
                }
            }
        }

        if (ManageMentClass.DataManagerClass.PlatformType != 1)
        {
            if (!bInAidStations())
            {
                //禁用摇杆
                joystickMgrObj = JoystickManager.Instance().GetJoystickObj().gameObject;
                if (joystickMgrObj != null)
                {
                    joystickMgrObj.SetActive(false);
                }
            }
        }
    }


    public void SetCameraTrainFun()
    {
        if (petCamera != null && selectPetObj != null)
        {
            petCamera.transform.position = new Vector3(selectPetObj.transform.position.x + 0.22f, selectPetObj.transform.position.y + 0.4f, selectPetObj.transform.position.z - 1f);

        }
    }

    public void SetSelectPetVisible(int petId, bool bShow, bool bAllShow = false)
    {
        foreach (var obj in dicPetObj)
        {
            PetItem petItem = obj.Value.GetComponent<PetItem>();
            if (petItem != null)
            {
                PetModelRecData petModelRecData = petItem.GetPetData();
                PetListRecData petListRecData = petItem.GetPetBoxData();
                if (petModelRecData != null && petModelRecData.id > 0)
                {
                    if (!bAllShow)
                    {
                        if (petModelRecData.id == petId)
                        {
                            selectPetObj = obj.Value;
                            obj.Value.SetActive(bShow);
                        }
                        else
                        {
                            obj.Value.SetActive(!bShow);
                        }
                    }
                    else
                    {
                        if (!obj.Value.activeSelf)
                        {
                            obj.Value.SetActive(bAllShow);
                        }
                    }
                }
                else
                {
                    if (petListRecData != null)//未变宠物起名时
                    {
                        if (!bAllShow)
                        {
                            if (petModelRecData.pet_box_id == petId)
                            {
                                selectPetObj = obj.Value;
                                obj.Value.SetActive(bShow);
                            }
                            else
                            {
                                obj.Value.SetActive(!bShow);
                            }
                        }
                        else
                        {
                            if (!obj.Value.activeSelf)
                            {
                                obj.Value.SetActive(bAllShow);
                            }
                        }
                    }
                }
            }
        }
    }

    public void CancelLookAtPet()
    {
        SetSelectPetVisible(0, false, true);
        SetHeadUIVisable(true);
        SetDogBgObjActiveFun(false);
        bLookAting = false;
        selectPetId = 0;
        selectPetObj.transform.localPosition = dogLastPos;

        if (petCamera != null)
        {
            DestroyImmediate(petCamera.gameObject);
        }

        if (ManageMentClass.DataManagerClass.PlatformType != 1)
        {
            if (!bInAidStations())
            {
                //启用摇杆
                if (joystickMgrObj != null)
                {
                    joystickMgrObj.SetActive(true);
                }

                //重置旋转角度
                if (selectPetObj != null)
                {
                    //  selectPetObj.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
                }
            }
            else
            {
                //重置旋转角度
                if (selectPetObj != null)
                {
                    // selectPetObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }


    }

    public void UpdatePetStatus(int petId)
    {
        PetModelRecData petModelRecData = GetPetModelData(petId);
        if (petModelRecData == null)
            return;
        int pos = GetPetModelPos(petId);
        dicPetUIObj.TryGetValue(pos, out petUIObj);
        if (petUIObj != null)
        {
            SetPetUIData(petModelRecData);
        }
    }

    public int GetPetNum()
    {
        int num = 0;
        foreach (var item in ManageMentClass.DataManagerClass.petListDataDic)
        {
            if (item.Value.status != (int)PetStatus.Evolved && item.Value.status != (int)PetStatus.Died)
            {
                num++;
            }
        }
        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.status != (int)PetStatus.Evolved && item.Value.status != (int)PetStatus.Died)
            {
                num++;
            }
        }
        return num;
    }

    public bool IsReceiveLoveCoin(int pet_id)
    {
        bool bReceive = true;
        if (loveCoinRecData == null || loveCoinRecData.list == null)
            return true;
        for (int i = 0; i < loveCoinRecData.list.Count; i++)
        {
            if (loveCoinRecData.list[i].pet_id == pet_id && loveCoinRecData.list[i].lovecoin > 0)
            {
                bReceive = false;
                break;
            }
        }

        return bReceive;
    }

    public void RemovePetLoveCoinData(int pet_id)
    {
        if (loveCoinRecData == null || loveCoinRecData.list == null)
            return;
        for (int i = loveCoinRecData.list.Count - 1; i >= 0; i--)
        {
            if (loveCoinRecData.list[i].pet_id == pet_id && loveCoinRecData.list[i].lovecoin > 0)
            {
                loveCoinRecData.list.RemoveAt(i);
                break;
            }
        }
    }

    public void PlayLoveCoinAni(int pet_id, int newValue)
    {
        Debug.Log("PlayLoveCoinAni   oldValue = " + ManageMentClass.DataManagerClass.loveCoin + "    newValue = " + newValue);
        if (newValue > ManageMentClass.DataManagerClass.loveCoin)
        {
            StartCoroutine(LoopCreateLoveCoin(pet_id, newValue));
        }
    }

    public void CreateLoveCoin(int pet_id, int newValue)
    {
        GameObject petUIObj;
        int pos = GetPetModelPos(pet_id);
        dicPetUIObj.TryGetValue(pos, out petUIObj);
        if (petUIObj != null)
        {
            Image statusImg = UnityHelper.FindTheChildNode(petUIObj, "ImgStatus").GetComponent<Image>();
            Canvas canvas = new GameObject("LoveCoinCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            GameObject loveCoinObj = Instantiate(statusImg.gameObject);
            loveCoinObj.name = "Coin";
            loveCoinObj.SetActive(true);
            Image loveImg = loveCoinObj.GetComponent<Image>();
            if (loveImg != null)
            {
                /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
                 Sprite sprite = atlas.GetSprite("aixin");*/

                loveImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun("aixin");
                loveImg.SetNativeSize();
            }

            Vector3 uiPos = new Vector3();
            Canvas cloneTransCanvas = statusImg.transform.GetComponentInParent<Canvas>();
            if (cloneTransCanvas != null)
            {
                if (cloneTransCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    uiPos = statusImg.transform.position;
                }
                else if (cloneTransCanvas.renderMode == RenderMode.WorldSpace)
                {
                    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, statusImg.transform.position);
                    RectTransform rt = canvas.transform.GetComponent<RectTransform>();
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPos, null, out uiPos);
                }
            }
            loveCoinObj.transform.position = uiPos;
            loveCoinObj.transform.localScale = Vector3.one;
            loveCoinObj.transform.parent = canvas.transform;

            Vector3 endPos = new Vector3();
            if (!bInAidStations())
            {
                PetdensUIForm petdensUIForm = UIManager.GetInstance().GetUIForm(FormConst.PETDENS_UIFORM) as PetdensUIForm;
                if (petdensUIForm != null)
                {
                    if (petdensUIForm.m_ImgLoveIcon != null)
                    {
                        endPos = petdensUIForm.m_ImgLoveIcon.transform.position;
                    }
                }
            }
            else
            {
                RescueStationUIForm rescueStationUIForm = UIManager.GetInstance().GetUIForm(FormConst.RESCUESTATION) as RescueStationUIForm;
                if (rescueStationUIForm != null)
                {
                    if (rescueStationUIForm.m_ImgLoveIcon != null)
                    {
                        endPos = rescueStationUIForm.m_ImgLoveIcon.transform.position;
                    }
                }
            }

            loveCoinObj.transform.DOMove(endPos, 2f).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                Destroy(canvas.gameObject);
                UpdateLoveCoinValue(newValue, 1f);
            });
            loveCoinObj.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 2f).SetEase(Ease.InOutCubic);
        }
    }

    IEnumerator LoopCreateLoveCoin(int pet_id, int newValue)
    {
        for (int i = 0; i < 5; i++)
        {
            CreateLoveCoin(pet_id, newValue);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateLoveCoinValue(int newValue, float duration)
    {
        if (!bInAidStations())
        {
            PetdensUIForm petdensUIForm = UIManager.GetInstance().GetUIForm(FormConst.PETDENS_UIFORM) as PetdensUIForm;
            if (petdensUIForm != null)
            {
                if (petdensUIForm.m_TextLove != null)
                {
                    int oldValue = ManageMentClass.DataManagerClass.loveCoin;
                    string txtStr = InterfaceHelper.GetCoinDisplay(oldValue);
                    int addValue = newValue - oldValue;
                    Debug.Log("oldValue = " + oldValue + "  newValue = " + newValue);
                    if (addValue > 0)
                    {
                        DOTween.To(delegate (float value)
                        {
                            var temp = Math.Floor(value);
                            petdensUIForm.m_TextLove.text = string.Format("{0}+{1}", txtStr, temp.ToString());
                        }, 0, addValue, duration).OnComplete(() =>
                        {
                            petdensUIForm.m_TextLove.text = InterfaceHelper.GetCoinDisplay(newValue);
                            ManageMentClass.DataManagerClass.loveCoin = newValue;
                        });
                    }
                }
            }
        }
        else
        {
            RescueStationUIForm petdensUIForm = UIManager.GetInstance().GetUIForm(FormConst.RESCUESTATION) as RescueStationUIForm;
            if (petdensUIForm != null)
            {
                if (petdensUIForm.m_TextLove != null)
                {
                    int oldValue = ManageMentClass.DataManagerClass.loveCoin;
                    string txtStr = InterfaceHelper.GetCoinDisplay(oldValue);
                    int addValue = newValue - oldValue;
                    Debug.Log("oldValue = " + oldValue + "  newValue = " + newValue);
                    if (addValue > 0)
                    {
                        DOTween.To(delegate (float value)
                        {
                            var temp = Math.Floor(value);
                            petdensUIForm.m_TextLove.text = string.Format("{0}+{1}", txtStr, temp.ToString());
                        }, 0, addValue, duration).OnComplete(() =>
                        {
                            petdensUIForm.m_TextLove.text = InterfaceHelper.GetCoinDisplay(newValue);
                            ManageMentClass.DataManagerClass.loveCoin = newValue;
                        });
                    }
                }
            }
        }
    }

    public void PlayAddLoveCoinAni(Transform cloneTrans, int newValue)
    {
        Debug.Log("PlayAddLoveCoinAni   oldValue = " + ManageMentClass.DataManagerClass.loveCoin + "    newValue = " + newValue);
        if (newValue > ManageMentClass.DataManagerClass.loveCoin)
        {
            StartCoroutine(LoopCloneLoveCoin(cloneTrans, newValue));
        }
    }

    IEnumerator LoopCloneLoveCoin(Transform trans, int newValue)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 4)
            {
                bClear = true;
            }
            CloneLoveCoin(trans, newValue);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void CloneLoveCoin(Transform cloneTrans, int newValue)
    {
        if (cloneTrans == null)
            return;
        Canvas canvas = new GameObject("LoveCoinCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;
        GameObject loveCoinObj = Instantiate(cloneTrans.gameObject);
        loveCoinObj.name = "Coin";
        loveCoinObj.SetActive(true);
        if (loveCoinObj.transform.childCount > 0)
        {
            for (int i = 0; i < loveCoinObj.transform.childCount; i++)
            {
                Transform child = loveCoinObj.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
        Image loveImg = loveCoinObj.GetComponent<Image>();
        if (loveImg != null)
        {
            /*   SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
               Sprite sprite = atlas.GetSprite("aixin");*/
            loveImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun("aixin");
            loveImg.SetNativeSize();
        }

        Vector3 uiPos = new Vector3();
        Canvas cloneTransCanvas = cloneTrans.GetComponentInParent<Canvas>();
        if (cloneTransCanvas != null)
        {
            if (cloneTransCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiPos = cloneTrans.transform.position;
            }
            else if (cloneTransCanvas.renderMode == RenderMode.WorldSpace)
            {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, cloneTrans.transform.position);
                RectTransform rt = canvas.transform.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, pos, null, out uiPos);
            }
        }
        loveCoinObj.transform.position = uiPos;
        loveCoinObj.transform.localScale = Vector3.one;
        loveCoinObj.transform.parent = canvas.transform;

        Vector3 endPos = new Vector3();
        if (!bInAidStations())
        {
            PetdensUIForm petdensUIForm = UIManager.GetInstance().GetUIForm(FormConst.PETDENS_UIFORM) as PetdensUIForm;
            if (petdensUIForm != null)
            {
                if (petdensUIForm.m_ImgLoveIcon != null)
                {
                    endPos = petdensUIForm.m_ImgLoveIcon.transform.position;
                }
            }
        }
        else
        {
            RescueStationUIForm rescueStationUIForm = UIManager.GetInstance().GetUIForm(FormConst.RESCUESTATION) as RescueStationUIForm;
            if (rescueStationUIForm != null)
            {
                if (rescueStationUIForm.m_ImgLoveIcon != null)
                {
                    endPos = rescueStationUIForm.m_ImgLoveIcon.transform.position;
                }
            }
        }
        loveCoinObj.transform.DOMove(endPos, 2f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            Destroy(canvas.gameObject);
            UpdateLoveCoinValue(newValue, 1f);
            if (bClear)
            {
                if (clearId > 0)
                {
                    DestroyFecesUI(clearId);
                    bClear = false;
                }
            }
        });
        loveCoinObj.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 2f).SetEase(Ease.InOutCubic);
    }

    /// <summary>
    /// 一键清理粪便动画
    /// </summary>
    /// <param name="cloneTrans"></param>
    /// <param name="newValue"></param>
    public void PlayAddLoveCoinAniOneKey(Transform cloneTrans, int newValue)
    {
        Debug.Log("PlayAddLoveCoinAni   oldValue = " + ManageMentClass.DataManagerClass.loveCoin + "    newValue = " + newValue);
        if (newValue > ManageMentClass.DataManagerClass.loveCoin)
        {
            StartCoroutine(LoopCloneLoveCoinOneKey(cloneTrans));
        }
    }

    IEnumerator LoopCloneLoveCoinOneKey(Transform trans)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 4)
            {
                bClear = true;
            }
            CloneLoveCoinOneKey(trans);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void CloneLoveCoinOneKey(Transform cloneTrans)
    {
        if (cloneTrans == null)
            return;
        Canvas canvas = new GameObject("LoveCoinCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;
        GameObject loveCoinObj = Instantiate(cloneTrans.gameObject);
        loveCoinObj.name = "Coin";
        loveCoinObj.SetActive(true);
        if (loveCoinObj.transform.childCount > 0)
        {
            for (int i = 0; i < loveCoinObj.transform.childCount; i++)
            {
                Transform child = loveCoinObj.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
        Image loveImg = loveCoinObj.GetComponent<Image>();
        if (loveImg != null)
        {
            /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
             Sprite sprite = atlas.GetSprite("aixin");*/
            loveImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun("aixin");
            loveImg.SetNativeSize();
        }

        Vector3 uiPos = new Vector3();
        Canvas cloneTransCanvas = cloneTrans.GetComponentInParent<Canvas>();
        if (cloneTransCanvas != null)
        {
            if (cloneTransCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiPos = cloneTrans.transform.position;
            }
            else if (cloneTransCanvas.renderMode == RenderMode.WorldSpace)
            {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, cloneTrans.transform.position);
                RectTransform rt = canvas.transform.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, pos, null, out uiPos);
            }
        }
        loveCoinObj.transform.position = uiPos;
        loveCoinObj.transform.localScale = Vector3.one;
        loveCoinObj.transform.parent = canvas.transform;

        Vector3 endPos = new Vector3();
        if (!bInAidStations())
        {
            PetdensUIForm petdensUIForm = UIManager.GetInstance().GetUIForm(FormConst.PETDENS_UIFORM) as PetdensUIForm;
            if (petdensUIForm != null)
            {
                if (petdensUIForm.m_ImgLoveIcon != null)
                {
                    endPos = petdensUIForm.m_ImgLoveIcon.transform.position;
                }
            }
        }
        else
        {
            RescueStationUIForm rescueStationUIForm = UIManager.GetInstance().GetUIForm(FormConst.RESCUESTATION) as RescueStationUIForm;
            if (rescueStationUIForm != null)
            {
                if (rescueStationUIForm.m_ImgLoveIcon != null)
                {
                    endPos = rescueStationUIForm.m_ImgLoveIcon.transform.position;
                }
            }
        }
        loveCoinObj.transform.DOMove(endPos, 2f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            Destroy(canvas.gameObject);
            if (bClear)
            {
                if (clearId > 0)
                {
                    DestroyFecesUI(clearId);
                    bClear = false;
                }
            }
        });
        loveCoinObj.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 2f).SetEase(Ease.InOutCubic);
    }

    public void CloneFeces(int pos, PetFeces data)
    {
        if (data == null)
            return;
        GameObject obj;
        if (!dicFecesObj.ContainsKey(data.id))
        {
            string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, "GBB_L");
            obj = ResourcesMgr.GetInstance().LoadAsset(path, true);
            obj.name = "PetFeces";
            dicFecesObj[data.id] = obj;
        }
        else
        {
            dicFecesObj.TryGetValue(data.id, out obj);
        }
        obj.SetActive(true);
        obj.transform.position = ChangeRoomManager.Instance().listFecesPos[pos];
        obj.transform.localScale = Vector3.one;
        obj.transform.SetParent(_TransPetSpanMgr);
        fecesObj = obj;
        PetFecesItem petFecesItem = obj.GetComponent<PetFecesItem>();
        if (petFecesItem != null)
        {
            petFecesItem.SetPetFecesData(data);
        }
        CloneFecesHud(data.id);
        SetFecesData(data);
    }

    public void CloneFecesHud(int id)
    {
        GameObject obj;
        if (!dicFecesUI.ContainsKey(id))
        {
            obj = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_FECESUI, true);
            obj.name = "FecesUI";
            dicFecesUI[id] = obj;
        }
        else
        {
            dicFecesUI.TryGetValue(id, out obj);
        }
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }
        UIFollowWorldObject uiFollowWorldObject = obj.transform.Find("UIRoot").GetComponent<UIFollowWorldObject>();
        if (uiFollowWorldObject != null)
        {
            Transform target = UnityHelper.FindTheChildNode(fecesObj, "UIRoot");
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                //canvas.sortingOrder = -1;
                //obj.transform.localRotation = Quaternion.Euler(0, 180f, 0);
                //obj.transform.localScale = Vector3.one;
                //uiFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());

                canvas.renderMode = RenderMode.WorldSpace;
                obj.transform.position = target.position;
                obj.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                if (bInAidStations())
                {
                    obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    canvas.sortingOrder = 1;
                }
                canvas.worldCamera = Camera.main;
                uiFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());
            }
        }
        fecesUIObj = obj;
    }

    public void SetFecesData(PetFeces data)
    {
        bClear = false;
        if (fecesUIObj == null || data == null)
            return;

        Transform btnClean = UnityHelper.FindTheChildNode(fecesUIObj, "BtnClean");
        btnClean.GetComponent<Button>().enabled = bOpt();//ManageMentClass.DataManagerClass.is_Owner;

        EventTriggerListener.Get(btnClean.gameObject).onClick = p =>
        {
            Debug.Log("OnClickClean");
            if (bOpt())
            {
                List<int> ids = new List<int>() { data.id };
                MessageManager.GetInstance().RequestClearFeces(ids, (p) =>
                 {
                     clearId = data.id;
                     btnClean.gameObject.SetActive(false);
                     Transform imgClean = UnityHelper.FindTheChildNode(btnClean.gameObject, "ImgClean");
                     PlayAddLoveCoinAni(imgClean, p.remain_lovecoin);
                     //ManageMentClass.DataManagerClass.loveCoin = p.remain_lovecoin;
                     DestroyFeces(data.id);
                 });
            }
        };
    }


    public Transform GetFecesCloneTrans(int id)
    {
        GameObject uiObj;
        dicFecesUI.TryGetValue(id, out uiObj);
        if (uiObj != null)
        {
            Transform imgClean = UnityHelper.FindTheChildNode(uiObj.gameObject, "ImgClean");
            return imgClean;
        }
        return null;
    }

    /// <summary>
    /// 清除所有的激活中的trigger缓存
    /// </summary>
    public void ResetAllTriggers(Animator animator)
    {
        AnimatorControllerParameter[] aps = animator.parameters;
        for (int i = 0; i < aps.Length; i++)
        {
            AnimatorControllerParameter paramItem = aps[i];
            if (paramItem.type == AnimatorControllerParameterType.Trigger)
            {
                string triggerName = paramItem.name;
                bool isActive = animator.GetBool(triggerName);
                if (isActive)
                {
                    animator.ResetTrigger(triggerName);
                }
            }
        }
    }

    public void SetTrigger(Animator animator, string trigger, Action callback = null)
    {
        if (animator == null)
            return;
        ResetAllTriggers(animator);

        animator.SetTrigger(trigger);
        if (callback == null)
            return;
        StartCoroutine(PlayAniEndCallback(animator, trigger, callback));
    }

    IEnumerator PlayAniEndCallback(Animator animator, string stateName, Action callback)
    {
        // 状态机的切换发生在帧的结尾
        yield return new WaitForEndOfFrame();

        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(stateName)) yield return null;

        yield return new WaitForSeconds(info.length);
        callback();
    }

    public GameObject GetCurPetObj(int petId)
    {
        GameObject petObj = null;
        foreach (var obj in dicPetObj)
        {
            PetItem petItem = obj.Value.GetComponent<PetItem>();
            if (petItem != null)
            {
                PetModelRecData petModelRecData = petItem.GetPetData();
                if (petModelRecData != null && petModelRecData.id > 0)
                {
                    if (petModelRecData.id == petId)
                    {
                        petObj = obj.Value;
                        break;
                    }
                }
            }
        }
        return petObj;
    }

    public void PlayPetAni(int petId, int feedType)
    {
        GameObject petObj = GetCurPetObj(petId);
        Animator animator = petObj.GetComponent<Animator>();

        if (animator != null)
        {
            switch (feedType)
            {
                case (int)PetStateAnimationType.Idle:
                    SetTrigger(animator, "Idle");
                    break;
                case (int)PetStateAnimationType.Feed:
                    SetPetFeed(petId);
                    SetTrigger(animator, "Feed", () =>
                    {
                        SetTrigger(animator, "Idle");
                        DestroyFeedObj();
                    });
                    break;
                case (int)PetStateAnimationType.Clean:
                    SetPetBath(petObj);
                    SetTrigger(animator, "Bathe", () =>
                     {
                         DestroyParticle();
                         SetTrigger(animator, "Idle");
                     });
                    break;
                case (int)PetStateAnimationType.Toy:
                    SetTrigger(animator, "Play", () =>
                     {
                         SetTrigger(animator, "Idle");
                     });
                    break;
                case (int)PetStateAnimationType.Cure:
                    SetPetInjected(petId);
                    SetTrigger(animator, "Treatment", () =>
                     {
                         SetTrigger(animator, "Idle");
                         StartCoroutine(DelayDestroyInjectedObj());
                     });
                    break;
                case (int)PetStateAnimationType.Sleep:
                    SetTrigger(animator, "Sleep");
                    break;
                case (int)PetStateAnimationType.Train:
                    SetTrigger(animator, "Train");
                    break;
            }
        }
    }

    public void SetPetBath(GameObject petObj)
    {
        if (petObj != null)
        {
            string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, "PaoPao");
            particleObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
            particleObj.name = "PaoPao";
            particleObj.transform.position = petObj.transform.position;
            particleObj.transform.localScale = new Vector3(0.74f, 0.74f, 0.74f);
            particleObj.tag = "_TagPet";
            InterfaceHelper.ChangeLayer(particleObj.transform, 11);
            particleObj.SetActive(true);
            for (int i = 0; i < particleObj.transform.childCount; i++)
            {
                Transform childTrans = particleObj.transform.GetChild(i);
                ParticleSystem particleSystem = childTrans.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
            }
        }
    }

    public void DestroyParticle()
    {
        if (particleObj != null)
        {
            Destroy(particleObj);
        }
    }

    public GameObject GetFecesObj(int id)
    {
        GameObject obj;
        dicFecesObj.TryGetValue(id, out obj);
        return obj;
    }

    public GameObject GetFecesUIObj(int id)
    {
        GameObject obj;
        dicFecesUI.TryGetValue(id, out obj);
        return obj;
    }

    public void DestroyFeces(int id)
    {
        GameObject fecesObj = GetFecesObj(id);
        if (fecesObj != null)
        {
            Destroy(fecesObj);
        }
        dicFecesObj.Remove(id);

    }
    public void DestroyFecesUI(int id)
    {
        GameObject fecesUIObj = GetFecesUIObj(id);
        if (fecesUIObj != null)
        {
            Destroy(fecesUIObj);
        }
        dicFecesUI.Remove(id);
    }

    public void SetHeadUIVisable(bool bShow)
    {
        foreach (var uiObj in dicBoxUIObj)
        {
            PetListRecData petListRecData = ManageMentClass.DataManagerClass.petListDataDic[uiObj.Key];
            if (uiObj.Value.activeSelf != bShow)
            {
                uiObj.Value.SetActive(bShow && (petListRecData != null && petListRecData.status < (int)PetStatus.InEvolution));
            }
        }

        foreach (var uiObj in dicPetUIObj)
        {
            if (uiObj.Value.activeSelf != bShow)
            {
                uiObj.Value.SetActive(bShow);
            }
        }

        foreach (var uiObj in dicFecesUI)
        {
            if (uiObj.Value.activeSelf != bShow)
            {
                uiObj.Value.SetActive(bShow);
            }
        }
    }

    /// <summary>
    /// 清洁
    /// </summary>
    /// <param name="petId"></param>
    public void SetDirtyTexture(int petId)
    {
        GameObject petObj = GetCurPetObj(petId);
        if (petObj == null)
            return;
        PetModelRecData petModleData = GetPetModelData(petId);
        if (petModleData == null)
            return;
        List<Material> materials = InterfaceHelper.FindChildSkinMeshRendererMaterials(petObj.transform);
        Texture mTexture = null;
        switch (petModleData.pet_type)
        {
            case (int)PetType.PetCubsMale:
                mTexture = Resources.Load("Male_Mod/gou_L_01 - Default_AlbedoTransparency_dirty", typeof(Texture)) as Texture;
                break;
            case (int)PetType.PetCubsFemale:
                mTexture = Resources.Load("Female_Mod/Body_D_dirty", typeof(Texture)) as Texture;
                break;
        }
        if (mTexture == null)
            return;
        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            material.SetTexture("_BaseMap", mTexture);
        }
    }
    public void SetNormalTexture(int petId)
    {
        GameObject petObj = GetCurPetObj(petId);
        if (petObj == null)
            return;
        PetModelRecData petModleData = GetPetModelData(petId);
        if (petModleData == null)
            return;
        List<Material> materials = InterfaceHelper.FindChildSkinMeshRendererMaterials(petObj.transform);
        Texture mTexture = null;
        switch (petModleData.pet_type)
        {
            case (int)PetType.PetCubsMale:
                mTexture = Resources.Load("Male_Mod/gou_L_01 - Default_AlbedoTransparency", typeof(Texture)) as Texture;
                break;
            case (int)PetType.PetCubsFemale:
                mTexture = Resources.Load("Female_Mod/Body_D", typeof(Texture)) as Texture;
                break;
        }
        if (mTexture == null)
            return;
        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            material.SetTexture("_BaseMap", mTexture);
        }
    }

    /// <summary>
    /// 治疗
    /// </summary>
    /// <param name="petId"></param>
    public void SetPetInjected(int petId)
    {
        GameObject petObj = GetCurPetObj(petId);
        if (petObj == null)
            return;
        PetModelRecData petModleData = GetPetModelData(petId);
        if (petModleData == null)
            return;
        string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, "Needle");
        injectedObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
        injectedObj.transform.SetParent(petObj.transform);
        injectedObj.name = "Needle";
        injectedObj.tag = "_TagPet";
        InterfaceHelper.ChangeLayer(injectedObj.transform, 11);
        Animator animator = injectedObj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Needle");
        }
    }

    IEnumerator DelayDestroyInjectedObj()
    {
        yield return new WaitForSeconds(0.1f);
        DestroyInjectedObj();
    }

    public void DestroyInjectedObj()
    {
        if (injectedObj != null)
        {
            DestroyImmediate(injectedObj);
        }
    }

    /// <summary>
    /// 喂食
    /// </summary>
    /// <param name="petId"></param>
    public void SetPetFeed(int petId)
    {
        GameObject petObj = GetCurPetObj(petId);
        if (petObj == null)
            return;
        PetModelRecData petModleData = GetPetModelData(petId);
        if (petModleData == null)
            return;
        string path = string.Format("{0}{1}", SysDefine.SYS_PATH_PETMODEL, "Bowl");
        feedObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
        Transform parent = UnityHelper.FindTheChildNode(petObj, "FeedRoot");
        feedObj.transform.position = new Vector3(parent.transform.position.x, feedObj.transform.position.y, parent.transform.position.z);
        feedObj.name = "Bowl";
        feedObj.tag = "_TagPet";
        InterfaceHelper.ChangeLayer(feedObj.transform, 11);
    }

    public void DestroyFeedObj()
    {
        if (feedObj != null)
        {
            DestroyImmediate(feedObj);
        }
    }

    public bool IsHealth(PetModelRecData data)
    {
        if (data == null)
            return false;
        Dictionary<int, bool> healthStatusDic = new Dictionary<int, bool>();
        for (int i = 0; i < data.pet_condition.Count; i++)
        {
            int curValue = data.pet_condition[i].cur_val;
            int conditionType = data.pet_condition[i].condition_type;

            petcondition petconditionTab = ManageMentClass.DataManagerClass.GetPetConditionTable(data.pet_type, conditionType);
            int goodMin = petconditionTab.good_min;
            //int gootMax = petconditionTab.good_max;
            //int poorMin = petconditionTab.poor_min;
            //int poorMax = petconditionTab.poor_max;
            //int badMin = petconditionTab.bad_min;
            //int badMax = petconditionTab.bad_max;

            bool bGood = curValue >= goodMin;
            //bool bPoor = curValue >= poorMin && curValue <= poorMax;
            //bool bbad = curValue >= badMin && curValue <= badMax;

            if (!healthStatusDic.ContainsKey(conditionType))
            {
                healthStatusDic.Add(conditionType, bGood);
            }
        }
        //是否全部状态都健康
        bool bHealth = true;
        foreach (var status in healthStatusDic)
        {
            if (!status.Value)
            {
                bHealth = false;
                break;
            }
        }
        return bHealth;
    }

    /// <summary>
    /// 是否训练中
    /// </summary>
    /// <param name="petId"></param>
    /// <returns></returns>
    public bool IsTraining(int petId)
    {
        bool bTrain = false;
        if (dicTrain.ContainsKey(petId))
        {
            dicTrain.TryGetValue(petId, out bTrain);
            return bTrain;
        }

        foreach (var item in ManageMentClass.DataManagerClass.petModelRecData)
        {
            if (item.Value.id == petId)
            {
                if (item.Value.train_info != null)
                {
                    return true;
                }
            }
        }
        return bTrain;
    }

    public void SetTrainData(int petId, bool bTrain)
    {
        if (!dicTrain.ContainsKey(petId))
        {
            dicTrain[petId] = bTrain;
        }
        else
        {
            if (dicTrain[petId] != bTrain)
            {
                dicTrain[petId] = bTrain;
            }
        }
    }

    public bool IsCreateNameBox(PetItem petItem)
    {
        PetListRecData petListData = petItem.GetPetBoxData();
        PetModelRecData petModelRecData = petItem.GetPetData();
        bool bBox = false;
        if (petListData != null)
        {
            if (petListData.status == (int)PetStatus.InEvolution)
            {
                bBox = true;
            }
        }
        bBox = (bBox && !petModelRecData.bAdopted);
        return bBox;
    }

    public bool bOpt()
    {
        bool canOpt = ManageMentClass.DataManagerClass.is_Owner || (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.ShelterScene);
        return canOpt;
    }

    public bool bInAidStations()
    {
        bool inAid = ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.ShelterScene;
        return inAid;
    }

    /// <summary>
    /// 设置开始训练时宠物位置刚体
    /// </summary>
    /// <param name="petId"></param>
    public void SetBeginTrainPetRigidbody(int petId)
    {
        int pos = GetPetModelPos(petId);
        if (pos > 0)
        {
            GameObject petTrain = GetCurPetObj(petId);
            if (petTrain != null)
            {
                Rigidbody rigidbody = petTrain.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    Destroy(rigidbody);
                }
                //  petTrain.transform.position = ChangeRoomManager.Instance().listSpanPos[pos - 1];
            }
        }
    }

    /// <summary>
    /// 设置结束训练时宠物位置刚体
    /// </summary>
    /// <param name="petId"></param>
    public void SetFinishTrainPetRigidbody(int petId)
    {
        int pos = GetPetModelPos(petId);
        if (pos > 0)
        {
            GameObject petTrain = GetCurPetObj(petId);
            if (petTrain != null)
            {
                Rigidbody rigidbody = petTrain.GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                    rigidbody = petTrain.AddComponent<Rigidbody>();
                    rigidbody.isKinematic = true;
                    rigidbody.useGravity = false;
                }
                petTrain.transform.position = ChangeRoomManager.Instance().listSpanPos[pos - 1];
            }
        }
    }

    public void RemovePetObj(int petId)
    {
        int pos = GetPetModelPos(petId);
        if (dicPetObj.ContainsKey(pos))
        {
            GameObject petObj;
            dicPetObj.TryGetValue(pos, out petObj);
            if (petObj != null)
            {
                Destroy(petObj);
                dicPetObj.Remove(pos);
            }
        }

        if (dicPetUIObj.ContainsKey(pos))
        {
            GameObject petUIObj;
            dicPetUIObj.TryGetValue(pos, out petUIObj);
            if (petUIObj != null)
            {
                Destroy(petUIObj);
                dicPetUIObj.Remove(pos);
            }
        }
    }

    public void OneKeyCleanFeces()
    {
        List<int> fecesIds = dicFecesObj.Keys.ToList();
        MessageManager.GetInstance().RequestClearFeces(fecesIds, (p) =>
        {

            for (int i = 0; i < fecesIds.Count; i++)
            {
                DestroyFeces(fecesIds[i]);
                clearId = fecesIds[i];
                Transform imgClean = GetFecesCloneTrans(fecesIds[i]);
                PlayAddLoveCoinAniOneKey(imgClean, p.remain_lovecoin);
            }
            dicFecesObj.Clear();
            fecesIds.Clear();
            fecesIds = dicFecesUI.Keys.ToList();
            for (int i = 0; i < fecesIds.Count; i++)
            {
                DestroyFecesUI(fecesIds[i]);
            }
            dicFecesUI.Clear();

            UpdateLoveCoinValue(p.remain_lovecoin, 2.5f);
            RequestPetFecesList();
        });
    }
    public int GetFecesNum()
    {
        return dicFecesObj.Count;
    }

    /// <summary>
    /// 设置狗狗背景
    /// </summary>
    /// <param name="isActive"></param>
    public void SetDogBgObjActiveFun(bool isActive)
    {
        if (dogBgObj == null)
        {
            GameObject room = Resources.Load("Prefabs/Dogroom/dogBg", typeof(GameObject)) as GameObject;
            if (room != null && dogBgObj == null)
            {
                Debug.Log("这里的内容迪斯科浪费进阿里经典福克斯的肌肤拉进来  这里的内容 ");
                dogBgObj = Instantiate(room);
                dogBgObj.transform.parent = transform;
                dogBgObj.transform.localPosition = new Vector3(0.22f, 0.4f, 2.8f);
                dogBgObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
        if (dogBgObj != null)
        {
            dogBgObj.SetActive(isActive);
        }
    }

    public void SetDogLayerFun(int layerID)
    {
        dogBgObj.gameObject.layer = layerID;
    }

    public void LookAtBg()
    {
        Debug.Log("进入了镜头选中的地方");
        SetAllPetVisible(false);
        SetHeadUIVisable(false);
        bLookAting = true;

        SetDogBgObjActiveFun(true);
        if (petCamera == null)
        {
            petCamera = new GameObject("PetCamera").AddComponent<Camera>();
        }
        petCamera.GetUniversalAdditionalCameraData().SetRenderer(0);
        petCamera.transform.position = new Vector3(0.22f, 0.4f, -2.72f);
        petCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        petCamera.cullingMask = (1 << 13);  /*(1 << 0) +*/
        if (ManageMentClass.DataManagerClass.PlatformType != 1)
        {
            if (!bInAidStations())
            {
                //禁用摇杆

                joystickMgrObj = JoystickManager.Instance().GetJoystickObj().gameObject;
                if (joystickMgrObj != null)
                {
                    joystickMgrObj.SetActive(false);
                }
            }
        }
    }

    public void outLookAtDogBg()
    {
        SetAllPetVisible(true);
        SetHeadUIVisable(true);
        bLookAting = false;
        SetDogBgObjActiveFun(false);

        if (petCamera != null)
        {
            DestroyImmediate(petCamera.gameObject);
        }
        if (ManageMentClass.DataManagerClass.PlatformType != 1)
        {
            if (!bInAidStations())
            {
                //禁用摇杆
                joystickMgrObj = JoystickManager.Instance().GetJoystickObj().gameObject;
                if (joystickMgrObj != null)
                {
                    joystickMgrObj.SetActive(true);
                }
            }
        }
    }



    public void SetAllPetVisible(bool bShow)
    {
        foreach (var obj in dicPetObj)
        {
            obj.Value.SetActive(bShow);
        }
    }

    public void SetAllObjRestPosFun()
    {

        int index = 0;
        foreach (var item in dicBoxObj)
        {
            if (item.Value != null)
            {
                item.Value.transform.position = ChangeRoomManager.Instance().listSpanPos[index];
            }
            index++;
        }
        foreach (var item in dicPetObj)
        {
            if (item.Value != null)
            {
                Debug.Log("输出一下这里的位置信息：   " + ChangeRoomManager.Instance().listSpanPos[index]);
                item.Value.transform.position = ChangeRoomManager.Instance().listSpanPos[index];
            }
            index++;
        }
        index = 0;
        foreach (var item in dicFecesObj)
        {
            if (item.Value != null)
            {
                item.Value.transform.position = ChangeRoomManager.Instance().listFecesPos[index];
            }
            index++;
        }
    }



}
