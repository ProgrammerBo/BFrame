using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class StringSerialize : ScriptableObject
{
    [SerializeField]
    public byte[] bytes;
    [SerializeField]
    public bool isEncrypt;

    /// <summary>
    /// 按位 取反函数 函数
    /// </summary>
    /// <param name="sData"></param>
    /// <returns></returns>
    public static byte[] ByteEncrypt(byte[] sData)
    {
        for (int index = 0; null != sData && index < sData.Length; index++)
            sData[index] ^= 0xff;
        return sData;
    }
    /// <summary>
    /// 获取字符串值
    /// </summary>
    public string valueString
    {
        get
        {
            // 解密
            if (isEncrypt)
                bytes = ByteEncrypt(bytes);
            isEncrypt = false;
            return new System.Text.UTF8Encoding(true).GetString(bytes);
        }
    }
    /// <summary>
    /// 获取字节流值
    /// </summary>
    public byte[] valueBytes
    {
        get
        {
            if (isEncrypt)
                bytes = ByteEncrypt(bytes);
            isEncrypt = false;
            return bytes;
        }
    }
}
