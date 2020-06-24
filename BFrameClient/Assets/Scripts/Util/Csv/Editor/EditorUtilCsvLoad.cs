using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUtilCsvLoad
{

	static Dictionary<System.Type, UtilCsvReader> _hashReader = new Dictionary<System.Type, UtilCsvReader>();
    static Dictionary<System.Type, object> _hashTable = new Dictionary<System.Type, object>();
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static UtilCsvReader GetReader<T>()
        where T : new()
    {
        System.Type type = typeof(T);
        if (_hashReader.ContainsKey(type))
            return _hashReader[type];
        string path = string.Format("{0}/../Config/CSV/{1}.csv", Application.dataPath, type.Name);
        UtilCsvReader result = new UtilCsvReader(System.IO.File.ReadAllText(path));
        _hashReader.Add(type, result);
        return result;
    }
    /// <summary>
    /// 获得所有文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> GetTable<T>()
        where T : new()
    {
        if (_hashTable.ContainsKey(typeof(T)))
            return _hashTable[typeof(T)] as List<T>;
        UtilCsvReader reader = GetReader<T>();
        List<T> result = reader.GetTableAndNew<T>();
        _hashTable.Add(typeof(T), result);
        return result;
    }
    
}
