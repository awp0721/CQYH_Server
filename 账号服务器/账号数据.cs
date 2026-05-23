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
			byte[] buf = new byte[32];
			安全随机.GetBytes(buf);
			char[] chars = new char[32];
			for (int i = 0; i < 32; i++)
			{
				chars[i] = RandomChars[buf[i] % RandomChars.Length];
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
