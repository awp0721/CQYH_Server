using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebApi;
using 游戏服务器.地图类;
using 游戏服务器.数据类;

namespace 游戏服务器.网络类
{
    public static class 网络服务网关
    {
        private static IPEndPoint 门票发送端;

        private static UdpClient 门票接收器;

        private static TcpListener 网络监听器;

        public static bool 网络服务停止;

        public static bool 未登录连接数;

        public static uint 已登录连接数;

        public static uint 已上线连接数;

        public static long 已发送字节数;

        public static long 已接收字节数;

        public static HashSet<客户网络> 网络连接表;

        // DoS 防护: 限制单 IP 并发 TCP 连接数 + 全局总量上限.
        // 原 444 行的 9999 全局上限被注释掉, 加上无 per-IP 限制 → 攻击者从单机即可耗尽 fd/内存.
        // 计数表只在 异步连接 (accept callback) 与 等待移除表 处理点变更, ConcurrentDictionary 即可.
        public const int 单IP最大连接数 = 10;
        public const int 全局最大连接数 = 9999;
        public static ConcurrentDictionary<string, int> 客户连接计数 = new ConcurrentDictionary<string, int>();

        public static ConcurrentQueue<客户网络> 等待移除表;

        public static ConcurrentQueue<客户网络> 等待添加表;

        public static ConcurrentQueue<游戏封包> 全服公告表;

        //public static ConcurrentQueue<string> Http门票数据;

        public static Dictionary<string, 门票信息> 门票数据表;

        //public static Http门票接收器 门票接收器Http;

        //private static WebApiService http;

        public static void 启动服务()
        {
            网络服务网关.网络服务停止 = false;
            网络服务网关.网络连接表 = new HashSet<客户网络>();
            网络服务网关.等待添加表 = new ConcurrentQueue<客户网络>();
            网络服务网关.等待移除表 = new ConcurrentQueue<客户网络>();
            网络服务网关.全服公告表 = new ConcurrentQueue<游戏封包>();
            网络服务网关.网络监听器 = new TcpListener(IPAddress.Any, Settings.客户连接端口);
            网络服务网关.网络监听器.Start();
            网络服务网关.网络监听器.BeginAcceptTcpClient(异步连接, null);
            网络服务网关.门票数据表 = new Dictionary<string, 门票信息>();
            网络服务网关.门票接收器 = new UdpClient(new IPEndPoint(IPAddress.Any, Settings.门票接收端口));
            /*
            网络服务网关.Http门票数据 = new ConcurrentQueue<string>();
            if (Settings.Http门票接收端口 != 0)
            {
                网络服务网关.门票接收器Http = new Http门票接收器(Settings.Http门票接收端口, 网络服务网关.Http门票数据);
                网络服务网关.门票接收器Http.Start();
            }
            //网络服务网关.http = new WebApiService();
            //网络服务网关.http.Start();
            */
        }

        //public static void RestartHttpService(int typeId = 0)
        public static void RestartHttpService()
        {
            网络服务网关.门票接收器?.Close();
            网络服务网关.门票接收器?.Dispose();
            //网络服务网关.门票接收器 = new UdpClient(new IPEndPoint(IPAddress.Loopback, Settings.门票接收端口));
            网络服务网关.门票接收器 = new UdpClient(new IPEndPoint(IPAddress.Any, Settings.门票接收端口));
            主程.添加系统日志("门票接收服务已重启");
            /*
            switch (typeId)
            {

                case 1:
                    网络服务网关.http?.Stop();
                    网络服务网关.http = new WebApiService();
                    网络服务网关.http.Start();
                    主程.添加系统日志("充值接口服务已重启");
                    break;
                case 0:
                    if (Settings.Http门票接收端口 != 0)
                    {
                        网络服务网关.门票接收器Http?.Stop();
                        网络服务网关.门票接收器Http = new Http门票接收器(Settings.Http门票接收端口, 网络服务网关.Http门票数据);
                        网络服务网关.门票接收器Http.Start();
                        主程.添加系统日志("Http门票接收服务已重启");
                    }

                    网络服务网关.门票接收器?.Close();
                    网络服务网关.门票接收器?.Dispose();
                    网络服务网关.门票接收器 = new UdpClient(new IPEndPoint(IPAddress.Loopback, Settings.门票接收端口));
                    主程.添加系统日志("门票接收服务已重启");
                    break;
            }
            */
        }

        public static void 结束服务()
        {
            网络服务网关.网络服务停止 = true;
        }

        public static void 循环结束()
        {
            网络服务网关.网络监听器?.Stop();
            网络服务网关.网络监听器 = null;
            网络服务网关.门票接收器?.Close();
            网络服务网关.门票接收器 = null;
            //网络服务网关.http?.Stop();
            //网络服务网关.门票接收器Http?.Stop();
            //网络服务网关.门票接收器Http = null;
        }

        public static void 处理数据()
        {
            #region 九八百牛专用网关登录器
            /*
            try
            {
                while (true)
                {
                    UdpClient udpClient;
                    udpClient = 网络服务网关.门票接收器;
                    if (udpClient == null || udpClient.Available == 0)
                    {
                        break;
                    }
                    byte[] bytes;
                    bytes = 网络服务网关.门票接收器.Receive(ref 网络服务网关.门票发送端);
                    string[] array;
                    array = Encoding.UTF8.GetString(bytes).Split(';');
                    if (array.Length != 5)
                    {
                        continue;
                    }
                    string text;
                    text = array[1];
                    if (text.StartsWith("按角色登录-"))
                    {
                        text = text.Substring(6);
                        if (!游戏数据网关.角色数据表.检索表.TryGetValue(text, out var value))
                        {
                            主程.添加系统日志("[本地登陆器-按角色]不存在的角色名:" + text);
                            break;
                        }
                        text = (value as 角色数据)?.所属账号.V.账号名字.V;
                    }
                    网络服务网关.门票数据表[array[0]] = new 门票信息
                    {
                        登录账号 = text,
                        有效时间 = 主程.当前时间.AddMinutes(5.0)
                    };
                    账号数据 账号数据;
                    账号数据 = ((!游戏数据网关.账号数据表.检索表.TryGetValue(text, out var value2)) ? new 账号数据(text) : (value2 as 账号数据));
                    账号数据.所属UUID.V = array[4];
                    if (账号数据.推广代码.V == string.Empty || 账号数据.推广代码.V == null)
                    {
                        账号数据.推广代码.V = array[2];
                        账号数据.推荐人码.V = array[3];
                    }
                }
            }
            catch (Exception ex)
            {
                主程.添加系统日志("接收登录门票时发生错误. " + ex.Message);
            }
            */
            //----- http门票，已禁用--------
            /*
            try
            {
                string result;
                while (网络服务网关.Http门票数据.TryDequeue(out result))
                {
                    string[] array2;
                    array2 = result.Split(';');
                    if (array2.Length == 5)
                    {
                        网络服务网关.门票数据表[array2[0]] = new 门票信息
                        {
                            登录账号 = array2[1],
                            有效时间 = 主程.当前时间.AddMinutes(5.0)
                        };
                        账号数据 账号数据2;
                        账号数据2 = ((!游戏数据网关.账号数据表.检索表.TryGetValue(array2[1], out var value3)) ? new 账号数据(array2[1]) : (value3 as 账号数据));
                        账号数据2.所属UUID.V = array2[4];
                        if (账号数据2.推广代码.V == string.Empty || 账号数据2.推广代码.V == null)
                        {
                            账号数据2.推广代码.V = array2[2];
                            账号数据2.推荐人码.V = array2[3];
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                主程.添加系统日志("接收Http登录门票时发生错误. " + ex2.Message);
            }
            */
            //--------------------
            /* -------百牛九八原来的------
            foreach (客户网络 item in 网络服务网关.网络连接表)
            {
                if (!item.正在断开 && item.绑定账号 == null && 主程.当前时间.Subtract(item.接入时间).TotalSeconds > 30.0)
                {
                    item.尝试断开连接(new Exception("登录超时, 断开连接!"));
                }
                else
                {
                    item.处理数据();
                }
            }
            while (!网络服务网关.等待移除表.IsEmpty)
            {
                if (网络服务网关.等待移除表.TryDequeue(out var result2))
                {
                    网络服务网关.网络连接表.Remove(result2);
                }
            }
            while (!网络服务网关.等待添加表.IsEmpty)
            {
                if (网络服务网关.等待添加表.TryDequeue(out var result3))
                {
                    网络服务网关.网络连接表.Add(result3);
                }
            }
            while (!网络服务网关.全服公告表.IsEmpty)
            {
                if (!网络服务网关.全服公告表.TryDequeue(out var result4))
                {
                    continue;
                }
                foreach (客户网络 item2 in 网络服务网关.网络连接表)
                {
                    if (item2.绑定角色 != null)
                    {
                        item2.发送封包(result4);
                    }
                }
            }
            */
            #endregion 
            if (Settings.专用网关登录器)
            {
                try
                {
                    //九八的专用网关登录器
                    while (true)
                    {
                        UdpClient udpClient;
                        udpClient = 网络服务网关.门票接收器;
                        if (udpClient == null || udpClient.Available == 0)
                        {
                            break;
                        }
                        byte[] bytes;
                        bytes = 网络服务网关.门票接收器.Receive(ref 网络服务网关.门票发送端);
                        string[] array;
                        array = Encoding.UTF8.GetString(bytes).Split(';');
                        if (array.Length != 5)
                        {
                            continue;
                        }
                        string text;
                        text = array[1];
                        if (text.StartsWith("按角色登录-"))
                        {
                            text = text.Substring(6);
                            if (!游戏数据网关.角色数据表.检索表.TryGetValue(text, out var value))
                            {
                                主程.添加系统日志("[本地登陆器-按角色]不存在的角色名:" + text);
                                break;
                            }
                            text = (value as 角色数据)?.所属账号.V.账号名字.V;
                        }
                        网络服务网关.门票数据表[array[0]] = new 门票信息
                        {
                            登录账号 = text,
                            有效时间 = 主程.当前时间.AddMinutes(5.0)
                        };
                        账号数据 账号数据;
                        账号数据 = ((!游戏数据网关.账号数据表.检索表.TryGetValue(text, out var value2)) ? new 账号数据(text) : (value2 as 账号数据));
                        账号数据.所属UUID.V = array[4];
                        if (账号数据.推广代码.V == string.Empty || 账号数据.推广代码.V == null)
                        {
                            账号数据.推广代码.V = array[2];
                            账号数据.推荐人码.V = array[3];
                        }
                    }
                }
                catch (Exception ex)
                {
                    主程.添加系统日志("接收登录门票时发生错误. " + ex.Message);
                }
            }
            else
            {
                //通用网关登录器
                try
                {
                    while (true)
                    {
                        string[] strArray;
                        do
                        {
                            UdpClient 门票接收器 = 网络服务网关.门票接收器;
                            if (门票接收器 != null)
                            {
                                if (门票接收器.Available != 0)
                                    strArray = Encoding.UTF8.GetString(网络服务网关.门票接收器.Receive(ref 网络服务网关.门票发送端)).Split(';');
                                else
                                    goto label_5;
                            }
                            else
                                goto label_5;
                        }
                        while (strArray.Length != 2);
                        网络服务网关.门票数据表[strArray[0]] = new 门票信息()
                        {
                            登录账号 = strArray[1],
                            有效时间 = 主程.当前时间.AddMinutes(5.0)
                        };
                    }
                }
                catch (Exception ex)
                {
                    主程.添加系统日志("接收登录门票时发生错误. " + ex.Message);
                }
            }


        label_5:

            //-----------------------
            foreach (客户网络 客户网络 in 网络服务网关.网络连接表)
            {
                if (!客户网络.正在断开 && 客户网络.绑定账号 == null && 主程.当前时间.Subtract(客户网络.接入时间).TotalSeconds > 30.0)
                    客户网络.尝试断开连接(new Exception("登录超时, 断开连接!"));
                else
                    客户网络.处理数据();//处理客户端数据
            }
            //-----------------------
            while (!网络服务网关.等待移除表.IsEmpty)
            {
                客户网络 result;
                if (网络服务网关.等待移除表.TryDequeue(out result))
                {
                    网络服务网关.网络连接表.Remove(result);
                    // 同步释放 per-IP 计数; 不能因 result.网络地址 为 null 就漏减
                    string ip = result?.网络地址;
                    if (!string.IsNullOrEmpty(ip))
                    {
                        网络服务网关.客户连接计数.AddOrUpdate(ip, 0, (_, c) => Math.Max(0, c - 1));
                    }
                }
            }
            //-----------------------
            while (!网络服务网关.等待添加表.IsEmpty)
            {
                客户网络 result;
                if (网络服务网关.等待添加表.TryDequeue(out result))
                    网络服务网关.网络连接表.Add(result);
            }
            //-----------------------
            while (!网络服务网关.全服公告表.IsEmpty)
            {
                游戏封包 result;
                if (网络服务网关.全服公告表.TryDequeue(out result))
                {
                    foreach (客户网络 客户网络 in 网络服务网关.网络连接表)
                    {
                        if (客户网络.绑定角色 != null)
                            客户网络.发送封包(result);
                    }
                }
            }
        }

        public static void 异步连接(IAsyncResult 异步参数)
        {
            try
            {
                if (网络服务网关.网络服务停止)
                {
                    return;
                }
                TcpClient tcpClient;
                tcpClient = 网络服务网关.网络监听器.EndAcceptTcpClient(异步参数);
                string text;
                text = tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0];
                if (系统数据.数据.网络封禁.ContainsKey(text) && !(系统数据.数据.网络封禁[text] < 主程.当前时间))
                {
                    tcpClient.Client.Close();
                }
                else if (网络服务网关.网络连接表.Count >= 全局最大连接数)
                {
                    // 全局连接到上限, 不再接受新连接 (防止内存/fd 耗尽)
                    tcpClient.Client.Close();
                }
                else if (网络服务网关.客户连接计数.AddOrUpdate(text, 1, (_, c) => c + 1) > 单IP最大连接数)
                {
                    // 该 IP 已超 per-IP 上限, 回滚计数并拒绝
                    网络服务网关.客户连接计数.AddOrUpdate(text, 0, (_, c) => Math.Max(0, c - 1));
                    tcpClient.Client.Close();
                }
                else
                {
                    网络服务网关.等待添加表?.Enqueue(new 客户网络(tcpClient));
                }
            }
            catch (Exception ex)
            {
                主程.添加系统日志("异步连接异常: " + ex.ToString());
            }
            if (!网络服务网关.网络服务停止)
            {
                网络服务网关.网络监听器.BeginAcceptTcpClient(异步连接, null);
            }
        }

        public static void 断网回调(object sender, Exception e)
        {
            客户网络 客户网络2;
            客户网络2 = sender as 客户网络;
            string text;
            text = "IP: " + 客户网络2.网络地址;
            if (客户网络2.绑定账号 != null)
            {
                text = text + " 账号: " + 客户网络2.绑定账号.账号名字.V;
            }
            if (客户网络2.绑定角色 != null)
            {
                text = text + " 角色: " + 客户网络2.绑定角色.对象名字;
            }
            主程.添加系统日志(text + " 信息: " + e.Message);
        }

        public static void 屏蔽网络(string 地址)
        {
            系统数据.数据.封禁网络(地址, 主程.当前时间.AddMinutes((int)Settings.异常屏蔽时间));
        }

        public static void 发送公告(string 内容, bool 滚动播报 = false, bool saveLog = true)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)3);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)144);
                binaryWriter.Write((byte)(滚动播报 ? 2 : 3));
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write(Encoding.UTF8.GetBytes(内容 + "\0"));
                网络服务网关.发送封包(new 接收聊天消息
                {
                    字节描述 = memoryStream.ToArray()
                });
            }
            if (saveLog)
            {
                主程.添加系统日志(内容, hardLog: true, showDiag: false);
            }
        }

        private static void 发送普通提示(玩家实例 玩家实例, string 内容)
        {
            if (玩家实例 == null || string.IsNullOrEmpty(内容))
            {
                return;
            }
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(0);
            binaryWriter.Write(0);
            binaryWriter.Write(1);
            binaryWriter.Write((int)玩家实例.当前等级);
            binaryWriter.Write(Encoding.UTF8.GetBytes(内容 + "\0"));
            binaryWriter.Write(string.Empty);
            binaryWriter.Write((byte)0);
            玩家实例.网络连接?.发送封包(new 接收聊天消息
            {
                字节描述 = memoryStream.ToArray()
            });
        }
        public static void 发送信息(玩家实例 玩家实例, string 内容)
        {
            发送普通提示(玩家实例, 内容);
        }
        public static void 发送封包(游戏封包 封包)
        {
            if (封包 != null)
            {
                网络服务网关.全服公告表?.Enqueue(封包);
            }
        }

        public static void 添加网络(客户网络 网络)
        {
            if (网络 != null)
            {
                网络服务网关.等待添加表.Enqueue(网络);
            }
        }

        public static void 移除网络(客户网络 网络)
        {
            if (网络 != null)
            {
                网络服务网关.等待移除表.Enqueue(网络);
            }
        }
    }
}
