using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMonoBehaviour : MonoBehaviour
{
    public const string prefabPath = "Assets/ResAB/Prefabs/{0}.prefab";

    private static UIMonoBehaviour _Instance;

    public static T UIShow<T>() where T : UIMonoBehaviour
    {
        Type t = typeof(T);
        GameObject obj = Instantiate(LoaderAssets.Load<GameObject>(string.Format(prefabPath, t.Name)));
        T com = obj.AddComponent<T>();
        obj.transform.parent = _Instance .transform;
        return com;
    }

    protected virtual void UIStart(){}

    protected virtual void UIUpdate(){}

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
           
    }

    private void Start()
    {
        UIStart();
    }

    private void Update()
    {
        UIUpdate();
    }
}
