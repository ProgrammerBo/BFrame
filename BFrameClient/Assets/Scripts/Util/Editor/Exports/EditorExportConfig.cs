using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorExportConfig : MonoBehaviour
{
    public static string CONFIG_PATH { get { return Application.dataPath + "/../Config/"; } }
    public static string EXPORT_PATH { get { return EditorExportAssets.EXPORT_PATH + "/config"; } }
    [MenuItem("Export/Config/Export Config")]
    // Start is called before the first frame update
    public static void MenuExportConfig()
    {
        List<string> paths = new List<string>();

        List<AssetBundleBuild> build_list = new List<AssetBundleBuild>();
        /// 获得所有配置文件
        EditorSearchFile.SearchPath(CONFIG_PATH, paths, null);

        
        /// 生成配置文件文本
        for (int index = paths.Count - 1; index >= 0; index--)
        {
            string path = paths[index].Replace("\\", "/").Replace("//", "/");
            string path_assets = path.Replace("/../Config/", "/_TEMP_CONFIG/");
            if(path_assets.LastIndexOf(".") == -1)
            {
                Debug.LogError(string.Format("Export Config Error > {0}", path));
                continue;
            }
            if (path_assets.LastIndexOf("/.") != -1)
            {
                Debug.LogWarning(string.Format("Export Config Error > {0}", path));
                continue;
            }
            /// 创建目录
            if (!System.IO.Directory.Exists(EditorSearchFile.FilePath(path_assets)))
                System.IO.Directory.CreateDirectory(EditorSearchFile.FilePath(path_assets));

            path_assets = path_assets.Substring(0, path_assets.LastIndexOf("."));
            path_assets = path_assets.Substring(path_assets.LastIndexOf("/Assets/") + 1);
            path_assets += ".asset";
            byte[] data = StringSerialize.ByteEncrypt(System.IO.File.ReadAllBytes(path));
            /// 如果相同
            /// 这个文件就不在导出处理
            /// 这样可以节省下来一些时间
            if (System.IO.File.Exists(Application.dataPath + "/../" + path_assets))
            {
                try
                {
                    if (EditorSearchFile.EqualsBytes(data, AssetDatabase.LoadAssetAtPath<StringSerialize>(path_assets).bytes))
                    {
                        Debug.Log(string.Format("Export Config : {0} -> Continue", path));
                        continue;
                    }
                }
                catch { Debug.LogWarning(string.Format("Export Config (隐藏目录): {0} -> Continue", path)); continue; }
            }
            /// 设置 assetsbundle 打包
            AssetBundleBuild buildTemp = new AssetBundleBuild();
            buildTemp.assetNames = new string[] { path_assets };
            buildTemp.assetBundleVariant = "ab";
            buildTemp.assetBundleName = path_assets.Replace("Assets/_TEMP_CONFIG/", "").ToLower().Replace(".asset", "");
            build_list.Add(buildTemp);
            Debug.Log(string.Format("Export Config : {0} -> New", path));
            /// 序列化文件
            StringSerialize data_serialize = ScriptableObject.CreateInstance<StringSerialize>();
            data_serialize.bytes = data;
            data_serialize.isEncrypt = true;
            AssetDatabase.CreateAsset(data_serialize, path_assets);
            AssetDatabase.ImportAsset(path_assets);
            AssetDatabase.AssetPathToGUID(path_assets);
            AssetDatabase.SaveAssets();
        }
        /// 导出 Config 资源
        if (build_list.Count > 0)
        {
            if (!System.IO.Directory.Exists(EXPORT_PATH))
                System.IO.Directory.CreateDirectory(EXPORT_PATH);
#if UNITY_ANDROID
            BuildPipeline.BuildAssetBundles("Export/assetsbundle/config", build_list.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
#endif
#if UNITY_IOS
            BuildPipeline.BuildAssetBundles("Export/assetsbundle/config", build_list.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
#endif
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    /// <summary>
    /// 清理没有用到的文件
    /// WILL DONE
    /// </summary>
    static void MenuExportClear()
    {

    }
}
