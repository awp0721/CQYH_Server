using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using 游戏服务器.网络类;

namespace 游戏服务器.数据类
{
	public sealed class 队伍数据 : 游戏数据
	{
		public readonly 数据监视器<byte> 分配方式;

		public readonly 数据监视器<角色数据> 队伍队长;

		public readonly 哈希监视器<角色数据> 队伍成员;

		public Dictionary<角色数据, DateTime> 申请列表;

		public Dictionary<角色数据, DateTime> 邀请列表;

		public Dictionary<物品数据, int> 拍卖列表;

		public Dictionary<int, Dictionary<角色数据, 拍卖详情>> 拍卖参与列表;

		public 物品数据 当前拍卖物品;

		public int 当前拍卖价格;

		public 角色数据 当前拍卖角色;

		public int 拍卖序号;

		public DateTime 处理间隔;

		public int 队伍编号 => base.数据索引.V;

		public int 队长编号 => this.队伍队长.V.数据索引.V;

		public int 队员数量 => this.队伍成员.Count;

		public byte 拾取方式 => this.分配方式.V;

		public string 队长名字 => this.队长数据.角色名字.V;

		public 角色数据 队长数据
		{
			get
			{
				return this.队伍队长.V;
			}
			set
			{
				if (this.队伍队长.V.数据索引.V != value.数据索引.V)
				{
					this.队伍队长.V = value;
				}
			}
		}

		public override string ToString()
		{
			return this.队长数据?.角色名字?.V;
		}

		public 队伍数据()
		{
			this.拍卖列表 = new Dictionary<物品数据, int>();
			this.申请列表 = new Dictionary<角色数据, DateTime>();
			this.邀请列表 = new Dictionary<角色数据, DateTime>();
			this.拍卖参与列表 = new Dictionary<int, Dictionary<角色数据, 拍卖详情>>();
		}

		public 队伍数据(角色数据 创建角色, byte 分配方式)
		{
			this.拍卖列表 = new Dictionary<物品数据, int>();
			this.申请列表 = new Dictionary<角色数据, DateTime>();
			this.邀请列表 = new Dictionary<角色数据, DateTime>();
			this.拍卖参与列表 = new Dictionary<int, Dictionary<角色数据, 拍卖详情>>();
			this.分配方式.V = 分配方式;
			this.队伍队长.V = 创建角色;
			this.队伍成员.Add(创建角色);
			游戏数据网关.队伍数据表.添加数据(this, 分配索引: true);
		}

		public override void 删除数据()
		{
			foreach (角色数据 item in this.队伍成员)
			{
				item.当前队伍 = null;
			}
			base.删除数据();
		}

		public void 发送封包(游戏封包 P, 角色数据 自身 = null)
		{
			foreach (角色数据 item in this.队伍成员)
			{
				if (item != 自身)
				{
					item.网络连接?.发送封包(P);
				}
			}
		}

		public void 发送同图封包(游戏封包 P, 角色数据 自身 = null)
		{
			foreach (角色数据 item in this.队伍成员)
			{
				if (item != 自身 && item.当前地图.V == 自身.当前地图.V)
				{
					item.网络连接?.发送封包(P);
				}
			}
		}

		public byte[] 队伍描述()
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(base.数据索引.V);
			binaryWriter.Write(this.队长数据.名字描述());
			binaryWriter.Seek(36, SeekOrigin.Begin);
			binaryWriter.Write(this.拾取方式);
			binaryWriter.Write(this.队长编号);
			binaryWriter.Write(11);
			binaryWriter.Write((ushort)this.队伍成员.Count);
			binaryWriter.Write(0);
			foreach (角色数据 item in this.队伍成员)
			{
				binaryWriter.Write(this.队友描述(item));
			}
			return memoryStream.ToArray();
		}

		public byte[] 队友描述(角色数据 队友)
		{
			using MemoryStream memoryStream = new MemoryStream(new byte[39]);
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(队友.数据索引.V);
			binaryWriter.Write(队友.名字描述());
			binaryWriter.Seek(36, SeekOrigin.Begin);
			binaryWriter.Write((byte)队友.角色性别.V);
			binaryWriter.Write((byte)队友.角色职业.V);
			binaryWriter.Write((byte)((队友.网络连接 == null) ? 3u : 0u));
			return memoryStream.ToArray();
		}

		public void 拍卖分配物品(角色数据 角色数据, int 拍卖序号)
		{
			List<物品数据> list;
			list = (from q in this.拍卖列表
				where ((KeyValuePair<物品数据, int>)q).Value == 拍卖序号
				select ((KeyValuePair<物品数据, int>)q).Key).ToList();
			if (list.Count != 1 || this.当前拍卖物品 == null)
			{
				return;
			}
			if (list[0] != this.当前拍卖物品)
			{
				this.队长数据.发送邮件(null, "组队竞拍失败", "物品组队流拍了，这个物品属于队长，请领取", list[0]);
				this.拍卖列表.Remove(list[0]);
				this.拍卖参与列表.Remove(拍卖序号);
				this.发送封包(new 结束组队拍卖
				{
					拍卖顺序 = 拍卖序号,
					对象编号 = this.队长数据.角色编号
				});
				this.拍卖下件物品();
				return;
			}
			if (this.当前拍卖角色 == null)
			{
				this.队长数据.发送邮件(null, "组队竞拍失败", "物品组队流拍了，这个物品属于队长，请领取", list[0]);
			}
			else
			{
				if (!this.拍卖参与列表[拍卖序号].ContainsKey(this.当前拍卖角色))
				{
					this.当前拍卖角色 = null;
					return;
				}
				int 出价;
				出价 = this.拍卖参与列表[拍卖序号][this.当前拍卖角色].出价;
				this.当前拍卖角色.发送邮件(null, "组队竞拍成功", $"组队竞拍成功，您花费了[{出价}]金币拍得以下物品，请注意查收。", this.当前拍卖物品);
				if (this.拍卖参与列表[拍卖序号].Count > 1)
				{
					int 数量;
					数量 = (int)((float)出价 * 0.95f / (float)(this.拍卖参与列表[拍卖序号].Count - 1));
					foreach (KeyValuePair<角色数据, 拍卖详情> item in this.拍卖参与列表[拍卖序号])
					{
						if (item.Key != this.当前拍卖角色)
						{
							item.Key.发送邮件(null, "组队竞拍分钱", "组队物品被成功拍卖，这是你的分红", 1, 数量);
						}
					}
				}
			}
			this.拍卖列表.Remove(this.当前拍卖物品);
			this.拍卖参与列表.Remove(拍卖序号);
			this.当前拍卖物品 = null;
			this.当前拍卖价格 = 0;
			this.当前拍卖角色 = null;
			this.发送封包(new 结束组队拍卖
			{
				拍卖顺序 = 拍卖序号,
				对象编号 = ((this.当前拍卖角色 == null) ? this.队长数据 : this.当前拍卖角色).角色编号
			});
			this.拍卖下件物品();
		}

		public void 拍卖下件物品()
		{
			if (this.当前拍卖物品 == null && this.拍卖列表.Count > 0)
			{
				this.当前拍卖物品 = this.拍卖列表.First().Key;
				this.当前拍卖物品.生成时间.V = 主程.当前时间.AddMinutes(2.0);
				this.发送封包(new 开始拍卖物品
				{
					拍卖顺序 = this.拍卖列表[this.当前拍卖物品],
					起拍价格 = 5000,
					参与人数 = this.拍卖参与列表[this.拍卖列表[this.当前拍卖物品]].Count
				});
			}
		}

		public void 竞价拍卖(角色数据 角色数据, int 拍卖序号, int 当前竞价)
		{
			// C04: 只允许对当前活动件序号出价. 原实现用全局 当前拍卖价格/当前拍卖角色 比价改写,
			// 却不校验 拍卖序号 属于活动件, 攻击者可对非活动序号越序出价劫持他人中标物品并打乱退款记账.
			if (this.当前拍卖物品 == null || !this.拍卖列表.TryGetValue(this.当前拍卖物品, out var 活动序号) || 活动序号 != 拍卖序号)
			{
				return;
			}
			if (5000 <= 当前竞价 && 角色数据.金币数量 >= 当前竞价 && 当前竞价 > this.当前拍卖价格 && this.拍卖参与列表.ContainsKey(拍卖序号) && this.拍卖参与列表[拍卖序号].ContainsKey(角色数据))
			{
				if (this.当前拍卖角色 != null && !this.拍卖参与列表[拍卖序号][this.当前拍卖角色].放弃 && this.拍卖参与列表[拍卖序号][this.当前拍卖角色].出价 > 0)
				{
					this.当前拍卖角色.发送邮件(null, "组队竞拍退钱", $"玩家[{角色数据.角色名字.V}]花费了[{当前竞价}]金币成功竞价，这是您出价花费的金币，现已退还", 1, this.拍卖参与列表[拍卖序号][this.当前拍卖角色].出价);
					this.拍卖参与列表[拍卖序号][this.当前拍卖角色].出价 = 0;
				}
				角色数据.网络连接.绑定角色.扣金币((uint)当前竞价);
				this.当前拍卖价格 = 当前竞价;
				this.当前拍卖角色 = 角色数据;
				this.拍卖参与列表[拍卖序号][角色数据].出价 += 当前竞价;
				this.发送封包(new 通知组队拍卖
				{
					拍卖顺序 = 拍卖序号,
					对象编号 = 角色数据.角色编号,
					当前价格 = 当前竞价,
					重置时间 = 120000
				});
				this.当前拍卖物品.生成时间.V = 主程.当前时间.AddMinutes(2.0);
				if (this.拍卖参与列表[拍卖序号].Where((KeyValuePair<角色数据, 拍卖详情> O) => !O.Value.放弃).Count() == 1)
				{
					this.拍卖分配物品(null, 拍卖序号);
				}
			}
		}

		public void 放弃拍卖(角色数据 角色数据, int 拍卖序号)
		{
			if (!this.拍卖参与列表.ContainsKey(拍卖序号) || !this.拍卖参与列表[拍卖序号].ContainsKey(角色数据))
			{
				return;
			}
			if (this.拍卖参与列表[拍卖序号][角色数据].出价 > 0)
			{
				角色数据.发送邮件(null, "组队竞拍退钱", "放弃了组队物品，这是你出价花费的金币", 1, this.拍卖参与列表[拍卖序号][角色数据].出价);
				this.拍卖参与列表[拍卖序号][角色数据].出价 = 0;
			}
			this.拍卖参与列表[拍卖序号][角色数据].放弃 = true;
			if (this.当前拍卖角色 == 角色数据)
			{
				this.当前拍卖角色 = null;
				KeyValuePair<角色数据, 拍卖详情> keyValuePair;
				keyValuePair = this.拍卖参与列表[拍卖序号].Where((KeyValuePair<角色数据, 拍卖详情> O) => !O.Value.放弃).MaxBy((KeyValuePair<角色数据, 拍卖详情> O) => O.Value.出价);
				// D01: MaxBy 在无未放弃参与者时返回 default(Value==null), 原 keyValuePair.Value.出价 抛 NRE (远程可达).
				this.当前拍卖价格 = keyValuePair.Value?.出价 ?? 0;
				this.发送封包(new 通知组队拍卖
				{
					拍卖顺序 = 拍卖序号,
					对象编号 = ((keyValuePair.Value != null && keyValuePair.Value.出价 > 0) ? keyValuePair.Key.角色编号 : 0),
					当前价格 = keyValuePair.Value?.出价 ?? 0,
					重置时间 = 120000
				});
				this.当前拍卖物品.生成时间.V = 主程.当前时间.AddMinutes(2.0);
			}
			角色数据.网络连接?.发送封包(new 放弃组队拍卖
			{
				拍卖顺序 = 拍卖序号,
				对象编号 = 角色数据.角色编号
			});
			int num;
			num = this.拍卖参与列表[拍卖序号].Where((KeyValuePair<角色数据, 拍卖详情> O) => !O.Value.放弃).Count();
			if (num == 0 || (this.当前拍卖角色 != null && num == 1))
			{
				this.拍卖分配物品(角色数据, 拍卖序号);
			}
		}

		public void 放弃所有拍卖(角色数据 角色数据)
		{
			// D01: 快照键迭代 + TryGetValue 重取 + MaxBy 空守卫. 原 foreach 直接枚举活字典, 而循环体内
			// 拍卖分配物品 会 Remove 同一字典 -> 集合已修改异常; 且 MaxBy 在无未放弃参与者时返回 default(Value==null) -> NRE.
			// 普通账号自建拍卖队 + 自踢/换图即可远程触发, 经主循环线程路径会跳过整 tick.
			foreach (int 序号 in this.拍卖参与列表.Keys.ToList())
			{
				if (!this.拍卖参与列表.TryGetValue(序号, out var 参与字典) || !参与字典.TryGetValue(角色数据, out var value))
				{
					continue;
				}
				if (value.出价 > 0)
				{
					角色数据.发送邮件(null, "组队竞拍退钱", "放弃了组队物品，这是你出价花费的金币", 1, value.出价);
					value.出价 = 0;
				}
				value.放弃 = true;
				if (this.拍卖序号 == 序号 && this.当前拍卖角色 == 角色数据)
				{
					this.当前拍卖角色 = null;
					KeyValuePair<角色数据, 拍卖详情> keyValuePair;
					keyValuePair = 参与字典.Where((KeyValuePair<角色数据, 拍卖详情> O) => !O.Value.放弃).MaxBy((KeyValuePair<角色数据, 拍卖详情> O) => O.Value.出价);
					this.当前拍卖价格 = keyValuePair.Value?.出价 ?? 0;
					this.发送封包(new 通知组队拍卖
					{
						拍卖顺序 = this.拍卖序号,
						对象编号 = ((keyValuePair.Value != null && keyValuePair.Value.出价 > 0) ? keyValuePair.Key.角色编号 : 0),
						当前价格 = keyValuePair.Value?.出价 ?? 0,
						重置时间 = 120000
					});
					this.当前拍卖物品.生成时间.V = 主程.当前时间.AddMinutes(2.0);
				}
				角色数据.网络连接?.发送封包(new 放弃组队拍卖
				{
					拍卖顺序 = 序号,
					对象编号 = 角色数据.角色编号
				});
				int num;
				num = 参与字典.Where((KeyValuePair<角色数据, 拍卖详情> O) => !O.Value.放弃).Count();
				if (num == 0 || (this.当前拍卖角色 != null && num == 1))
				{
					this.拍卖分配物品(角色数据, 序号);
				}
			}
		}

		public void 添加拍卖物品(物品数据 拍卖物品, 角色数据 拾取人物)
		{
			this.拍卖序号++;
			this.拍卖列表.Add(拍卖物品, this.拍卖序号);
			if (!this.拍卖参与列表.ContainsKey(this.拍卖序号))
			{
				this.拍卖参与列表[this.拍卖序号] = new Dictionary<角色数据, 拍卖详情>();
			}
			else
			{
				this.拍卖参与列表[this.拍卖序号].Clear();
			}
			foreach (角色数据 item in this.队伍成员.Where((角色数据 O) => O.网络连接 != null && O.当前地图.V == 拾取人物.当前地图.V).ToList())
			{
				this.拍卖参与列表[this.拍卖序号].Add(item, new 拍卖详情());
				item.网络连接?.发送封包(new 添加拍卖物品
				{
					拍卖顺序 = this.拍卖序号,
					物品描述 = 拍卖物品.字节描述()
				});
			}
		}

		public void 处理数据()
		{
			if (主程.当前时间 < this.处理间隔)
			{
				return;
			}
			this.处理间隔 = 主程.当前时间.AddMilliseconds(1000.0);
			if (this.拍卖列表.Count > 0)
			{
				if (this.当前拍卖物品 == null)
				{
					this.当前拍卖物品 = this.拍卖列表.First().Key;
					this.当前拍卖物品.生成时间.V = 主程.当前时间.AddMinutes(2.0);
					this.发送封包(new 开始拍卖物品
					{
						拍卖顺序 = this.拍卖列表[this.当前拍卖物品],
						起拍价格 = 5000,
						参与人数 = this.拍卖参与列表[this.拍卖列表[this.当前拍卖物品]].Count
					});
				}
				else if (this.当前拍卖物品.生成时间.V < 主程.当前时间)
				{
					this.拍卖分配物品(this.当前拍卖角色, this.拍卖列表[this.当前拍卖物品]);
				}
			}
		}
	}
}
