
using UnityEngine;

/// <summary>
/// 数据
/// </summary>

public class BuildingAssets : ScriptableObject
{
    /// <summary>
    /// 平台
    /// </summary>
    public int buildPlatform = 9;
    /// <summary>
    /// 构建选项
    /// </summary>
    public int buildOptions = 0;

    /// <summary>
    /// 序列化的序列帧事件
    /// </summary>
    public BuildingAssetsAnt[] ants = { };

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="sAssets"></param>
    /// <param name="sIndex"></param>
    public static void Done(string sAssets, int sIndex)
    {

    }
}
/// <summary>
/// 类型数据
/// </summary>
[System.Serializable]
public class BuildingAssetsAnt
{
    /// <summary>
    /// 创建函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sTitle"></param>
    /// <returns></returns>
    public static BuildingAssetsAnt Create<T>(string sTitle)
    {
        BuildingAssetsAnt result = new BuildingAssetsAnt();
        result.typeName = typeof(T).FullName;
        result.typeAssembly = typeof(T).Assembly.FullName;
        result.functionName = null;
        result.title = sTitle;
        return result;
    }
    /// <summary>
    /// 属性信息
    /// </summary>
    public string title;
    public string typeName;
    public string functionName;
    public string typeAssembly;
}