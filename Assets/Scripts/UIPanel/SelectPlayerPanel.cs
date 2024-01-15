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
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //��������
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Translucence; //��͸�������ܴ�͸
        RigisterButtonObjectEvent("Cloose_Btn", p =>
        {
            Debug.Log("1");
            //�رձ����
            CloseUIForm();
        });
        RigisterButtonObjectEvent("ChooseA_Btn", p =>
        {
            Debug.Log("2");
            //ѡ������
            //�ر����
        });
        RigisterButtonObjectEvent("ChooseB_Btn", p =>
        {
            Debug.Log("3");
            //ѡ������
            //�ر����
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
    /// ȷ��ѡ������
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
