/***
 * 
 *    Title: "UIFW" UI框架项目
 *           主题： 消息（传递）中心
 *    Description: 
 *           功能： 负责UI框架中，所有UI窗体中间的数据传值。
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class MessageCenter
    {
        //委托：消息传递
        public delegate void DelMessageDelivery(KeyValuesUpdate kv);

        //消息中心缓存集合
        //<string : 数据大的分类，DelMessageDelivery 数据执行委托>
        public static Dictionary<string, DelMessageDelivery> _dicMessages = new Dictionary<string, DelMessageDelivery>();

        /// <summary>
        /// 增加消息的监听。
        /// </summary>
        /// <param name="messageType">消息分类</param>
        /// <param name="handler">消息委托</param>
	    public static void AddMsgListener(string messageType, DelMessageDelivery handler)
        {
            if (!_dicMessages.ContainsKey(messageType))
            {
                _dicMessages.Add(messageType, null);
            }
            _dicMessages[messageType] += handler;
        }

        /// <summary>
        /// 取消消息的监听
        /// </summary>
        /// <param name="messageType">消息分类</param>
        /// <param name="handele">消息委托</param>
	    public static void RemoveMsgListener(string messageType, DelMessageDelivery handele)
        {
            if (_dicMessages.ContainsKey(messageType))
            {
                _dicMessages[messageType] -= handele;
            }

        }

        /// <summary>
        /// 取消所有指定消息的监听
        /// </summary>
	    public static void ClearALLMsgListener()
        {
            if (_dicMessages != null)
            {
                _dicMessages.Clear();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageType">消息的分类</param>
        /// <param name="kv">键值对(对象)</param>
	    public static void SendMessage(string messageType, KeyValuesUpdate kv)
        {
            DelMessageDelivery del;                         //委托
            if (_dicMessages.TryGetValue(messageType, out del))
            {
                if (del != null)
                {
                    //调用委托
                    del(kv);
                }
            }
            KeyValuesUpdate.Push(kv);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageType">消息的分类</param>
        /// <param name="kv">键值对(对象)</param>
        public static void SendMessage(string messageType, string key, object data)
        {
            KeyValuesUpdate kv = KeyValuesUpdate.Pop(key, data);
            DelMessageDelivery del;                         //委托
            if (_dicMessages.TryGetValue(messageType, out del))
            {
                if (del != null)
                {
                    //调用委托
                    del(kv);
                }
            }
            KeyValuesUpdate.Push(kv);
        }
    }

    /// <summary>
    /// 键值更新对
    /// 功能： 配合委托，实现委托数据传递
    /// </summary>
    public class KeyValuesUpdate
    {   //键
        private string _Key;
        //值
        private object _Values;

        /*  只读属性  */

        public string Key
        {
            get { return _Key; }
        }
        public object Values
        {
            get { return _Values; }
        }

        public bool IsFromPool = false;

        public KeyValuesUpdate(string key, object valueObj)
        {
            _Key = key;
            _Values = valueObj;
        }

        public void SetKv(string key, object valueObj)
        {
            _Key = key;
            _Values = valueObj;
        }

        public void Reset()
        {
            _Key = "";
            _Values = null;
        }


        //默认空
        private static KeyValuesUpdate _empty = new KeyValuesUpdate("", null);
        public static KeyValuesUpdate Empty => _empty;
        //对象池子
        private static Queue<KeyValuesUpdate> kvQueue = new Queue<KeyValuesUpdate>();
        public static KeyValuesUpdate Pop(string key, object valueObj)
        {
            if (kvQueue.Count > 0)
            {
                KeyValuesUpdate kv = kvQueue.Dequeue();
                kv.IsFromPool = true;
                kv.SetKv(key, valueObj);
                return kv;
            }
            KeyValuesUpdate _kv = new KeyValuesUpdate(key, valueObj);
            _kv.IsFromPool = true;
            return _kv;
        }
        public static void Push(KeyValuesUpdate kv)
        {
            if (kv == null) return;
            if (!kv.IsFromPool) return;
            kv.Reset();
            kvQueue.Enqueue(kv);
        }
    }


}