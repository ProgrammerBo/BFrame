using System.Collections.Generic;
using UnityEngine;

public class LoaderAssets
{
    private const string ABPath = "Assets/StreamingAssets/";

    private static Dictionary<string,AssetBundle > dicBundles = new Dictionary<string, AssetBundle>();

    private static AssetBundle mainManifest;
    private static void LoadMainManifest()
    {
        if (null == mainManifest)
        {
            string filePath = ABPath + "StreamingAssets";
            mainManifest = AssetBundle.LoadFromFile(filePath);
            dicBundles.Add(filePath, mainManifest);
        }
    }

    public static T Load<T>(string path) where T : Object
    {
#if !UNITY_EDITOR
        LoadMainManifest();
        int index = path.LastIndexOf(".");
        if (index > 0)
            path = path.Substring(0,path.LastIndexOf("."));
        string realPath = ABPath + path;
        string fileName = path.Substring(path.LastIndexOf("/") + 1);
        AssetBundleManifest manifest = mainManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] dependencies = manifest.GetAllDependencies(path.ToLower());
        string filePath;
        AssetBundle asset;

        if (dependencies.Length != 0)
        {
            foreach (string s in dependencies)
            {
                //挨个加载依赖关系
                filePath = ABPath + s;
                if (dicBundles.ContainsKey(filePath)) continue;
                AssetBundle temp = AssetBundle.LoadFromFile(filePath);
                dicBundles.Add(filePath, temp);
            }
        }
        filePath = realPath;
        if (dicBundles.ContainsKey(filePath))
        {
            asset = dicBundles[filePath];
        }
        else
        {
            asset = AssetBundle.LoadFromFile(filePath);
        }
        
        T obj = asset.LoadAsset<T>(fileName);
        return obj;
#else
        T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        return obj;
#endif
    }
}
