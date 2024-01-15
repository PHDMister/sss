using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUntity
{
    /// <summary>
    /// ���ֵ��������л�Ϊjson�ַ���
    /// </summary>
    /// <typeparam name="TKey">�ֵ�key</typeparam>
    /// <typeparam name="TValue">�ֵ�value</typeparam>
    /// <param name="dict">Ҫ���л����ֵ�����</param>
    /// <returns>json�ַ���</returns>
    public static string SerializeDictionaryToJsonString<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0)
            return "";

        string jsonStr = JsonConvert.SerializeObject(dict);
        return jsonStr;
    }

    /// <summary>
    /// ��json�ַ��������л�Ϊ�ֵ�����
    /// </summary>
    /// <typeparam name="TKey">�ֵ�key</typeparam>
    /// <typeparam name="TValue">�ֵ�value</typeparam>
    /// <param name="jsonStr">json�ַ���</param>
    /// <returns>�ֵ�����</returns>
    public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
    {
        if (string.IsNullOrEmpty(jsonStr))
            return new Dictionary<TKey, TValue>();

        Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

        return jsonDict;

    }

    /// <summary>
    /// �Ѷ���ת��ΪJSON�ַ���
    /// </summary>
    /// <param name="o">����</param>
    /// <returns>JSON�ַ���</returns>
    public static string ToJSON(this object o)
    {
        if (o == null)
        {
            return null;
        }
        return JsonConvert.SerializeObject(o);
    }
    /// <summary>
    /// ��Json�ı�תΪʵ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public static T FromJSON<T>(this string input)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
        catch (Exception ex)
        {
            return default(T);
        }
    }
}

