using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Reflection;
public class EditorUtilCsvReader : Editor
{
    /// <summary>
    /// 导出类
    /// </summary>
    [MenuItem("Export/CSV/Export C# Class")]
    static void MenuCsvExportClass()
    {
        List<string> unexport = new List<string>();
		string pathCsv = Application.dataPath + "/../Config/Csv";
        string exportFile = Application.dataPath + "/Ling-Script/Util/Csv/UtilCsvReaderType.cs";
        List<string> fileCsv = new List<string>();
        EditorSearchFile.SearchPath(pathCsv, fileCsv, ".csv");
        string text_class = "" +
            "using UnityEngine;\n" +
            "using System.Collections;\n" + 
            "using System.Collections.Generic;\n" +
            "namespace CSV\n{\n" +
            "\tpublic class CSVInfo\n" +
            "\t{\n" +
            "\t\tpublic static readonly string[] CLASS_NAME = { {REPACE-0} };\n" +
            "\t\tpublic static readonly System.Type[] CLASS_TYPE = { {REPACE-1} };\n" +
            "\t}\n";

        string text_class_name = "";
        string text_class_type = "";

        /// 遍历csv文件
        for (int index = 0; index < fileCsv.Count; index++)
        {
            string fileName = EditorSearchFile.FileName(fileCsv[index]);

            if (unexport.IndexOf(fileName) != -1)
                continue;

            text_class_name += (string.IsNullOrEmpty(text_class_name) ? "\"" : ", \"") + fileName + "\"";
            text_class_type += (string.IsNullOrEmpty(text_class_type) ? "typeof(CSV." : ", typeof(CSV.") + fileName + ")";
            char[] split_char = null;
            switch (fileName)
            {
                case "Dictionary":
                case "DictionaryStatic":
                case "DictionaryLibrary":
                    split_char = new char[] { '\t' };
                    break;
                default:
                    split_char = new char[] { ',' };
                    break;
            }
            /// 检测类中变量是否存在
            string[] data = System.IO.File.ReadAllLines(fileCsv[index]);
            string[] title = data[0].Split(split_char);
            string[] type = data[1].Split(split_char);
            string[] info = data[2].Split(split_char);
            for (int i = 0; i < title.Length; i++)
            {
                if (string.IsNullOrEmpty(title[i]))
                    continue;
            }
            text_class += "\tpublic class " + fileName + " : System.Object\n";
            text_class += "\t{\n";
            for (int i = 0; i < title.Length; i++)
            {
                if (string.IsNullOrEmpty(title[i]))
                    continue;
                text_class += "\t\t/// " + (info.Length > i ? info[i] : "") + "\n";
                text_class += "\t\tpublic " + (type.Length > i && !string.IsNullOrEmpty(type[i]) ? type[i] : "string").Replace(" ", "") + " " + title[i] + ";\n";
            }
            text_class += "\t}\n";
        }
        text_class += "}\n";

        text_class = text_class.Replace("{REPACE-0}", text_class_name);
        text_class = text_class.Replace("{REPACE-1}", text_class_type);
        System.IO.File.WriteAllText(exportFile, text_class, new System.Text.UTF8Encoding(false));
    }
}
