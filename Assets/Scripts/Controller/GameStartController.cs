using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;

public class GameStartController : MonoBehaviour
{
    //人物本身
    public static GameObject PlayerObj;
    //人物Item
    public static PlayerItem PlayerItem;

    public static Cinemachine.CinemachineFreeLook freeLook;

    public static Cinemachine.CinemachineBrain cameraBrain;


    public static GameObject mainCamera;




    private static List<Material> characterMaterial = new List<Material>();//角色身上所有材质列表
    public float lerpDuration = 1f;//角色生成渐变持续时间
    private static float _timeElapsed = 0f;

    private void Awake()
    {

        _timeElapsed = 0;

        UIManager.GetInstance().CloseUIForms(FormConst.LOGINLOADINGPANEL);
        GameObject rootNode = GameObject.Find("AllFurniture_Root");
        GameObject posNode = GameObject.Find("AllFurniturePos_Root");
        RoomFurnitureCtrl.Instance().InitNodeFun(rootNode, posNode);
        if (!ManageMentClass.DataManagerClass.bClickOtherSpace)
        {
            PlayerCtrlManager.Instance().Init();
            TransferEffectManager.Instance().Init();
        }
    }
    private void Start()
    {
     
        MessageManager.GetInstance().RequestCharacterList(() =>
        {
            Dictionary<int, CharacterData> m_CharacterServerData = MessageManager.GetInstance().GetCharacterData();
            for (int i = 0; i < m_CharacterServerData.Count; i++)
            {
                if (m_CharacterServerData[i].is_selected == 1)
                {
                    PlayerPrefs.SetInt("CurCharacterId", m_CharacterServerData[i].item_id);
                    break;
                }
            }
            //AffirmSelectPlayerFun();
            if (TransferEffectManager.Instance().bTransferSpace)
            {
                TransferEffectManager.Instance().bTransferSpace = false;
                return;
            }
            if (ManageMentClass.DataManagerClass.bClickOtherSpace)
            {
                ManageMentClass.DataManagerClass.bClickOtherSpace = false;
                return;
            }
            //  CharacterManager.Instance().Load();
        });
    }

}
