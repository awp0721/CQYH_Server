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
        public static void 请求创建角色(创角请求 Q)
        {
            主程.创角请求列表.Enqueue(Q);
        }
        /*
        public static void 返回创建角色(创角请求 Q)
        {
            主程.创角返回列表.Enqueue(Q);
        }
        
        public static int HttpPost2(string url, string sendData, out string reslut)
        {
            reslut = "";
            try
            {
                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(sendData);
                HttpWebRequest httpWebRequest;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Proxy = null;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.ContentLength = bytes.Length;
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                using Stream stream2 = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
                using StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8);
                reslut = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                reslut = ex.Message;
                return -1;
            }
            return 0;
        }

        public static async Task<string> HttpPostAsync(string url, string sendData)
        {
            _ = string.Empty;
            string result;
            try
            {
                Encoding.UTF8.GetBytes(sendData);
                if (主程.Http == null)
                {
                    主程.Http = new HttpClient
                    {
                        Timeout = TimeSpan.FromSeconds(30.0)
                    };
                }
                HttpResponseMessage obj;
                obj = await 主程.Http.PostAsync(url, new StringContent(sendData));
                obj.EnsureSuccessStatusCode();
                result = await obj.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return result;
        }
        */
        //public static async Task 处理创建角色请求()
        public static Task 处理创建角色请求() //async
        {
            if (创角请求列表.IsEmpty)
            {
                Thread.Sleep(500);
                return Task.CompletedTask;
            }
            string result;
            result = "";
            try
            {
                if (创角请求列表.TryDequeue(out var Q) && Q != null)
                {
                    添加系统日志($"创建角色请求:{Q}", hardLog: true, showDiag: false);
                    Q.成功 = true;
                    /*
                    result = await 主程.HttpPostAsync("https://pay.tengcanol.com/admin/site/clientRegister", JsonConvert.SerializeObject(new
                    {
                        inviteCode = Q.推荐人,
                        account = Q.账号,
                        areaname = Settings.游戏区服名称
                    }));
                    */
                    添加系统日志("创建角色请求:clientRegister = " + result, hardLog: true, showDiag: false);
                    /*
                    result = await 主程.HttpPostAsync("https://pay.tengcanol.com/admin/site/clientRegisterRole", JsonConvert.SerializeObject(new
                    {
                        uuid = Settings.统计UUID代码,
                        areaname = Settings.游戏区服名称,
                        account = Q.账号,
                        roleName = Q.名字
                    }));
                    */
                    添加系统日志("创建角色请求:clientRegisterRole = " + result, hardLog: true, showDiag: false);
                    /*
                    创角返回 创角返回;
                    
                    创角返回 = JsonConvert.DeserializeObject<创角返回>(result, new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        TypeNameHandling = TypeNameHandling.Auto,
                        Formatting = Formatting.Indented
                    });
                    
                    添加系统日志($"创建角色请求:rc.code = {创角返回.code}", hardLog: true, showDiag: false);
                    */
                }
            }
            catch (Exception ex)
            {
                添加系统日志("创建角色请求 Error:" + result + " " + ex.Message + "\r\n" + ex.StackTrace);
            }

            return Task.CompletedTask;
        }

        public static void 返回客户端创建角色()
        {
        }
    }
}
