using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Reflection;
public class EditorUtilInfoReader : Editor
{
    /// <summary>
    /// 导出类
    /// </summary>
    [MenuItem("Export/Info/Export C# Class")]
    static void MenuInfoExportClass()
    {
        List<string> unexport = new List<string>();
		string pathCsv = Application.dataPath + "/../Config/Info";
        string exportFile = Application.dataPath + "/Ling-Script/Util/Info/UtilInfoReaderType.cs";
        List<string> files = new List<string>();
        EditorSearchFile.SearchPath(pathCsv, files, ".info");
        string text_class = "" +
            "using UnityEngine;\n" +
            "using System.Collections;\n" + 
            "using System.Collections.Generic;\n" +
            "namespace Info\n{\n" +
            "\tpublic class InfoInfo\n" +
            "\t{\n" +
            "\t\tpublic static readonly string[] CLASS_NAME = { {REPACE-0} };\n" +
            "\t\tpublic static readonly System.Type[] CLASS_TYPE = { {REPACE-1} };\n" +
            "\t}\n";
        string text_class_name = "";
        string text_class_type = "";

        /// 遍历csv文件
        for (int index = 0; index < files.Count; index++)
        {
            string fileName = EditorSearchFile.FileName(files[index]);

            if (unexport.IndexOf(fileName) != -1)
                continue;

            text_class_name += (string.IsNullOrEmpty(text_class_name) ? "\"" : ", \"") + fileName + "\"";
            text_class_type += (string.IsNullOrEmpty(text_class_type) ? "typeof(Info." : ", typeof(Info.") + fileName + ")";
            
            /// 检测类中变量是否存在
            string[] lines = System.IO.File.ReadAllLines(files[index]);
            text_class += "\tpublic class " + fileName + " : System.Object\n";
            text_class += "\t{\n";
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                string[] splits = lines[i].Split('=');
                /// 数据 不够 注释补充
                if (splits.Length <= 1)
                {
                    text_class += "\t\t/// " + lines[i] + "\n";
                    continue;
                }
                splits[0] = splits[0].Trim();
                splits[1] = splits[1].Trim();
                /// 属性
                if (splits[0].IndexOf(" ") == -1)
                    text_class += "\t\tpublic string " + splits[0] + ";\n";
                else
                    text_class += "\t\tpublic " + splits[0] + ";\n";
            }
            text_class += "\t}\n";
        }
        text_class += "}\n";

        text_class = text_class.Replace("{REPACE-0}", text_class_name);
        text_class = text_class.Replace("{REPACE-1}", text_class_type);
        System.IO.File.WriteAllText(exportFile, text_class, new System.Text.UTF8Encoding(false));
    }
}
