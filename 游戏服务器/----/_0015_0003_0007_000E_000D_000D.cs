using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using _001D_000F_0007_0013_0011_0015;
using 游戏服务器;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using Newtonsoft.Json;

namespace _0015_0003_0007_000E_000D_000D
{
    internal class _0001_0018_000E_0012_0007_0006 : HttpService
    {
        public class roleInfo
        {
            public int id { get; set; }

            public string account { get; set; }

            public string name { get; set; }

            public int level { get; set; }

            public int job { get; set; }

            public int sex { get; set; }

            public int pow { get; set; }

            public uint gold { get; set; }

            public uint bindGold { get; set; }

            public uint gameGold { get; set; }
        }

        private Thread _000C_000F_0003_0004_0014_0019;

        public _0001_0018_000E_0012_0007_0006()
        {
            //base.Host = $"http://+:{Settings.充值监听端口}/";
        }

        public void Start()
        {
            this._000C_000F_0003_0004_0014_0019 = new Thread(base.Listen);
            this._000C_000F_0003_0004_0014_0019.Start();
        }

        public new void Stop()
        {
            base.Stop();
            Thread.Sleep(1000);
        }

        public override void OnPostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string text;
            text = request.Url.PathAndQuery;
            if (text.Contains("?"))
            {
                text = text.Substring(0, text.IndexOf("?", StringComparison.Ordinal)).ToLower();
            }
            try
            {
                Dictionary<string, string> dictionary;
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(HttpUtility.UrlDecode(new StreamReader(request.InputStream, Encoding.UTF8).ReadToEnd()));
                if (dictionary.TryGetValue("sign", out var value))
                {
                    dictionary.Remove("sign");
                    string text2;
                    text2 = _0001_0018_000E_0012_0007_0006.Sign(_0001_0018_000E_0012_0007_0006.http_build_query(dictionary.OrderBy((KeyValuePair<string, string> p) => p.Key).ToDictionary((KeyValuePair<string, string> p) => p.Key, (KeyValuePair<string, string> o) => o.Value)));
                    // 常量时间比较防时序攻击; 同时 Sign() 在密钥未配置时返回固定哨兵, 此比较必然 false.
                    bool 签名有效 = value != null && text2 != null && value.Length == text2.Length
                        && System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                            Encoding.ASCII.GetBytes(value), Encoding.ASCII.GetBytes(text2));
                    if (签名有效)
                    {
                        switch (text)
                        {
                            case "/useCmd":
                                base.waitResponse = response;
                                //主程.AddWebEvent(WebDataType.UseCmd, dictionary, response);
                                break;
                            case "/":
                                base.WriteResponse(response, "");
                                break;
                            case "/paymentcallback":
                                base.waitResponse = response;
                                //主程.AddWebEvent(WebDataType.PayMent, dictionary, response);
                                break;
                            case "/onlineCount":
                                base.WriteResponse(response, 网络服务网关.网络连接表.Count.ToString());
                                break;
                            case "/getRoleList":
                                {
                                    int num;
                                    num = Convert.ToInt32(dictionary["startWith"]);
                                    int num2;
                                    num2 = Convert.ToInt32(dictionary["endWith"]);
                                    if (num > num2)
                                    {
                                        base.WriteResponse(response, "wrong range");
                                        break;
                                    }
                                    List<roleInfo> list;
                                    list = new List<roleInfo>();
                                    for (int i = num; i < num2; i++)
                                    {
                                        if (游戏数据网关.角色数据表.数据表.TryGetValue(i, out var value2) && value2 is 角色数据 角色数据)
                                        {
                                            list.Add(new roleInfo
                                            {
                                                id = i,
                                                account = 角色数据.所属账号.V.账号名字.V,
                                                name = 角色数据.角色名字.V,
                                                level = 角色数据.角色等级,
                                                pow = 角色数据.角色战力,
                                                sex = (int)角色数据.角色性别.V,
                                                job = (int)角色数据.角色职业.V,
                                                gold = 角色数据.金币数量,
                                                bindGold = 角色数据.银币数量,
                                                gameGold = 角色数据.元宝数量
                                            });
                                        }
                                    }
                                    base.WriteResponse(response, JsonConvert.SerializeObject(list));
                                    break;
                                }
                            case "/modifyRole":
                                base.waitResponse = response;
                                //主程.AddWebEvent(WebDataType.ModifyRole, dictionary, response);
                                break;
                            default:
                                base.WriteResponse(response, "error");
                                break;
                            case "/getRoleInfo":
                                break;
                        }
                    }
                    else
                    {
                        base.WriteResponse(response, "sign error");
                    }
                }
                else
                {
                    base.WriteResponse(response, "error");
                }
            }
            catch (Exception ex)
            {
                base.WriteResponse(response, "request error: " + ex);
            }
        }

        public static string returnMd5(string str)
        {
            byte[] array;
            array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(str));
            string text;
            text = "";
            for (int i = 0; i < array.Length; i++)
            {
                text += array[i].ToString("x2");
            }
            return text;
        }

        // HMAC-SHA256 替代原 MD5(str + "&"). 原方案用硬编码 "&" 作密钥, 等同无密钥, 任意人可伪造签名.
        // 改为读取 Settings.充值签名密钥; 若密钥未配置 (空字符串), 返回特殊标记导致后续比对必然失败 -> 安全拒绝.
        public static string Sign(string str)
        {
            if (string.IsNullOrEmpty(游戏服务器.Settings.充值签名密钥))
            {
                return "__signing_key_not_configured__";
            }
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(游戏服务器.Settings.充值签名密钥)))
            {
                byte[] array = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sb = new StringBuilder(array.Length * 2);
                for (int i = 0; i < array.Length; i++)
                {
                    sb.Append(array[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string http_build_query(Dictionary<string, string> dict = null)
        {
            if (dict == null)
            {
                return "";
            }
            string text;
            text = string.Empty;
            foreach (KeyValuePair<string, string> item in dict)
            {
                text = text + item.Key + "=" + item.Value + "&";
            }
            return text.Substring(0, text.Length - 1);
        }

        public override void OnGetRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            Console.WriteLine("GET request: {0}", request.Url);
        }
    }
}
