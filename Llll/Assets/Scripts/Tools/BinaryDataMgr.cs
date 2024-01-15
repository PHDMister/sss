using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Reflection;

public interface ISingleton
{
    void Init();
}
public abstract class Singleton<T> where T : ISingleton, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Lazy<T>(true).Value;
                instance.Init();
            }
            return instance;
        }
    }
}
public class BinaryDataMgr : ISingleton
{
    /// <summary>
    /// 2进制数据存储位置路径
    /// </summary>
    public static string DATA_BINARY_PATH = Application.dataPath + "/Resources/Binary/";

    /// <summary>
    /// 用于存储所有Excel表数据的容器
    /// </summary>
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();

    /// <summary>
    /// 数据存储的位置
    /// </summary>

    private static string SAVE_PATH = Application.persistentDataPath + "/Data/";
    public static BinaryDataMgr Instance => Singleton<BinaryDataMgr>.Instance;

    public void LoadTable<T, K>()
    {
        Debug.Log(typeof(K).Name);
        TextAsset data = Resources.Load("Binary/" + typeof(K).Name) as TextAsset;
        byte[] bytes = data.bytes;
        //using (FileStream fs = File.Open(DATA_BINARY_PATH + typeof(K).Name + ".txt", FileMode.Open, FileAccess.Read))
        //{
        //byte[] bytes = new byte[fs.Length];
        //fs.Read(bytes,0,bytes.Length);
        //fs.Close();

        //读取多少行数据
        int index = 0;
        int count = BitConverter.ToInt32(bytes, index);
        index += 4;

        int keyNameLength = BitConverter.ToInt32(bytes, index);
        index += 4;
        string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
        index += keyNameLength;

        //创建容器类对象
        Type contaninerType = typeof(T);
        object contaninerObj = Activator.CreateInstance(contaninerType);
        Type classType = typeof(K);
        FieldInfo[] infos = classType.GetFields();

        //Debug.Log(typeof(K).Name + "行数：" + count);
        for (int i = 0; i < count; i++)
        {
            object dataObj = Activator.CreateInstance(classType);
            foreach (FieldInfo info in infos)
            {
                if (info.FieldType == typeof(int))
                {
                    if (index < bytes.Length)//防止取的行数比实际行数大做保护
                    {
                        info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else
                    {
                        index += 4;
                    }
                }
                else if (info.FieldType == typeof(float))
                {
                    if (index < bytes.Length)
                    {
                        info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else
                    {
                        index += 4;
                    }
                }
                else if (info.FieldType == typeof(bool))
                {
                    if (index < bytes.Length)
                    {
                        info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else
                    {
                        index += 1;
                    }
                }
                else if (info.FieldType == typeof(string))
                {
                    if (index < bytes.Length)
                    {
                        int length = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, length));
                        index += length;
                    }
                    else
                    {
                        index += 4;
                    }
                }
            }
            if (index <= bytes.Length)
            {
                object dicObject = contaninerType.GetField("dataDic").GetValue(contaninerObj);
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObj });
            }
        }
        tableDic.Add(typeof(T).Name, contaninerObj);
        //    fs.Close();
        //}
    }
    private Decimal ChangeDataToD(string strData)
    {
        Decimal dData = 0.0M;
        if (strData.Contains("E"))
        {
            dData = Decimal.Parse(strData, System.Globalization.NumberStyles.Float);
        }
        return dData;
    }

    public T LoadTableById<T>(string contaninerName)
    {
        object tableData = null;
        tableDic.TryGetValue(contaninerName, out tableData);
        return (T)tableData;
    }

    public void Init()
    {
        //throw new NotImplementedException();
    }
}
