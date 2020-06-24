using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.shuiqinling.data.utilloadasset
{
    public enum UtilLoadType
    {
        WWW = 1,
        Resource,
        AssetBundle
    }
    interface IUtilLoadAssetsDataValue<T> : IUtilLoadAssetsData
    {
        /// <summary>
        /// 为了获得值的接口
        /// </summary>
        T value { get; }
    }
    /// <summary>
    /// 资源加载接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IUtilLoadAssetsData
    {
        /// <summary>
        /// 必须为绝对路径！！！！这里不做任何路径处理操作
        /// </summary>
        string url { get; }
        bool isDone { get; }
        float progress { get; }
        /// <summary>
        /// 只有加载才会实例化
        /// </summary>
        void Load();
        void Clear();
        string error { get; }

        UtilLoadType type { get; }
        /// 获取资源
        T GetAsset<T>() where T : UnityEngine.Object;
    }

    /// <summary>
    /// 用 WWW 加载
    /// </summary>
    public class UtilLoadAssetsDataWWW : IUtilLoadAssetsData, IUtilLoadAssetsDataValue<WWW>
    {
        string _url;
        WWW _www;
        public UtilLoadAssetsDataWWW(string sUrl)
        {
            _url = sUrl;
        }
        public bool isDone { get { return null != _www && _www.isDone; } }
        public float progress { get { return null != _www ? _www.progress : 0f; } }
        public string url { get { return _url; } }
        public WWW value { get { return _www; } }
        public string error { get { return null != _www ? _www.error : null; } }
        public UtilLoadType type { get { return UtilLoadType.WWW; } }

        public void Load()
        {
            if (null != _www)
                return;
            //UnityEngine.Networking.UnityWebRequest a = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(_url);
            //(a.downloadHandler as UnityEngine.Networking.DownloadHandlerAssetBundle).;
            _www = new WWW(_url);
        }
        public void Clear()
        {
            if (null != _www)
                _www.Dispose();
            _www = null;
            _url = null;
        }
        /// <summary>
        /// 未 完成 ~
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAsset<T>() where T : UnityEngine.Object
        {
            if (!isDone)
                return null;
            if (typeof(T) == typeof(Texture2D))
                return _www.texture as T;
            
            return default(T);
        }
    }

    /// <summary>
    /// 用 Resource 加载
    /// </summary>
    public class UtilLoadAssetsDataResource : IUtilLoadAssetsData, IUtilLoadAssetsDataValue<ResourceRequest>
    {
        string _url;
        ResourceRequest _request;
        public UtilLoadAssetsDataResource(string sUrl)
        {
            _url = sUrl;
        }
        public bool isDone { get { return null != _request && _request.isDone; } }
        public float progress { get { return null != _request ? _request.progress : 0f; } }
        public string url { get { return _url; } }
        public ResourceRequest value { get { return _request; } }
        public string error { get { return null; } }
        public UtilLoadType type { get { return UtilLoadType.Resource; } }
        public void Load()
        {
            if (null != _request)
                return;
            _request = Resources.LoadAsync(_url);
        }
        public void Clear()
        {
            if (null != _request)
                Resources.UnloadAsset(_request.asset);
            _request = null;
            _url = null;
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            if (null == _request || null == _request.asset)
                return null;
            return (T)_request.asset;
        }
    }



    /// <summary>
    /// 用 AssetBundle 加载
    /// </summary>
    public class UtilLoadAssetsDataAssetBundle : IUtilLoadAssetsData, IUtilLoadAssetsDataValue<AssetBundleCreateRequest>
    {
        /// <summary>
        /// AB 名字
        /// </summary>
        string _ABName;
        /// <summary>
        /// 获取 返回值
        /// </summary>
        AssetBundleCreateRequest _request;
#if UNITY_EDITOR
        UnityEngine.Object _assetsEditor;
        bool _isDoneEditor = false;
#endif
        /// <summary>
        /// 对象 返回值 列表
        /// </summary>
        UnityEngine.Object[] _assets = null;
        /// <summary>
        /// 依赖 次数
        /// </summary>
        public int dependenciesCount = 0;
        /// <summary>
        /// 是否 是 依赖
        /// 初始化时候-赋值
        /// 其他为卸载保障
        /// </summary>
        public bool isDependencies = false;
        public static string PathReal(string sABName)
        {
#if UNITY_EDITOR
            /// +这一行 是因为要 不要黄色警告
            /// 好烦！！！！
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
                return string.Format("{0}/assetsbundle/{1}", Application.dataPath + "/../Export", sABName);
#endif
            string result = string.Format("{0}/assetsbundle/{1}", Application.persistentDataPath, sABName);
            if (System.IO.File.Exists(result))
                return result;
            return string.Format("{0}/assetsbundle/{1}", Application.streamingAssetsPath, sABName);
        }
        public UtilLoadAssetsDataAssetBundle(string sABName, bool sIsDependencies = false)
        {
            _ABName = sABName;
            isDependencies = sIsDependencies;
            if (sIsDependencies)
                dependenciesCount = 1;
        }
        public bool isDone
        {
            get
            {
#if UNITY_EDITOR
                return _isDoneEditor;
#endif
                return null != _request && _request.isDone;
            }
        }
        public float progress
        {
            get
            {
#if UNITY_EDITOR
                if (_isDoneEditor)
                    return 1f;
#endif
                return null != _request ? _request.progress : 0f;
            }
        }
        public string url { get { return _ABName; } }
        public AssetBundleCreateRequest value { get { return _request; } }
        public string error { get { return null; } }
        public UtilLoadType type { get { return UtilLoadType.AssetBundle; } }
        public void Load()
        {
            if (null != _request)
                return;
#if UNITY_EDITOR
            _isDoneEditor = true;
            try
            {
                _assetsEditor = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(_ABName)[0]);
            }
            catch { Debug.LogErrorFormat("Error Assets {0}", _ABName); }
            if (_isDoneEditor)
                return;
#endif
            _request = AssetBundle.LoadFromFileAsync(PathReal(_ABName));
        }
        public void Clear()
        {
            if (null != _request && null != _request.assetBundle)
                _request.assetBundle.Unload(true);
#if UNITY_EDITOR
            _assetsEditor = null;
#endif
            _request = null;
            _ABName = null;
        }
        public T GetAsset<T>() where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (null != _assetsEditor)
                return (T)_assetsEditor;
#endif

            if (null == _request || null == _request.assetBundle)
                return null;
            if (null == _assets)
                _assets = _request.assetBundle.LoadAllAssets<T>() as UnityEngine.Object[];
            if (null != _assets && _assets.Length > 0)
                return (T)_assets[0];
            return default(T);
        }

    }
}
