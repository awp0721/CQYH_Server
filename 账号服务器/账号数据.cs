using System;
using System.Security.Cryptography;
using System.Text;

namespace 账号服务器
{
	public sealed class 账号数据
	{
		private static readonly RandomNumberGenerator 安全随机 = RandomNumberGenerator.Create();

		// 常量时间字符串比较, 防止密码/密保答案被时序攻击逐字节还原.
		// 长度不同直接返回 false (长度本身不构成可放大的口令信息泄漏).
		public static bool 安全比较(string a, string b)
		{
			if (a == null || b == null) return false;
			byte[] ba = Encoding.UTF8.GetBytes(a);
			byte[] bb = Encoding.UTF8.GetBytes(b);
			if (ba.Length != bb.Length) return false;
			return CryptographicOperations.FixedTimeEquals(ba, bb);
		}

		// 协议 v2: 客户端发包前对密码做 SHA256("YH-Auth-v2:" || 账号 || ":" || 明文) 形成 64-char hex.
		// 服务端校验: 旧账号 (持久化字段为明文) 用此函数算出"期望哈希"再与客户端发的哈希做常量时间比较;
		// 新账号 (持久化字段已是 64-char hex) 直接和客户端发的哈希比较.
		// 两端实现必须一致, 客户端见 游戏登录器/登录界面.cs 同名实现.
		public static string 密码哈希(string 账号, string 明文密码)
		{
			byte[] bytes = Encoding.UTF8.GetBytes("YH-Auth-v2:" + 账号 + ":" + 明文密码);
			byte[] hash = SHA256.HashData(bytes);
			StringBuilder sb = new StringBuilder(hash.Length * 2);
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("x2"));
			}
			return sb.ToString();
		}

		// 判断字符串是否为合法的 64 char 小写 hex (即 v2 协议的密码哈希格式)
		public static bool 是哈希格式(string s)
		{
			if (string.IsNullOrEmpty(s) || s.Length != 64)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				bool ok = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
				if (!ok)
				{
					return false;
				}
			}
			return true;
		}

		// 通用密码校验: 自动兼容存储为明文 (历史数据) 或 hash (v2 协议) 的账号.
		// 客户端传来的字段必须是 hash. 返回 true 表示匹配;
		// out 需要升级存储 表示该账号目前还是明文存储, 校验通过后调用方应保存为 hash 格式.
		public static bool 校验客户端哈希(账号数据 账号, string 客户端哈希, out bool 需要升级存储)
		{
			需要升级存储 = false;
			if (账号 == null || !是哈希格式(客户端哈希))
			{
				return false;
			}
			if (是哈希格式(账号.账号密码))
			{
				return 安全比较(客户端哈希, 账号.账号密码);
			}
			string 期望 = 密码哈希(账号.账号名字, 账号.账号密码);
			if (安全比较(客户端哈希, 期望))
			{
				需要升级存储 = true;
				return true;
			}
			return false;
		}

		private static char[] RandomChars = new char[62]
		{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
		'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
		'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
		'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
		'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
		'y', 'z'
		};

		public string 账号名字;

		public string 账号密码;

		public string 密保问题;

		public string 密保答案;

		public DateTime 创建日期;

		public static string 生成门票()
		{
			// 用 GetInt32 做拒绝-重抽, 消除 256 % 62 = 8 带来的模偏置 (LOW-M).
			char[] chars = new char[32];
			for (int i = 0; i < 32; i++)
			{
				chars[i] = RandomChars[RandomNumberGenerator.GetInt32(0, RandomChars.Length)];
			}
			return "ULS21-" + new string(chars);
		}

		public 账号数据(string 账号, string 密码, string 问题, string 答案)
		{
			账号名字 = 账号;
			账号密码 = 密码;
			密保问题 = 问题;
			密保答案 = 答案;
			创建日期 = DateTime.Now;
		}
	}
}
