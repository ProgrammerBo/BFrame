using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class UtilInfoReader
{
    
    
    /// 我的文本信息
    private string _text;
    /// <summary>
    /// 进行字符串的裁切属性
    /// </summary>
    private static readonly string _STR_R = "\r";
    private static readonly string _STR_N = "\n";

    /// <summary>
    /// 默认 为 , 分割
    /// </summary>
    /// <param name="sText"></param>
    public UtilInfoReader(string sText)
    {
        _text = sText;
        InitText();
    }
    public UtilInfoReader(FileStream sStream)
    {
        if (sStream == null)
        {
            Debug.LogError("UtilInfoReader sStream is null!!");
            return;
        }
        byte[] temp = new byte[sStream.Length];
        sStream.Read(temp, 0, (int)sStream.Length);
        _text = System.Text.UTF8Encoding.UTF8.GetString(temp);
        InitText();
    }
    /// <summary>
    ///  数据
    /// </summary>
    Dictionary<string, string> _data = new Dictionary<string, string>();

    /// 初始化文本>>数据转换处理
    private void InitText()
    {
        /// 拆分数据
        _text = _text.Replace(_STR_R, _STR_N);
        _text = _text.Replace(_STR_N + _STR_N, _STR_N);
        string[] lines = _text.Split(_STR_N.ToCharArray());
        /// 去表头
        if (lines.Length > 0)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(lines[0]);
            if (byt.Length >= 3 &&
                byt[0] == 0xEF &&
                byt[1] == 0xBB &&
                byt[2] == 0xBF)
            {
                lines[0] = System.Text.Encoding.UTF8.GetString(byt, 3, byt.Length - 3);
            }
        }
        /// 数据拆解
        for(int index = 0; index < lines.Length; index++)
        {
            /// 空白 跳过
            if (string.IsNullOrEmpty(lines[index]))
                continue;
            string[] splits = lines[index].Split('=');
            /// 裁切数据不够
            if (splits.Length <= 1)
                continue;
            splits[0] = splits[0].Trim();
            splits[1] = splits[1].Trim();
            if (string.IsNullOrEmpty(splits[0]) || string.IsNullOrEmpty(splits[1]))
                continue;
            /// 裁切出 Key
            string[] keys = splits[0].Split(' ');
            /// 如果有相同 值 不存入
            if(_data.ContainsKey(keys[keys.Length - 1]))
            {
                Debug.LogErrorFormat("Same Key {0} = {1}", keys[keys.Length - 1], splits[1]);
                continue;
            }
            /// 存入值
            _data.Add(keys[keys.Length - 1], splits[1]);
        }
        /// 原有文本清空
        _text = null;
        lines = null;
    }

    /// <summary>
    /// 属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T ValueNew<T>()
        where T : new()
    {
        T result = new T();
        System.Type type = typeof(T);
        /// 获得属性
        foreach(string key in _data.Keys)
        {
            System.Reflection.FieldInfo field = type.GetField(key);
            if (null == field)
                continue;
            SetFieldValue(field, result);
        }
        return result;
    }


    /// 字段复制
	private void SetFieldValue<T>(System.Reflection.FieldInfo sField, T sObj)
        where T : new()
    {
        if (sObj == null) return;
        if (sField == null) return;
        /// 数据中没有就返回
        string fieldName = sField.Name;

        if (!_data.ContainsKey(fieldName)) return;

        try
        {
            if (sField.FieldType == typeof(string[][]))
            {
                if (_data[fieldName] == null || _data[fieldName] == "" || _data[fieldName] == "#")
                {
                    sField.SetValue(sObj, null);
                    return;
                }
                //先看是不是用#作分割
                string[] arrSplit1 = _data[fieldName].Split("#".ToCharArray());
                if (null != arrSplit1)
                {
                    List<string[]> lstSplit2 = new List<string[]>();
                    for (int i = 0; i < arrSplit1.Length; i++)
                    {
                        //空的就不做考虑
                        if (string.IsNullOrEmpty(arrSplit1[i])) continue;
                        //再分
                        string[] arrTemp = arrSplit1[i].Split("|".ToCharArray());
                        lstSplit2.Add(arrTemp);
                    }
                    if (lstSplit2.Count > 0)
                    {
                        //二维数组赋给二维数组
                        sField.SetValue(sObj, lstSplit2.ToArray());
                    }
                }
                return;
            }

            /// 2016.12.16 add by gus 获得数组
            if (sField.FieldType == typeof(long[]))
            {
                if (_data[fieldName] == "#" || string.IsNullOrEmpty(_data[fieldName]))
                    sField.SetValue(sObj, new long[0]);
                else
                {
                    string[] valS = _data[fieldName].Split("|".ToCharArray());
                    long[] valL = new long[valS.Length];
                    for (int i = 0; i < valS.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(valS[i]))
                            valL[i] = long.Parse(valS[i]);
                    }
                    sField.SetValue(sObj, valL);
                }
                return;
            }
            //如果不是二维数组，直接存起来
            if (sField.FieldType == typeof(string[]))
            {
                if (_data[fieldName] == null || _data[fieldName] == "" || _data[fieldName] == "#")
                    sField.SetValue(sObj, null);
                else
                    sField.SetValue(sObj, _data[fieldName].Split("|".ToCharArray()));
                return;
            }

            if (sField.FieldType == typeof(bool))
            {
                sField.SetValue(sObj, _data[fieldName] == "1");
                return;
            }
            if (sField.FieldType == typeof(long))
            {
                if (_data[fieldName] == "#" || string.IsNullOrEmpty(_data[fieldName]))
                    sField.SetValue(sObj, (long)0);
                else
                    sField.SetValue(sObj, long.Parse(_data[fieldName]));
                return;
            }
            if (sField.FieldType == typeof(ulong))
            {
                if (_data[fieldName] == "#" || string.IsNullOrEmpty(_data[fieldName]))
                    sField.SetValue(sObj, (ulong)0);
                else
                    sField.SetValue(sObj, ulong.Parse(_data[fieldName]));
                return;
            }
            if (sField.FieldType == typeof(int))
            {
                if (_data[fieldName] == "#" || string.IsNullOrEmpty(_data[fieldName]))
                    sField.SetValue(sObj, 0);
                else
                    sField.SetValue(sObj, int.Parse(_data[fieldName]));
                return;
            }
            if (sField.FieldType == typeof(float))
            {
                if (_data[fieldName] == "#" || string.IsNullOrEmpty(_data[fieldName]))
                    sField.SetValue(sObj, 0f);
                else
                    sField.SetValue(sObj, float.Parse(_data[fieldName]));
                return;
            }
            /// 优化内存
            sField.SetValue(sObj, string.Intern(_data[fieldName]));
        }
        catch
        {
            UtilLog.LogWarning(string.Format("UtilInfoReader Set Value Error : {0}.{1} = {2}", sObj.GetType().Name, fieldName, _data[fieldName]));
        }
    }



    /// <summary>
    /// 直接获得属性
    /// </summary>
    public Dictionary<string, string> Value
    {
        get
        {
            return _data;
        }
    }
    /// <summary>
    /// 直接获得方式
    /// </summary>
    /// <param name="sKey"></param>
    /// <returns></returns>
    public string this[string sKey]
    {
        get
        {
            if (_data.ContainsKey(sKey))
                return _data[sKey];
            return null;
        }
    }
}
