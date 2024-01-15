using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fight;
using Google.Protobuf;
using System;
using UIFW;

public class GameController : BaseUIForm
{
    private static GameController _instance = null;

    public Dictionary<ulong, DoubleacingUserData> DoubleUserDataDic = new Dictionary<ulong, DoubleacingUserData>();

    public Dictionary<int, GameObject> ObssObjDic = new Dictionary<int, GameObject>();

    public Dictionary<int, GameObject> BoxObjDic = new Dictionary<int, GameObject>();


    public static GameController Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject().AddComponent<GameController>();
            _instance.name = "GameControllerRoot";
        }
        return _instance;
    }
    // Start is called before the first frame update

    public bool GameRuning = false;

    public PlayerCharacter_Imp playerCharacter_Imp;
    public PlayerCharacter_Imp moveCharacter_Imp;
    public float[] trackXPos = new float[] { -2.5f, 2.5f };
    public float trackZPos = -40f;

    private GameObject trackBoxRoot;
    private GameObject trackObssRoot;

    public void InitFun()
    {
        UnityEngine.Application.targetFrameRate = 60;
        RegistFun();
        StartInsTrackFun();
        StartInsCarFun();
    }

    /// <summary>
    /// ��ʼ������
    /// </summary>
    void StartInsTrackFun()
    {
        GameObject trackObjRoot = new GameObject("TrackRoot");
        trackObjRoot.transform.parent = transform;
        trackObjRoot.transform.position = Vector3.zero;

        Vector3 trackPos = Vector3.zero;

        for (int i = 0; i < 15; i++)
        {
            GameObject trackObj = Instantiate(Resources.Load("Prefabs/Track/model_zhengti", typeof(GameObject)) as GameObject, trackObjRoot.transform);

            trackPos.z = i * 100;
            trackObj.transform.position = trackPos;

            GameObject dongwu = Instantiate(Resources.Load("Prefabs/Track/dongwu", typeof(GameObject)) as GameObject, trackObjRoot.transform);
            dongwu.transform.position = trackPos;
        }
        GameObject zhongdian = Instantiate(Resources.Load("Prefabs/Track/M_zhongdian01", typeof(GameObject)) as GameObject, trackObjRoot.transform);
        zhongdian.transform.position = trackPos;
    }
    /// <summary>
    /// ��ʼ������
    /// </summary>
    void StartInsCarFun()
    {
        GameObject trackObj = new GameObject("CarRoot");
        trackObj.transform.parent = transform;
        trackObj.transform.position = Vector3.zero;


        GameObject UserCar = Instantiate(Resources.Load("Prefabs/Car/CarRoot", typeof(GameObject)) as GameObject, transform);
        UserCar.name = "UserCar";
        UserCar.transform.parent = trackObj.transform;
        UserCar.AddComponent<PlayerCharacter_Imp>();
        playerCharacter_Imp = UserCar.GetComponent<PlayerCharacter_Imp>();

        GameObject OtherCar = Instantiate(Resources.Load("Prefabs/Car/CarRoot", typeof(GameObject)) as GameObject, transform);
        UserCar.name = "OtherCar";
        OtherCar.transform.parent = trackObj.transform;
        OtherCar.AddComponent<PlayerCharacter_Imp>();
        moveCharacter_Imp = OtherCar.GetComponent<PlayerCharacter_Imp>();

        trackBoxRoot = new GameObject("TrackBoxRoot");
        trackBoxRoot.transform.parent = transform;
        trackBoxRoot.transform.position = Vector3.zero;

        trackObssRoot = new GameObject("TrackObssRoot");
        trackObssRoot.transform.parent = transform;
        trackObssRoot.transform.position = Vector3.zero;

    }
    /// <summary>
    /// �����������
    /// </summary>
    public void SetMainCameraFollowFun()
    {
        if (playerCharacter_Imp)
        {
            Camera.main.gameObject.AddComponent<MainCameraFollow>();
            MainCameraFollow mainCameraFollow = Camera.main.GetComponent<MainCameraFollow>();
            mainCameraFollow.target = playerCharacter_Imp.transform;
        }
    }

    /// <summary>
    /// ��ʼ����Ϸ
    /// </summary>
    public void StartGameFun()
    {
        GameRuning = true;
        playerCharacter_Imp.StartGameFun();
        moveCharacter_Imp.StartGameFun();
        SendMessage("GameBegin", "Success", null);
    }

    public void RegistFun()
    {
        // ״̬ˢ��
        // WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.UserInfoUpdatePush, OnRefeshInfo);
        //λ���ƶ�
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.Move, OnMoveCallbackFun);
        //ʹ�õ��ߵ���Ӧ
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.PropResp, OnUseSkillTipFun);
        //����ʹ�õ���
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.PropPush, OnOtherUseSkillFun);
        //����ʹ�õ���������
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.HitPropPush, OnOtherUserUseSuccessSkillFun);


        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.UserInfoUpdatePush, OnUserInfoUpdatePushFun);

        /* //�����ϰ���
         WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.HitObsPush, OnHitObssFun);

         //��������
         WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.HitBoxPush, OnHitBoxFun);*/
        //��Ϸ����
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.GameEndPush, OnGameEndFun);
    }
    private void OnRefeshInfo(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        Debug.Log("���ӵ�״̬ˢ��");
    }
    /// <summary>
    /// move
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnMoveCallbackFun(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        Debug.Log("�������ƶ�");
        Move moveData = Move.Parser.ParseFrom(data);

        if (moveData.UserId == ManageMentClass.DataManagerClass.userId)
        {
            return;
        }
        if (moveData.Control == 3)
        {
            ControCarLeftFun(moveData.UserId);
        }
        else if (moveData.Control == 4)
        {
            ControCarRightFun(moveData.UserId);
        }
        Debug.Log("���һ��Move�ƶ�ʱ����ʲô���Ķ�����ֵ�� " + moveData.ToJSON());
    }
    /// <summary>
    /// ʹ�õ��ߺ����Ӧ
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnUseSkillTipFun(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        Debug.Log("��������յ������Ѿ��ͷ�������Ϣ��");
        PropResp sitdownPush = PropResp.Parser.ParseFrom(data);
        SendMessage("OnUseSkillTipFun", "Success", sitdownPush);
    }
    /// <summary>
    /// ����ʹ�õ���
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnOtherUseSkillFun(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        PropPush propPush = PropPush.Parser.ParseFrom(data);
        Debug.Log("���һ�±���ʹ�õ��߷��͹��������ݣ� " + propPush.ToJSON() + "   �Լ���UserID : " + ManageMentClass.DataManagerClass.userId);
        if (propPush.UserId == ManageMentClass.DataManagerClass.userId)
        {
            return;
        }
        if (propPush.PropId == 1)
        {
            OtherAttackOtherFun();
        }
    }


    /*  Vector3 a = new Vector3();
      Vector3 b = new Vector3();*/
    private void OnUserInfoUpdatePushFun(uint clientCode, ByteString data)
    {
        UserInfoUpdatePush userInfoUpDate = UserInfoUpdatePush.Parser.ParseFrom(data);
        //  Debug.Log("���һ��updataInfopush���������ֵ�� " + userInfoUpDate.ToJSON());
        /* int nowTime = System.Environment.TickCount;
         a.x = playerCharacter_Imp.transform.position.x;
         a.y = playerCharacter_Imp.transform.position.y;
         a.z = playerCharacter_Imp.transform.position.z + 40;

         b.x = moveCharacter_Imp.transform.position.x;
         b.y = moveCharacter_Imp.transform.position.y;
         b.z = moveCharacter_Imp.transform.position.z + 40;


         if (SSSSSS.Info.UserId == ManageMentClass.DataManagerClass.userId)
         {
             string msg = "���һ��״̬���µ����ݣ� " + "   userID: " + SSSSSS.Info.UserId + " ������λ�� :" + (SSSSSS.Info.CurPos.Pos.Z / 100) + "    �ͻ����Լ��� " + a + "     ������ʱ�����  " + SSSSSS.Info.Timestamp + "  ����ʱ����� " + nowTime;

             Debug.Log($"<color=#FF4040> [�Լ��ĳ���]   {msg} </color>");
         }
         else
         {
             string msg = "���һ��״̬���µ����ݣ� " + "   userID: " + SSSSSS.Info.UserId + " ������λ�� :" + (SSSSSS.Info.CurPos.Pos.Z / 100) + "    ��һ���ˣ� " + b + "     ������ʱ�����  " + SSSSSS.Info.Timestamp + "  ����ʱ����� " + nowTime;

             Debug.Log($"<color=green> [�����˵ĳ���]  {msg} </color>");
         }*/
        if (userInfoUpDate.Info.UserId == ManageMentClass.DataManagerClass.userId)
        {

            string msg = "���һ��updataInfopush���������ֵ�� " + userInfoUpDate.ToJSON();
            Debug.Log($"<color=#FF4040> [�Լ��ĳ���]   {msg} </color>");
            //�Լ�
            //�л��ϰ�����߼�
            playerCharacter_Imp.SetHitStateFun(userInfoUpDate.Info);
            playerCharacter_Imp.SetFrozenStateFun(userInfoUpDate.Info);
            playerCharacter_Imp.SetProtectStateFun(userInfoUpDate.Info);

        }
        else
        {
            string msg = "���һ��updataInfopush���������ֵ�� " + userInfoUpDate.ToJSON();
            Debug.Log($"<color=green> [�����˵ĳ���]  {msg} </color>");
            //��һ����
            moveCharacter_Imp.SetHitStateFun(userInfoUpDate.Info);
            moveCharacter_Imp.SetFrozenStateFun(userInfoUpDate.Info);
            moveCharacter_Imp.SetProtectStateFun(userInfoUpDate.Info);
        }
    }
    /// <summary>
    /// ����ʹ�õ������гɹ�
    /// </summary>
    private void OnOtherUserUseSuccessSkillFun(uint clientCode, ByteString data)
    {

    }
    /*  /// <summary>
      /// �����ϰ���
      /// </summary>
      /// <param name="clientCode"></param>
      /// <param name="data"></param>
      private void OnHitObssFun(uint clientCode, ByteString data)
      {

          HitObsPush hitObsPush = HitObsPush.Parser.ParseFrom(data);

          if (hitObsPush.UserId == ManageMentClass.DataManagerClass.userId)
          {

              playerCharacter_Imp.SetCarStateFun(MoveControllerBase.CharacterState.beHit);
          }
          else
          {
              moveCharacter_Imp.SetCarStateFun(MoveControllerBase.CharacterState.beHit);
          }
      }
      /// <summary>
      /// ��������
      /// </summary>
      /// <param name="clientCode"></param>
      /// <param name="data"></param>
      private void OnHitBoxFun(uint clientCode, ByteString data)
      {
          HitBoxPush hitBoxPush = HitBoxPush.Parser.ParseFrom(data);
          if (hitBoxPush.UserId == ManageMentClass.DataManagerClass.userId)
          {
              if (hitBoxPush.BoxId == 1)
              {
                  //����
                  PlayerAttackOtherFun();
              }
              else if (hitBoxPush.BoxId == 2)
              {
                  //����
                  PlayerDenfensFun();
              }
          }
          else
          {
              if (hitBoxPush.BoxId == 1)
              {
                  //����
                  OtherAttackOtherFun();
              }
              else if (hitBoxPush.BoxId == 2)
              {
                  //����
                  OtherDenfensFun();
              }

          }
      }*/
    /// <summary>
    /// ��Ϸ����
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnGameEndFun(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        GameRuning = false;
        GameEndPush gameEndPush = GameEndPush.Parser.ParseFrom(data);
        if (gameEndPush.IsWin)
        {
            //ʤ��
            OpenUIForm(FormConst.WINGAMETIPSPANEL);
            WinGameTips_Panel winGameTipsPanel = UIManager.GetInstance().GetUIForm(FormConst.WINGAMETIPSPANEL) as WinGameTips_Panel;
            winGameTipsPanel.SetRoleData(gameEndPush);
        }
        else
        {
            //ʧ��
            OpenUIForm(FormConst.FAILGAMETIPSPANEL);
            FailGameTips_Panel failGameTipsPanel = UIManager.GetInstance().GetUIForm(FormConst.FAILGAMETIPSPANEL) as FailGameTips_Panel;
            failGameTipsPanel.SetRoleId((int)DoubleUserDataDic[ManageMentClass.DataManagerClass.userId].RoleTypeId);
        }
    }
    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void OnInitAllGameModelDataFun(ByteString data)
    {


        SceneInitPush sitdownPush = SceneInitPush.Parser.ParseFrom(data);
        Debug.Log("���һ����������Ļص���������ʲô�� " + sitdownPush.ToJSON());

        Debug.Log("������ӵ�ֵ�� " + sitdownPush.Boxs.ToJSON());
        Debug.Log("����ϰ����ֵ�� " + sitdownPush.Obss.ToJSON());
        Debug.Log("���������Ϣ��ֵ�� " + sitdownPush.Infos.ToJSON());
        InitUserDataFun(sitdownPush);
        OnInitTrackBoxFun(sitdownPush);
        OnInitTrackObssFun(sitdownPush);
        OnInitCarFun(sitdownPush);
    }
    public void InitUserDataFun(SceneInitPush sitdownPush)
    {
        DoubleUserDataDic.Clear();
        foreach (var item in sitdownPush.Infos)
        {
            DoubleacingUserData doubleacingUserData = new DoubleacingUserData();
            doubleacingUserData.UserID = item.UserId;
            doubleacingUserData.RoleTypeId = item.RoleType;
            doubleacingUserData.isOwner = ManageMentClass.DataManagerClass.userId == doubleacingUserData.UserID ? true : false;
            doubleacingUserData.UserName = item.UserName;
            doubleacingUserData.TrackStateId = item.CurPos.Pos.X;
            doubleacingUserData.SpeedValue = item.Speed;
            DoubleUserDataDic.Add(item.UserId, doubleacingUserData);
        }
    }
    /// <summary>
    /// ��ʼ����������
    /// </summary>
    public void OnInitTrackBoxFun(SceneInitPush sitdownPush)
    {
        BoxObjDic.Clear();
        Debug.Log("������ɰ�����ĺ����ж��ٸ���   " + trackBoxRoot.transform.childCount);
        if (trackBoxRoot.transform.childCount > 0)
        {
            for (int i = 0; i < trackBoxRoot.transform.childCount; i++)
            {
                Debug.Log("�ڵڼ���ɾ����������������ݣ�  " + i);
                Destroy(trackBoxRoot.transform.GetChild(i).gameObject);
            }
        }
        foreach (var item in sitdownPush.Boxs)
        {
            GameObject box = Instantiate(Resources.Load("Prefabs/Car/Box", typeof(GameObject)) as GameObject, trackBoxRoot.transform);
            box.transform.localPosition = new Vector3(trackXPos[item.Pos.X], item.Pos.Y, (item.Pos.Z / 100 + trackZPos));
            box.AddComponent<BoxPropItem>();
            BoxObjDic.Add(item.Pos.Z, box);
        }
    }
    /// <summary>
    /// ��ʼ���ϰ���
    /// </summary>
    public void OnInitTrackObssFun(SceneInitPush sitdownPush)
    {
        ObssObjDic.Clear();
        if (trackObssRoot.transform.childCount > 0)
        {
            for (int i = 0; i < trackObssRoot.transform.childCount; i++)
            {
                if (trackObssRoot.transform.GetChild(i))
                {
                    Destroy(trackObssRoot.transform.GetChild(i).gameObject);
                }
            }
        }
        foreach (var item in sitdownPush.Obss)
        {
            GameObject box = Instantiate(Resources.Load("Prefabs/Car/Obss", typeof(GameObject)) as GameObject, trackObssRoot.transform);
            box.transform.localPosition = new Vector3(trackXPos[item.Pos.X], item.Pos.Y, (item.Pos.Z / 100 + trackZPos));
            // box.AddComponent<BoxPropItem>();
            ObssObjDic.Add(item.Pos.Z, box);
        }
    }
    private void OnInitCarFun(SceneInitPush sitdownPush)
    {
        Vector3 v3 = Vector3.zero;
        foreach (var item in sitdownPush.Infos)
        {
            if (item.UserId == ManageMentClass.DataManagerClass.userId)
            {
                /*  int posz = 0;
                  int speedA = 0;*/

                //�Լ�
                playerCharacter_Imp.InitStartDataFun(new Vector3(trackXPos[item.CurPos.Pos.X], item.CurPos.Pos.Y, (item.CurPos.Pos.Z / 100 - 40)), item.Speed / 100, (MoveControllerBase.CharacterRole)(item.RoleType - 1));
                /* if (item.CurPos.Pos.X == 0)
                 {
                     posz = 0;
                     speedA = SelfConfigData.CarA;
                 }
                 else
                 {
                     posz = SelfConfigData.pos;
                     speedA = SelfConfigData.CarB;
                 }
                 playerCharacter_Imp.InitStartDataFun(new Vector3(trackXPos[item.CurPos.Pos.X], item.CurPos.Pos.Y, (posz - 40)), speedA, (MoveControllerBase.CharacterRole)(item.RoleType - 1));*/
            }
            else
            {

                /*int posz = 0;
                int speedA = 0;

                //�Լ�
                //  playerCharacter_Imp.InitStartDataFun(new Vector3(trackXPos[item.CurPos.Pos.X], item.CurPos.Pos.Y, (item.CurPos.Pos.Z / 100 - 40)), item.Speed / 100, (MoveControllerBase.CharacterRole)(item.RoleType - 1));
                if (item.CurPos.Pos.X == 0)
                {
                    posz = 0;
                    speedA = SelfConfigData.CarA;
                }
                else
                {
                    posz = SelfConfigData.pos;
                    speedA = SelfConfigData.CarB;
                }*/
                moveCharacter_Imp.InitStartDataFun(new Vector3(trackXPos[item.CurPos.Pos.X], item.CurPos.Pos.Y, (item.CurPos.Pos.Z / 100 - 40)), item.Speed / 100, (MoveControllerBase.CharacterRole)(item.RoleType - 1));
                //  moveCharacter_Imp.InitStartDataFun(new Vector3(trackXPos[item.CurPos.Pos.X], item.CurPos.Pos.Y, (posz - 40)), speedA, (MoveControllerBase.CharacterRole)(item.RoleType - 1));
            }
        }
        Debug.Log("���һ����Ӧ��׼��");
        playerCharacter_Imp.InitStartModelFun();
        moveCharacter_Imp.InitStartModelFun();
        SetMainCameraFollowFun();
    }
    //����������
    public void PlayerAttackOtherFun()
    {
        playerCharacter_Imp.AttackSkillFun(moveCharacter_Imp.gameObject);
    }
    //�����˹�����
    public void OtherAttackOtherFun()
    {
        ToastManager.Instance.ShowNewToast($"<color=red>Σ�գ�</color>", 2f);
        moveCharacter_Imp.AttackSkillFun(playerCharacter_Imp.gameObject);
    }
    /// <summary>
    /// ���Ƴ�������
    /// </summary>
    public void ControCarLeftFun(ulong userID)
    {
        if (DoubleUserDataDic.ContainsKey(userID))
        {

            if (DoubleUserDataDic[userID].TrackStateId > 0)
            {
                if (userID == ManageMentClass.DataManagerClass.userId)
                {
                    if (playerCharacter_Imp.characterFrozenState == MoveControllerBase.CharacterFrozenState.normoal && playerCharacter_Imp.characterHitState == MoveControllerBase.CharacterHitState.normoal)
                    {
                        playerCharacter_Imp.transform.position = new Vector3(trackXPos[0], playerCharacter_Imp.transform.position.y, playerCharacter_Imp.transform.position.z);
                        Move move = new Move();
                        move.Control = 3;
                        move.UserId = ManageMentClass.DataManagerClass.userId;
                        WebSocketAgent.Ins.Send((uint)MessageId.Types.Enum.Move, move, true);
                        DoubleUserDataDic[userID].TrackStateId = 0;
                    }
                }
                else
                {
                    moveCharacter_Imp.transform.position = new Vector3(trackXPos[0], moveCharacter_Imp.transform.position.y, moveCharacter_Imp.transform.position.z);
                    DoubleUserDataDic[userID].TrackStateId = 0;
                }

            }
        }
    }
    /// <summary>
    /// ���Ƴ������ƶ�
    /// </summary>
    public void ControCarRightFun(ulong userID)
    {
        if (DoubleUserDataDic.ContainsKey(userID))
        {
            Debug.Log("�ұ��ƶ� userID : " + userID + " manda UserID: " + ManageMentClass.DataManagerClass.userId);
            if (DoubleUserDataDic[userID].TrackStateId <= 0)
            {
                Debug.Log("������������");
                if (userID == ManageMentClass.DataManagerClass.userId)
                {

                    if (playerCharacter_Imp.characterFrozenState == MoveControllerBase.CharacterFrozenState.normoal && playerCharacter_Imp.characterHitState == MoveControllerBase.CharacterHitState.normoal)
                    {
                        Debug.Log("������ӵ����ұ�");
                        playerCharacter_Imp.transform.position = new Vector3(trackXPos[1], playerCharacter_Imp.transform.position.y, playerCharacter_Imp.transform.position.z);
                        Move move = new Move();
                        move.Control = 4;
                        move.UserId = ManageMentClass.DataManagerClass.userId;
                        WebSocketAgent.Ins.Send((uint)MessageId.Types.Enum.Move, move, true);
                        DoubleUserDataDic[userID].TrackStateId = 1;
                    }
                }
                else
                {
                    moveCharacter_Imp.transform.position = new Vector3(trackXPos[1], moveCharacter_Imp.transform.position.y, moveCharacter_Imp.transform.position.z);
                    DoubleUserDataDic[userID].TrackStateId = 1;
                }

            }
        }
    }
}
