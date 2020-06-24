using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.shuiqinling.data.utilloadasset;
/// <summary>
/// 资源加载处理类
/// 静态方法处理
/// 
/// 同步异步处理 主要处理
/// 
/// Resource 加载
/// AssetsBundle 加载
/// WWW 加载
/// 
/// </summary>
public class UtilLoadAssets : MonoBehaviour
{
    static UtilLoadAssets _instance;
    /// <summary>
    /// 初始化函数
    /// </summary>
    public static void Init()
    {
        if (null != _instance)
            return;
        GameObject obj = new GameObject("____UtilLoadAssets");
        _instance = obj.AddComponent<UtilLoadAssets>();
        UnityEngine.Object.DontDestroyOnLoad(obj);
        /// 获取 manifest
        string path_manifest = UtilLoadAssetsDataAssetBundle.PathReal("assetsbundle");
        try
        {
#if UNITY_EDITOR
            if (!System.IO.File.Exists(path_manifest))
                return;
#endif
            AssetBundle manifestAB = AssetBundle.LoadFromFile(path_manifest);
            string[] names = manifestAB.GetAllAssetNames();
            _manifest = manifestAB.LoadAsset<AssetBundleManifest>(names[0]);
        }
        catch { Debug.LogError(string.Format("Error : Undfind manifest file = {0}", path_manifest)); }
    }

    public delegate void UtilLoadAssetsFunction(string sUrl);
    /// <summary>
    /// 下载完毕的资源缓存地址
    /// </summary>
    static List<IUtilLoadAssetsData> _assets_finished = new List<IUtilLoadAssetsData>();
    /// <summary>
    /// 将要下载的资源缓存地址
    /// </summary>
    static List<IUtilLoadAssetsData> _assets_will_load = new List<IUtilLoadAssetsData>();
    /// <summary>
    /// 正在下载的资源 （定义为数组是因为想要每次会有多个资源同时加载 这样应该会效率一些）
    /// </summary>
    const int MAX_DOWN = 10;
    static List<IUtilLoadAssetsData> _current = new List<IUtilLoadAssetsData>();

    /// <summary>
    /// 这个只是为了这个 东西对数组的指向
    /// 利用了 C# 中 new 类型赋值 指向的是地址的便利
    /// 并不会对内存造成对象拷贝（已字符串为key 值 这地方 不知否优化）
    /// </summary>
    static Dictionary<string, List<IUtilLoadAssetsData>> _hash_list = new Dictionary<string, List<IUtilLoadAssetsData>>();
    static Dictionary<string, IUtilLoadAssetsData> _hash_data = new Dictionary<string, IUtilLoadAssetsData>();
    /// <summary>
    /// 依赖文件加载
    /// </summary>
    static AssetBundleManifest _manifest = null;

    /// <summary>
    /// 侦听列表
    /// </summary>
    static List<HD> _list_hd = new List<HD>();
    /// <summary>
    /// 侦听~用的比较费的方法
    /// </summary>
    class HD
    {
        public HD(List<string> sUrl, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
        {
            url = sUrl;
            succeed = sSucceed;
            error = sError;
        }
        public List<string> url;
        public List<string> dependencies;
        public UtilLoadAssetsFunction succeed;
        public UtilLoadAssetsFunction error;
    }

    /// <summary>
    /// 加载 ab 资源 方式
    /// </summary>
    /// <param name="sUrl"> assetbundle 地址 信息  </param>
    /// <param name="sSucceed"> 成功 回调 </param>
    /// <param name="sError"> 失败 回调 </param>
    public static void LoadAssetBundleAsync(string sUrl, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
    {
        LoadAssetBundleAsync(new List<string>(new string[] { sUrl }), sSucceed, sError);
    }
    public static void LoadAssetBundleAsync(List<string> sUrlList, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
    {
        UtilLoadAssetsDataAssetBundle assetload = null;
        List<string> dependencies = new List<string>();
        for (int index = 0; index < sUrlList.Count; index++)
        {
            string url = string.Intern(sUrlList[index].ToLower());
            if (!_hash_list.ContainsKey(url))
            {
                assetload = new UtilLoadAssetsDataAssetBundle(url);
                _assets_will_load.Add(assetload);
                /// 地址指针指向这个数组
                _hash_list.Add(url, _assets_will_load);
                _hash_data.Add(url, assetload);

                /// 对依赖文件进行处理操作
                LoadAssetBundleDependencies(url, dependencies);
            }
        }
        /// 对 回调处理
        if (null != sSucceed || null != sError)
        {
            HD hd = new HD(sUrlList, sSucceed, sError);
            hd.dependencies = dependencies;
            _list_hd.Add(hd);
        }
    }
    /// <summary>
    /// 加载 依赖 资源 信息
    /// </summary>
    static void LoadAssetBundleDependencies(string sABName, List<string> sResult)
    {
        if (null == _manifest)
            return;
        UtilLoadAssetsDataAssetBundle assetload = null;
        string[] dependencies = _manifest.GetAllDependencies(sABName);
        for(int index = 0; null != dependencies && index < dependencies.Length; index++)
        {
            string url = string.Intern(dependencies[index]);
            if (!_hash_list.ContainsKey(dependencies[index]))
            {
                assetload = new UtilLoadAssetsDataAssetBundle(url, true);
                _assets_will_load.Add(assetload);
                /// 地址指针指向这个数组
                _hash_list.Add(url, _assets_will_load);
                _hash_data.Add(url, assetload);
            }
            /// 计数器累加
            else (_hash_data[url] as UtilLoadAssetsDataAssetBundle).dependenciesCount++;
            sResult.Add(url);
            /// 我不想做递归的
            /// 但是这样最省事。。。。
            /// 好烦！！！
            /// 貌似能开的接口能全部搞定，爽呀！！！！
            //LoadAssetBundleDependencies(url, sResult);
        }
        /// 加载
        
        
    }
    /// <summary>
    /// 加载 Resource 资源 方式
    /// </summary>
    /// <param name="sUrl"> Resource 地址 信息  </param>
    /// <param name="sSucceed"> 成功 回调 </param>
    /// <param name="sError"> 失败 回调 </param>
    public static void LoadResourceAsync(string sUrl, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
    {
        LoadResourceAsync(new List<string>(new string[] { sUrl }), sSucceed, sError);
    }
    public static void LoadResourceAsync(List<string> sUrlList, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
    {
        UtilLoadAssetsDataResource assetload = null;
        for (int index = 0; index < sUrlList.Count; index++)
        {
            string url = string.Intern(sUrlList[index].ToLower());
            if (!_hash_list.ContainsKey(url))
            {
                assetload = new UtilLoadAssetsDataResource(url);
                _assets_will_load.Add(assetload);
                /// 地址指针指向这个数组
                _hash_list.Add(url, _assets_will_load);
                _hash_data.Add(url, assetload);
            }
        }
        /// 对 回调处理
        if (null != sSucceed || null != sError)
            _list_hd.Add(new HD(sUrlList, sSucceed, sError));
    }
    /// <summary>
    /// 加载 WWW 资源 方式
    /// </summary>
    /// <param name="sUrl"> WWW url 地址 信息  </param>
    /// <param name="sSucceed"> 成功 回调 </param>
    /// <param name="sError"> 失败 回调 </param>
    public static void LoadWWWAsync(string sUrl, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
    {
        LoadWWWAsync(new List<string>(new string[] { sUrl }), sSucceed, sError);
    }
    public static void LoadWWWAsync(List<string> sUrlList, UtilLoadAssetsFunction sSucceed, UtilLoadAssetsFunction sError)
    {
        UtilLoadAssetsDataWWW assetload = null;
        for (int index = 0; index < sUrlList.Count; index++)
        {
            string url = string.Intern(sUrlList[index].ToLower());
            if (!_hash_list.ContainsKey(url))
            {
                assetload = new UtilLoadAssetsDataWWW(url);
                _assets_will_load.Add(assetload);
                /// 地址指针指向这个数组
                _hash_list.Add(url, _assets_will_load);
                _hash_data.Add(url, assetload);
            }
        }
        /// 对 回调处理
        if (null != sSucceed || null != sError)
            _list_hd.Add(new HD(sUrlList, sSucceed, sError));
    }

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sUrl"></param>
    /// <returns></returns>
    public static T GetAsset<T>(string sUrl) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(sUrl))
            return null;
        sUrl = sUrl.ToLower();
        if (_hash_data.ContainsKey(sUrl))
            return _hash_data[sUrl].GetAsset<T>();
        return default(T);
    }
    /// <summary>
    /// 检测 是否拥有资源
    /// </summary>
    /// <param name="sUrl"></param>
    /// <returns></returns>
    public static bool HasAsset(string sUrl)
    {
        if (_hash_data.ContainsKey(sUrl))
            return true;
        return false;
    }
    /// <summary>
    /// 检测是否下载完成
    /// </summary>
    /// <param name="sUrl"></param>
    /// <returns></returns>
    public static  bool HasAssetDone(string sUrl)
    {
        if (_hash_data.ContainsKey(sUrl))
            if (_hash_data[sUrl].isDone)
                return _hash_data[sUrl].isDone;
        return false;
    }
    /// <summary>
    /// 进行 卸载
    /// </summary>
    /// <param name="sUrl"></param>
    public static void Unload(string sUrl)
    {
        if (_hash_data.ContainsKey(sUrl))
            _hash_data[sUrl].Clear();
    }
    /// <summary>
    /// 排除 卸载 命令
    /// </summary>
    /// <param name="sUrl"></param>
    public static void UnloadRuledOut(List<string> sUrl)
    {
        foreach (string key in _hash_data.Keys)
            if (sUrl.IndexOf(key) == -1)
                Unload(key);
    }
    /// ================================================================================================================================
    /// 刷新方法处理
    /// 非静态 逻辑处理方式

    /// 刷新函数
    void Update()
    {
        /// 下载 数据
        while (_current.Count < MAX_DOWN)
        {
            /// 数据 增加
            if (_assets_will_load.Count > 0)
            {
                IUtilLoadAssetsData assetdata = _assets_will_load[0];
                _assets_will_load.RemoveAt(0);
                _current.Add(assetdata);
                _hash_list[assetdata.url] = _current;
                /// 直接在这里进行下载
                assetdata.Load();
                continue;
            }
            /// 走到这里就证明没数据了 直接返回就ok
            break;
        }
        /// 对 资源 下载
        for (int index = _current.Count - 1; index >= 0; index--)
        {
            /// 如果 资源 下载完了
            if (_current[index].isDone)
            {
                IUtilLoadAssetsData assetdata = _current[index];
                _current.RemoveAt(index);
                _assets_finished.Add(assetdata);
                _hash_list[assetdata.url] = _assets_finished;
                if (!string.IsNullOrEmpty(assetdata.error))
                    Debug.LogError(assetdata.error);
                continue;
            }
        }
        /// 对 回调进行处理
        for (int index = _list_hd.Count - 1; index >= 0; index--)
        {
            HD hd = _list_hd[index];
            string error = "";
            string url = "";
            string errorurl = "";
            for (int i = 0; null != hd && i < hd.url.Count; i++)
            {
                url = string.Intern(hd.url[i]);
                if (!_hash_data.ContainsKey(url))
                    continue;
                if (_hash_data[url].isDone)
                    continue;
                if (!string.IsNullOrEmpty(_hash_data[url].error))
                    errorurl += url + "\n";
                error += !string.IsNullOrEmpty(_hash_data[url].error) ? _hash_data[url].error : "";
                hd = null;
                break;
            }
            /// 对依赖文件进行查看处理
            for (int i = 0; null != hd && null != hd.dependencies && i < hd.dependencies.Count; i++)
            {
                string dependencies = string.Intern(hd.dependencies[i]);
                if (!_hash_data.ContainsKey(dependencies))
                    continue;
                if (_hash_data[dependencies].isDone)
                    continue;
                if (!string.IsNullOrEmpty(_hash_data[dependencies].error))
                    errorurl += url + "\n";
                error += !string.IsNullOrEmpty(_hash_data[dependencies].error) ? _hash_data[dependencies].error : "";
                hd = null;
                break;
            }


            /// 调用 侦听
            if (null != hd)
            {
                try
                {
                    if (null != hd.succeed)
                        hd.succeed(url);
                    if (null != hd.error && !string.IsNullOrEmpty(error))
                        hd.error(errorurl);
                    if (!string.IsNullOrEmpty(error))
                        Debug.LogError(error);
                } catch (System.Exception sE) { Debug.LogError(sE); }
                _list_hd.Remove(hd);
            }
        }
    }    
}
