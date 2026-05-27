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
        public static void 启动服务()
        {
            主程.DefaultNPCID = 主程.随机数.Next(1000000, 1999999);
            主程.DefaultNPC = NPCScript.GetOrAdd(主程.DefaultNPCID, Settings.DefaultNPCFilename, NPCScriptType.AutoPlayer);
            主程.上次保存时间 = DateTime.Now;
            if (!主程.已经启动)
            {
                Thread obj;
                obj = new Thread(服务循环)
                {
                    IsBackground = true
                };
                主程.主线程 = obj;
                obj.Start();
            }
        }

        public static void 停止服务()
        {
            主程.已经启动 = false;
            网络服务网关.结束服务();
        }

        public static void 保存数据库()
        {
            if (主程.自动保存中)
            {
                return;
            }
            主程.自动保存中 = true;
            Task.Run(delegate
            {
                Stopwatch stopwatch;
                stopwatch = Stopwatch.StartNew();
                try
                {
                    主程.添加系统日志("正在保存客户数据到磁盘...");
                    游戏数据网关.导出数据();
                    stopwatch.Stop();
                    主程.添加系统日志($"客户数据保存完毕 , 耗时:{stopwatch.ElapsedMilliseconds} 线程ID:{Thread.CurrentThread.ManagedThreadId}");
                }
                catch (Exception ex)
                {
                    主程.添加系统日志($"客户数据保存异常 , 耗时:{stopwatch.ElapsedMilliseconds} 线程ID:{Thread.CurrentThread.ManagedThreadId} e:{ex.Message} {((ex.InnerException != null) ? (".IE:" + ex.InnerException.Message) : "...")}");
                }
                主程.自动保存中 = false;
            });
        }

        public static void ReloadNPCs(int[] scriptIds = null)
        {
            if (scriptIds == null)
            {
                foreach (int key2 in 主程.Scripts.Keys)
                {
                    主程.Scripts[key2].Load();
                }
            }
            else
            {
                foreach (int key in scriptIds)
                {
                    if (主程.Scripts.TryGetValue(key, out var value))
                    {
                        value.Load();
                        主程.添加系统日志("NPC " + value.FileName + " 脚本重载成功...");
                    }
                }
            }
            游戏脚本.初始化脚本系统();
            主程.添加系统日志("NPC 脚本重载成功...");
        }
    }
}
