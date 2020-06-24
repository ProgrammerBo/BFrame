using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorBuildAssetBundle : Editor
{
    [MenuItem("Tools/AssetBundle/SetAndBuild")]
    static void SetAndBuild()
    {
        //清除所有的AssetBundleName
        Clear();
        //设置指定路径下所有需要打包的assetbundlename
        Set();
        //打包所有需要打包的asset
        Build();
    }

    [MenuItem("Tools/AssetBundle/Set")]
    static void Set()
    {
        string path = Path.Combine(Application.dataPath, "ResAB");
        SetAssetBundlesName(path);
    }

    [MenuItem("Tools/AssetBundle/Builde")]
    static void Build()
    {
        string outputPath = Application.streamingAssetsPath;
        BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
#if UNITY_ANDROID
        BuildTarget targetPlatform = BuildTarget.Android;
#elif UNITY_IOS
        BuildTarget targetPlatform = BuildTarget.iOS;
#else
        BuildTarget targetPlatform = BuildTarget.StandaloneWindows64;
#endif
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform);
    }



    [MenuItem("Tools/AssetBundle/Clear")]
    /// <summary>
    /// 清除所有的AssetBundleName，由于打包方法会将所有设置过AssetBundleName的资源打包，所以自动打包前需要清理
    /// </summary>
    static void Clear()
    {
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();

        //强制删除所有AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
    }

    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName
    /// </summary>
    static void SetAssetBundlesName(string _assetsPath)
    {
        //先获取指定路径下的所有Asset，包括子文件夹下的资源
        DirectoryInfo dir = new DirectoryInfo(_assetsPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos(); //GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)  //如果是文件夹则递归处理
            {
                SetAssetBundlesName(files[i].FullName);
            }
            else if (!files[i].Name.EndsWith(".meta")) //如果是文件的话，则设置AssetBundleName，并排除掉.meta文件
            {
                SetABName(files[i].FullName);     //逐个设置AssetBundleName
            }
        }

    }

    /// <summary>
    /// 设置单个AssetBundle的Name
    /// </summary>
    /// <param name="filePath"></param>
    static void SetABName(string assetPath)
    {
        assetPath = assetPath.Replace("\\", "/");

        string importerPath = assetPath.Replace(Application.dataPath,"Assets");  //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);  //得到Asset
        string assetBundleName = importerPath.Substring(0, importerPath.LastIndexOf("."));
        assetImporter.assetBundleName = assetBundleName;    //最终设置assetBundleName
        //assetImporter.assetBundleVariant = "ab";
    }

}
