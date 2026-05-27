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
        private static void 处理网页事件()
        {
            if (!主程.WebEvent.IsEmpty && 主程.WebEvent.TryDequeue(out var result))
            {
                switch (result.Type)
                {
                    case WebDataType.PayMent:
                        主程.WebPayMent(result);
                        break;
                    case WebDataType.ModifyRole:
                        主程.WebModifyRole(result);
                        break;
                    case WebDataType.UseCmd:
                        主程.WebUseCmd(result);
                        break;
                }
            }
        }

        public static void WebModifyRole(WebData webData)
        {
            if (webData.Data.ContainsKey("roleId") && int.TryParse(webData.Data["roleId"], out var result))
            {
                if (webData.Data.ContainsKey("type") && int.TryParse(webData.Data["type"], out var _))
                {
                    游戏数据 value;
                    if (!webData.Data.ContainsKey("value"))
                    {
                        HttpService.Return(webData.Respons, "wrong param 'value'");
                    }
                    else if (游戏数据网关.角色数据表.数据表.TryGetValue(result, out value) && value is 角色数据)
                    {
                        HttpService.Return(webData.Respons, "wrong type");
                        HttpService.Return(webData.Respons, "success");
                    }
                    else
                    {
                        HttpService.Return(webData.Respons, "not find role");
                    }
                }
                else
                {
                    HttpService.Return(webData.Respons, "wrong param 'type'");
                }
            }
            else
            {
                HttpService.Return(webData.Respons, "wrong param 'roleId'");
            }
        }

        public static void WebUseCmd(WebData webData)
        {
            if (!webData.Data.ContainsKey("cmd"))
            {
                HttpService.Return(webData.Respons, "wrong param 'cmd'");
                return;
            }
            string text;
            text = webData.Data["cmd"];
            主程.添加命令日志("=> " + text);
            if (text[0] != '@')
            {
                HttpService.Return(webData.Respons, "<= 命令解析错误, GM命令必须以 '@' 开头. 输入 '@查看命令' 获取所有受支持的命令格式");
                主程.添加命令日志("<= 命令解析错误, GM命令必须以 '@' 开头. 输入 '@查看命令' 获取所有受支持的命令格式");
            }
            else if (text.Trim('@', ' ').Length == 0)
            {
                HttpService.Return(webData.Respons, "<= 命令解析错误, GM命令不能为空. 输入 '@查看命令' 获取所有受支持的命令格式");
                主程.添加命令日志("<= 命令解析错误, GM命令不能为空. 输入 '@查看命令' 获取所有受支持的命令格式");
            }
            else
            {
                if (!GM命令.解析命令(text, out var 命令))
                {
                    return;
                }
                if (命令.执行方式 == 执行方式.前台立即执行)
                {
                    命令.执行命令();
                    HttpService.Return(webData.Respons, "success");
                }
                else if (命令.执行方式 == 执行方式.优先后台执行)
                {
                    if (主程.已经启动)
                    {
                        主程.外部命令.Enqueue(命令);
                        HttpService.Return(webData.Respons, "success");
                    }
                    else
                    {
                        命令.执行命令();
                    }
                }
                else if (命令.执行方式 == 执行方式.只能后台执行)
                {
                    if (主程.已经启动)
                    {
                        主程.外部命令.Enqueue(命令);
                        HttpService.Return(webData.Respons, "success");
                    }
                    else
                    {
                        HttpService.Return(webData.Respons, "<= 命令执行失败, 当前命令只能在服务器运行时执行, 请先启动服务器");
                        主程.添加命令日志("<= 命令执行失败, 当前命令只能在服务器运行时执行, 请先启动服务器");
                    }
                }
                else if (命令.执行方式 == 执行方式.只能空闲执行)
                {
                    if (!主程.已经启动 && (主程.主线程 == null || !主程.主线程.IsAlive))
                    {
                        命令.执行命令();
                        HttpService.Return(webData.Respons, "success");
                    }
                    else
                    {
                        HttpService.Return(webData.Respons, "<= 命令执行失败, 当前命令只能在服务器未运行时执行, 请先关闭服务器");
                        主程.添加命令日志("<= 命令执行失败, 当前命令只能在服务器未运行时执行, 请先关闭服务器");
                    }
                }
            }
        }

        public static void WebPayMent(WebData webData)
        {
            int result;
            if (!webData.Data.ContainsKey("account"))
            {
                HttpService.Return(webData.Respons, "wrong param 'account'");
            }
            else if (webData.Data.ContainsKey("money") && int.TryParse(webData.Data["money"], out result))
            {
                if (webData.Data.ContainsKey("amount") && uint.TryParse(webData.Data["amount"], out var result2))
                {
                    int result3;
                    result3 = 0;
                    游戏数据 value2;
                    if (!webData.Data.TryGetValue("encourage", out var value) && !int.TryParse(value, out result3))
                    {
                        HttpService.Return(webData.Respons, "wrong param 'encourageStr'");
                    }
                    else if (游戏数据网关.角色数据表.检索表.TryGetValue(webData.Data["account"], out value2) && value2 is 角色数据 角色数据)
                    {
                        uint num;
                        num = 100u;
                        if (Settings.充值货币类型 == 0)
                        {
                            uint num2;
                            num2 = result2 * num;
                            角色数据.元宝数量 += num2;
                            角色数据.累计充值.V += result;
                            角色数据.今日充值.V += result;
                            主程.添加货币日志(角色数据, "玩家充值元宝", 游戏货币.元宝, num2);
                            角色数据.网络连接?.发送封包(new 同步元宝数量
                            {
                                元宝数量 = 角色数据.元宝数量
                            });
                            主程.添加系统日志($"{角色数据.角色名字} 充值[{result}元],赠送{result3}, 当前元宝: {角色数据.元宝数量}");
                            HttpService.Return(webData.Respons, "success");
                            //主程.WebLog(LogDataType.WebsiteRechargeLog, Settings.统计UUID代码, Settings.游戏区服名称, "", 角色数据.角色名字.V, 角色数据.所属账号.V.账号名字.V, result2.ToString(), result.ToString(), "元宝");
                            充值奖励.来钱了(角色数据, (uint)result);
                            if (Settings.充值公告 != "")
                            {
                                网络服务网关.发送公告(Settings.充值公告.Replace("%P%", 角色数据.角色名字.ToString()).Replace("%M%", num2.ToString()));
                            }
                        }
                        else if (Settings.充值货币类型 == 1)
                        {
                            uint num3;
                            num3 = ((num > 100) ? (result2 * num / 100) : result2);
                            角色数据.银币数量 += num3;
                            角色数据.累计充值.V += result;
                            角色数据.今日充值.V += result;
                            主程.添加货币日志(角色数据, "玩家充值银币", 游戏货币.银币, num3);
                            角色数据.网络连接?.发送封包(new 货币数量变动
                            {
                                货币类型 = 0,
                                货币数量 = 角色数据.银币数量
                            });
                            主程.添加系统日志($"{角色数据.角色名字} 充值[{result}元],赠送{result3}, 当前银币: {角色数据.银币数量}");
                            HttpService.Return(webData.Respons, "success");
                            //主程.WebLog(LogDataType.WebsiteRechargeLog, Settings.统计UUID代码, Settings.游戏区服名称, "", 角色数据.角色名字.V, 角色数据.所属账号.V.账号名字.V, result2.ToString(), result.ToString(), "银币");
                            充值奖励.来钱了(角色数据, (uint)result);
                            if (Settings.充值公告 != "")
                            {
                                网络服务网关.发送公告(Settings.充值公告.Replace("%P%", 角色数据.角色名字.ToString()).Replace("%M%", num3.ToString()));
                            }
                        }
                    }
                    else
                    {
                        主程.添加系统日志($"{webData.Data["account"]}尝试充值[{result}]元,赠送{result3}，但是没有找到此玩家");
                        HttpService.Return(webData.Respons, "not find role'");
                    }
                }
                else
                {
                    HttpService.Return(webData.Respons, "wrong param 'amount'");
                }
            }
            else
            {
                HttpService.Return(webData.Respons, "wrong param 'money'");
            }
        }
    }
}
