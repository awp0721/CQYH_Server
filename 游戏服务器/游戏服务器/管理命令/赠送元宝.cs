using 游戏服务器.地图类;
using 游戏服务器.数据类;

namespace 游戏服务器.管理命令
{
    public class 赠送元宝 : 玩家命令
    {
        [字段描述(0)]
        public string 角色名字;

        [字段描述(1)]
        public uint 元宝数量;

        public override 执行方式 执行方式 => 执行方式.优先后台执行;

        public override void 执行命令()
        {
        }

        public override void 执行命令(玩家实例 玩家)
        {
            if (this.角色名字 != null && !(this.角色名字 == string.Empty))
            {
                if (this.角色名字 == 玩家.对象名字)
                {
                    玩家.发送系统消息("无法对自己操作");
                    return;
                }
                if (this.元宝数量 == 0 || this.元宝数量 > uint.MaxValue / 100u)
                {
                    玩家.发送系统消息("错误的数值信息");
                    return;
                }
                uint num;
                num = this.元宝数量 * 100u;
                if (玩家.元宝数量 < num)
                {
                    玩家.发送系统消息("元宝不足");
                    return;
                }
                if (!游戏数据网关.角色数据表.检索表.TryGetValue(this.角色名字, out var value))
                {
                    玩家.发送系统消息("[" + this.角色名字 + "]不存在");
                    return;
                }
                角色数据 角色数据;
                角色数据 = value as 角色数据;
                if (角色数据.商人角色.V)
                {
                    玩家实例 玩家实例;
                    玩家实例 = 角色数据?.网络连接?.绑定角色;
                    if (角色数据.所属账号.V.元宝数量.V + num > int.MaxValue)
                    {
                        玩家.发送系统消息("超出[" + this.角色名字 + "]身上元宝最大元宝数量，本次操作不会扣除您的元宝，请与被赠与方联系");
                        return;
                    }
                    if (玩家实例 == null)
                    {
                        玩家.发送系统消息("[" + this.角色名字 + "]不在线，无法赠送");
                        return;
                    }
                    玩家.修改货币("-", 游戏货币.元宝, num);
                    玩家.发送系统消息($"成功赠送给[{this.角色名字}]{this.元宝数量}个元宝");
                    主程.添加货币日志(玩家, "玩家赠送元宝", 游戏货币.元宝, num);
                    玩家实例.修改货币("+", 游戏货币.元宝, num);
                    玩家实例.发送系统消息($"你从[{玩家.对象名字}]处获得了[{this.元宝数量}]个元宝");
                    主程.添加货币日志(玩家实例, "玩家获赠元宝", 游戏货币.元宝, num);
                    主程.添加系统日志($"[{玩家.对象名字}]=>[{this.角色名字}] [{this.元宝数量}]个元宝");
                    //主程.WebLog(LogDataType.MerchantRecyLog, Settings.统计UUID代码, Settings.游戏区服名称, 玩家实例.对象名字, 玩家.对象名字, 玩家.所属账号.账号名字.V, num.ToString(), 玩家实例.元宝数量.ToString(), "元宝");
                }
            }
            else
            {
                玩家.发送系统消息("错误的角色名字");
            }
        }
    }
}
