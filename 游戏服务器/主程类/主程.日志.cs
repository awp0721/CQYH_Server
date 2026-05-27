using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoBattle;
using _001D_000F_0007_0013_0011_0015;
using 游戏服务器.地图类;
using 游戏服务器.管理命令;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using Newtonsoft.Json;

namespace 游戏服务器
{
    public static partial class 主程
    {
        public static void 添加系统日志(string log, bool hardLog = true, bool showDiag = true)
        {
            if (主程.OldForm)
            {
                主窗口.添加系统日志(log);
            }
            if (主程.UseLogConsole)
            {
                log = $"[{主程.当前时间:F}]: {log}";
                Console.WriteLine(log);
                return;
            }
            log = $"[{主程.当前时间:F}]: {log}";
            if (主程.DisplayLogs.Count < 100 && showDiag)
            {
                主程.DisplayLogs.Enqueue(log);
            }
            if (hardLog && 主程.Logs.Count < 1000)
            {
                主程.Logs.Enqueue(log);
            }
        }

        public static void 添加聊天日志(string 前缀, byte[] 内容)
        {
            if (主程.OldForm)
            {
                主窗口.添加聊天日志(前缀, 内容);
                return;
            }
            string item;
            item = $"[{主程.当前时间:F}]: {前缀 + Encoding.UTF8.GetString(内容).Trim('\0')}";
            if (主程.DisplayChatLogs.Count < 500)
            {
                主程.DisplayChatLogs.Enqueue(item);
            }
            if (主程.ChatLogs.Count < 1000)
            {
                主程.ChatLogs.Enqueue(item);
            }
        }

        public static void 添加命令日志(string 文本)
        {
            if (主程.OldForm)
            {
                主窗口.添加系统日志(文本);
                return;
            }
            文本 = $"[{主程.当前时间:F}]: {文本}";
            if (主程.DisplayCommandLogs.Count < 500)
            {
                主程.DisplayCommandLogs.Enqueue(文本);
            }
            主程.Logs.Enqueue(文本);
        }

        internal static void WriteLogs()
        {
            string text;
            text = Settings.游戏数据目录 + "\\Log";
            List<string> list;
            list = new List<string>();
            while (!主程.Logs.IsEmpty)
            {
                if (主程.Logs.TryDequeue(out var result))
                {
                    list.Add(result);
                }
            }
            if (!Directory.Exists(text + "\\SystemLog"))
            {
                Directory.CreateDirectory(text + "\\SystemLog");
            }
            if (list.Count > 0)
            {
                File.AppendAllLines($"{text}\\SystemLog\\{DateTime.Now:yyyy-MM-dd HH 00 00}.txt", list);
            }
            list.Clear();
            while (!主程.ChatLogs.IsEmpty)
            {
                if (主程.ChatLogs.TryDequeue(out var result2))
                {
                    list.Add(result2);
                }
            }
            if (!Directory.Exists(text + "\\ChatLogs"))
            {
                Directory.CreateDirectory(text + "\\ChatLogs");
            }
            if (list.Count > 0)
            {
                File.AppendAllLines($"{text}\\ChatLogs\\{DateTime.Now:yyyy-MM-dd HH 00 00}.txt", list);
            }
            list.Clear();
            while (!主程.GameLogs.IsEmpty)
            {
                if (主程.GameLogs.TryDequeue(out var result3))
                {
                    list.Add(result3);
                }
            }
            if (!Directory.Exists(text + "\\GameLogs"))
            {
                Directory.CreateDirectory(text + "\\GameLogs");
            }
            if (list.Count > 0)
            {
                File.AppendAllLines($"{text}\\GameLogs\\{DateTime.Now:yyyy-MM-dd HH 00 00}.txt", list);
            }
            list.Clear();
            while (!主程.ItemLogs.IsEmpty)
            {
                if (主程.ItemLogs.TryDequeue(out var result4))
                {
                    list.Add(result4);
                }
            }
            if (!Directory.Exists(text + "\\ItemLogs"))
            {
                Directory.CreateDirectory(text + "\\ItemLogs");
            }
            if (list.Count > 0)
            {
                File.AppendAllLines($"{text}\\ItemLogs\\{DateTime.Now:yyyy-MM-dd HH 00 00}.txt", list);
            }
            list.Clear();
            while (!主程.CurrencyLogs.IsEmpty)
            {
                if (主程.CurrencyLogs.TryDequeue(out var result5))
                {
                    list.Add(result5);
                }
            }
            if (!Directory.Exists(text + "\\CurrencyLogs"))
            {
                Directory.CreateDirectory(text + "\\CurrencyLogs");
            }
            if (list.Count > 0)
            {
                File.AppendAllLines($"{text}\\CurrencyLogs\\{DateTime.Now:yyyy-MM-dd HH 00 00}.txt", list);
            }
            list.Clear();
            /*
            while (!主程.WebLogs.IsEmpty)
            {
                if (主程.WebLogs.TryDequeue(out var result6))
                {
                    list.Add(result6);
                }
            }
            
            if (!Directory.Exists(text + "\\WebLogs"))
            {
                Directory.CreateDirectory(text + "\\WebLogs");
            }
            
            if (list.Count > 0)
            {
                File.AppendAllLines($"{text}\\WebLogs\\{DateTime.Now:yyyy-MM-dd HH 00 00}.txt", list);
            }
            */
            list.Clear();
            list.Clear();
        }

        public static void 添加重铸日志(角色数据 角色数据, 装备数据 关联物品, 列表监视器<随机属性> 随机属性)
        {
            foreach (随机属性 item2 in 随机属性)
            {
                string item;
                item = $"{主程.当前时间:F} {角色数据.角色名字.V}\t玩家洗练装备\t{关联物品.物品名字}\t{item2.属性编号.ToString()}\t{item2.属性数值.ToString()}\t{item2.属性描述}";
                主程.GameLogs.Enqueue(item);
            }
        }

        public static void 添加物品日志(玩家实例 玩家对象, string 动作名称, 物品数据 关联物品, int 物品数量, string remark = null)
        {
            主程.添加物品日志(玩家对象.角色数据, 动作名称, 关联物品, 物品数量, remark);
        }

        public static void 添加物品日志(角色数据 角色数据, string 动作名称, 物品数据 关联物品, int 物品数量, string remark = null)
        {
            string item;
            item = $"{主程.当前时间:F} {角色数据.角色名字.V}\t{动作名称.ToString()}\t{关联物品.物品名字}\t{关联物品.物品编号}\t{关联物品.数据索引}\t{物品数量.ToString()}\t{((!关联物品.能否堆叠) ? 1 : 关联物品.当前持久.V)}\t{remark}";
            主程.ItemLogs.Enqueue(item);
        }

        public static void 添加货币日志(玩家实例 玩家对象, string 动作名称, 游戏货币 货币名字, uint 货币数量)
        {
            主程.添加货币日志(玩家对象.角色数据, 动作名称, 货币名字, (int)货币数量);
        }

        public static void 添加货币日志(玩家实例 玩家对象, string 动作名称, 游戏货币 货币名字, int 货币数量)
        {
            主程.添加货币日志(玩家对象.角色数据, 动作名称, 货币名字, 货币数量);
        }

        public static void 添加货币日志(角色数据 角色数据, string 动作名称, 游戏货币 货币名字, uint 货币数量)
        {
            主程.添加货币日志(角色数据, 动作名称, 货币名字, (int)货币数量);
        }

        public static void 添加货币日志(角色数据 角色数据, string 动作名称, 游戏货币 货币名字, int 货币数量)
        {
            uint num;
            num = 0u;
            uint v;
            if (货币名字 == 游戏货币.元宝)
            {
                num = 角色数据.元宝数量;
            }
            else if (角色数据.角色货币.TryGetValue(货币名字, out v))
            {
                num = v;
            }
            string item;
            item = $"{主程.当前时间:F} {角色数据.角色名字.V}\t{动作名称}\t{货币名字.ToString()}\t{货币数量.ToString()}\t{num.ToString()}";
            主程.CurrencyLogs.Enqueue(item);
            if (货币数量 > 0)
            {
                //主程.WebLog(LogDataType.OutputLog, Settings.统计UUID代码, Settings.游戏区服名称, 角色数据.角色名字.V, 角色数据.所属账号.V.账号名字.V, 货币数量.ToString(), 动作名称, 货币名字.ToString());
            }
            if (货币数量 < 0)
            {
                //主程.WebLog(LogDataType.ConsumptionLog, Settings.统计UUID代码, Settings.游戏区服名称, 角色数据.角色名字.V, 角色数据.所属账号.V.账号名字.V, (-货币数量).ToString(), 动作名称, 货币名字.ToString());
            }
        }
    }
}
