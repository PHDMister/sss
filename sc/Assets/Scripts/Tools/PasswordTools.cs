using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class PasswordTools
{
    /// <summary>
    /// Aes 加密
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="StrKey"></param>
    /// <param name="StrIv"></param>
    /// <returns></returns>
    public static string EncryptString(string plainText, string StrKey, string StrIv)
    {
        byte[] key = Encoding.UTF8.GetBytes(StrKey);
        byte[] iv = Encoding.UTF8.GetBytes(StrIv);
        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] encryptedBytes = null;

            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = ms.ToArray();
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }
    /// <summary>
    /// Aes解密
    /// </summary>
    /// <param name="encryptedText"></param>
    /// <param name="StrKey"></param>
    /// <param name="StrIv"></param>
    /// <returns></returns>
    public static string DecryptString(string encryptedText, string StrKey, string StrIv)
    {

        byte[] key = Encoding.UTF8.GetBytes(StrKey);
        byte[] iv = Encoding.UTF8.GetBytes(StrIv);

        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = null;

            Debug.Log("encry::   " + encryptedBytes.Length);
            using (var ms = new System.IO.MemoryStream(encryptedBytes))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[encryptedBytes.Length];
                    
                    int bytesRead = cs.Read(buffer, 0, buffer.Length);
                   
                    decryptedBytes = new byte[bytesRead];

                   
                    Array.Copy(buffer, decryptedBytes, bytesRead);
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

    // 将字符串编码为 Base64
    public static string Base64Encode(this string text)
    {
        return (Convert.ToBase64String(Encoding.Default.GetBytes(text)));
    }
    // 将 Base64 解码还原字符串
    public static string Base64_Decode(string plainText)
    {
        // var plainTextBytes = System.Convert.FromBase64String(plainText);
        return (Encoding.Default.GetString(Convert.FromBase64String(plainText)));
    }
}
