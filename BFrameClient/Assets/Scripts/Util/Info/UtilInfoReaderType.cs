using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Info
{
	public class InfoInfo
	{
		public static readonly string[] CLASS_NAME = { "Battle" };
		public static readonly System.Type[] CLASS_TYPE = { typeof(Info.Battle) };
	}
	public class Battle : System.Object
	{
		/// 费用 最多
		public int COST_MAX;
		/// 费用 初始化 上限
		public int COST_MAX_INIT;
		/// 费用 初始化
		public int COST_INIT;
		/// 费用 从初始增长到上限 每个的时间/秒
		public int COST_TIME_GROW;
		/// 费用 恢复时间/秒
		public int COST_TIME_RECOVER;
		/// 丢弃时间/秒
		public int DISCARD_TIME;
	}
}
