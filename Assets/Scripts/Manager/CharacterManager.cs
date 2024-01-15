using Cinemachine;
using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private Transform _TransCharacter = null;
    private Vector3 initPos = new Vector3(-5.422f, 0, 5.61f);
    private Vector3 initEulerAngles = new Vector3(0f, 180f, 0f);
    private List<Material> characterMaterial = new List<Material>();//角色身上所有材质列表
    public float lerpDuration = 1f;//角色生成渐变持续时间
    private static float _timeElapsed = 0f;
    private int m_LastCharacterId = 0;
    public PlayerItem playerItem;
    public CharacterObjData characterObjData = new CharacterObjData();

    //所有人物的存储
    private Dictionary<int, List<GameObject>> DicHotManObjDic = new Dictionary<int, List<GameObject>>();
    //支持换装的人物的名字
    private string AvatarModelName = "DefaultChar";
    private static CharacterManager _instance = null;
    public static CharacterManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_CharacterManager").AddComponent<CharacterManager>();
        }
        return _instance;
    }

    private void Awake()
    {
        _TransCharacter = this.gameObject.transform;
        DontDestroyOnLoad(_TransCharacter);
    }
    /// <summary>
    /// 设置人物角色
    /// </summary>
    /// <param name="ItemID"></param>
    public void SetOneSelfHotmanFun(int ItemID)
    {
        GameObject characterObj = CharacterObjPoolFun(ItemID);

        if (characterObjData.playerObj != null)
        {
            if (ItemID != characterObjData.ItemID)
            {
                characterObjData.playerObj.SetActive(false);
            }
            characterObj.transform.localPosition = new Vector3(characterObjData.playerObj.transform.position.x, characterObjData.playerObj.transform.position.y, characterObjData.playerObj.transform.position.z);
            characterObj.transform.localEulerAngles = new Vector3(characterObjData.playerObj.transform.localEulerAngles.x, characterObjData.playerObj.transform.localEulerAngles.y, characterObjData.playerObj.transform.localEulerAngles.z);
        }
        else
        {
            characterObj.transform.localPosition = initPos;
            characterObj.transform.localEulerAngles = initEulerAngles;
        }
        characterObj.SetActive(true);
        Transform cameraRoot = characterObj.transform.Find("CameraRoot");
        Transform heatObj = characterObj.transform.Find("Heat");
        CameraManager.Instance().SetFollow(cameraRoot, heatObj);

        Debug.Log("这里的内容的值： " + characterMaterial.Count);
        SetPlayerObj(characterObj, ItemID);
        var item = characterObj.GetComponent<PlayerItem>();
        Debug.Log("输出一下更换人物后的名字和代码列表的名字： " + characterObj.name + " 名字值：  " + item.gameObject.name);
        if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.TreasureDigging)
        {
            Singleton<TreasuringController>.Instance.UpdateSelfPlayerControllerImp(characterObj);
        }
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.parlorScene)
        {
            Singleton<ParlorController>.Instance.UpdateSelfPlayerControllerImp(characterObj);
        }

        PlayOtherPlayerSpecialEffect(characterObj);
        SetPlayerItem(item);
        // ResetMaterialProperty();
        SetMaterialProperty(1.5f);

    }

    public GameObject GetOtherPlayerHotManFun(int ItemID)
    {
        GameObject characterObj = CreatPlayerObjFun(ItemID);
        if (characterObj.GetComponent<MoveControllerImp>() == null)
        {
            characterObj.AddComponent<MoveControllerImp>();
        }
        return characterObj;
    }
    /// <summary>
    /// 玩家的对象池
    /// </summary>
    private GameObject CharacterObjPoolFun(int ItemID)
    {
        if (!DicHotManObjDic.ContainsKey(ItemID))
        {
            Debug.Log("走到了这里来了");
            GameObject characterObj = CreatPlayerObjFun(ItemID);
            if (characterObj != null)
            {
                List<GameObject> listGame = new List<GameObject>();
                listGame.Add(characterObj);
                DicHotManObjDic.Add(ItemID, listGame);
            }
            return characterObj;
        }
        else
        {
            Debug.Log("走到了这里来了啊啊啊");
            for (int i = 0; i < DicHotManObjDic[ItemID].Count; i++)
            {
                if (DicHotManObjDic[ItemID][i] != null)
                {
                    if (!DicHotManObjDic[ItemID][i].activeSelf)
                    {
                        return DicHotManObjDic[ItemID][i];
                    }
                }
            }
            GameObject characterObj = CreatPlayerObjFun(ItemID);
            if (characterObj != null)
            {
                DicHotManObjDic[ItemID].Add(characterObj);
            }
            return characterObj;
        }
    }
    /// <summary>
    /// 创建人物形象
    /// </summary>
    /// <param name="ItemID"></param>
    /// <returns></returns>
    public GameObject CreatPlayerObjFun(int ItemID)
    {
        string m_ModelName = "";
        if (ItemID == 0)
        {
            m_ModelName = AvatarModelName;
        }
        else
        {
            avatar m_CurHotManConfig = ManageMentClass.DataManagerClass.GetAvatarTableFun(ItemID);
            m_ModelName = m_CurHotManConfig.avatar_model;
        }
        string path = string.Format("{0}{1}", SysDefine.SYS_PATH_CHARACTER, m_ModelName);
        GameObject m_CurHotManObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
        if (m_CurHotManObj != null)
        {
            if (m_CurHotManObj.GetComponent<PlayerItem>() == null)
            {
                m_CurHotManObj.AddComponent<PlayerItem>();
            }

            m_CurHotManObj.name = m_ModelName;
            m_CurHotManObj.gameObject.SetActive(false);
            m_CurHotManObj.transform.SetParent(_TransCharacter);
            m_CurHotManObj.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
            return m_CurHotManObj;
        }
        else
        {
            Debug.LogError("模型加载为空： path: " + path);
        }
        return null;
    }



    /// <summary>
    /// 播放人物特殊效果动画
    /// 一般用于进场时播放
    /// </summary>
    public void PlayPlayerSpecialEffectFun()
    {
        if (characterObjData.playerObj != null)
        {
            characterMaterial.Clear();
            characterMaterial = InterfaceHelper.FindChildMaterials(characterObjData.playerObj.transform);
            if (characterMaterial.Count > 0)
            {
                ResetMaterialProperty();
                //StartCoroutine(PlaySecialEffectFun());
            }
        }
    }

    public void PlayOtherPlayerSpecialEffect(GameObject otherPlayerObj)
    {
        if (otherPlayerObj != null)
        {
            List<Material> materials = InterfaceHelper.FindChildMaterials(otherPlayerObj.transform);
            if (materials.Count > 0)
            {
                SetOtherPlayerMaterialProperty(materials, 1.5f);
            }
        }
    }
    /// <summary>
    /// 播放效果
    /// </summary>
    /// <returns></returns>
    IEnumerator PlaySecialEffectFun()
    {
        bool isOn = true;
        Debug.Log("输出一下 _timeelapsed: " + _timeElapsed + "  lerpDuration: " + lerpDuration);
        while (isOn)
        {
            _timeElapsed += Time.deltaTime;
            if (_timeElapsed < lerpDuration)
            {

                float value = Mathf.Lerp(0f, 1.5f, Mathf.Pow(_timeElapsed / lerpDuration, 4));
                Debug.Log("_timeElapsed:  " + _timeElapsed + "  value:   " + value);
                SetMaterialProperty(value);
            }
            else
            {
                _timeElapsed = lerpDuration;
                SetMaterialProperty(1.5f);
                isOn = false;
                StopCoroutine(PlaySecialEffectFun());
            }
            yield return null;
        }

    }


    public void ResetMaterialProperty()
    {
        _timeElapsed = 0;
        SetMaterialProperty(1.5f);
    }
    public void SetMaterialProperty(float value)
    {
        for (int i = 0; i < characterMaterial.Count; i++)
        {
            Material material = characterMaterial[i];
            material.SetFloat("_DissolveAmount", value);
        }
    }

    public void SetOtherPlayerMaterialProperty(List<Material> materials, float value)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            material.SetFloat("_DissolveAmount", value);
        }
    }

    public void SetPlayerItem(PlayerItem player)
    {
        if (player == null)
            return;
        Debug.Log("输出一下playeritem的名字： " + player.gameObject.name);
        playerItem = player;
    }
    public PlayerItem GetPlayerItem()
    {
        return playerItem;
    }
    public void SetPlayerObj(GameObject player, int ID)
    {
        if (characterObjData == null)
            characterObjData = new CharacterObjData();
        characterObjData.playerObj = player;
        characterObjData.ItemID = ID;
    }

    public GameObject GetPlayerObj()
    {
        return characterObjData.playerObj;
    }
    /*  public void SetPlayerActive(bool active)
      {
          if (characterObjData.playerObj != null)
          {
              if (characterObjData.playerObj.CompareTag("AvatarPlayer"))
              {
                  AvatarManager.Instance().SetOpenActiveFun(active);
              }
              else
              {
                  characterObjData.playerObj.SetActive(active);
              }
          }
          else
          {
              Debug.LogError("关闭人物角色时，人物为空");
          }
      }*/


    public void SetLastCharacterId(int id)
    {
        m_LastCharacterId = id;
    }

    public void SetCharacterPos(Vector3 newPos)
    {
        GameObject playerObj = GetPlayerObj();
        if (playerObj != null)
        {
            playerObj.transform.localPosition = newPos;
            playerObj.transform.localRotation = Quaternion.identity;
        }
    }

    public void SetCharacterPosAndRotation(Vector3 newPos, Quaternion quaternion)
    {
        GameObject playerObj = GetPlayerObj();
        if (playerObj != null)
        {
            playerObj.transform.localPosition = newPos;
            playerObj.transform.localRotation = quaternion;
        }
    }

    public void SetFollow()
    {
        GameObject playerObj = GetPlayerObj();
        if (playerObj != null)
        {
            Transform cameraRoot = playerObj.transform.Find("CameraRoot");
            Transform heatObj = playerObj.transform.Find("Heat");
            CameraManager.Instance().SetFollow(cameraRoot, heatObj);
        }
    }

    public void SetSpaceCinemachineLook()
    {
        CinemachineFreeLook m_CinemachineFreeLook = CameraManager.Instance().GetFreeLook();
        if (m_CinemachineFreeLook != null)
        {
            m_CinemachineFreeLook.m_Orbits[0].m_Height = 2f;
            m_CinemachineFreeLook.m_Orbits[0].m_Radius = 1.66f;

            m_CinemachineFreeLook.m_Orbits[1].m_Height = 0.9f;
            m_CinemachineFreeLook.m_Orbits[1].m_Radius = 5f;

            m_CinemachineFreeLook.m_Orbits[2].m_Height = 0.45f;
            m_CinemachineFreeLook.m_Orbits[2].m_Radius = 2.61f;

            //m_CinemachineFreeLook.m_Lens.FieldOfView = 40f;
            //m_CinemachineFreeLook.m_XAxis.Value = 35f;
            //Vector3 pos = Camera.main.transform.position - playerObj.transform.position;
            //pos.x = pos.z = 0f;
            //playerObj.transform.LookAt(Camera.main.transform.position - pos);
        }
    }

    public void SetPetdensCinemachineLook()
    {
        CinemachineFreeLook m_CinemachineFreeLook = CameraManager.Instance().GetFreeLook();
        if (m_CinemachineFreeLook != null)
        {
            m_CinemachineFreeLook.m_Orbits[0].m_Height = 2f;
            m_CinemachineFreeLook.m_Orbits[0].m_Radius = 1.66f;

            m_CinemachineFreeLook.m_Orbits[1].m_Height = 0.9f;
            m_CinemachineFreeLook.m_Orbits[1].m_Radius = 10f;

            m_CinemachineFreeLook.m_Orbits[2].m_Height = 0.45f;
            m_CinemachineFreeLook.m_Orbits[2].m_Radius = 2.61f;

            //m_CinemachineFreeLook.m_Lens.FieldOfView = 60f;
            //m_CinemachineFreeLook.m_XAxis.Value = 35f;
            //Vector3 pos = Camera.main.transform.position - playerObj.transform.position;
            //pos.x = pos.z = 0f;
            //playerObj.transform.LookAt(Camera.main.transform.position - pos);
        }
    }

    public Transform GetTransCharacter()
    {
        return _TransCharacter;
    }
}
