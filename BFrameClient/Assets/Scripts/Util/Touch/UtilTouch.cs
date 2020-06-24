using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UtilTouch : MonoBehaviour
{
	static UtilTouch _instance;
	public static UtilTouch instance
    {
        get
        {
            if(null == _instance)
            {
				GameObject obj = new GameObject("__UtilTouch");
				_instance = obj.AddComponent<UtilTouch>();
            }
			return _instance;
        }
    }
    /// <summary>
    /// 检测是两只手指还是一只手指
    /// </summary>
    public enum UITouchType
    {
        None = 0,
        Touch_1 = 1,
        Touch_2 = 2,
        Touch_3 = 3
    }
    /// <summary>
    /// 当前类型
    /// </summary>
    UITouchType _type = UITouchType.None;
    public UITouchType TouchType { get { return _type; } }
    /// <summary>
    /// 点的数据
    /// </summary>
    public class TouchData
    {
        public TouchData(Touch sTouch)
        {
            fingerId = sTouch.fingerId;
            beginPosition = sTouch.position;
            touch = sTouch;
        }
        public TouchData()
        {
            beginPosition = Input.mousePosition;
        }
        public Touch touch;
        public int fingerId;
        public Vector3 beginPosition;
        public Vector3 currentPosition
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                    return Input.mousePosition;
#endif
                return touch.position;
            }
        }
        public bool IsCanceled = false;
    }
    /// <summary>
    /// 鼠标存储点
    /// </summary>
    Dictionary<int, TouchData> _hashTouch = new Dictionary<int, TouchData>();
    List<TouchData> _listTouch = new List<TouchData>();
	/// <summary>
    /// 刷新数据
    /// </summary>
	void Update ()
    {
        /// 数据初始化
        for (int index = 0; index < _listTouch.Count; index++)
        {
            _listTouch[index].IsCanceled = true;
        }

#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            if (null == EventSystem.current || !EventSystem.current.IsPointerOverGameObject())
                if (!_hashTouch.ContainsKey(0))
                {
                    _hashTouch.Add(0, new TouchData());
                    _listTouch.Add(_hashTouch[0]);
                }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_hashTouch.ContainsKey(0))
            {
                _listTouch.Remove(_hashTouch[0]);
                _hashTouch.Remove(0);
            }
        }
        for (int index = 0; index < _listTouch.Count; index++)
        {
            _listTouch[index].IsCanceled = false;
        }
#endif

        /// 检测所有Touch点
        for (int index = 0; null != Input.touches && index < Input.touches.Length; index++)
        {
            ///Debug.LogFormat("nput.touches[{0}].phase = {1}", index, Input.touches[index].phase);
            /// 新增 Touch 点
            if (Input.touches[index].phase == TouchPhase.Began)
                /// 排除UI层
                if (null == EventSystem.current || !EventSystem.current.IsPointerOverGameObject(Input.touches[index].fingerId))
                    if (!_hashTouch.ContainsKey(Input.touches[index].fingerId))
                    {
                        _hashTouch.Add(Input.touches[index].fingerId, new TouchData(Input.touches[index]));
                        _listTouch.Add(_hashTouch[Input.touches[index].fingerId]);
                        continue;
                    }
            /// 移除 Touch 点
            if (Input.touches[index].phase == TouchPhase.Canceled)
                if (_hashTouch.ContainsKey(Input.touches[index].fingerId))
                {
                    _listTouch.Remove(_hashTouch[Input.touches[index].fingerId]);
                    _hashTouch.Remove(Input.touches[index].fingerId);
                    continue;
                }
            /// 自证 Touch 点没移除
            if (_hashTouch.ContainsKey(Input.touches[index].fingerId))
            {
                _hashTouch[Input.touches[index].fingerId].IsCanceled = false;
                _hashTouch[Input.touches[index].fingerId].touch = Input.touches[index];
            }

        }

        /// 对 Touch 点进行清理操作
        for (int index = _listTouch.Count - 1; index >= 0; index--)
        {
            if (_listTouch[index].IsCanceled)
            {
                _hashTouch.Remove(_listTouch[index].fingerId);
                _listTouch.RemoveAt(index);
            }
        }
        /// 对状态 进行清理操作
        switch(_type)
        {
            case UITouchType.None:
                /// 原始状态转换
                if (_listTouch.Count > 0)
                    _type = (UITouchType)Mathf.Min(_listTouch.Count, 3);
                break;
            case UITouchType.Touch_1:
                /// 一根手指时候可以不考虑逻辑
                /// 随意切换
                _type = (UITouchType)Mathf.Min(_listTouch.Count, 3);
                break;
            case UITouchType.Touch_2:
            case UITouchType.Touch_3:
                /// 三个或者两个手指 直接切换为空状态
                if (_listTouch.Count <= 1)
                    _type = UITouchType.None;
                break;
        }
    }

    /// <summary>
    /// 缩放数据
    /// </summary>
    public float TouchDistance
    {
        get
        {
            switch(_type)
            {
                case UITouchType.None:
                case UITouchType.Touch_1:
                    return 0f;
                case UITouchType.Touch_2:
                case UITouchType.Touch_3:
                    if (_listTouch.Count >= 2)
                        return Vector3.Distance(_listTouch[0].currentPosition, _listTouch[1].currentPosition) - Vector3.Distance(_listTouch[0].beginPosition, _listTouch[1].beginPosition);
                    break;
            }
            return 0f;
        }
    }
    /// <summary>
    /// 移动处理
    /// </summary>
    public Vector3 TouchMove
    {
        get
        {
            if (_type == UITouchType.Touch_1)
                return _listTouch[0].currentPosition - _listTouch[0].beginPosition;
            return Vector3.zero;
        }
    }
}
