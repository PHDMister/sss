using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

enum RoomType
{
    AnimShelter = 0,
    DogRoom1 = 1,
    DogRoom2 = 2
}
public class ChangeRoomManager : MonoBehaviour
{
    private Dictionary<int, GameObject> roomDic = new Dictionary<int, GameObject>();

    public List<DogRoomItemData> AllDogRoomItemData = new List<DogRoomItemData>();


    private GameObject nowRoom;
    //��ʽID
    private int roomID_Offic = 0;
    //Ԥ��ID
    private int roomID_Temp = 0;

    public int DogDataListIndex = 0;

    private petHouse petHouseData;

    private GameObject changeRoomRootNode = null;
    private static ChangeRoomManager _instance = null;

    public RoomAreaData roomAreaData = new RoomAreaData(0, 0, 0, 0);

    //���� ����λ��
    private Vector3[] arrSpanPosA = new Vector3[5] { new Vector3(-0.12f, 0f, -0.365f), new Vector3(0.626f, 0f, 0.66f), new Vector3(-1.583f, 0f, 0.626f), new Vector3(1.14f, 0f, -1.45f), new Vector3(-1.157f, 0f, -1.34f) };
    // ����վ ����λ��
    private Vector3[] arrSpanPosB = new Vector3[5] { new Vector3(-1.56f, 0f, -3f), new Vector3(0.35f, 0f, -3f), new Vector3(0.85f, 0f, -1.5f), new Vector3(-0.7f, 0f, -1.5f), new Vector3(-2.2f, 0f, -1.5f) };

    // ���� ����λ��
    private Vector3[] arrFecesPosA = new Vector3[10] { new Vector3(-0.12f, 0f, 0.35f), new Vector3(0.626f, 0f, 1.6f), new Vector3(-1.583f, 0f, 1.364f), new Vector3(1.14f, 0f, -0.634f), new Vector3(-1.157f, 0f, -0.559f), new Vector3(0.419f, 0f, -2.354f), new Vector3(-1.873f, 0f, -2.046f), new Vector3(-2.227f, 0f, -0.444f), new Vector3(1.939f, 0f, 0.224f), new Vector3(-0.596f, 0f, 1.885f) };
    //����վ ����λ��
    private Vector3[] arrFecesPosB = new Vector3[10] { new Vector3(-1.32f, 0f, -0.9f), new Vector3(0f, 0f, -1.14f), new Vector3(-1.1f, 0f, -1.9f), new Vector3(0.26f, 0f, -2.7f), new Vector3(-2.3f, 0f, -3.2f), new Vector3(0f, 0f, -3.8f), new Vector3(1.54f, 0f, -2.3f), new Vector3(-0.95f, 0f, -2.9f), new Vector3(-2.3f, 0f, -1.5f), new Vector3(1.4f, 0f, -3.3f) };

    //���� ����λ��
    public List<Vector3> listSpanPos = new List<Vector3>();
    //���� ����λ��
    public List<Vector3> listFecesPos = new List<Vector3>();

    //���ѱ���ͼ
    public Dictionary<int, Texture> DicDogRoomBgTexture = new Dictionary<int, Texture>();



    public static ChangeRoomManager Instance()
    {
        if (_instance == null)
        {
            var changeroomManger = new GameObject("_ChangeRoomManager");
            _instance = changeroomManger.AddComponent<ChangeRoomManager>();
            DontDestroyOnLoad(changeroomManger);
        }
        return _instance;
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void InitRoom()
    {
        Debug.Log("alldogroomitemdata����: " + AllDogRoomItemData.Count);
        // AllDogRoomItemData[0].is_select = 1;
        RefreshSelectDataFun();
        Debug.Log("�������ʼ�����ݣ� " + roomID_Offic);
        // roomID_Offic = 1002;
        InitRoomFun();
    }


    private void RefreshSelectDataFun()
    {
        for (int i = 0; i < AllDogRoomItemData.Count; i++)
        {
            AllDogRoomItemData[i].is_UISelect = 0;
        }
        for (int i = 0; i < AllDogRoomItemData.Count; i++)
        {
            if (AllDogRoomItemData[i].is_select == 1)
            {
                roomID_Offic = AllDogRoomItemData[i].item_id;
                DogDataListIndex = i + 1;
                AllDogRoomItemData[i].is_UISelect = 1;
                break;
            }
        }
        if (roomID_Offic == 0)
        {
            for (int i = 0; i < AllDogRoomItemData.Count; i++)
            {
                if (AllDogRoomItemData[i].item_default != 0)
                {
                    roomID_Offic = AllDogRoomItemData[i].item_id;
                    AllDogRoomItemData[i].is_select = 1;
                    DogDataListIndex = i + 1;
                    AllDogRoomItemData[i].is_UISelect = 1;
                    break;
                }
            }
        }
    }


    /// <summary>
    /// ��ʼ������
    /// </summary>
    private void InitRoomFun()
    {
        petHouseData = ManageMentClass.DataManagerClass.GetPetHouseTableFun(roomID_Offic);
        if (changeRoomRootNode == null)
        {
            changeRoomRootNode = new GameObject("_ChangeRoomRootNode");
            // DontDestroyOnLoad(changeRoomRootNode.transform);
        }
        roomID_Temp = roomID_Offic;
        SetRoomObjPosFun(roomID_Offic);
        AddRoom(roomID_Offic);
        SetRoomAreaDataFun(roomID_Offic);
        SetAnimalShelterCameraDataFun(roomID_Offic);
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="Id"></param>
    public void ChangeRoom(int Id)
    {
        if (Id != roomID_Offic)
        {

            petHouseData = ManageMentClass.DataManagerClass.GetPetHouseTableFun(Id);
            RemoveRoom();
            AddRoom(Id);
            SetRoomAreaDataFun(Id);
            SetRoomObjPosFun(Id);
            SetAnimalShelterCameraDataFun(Id);
            Debug.Log("����������������");
        }
    }

    /// <summary>
    /// �������䱳��ͼ
    /// </summary>
    /// <param name="roomID"></param>
    public void ChangeRoomDogBgFun(int roomID, bool isAll = false)
    {
        roomID_Temp = roomID;
        Debug.Log("���һ�¸�������ͼ�� " + isAll);
        SetDogRoomBgTextureFun(roomID, isAll);
    }

    /// <summary>
    /// ����Ϊ��ǰ����
    /// </summary>
    public void SaveRoomFun()
    {
        if (roomID_Offic != roomID_Temp)
        {

            ChangeRoom(roomID_Temp);
        }
    }
    /// <summary>
    /// �����Ƿ񱣴�
    /// </summary>
    public bool IsSaveRoomFun()
    {
        if (roomID_Offic != roomID_Temp)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// �����Ƿ���
    /// </summary>
    /// <returns></returns>
    public bool IsHaveRoomFun(int index)
    {
        return AllDogRoomItemData[index].is_buy == 1 || AllDogRoomItemData[index].item_default != 0;
    }
    /// <summary>
    /// ��ӷ���
    /// </summary>
    /// <param name="id"></param>
    private void AddRoom(int id)
    {
        if (roomDic.ContainsKey(id))
        {
            roomDic[id].SetActive(true);

        }
        else
        {
            //ʵ������������
            Debug.Log("�ļ����֣� " + petHouseData.item_model);
            GameObject room = Resources.Load("Prefabs/Dogroom/" + petHouseData.item_model, typeof(GameObject)) as GameObject;
            if (room != null)
            {
                GameObject itemPrefab = Instantiate(room);

                if (itemPrefab != null)
                {
                    itemPrefab.transform.parent = changeRoomRootNode.transform;
                    itemPrefab.SetActive(true);
                    if (id == 2001)
                    {
                        itemPrefab.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    else
                    {
                        itemPrefab.transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    roomDic.Add(id, itemPrefab);
                }
            }
            else
            {
                Debug.Log("���һ������Ϊ��");
            }
        }
        Debug.Log("����ĳ��� roomDic:  " + roomDic.Count + "   id : " + id);
        if (roomDic.ContainsKey(id))
        {
            roomID_Offic = roomID_Temp;
            Debug.Log("���һ��rooid_Offic��ֵ��  " + roomID_Offic);
            //���÷���
            SetNowRoom(roomDic[id], id);
        }

    }
    /// <summary>
    /// �Ƴ�����
    /// </summary>
    private void RemoveRoom()
    {
        if (roomDic.ContainsKey(roomID_Offic))
        {
            roomDic[roomID_Offic].SetActive(false);
        }
    }
    private void SetNowRoom(GameObject room, int id)
    {
        if (nowRoom != null)
        {
            roomID_Temp = id;
            nowRoom = room;
        }
    }
    public int GetNowRoomID()
    {
        return roomID_Offic;
    }
    public int GetTempRoomID()
    {
        return roomID_Temp;
    }

    public GameObject GetNowRoom()
    {
        return nowRoom;
    }
    private void SetRoomAreaDataFun(int id)
    {
        switch (id)
        {
            case 1001:
                //����1
                roomAreaData.leftBoundary = 3.25f;
                roomAreaData.rightBoundary = -3.7f;
                roomAreaData.topBoundary = -3.22f;
                roomAreaData.bottomBoundary = 3.5f;
                break;

            case 1002:
                //����2
                roomAreaData.leftBoundary = 4.36f;
                roomAreaData.rightBoundary = -3.7f;
                roomAreaData.topBoundary = -3.22f;
                roomAreaData.bottomBoundary = 3.5f;
                break;

            case 2001:
                //����վ1
                roomAreaData.leftBoundary = 1.95f;
                roomAreaData.rightBoundary = -3.6f;
                roomAreaData.topBoundary = -4.1f;
                roomAreaData.bottomBoundary = -0.3f;
                break;
        }
    }

    private void SetRoomObjPosFun(int id)
    {
        listSpanPos.Clear();
        listFecesPos.Clear();
        switch (id)
        {
            case 1001:
                for (int i = 0; i < arrSpanPosA.Length; i++)
                {
                    listSpanPos.Add(arrSpanPosA[i]);
                }
                for (int i = 0; i < arrFecesPosA.Length; i++)
                {
                    listFecesPos.Add(arrFecesPosA[i]);
                }
                break;
            case 1002:
                for (int i = 0; i < arrSpanPosA.Length; i++)
                {
                    listSpanPos.Add(arrSpanPosA[i]);
                }
                for (int i = 0; i < arrFecesPosA.Length; i++)
                {
                    listFecesPos.Add(arrFecesPosA[i]);
                }
                break;
            case 2001:
                for (int i = 0; i < arrSpanPosB.Length; i++)
                {
                    listSpanPos.Add(arrSpanPosB[i]);
                }
                for (int i = 0; i < arrFecesPosB.Length; i++)
                {
                    listFecesPos.Add(arrFecesPosB[i]);
                }
                break;
        }
    }

    /// <summary>
    /// ���þ���վ����Ĳ���
    /// </summary>
    private void SetAnimalShelterCameraDataFun(int ID)
    {
        if (PetSpanManager.Instance().bInAidStations())
        {
            switch (ID)
            {
                case 1001:
                    Camera.main.transform.position = new Vector3(-0.2f, 2f, -6.7f);
                    Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case 1002:
                    Camera.main.transform.position = new Vector3(-0.2f, 2f, -6f);
                    Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case 2001:
                    Camera.main.transform.position = new Vector3(-0.2f, 2f, -8.3f);
                    Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
            }
        }
    }
    /// <summary>
    /// ���ù��ѱ���
    /// </summary>
    public void SetDogRoomBgTextureFun(int roomID, bool All = false)
    {
        Debug.Log("roomID �������ƣ� " + roomID + "   all " + All);
        string resName = "";
        switch (roomID)
        {
            case 1001:
                if (All)
                {
                    resName = "dogrom1bgA";
                }
                else
                {
                    resName = "dogrom1bg";
                }

                break;
            case 1002:

                if (All)
                {
                    resName = "dogrom2bgA";
                }
                else
                {
                    resName = "dogrom2bg";
                }
                break;
            case 2001:
                if (All)
                {
                    resName = "AnimalShelter01bgA";
                }
                else
                {
                    resName = "AnimalShelter01bg";
                }
                break;
        }
        Debug.Log("���һ�·����ͼƬ����L�� " + resName);
        if (!DicDogRoomBgTexture.ContainsKey(roomID))
        {
            Debug.Log("���������ֵ�� " + resName);
            Texture mTexture = Resources.Load("UIRes/Texture/DogRoomBG/" + resName, typeof(Texture)) as Texture;

            PetSpanManager.Instance().dogBgObj.GetComponent<MeshRenderer>().materials[0].SetTexture("_BaseMap", mTexture);
            /*  if (mTexture != null)
              {
                  DicDogRoomBgTexture.Add(roomID, mTexture);
              }
              else
              {
                  Debug.LogError("����Ϊ�գ����һ�����ݵ�ֵ");
              }*/
        }
        //  SetBgFun(roomID);
    }
    private void SetBgFun(int id)
    {
        if (DicDogRoomBgTexture.ContainsKey(id))
        {
            PetSpanManager.Instance().dogBgObj.GetComponent<MeshRenderer>().materials[0].SetTexture("_BaseMap", DicDogRoomBgTexture[id]);
        }
    }

    public void UnLoadAllRoomFun()
    {
        var keys = roomDic.Keys;
        foreach (var key in keys)
        {
            Destroy(roomDic[key]);
        }
        roomDic.Clear();
        nowRoom = null;
        Debug.Log("ж������");
    }
    /// <summary>
    /// ����󣬽����������д��
    /// </summary>
    /// <param name="roomID"></param>
    public void SaveBuyRoomDataFun(int roomID)
    {
        for (int i = 0; i < AllDogRoomItemData.Count; i++)
        {
            if (AllDogRoomItemData[i].item_id == roomID)
            {
                AllDogRoomItemData[i].is_buy = 1;
            }
        }
    }
    /// <summary>
    /// ����󣬾ͱ���������д��
    /// </summary>
    /// <param name="roomID"></param>
    public void SaveUseRoomDataFun(int roomID)
    {
        for (int i = 0; i < AllDogRoomItemData.Count; i++)
        {
            if (AllDogRoomItemData[i].item_id == roomID)
            {
                AllDogRoomItemData[i].is_select = 1;
            }
            else
            {
                AllDogRoomItemData[i].is_select = 0;
            }
        }
    }
    /// <summary>
    /// ����װ��ģʽ
    /// </summary>
    public void IntoDecoratingModelFun()
    {
        Debug.Log("������װ��ģʽ:  " + roomID_Offic);
        RefreshSelectDataFun();
        PetSpanManager.Instance().LookAtBg();
        PetSpanManager.Instance().SetDogLayerFun(13);
        Debug.Log("���һ��rooid_offic: ������� " + roomID_Offic);
        SetDogRoomBgTextureFun(roomID_Offic, true);
        ChangeRoomDogBgFun(roomID_Offic, true);

    }
    /// <summary>
    /// �˳�װ��ģʽ
    /// </summary>
    public void OutDecoratingModelFun()
    {
        PetSpanManager.Instance().SetDogLayerFun(11);
        PetSpanManager.Instance().outLookAtDogBg();
    }


    public void SetUISelectFun(int index)
    {
        for (int i = 0; i < AllDogRoomItemData.Count; i++)
        {
            if (i == index)
            {
                AllDogRoomItemData[i].is_UISelect = 1;
            }
            else
            {
                AllDogRoomItemData[i].is_UISelect = 0;
            }
        }
    }

}
