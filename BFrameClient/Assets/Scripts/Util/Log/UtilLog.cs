using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UtilLog : MonoBehaviour 
{
	public delegate void FunctionLog(object sObj);
#if UNITY_EDITOR || POWERFULL
    public static FunctionLog Log = Debug.Log;
	public static FunctionLog LogError = Debug.LogError;
	public static FunctionLog LogWarning = Debug.LogWarning;
#else
 	public static FunctionLog Log = UtilLog.myLog;
 	public static FunctionLog LogError = UtilLog.myLogError;
 	public static FunctionLog LogWarning = UtilLog.myLogWarning;
#endif

    /// 是否有对象
	private static UtilLog _logView;
    /// 是否开启log模式
    public static bool isLogView
	{
		set
		{
			if(value)
			{
				if(null != _logView)
					return;
				GameObject myObj = new GameObject("____screenLog");
				myObj.tag = "undestroy";
				_logView = myObj.AddComponent<UtilLog>();
			}
			if(!value)
			{
				if(null == _logView)
					return;
				_logView.destroy();
			}
		}
		get { return null != _logView; }
	}
	/// 打印的log需不需要提供路径
	public static bool isLogStackTrace = false;
    /// 是否开启log
	public static bool isLogOpen = false;
    /// 是否捕捉写入外部文件
	public static bool isOutFile = false;

	/// 清除内存
	public static void clear()
	{
		_logs.Clear();
	}
	
	/// 输出普通信息
	private static void myLog(object sObj)
	{
		if(!isLogOpen)
			return;
        if (isOutFile)
            Debug.Log(sObj);

		string stackTrace = isLogStackTrace ?  StackTraceUtility.ExtractStackTrace() : null;
		string objString = null == sObj ? "null" : sObj.ToString();
		_logs.Add(new UIScreenLogInfo(objString, stackTrace, 0));
		logSub();
	}
	/// 输出错误信息
	private static void myLogError(object sObj)
	{
		if(!isLogOpen)
			return;
        if (isOutFile)
            Debug.LogError(sObj);

        string stackTrace = isLogStackTrace ?  StackTraceUtility.ExtractStackTrace() : null;
		string objString = null == sObj ? "null" : sObj.ToString();
		_logs.Add(new UIScreenLogInfo(objString, stackTrace, 2));
		logSub();
	}
	/// 输出黄色信息
	private static void myLogWarning(object sObj)
	{
		if(!isLogOpen)
			return;
        if (isOutFile)
            Debug.LogWarning(sObj);

        string stackTrace = isLogStackTrace ? StackTraceUtility.ExtractStackTrace() : null;
        string objString = null == sObj ? "null" : sObj.ToString();
		_logs.Add(new UIScreenLogInfo(objString, stackTrace, 1));
		logSub();
	}
	/// log信息
	private static void logSub()
	{
		while(_logs.Count > 200)
			_logs.RemoveAt(0);
	}
	/// 我的log信息
	private static List<UIScreenLogInfo> _logs = new List<UIScreenLogInfo>();
	/// 我要显示的信息
	private class UIScreenLogInfo
	{
			
		public UIScreenLogInfo(string sValue, string sStackTrace, int sType)
		{
			myValue = sValue;
			myStackTrace = sStackTrace;
			myType = sType;
		}
		public string myValue;
		public string myStackTrace;
		
		/// 类型 0 是普通 1 警示 2 错误信息
		public int myType = 0;
	}
	
	
	/// 自我销毁
	public void destroy()
	{
		_logView = null;
		GameObject.Destroy(this);
		GameObject.Destroy(gameObject);
	}
	
	
	
	/// ===================================================================================================================
	/// 对象
	
	Vector2 postion = Vector2.zero;
	
	void Update()
	{
		if(Input.touches.Length > 0)
			postion.y += Input.touches[0].deltaPosition.y * 10f;
		if(Input.GetAxis("Mouse ScrollWheel") != 0)
			postion.y += Input.GetAxis("Mouse ScrollWheel") * -100f;
		postion.y = Mathf.Max(0f, postion.y);
	}
	
	void OnGUI()
	{
		int len = _logs.Count;
		
		float height = 18f;
		float sx = 0f;
		float sy = 0f;
		/// 绘制底图
		GUI.backgroundColor = Color.black;
		GUI.Box(new Rect(0, 0, 960, 640), "");

		GUI.color = Color.green;
		GUI.Label(new Rect(0f, sy, 960f, height), "Log>> StackTrace : " + (isLogStackTrace ? "O" : "X"));
		sy += height;
		
		/// 滚动条
		GUI.BeginScrollView(new Rect(0f, sy, 960f, 640f), postion, new Rect(0f, sy, 960f, 640f * 15f));
		/// 绘制错误信息
		for(int index = len - 1; index >= 0; index--)
		{
			if(_logs[index].myType == 0)
				GUI.color = Color.white;
			if(_logs[index].myType == 1)
				GUI.color = Color.yellow;
			if(_logs[index].myType == 2)
				GUI.color = Color.red;
			GUI.Label(new Rect(sx, sy, 960f, height), _logs[index].myValue);
			sy += height;
			if(null != _logs[index].myStackTrace)
			{
				int myStackTraceLine = _logs[index].myStackTrace.Split('\n').Length;
				GUI.Label(new Rect(sx + 20f, sy, 960f, height * myStackTraceLine), _logs[index].myStackTrace);
				sy += height * (myStackTraceLine);
			}
		}
		GUI.EndScrollView();
	}	
}
