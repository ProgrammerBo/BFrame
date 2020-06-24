using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorSettingAssetBundle : MonoBehaviour
{
    /// <summary>
    /// 设置 AB 信息
    /// </summary>
    [MenuItem("Assets/Setting/Setting AssetBundle")]
    static void MenuSettingAB()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        List<string> fileList = new List<string>();
        List<string> fileRemove = new List<string>();
        /// 遍历 目录 操作
        if (null != Selection.objects)
        {
            for (int index = 0; index < Selection.objects.Length; index++)
                if (null != Selection.objects[index])
                {
                    string path = string.Format("{0}/../{1}", Application.dataPath, AssetDatabase.GetAssetPath(Selection.objects[index]));
                    if (System.IO.Directory.Exists(path))
                        EditorSearchFile.SearchPath(path, fileList, "");
                    else
                        fileList.Add(path);
                }
        }
        /// 资源剔除处理
        RemoveSameAndUnExportAssets(fileList, fileRemove);
        /// 打印 LOG
        for (int index = 0; index < fileList.Count; index++)
        {
            UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath("Assets" + fileList[index].Substring(fileList[index].IndexOf("/Resources-/")));
            if (null == importer)
                continue;
            string assetBundleName = fileList[index].Substring(fileList[index].IndexOf("/Resources-/") + 12);
            assetBundleName = assetBundleName.Substring(0, assetBundleName.LastIndexOf("."));
            assetBundleName = assetBundleName.ToLower();
            string assetBundleVariant = "ab";
            /// 重名返回
            if (importer.assetBundleName == assetBundleName && importer.assetBundleVariant == assetBundleVariant)
                continue;
            /// 设置
            importer.SetAssetBundleNameAndVariant(assetBundleName, assetBundleVariant);
            importer.SaveAndReimport();
            Debug.Log(string.Format("Setting AssetBundle : {0}.{1}", importer.assetBundleName, importer.assetBundleVariant));
        }

        for (int index = 0; index < fileRemove.Count; index++)
        {
            UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath("Assets" + fileRemove[index].Substring(fileRemove[index].IndexOf("/Resources-/")));
            if (null == importer)
                continue;
            /// 没设置 返回
            if (string.IsNullOrEmpty(importer.assetBundleName) && string.IsNullOrEmpty(importer.assetBundleVariant))
                continue;
            /// 的是依赖 返回
            if (importer.assetBundleName.StartsWith("d/"))
                continue;
            /// 设置
            
            Debug.LogError(string.Format("Setting AssetBundle : {0}.{1}", importer.assetBundleName, importer.assetBundleVariant));
            importer.SetAssetBundleNameAndVariant("", "");
            importer.SaveAndReimport();
        }
    }


    [MenuItem("Assets/Setting/Remove AssetBundle")]
    static void MenuRemoveAB()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        List<string> fileList = new List<string>();
        List<string> fileRemove = new List<string>();
        /// 遍历 目录 操作
        if (null != Selection.objects)
        {
            for (int index = 0; index < Selection.objects.Length; index++)
                if (null != Selection.objects[index])
                {
                    string path = string.Format("{0}/../{1}", Application.dataPath, AssetDatabase.GetAssetPath(Selection.objects[index]));
                    if (System.IO.Directory.Exists(path))
                        EditorSearchFile.SearchPath(path, fileList, "");
                    else
                        fileList.Add(path);
                }
        }
        /// 资源剔除处理
        RemoveSameAndUnExportAssets(fileList, fileRemove);
        /// 打印 LOG
        for (int index = 0; index < fileList.Count; index++)
        {
            UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath("Assets" + fileList[index].Substring(fileList[index].IndexOf("/Resources-/")));
            if (null == importer)
                continue;
            
            /// 为空返回
            if (string.IsNullOrEmpty(importer.assetBundleName) && string.IsNullOrEmpty(importer.assetBundleVariant))
                continue;
            /// 设置
            Debug.Log(string.Format("Remove AssetBundle : {0}.{1}", importer.assetBundleName, importer.assetBundleVariant));
            importer.SetAssetBundleNameAndVariant("", "");
            importer.SaveAndReimport();
            
        }

        for (int index = 0; index < fileRemove.Count; index++)
        {
            UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath("Assets" + fileRemove[index].Substring(fileRemove[index].IndexOf("/Resources-/")));
            if (null == importer)
                continue;
            /// 没设置 返回
            if (string.IsNullOrEmpty(importer.assetBundleName) && string.IsNullOrEmpty(importer.assetBundleVariant))
                continue;
            /// 的是依赖 返回
            if (importer.assetBundleName.StartsWith("d/"))
                continue;
            /// 设置

            Debug.LogError(string.Format("Remove AssetBundle : {0}.{1}", importer.assetBundleName, importer.assetBundleVariant));
            importer.SetAssetBundleNameAndVariant("", "");
            importer.SaveAndReimport();
        }
    }


    /// 剔除 重名
    /// 剔除  不需要的东西
    static void RemoveSameAndUnExportAssets(List<string> sResult, List<string> sRemove = null)
    {
        sResult.Sort(new IComparerString());
        /// 为了重名的处理
        HashSet<string> hashFile = new HashSet<string>();
        for (int index = sResult.Count - 1; index >= 0; index--)
        {
            string path = sResult[index];
            sResult[index] = path = path.Replace("\\", "/");
            bool isRemove = false;
            /// 如果不在这个目录跳出 不设置！！！
            if (path.IndexOf("/Resources-/") == -1)
            {
                Debug.LogError(string.Format("Error file not in 'Resources-/' : {0}", path));
                isRemove = true;
            }
            /// 如果是 .meta 跳出
            if (!isRemove && path.ToLower().EndsWith(".meta"))
                isRemove = true;
            if (!isRemove && path.ToLower().EndsWith(".cs"))
                isRemove = true;
            if (!isRemove && path.ToLower().EndsWith(".exe"))
                isRemove = true;
            if (!isRemove && path.ToLower().EndsWith(".dll"))
                isRemove = true;
            if (!isRemove && path.ToLower().EndsWith(".sh"))
                isRemove = true;
            if (!isRemove && path.ToLower().EndsWith(".fbx"))
                isRemove = true;
            /// 证明这玩意没扩展名！！！
            if (!isRemove && path.LastIndexOf(".") < path.LastIndexOf("/"))
                isRemove = true;
            
            /// 重名的也要剔除
            string filePathAndName = path.Substring(0, path.LastIndexOf("."));
            if (!isRemove && hashFile.Contains(filePathAndName))
                isRemove = true;

            /// 剔除
            if (isRemove)
            {
                /// 为了取消设置AB 做准备呀
                if (null != sRemove) sRemove.Add(sResult[index]);
                sResult.RemoveAt(index);
            }
            else hashFile.Add(filePathAndName);
        }
    }
    /// <summary>
    /// 按照扩展名排序
    /// 因为要从后往前查找 所以是反向排序
    /// 
    /// </summary>
    class IComparerString : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string xLast = x.Substring(x.LastIndexOf(".") + 1).ToLower();
            string yLast = y.Substring(y.LastIndexOf(".") + 1).ToLower();

            string[] names = new string[] { "prefab", "mat", "png", "jpg", "tga", "controller", "anim", "ogg", "mp3", "wav"};
            for(int index = 0; index < names.Length; index++)
            {
                if (xLast == names[index])
                    return 1;
                if (yLast == names[index])
                    return -1;
            }
            return 0;
        }
    }
}
