using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;
public class EditorBuildAssetsCreate : MonoBehaviour
{

    /// <summary>
    /// 创建！！！
    /// </summary>
    class EditorBuildAssetsProfile : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            BuildingAssets profile = EditorBuildAssetsFactory.Create(pathName);
            ProjectWindowUtil.ShowCreatedAsset(profile);
        }
    }
    public class EditorBuildAssetsFactory
    {
        [MenuItem("Assets/Create/Building-Assets-Info", priority = 201)]
        static void MenuCreatePostProcessingProfile()
        {
            var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<EditorBuildAssetsProfile>(), "New Building-Assets-Info.asset", icon, null);
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static BuildingAssets Create(string path)
        {
            BuildingAssets profile = ScriptableObject.CreateInstance<BuildingAssets>();
            profile.name = Path.GetFileName(path);
            profile.ants = new BuildingAssetsAnt[]
                {
                    BuildingAssetsAnt.Create<EditorExportVersion>("设置版本号"),
                    BuildingAssetsAnt.Create<EditorExportConfig>("导出 CSV 文件"),
                    BuildingAssetsAnt.Create<EditorExportAssets>("导出 AB 文件"),
                    BuildingAssetsAnt.Create<EditorExportCopy>("拷贝 AB 文件"),
                    BuildingAssetsAnt.Create<EditorExportPlayer>("创建包体")
                };
            AssetDatabase.CreateAsset(profile, path);
            return profile;
        }
    }
}
