using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorExportAssets : MonoBehaviour
{
    public static string EXPORT_PATH { get { return Application.dataPath + "/../Export/assetsbundle"; } }
    [MenuItem("Export/Assets/Export AssetBundle")]
    public static void MenuExportAssets()
    {
        /// 如果没有 就创建
        if (!System.IO.Directory.Exists(EditorExportAssets.EXPORT_PATH))
            System.IO.Directory.CreateDirectory(EditorExportAssets.EXPORT_PATH);
#if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(EXPORT_PATH, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
#endif
#if UNITY_IOS
        BuildPipeline.BuildAssetBundles(EXPORT_PATH, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
#endif

    }
}
