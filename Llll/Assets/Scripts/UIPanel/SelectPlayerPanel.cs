using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;
public class SelectPlayerPanel : BaseUIForm
{
    private Button btnA;
    private Button btnB;
    private Cinemachine.CinemachineFreeLook freeLook;
    private GameObject playerObj;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Translucence; //半透明，不能穿透
        RigisterButtonObjectEvent("Cloose_Btn", p =>
        {
            Debug.Log("1");
            //关闭本面板
            CloseUIForm();
        });
        RigisterButtonObjectEvent("ChooseA_Btn", p =>
        {
            Debug.Log("2");
            //选择人物
            //关闭面板
        });
        RigisterButtonObjectEvent("ChooseB_Btn", p =>
        {
            Debug.Log("3");
            //选择人物
            //关闭面板
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        freeLook = GameObject.Find("CM FreeLook1").GetComponent<Cinemachine.CinemachineFreeLook>();
        playerObj = GameObject.Find("Character/DefaultChar").gameObject;
        AffirmSelectPlayerFun(playerObj);
    }
    /// <summary>
    /// 确认选择的玩家
    /// </summary>
    void AffirmSelectPlayerFun(GameObject playerObj)
    {
       /* PlayerController.PlayerObj = playerObj;
        PlayerController.PlayerItem = playerObj.GetComponent<PlayerItem>();
        var followA = PlayerController.PlayerObj.transform.Find("CameraRoot").gameObject;
        var heat = PlayerController.PlayerObj.transform.Find("Heat").gameObject;
        setFreeLookDataFun(followA, heat);*/
    }
    void setFreeLookDataFun(GameObject follow, GameObject lookAt)
    {
        freeLook.Follow = follow.transform;
        freeLook.LookAt = lookAt.transform;
    }
}
