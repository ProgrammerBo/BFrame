using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GMath
{
	/// 转换成小树
	public static float toFloat(object val)
	{
		float result = 0f;
		try
		{
			result = float.Parse(val.ToString());
		} catch {}
		return result;
	}
	/// 转换成整数
	public static int toInt(object val)
	{
		int result = 0;
		try
		{
			result = (int)double.Parse(val.ToString());
		} catch {}
		return result;
	}
	/// 查找索引
	public static int indexOf<T>(T val, T[] array)
	{
		if(null == array)
			return -1;
		for(int index = 0; index < array.Length; index++)
		{
			
			if(object.Equals(val, array[index]))
				return index;
		}
		return -1;
	}
	/// 长度
	public static float distance(float x, float y)
	{
		return Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f));
	}
	public static float distanceXZ(Vector3 vec)
	{
		return Mathf.Sqrt(Mathf.Pow(vec.x, 2f) + Mathf.Pow(vec.z, 2f));
	}
	

	// add by ssy
	public static int[] stringToIntArray(string[] val)
	{
		if(null == val || val.Length <= 0)
		{
			return null;
		}
		
		List<int> ret = new List<int> ();
		foreach(string str in val)
		{
			ret.Add(toInt(str));
		}
		
		return ret.ToArray();
	}
	/// 取最大
	public static double maxDouble(double a, double b)
	{
		if(a > b)
			return a;
		return b;
	}
	
	// add end
	
	
	public static int RoundToInt(float f)
	{
		return (f - (int)f-0.5f >= 0 ? (int)f + 1 : (int)f);
	}
}
