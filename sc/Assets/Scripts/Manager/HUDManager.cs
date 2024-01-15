using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private Transform _TransHUD = null;
    public float height = 750f;
    public GameObject m_PetBtn;
    private static HUDManager _instance = null;
    public static HUDManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_HUDManager").AddComponent<HUDManager>();
        }
        return _instance;
    }

    private void Awake()
    {
        _TransHUD = this.gameObject.transform;
        DontDestroyOnLoad(_TransHUD);
    }
    public void ShowPetEntry()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.HUD);
        InterfaceHelper.SetJoyStickState(true);
        //BaseUIForm baseUIForm = UIManager.GetInstance().GetUIForm(FormConst.HUD);
        //GameObject hudObj = baseUIForm.gameObject;
        //m_PetBtn = UnityHelper.FindTheChildNode(hudObj, "BtnPetdens").gameObject;
        //GameObject playerObj = CharacterManager.Instance().GetPlayerObj();
        //if (playerObj != null)
        //{
        //    Transform hud = playerObj.transform.Find("Hud");
        //    if(hud != null)
        //    {
        //        Vector3 screenPos = Camera.main.WorldToScreenPoint(hud.position);
        //        m_PetBtn.transform.position = new Vector3(screenPos.x, screenPos.y, 0f);
        //    }
        //}
    }

    public void HidePetEntry()
    {
        UIManager.GetInstance().CloseUIForms(FormConst.HUD);
    }
}
