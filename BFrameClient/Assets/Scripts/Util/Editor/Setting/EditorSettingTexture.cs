using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorSettingTexture : MonoBehaviour
{
    /// <summary>
    /// 设置 图片样式
    /// </summary>
    [MenuItem("Assets/Setting/Texture/RGB(A) ASTC 4*4", false , 1)]
    static void MenuSettingTexture_RGBA_ASTC_4()
    {
        SettingTexture(TextureImporterFormat.ASTC_RGBA_4x4);
    }
    [MenuItem("Assets/Setting/Texture/RGB(A) ASTC 8*8", false, 2)]
    static void MenuSettingTexture_RGBA_ASTC_8()
    {
        SettingTexture(TextureImporterFormat.ASTC_RGBA_8x8);
    }
    [MenuItem("Assets/Setting/Texture/RGBA32", false, 3)]
    static void MenuSettingTexture_RGBA32()
    {
        SettingTexture(TextureImporterFormat.RGBA32);
    }
    [MenuItem("Assets/Setting/Texture/RGB24", false, 4)]
    static void MenuSettingTexture_RGB24()
    {
        SettingTexture(TextureImporterFormat.RGB24);
    }



    /// <summary>
    /// 设置图片 样式
    /// </summary>
    /// <param name="iOSFroamt"></param>
    /// <param name="AndroidFroamt"></param>
    static void SettingTexture(TextureImporterFormat iOSFroamt, TextureImporterFormat? AndroidFroamt = null)
    {
        if (null == AndroidFroamt)
            AndroidFroamt = iOSFroamt;
        List<string> fileList = new List<string>();
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
        for (int index = 0; index < fileList.Count; index++)
        {
            string path = fileList[index];
            while (path.LastIndexOf("Assets/", System.StringComparison.Ordinal) != 0 && path.LastIndexOf("Assets/", System.StringComparison.Ordinal) != -1)
                path = path.Substring(path.LastIndexOf("Assets/", System.StringComparison.Ordinal)); 
            UnityEditor.TextureImporter importer = UnityEditor.TextureImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            if (null == importer)
                continue;
            importer.mipmapEnabled = false;
            /// 读取
            TextureImporterPlatformSettings iOS = importer.GetPlatformTextureSettings("iPhone");
            TextureImporterPlatformSettings Android = importer.GetPlatformTextureSettings("Android");
            /// 重写 设成 true
            Android.overridden = iOS.overridden = true;
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            /// 类型设置
            Android.format = (TextureImporterFormat)AndroidFroamt;
            iOS.format = iOSFroamt;
            /// 赋值
            importer.SetPlatformTextureSettings(iOS);
            importer.SetPlatformTextureSettings(Android);
            /// LOG
            Debug.Log(string.Format("Setting Texture : [ Android = {1}, iOS = {2} ]{0}", path, AndroidFroamt, iOSFroamt));
            /// 保存
            importer.SaveAndReimport();
        }
    }
}
