using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EditorExportPlayer : MonoBehaviour
{
	public static string BUILD_PATH
    {
#if UNITY_IOS || UNITY_IPHONE
		get { return Application.dataPath + "/../Building-iOS/"; }
#endif
#if UNITY_ANDROID
        get { return Application.dataPath + "/../Building-Android/"; }
#endif
	}
	[MenuItem("Export/Building/Player")]
	public static void MenuBuildingPlayer()
	{
		/// 创建目录
		if (!System.IO.Directory.Exists(BUILD_PATH))
			System.IO.Directory.CreateDirectory(BUILD_PATH);
		/// 刷新
#if UNITY_IOS || UNITY_IPHONE
		BuildingPlayer_iOS();
#endif
#if UNITY_ANDROID
        BuildingPlayer_Android();
#endif
	}
    /// <summary>
    /// 创建 ios 包体
    /// </summary>

	static void BuildingPlayer_iOS()
    {
		string[] scenes = { "Assets/_Document.unity" };
	    string path = BUILD_PATH;
	    BuildOptions options = BuildOptions.None;
		options = options | BuildOptions.AcceptExternalModificationsToPlayer;
		UnityEditor.BuildPipeline.BuildPlayer(scenes, path, BuildTarget.iOS, options);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
    /// <summary>
    /// 暂时不考虑
    /// </summary>
	static void BuildingPlayer_Android()
	{

		string[] scenes = { "Assets/_Document" };
		string path = BUILD_PATH;
		BuildOptions options = BuildOptions.None;
		UnityEditor.BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, options);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}
