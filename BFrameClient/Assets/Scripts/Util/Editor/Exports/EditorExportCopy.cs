using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorExportCopy : MonoBehaviour
{
	public static string FROM_PATH { get { return Application.dataPath + "/../Export/assetsbundle/"; } }
	public static string TO_PATH { get { return Application.streamingAssetsPath + "/assetsbundle/"; } }
	[MenuItem("Export/Copy/Copy Export 2 StreamingAssets")]
	public static void MenuCopyExport2StreamingAssets()
	{
        /// 删除资源
		if (System.IO.Directory.Exists(TO_PATH))
			System.IO.Directory.Delete(TO_PATH, true);
		if (System.IO.File.Exists(TO_PATH + "../assetsbundle.meta"))
			System.IO.File.Delete(TO_PATH + "../assetsbundle.meta");

		/// 搜索资源
		List<string> paths = new List<string>();
		EditorSearchFile.SearchPath(FROM_PATH, paths, ".ab");
        for(int index = 0; index < paths.Count; index++)
        {
			string path = EditorSearchFile.FilePath(paths[index]);
			path = TO_PATH + path.Substring(path.LastIndexOf("/assetsbundle/") + "/assetsbundle/".Length);
            string name = EditorSearchFile.FileName(paths[index]);
			if (!System.IO.Directory.Exists(path))
				System.IO.Directory.CreateDirectory(path);
			System.IO.File.Copy(paths[index], path + "/" + name + paths[index].Substring(paths[index].LastIndexOf(".")));
		}
        /// AB 资源赋值
		if (System.IO.File.Exists(FROM_PATH + "assetsbundle"))
			System.IO.File.Copy(FROM_PATH + "assetsbundle", TO_PATH + "assetsbundle");

		UnityEditor.AssetDatabase.Refresh();
		UnityEditor.AssetDatabase.SaveAssets();
        /// 刷新
	}
	
}
