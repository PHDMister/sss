/***
 * 
 *    Title: "UIFW" UI框架项目
 *           主题: UI窗体的父类
 *    Description: 
 *           功能：定义所有UI窗体的父类。
 *           定义四个生命周期
 *           
 *           1：Display 显示状态。
 *           2：Hiding 隐藏状态
 *           3：ReDisplay 再显示状态。
 *           4：Freeze 冻结状态。
 *           
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = System.Object;

namespace UIFW
{
    public class BaseUIForm : MonoBehaviour
    {
        #region 用于储存UI打开时需要的数据  一次性存储
        protected static Dictionary<string, object> BlackDatas = new Dictionary<string, object>(4);
        public static void AddBlackData(string key, object data)
        {
            BlackDatas[key] = data;
        }
        public static T PopBlackData<T>(string key) where T : class, new()
        {
            if (BlackDatas.TryGetValue(key, out object data))
            {
                BlackDatas.Remove(key);
                return data as T;
            }
            return null;
        }
        public static object PopBlackData(string key)
        {
            if (BlackDatas.TryGetValue(key, out object data))
            {
                BlackDatas.Remove(key);
                return data;
            }
            return null;
        }
        public static T PeekBlackData<T>(string key) where T : class, new()
        {
            if (BlackDatas.TryGetValue(key, out object data))
            {
                return data as T;
            }
            return null;
        }
        public static void RemoveBlackData(string key)
        {
            BlackDatas.Remove(key);
        }
        #endregion



        /*字段*/
        private UIType _CurrentUIType = new UIType();

        /* 属性*/
        //当前UI窗体类型
        public UIType CurrentUIType
        {
            get { return _CurrentUIType; }
            set { _CurrentUIType = value; }
        }


        #region  窗体的四种(生命周期)状态

        /// <summary>
        /// 显示状态
        /// </summary>
	    public virtual void Display()
        {
            this.gameObject.SetActive(true);
            //设置模态窗体调用(必须是弹出窗体)
            if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, _CurrentUIType.UIForm_LucencyType);
            }

            if (CurrentUIType.UIForms_Type == UIFormType.PopUp || CurrentUIType.UIForms_Type == UIFormType.Top)
            {
                InterfaceHelper.SetJoyStickState(false);
            }
        }

        /// <summary>
        /// 隐藏状态
        /// </summary>
	    public virtual void Hiding()
        {
            this.gameObject.SetActive(false);
            //取消模态窗体调用
            if (_CurrentUIType.UIForms_Type == UIFormType.PopUp || _CurrentUIType.UIForms_Type == UIFormType.Top)
            {
                UIMaskMgr.GetInstance().CancelMaskWindow();
            }

            if (CurrentUIType.UIForms_Type == UIFormType.PopUp || CurrentUIType.UIForms_Type == UIFormType.Top)
            {
                InterfaceHelper.SetJoyStickState(true);
            }
        }

        /// <summary>
        /// 重新显示状态
        /// </summary>
	    public virtual void Redisplay()
        {
            this.gameObject.SetActive(true);
            //设置模态窗体调用(必须是弹出窗体)
            if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, _CurrentUIType.UIForm_LucencyType);
            }
            if (CurrentUIType.UIForms_Type == UIFormType.PopUp || CurrentUIType.UIForms_Type == UIFormType.Top)
            {
                InterfaceHelper.SetJoyStickState(false);
            }
        }

        /// <summary>
        /// 冻结状态
        /// </summary>
	    public virtual void Freeze()
        {
            this.gameObject.SetActive(false);
        }


        #endregion

        #region 封装子类常用的方法

        /// <summary>
        /// 注册按钮事件
        /// </summary>
        /// <param name="buttonName">按钮节点名称</param>
        /// <param name="delHandle">委托：需要注册的方法</param>
	    protected void RigisterButtonObjectEvent(string buttonName, EventTriggerListener.VoidDelegate delHandle)
        {
            GameObject goButton = UnityHelper.FindTheChildNode(this.gameObject, buttonName).gameObject;
            //给按钮注册事件方法
            if (goButton != null)
            {
                EventTriggerListener.Get(goButton).onClick = delHandle;
            }
        }

        protected void RigisterCompEvent(Component comp, EventTriggerListener.VoidDelegate delHandle)
        {
            if (!comp) return;
            EventTriggerListener.Get(comp.gameObject).onClick = delHandle;
        }
        public T FindComp<T>(string path) where T : Component
        {
            if (string.IsNullOrEmpty(path)) return null;
            Transform trans = transform.Find(path);
            if (!trans) return null;
            return trans.GetComponent<T>();
        }
        public T FindComp<T>(Transform parent, string path) where T : Component
        {
            if (string.IsNullOrEmpty(path)) return null;
            Transform trans = parent.Find(path);
            if (!trans) return null;
            return trans.GetComponent<T>();
        }
        /// <summary>
        /// 打开UI窗体
        /// Modify by 20180918
        /// </summary>
        /// <param name="uiFormName">窗体名称</param>
        /// <param name="IsRedirection">是否直接转向</param>
	    protected void OpenUIForm(string uiFormName, bool IsRedirection = false)
        {
            //print(GetType() + "/OpenUIForm()/打开UI窗体名称[uiFormName]= " + uiFormName);//Test
            UIManager.GetInstance().ShowUIForms(uiFormName, IsRedirection);
        }

        /// <summary>
        /// 关闭当前UI窗体
        /// </summary>
	    protected void CloseUIForm()
        {
            string strUIFromName = string.Empty;            //处理后的UIFrom 名称
            int intPosition = -1;

            strUIFromName = GetType().ToString();             //命名空间+类名
            //print("### strUIFromName= " + strUIFromName);

            intPosition = strUIFromName.IndexOf('.');
            if (intPosition != -1)
            {
                //剪切字符串中“.”之间的部分
                strUIFromName = strUIFromName.Substring(intPosition + 1);
            }

            //print("### 剪切后的 strUIFromName= " + strUIFromName);
            UIManager.GetInstance().CloseUIForms(strUIFromName);
        }

        protected void OpenUIFormCheckOpen(string uiFormName, bool IsRedirection = false)
        {
            if (IsUIOpen(FormConst.UITREASUREPARTNERLEAVETIP)) return;
            //print(GetType() + "/OpenUIForm()/打开UI窗体名称[uiFormName]= " + uiFormName);//Test
            UIManager.GetInstance().ShowUIForms(uiFormName, IsRedirection);
        }

        protected bool IsUIOpen(string uiFormName)
        {
            return UIManager.GetInstance().IsOpend(uiFormName);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgType">消息的类型</param>
        /// <param name="msgName">消息名称</param>
        /// <param name="msgContent">消息内容</param>
	    protected void SendMessage(string msgType, string msgName, object msgContent)
        {
            KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
            MessageCenter.SendMessage(msgType, kvs);
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="messagType">消息分类</param>
        /// <param name="handler">消息委托</param>
	    public void ReceiveMessage(string messagType, MessageCenter.DelMessageDelivery handler)
        {
            MessageCenter.AddMsgListener(messagType, handler);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="handler"></param>
        public void RemoveMsgListener(string messageType, MessageCenter.DelMessageDelivery handler)
        {
            MessageCenter.RemoveMsgListener(messageType, handler);
        }
        /// <summary>
        /// 显示语言
        /// </summary>
        /// <param name="id"></param>
	    public string Show(string id)
        {
            string strResult = string.Empty;

            strResult = LauguageMgr.GetInstance().ShowText(id);
            return strResult;
        }

        protected void SetIcon(Image image, string atlas, string sName)
        {
            if (!image || string.IsNullOrEmpty(atlas) || string.IsNullOrEmpty(sName)) return;
            SpriteAtlas sAtlas = Resources.Load<SpriteAtlas>(string.Format("UIRes/Atlas/{0}", atlas));
            Sprite sprite = sAtlas.GetSprite(sName);
            image.sprite = sprite;
        }
        #endregion

    }
}