using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUntity
{
    /// <summary>
    /// 将字典类型序列化为json字符串
    /// </summary>
    /// <typeparam name="TKey">字典key</typeparam>
    /// <typeparam name="TValue">字典value</typeparam>
    /// <param name="dict">要序列化的字典数据</param>
    /// <returns>json字符串</returns>
    public static string SerializeDictionaryToJsonString<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0)
            return "";

        string jsonStr = JsonConvert.SerializeObject(dict);
        return jsonStr;
    }

    /// <summary>
    /// 将json字符串反序列化为字典类型
    /// </summary>
    /// <typeparam name="TKey">字典key</typeparam>
    /// <typeparam name="TValue">字典value</typeparam>
    /// <param name="jsonStr">json字符串</param>
    /// <returns>字典数据</returns>
    public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
    {
        if (string.IsNullOrEmpty(jsonStr))
            return new Dictionary<TKey, TValue>();

        Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

        return jsonDict;

    }

    /// <summary>
    /// 把对象转换为JSON字符串
    /// </summary>
    /// <param name="o">对象</param>
    /// <returns>JSON字符串</returns>
    public static string ToJSON(this object o)
    {
        if (o == null)
        {
            return null;
        }
        return JsonConvert.SerializeObject(o);
    }
    /// <summary>
    /// 把Json文本转为实体
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

