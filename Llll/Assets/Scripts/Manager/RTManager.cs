using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.Rendering.Universal;

public class RTManager : MonoBehaviour
{
    private GameObject m_ModelObj;
    private GameObject m_CurCharacterObj;
    private GameObject characterBgObj;
    private static RTManager _Instance = null;
    private Transform _TransRTManager = null;
    public static RTManager GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_RTManager").AddComponent<RTManager>();
        }
        return _Instance;
    }

    private void Awake()
    {
        _TransRTManager = this.gameObject.transform;
        DontDestroyOnLoad(_TransRTManager);
    }

    public enum TableType
    {
        Furniture = 1,
        Action = 2,
        HotMan = 3,
    }

    public void LoadRTModel(int itemId)
    {
        Transform modelParent = UIManager.GetInstance().GetModelTrans();
        if (modelParent != null)
        {
            for (int i = 0; i < modelParent.childCount; i++)
            {
                Destroy(modelParent.GetChild(i).gameObject);
            }
        }
        item m_ItemConfig = ManageMentClass.DataManagerClass.GetItemTableFun(itemId);
        if (m_ItemConfig != null)
        {
            int m_ItemType = m_ItemConfig.item_type1;
            if (m_ItemType == (int)TableType.Furniture)
            {
                furniture m_ModelItem = ManageMentClass.DataManagerClass.GetFurnitureTableFun(itemId);
                if (m_ModelItem != null)
                {
                    string modelName = m_ModelItem.furniture_model;
                    GameObject modelObj = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_MODEL + modelName, false);
                    UnityHelper.SetLayer(modelObj, 8);
                    modelObj.transform.SetParent(modelParent);
                    modelObj.transform.localPosition = new Vector3(0, 0f, 0f);//Vector3.zero;
                    modelObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);//Vector3.one;
                    modelObj.transform.localEulerAngles = Vector3.zero;
                    if (modelObj.GetComponent<CamCtrl>() == null)
                    {
                        CamCtrl camCtrl = modelObj.AddComponent<CamCtrl>();
                        camCtrl.modelObj = modelParent.transform;
                    }
                    Vector3 newPos = new Vector3();
                    MeshRenderer meshRender = modelObj.GetComponent<MeshRenderer>();
                    if (meshRender != null)
                    {
                        newPos = meshRender.bounds.center;
                    }
                    else
                    {
                        for (int i = 0; i < modelObj.transform.childCount; i++)
                        {
                            if (modelObj.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                            {
                                newPos = modelObj.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                                break;
                            }
                        }
                    }


                    if (UIManager.GetInstance().GetModelCameraTrans() != null)
                    {
                        UIManager.GetInstance().GetModelCameraTrans().position = new Vector3(newPos.x - 12, newPos.y, newPos.z - 0.7f);
                        UIManager.GetInstance().GetModelCameraTrans().localEulerAngles = new Vector3(0, 90, 0);
                    }

                    string path = string.Format("{0}", "Prefabs/CharacterChanging/Body/CharacterBg");
                    if (characterBgObj == null)
                    {
                        characterBgObj = ResourcesMgr.GetInstance().LoadAsset(path, false);
                    }
                    Transform platformTrans = UnityHelper.FindTheChildNode(characterBgObj, "M_Rzhanshitai01(Clone)");
                    if (platformTrans != null)
                    {
                        platformTrans.gameObject.SetActive(false);
                    }
                    characterBgObj.transform.localPosition = new Vector3(newPos.x, newPos.y, newPos.z);
                    characterBgObj.transform.localRotation = Quaternion.Euler(0, 180f, 0);
                    characterBgObj.transform.localScale = new Vector3(1f, 1f, 1f);
                    characterBgObj.transform.SetParent(modelParent.transform.parent);
                    UnityHelper.SetLayer(characterBgObj, 6);
                }
            }
            //else
            //{
            //int m_SelectCharacterId = PlayerPrefs.GetInt("CurCharacterId");
            //hotman m_HotManConfig = null;
            //if (m_SelectCharacterId > 0)
            //{
            //    m_HotManConfig = ManageMentClass.DataManagerClass.GetHotmanTableFun(m_SelectCharacterId);
            //}
            //else
            //{
            //    m_HotManConfig = ManageMentClass.DataManagerClass.GetHotmanTableFun(1000);//Ĭ�Ͻ�ɫ
            //}
            //if (m_HotManConfig == null)
            //    return;
            //string hotManName = m_HotManConfig.hotman_mode;
            //GameObject modelObj = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_CHARACTER + hotManName, false);
            //UnityHelper.SetLayer(modelObj, 8);
            //modelObj.transform.SetParent(modelParent);
            //modelObj.transform.localPosition = Vector3.zero;
            //modelObj.transform.localEulerAngles = new Vector3(0, -90, 0);
            //modelObj.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
            //m_ModelObj = modelObj;

            //if (UIManager.GetInstance().GetModelCameraTrans() != null)
            //{
            //    UIManager.GetInstance().GetModelCameraTrans().position = new Vector3(1995, 2001, 0);
            //    UIManager.GetInstance().GetModelCameraTrans().localEulerAngles = new Vector3(0, 90, 0);
            //}
            //PlayerItem playerItem = modelObj.GetComponent<PlayerItem>();
            //animation m_AnimationConfig = ManageMentClass.DataManagerClass.GetAnimationTableFun(itemId);
            //if (playerItem != null)
            //{
            //    if (m_AnimationConfig != null)
            //    {
            //        playerItem.SetAnimator(m_AnimationConfig.animation_model);
            //    }
            //}
            //}
        }
    }

    public void LoadCharacter(ulong userID = 0)
    {
        if (userID == ManageMentClass.DataManagerClass.userId)
        {
            userID = 0;
        }
        Debug.Log("�����ֵ�� չʾ");
        Transform modelParent = UIManager.GetInstance().GetModelTrans();
        if (modelParent != null)
        {
            for (int i = 0; i < modelParent.childCount; i++)
            {
                Destroy(modelParent.GetChild(i).gameObject);
            }
            modelParent.transform.localPosition = new Vector3(2000f, 0f, 0f);
            modelParent.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        Transform cameraTrans = UIManager.GetInstance().GetModelCameraTrans();
        if (cameraTrans != null)
        {
            cameraTrans.transform.localPosition = new Vector3(2000.5f, 1f, -14f);
            cameraTrans.transform.localRotation = Quaternion.Euler(Vector3.zero);
            Camera camera = cameraTrans.GetComponent<Camera>();
            if (camera != null)
            {
                camera.fieldOfView = 20f;
                camera.cullingMask = (1 << 6) + (1 << 8);
                camera.GetUniversalAdditionalCameraData().SetRenderer(2);
            }
        }

        if (AvatarManager.Instance().avatarGamePlayer == null)
        {
            if (userID == 0)
            {
                AvatarManager.Instance().GetUIShowPlayerObjFun();
            }
            else
            {
                AvatarManager.Instance().GetOtherUIPlayerObjFun(userID);
            }
        }


        AvatarManager.Instance().avatarGamePlayer.transform.position = new Vector3(2000f, 0f, 0f);
        AvatarManager.Instance().avatarGamePlayer.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        AvatarManager.Instance().avatarGamePlayer.transform.rotation = Quaternion.Euler(0, 180f, 0);


        string path = string.Format("{0}", "Prefabs/CharacterChanging/Body/CharacterBg");
        if (characterBgObj == null)
        {
            characterBgObj = ResourcesMgr.GetInstance().LoadAsset(path, false);
        }
        Transform platformTrans = UnityHelper.FindTheChildNode(characterBgObj, "M_Rzhanshitai01(Clone)");
        if (platformTrans != null)
        {
            platformTrans.gameObject.SetActive(true);
        }
        characterBgObj.transform.localPosition = new Vector3(1998.541f, 1.298784f, 1f);
        characterBgObj.transform.localRotation = Quaternion.Euler(0, 90f, 0);
        characterBgObj.transform.localScale = new Vector3(1f, 1f, 1f);
        characterBgObj.transform.SetParent(modelParent.transform.parent);
        UnityHelper.SetLayer(characterBgObj, 6);
        
        UIManager.GetInstance().RT.SetActive(true);
        Cinemachine.CinemachineBrain cinemachineBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = false;
        }
    }
    /*
        public Vector3 GetOriginPos()
        {
          //  return originPos;
        }

        public Vector3 GetOriginRotation()
        {
            //return originEuler;
        }*/

    public void ResetCharacter(bool isGoShop = false)
    {
        /* callNum = 0;
         GameObject mCurCharacterObj = CharacterManager.Instance().GetPlayerObj();
         mCurCharacterObj.transform.position = originPos;
         mCurCharacterObj.transform.rotation = Quaternion.Euler(originEuler);
         CharacterManager.Instance().SetPlayerActive(true);
         //  mCurCharacterObj.gameObject.SetActive(true);

         if (!isGoShop)
         {
             CamCtrl camCtrl = mCurCharacterObj.GetComponent<CamCtrl>();
             if (camCtrl != null)
             {
                 Destroy(camCtrl);
                 camCtrl = null;
             }
         }
         UnityHelper.SetLayer(mCurCharacterObj, 8);*/
    }
    public void DestroyCharacter()
    {
        if (AvatarManager.Instance().avatarGamePlayer != null)
        {
            AvatarManager.Instance().RecycleUIShowPlayerFun();
        }

        if (characterBgObj != null)
        {
            Destroy(characterBgObj);
            characterBgObj = null;
        }

        UIManager.GetInstance().RT.SetActive(false);
        Cinemachine.CinemachineBrain cinemachineBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = true;
        }
    }

    public void SetCharacterVisable(bool bShow)
    {
        if (AvatarManager.Instance().avatarGamePlayer != null)
        {
            AvatarManager.Instance().avatarGamePlayer.SetActive(bShow);
        }
        if (m_ModelObj != null)
        {
            m_ModelObj.SetActive(!bShow);
        }
    }
}
