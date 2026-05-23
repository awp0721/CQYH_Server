using System.Collections.Generic;

namespace 游戏服务器.地图类
{
	public class NPCActions
	{
		public ActionType Type;

		public List<string> Params = new List<string>();

		public 类数据读写 F;

		public NPCActions(ActionType action, params string[] p)
		{
			this.Type = action;
			this.Params.AddRange(p);
		}
	}
}
