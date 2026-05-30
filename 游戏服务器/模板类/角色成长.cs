using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using _001D_000F_0007_0013_0011_0015;

namespace 游戏服务器.模板类
{
	public sealed class 角色成长
	{
		public static Dictionary<int, Dictionary<游戏对象属性, int>> 数据表;

		public static Dictionary<byte, long> 升级所需经验;

		public static Dictionary<byte, int> 升级增加战力;

		public static readonly ushort[] 宠物升级经验;

		static 角色成长()
		{
			角色成长.升级所需经验 = new Dictionary<byte, long>();
			角色成长.升级增加战力 = new Dictionary<byte, int>();
			角色成长.宠物升级经验 = new ushort[9] { 5, 10, 15, 20, 25, 30, 35, 40, 45 };
			角色成长.数据表 = new Dictionary<int, Dictionary<游戏对象属性, int>>();
			string 成长属性文件 = Settings.游戏数据目录 + "\\System\\成长属性.txt";
			string[] array;
			array = Regex.Split(File.ReadAllText(成长属性文件).Trim('\r', '\n', '\r'), "\r\n", RegexOptions.IgnoreCase);
			string[] 属性名数组;
			属性名数组 = array[0].Split('\t');
			Dictionary<string, int> dictionary;
			dictionary = 属性名数组.ToDictionary( (string K) => K, (string V) => Array.IndexOf(属性名数组, V));
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2;
				array2 = array[i].Split('\t');
				if (array2.Length <= 1)
				{
					continue;
				}
				try
				{
					Dictionary<游戏对象属性, int> dictionary2;
					dictionary2 = new Dictionary<游戏对象属性, int>();
					int key;
					key = (int)(游戏对象职业)Enum.Parse(typeof(游戏对象职业), array2[0]) * 256 + Convert.ToInt32(array2[1]);
					for (int j = 2; j < 属性名数组.Length; j++)
					{
						if (Enum.TryParse<游戏对象属性>(属性名数组[j], out var result) && Enum.IsDefined(typeof(游戏对象属性), result))
						{
							dictionary2[result] = Convert.ToInt32(array2[dictionary[result.ToString()]]);
						}
					}
					角色成长.数据表.Add(key, dictionary2);
				}
				catch (Exception ex)
				{
					throw 角色成长.解析异常(成长属性文件, i + 1, array[i], ex);
				}
			}
			角色成长.加载键值文件(Settings.游戏数据目录 + "\\System\\玩家升级经验.txt", (string 键, string 值) => 角色成长.升级所需经验.Add(byte.Parse(键), long.Parse(值)));
			角色成长.加载键值文件(Settings.游戏数据目录 + "\\System\\玩家升级战力.txt", (string 键, string 值) => 角色成长.升级增加战力.Add(byte.Parse(键), int.Parse(值)));
		}

		// 逐行解析 `键=值` 配置文件;任意一行解析失败都抛出带 文件名 + 行号 + 原始行内容 的异常,
		// 取代原先只甩一个 `'10 2'` 的 FormatException(空格等坏数据在静态构造里抛出会被包成
		// TypeInitializationException 一路冒泡成未处理异常致服务器启动失败,且无从定位是哪行哪文件)。
		private static void 加载键值文件(string 文件路径, Action<string, string> 写入)
		{
			string[] array = Regex.Split(File.ReadAllText(文件路径).Trim('\r', '\n', '\r'), "\r\n", RegexOptions.IgnoreCase);
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(array[i]))
				{
					continue;
				}
				string[] 键值;
				键值 = array[i].Split('=');
				if (键值.Length < 2)
				{
					throw 角色成长.解析异常(文件路径, i + 1, array[i], new FormatException("缺少 '=' 分隔符或值为空"));
				}
				try
				{
					写入(键值[0].Trim(), 键值[1].Trim());
				}
				catch (Exception ex)
				{
					throw 角色成长.解析异常(文件路径, i + 1, array[i], ex);
				}
			}
		}

		private static InvalidDataException 解析异常(string 文件路径, int 行号, string 原始行, Exception 内层)
		{
			return new InvalidDataException($"加载「{Path.GetFileName(文件路径)}」第 {行号} 行失败:「{原始行}」({内层.Message})", 内层);
		}

		public static Dictionary<游戏对象属性, int> 获取数据(游戏对象职业 职业, byte 等级)
		{
			return 角色成长.数据表[(byte)职业 * 256 + Math.Max(等级, (byte)1)];
		}

		public static int 等级战力(byte 等级)
		{
			int num;
			num = 0;
			for (byte b = 1; b < 等级 + 1; b++)
			{
				num += 角色成长.升级增加战力[b];
			}
			return num;
		}
	}
}
