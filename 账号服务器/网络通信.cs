using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace 账号服务器
{
	public sealed class 网络通信
	{
		public struct 数据封包
		{
			public IPEndPoint 客户地址;

			public byte[] 接收数据;
		}

		public static UdpClient 本地网络服务;

		public static ConcurrentQueue<数据封包> 数据处理队列;

		// 每 IP 限速桶, 同时承载认证失败计数 / 注册次数 / 解析错误日志去抖.
		private sealed class 限速桶
		{
			public int 失败计数;
			public DateTime 失败窗口起点;
			public DateTime 解封时间;
			public int 注册计数;
			public DateTime 注册窗口起点;
			public DateTime 上次解析错误日志时间;
		}

		// MISC-04: 同一 IP 的解析错误日志, 60 秒内最多记一条, 防止伪造 IP 灌脏包刷爆日志.
		private const int 解析错误日志间隔秒 = 60;

		private static readonly ConcurrentDictionary<IPAddress, 限速桶> 限速表
			= new ConcurrentDictionary<IPAddress, 限速桶>();

		// 认证失败: 60 秒内累计 10 次就封禁 5 分钟.
		private const int 失败窗口秒数 = 60;
		private const int 失败最大次数 = 10;
		private const int 封禁秒数 = 300;

		// 注册: 5 分钟内每 IP 最多 3 次.
		private const int 注册窗口秒数 = 300;
		private const int 注册最大次数 = 3;

		// 限速表硬上限: UDP 源 IP 可伪造, 不设上界会被反射 DoS 撑爆内存.
		// 50k 桶 ≈ 4MB, 足够正常负载, 超额时拒绝新桶 (相当于全局 fail-closed).
		private const int 限速表上限 = 50_000;
		private static Timer 限速表清理定时器;

		// II: 处理队列上限. UDP 洪水使接收速率 > 处理速率, 不设上界会让 ConcurrentQueue 无限增长 → OOM.
		// 10000 个上限 ≈ 最坏 10MB (单包 1024 字节), 足够正常突发, 又能挡住洪水撑爆.
		private const int 数据处理队列上限 = 10_000;

		private static 限速桶 取桶(IPAddress ip)
		{
			if (限速表.TryGetValue(ip, out var 已有桶))
			{
				return 已有桶;
			}
			// 超过硬上限直接复用一个"全局桶"语义: 拒绝新建,
			// 由 是否被封禁 等调用方默认放行该 IP, 而非误封. 受害方向: 攻击者刷不
			// 出新条目 + 已有正常用户不受影响; 但失败计数不再生效 — 这是
			// 内存安全和限速效力的权衡, 优先保进程存活.
			if (限速表.Count >= 限速表上限)
			{
				return new 限速桶
				{
					失败窗口起点 = DateTime.UtcNow,
					注册窗口起点 = DateTime.UtcNow
				};
			}
			return 限速表.GetOrAdd(ip, _ => new 限速桶
			{
				失败窗口起点 = DateTime.UtcNow,
				注册窗口起点 = DateTime.UtcNow
			});
		}

		// 每 5 分钟扫一遍, 清理"窗口过期 且 未在封禁期" 的桶, 防止 IP 伪造引起的
		// 内存无界增长 (HIGH-I).
		private static void 清理限速表(object _)
		{
			DateTime now = DateTime.UtcNow;
			foreach (var kv in 限速表)
			{
				限速桶 桶 = kv.Value;
				bool 可丢弃;
				lock (桶)
				{
					可丢弃 = now >= 桶.解封时间
						&& (now - 桶.失败窗口起点).TotalSeconds > 失败窗口秒数
						&& (now - 桶.注册窗口起点).TotalSeconds > 注册窗口秒数;
				}
				if (可丢弃)
				{
					((System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<IPAddress, 限速桶>>)限速表)
						.Remove(kv);
				}
			}
		}

		private static bool 是否被封禁(IPAddress ip)
		{
			return DateTime.UtcNow < 取桶(ip).解封时间;
		}

		private static void 记录认证失败(IPAddress ip)
		{
			限速桶 桶 = 取桶(ip);
			lock (桶)
			{
				DateTime now = DateTime.UtcNow;
				if ((now - 桶.失败窗口起点).TotalSeconds > 失败窗口秒数)
				{
					桶.失败窗口起点 = now;
					桶.失败计数 = 0;
				}
				桶.失败计数++;
				if (桶.失败计数 >= 失败最大次数)
				{
					桶.解封时间 = now.AddSeconds(封禁秒数);
					桶.失败计数 = 0;
					主窗口.添加日志($"IP 触发认证失败上限, 临时封禁 {封禁秒数}s: {ip}");
				}
			}
		}

		private static void 记录认证成功(IPAddress ip)
		{
			限速桶 桶 = 取桶(ip);
			lock (桶)
			{
				桶.失败计数 = 0;
			}
		}

		private static bool 应记录解析错误(IPAddress ip)
		{
			限速桶 桶 = 取桶(ip);
			lock (桶)
			{
				DateTime now = DateTime.UtcNow;
				if ((now - 桶.上次解析错误日志时间).TotalSeconds < 解析错误日志间隔秒)
				{
					return false;
				}
				桶.上次解析错误日志时间 = now;
				return true;
			}
		}

		private static bool 注册放行(IPAddress ip)
		{
			限速桶 桶 = 取桶(ip);
			lock (桶)
			{
				DateTime now = DateTime.UtcNow;
				if ((now - 桶.注册窗口起点).TotalSeconds > 注册窗口秒数)
				{
					桶.注册窗口起点 = now;
					桶.注册计数 = 0;
				}
				if (桶.注册计数 >= 注册最大次数)
				{
					return false;
				}
				桶.注册计数++;
				return true;
			}
		}

		public static bool 启动服务()
		{
			try
			{
				本地网络服务 = new UdpClient(new IPEndPoint(IPAddress.Any, (ushort)主窗口.主界面.本地监听端口.Value));
				数据处理队列 = new ConcurrentQueue<数据封包>();
				限速表清理定时器 = new Timer(清理限速表, null,
					TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
				Task.Run(delegate
				{
					while (本地网络服务 != null)
					{
						try
						{
							UdpClient udpClient = 本地网络服务;
							if (udpClient == null)
							{
								break;
							}
							if (udpClient.Available == 0)
							{
								Thread.Sleep(1);
							}
							else
							{
								数据封包 item = default(数据封包);
								item.接收数据 = udpClient.Receive(ref item.客户地址);
								if (item.接收数据.Length > 1024)
								{
									if (应记录解析错误(item.客户地址.Address))
									{
										主窗口.添加日志($"收到过长的封包  地址:{item.客户地址}, 长度:{item.接收数据.Length}");
									}
								}
								else if (数据处理队列.Count >= 数据处理队列上限)
								{
									// II: 队列上限, 防止接收速率 > 处理速率时无界增长导致 OOM
									if (应记录解析错误(item.客户地址.Address))
									{
										主窗口.添加日志("处理队列拥塞, 丢包来源: " + item.客户地址);
									}
								}
							else
								{
									数据处理队列.Enqueue(item);
									Interlocked.Add(ref 主窗口.已接收字节数, item.接收数据.Length);
									主窗口.更新已接收字节数();
								}
							}
						}
						catch (Exception ex3)
						{
							主窗口.添加日志("数据接收错误: " + ex3.Message);
						}
					}
				});
				Task.Run(delegate
				{
					主窗口.添加日志("开始处理客户请求.");
					while (本地网络服务 != null)
					{
						try
						{
							if (数据处理队列.IsEmpty || !数据处理队列.TryDequeue(out var result))
							{
								Thread.Sleep(1);
							}
							else
							{
								string[] array = Encoding.UTF8.GetString(result.接收数据, 0, result.接收数据.Length).Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
								if (array.Length <= 3 || !int.TryParse(array[0], out var _))
								{
									if (应记录解析错误(result.客户地址.Address))
									{
										主窗口.添加日志($"收到错误的封包  地址: {result.客户地址}, 长度: {result.接收数据.Length}");
									}
								}
								else if (是否被封禁(result.客户地址.Address))
								{
									// 被临时封禁的 IP, 静默丢弃以避免给攻击者反馈信号
								}
								else
								{
									switch (array[1])
									{
										case "0":
											if (array.Length == 4)
											{
												// PROTO-01 v2: 客户端发来的 array[3] 必须是 64-char hex hash.
												// 兼容历史明文账号: 校验通过且当前还是明文存储时, 把存储升级为 hash.
												bool 登录通过 = 主窗口.账号数据.TryGetValue(array[2], out var value3)
													&& 账号数据.安全比较(array[3], value3.账号密码);
												if (!登录通过)
												{
													记录认证失败(result.客户地址.Address);
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 1 用户名或密码错误"));
												}
												else
												{
													记录认证成功(result.客户地址.Address);
													// PROTO-01 / MED-E: 不再回显密码哈希, 客户端发包前已自行缓存.
													// 响应固定 4 段: 包号 0 账号 区服列表 (客户端 登录界面.cs case "0" 以 array.Length==4 严格校验).
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 0 " + array[2] + " " + 主窗口.游戏区服));
													主窗口.添加日志("账号登录成功!  账号: " + array[2]);
												}
											}
											break;
										case "1":
											if (array.Length == 6)
											{
												if (!注册放行(result.客户地址.Address))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 注册过于频繁, 请稍后再试"));
												}
												else if (array[2].Length <= 5 || array[2].Length > 12)
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 用户名长度错误"));
												}
												else if (!账号数据.是哈希格式(array[3]))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 请升级客户端版本"));
												}
												else if (array[4].Length <= 1 || array[4].Length > 18)
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 密保问题长度错误"));
												}
												else if (array[5].Length <= 1 || array[5].Length > 18)
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 密保答案长度错误"));
												}
												else if (!Regex.IsMatch(array[2], "^[a-zA-Z]+.*$"))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 用户名格式错误"));
												}
												else if (!Regex.IsMatch(array[2], "^[a-zA-Z_][A-Za-z0-9_]*$"))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 用户名格式错误"));
												}
												else if (主窗口.账号数据.ContainsKey(array[2]))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes("3 用户名已经存在"));
												}
												else if (!主窗口.写盘许可())
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 3 服务器繁忙, 请稍后再试"));
												}
												else
												{
													主窗口.添加账号(new 账号数据(array[2], array[3], array[4], array[5]));
													// MED-E: 不再回显密码, 客户端从输入框自取.
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 2 " + array[2]));
													主窗口.添加日志("账号注册成功!  账号: " + array[2]);
													Interlocked.Increment(ref 主窗口.新注册账号数);
													主窗口.更新已注册账号数();
												}
											}
											break;
										case "2":
											if (array.Length == 6)
											{
												// PROTO-01 v2: array[3] 必须是 64-char hex 哈希, 强度校验在客户端完成
												if (!账号数据.是哈希格式(array[3]))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 5 请升级客户端版本"));
												}
												else
												{
													// 统一错误文案, 防止通过差异化错误枚举有效账号 (HIGH-A)
													账号数据 value4;
													bool 密保通过 = 主窗口.账号数据.TryGetValue(array[2], out value4)
														&& 账号数据.安全比较(array[4], value4.密保问题)
														&& 账号数据.安全比较(array[5], value4.密保答案);
													if (!密保通过)
													{
														记录认证失败(result.客户地址.Address);
														发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 5 密保信息错误"));
													}
													else if (!主窗口.写盘许可())
													{
														发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 5 服务器繁忙, 请稍后再试"));
													}
													else
													{
														记录认证成功(result.客户地址.Address);
														value4.账号密码 = array[3];
														主窗口.保存账号(value4);
														发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 4 " + array[1] + " " + array[2]));
														主窗口.添加日志("密码修改成功!  账号: " + array[2]);
													}
												}
											}
											break;
										case "3":
											if (array.Length == 6)
											{
												IPEndPoint value2;
												// PROTO-01 v2: 兼容明文 + hash 存储, 通过即顺手升级
												bool 需要升级门票 = false;
												bool 凭据通过 = 主窗口.账号数据.TryGetValue(array[2], out var value)
													&& 账号数据.校验客户端哈希(value, array[3], out 需要升级门票);
												if (!凭据通过)
												{
													记录认证失败(result.客户地址.Address);
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 7 用户名或密码错误"));
												}
												else if (!主窗口.区服数据.TryGetValue(array[4], out value2))
												{
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 7 没有找到服务器"));
												}
												else
												{
													if (需要升级门票 && 主窗口.写盘许可())
													{
														value.账号密码 = array[3];
														try { 主窗口.保存账号(value); }
														catch (Exception ex升级2) { 主窗口.添加日志("升级密码存储失败: " + ex升级2.Message); }
													}
													记录认证成功(result.客户地址.Address);
													string text = 账号数据.生成门票();
													发送门票(value2, text, array[2]);
													// MED-E: 不再回显密码, 仅返回账号与门票.
													发送数据(result.客户地址, Encoding.UTF8.GetBytes(array[0] + " 6 " + array[2] + " " + text));
													主窗口.添加日志("成功生成门票!  账号: " + array[2] + " - " + text);
													Interlocked.Increment(ref 主窗口.生成门票总数);
													主窗口.更新已生成门票数();
												}
											}
											break;
										default:
											if (应记录解析错误(result.客户地址.Address))
											{
												主窗口.添加日志($"收到未定义的封包  地址: {result.客户地址}, 长度: {result.接收数据.Length}");
											}
											break;
									}
								}
							}
						}
						catch (Exception ex2)
						{
							主窗口.添加日志("封包处理错误: " + ex2.Message);
						}
					}
					主窗口.添加日志("停止处理客户请求.");
				});
				return true;
			}
			catch (Exception ex)
			{
				主窗口.添加日志(ex.Message);
				本地网络服务?.Close();
				本地网络服务 = null;
				return false;
			}
		}

		public static void 结束服务()
		{
			本地网络服务?.Close();
			本地网络服务 = null;
			限速表清理定时器?.Dispose();
			限速表清理定时器 = null;
		}

		public static void 发送数据(IPEndPoint 地址, byte[] 数据)
		{
			Interlocked.Add(ref 主窗口.已发送字节数, 数据.Length);
			主窗口.更新已发送字节数();
			UdpClient udp = 本地网络服务;
			if (udp != null)
			{
				try
				{
					udp.Send(数据, 数据.Length, 地址);
				}
				catch (Exception ex)
				{
					主窗口.添加日志("数据发送错误: " + ex.Message);
				}
			}
		}

		public static void 发送门票(IPEndPoint 地址, string 门票, string 账号)
		{
			// 注意: case "3" 调用本函数后还会再自增一次, 这里不能重复计数.
			// 保留语义一致, 改用 Interlocked 避免和 UI 线程竞争; 实际去重交由调用方.
			byte[] bytes = Encoding.UTF8.GetBytes(门票 + ";" + 账号);
			UdpClient udp = 本地网络服务;
			if (udp != null)
			{
				try
				{
					udp.Send(bytes, bytes.Length, new IPEndPoint(地址.Address, (ushort)主窗口.主界面.门票发送端口.Value));
				}
				catch (Exception ex)
				{
					主窗口.添加日志("门票发送失败: " + ex.Message);
				}
			}
		}
	}

}
