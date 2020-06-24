
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
/// <summary>
/// 这个类的自定义面板
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(BuildingAssets))]
public class BuildingAssetsInstance : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();
        BuildingAssets asset = (BuildingAssets)target;
        if (null == asset)
            return;
        GUILayout.Space(20f);
        GUI.backgroundColor = Color.cyan;
        UnityEditor.EditorGUILayout.BeginHorizontal("Button");
        {
            /// 绘制平台
            asset.buildPlatform = (int)(UnityEditor.BuildTarget)UnityEditor.EditorGUILayout.EnumPopup("Platform", (UnityEditor.BuildTarget)asset.buildPlatform);
            /// 选项
            asset.buildOptions = (int)(UnityEditor.BuildOptions)UnityEditor.EditorGUILayout.EnumFlagsField("Options", (UnityEditor.BuildOptions)asset.buildOptions);
        }
        UnityEditor.EditorGUILayout.EndHorizontal();
        /// 绘制流程按钮
        GUILayout.Space(20f);
        FieldInfo fieldAnts = target.GetType().GetField("ants");
        BuildingAssetsAnt[] ants = (BuildingAssetsAnt[])fieldAnts.GetValue(target);
        //BuildingAssetsAnt[] ants = serializedObject.FindProperty("ants").arraySize as BuildingAssetsAnt[];
        FieldInfo fieldFuction = typeof(BuildingAssetsAnt).GetField("functionName");

        //for (int i = 0; null != ants && i < ants.Length; i++)
        for (int i = 0; i < serializedObject.FindProperty("ants").arraySize; i++)
        {
            SerializedProperty antProperty = serializedObject.FindProperty("ants").GetArrayElementAtIndex(i);
            if (null == antProperty)
            {
                GUI.backgroundColor = Color.red;
                UnityEditor.EditorGUILayout.BeginHorizontal("Button");
                {
                    UnityEditor.EditorGUILayout.LabelField("Error Data");
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
                continue;
            }
            /// 绘制选项
            GUI.backgroundColor = string.IsNullOrEmpty(antProperty.FindPropertyRelative("functionName").stringValue) ? Color.red : Color.green;
            UnityEditor.EditorGUILayout.BeginHorizontal("TextField");
            {
                DrawMethodSelect(antProperty);
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
            GUILayout.Space(6f);
        }
        /// 保存
        
        serializedObject.ApplyModifiedProperties();
        UnityEditor.AssetDatabase.SaveAssets();
        GUILayout.Space(50f);
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("DONE ANT"))
        {
            DoneAnts(ants);
        }
    }
    /// <summary>
    /// 执行 ANT 打包
    /// </summary>
    public static void DoneAnts(BuildingAssetsAnt[] sAnts)
    {
        for (int i = 0; null != sAnts && i < sAnts.Length; i++)
        {
            if (null == sAnts[i])
            {
                Debug.LogErrorFormat("Unfind ants[{0}]", i);
                continue;
            }
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(sAnts[i].typeAssembly);
            System.Type type = assembly.GetType(sAnts[i].typeName);

            if (null == type)
            {
                Debug.LogErrorFormat("Unfind Type [{0}] = {1}", i, sAnts[i].typeName);
                continue;
            }
            if (string.IsNullOrEmpty(sAnts[i].functionName))
            {
                Debug.LogFormat("Pass Ant [{0}] = {1}.{2}", i, sAnts[i].typeName, sAnts[i].functionName);
                continue;
            }
            /// 调用
            try
            {
                Debug.LogFormat("Done Ant [{0}] = {1}.{2} Begin", i, sAnts[i].typeName, sAnts[i].functionName);
                type.InvokeMember(sAnts[i].functionName, BindingFlags.InvokeMethod, null, null, null);
                Debug.LogFormat("Done Ant [{0}] = {1}.{2} Succeed", i, sAnts[i].typeName, sAnts[i].functionName);
            }
            catch (System.Exception sE) { Debug.LogErrorFormat("Done Ant [{0}] = {1}.{2} Error : \n{3}", i, sAnts[i].typeName, sAnts[i].functionName, sE); }
        }
    }
    /// <summary>
    /// 绘制按钮类型
    /// </summary>
    Dictionary<System.Type, string[]> _HASH_METHOD = new Dictionary<System.Type, string[]>();
    public void DrawMethodSelect(SerializedProperty sData)
    {
        if (null == sData)
            return;

        System.Type type = System.Type.GetType(sData.FindPropertyRelative("typeName").stringValue);
        if (!_HASH_METHOD.ContainsKey(type))
        {
            List<string> HD = new List<string>();
            HD.Add("NonFunction");
            if (null != type)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name.StartsWith("set_"))
                        continue;
                    if (methods[i].Name.StartsWith("get_"))
                        continue;

                    HD.Add(methods[i].Name);
                }
            }
            _HASH_METHOD.Add(type, HD.ToArray());
        }
        /// 选择函数
        int methodIndex = 0;
        string value = sData.FindPropertyRelative("functionName").stringValue;
        if (!string.IsNullOrEmpty(value))
            for (int index = 0; index < _HASH_METHOD[type].Length; index++)
                if (value == _HASH_METHOD[type][index])
                {
                    methodIndex = index;
                    break;
                }

        methodIndex = UnityEditor.EditorGUILayout.Popup(string.Format("Event : {0}", sData.FindPropertyRelative("title").stringValue), methodIndex, _HASH_METHOD[type], "TextField");

        if (methodIndex == 0)
            sData.FindPropertyRelative("functionName").stringValue = "";
        else
            sData.FindPropertyRelative("functionName").stringValue = _HASH_METHOD[type][methodIndex];
    }
}