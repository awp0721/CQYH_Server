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
        public class 创角请求
        {
            public string 账号;

            public string 推荐人;

            public string 名字;

            public 游戏对象职业 职业;

            public 游戏对象性别 性别;

            public 对象发色分类 发色;

            public 对象发型分类 发型;

            public 对象脸型分类 脸型;

            public bool 成功;

            public override string ToString()
            {
                return $"{this.账号},{this.推荐人},{this.名字},{this.职业},{this.性别},{this.发色},{this.发型},{this.脸型}";
            }
        }

        public sealed class 创角返回
        {
            public int code;

            public string message;

            public int timestamp;

            public string traceID;
        }

        public static DateTime 上次保存时间;

        public static DateTime 当前时间;

        public static DateTime 每秒计时;

        public static ConcurrentQueue<GM命令> 外部命令;

        public static uint 循环计数;

        public static bool 已经启动;

        public static bool 正在保存;

        public static Thread 主线程;

        public static Random 随机数;

        public static ConcurrentQueue<Action> 重载任务列表;

        public static DateTime 下次重载任务时间;

        public static Dictionary<int, NPCScript> Scripts;

        public static int ScriptIndex;

        public static NPCScript DefaultNPC;

        public static NPCScript MonsterNPC;

        public static NPCScript RobotNPC;

        public static int DefaultNPCID;

        public static DateTime 每日执行时间;

        public static ConcurrentQueue<string> DisplayLogs;

        public static ConcurrentQueue<string> Logs;

        public static bool UseLogConsole;

        public static bool OldForm;

        public static ConcurrentQueue<string> DisplayChatLogs;

        public static ConcurrentQueue<string> ChatLogs;

        public static ConcurrentQueue<string> GameLogs;

        public static ConcurrentQueue<string> ItemLogs;

        public static ConcurrentQueue<string> CurrencyLogs;

        public static ConcurrentQueue<string> WebLogs;

        private static ConcurrentQueue<LogData> WebLogDatas;

        public static ConcurrentQueue<string> DisplayCommandLogs;

        public static ConcurrentQueue<创角请求> 创角请求列表;

        public static ConcurrentQueue<创角请求> 创角返回列表;

        private static HttpClient Http;

        public static string LogWebSite;

        public static ConcurrentQueue<WebData> WebEvent;

        public static bool 自动保存中;

        public static int 开区天数 => 计算类.日期转换(主程.当前时间) - 计算类.日期转换(Settings.开服日期);

        public static int 开区节点
        {
            get
            {
                if (主程.开区天数 >= Settings.龙耀雪山节点开放天数)
                {
                    return 7;
                }
                if (主程.开区天数 >= Settings.苍月惊变节点开放天数)
                {
                    return 6;
                }
                if (主程.开区天数 >= Settings.魔龙之城节点开放天数)
                {
                    return 4;
                }
                if (主程.开区天数 >= Settings.白日赤月节点开放天数)
                {
                    return 3;
                }
                if (主程.开区天数 >= Settings.幽冥海底节点开放天数)
                {
                    return 1;
                }
                return 0;
            }
        }
        static 主程()
        {
            主程.重载任务列表 = new ConcurrentQueue<Action>();
            主程.Scripts = new Dictionary<int, NPCScript>();
            主程.DisplayLogs = new ConcurrentQueue<string>();
            主程.Logs = new ConcurrentQueue<string>();
            主程.UseLogConsole = false;
            主程.OldForm = false;
            主程.DisplayChatLogs = new ConcurrentQueue<string>();
            主程.ChatLogs = new ConcurrentQueue<string>();
            主程.GameLogs = new ConcurrentQueue<string>();
            主程.ItemLogs = new ConcurrentQueue<string>();
            主程.CurrencyLogs = new ConcurrentQueue<string>();
            //主程.WebLogs = new ConcurrentQueue<string>();
            //主程.WebLogDatas = new ConcurrentQueue<LogData>();
            主程.DisplayCommandLogs = new ConcurrentQueue<string>();
            主程.创角请求列表 = new ConcurrentQueue<创角请求>();
            主程.创角返回列表 = new ConcurrentQueue<创角请求>();
            //主程.LogWebSite = "http://58.87.104.125:8081/";
            //主程.WebEvent = new ConcurrentQueue<WebData>();
            主程.当前时间 = DateTime.Now;
            主程.每秒计时 = DateTime.Now.AddSeconds(1.0);
            主程.随机数 = new Random();
        }
    }
}
