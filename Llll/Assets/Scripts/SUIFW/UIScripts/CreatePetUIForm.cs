using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;
using WebGLSupport;
using UnityEngine.U2D;
using UnityEngine.EventSystems;

public class CreatePetUIForm : BaseUIForm
{
    public InputField m_InputField;
    public Image m_ImgSex;
    public Text m_TextBirth;
    public Text m_TextNumber;
    private CreatePetRecData createPetRecData = null;
    private PetListRecData petListRecData = null;
    // Start is called before the first frame update
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("BtnConfirm", p =>
        {

            string name = m_InputField.text;
            if (string.IsNullOrEmpty(name))
            {
                ToastManager.Instance.ShowPetToast("请输入昵称", 5f);
                return;
            }
            CloseUIForm();
            name = TextTools.setCutAddString(name, 7, "");

            UIManager.GetInstance().ShowUIForms(FormConst.CREATEPETTIPS_UIFORM);
            PetAdoptV2ReqData _data = new PetAdoptV2ReqData();
            _data.pet_name = name;
            _data.birthday = createPetRecData.birthday;
            _data.pet_number = createPetRecData.pet_number;
            _data.pet_type = createPetRecData.pet_type;
            _data.pet_id = petListRecData.id;
            SendMessage("PetAdoptV2", "Confirm", _data);
            GameObject btnConfirm = UnityHelper.FindTheChildNode(this.gameObject, "BtnConfirm").gameObject;
            EventSystem.current.SetSelectedGameObject(btnConfirm);
        });

        RigisterButtonObjectEvent("BtnCancel", p =>
        {
            CloseUIForm();
            GameObject btnCancel = UnityHelper.FindTheChildNode(this.gameObject, "BtnCancel").gameObject;
            EventSystem.current.SetSelectedGameObject(btnCancel);
        });

        ReceiveMessage("CreatePet", p =>
         {
             object[] args = p.Values as object[];
             createPetRecData = args[0] as CreatePetRecData;
             petListRecData = args[1] as PetListRecData;
             SetUIData();
         });
        m_InputField.onEndEdit.AddListener(OnInputFieldEndEdit);
    }



    void OnInputFieldEndEdit(string arg)
    {

        Debug.Log("适当放宽是的是的咯");

        if (arg.Length > 7)
        {
            m_InputField.text = TextTools.setCutAddString(arg, 7, "");
        }
    }

    public void SetUIData()
    {
        if (createPetRecData == null)
            return;
        string spriteName = "";
        switch (createPetRecData.pet_type)
        {
            case (int)PetType.PetCubsMale:
                spriteName = string.Format("{0}", "icon-boy");
                break;
            case (int)PetType.PetCubsFemale:
                spriteName = string.Format("{0}", "icon-gril");
                break;
        }

        /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
         Sprite sprite = atlas.GetSprite(spriteName);*/
        m_ImgSex.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun(spriteName);

        m_TextBirth.text = createPetRecData.birthday;
        m_TextNumber.text = createPetRecData.pet_number;
    }

    public override void Display()
    {
        base.Display();
        m_InputField.text = string.Empty;
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        PetSpanManager.Instance().CancelLookAtPet();
        InterfaceHelper.SetJoyStickState(true);
    }
}
