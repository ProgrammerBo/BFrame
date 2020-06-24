using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CSV
{
	public class CSVInfo
	{
		public static readonly string[] CLASS_NAME = { "Adviser", "Buff", "Constant", "Event", "Map", "Skill", "Trigger" };
		public static readonly System.Type[] CLASS_TYPE = { typeof(CSV.Adviser), typeof(CSV.Buff), typeof(CSV.Constant), typeof(CSV.Event), typeof(CSV.Map), typeof(CSV.Skill), typeof(CSV.Trigger) };
	}
	public class Adviser : System.Object
	{
		/// ID
		public long ID;
		/// 名字
		public string Name;
		/// 产出
		public int Output;
	}
	public class Buff : System.Object
	{
		/// ID
		public long ID;
		/// 类型(1Rx/2有症状/3确诊/4重症/5重症死亡/6重症治愈/7民众不满)
		public int Type;
		/// "运算(1+
		public int Arithmetic;
		/// 2-
		public int Value;
		/// 3*)"
		public int Day;
	}
	public class Constant : System.Object
	{
		/// 常量定义
		public string ID;
		/// 数值
		public int Value;
		/// 注释
		public string Notes;
	}
	public class Event : System.Object
	{
		/// ID
		public long ID;
		/// 类型(0无效/1固定/2选择/3随机)
		public int Type;
		/// 触发Buff
		public long[] Buff;
		/// 分支事件ID|分支事件ID
		public long[] BranchID;
		/// 按钮文本
		public string[] Button;
		/// 事件标题
		public string Title;
		/// 事件内容
		public string Desc;
	}
	public class Map : System.Object
	{
		/// ID
		public long ID;
		/// 名字
		public string Name;
	}
	public class Skill : System.Object
	{
		/// ID
		public long ID;
		/// 前置ID
		public long PreID;
		/// 触发buff
		public long Buff;
		/// 解锁花费
		public int Cost;
		/// 描述标题|内容
		public string[] Desc;
	}
	public class Trigger : System.Object
	{
		/// ID
		public long ID;
		/// 类型（1天）
		public int Type;
		/// 参数
		public int Args;
		/// 权重
		public int Weight;
	}
}
