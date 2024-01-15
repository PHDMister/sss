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
    /// 初始化赛道
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
    /// 初始化赛车
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
    /// 设置相机跟随
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
    /// 初始化游戏
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
        // 状态刷新
        // WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.UserInfoUpdatePush, OnRefeshInfo);
        //位置移动
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.Move, OnMoveCallbackFun);
        //使用道具的相应
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.PropResp, OnUseSkillTipFun);
        //别人使用道具
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.PropPush, OnOtherUseSkillFun);
        //别人使用道具命中了
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.HitPropPush, OnOtherUserUseSuccessSkillFun);


        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.UserInfoUpdatePush, OnUserInfoUpdatePushFun);

        /* //遇到障碍物
         WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.HitObsPush, OnHitObssFun);

         //遇到宝箱
         WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.HitBoxPush, OnHitBoxFun);*/
        //游戏结束
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.GameEndPush, OnGameEndFun);
    }
    private void OnRefeshInfo(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        Debug.Log("车子的状态刷新");
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
        Debug.Log("发生了移动");
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
        Debug.Log("输出一下Move移动时做了什么样的东西的值： " + moveData.ToJSON());
    }
    /// <summary>
    /// 使用道具后的相应
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnUseSkillTipFun(uint clientCode, ByteString data)
    {
        if (!GameRuning)
        {
            return;
        }
        Debug.Log("在这里接收到技能已经释放连的消息了");
        PropResp sitdownPush = PropResp.Parser.ParseFrom(data);
        SendMessage("OnUseSkillTipFun", "Success", sitdownPush);
    }
    /// <summary>
    /// 别人使用道具
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
        Debug.Log("输出一下被人使用道具发送过来的数据： " + propPush.ToJSON() + "   自己的UserID : " + ManageMentClass.DataManagerClass.userId);
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
        //  Debug.Log("输出一下updataInfopush里面的数据值： " + userInfoUpDate.ToJSON());
        /* int nowTime = System.Environment.TickCount;
         a.x = playerCharacter_Imp.transform.position.x;
         a.y = playerCharacter_Imp.transform.position.y;
         a.z = playerCharacter_Imp.transform.position.z + 40;

         b.x = moveCharacter_Imp.transform.position.x;
         b.y = moveCharacter_Imp.transform.position.y;
         b.z = moveCharacter_Imp.transform.position.z + 40;


         if (SSSSSS.Info.UserId == ManageMentClass.DataManagerClass.userId)
         {
             string msg = "输出一下状态更新的内容： " + "   userID: " + SSSSSS.Info.UserId + " 服务器位置 :" + (SSSSSS.Info.CurPos.Pos.Z / 100) + "    客户端自己： " + a + "     服务器时间戳：  " + SSSSSS.Info.Timestamp + "  本地时间戳： " + nowTime;

             Debug.Log($"<color=#FF4040> [自己的车子]   {msg} </color>");
         }
         else
         {
             string msg = "输出一下状态更新的内容： " + "   userID: " + SSSSSS.Info.UserId + " 服务器位置 :" + (SSSSSS.Info.CurPos.Pos.Z / 100) + "    另一个人： " + b + "     服务器时间戳：  " + SSSSSS.Info.Timestamp + "  本地时间戳： " + nowTime;

             Debug.Log($"<color=green> [其他人的车子]  {msg} </color>");
         }*/
        if (userInfoUpDate.Info.UserId == ManageMentClass.DataManagerClass.userId)
        {

            string msg = "输出一下updataInfopush里面的数据值： " + userInfoUpDate.ToJSON();
            Debug.Log($"<color=#FF4040> [自己的车子]   {msg} </color>");
            //自己
            //切换障碍物的逻辑
            playerCharacter_Imp.SetHitStateFun(userInfoUpDate.Info);
            playerCharacter_Imp.SetFrozenStateFun(userInfoUpDate.Info);
            playerCharacter_Imp.SetProtectStateFun(userInfoUpDate.Info);

        }
        else
        {
            string msg = "输出一下updataInfopush里面的数据值： " + userInfoUpDate.ToJSON();
            Debug.Log($"<color=green> [其他人的车子]  {msg} </color>");
            //另一个人
            moveCharacter_Imp.SetHitStateFun(userInfoUpDate.Info);
            moveCharacter_Imp.SetFrozenStateFun(userInfoUpDate.Info);
            moveCharacter_Imp.SetProtectStateFun(userInfoUpDate.Info);
        }
    }
    /// <summary>
    /// 别人使用道具命中成功
    /// </summary>
    private void OnOtherUserUseSuccessSkillFun(uint clientCode, ByteString data)
    {

    }
    /*  /// <summary>
      /// 遇到障碍物
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
      /// 遇到宝箱
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
                  //攻击
                  PlayerAttackOtherFun();
              }
              else if (hitBoxPush.BoxId == 2)
              {
                  //冰冻
                  PlayerDenfensFun();
              }
          }
          else
          {
              if (hitBoxPush.BoxId == 1)
              {
                  //攻击
                  OtherAttackOtherFun();
              }
              else if (hitBoxPush.BoxId == 2)
              {
                  //冰冻
                  OtherDenfensFun();
              }

          }
      }*/
    /// <summary>
    /// 游戏结束
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
            //胜利
            OpenUIForm(FormConst.WINGAMETIPSPANEL);
            WinGameTips_Panel winGameTipsPanel = UIManager.GetInstance().GetUIForm(FormConst.WINGAMETIPSPANEL) as WinGameTips_Panel;
            winGameTipsPanel.SetRoleData(gameEndPush);
        }
        else
        {
            //失败
            OpenUIForm(FormConst.FAILGAMETIPSPANEL);
            FailGameTips_Panel failGameTipsPanel = UIManager.GetInstance().GetUIForm(FormConst.FAILGAMETIPSPANEL) as FailGameTips_Panel;
            failGameTipsPanel.SetRoleId((int)DoubleUserDataDic[ManageMentClass.DataManagerClass.userId].RoleTypeId);
        }
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public void OnInitAllGameModelDataFun(ByteString data)
    {


        SceneInitPush sitdownPush = SceneInitPush.Parser.ParseFrom(data);
        Debug.Log("输出一下输出场景的回调的数据是什么： " + sitdownPush.ToJSON());

        Debug.Log("输出箱子的值： " + sitdownPush.Boxs.ToJSON());
        Debug.Log("输出障碍物的值： " + sitdownPush.Obss.ToJSON());
        Debug.Log("输出人物信息的值： " + sitdownPush.Infos.ToJSON());
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
    /// 初始化奖励箱子
    /// </summary>
    public void OnInitTrackBoxFun(SceneInitPush sitdownPush)
    {
        BoxObjDic.Clear();
        Debug.Log("输出嫌疑啊这里的孩子有多少个：   " + trackBoxRoot.transform.childCount);
        if (trackBoxRoot.transform.childCount > 0)
        {
            for (int i = 0; i < trackBoxRoot.transform.childCount; i++)
            {
                Debug.Log("在第几个删除掉了这里面的内容：  " + i);
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
    /// 初始化障碍物
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

                //自己
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

                //自己
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
        Debug.Log("输出一下相应了准备");
        playerCharacter_Imp.InitStartModelFun();
        moveCharacter_Imp.InitStartModelFun();
        SetMainCameraFollowFun();
    }
    //攻击其他人
    public void PlayerAttackOtherFun()
    {
        playerCharacter_Imp.AttackSkillFun(moveCharacter_Imp.gameObject);
    }
    //其他人攻击人
    public void OtherAttackOtherFun()
    {
        ToastManager.Instance.ShowNewToast($"<color=red>危险！</color>", 2f);
        moveCharacter_Imp.AttackSkillFun(playerCharacter_Imp.gameObject);
    }
    /// <summary>
    /// 控制车子左移
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
    /// 控制车子右移动
    /// </summary>
    public void ControCarRightFun(ulong userID)
    {
        if (DoubleUserDataDic.ContainsKey(userID))
        {
            Debug.Log("右变移动 userID : " + userID + " manda UserID: " + ManageMentClass.DataManagerClass.userId);
            if (DoubleUserDataDic[userID].TrackStateId <= 0)
            {
                Debug.Log("这个车子在左边");
                if (userID == ManageMentClass.DataManagerClass.userId)
                {

                    if (playerCharacter_Imp.characterFrozenState == MoveControllerBase.CharacterFrozenState.normoal && playerCharacter_Imp.characterHitState == MoveControllerBase.CharacterHitState.normoal)
                    {
                        Debug.Log("这个车子到了右边");
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
