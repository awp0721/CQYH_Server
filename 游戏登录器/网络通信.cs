using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 游戏登录器
{

	public sealed class 网络通信
	{
		private const int 最大队列长度 = 256;
		private const int 最大封包长度 = 1024;

		public static UdpClient 通信实例;
		public static IPEndPoint 连接地址;
		public static ConcurrentQueue<byte[]> 接收队列;

		public static void 开始通信()
		{
			通信实例 = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
			接收队列 = new ConcurrentQueue<byte[]>();
			Task.Run(delegate
			{
				while (通信实例 != null)
				{
					try
					{
						IPEndPoint remoteEP = null;
						byte[] item = 通信实例.Receive(ref remoteEP);

						// 校验来源：只接受已配置的账号服务器地址，防止任意 IP 伪造响应
						if (连接地址 == null || remoteEP == null
							|| !remoteEP.Address.Equals(连接地址.Address)
							|| remoteEP.Port != 连接地址.Port)
						{
							continue;
						}

						// 长度校验，丢弃异常长包
						if (item == null || item.Length == 0 || item.Length > 最大封包长度)
						{
							continue;
						}

						// 队列上限，防止内存无限增长
						if (接收队列.Count >= 最大队列长度)
						{
							continue;
						}

						接收队列.Enqueue(item);
					}
					catch (ObjectDisposedException)
					{
						// Socket 已关闭，正常退出循环
						return;
					}
					catch (SocketException ex2)
					{
						// 仅在确实是连接被对端重置时提示，其他错误静默忽略避免强退
						if (ex2.SocketErrorCode == SocketError.ConnectionReset)
						{
							// UDP 上的 10054 通常意味着上一次发送目标不可达，丢弃本次接收继续即可
							continue;
						}
						// 其他 socket 错误也不强退，等待用户主动操作
						continue;
					}
					catch
					{
						// 兜底：单包异常不应导致进程退出
						continue;
					}
				}
			});
		}

		public static void 停止通信()
		{
			UdpClient temp = 通信实例;
			通信实例 = null;
			try
			{
				temp?.Close();
				temp?.Dispose();
			}
			catch
			{
			}
		}

		public static bool 发送数据(byte[] 数据)
		{
			if (通信实例 != null && 连接地址 != null && 数据 != null && 数据.Length > 0 && 数据.Length <= 最大封包长度)
			{
				try
				{
					通信实例.Send(数据, 数据.Length, 连接地址);
					return true;
				}
				catch
				{
					MessageBox.Show("连接服务器失败");
					return false;
				}
			}
			return false;
		}

	}
}
