using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EditorSearchFile
{
    /// <summary>
    /// 递归按照扩展名查找目录所有文件以及索引
    /// </summary>
    /// <param name="sPath"></param>
    /// <param name="sResult"></param>
    /// <param name="sEndStr"></param>
    public static void SearchPath(string sPath, List<string> sResult, string sEndStr = ".prefab")
    {
        if (Directory.Exists(sPath))
        {
            string[] assetList = Directory.GetFileSystemEntries(sPath);
            foreach (string asset in assetList)
            {
                if (File.Exists(asset))
                {
                    /// 如果 扩展名是空的话 就把所有文件都装载
                    if (!string.IsNullOrEmpty(sEndStr) && !asset.EndsWith(sEndStr)) 
                        continue;
                    sResult.Add(asset);
                    continue;
                }
                if (Directory.Exists(asset))
                {
                    SearchPath(asset, sResult, sEndStr);
                }
            }
        }
    }
    /// <summary>
    /// 获得文件名
    /// 舍弃掉扩展名的
    /// </summary>
    /// <param name="sPath"></param>
    /// <returns></returns>
    public static string FileName(string sPath)
    {
        if (string.IsNullOrEmpty(sPath))
            return sPath;
        string result = sPath.Replace("\\", "/").Replace("//", "/");
        result = result.Substring(result.LastIndexOf("/") + 1);
        if (result.LastIndexOf(".") > 0)
            result = result.Substring(0, result.LastIndexOf("."));
        return result;
    }

    public static string FilePath(string sPath)
    {
        if (string.IsNullOrEmpty(sPath))
            return sPath;
        string result = sPath.Replace("\\", "/").Replace("//", "/");
        if (result.LastIndexOf("/") >= 0)
            result = result.Substring(0, result.LastIndexOf("/"));
        return result;
    }
    /// <summary>
    /// 比对字节
    /// 为什么这么写我也不知道
    /// 可就是安全感爆棚！！！
    /// </summary>
    /// <param name="sByteA"></param>
    /// <param name="sByteB"></param>
    /// <returns></returns>
    public static bool EqualsBytes(byte[] sByteA, byte[] sByteB)
    {
        if (null == sByteA && null != sByteB)
            return false;
        if (null != sByteA && null == sByteB)
            return false;
        if (null == sByteA && null == sByteB)
            return true;
        if (sByteA.Length != sByteB.Length)
            return false;
        for (int index = 0; index < sByteA.Length; index++)
            if (sByteA[index] != sByteB[index]) return false;
        return true;
    }

}
