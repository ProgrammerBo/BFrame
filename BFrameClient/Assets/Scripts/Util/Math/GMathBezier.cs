using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GMathBezier
{
	/// 数据原型
	public class BezierData : System.Object
	{
		public float t;
		public Vector3 result;
		public List<Vector3> pointArr;
		public List<Vector3[]> pointPCArr;
		public List<float> tArr;

		/// 计算平面距离
		public float distanceXZ
		{
			get
			{
				float distance = 0f;
				for(int index = 0; index < pointArr.Count - 1; index++)
				{
					Vector3 temp = pointArr[index] - pointArr[index + 1];
					distance = Mathf.Sqrt(temp.x * temp.x + temp.z * temp.z);
				}
				return distance;
			}
		}

		public void Destroy()
		{
			pointArr.Clear();
			pointPCArr.Clear();
			tArr.Clear();
		}
	}
	public static BezierData GetBezier(List<Vector3> sPoints, List<float> sTArr, float sT)
	{
		BezierData bezierData = new BezierData();
		bezierData.pointArr = sPoints;
		/// 分割锚点
		MathCPPoint(bezierData);
		/// 分割时间
		bezierData.tArr = sTArr;
		if(sTArr == null)
			MathT(bezierData);
		
		/// 真正计算值的地方
		return GetBezier(bezierData, sT);
	}
	/// 计算各种锚点数据，然后进行数据缓存
	public static BezierData GetBezier(List<Vector3> sPoints, float sT)
	{
		BezierData bezierData = new BezierData();
		bezierData.pointArr = sPoints;
		/// 分割锚点
		MathCPPoint(bezierData);
		/// 分割时间
		MathT(bezierData);
		
		/// 真正计算值的地方
		return GetBezier(bezierData, sT);
	}
	
	
	/// 所有点已经缓存好，可以进行高效计算。血省啊
	public static BezierData GetBezier(BezierData sBezierData, float sT)
	{
		int index = 0;
		/// 找出当前相近的时间
		for(index = 0; index < sBezierData.tArr.Count - 1; index ++)
		{
			if(sBezierData.tArr[index] > sT)
				break;
		}
		float t = (sT - (index == 0 ? 0f : sBezierData.tArr[index - 1])) / (sBezierData.tArr[index] - (index == 0 ? 0f : sBezierData.tArr[index - 1]));
		/// 计算出贝塞尔曲线
		sBezierData.result = MathBezier(sBezierData.pointArr[index], sBezierData.pointPCArr[index][0], sBezierData.pointPCArr[index][1], sBezierData.pointArr[index + 1], t);
        sBezierData.t = sT;
        return sBezierData;
	}
	
	
	/// 分割锚点
	private static void MathCPPoint(BezierData sBezierData)
	{
		sBezierData.pointPCArr = new List<Vector3[]>();
		/// 分割锚点
		for(int index = 0; index < sBezierData.pointArr.Count - 1; index++)
		{
			sBezierData.pointPCArr.Add(new Vector3[]{Vector3.zero, Vector3.zero});
			Vector3 pointA = Vector3.zero;
			Vector3 pointB = Vector3.zero;
			Vector3 pointC = Vector3.zero;
			
			/// 计算 P2点
			pointA = (sBezierData.pointArr[index] + sBezierData.pointArr[index + 1]) * 0.5f;
			pointB = (sBezierData.pointArr[index] + sBezierData.pointArr[Mathf.Abs(index - 1)]) * 0.5f;
			pointC = (pointA + pointB) * 0.5f;
			sBezierData.pointPCArr[index][0] = (sBezierData.pointArr[index] - pointC) + pointA;
			
			/// 计算P3点
			pointB = (sBezierData.pointArr[index + 1] + sBezierData.pointArr[Mathf.Min(index + 2, sBezierData.pointArr.Count - 1)]) * 0.5f;
			pointC = (pointA + pointB) * 0.5f;
			sBezierData.pointPCArr[index][1] = (sBezierData.pointArr[index + 1] - pointC) + pointA;
		}
	}
	/// 分割时间
	private static void MathT(BezierData sBezierData)
	{
		sBezierData.tArr = new List<float>();
		float disTotal = 0f;
		float disAdd = 0f;
		for(int index = 0; index < sBezierData.pointArr.Count - 1; index++)
		{
			disTotal += Vector3.Distance(sBezierData.pointArr[index], sBezierData.pointArr[index + 1]);
		}
		for(int index = 0; index < sBezierData.pointArr.Count - 1; index++)
		{
			disAdd += Vector3.Distance(sBezierData.pointArr[index], sBezierData.pointArr[index + 1]);
			sBezierData.tArr.Add(disAdd / disTotal);
		}
	}
	/// 计算3次贝塞尔曲线
	private static Vector3 MathBezier(Vector3 sA, Vector3 sB, Vector3 sC, Vector3 sD, float sT)
	{
		sT = Mathf.Max(0f, sT);
		sT = Mathf.Min(1f, sT);
		Vector3 result = Vector3.zero;
		
		result.x = sA.x * Mathf.Pow(1f - sT, 3f) + 3 * sB.x * sT * Mathf.Pow(1f - sT, 2f) + 3 * sC.x * sT * sT * (1 - sT) + sD.x * sT * sT * sT;
		result.y = sA.y * Mathf.Pow(1f - sT, 3f) + 3 * sB.y * sT * Mathf.Pow(1f - sT, 2f) + 3 * sC.y * sT * sT * (1 - sT) + sD.y * sT * sT * sT;
		result.z = sA.z * Mathf.Pow(1f - sT, 3f) + 3 * sB.z * sT * Mathf.Pow(1f - sT, 2f) + 3 * sC.z * sT * sT * (1 - sT) + sD.z * sT * sT * sT;
		
		return result;
	}
}
