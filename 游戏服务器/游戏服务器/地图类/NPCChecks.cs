using System.Collections.Generic;

namespace 游戏服务器.地图类
{
	public class NPCChecks
	{
		public CheckType Type;

		public bool Not;

		public List<string> Params = new List<string>();

		public 类数据读写 F;

		public NPCChecks(bool notMode, CheckType check, params string[] p)
		{
			this.Type = check;
			this.Not = notMode;
			for (int i = 0; i < p.Length; i++)
			{
				this.Params.Add(p[i]);
			}
		}
	}
}
