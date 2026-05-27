using 游戏服务器.地图类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.管理命令
{
    public class 发货 : 商人命令
    {
        [字段描述(0)]
        public string 角色名字;

        [字段描述(1)]
        public uint 元宝数量;

        public override 执行方式 执行方式 => 执行方式.优先后台执行;

        public override void 执行命令()
        {
        }

        public override void 执行命令(玩家实例 商人)
        {
            if (this.角色名字 != null && !(this.角色名字 == string.Empty))
            {
                if (this.角色名字 == 商人.对象名字)
                {
                    商人.发送系统消息("无法对自己操作");
                    主程.添加命令日志("<= @" + base.GetType().Name + " 命令执行失败, 无法对自己操作");
                    return;
                }
                // HIGH-04 整数溢出: 原版只校验 元宝数量 == 0, 未限上界.
                // 攻击者商人输 67_108_864, * 100 = 6.7e9 → uint 回绕 ≈ 2.4e9, 后续接收方
                // 元宝累加再溢出 → 受害者元宝大幅减少. 上限对齐 [[赠送元宝]] 的 uint.MaxValue/100.
                if (this.元宝数量 == 0 || this.元宝数量 > uint.MaxValue / 100u)
                {
                    商人.发送系统消息("错误的数值信息");
                    主程.添加命令日志("<= @" + base.GetType().Name + " 命令执行失败, 错误的数值信息");
                    return;
                }
                uint num;
                num = this.元宝数量 * 100;
                if (商人.元宝数量 < num)
                {
                    商人.发送系统消息("发货失败，你的元宝不够");
                    主程.添加命令日志("<= @" + base.GetType().Name + " 命令执行失败, 发货失败，你的元宝不够");
                    return;
                }
                if (!游戏数据网关.角色数据表.检索表.TryGetValue(this.角色名字, out var value))
                {
                    商人.发送系统消息("玩家[" + this.角色名字 + "]不存在");
                    主程.添加命令日志("<= @" + base.GetType().Name + " 命令执行失败，玩家[" + this.角色名字 + "]不存在");
                    return;
                }
                角色数据 角色数据;
                角色数据 = value as 角色数据;
                玩家实例 玩家实例;
                玩家实例 = 角色数据?.网络连接?.绑定角色;
                // 上限改用 ulong 中间累加, 防止 uint+uint 回绕导致校验失效
                if ((ulong)角色数据.所属账号.V.元宝数量.V + num > int.MaxValue)
                {
                    商人.发送系统消息($"发货失败，超出最大元宝数量，玩家[{this.角色名字}]身上元宝{(float)角色数据.所属账号.V.元宝数量.V / 100f}个.");
                    return;
                }
                if (Settings.商人发货公告 != "")
                {
                    网络服务网关.发送公告(Settings.商人发货公告.Replace("%P%", 角色数据.角色名字.ToString()).Replace("%S%", 商人.ToString()).Replace("%M%", num.ToString()));
                }
                if (玩家实例 == null)
                {
                    商人.发送系统消息($"发货成功，玩家[{this.角色名字}]没有在线，身上元宝{(float)角色数据.所属账号.V.元宝数量.V / 100f}个，发货后元宝{(float)(角色数据.所属账号.V.元宝数量.V + num) / 100f}");
                    角色数据.所属账号.V.元宝数量.V += num;
                    主程.添加货币日志(角色数据, "商人发货增加", 游戏货币.元宝, num);
                    商人.修改货币("-", 游戏货币.元宝, num);
                    主程.添加货币日志(商人, "商人发货扣除", 游戏货币.元宝, num);
                    主程.添加系统日志($"[{商人.对象名字}]=>[{this.角色名字}] [{this.元宝数量}]个元宝");
                    //主程.WebLog(LogDataType.MerchantLog, Settings.统计UUID代码, Settings.游戏区服名称, 商人.对象名字, this.角色名字, 角色数据.所属账号.V.账号名字.V, num.ToString(), 商人.元宝数量.ToString(), "元宝");
                }
                else
                {
                    商人.修改货币("-", 游戏货币.元宝, num);
                    主程.添加货币日志(商人, "商人发货扣除", 游戏货币.元宝, num);
                    商人.发送系统消息($"发货成功，玩家[{this.角色名字}]在线，身上元宝{(float)玩家实例.元宝数量 / 100f}个，发货后元宝{(float)(玩家实例.元宝数量 + num) / 100f}");
                    玩家实例.修改货币("+", 游戏货币.元宝, num);
                    主程.添加货币日志(玩家实例, "商人发货增加", 游戏货币.元宝, num);
                    玩家实例.发送系统消息($"你从[{商人.对象名字}]处获得了[{this.元宝数量}]个元宝");
                    主程.添加系统日志($"[{商人.对象名字}]=>[{this.角色名字}] [{this.元宝数量}]个元宝");
                    //主程.WebLog(LogDataType.MerchantLog, Settings.统计UUID代码, Settings.游戏区服名称, 商人.对象名字, 玩家实例.对象名字, 玩家实例.所属账号.账号名字.V, num.ToString(), 商人.元宝数量.ToString(), "元宝");
                }
            }
            else
            {
                商人.发送系统消息("错误的角色名字");
                主程.添加命令日志("<= @" + base.GetType().Name + " 命令执行失败, 错误的角色名字");
            }
        }
    }
}
