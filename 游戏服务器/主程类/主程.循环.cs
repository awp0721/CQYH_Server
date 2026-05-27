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
        private static async Task WebPostLoop()
        {
            while (主程.已经启动)
            {
                await 主程.处理创建角色请求();
            }
        }

        private static void WriteLogsLoop()
        {
            DateTime dateTime;
            dateTime = 主程.当前时间.AddSeconds(10.0);
            DateTime dateTime2;
            dateTime2 = 主程.当前时间.AddMinutes(10.0);
            while (主程.已经启动)
            {
                //主程.ProcessWebLog();
                if (主程.当前时间 > dateTime2)
                {
                    long num;
                    num = 0L;
                    long num2;
                    num2 = 0L;
                    long num3;
                    num3 = 0L;
                    long num4;
                    num4 = 0L;
                    long num5;
                    num5 = 0L;
                    long num6;
                    num6 = 0L;
                    foreach (KeyValuePair<int, 游戏数据> item in 游戏数据网关.角色数据表.数据表)
                    {
                        角色数据 角色数据;
                        角色数据 = item.Value as 角色数据;
                        if (角色数据.商人角色.V)
                        {
                            num4 += 角色数据.金币数量;
                            num6 += 角色数据.银币数量;
                        }
                        else
                        {
                            num += 角色数据.金币数量;
                            num3 += 角色数据.银币数量;
                        }
                    }
                    foreach (KeyValuePair<int, 游戏数据> item2 in 游戏数据网关.账号数据表.数据表)
                    {
                        账号数据 账号数据;
                        账号数据 = item2.Value as 账号数据;
                        if (账号数据.角色列表.FirstOrDefault((角色数据 x) => x.商人角色.V) != null)
                        {
                            num5 += 账号数据.元宝数量.V;
                        }
                        else
                        {
                            num2 += 账号数据.元宝数量.V;
                        }
                    }
                    uint 已登录连接数;
                    已登录连接数 = 网络服务网关.已登录连接数;
                    int num7;
                    num7 = 游戏数据网关.账号数据表.数据表.Values.Select((游戏数据 x) => (x as 账号数据).网络连接?.物理地址).Distinct().Count();
                    //主程.WebLog(LogDataType.ProxyDataUp, Settings.统计UUID代码, Settings.游戏区服名称, 已登录连接数.ToString(), num7.ToString(), num2.ToString(), num.ToString(), num3.ToString(), num5.ToString(), num4.ToString(), num6.ToString());
                    dateTime2 = 主程.当前时间.AddMinutes(10.0);
                }
                if (主程.当前时间 < dateTime)
                {
                    Thread.Sleep(1);
                    continue;
                }

                try
                {
                    主程.WriteLogs();
                }
                catch (Exception ex)
                {
                    主程.添加系统日志("WriteLogs Error:" + ex.Message + "\r\n" + ex.StackTrace);
                }

                dateTime = 主程.当前时间.AddSeconds(10.0);

            }
        }

        private static void 服务循环()
        {
            主程.外部命令 = new ConcurrentQueue<GM命令>();
            主程.添加系统日志("正在生成地图元素...");
            地图处理网关.开启地图();
            主程.添加系统日志("正在启动网络服务...");
            网络服务网关.启动服务();
            主程.添加系统日志("服务器已成功开启");
            主程.添加系统日志($"逻辑线程ID: {Thread.CurrentThread.ManagedThreadId}");
            主程.已经启动 = true;
            主窗口.服务启动回调();
            主程.每日执行时间 = 主程.当前时间;
            Thread thread;
            thread = new Thread(WriteLogsLoop);
            thread.IsBackground = true;
            thread.Start();

            Task.Run(async () =>
            {
                await 主程.WebPostLoop();
            });

            AutoBattleManager.Start();

            机器人.初始化();
            while (主程.已经启动 || 网络服务网关.网络连接表.Count != 0)
            {
                try
                {
                    Thread.Sleep(1);
                    主程.当前时间 = DateTime.Now;
                    if (主窗口.暂停界面更新)
                    {
                        continue;
                    }
                    if (主程.当前时间 > 主程.每秒计时)
                    {
                        if (!主程.自动保存中)
                        {
                            游戏数据网关.保存数据();
                        }
                        SMain.UpdateDelay(地图处理网关.激活对象表.Count, 地图处理网关.次要对象表.Count, 地图处理网关.地图对象表.Count);
                        SMain.UpdateTick(主程.循环计数);
                        主程.循环计数 = 0u;
                        主程.每秒计时 = 主程.当前时间.AddSeconds(1.0);
                    }
                    else
                    {
                        主程.循环计数++;
                    }
                    GM命令 result;
                    while (主程.外部命令.TryDequeue(out result))
                    {
                        result.执行命令();
                    }
                    网络服务网关.处理数据();
                    地图处理网关.处理数据();
                    游戏数据网关.处理数据();
                    系统公告.处理数据();
                    机器人.处理数据();
                    主程.处理重载任务();
                    //主程.处理网页事件();
                    if (主程.当前时间 > 主程.上次保存时间.AddMinutes((int)Settings.自动保存时间))
                    {
                        主程.上次保存时间 = 主程.当前时间;
                        游戏数据网关.保存数据();
                        主程.保存数据库();
                    }
                    if (主程.当前时间.Date != 主程.每日执行时间.Date)
                    {
                        主程.每日执行时间 = 主程.当前时间;
                        地图处理网关.处理在线玩家天数变更();
                    }
                    游戏脚本.垃圾收集();
                }
                catch (Exception ex)
                {
                    主程.添加系统日志("发生致命错误, 服务器即将停止");
                    if (!Directory.Exists("..\\Log\\Error"))
                    {
                        Directory.CreateDirectory("..\\Log\\Error");
                    }
                    File.WriteAllText($"..\\Log\\Error\\{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.txt", "错误信息:\r\n" + ex.Message + "\r\n堆栈信息:\r\n" + ex.StackTrace);
                    主程.添加系统日志("错误信息已保存到日志, 请注意查看");
                }
            }
            主程.添加系统日志("正在清理物品数据...");
            地图处理网关.清理物品();
            Thread.Sleep(10000);
            主程.添加系统日志("正在保存客户数据...");
            游戏数据网关.检测物品数据(添加到数据表: true);
            游戏数据网关.强制保存();
            Thread.Sleep(10000);
            主程.添加系统日志("正在导出客户数据...");
            游戏数据网关.导出数据();
            主窗口.服务停止回调();
            网络服务网关.循环结束();
            主程.主线程 = null;
            主程.添加系统日志("服务器已成功关闭");

        }

        private static void 处理重载任务()
        {
            if (主程.当前时间 > 主程.下次重载任务时间 && 主程.重载任务列表.TryDequeue(out var result))
            {
                result();
                if (主程.重载任务列表.Count != 0)
                {
                    主程.下次重载任务时间 = 主程.当前时间.AddMilliseconds(500.0);
                }
            }
        }
    }
}
