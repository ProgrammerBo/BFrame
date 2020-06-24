using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using System.Text;

//TODO: 找到后需要删除一部分缓存的。

/// csv 表格读取
public class UtilCsvReader
{
	public enum UtilCsvReaderType
    {
        comma,
        tab
    }


    private static readonly string _STR_R = "\r";
	private static readonly string _STR_N = "\n";
    /// <summary>
    /// 不同制表符
    /// </summary>
    static readonly char[] _SPLIT_COMMA = new char[] { ',' };
    static readonly char[] _SPLIT_TAB = new char[] { '\t' };

    /// <summary>
    /// 不同分隔符的正则
    /// </summary>
    static readonly System.Text.RegularExpressions.Regex _SPLIT_REGEX_COMMA = new System.Text.RegularExpressions.Regex("\\G((?:^|,)(?:\"([^\"]*(?:\"\"[^\"]*)*)\"|([^\",]*)))");
    static readonly System.Text.RegularExpressions.Regex _SPLIT_REGEX_TAB = new System.Text.RegularExpressions.Regex("\\G((?:^|\t)(?:\"([^\"]*(?:\"\"[^\"]*)*)\"|([^\"\t]*)))");

    /// a,,b
    /// |[\""]{1,1}([^\""]*?)[\""]{1,1}
    //static readonly System.Text.RegularExpressions.Regex _SPLIT_REGEX_COMMA = new System.Text.RegularExpressions.Regex(@"([^\""\,]*[^\""\,])", System.Text.RegularExpressions.RegexOptions.Multiline);
    //static readonly System.Text.RegularExpressions.Regex _SPLIT_REGEX_TAB = new System.Text.RegularExpressions.Regex(@"([^\""\t]*[^\""\t])|[\""]([^\""]*)[\""]");

    //static readonly System.Text.RegularExpressions.Regex _SPLIT_REGEX_COMMA = new System.Text.RegularExpressions.Regex(@"(?<!""\w+),(?=[\s""])");
    //static readonly System.Text.RegularExpressions.Regex _SPLIT_REGEX_TAB = new System.Text.RegularExpressions.Regex(@"(?<!""\w+)\t(?=[\s""])");

    private UtilCsvReaderType _split_type = UtilCsvReaderType.comma;
    /// <summary>
    /// 不同的分隔符引用操作
    /// </summary>
    private char[] split
    {
        get
        {
            switch(_split_type)
            {
                case UtilCsvReaderType.tab:
                    return _SPLIT_TAB;
                case UtilCsvReaderType.comma:
                    return _SPLIT_COMMA;
            }
            return _SPLIT_COMMA;
        }
    }
    private System.Text.RegularExpressions.Regex regex
    {
        get
        {
            switch (_split_type)
            {
                case UtilCsvReaderType.tab:
                    return _SPLIT_REGEX_TAB;
                case UtilCsvReaderType.comma:
                    return _SPLIT_REGEX_COMMA;
            }
            return _SPLIT_REGEX_COMMA;
        }
    }
    /// 我的文本信息
    private string _text;
	/// 键值
	public Dictionary<string, int> _key = new Dictionary<string, int>();
	/// 我的值存放地点
	private List<string[]> _keyValue = new List<string[]>();
	/// 数据缓存 = 这个文本的数据进行 缓存
	private Dictionary<string, List<Dictionary<string, string>>> _searchs = new Dictionary<string, List<Dictionary<string, string>>>();
	
	/// 是否清除数据
	public bool isClearData = false;

    /// <summary>
    /// 获取hash name
    /// </summary>
    public string fieldNameEndStr;

    /// <summary>
    /// 默认 为 , 分割
    /// </summary>
    /// <param name="sText"></param>
    public UtilCsvReader(string sText, UtilCsvReaderType sSpliteType = UtilCsvReaderType.comma)
	{
        _split_type = sSpliteType;
        _text = sText;
		InitText();
	}
    public UtilCsvReader(FileStream sStream, UtilCsvReaderType sSpliteType = UtilCsvReaderType.comma)
	{
        _split_type = sSpliteType;
        if (sStream == null)
		{
            Debug.LogError("UtilCsvReader sStream is null!!");
            return;
		}
		byte[] temp = new byte[sStream.Length]; 
		sStream.Read(temp, 0, (int)sStream.Length);
		_text = System.Text.UTF8Encoding.UTF8.GetString(temp);
		InitText();
	}


	/// 初始化文本>>数据转换处理
	private void InitText()
	{
		/// 拆分数据
		_text = _text.Replace(_STR_R, _STR_N);
        _text = _text.Replace(_STR_N + _STR_N, _STR_N);
        string[] arr = _text.Split(_STR_N.ToCharArray());
        /// 去表头
        if (arr.Length > 0)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(arr[0]);
            if (byt.Length >= 3 &&
                byt[0] == 0xEF &&
                byt[1] == 0xBB &&
                byt[2] == 0xBF)
            {
                arr[0] = System.Text.Encoding.UTF8.GetString(byt, 3, byt.Length - 3);
            }
        }
        /// 检验数据有效性
        if (arr.Length < 2)
		{
            UtilLog.LogError("表太短>>\n" + arr.Length);
            UtilLog.LogError(_text);
            return;
		}
		
		/// 生成字段key值
		string[] arrLabel = arr[0].Split(split);
		int index = 0;
		for(index = 0; index < arrLabel.Length; index++)
		{
            if (string.IsNullOrEmpty(arrLabel[index]))
                continue;
            if (_key.ContainsKey(arrLabel[index]))
            {
                UtilLog.LogError("表中有相同重复字段>>\n" + arr[0]);
                continue;
            }
			_key.Add(arrLabel[index], index);
			
		}
		/// 生成值内容 [属性有问题再改WILL]
		for(index = 3; index < arr.Length; index++)
		{
            /// 去掉无效行
            if (string.IsNullOrEmpty(arr[index]))
                continue;
            /// 三行 无字段 剔除
            if (arr[index].StartsWith(",,,"))
                continue;
            /// 拆分
            bool IsHead = arr[index][0] == ',';
            /// 使用正则拆分！！！难受
            System.Text.RegularExpressions.MatchCollection regex_math = regex.Matches(IsHead ? "&" + arr[index] : arr[index]); 
            if (regex_math.Count <= 0)
                continue;
            string[] arrValue = new string[regex_math.Count];
            for (int i = 0; i < regex_math.Count; i++)
            {
                arrValue[i] = regex_math[i].Value;
                if (this._split_type == UtilCsvReaderType.comma)
                {
                    /// 去掉没用的双引号
                    if (arrValue[i].StartsWith(",\""))
                    {
                        arrValue[i] = arrValue[i].Substring(2, arrValue[i].Length - 3);
                        continue;
                    }
                    /// 不可能出现小于这个的情况
                    switch (arrValue[i][0])
                    {
                        case ',':
                        case '&':
                            arrValue[i] = arrValue[i].Substring(1);
                            break;
                        /// 头文件没有,号
                        case '"':
                            arrValue[i] = arrValue[i].Substring(1, arrValue[i].Length - 2);
                            break;
                    }
                }
                if (this._split_type == UtilCsvReaderType.tab)
                {
                    /// 去掉没用的双引号
                    if (arrValue[i].StartsWith("\t\""))
                    {
                        arrValue[i] = arrValue[i].Substring(2, arrValue[i].Length - 3);
                        continue;
                    }
                    /// 长度不够
                    if (arrValue[i].Length <= 0)
                        continue;
                    /// 不可能出现小于这个的情况
                    switch (arrValue[i][0])
                    {
                        case '\t':
                        case '&':
                            arrValue[i] = arrValue[i].Substring(1);
                            break;
                        /// 头文件没有,号
                        case '"':
                            arrValue[i] = arrValue[i].Substring(1, arrValue[i].Length - 2);
                            break;
                    }
                }

            }
            _keyValue.Add(arrValue);
		}
		/// 原有文本清空
		_text = null;
		arr = null;
	}
    public void InternValue()
    {
        for (int i = 0; null != _keyValue && i < _keyValue.Count; i++)
            for (int j = 0; null != _keyValue[i] && j < _keyValue[i].Length; j++)
                if (!string.IsNullOrEmpty(_keyValue[i][j]))
                    _keyValue[i][j] = string.Intern(_keyValue[i][j]);
    }
	/// 寻找多行数据
	public List<Dictionary<string, string>> Searchs(params object[] objs)
	{
		if(objs.Length < 2 || objs.Length % 2 != 0)
		{
			//GUtilLog.LogError("参数传递不正确, 参数必须成对出现");
			return null;
		}
		StringBuilder key = new StringBuilder ();
		for(int index = 0; index < objs.Length; index++)
		{
			/// 检测字段有没有效
			if(index % 2 == 0)
			{
				/// 如果不存在
				if(!_key.ContainsKey(objs[index].ToString()))
				{
					Debug.LogError("参数中出现的未找到的字段 " + objs[index].ToString());
					return null;
				}
			}
			key.Append(objs[index]);
			key.Append(split);
		}
        /// WILL DEL
        //if (objs[1].ToString() == "PlayerExSS")
        //    UtilLog.LogError("UtilCsvRead >> " + key);
		/// 检测缓存
		if(_searchs.ContainsKey(key.ToString()))
		{
			return _searchs[key.ToString()];
		}
		List<Dictionary<string, string>> result = null;
		
		/// 2014.07.28 我搜索到的内容信息
		List<int> searchIndex = null;
		/// 缓存不存在,搜索吧
		for(int i = 0; i < _keyValue.Count; i++)
		{
			bool isSearch = true;
			/// 检测数据
			for(int j = 0; j < objs.Length; j += 2)
			{
				/// 索引
				int index = _key[objs[j].ToString()];
				if(null == _keyValue[i] || _keyValue[i].Length <= index)
				{
					isSearch = false;
					break;
				}
				if(_keyValue[i][index] != objs[j + 1].ToString())
				{
                    isSearch = false;
					break;
				}
			}
			/// 如果找到了
			if(isSearch)
			{
                
                Dictionary<string, string> v = new Dictionary<string, string>();
				foreach(string k in _key.Keys)
				{
					/// 越界了不追究
					if(_keyValue[i].Length <= _key[k])
						continue;
                    /// 装在数据
                    /// 优化内存
                    //v.Add(string.Intern(k), string.Intern(_keyValue[i][_key[k]]));
                    v.Add(string.Intern(k), _keyValue[i][_key[k]]);
                }
				if(null == result)
					result = new List<Dictionary<string, string>>();
				result.Add(v);
                /// 2014.07.28 把我存的东西缓存出来
                if (isClearData)
				{
					if(null == searchIndex)
						searchIndex = new List<int>();
					searchIndex.Add(i);
				}
				
			}
		}
		
		/// 2014.07.28 清除掉没用的表格信息内容
		if(null != searchIndex)
		{
			for(int i = searchIndex.Count - 1; i >= 0; i--)
			{
				_keyValue.RemoveAt(searchIndex[i]);
			}
		}
		/// 装载缓存
		_searchs.Add(key.ToString(), result);
		return result;
	}
	/// 寻找1行数据
	public Dictionary<string, string> Search(params object[] objs)
	{
		/// 效率优化
		if(objs.Length < 2 || objs.Length % 2 != 0)
		{
			//GUtilLog.LogError("参数传递不正确, 参数必须成对出现");
			return null;
		}
		StringBuilder key = new StringBuilder();
		for(int index = 0; index < objs.Length; index++)
		{
			/// 检测字段有没有效
			if(index % 2 == 0)
			{
				/// 如果不存在
				if(!_key.ContainsKey(objs[index].ToString()))
				{
					Debug.LogError("参数中出现的未找到的字段 " + objs[index].ToString());
					return null;
				}
			}
			key.Append(objs[index]);
			key.Append(split);
		}
		/// 检测缓存
		if(_searchs.ContainsKey(key.ToString()))
		{
			if(null == _searchs[key.ToString()] || _searchs[key.ToString()].Count <= 0)
				return null;
			return _searchs[key.ToString()][0];
		}
		key.Append("searchOne");
		/// 检测单独的缓存
		if(_searchs.ContainsKey(key.ToString()))
		{
			if(null == _searchs[key.ToString()] || _searchs[key.ToString()].Count <= 0)
				return null;
			return _searchs[key.ToString()][0];
		}
		
		Dictionary<string, string> result = null;
		/// 缓存不存在,搜索吧
		for(int i = 0; i < _keyValue.Count; i++)
		{
			bool isSearch = true;
			/// 检测数据
			for(int j = 0; j < objs.Length; j += 2)
			{
				/// 索引
				int index = _key[objs[j].ToString()];
				if(_keyValue[i].Length <= index)
				{
					isSearch = false;
					break;
				}
				
				if(_keyValue[i][index] != objs[j + 1].ToString())
				{
					isSearch = false;
					break;
				}
			}
			/// 如果找到了
			if(isSearch)
			{
				result = new Dictionary<string, string>();
				foreach(string k in _key.Keys)
				{
					/// 越界了不追究
					if(_keyValue[i].Length <= _key[k])
						continue;
                    /// 优化内存
                    //result.Add(string.Intern(k), string.Intern(_keyValue[i][_key[k]]));
                    result.Add(string.Intern(k), _keyValue[i][_key[k]]);
                }
				
				/// 2014.07.28 清除掉没用的表格信息内容
				if(isClearData)
				{
					_keyValue.RemoveAt(i);
				}
				/// 如果寻到了返回
				break;
			}
		}
		/// 装载缓存
		List<Dictionary<string, string>> save = null;
		if(null != result)
		{
			save = new List<Dictionary<string, string>>();
			save.Add(result);
		}
		_searchs.Add(key.ToString(), save);
		return result;
	}
	
	/// 直接为差价的变量复制
	public bool SearchAndSet<T>(T val, params object[] objs)
	{
		bool result = false;
		Dictionary<string, string> searchValue = (Dictionary<string, string>)this.GetType().GetMethod("Search").Invoke(this, new object[]{objs});
		
		/// 获得所有字段,为字段复制
		if(null != val && null != searchValue)
		{
			FieldInfo[] fields = val.GetType().GetFields();
			/// 赋值
			foreach(FieldInfo field in fields)
			{
				SetFieldValue(field, val, searchValue);
			}
			result = true;
		}
		return result;
	}
	
	/// 直接为差价的变量复制
	public T SearchAndNew<T>(params object[] objs)
		where T : new()
	{
		T result = default(T);
		Dictionary<string, string> searchValue = (Dictionary<string, string>)this.GetType().GetMethod("Search").Invoke(this, new object[]{objs});
		
		/// 获得所有字段,为字段复制
		if(null != searchValue)
		{
			result = new T();
			FieldInfo[] fields = result.GetType().GetFields();
			/// 赋值
			foreach(FieldInfo field in fields)
			{
				SetFieldValue(field, result, searchValue);
			}
		}
		return result;
	}
	
	/// 直接获得一组数据,直接为差价的变量复制
	public List<T> SearchsT<T>(params object[] objs) 
		where T : new()
	{
		List<T> result = new List<T>();
		List<Dictionary<string, string>> searchValues = (List<Dictionary<string, string>>)this.GetType().GetMethod("Searchs").Invoke(this, new object[]{objs});
		/// 获得所有字段,为字段复制
		if(null != searchValues)
		{
			for(int i = 0; i < searchValues.Count; i++)
			{
				Dictionary<string, string> searchValue = searchValues[i];
				T val = new T();
				FieldInfo[] fields = val.GetType().GetFields();
				/// 赋值
				foreach(FieldInfo field in fields)
				{
					SetFieldValue(field, val, searchValue);
				}
				result.Add(val);
			}
		}
		return result;
	}
	/// 获得所有列的变量
	public T GetAttribue<T>(string attribueName, string attribueValue)
		where T : new()
	{
		
		Dictionary<string, string> searchValue = new Dictionary<string, string>();
		if(!_key.ContainsKey(attribueName) || !_key.ContainsKey(attribueValue))
		{
			return default(T);
		}
		int indexName = _key[attribueName];
		int indexValue = _key[attribueValue];
		for(int index = 0; index < _keyValue.Count; index++)
		{
			if(_keyValue[index].Length <= indexName || _keyValue[index].Length <= indexValue)
				continue;
			if(searchValue.ContainsKey(_keyValue[index][indexName]))
				continue;
			searchValue.Add(_keyValue[index][indexName], _keyValue[index][indexValue]);
		}
		/// 只为变量进行解析
		T result = new T();
		FieldInfo[] fields = result.GetType().GetFields();
		/// 赋值
		foreach(FieldInfo field in fields)
		{
			SetFieldValue(field, result, searchValue);
		}
		return result;
	}
	/// 获得整个报表
	public List<Dictionary<string, string>> GetTable()
	{
		List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
		/// 缓存不存在,搜索吧
		for(int i = 0; i < _keyValue.Count; i++)
		{
			Dictionary<string, string> v = new Dictionary<string, string>();
			foreach(string k in _key.Keys)
			{
				/// 越界了不追究
				if(_keyValue[i].Length <= _key[k])
					continue;
				/// 装在数据
				v.Add(k, _keyValue[i][_key[k]]);
			}
			result.Add(v);
		}
		return result;
	}
	/// 获得整体表的数据
    /// 此函数从原始string[]中
	public List<T> GetTableAndNew<T>()
		where T : new()
	{
		List<T> result = new List<T>();
		List<Dictionary<string, string>> searchValues = GetTable();

		/// 缓存不存在,搜索吧
		/// 获得所有字段,为字段复制
		if(null != searchValues)
		{
			for(int i =0; i < searchValues.Count; i++)
			{
				Dictionary<string, string> searchValue = searchValues[i];
				try
				{
					T val = new T();
					FieldInfo[] fields = val.GetType().GetFields();
					///逐列 赋值 [这个过程比较慢]
					foreach(FieldInfo field in fields)
					{
						SetFieldValue(field, val, searchValue);
	
					}
					result.Add(val);
				} catch {continue;}
			}
		}
		return result;
	}
	
	
	
	/// 字段复制
	private void SetFieldValue(FieldInfo field, object obj, Dictionary<string, string> hash)
	{
		if(obj == null) return;
		if(field == null) return;
		if(hash == null) return;

        string fieldName = field.Name;
        if (!string.IsNullOrEmpty(fieldNameEndStr) && hash.ContainsKey(fieldName + fieldNameEndStr))
            fieldName = fieldName + fieldNameEndStr;

        if (!hash.ContainsKey(fieldName)) return;
		
		try
		{
			if(field.FieldType == typeof(string[][]))				
			{
				if(hash[fieldName] == null || hash[fieldName] == "" || hash[fieldName] == "#")
				{
					field.SetValue(obj, null);
					return;
				}
				//先看是不是用#作分割
				string[] arrSplit1 = hash[fieldName].Split("#".ToCharArray());			
				if(null != arrSplit1)														
				{
					List<string[]> lstSplit2 = new List<string[]>();
					for(int i = 0;i < arrSplit1.Length;i++)
					{
						//空的就不做考虑
						if(string.IsNullOrEmpty(arrSplit1[i]))continue;
						//再分
						string[] arrTemp = arrSplit1[i].Split("|".ToCharArray());
						lstSplit2.Add(arrTemp);
					}
					if(lstSplit2.Count > 0)
					{
						//二维数组赋给二维数组
						field.SetValue(obj,lstSplit2.ToArray());
					}
				}
				return;
			}
            /// long[][]
			if (field.FieldType == typeof(long[][]))
			{
				if (hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
					field.SetValue(obj, new long[0][]);
				else
				{
					//先看是不是用#作分割
					string[] arrSplit1 = hash[fieldName].Split("#".ToCharArray());
					if (null != arrSplit1)
					{
						List<long[]> lstSplit2 = new List<long[]>();
						for (int i = 0; i < arrSplit1.Length; i++)
						{
							//空的就不做考虑
							if (string.IsNullOrEmpty(arrSplit1[i])) continue;
							//再分
							string[] arrTemp = arrSplit1[i].Split("|".ToCharArray());
							long[] arrLong = new long[arrTemp.Length];
							for (int j = 0; j < arrTemp.Length; j++)
								long.TryParse(arrTemp[j], out arrLong[j]);
							lstSplit2.Add(arrLong);
						}
						if (lstSplit2.Count > 0)
						{
							//二维数组赋给二维数组
							field.SetValue(obj, lstSplit2.ToArray());
						}
					}
				}
				return;
			}
            /// float[][]
			if (field.FieldType == typeof(float[][]))
			{
				if (hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
					field.SetValue(obj, new float[0][]);
				else
				{
					//先看是不是用#作分割
					string[] arrSplit1 = hash[fieldName].Split("#".ToCharArray());
					if (null != arrSplit1)
					{
						List<float[]> lstSplit2 = new List<float[]>();
						for (int i = 0; i < arrSplit1.Length; i++)
						{
							//空的就不做考虑
							if (string.IsNullOrEmpty(arrSplit1[i])) continue;
							//再分
							string[] arrTemp = arrSplit1[i].Split("|".ToCharArray());
							float[] arrLong = new float[arrTemp.Length];
							for (int j = 0; j < arrTemp.Length; j++)
								float.TryParse(arrTemp[j], out arrLong[j]);
							lstSplit2.Add(arrLong);
						}
						if (lstSplit2.Count > 0)
						{
							//二维数组赋给二维数组
							field.SetValue(obj, lstSplit2.ToArray());
						}
					}
				}
				return;
			}
			/// long[] 获得数组
			if (field.FieldType == typeof(long[]))
			{
				if (hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
					field.SetValue(obj, new long[0]);
				else
				{
					string[] valS = hash[fieldName].Split("|".ToCharArray());
					long[] valL = new long[valS.Length];
					for(int i = 0; i < valS.Length; i++)
					{
						if (!string.IsNullOrEmpty(valS[i]))
							valL[i] = long.Parse(valS[i]);
					}
					field.SetValue(obj, valL);
				}
				return;
			}
			/// float[] 获得数组
			if (field.FieldType == typeof(float[]))
			{
				if (hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
					field.SetValue(obj, new float[0]);
				else
				{
					string[] valS = hash[fieldName].Split("|".ToCharArray());
					float[] valL = new float[valS.Length];
					for (int i = 0; i < valS.Length; i++)
						float.TryParse(valS[i], out valL[i]);
					field.SetValue(obj, valL);
				}
				return;
			}

			//如果不是二维数组，直接存起来
			if (field.FieldType == typeof(string[]))
            {
                if (hash[fieldName] == null || hash[fieldName] == "" || hash[fieldName] == "#")
                    field.SetValue(obj, null);
                else
                    field.SetValue(obj, hash[fieldName].Split("|".ToCharArray()));
                return;
            }

            if (field.FieldType == typeof(bool))
			{
				field.SetValue(obj, hash[fieldName] == "1");
				return;
			}
            if (field.FieldType == typeof(long))
            {
                if (hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
                    field.SetValue(obj, (long)0);
                else
                    field.SetValue(obj, long.Parse(hash[fieldName]));
                return;
            }
			if(field.FieldType == typeof(ulong))
			{
				if(hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
					field.SetValue(obj, (ulong)0);
				else
					field.SetValue(obj, ulong.Parse(hash[fieldName]));
				return;
			}
            if (field.FieldType == typeof(int))
            {
                if (hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
                    field.SetValue(obj, 0);
                else
                    field.SetValue(obj, int.Parse(hash[fieldName]));
                return;
            }
			if(field.FieldType == typeof(float))
			{
				if(hash[fieldName] == "#" || string.IsNullOrEmpty(hash[fieldName]))
					field.SetValue(obj, 0f);
				else
					field.SetValue(obj, float.Parse(hash[fieldName]));
				return;
			}
			/// 优化内存
			field.SetValue(obj, string.Intern(hash[fieldName])); //.Replace("\\n", "\n"
        } catch {
			//Debug.LogError(se.ToString());
			if(hash.ContainsKey("ID"))
                UtilLog.LogWarning(string.Format("UtilCsvReader Set Value Error : {0}.{1} = {2}, ID = {3}", obj.GetType().Name, fieldName, hash[fieldName], hash["ID"]));
            else
                UtilLog.LogWarning(string.Format("UtilCsvReader Set Value Error : {0}.{1} = {2}", obj.GetType().Name, fieldName, hash[fieldName]));
		}
	}
	/// 销毁
	public void Destroy()
	{
		_key.Clear();
		_keyValue.Clear();
		_searchs.Clear();
		_text = null;
        //GameObject.Destroy(this);
	}
}
