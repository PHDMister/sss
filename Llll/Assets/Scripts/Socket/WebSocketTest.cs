using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cinemachine;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;


//长链接测试脚本
public class WebSocketTest : BaseUIForm
{
    private InputField input;
    private InputField input2;
    public Text LogText;

    private StringBuilder stringBuilder = new StringBuilder();

    private Dropdown dropdown;
    private InputField InputURL;
    private ScrollRect scrollRect;

    public void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.HideOther;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;



        dropdown = FindComp<Dropdown>("Dropdown");
        dropdown.onValueChanged.AddListener(OnDropDownHandle);
        InputURL = FindComp<InputField>("InputURL");
        scrollRect = FindComp<ScrollRect>("Scroll View");
        input = FindComp<InputField>("InputField");
        input2 = FindComp<InputField>("InputField2");


        RigisterButtonObjectEvent("btn1", Mode1);
        RigisterButtonObjectEvent("btn2", Mode2);
        RigisterButtonObjectEvent("btn3", Mode3);
        RigisterButtonObjectEvent("btn4", OnClear);
        RigisterButtonObjectEvent("btn5", OnConnect);
        RigisterButtonObjectEvent("btn6", OnDisConnect);
        RigisterButtonObjectEvent("btn7", go =>
        {
            UIManager.GetInstance().CloseUIForms("WebSocketTest");
        });
        RigisterButtonObjectEvent("btn8", SendFunc);
        RigisterButtonObjectEvent("btn9", GenProxyParamsTemplete);

        ReceiveMessage("WebSocketAgent_log", kv =>
        {
            ShowLog(kv.Values.ToString());
        });
        ReceiveMessage("WebSocketAgent_logerror", kv =>
        {
            ShowLog(kv.Values.ToString());
        });

        transform.Find("btn1").gameObject.SetActive(false);
        transform.Find("btn2").gameObject.SetActive(false);
        transform.Find("btn3").gameObject.SetActive(false);
    }

    private void ShowLog(string log)
    {
        stringBuilder.AppendLine(log);
        LogText.text = stringBuilder.ToString();
        scrollRect.verticalNormalizedPosition = 0;
    }

    private object ParamMap(string key)
    {
        switch (key)
        {
            case "[userid]": return ManageMentClass.DataManagerClass.userId;
            case "[token]": return ManageMentClass.DataManagerClass.tokenValue_App;
            case "[gametoken]": return ManageMentClass.DataManagerClass.tokenValue_Game;
            case "[landid]": return ManageMentClass.DataManagerClass.landId;
            case "[sceneid]": return ManageMentClass.DataManagerClass.SceneID;
            case "[roomid]": return ManageMentClass.DataManagerClass.roomId;
        }
        return key;
    }

    private string PropNameMatchValue(string pName, string define)
    {
        switch (pName.ToLower())
        {
            case "userid": return "[userid]";
            case "landid": return "[landid]";
            case "roomid": return "[roomid]";
        }
        return define;
    }

    public void Start()
    {
        foreach (var value in Enum.GetValues(typeof(MessageId.Types.Enum)))
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = value + "_" + (int)value;
            dropdown.options.Add(data);
        }

        InputURL.text = WebSocketAgent.Ins.address;
    }

    public override void Display()
    {
        base.Display();
    }

    public override void Hiding()
    {
        base.Hiding();
    }


    public void OnDropDownHandle(int value)
    {
        Dropdown.OptionData optionData = dropdown.options[value];
        string[] proCodeStr = optionData.text.Split('_');
        input.text = string.Concat("Treasure.", proCodeStr[0]);
    }
    private void GenProxyParamsTemplete(GameObject go)
    {
        string ClassName = input.text;
        Type msgType = Type.GetType(ClassName);
        if (msgType == null) return;
        PropertyInfo[] propsInfos = msgType.GetProperties();
        StringBuilder sbBuilder = new StringBuilder();
        foreach (var pInfo in propsInfos)
        {
            if (!CheckPropStatic(pInfo) && pInfo.Name != "Parser" && pInfo.Name != "Descriptor")
                DeepProp(sbBuilder, pInfo, null);
        }
        input2.text = sbBuilder.ToString();
    }
    private void DeepProp(StringBuilder sb, PropertyInfo info, string parentInfo)
    {
        if (info.PropertyType.IsClass && info.PropertyType.IsSealed && !CheckPropStatic(info) && info.PropertyType.Namespace == "Treasure")
        {
            if (parentInfo == null) parentInfo = "";
            parentInfo = info.Name + ".";
            PropertyInfo[] propsInfos = info.PropertyType.GetProperties();
            foreach (var pInfo in propsInfos)
            {
                if (!CheckPropStatic(pInfo) && pInfo.Name != "Parser" && pInfo.Name != "Descriptor")
                    DeepProp(sb, pInfo, parentInfo);
            }
        }
        else
        {
            string defineValue = info.PropertyType.IsValueType ? Activator.CreateInstance(info.PropertyType).ToString() : "null";
            string value = PropNameMatchValue(info.Name, defineValue);
            if (!string.IsNullOrEmpty(parentInfo)) sb.AppendLine(parentInfo + info.Name + "=" + value);
            else sb.AppendLine(info.Name + "=" + value);
        }
    }
    protected bool CheckPropStatic(PropertyInfo propertyInfo)
    {
        var getMethod = propertyInfo.GetMethod;
        if (getMethod != null)
        {
            return getMethod.IsStatic;
        }
        else
        {
            var setMethod = propertyInfo.SetMethod;
            return setMethod.IsStatic;
        }
    }

    public void SendFunc(GameObject go)
    {
        Dropdown.OptionData optionData = dropdown.options[dropdown.value];
        string[] proCodeStr = optionData.text.Split('_');
        uint procol = uint.Parse(proCodeStr[1]);
        if (procol == 0) return;

        string protoMsg = input.text;
        if (string.IsNullOrEmpty(protoMsg)) protoMsg = "Treasure." + proCodeStr[0];
        if (!protoMsg.Contains("Treasure")) protoMsg = "Treasure." + protoMsg;
        ShowLog("WebSocketTest  protoMsg =" + protoMsg);

        string paramStr = input2.text;
        if (string.IsNullOrEmpty(paramStr))
        {
            Type msgType = Type.GetType(protoMsg, true);
            object target = Activator.CreateInstance(msgType);
            ShowLog("WebSocketTest   send proxy msg:" + JsonUntity.ToJSON(target));
            WebSocketAgent.SendMsg(procol, target as IMessage);
        }
        else
        {
            Type msgType = Type.GetType(protoMsg, true);
            object target = Activator.CreateInstance(msgType);
            string[] codeStrings = paramStr.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in codeStrings)
            {
                string[] propStrs = s.Split(new[] { '=', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string propName = propStrs[0];   //第一位是属性名
                string propValue = propStrs[1];   //第二位是值
                if (propValue == "null") continue;

                if (propName.Contains("."))
                {
                    string[] propList = propName.Split('.');
                    if (propList.Length > 1)
                    {
                        object LastPropInstance = target;
                        Type lastType = msgType;
                        List<string> newPropList = new List<string>(propList);
                        newPropList.RemoveAt(newPropList.Count - 1);
                        foreach (var s1 in newPropList)
                        {
                            PropertyInfo property = lastType.GetProperty(s1);
                            if (property != null)
                            {
                                object lastPropValue = property.GetValue(LastPropInstance);
                                if (lastPropValue == null)
                                {
                                    object instance = Activator.CreateInstance(property.PropertyType);
                                    property.SetValue(LastPropInstance, instance);
                                    lastType = property.PropertyType;
                                    LastPropInstance = instance;
                                }
                                else
                                {
                                    lastType = property.PropertyType;
                                    LastPropInstance = lastPropValue;
                                }
                            }
                        }
                        string LastPropName = propList[propList.Length - 1];
                        PropertyInfo lastProp = lastType.GetProperty(LastPropName);
                        if (lastProp != null)
                        {
                            object newValue = Convert.ChangeType(propValue, lastProp.PropertyType);
                            lastProp.SetValue(LastPropInstance, newValue);
                        }
                    }
                }
                else
                {
                    PropertyInfo property = msgType.GetProperty(propName);

                    if (property != null)
                    {
                        string propValueName = propValue.ToLower();
                        if (propValueName.Contains("[") && propValueName.Contains("]"))
                        {
                            property.SetValue(target, ParamMap(propValueName));
                        }
                        else
                        {
                            object newValue = Convert.ChangeType(propValue, property.PropertyType);
                            property.SetValue(target, newValue);
                        }
                    }
                }
            }
            ShowLog("WebSocketTest  [Request]  send proxy msg:" + JsonUntity.ToJSON(target));
            WebSocketAgent.SendMsg(procol, target as IMessage, (code, bytes) =>
            {
                ShowLog($"WebSocketTest [Respones]  {proCodeStr[0]}    code:{code}  bytesLen:{bytes.Length}");
            });
        }
    }


    //第一种方式
    public void Mode1(GameObject go)
    {
        WebSocketAgent.Ins.RemoveAllListener();
        WebSocketAgent.Ins.AddProxy((uint)MessageId.Types.Enum.ChangeSkinResp, (code, bytes) =>
        {
            ChangeSkinResp changeSkin = ChangeSkinResp.Parser.ParseFrom(bytes);
            SendMessage("WebSocketAgent_log", "", $"[WebSocket] mode1 proxy:{(uint)MessageId.Types.Enum.ChangeSkinResp} json={JsonUntity.ToJSON(changeSkin)}");
        });

        ChangeSkinReq caSkinReq = new ChangeSkinReq();
        caSkinReq.UserId = 1000000;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ChangeSkinReq, caSkinReq);
    }
    //第二种方式
    public void Mode2(GameObject go)
    {
        ChangeSkinReq caSkinReq = new ChangeSkinReq();
        caSkinReq.UserId = 20000000;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ChangeSkinReq, caSkinReq, (code, datas) =>
        {
            ChangeSkinResp changeSkinResp = ChangeSkinResp.Parser.ParseFrom(datas);
            SendMessage("WebSocketAgent_log", "", $"[WebSocket] mode2 proxy:{(uint)MessageId.Types.Enum.ChangeSkinReq} json={JsonUntity.ToJSON(changeSkinResp)}");
        });
    }
    //第三种方式
    private TestSyncNetView testSyncNetView;
    public void Mode3(GameObject go)
    {
        if (testSyncNetView == null)
        {
            testSyncNetView = new TestSyncNetView();
        }
        testSyncNetView.UnBindNetAgent();
        testSyncNetView.BindNetAgent(WebSocketAgent.Ins);

        ChangeSkinReq caSkinReq = new ChangeSkinReq();
        caSkinReq.UserId = 30000000;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ChangeSkinReq, caSkinReq);
    }
    //清除log
    public void OnClear(GameObject go)
    {
        stringBuilder.Clear();
        LogText.text = "";
    }

    public void OnConnect(GameObject go)
    {
        if (!WebSocketAgent.Ins.IsConnected)
        {
            string url = InputURL.text;
            if (!string.IsNullOrEmpty(url) && (url.Contains("wss://") || url.Contains("ws://")))
            {
                WebSocketAgent.Ins.address = url;
            }
            WebSocketAgent.Ins.Connect(new WebSocketConfig());
            ReceiveMessage(WebSocketConst.WsNet_OnFirstOpen, kv =>
            {
                RegisterReq registerReq = new RegisterReq();
                registerReq.Token = ManageMentClass.DataManagerClass.tokenValue_Game;
                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.Register, registerReq, (code, bytes) =>
                {
                    if (code != 0) return;
                    RegisterResp registerResp = RegisterResp.Parser.ParseFrom(bytes);
                    if (registerResp.StatusCode == 0)
                    {
                        ManageMentClass.DataManagerClass.userId = registerResp.UserId;
                        ShowLog($"[WebSocket] RegisterResp userId:{registerResp.UserId}");
                    }
                });
            });
        }
    }

    public void OnDisConnect(GameObject go)
    {
        WebSocketAgent.Ins.DisConnect();
    }
}

public class TestSyncNetView : BaseNetView
{
    public TestSyncNetView()
    {
        syncProxy.Add((uint)MessageId.Types.Enum.ChangeSkinResp);
    }

    public override void OnMsgError(uint proxy, int errorCode)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", "[WebSocket] mode3 网络错误 断开");
        MessageCenter.SendMessage("WebSocketAgent_log", kvs);
    }

    public override void OnNetRestore()
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", "[WebSocket] mode3 网络恢复");
        MessageCenter.SendMessage("WebSocketAgent_log", kvs);
    }

    public override void Push(uint proxy, ByteString dataBytes)
    {
        MessageId.Types.Enum msgId = (MessageId.Types.Enum)proxy;
        if (msgId == MessageId.Types.Enum.ChangeSkinResp)
        {
            ChangeSkinResp changeSkinResp = ChangeSkinResp.Parser.ParseFrom(dataBytes);

            KeyValuesUpdate kvs = new KeyValuesUpdate("", $"[WebSocket] mode3 proxy:{(uint)MessageId.Types.Enum.ChangeSkinResp} json={JsonUntity.ToJSON(changeSkinResp)}");
            MessageCenter.SendMessage("WebSocketAgent_log", kvs);
        }
    }
}

