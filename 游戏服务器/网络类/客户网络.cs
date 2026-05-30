using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using 游戏服务器.地图类;
using 游戏服务器.模板类;
using 游戏服务器.数据类;

namespace 游戏服务器.网络类
{
    public sealed class 客户网络
    {
        private DateTime 断开时间;

        private bool 正在发送;

        private byte[] 剩余数据;

        private readonly EventHandler<Exception> 断网事件;

        private ConcurrentQueue<游戏封包> 接收列表;

        private ConcurrentQueue<游戏封包> 发送列表;

        public bool 正在断开;

        public bool 已经获取IP;

        public readonly DateTime 接入时间;

        public readonly TcpClient 当前连接;

        public 游戏阶段 当前阶段;

        public 账号数据 绑定账号;

        public 玩家实例 绑定角色;

        public string 网络地址;

        public string 物理地址;

        public int 发送总数;

        public int 接收总数;

        public 客户网络(TcpClient 客户端)
        {
            this.已经获取IP = false;
            this.剩余数据 = new byte[0];
            this.接收列表 = new ConcurrentQueue<游戏封包>();
            this.发送列表 = new ConcurrentQueue<游戏封包>();
            this.当前连接 = 客户端;
            this.当前连接.NoDelay = true;
            this.接入时间 = 主程.当前时间;
            this.断开时间 = 主程.当前时间.AddMinutes((int)Settings.掉线判定时间);
            this.断网事件 = (EventHandler<Exception>)Delegate.Combine(this.断网事件, new EventHandler<Exception>(网络服务网关.断网回调));
            this.网络地址 = this.当前连接.Client.RemoteEndPoint.ToString().Split(':')[0];
            this.开始异步接收();
        }

        public void 处理数据()
        {
            try
            {
                if (!this.正在断开 && !网络服务网关.网络服务停止)
                {
                    if (主程.当前时间 > this.断开时间)
                    {
                        this.尝试断开连接(new Exception("网络长时间无回应, 断开连接."));
                        return;
                    }
                    this.处理已收封包();
                    this.发送全部封包();
                }
                else if (!this.正在发送 && this.接收列表.Count == 0 && this.发送列表.Count == 0)
                {
                    this.绑定角色?.玩家角色下线();
                    this.绑定账号?.账号下线();
                    网络服务网关.移除网络(this);
                    this.当前连接.Client.Shutdown(SocketShutdown.Both);
                    this.当前连接.Close();
                    this.接收列表 = null;
                    this.发送列表 = null;
                    this.当前阶段 = 游戏阶段.正在登录;
                }
                else
                {
                    this.处理已收封包();
                    this.发送全部封包();
                }
            }
            catch (Exception ex)
            {
                string[] array;
                object obj;
                if (this.绑定角色 != null)
                {
                    array = new string[12]
                    {
                        "处理网络数据时出现异常, 已断开对应连接\r\n账号:[", null, null, null, null, null, null, null, null, null,
                        null, null
                    };
                    账号数据 账号数据;
                    账号数据 = this.绑定账号;
                    if (账号数据 == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        obj = 账号数据.账号名字.V;
                        if (obj != null)
                        {
                            goto IL_011a;
                        }
                    }
                    obj = "无";
                    goto IL_011a;
                }
                goto IL_01ce;
            IL_011a:
                array[1] = (string)obj;
                array[2] = "]\r\n角色:[";
                玩家实例 玩家实例;
                玩家实例 = this.绑定角色;
                object obj2;
                if (玩家实例 == null)
                {
                    obj2 = null;
                }
                else
                {
                    obj2 = 玩家实例.对象名字;
                    if (obj2 != null)
                    {
                        goto IL_014f;
                    }
                }
                obj2 = "无";
                goto IL_014f;
            IL_014f:
                array[3] = (string)obj2;
                array[4] = "]\r\n网络地址:[";
                array[5] = this.网络地址;
                array[6] = "]\r\n物理地址:[";
                array[7] = this.物理地址;
                array[8] = "]\r\n错误提示:";
                array[9] = ex.Message;
                array[10] = "]\r\n错误提示2:" + ex.InnerException?.Message + "\r\n堆栈信息:";
                array[11] = ex.ToString();
                主程.添加系统日志(string.Concat(array));
                goto IL_01ce;
            IL_01ce:
                if (this.绑定角色 == null || !this.绑定角色.管理员模式)
                {
                    this.绑定角色?.角色数据.发送邮件(null, "重要通知", "您可能因特殊的异常导致掉线，请联系管理员并告知掉线前您的具体游戏操作", null);
                    this.绑定角色?.玩家角色下线();
                    this.绑定账号?.账号下线();
                    网络服务网关.移除网络(this);
                    this.当前连接.Client?.Shutdown(SocketShutdown.Both);
                    this.当前连接?.Close();
                    this.接收列表 = null;
                    this.发送列表 = null;
                    this.当前阶段 = 游戏阶段.正在登录;
                }
            }
        }

        public void 发送封包(游戏封包 封包)
        {
            if (!this.正在断开 && !网络服务网关.网络服务停止 && 封包 != null)
            {
                this.发送列表.Enqueue(封包);
            }
        }

        public void SendRaw(ushort type, ushort length, byte[] data, bool encoded = true)
        {
            byte[] array;
            if (length == 0)
            {
                array = new byte[data.Length + 4];
                Array.Copy(BitConverter.GetBytes(type), 0, array, 0, 2);
                Array.Copy(BitConverter.GetBytes((ushort)array.Length), 0, array, 2, 2);
                Array.Copy(data, 0, array, 4, data.Length);
            }
            else
            {
                array = new byte[data.Length + 2];
                Array.Copy(BitConverter.GetBytes(type), 0, array, 0, 2);
                Array.Copy(data, 0, array, 2, data.Length);
            }
            if (encoded)
            {
                for (int i = 4; i < array.Length; i++)
                {
                    array[i] ^= 游戏封包.加密字节;
                }
            }
            this.当前连接.Client.Send(array);
        }

        public void 尝试断开连接(Exception e)
        {
            if (!this.正在断开)
            {
                this.正在断开 = true;
                this.断网事件?.Invoke(this, e);
            }
        }

        public void 错误消息提示(Exception e)
        {
            this.断网事件?.Invoke(this, e);
        }

        private void 处理已收封包()
        {
            while (true)
            {
                if (this.接收列表.IsEmpty)
                {
                    return;
                }
                if (this.接收列表.Count > Settings.封包限定数量)
                {
                    break;
                }
                if (this.接收列表.TryDequeue(out var result))
                {
                    if (!游戏封包.封包处理方法表.TryGetValue(result.封包类型, out var value))
                    {
                        this.尝试断开连接(new Exception("没有找到封包处理方法, 断开连接. 封包类型: " + result.封包类型.FullName));
                        return;
                    }
                    value.Invoke(this, new object[1] { result });
                }
            }
            this.接收列表 = new ConcurrentQueue<游戏封包>();
            this.尝试断开连接(new Exception("封包过多, 断开连接并限制登录."));
        }

        private void 发送全部封包()
        {
            if (Settings.开启线程发包)
            {
                Task.Run(delegate
                {
                    List<byte> list2;
                    list2 = new List<byte>();
                    while (!this.发送列表.IsEmpty)
                    {
                        if (this.发送列表.TryDequeue(out var result2))
                        {
                            list2.AddRange(result2.取字节());
                        }
                    }
                    if (list2.Count != 0)
                    {
                        this.开始同步发送(list2);
                    }
                });
                return;
            }
            List<byte> list;
            list = new List<byte>();
            while (!this.发送列表.IsEmpty)
            {
                if (this.发送列表.TryDequeue(out var result))
                {
                    list.AddRange(result.取字节());
                }
            }
            if (list.Count != 0)
            {
                this.开始异步发送(list);
            }
        }

        private void 延迟掉线时间()
        {
            this.断开时间 = 主程.当前时间.AddMinutes((int)Settings.掉线判定时间);
        }

        private void 开始异步接收()
        {
            try
            {
                if (!this.正在断开 && !网络服务网关.网络服务停止)
                {
                    byte[] array;
                    array = new byte[8192];
                    this.当前连接.Client.BeginReceive(array, 0, array.Length, SocketFlags.None, 接收完成回调, array);
                }
            }
            catch (Exception ex)
            {
                this.尝试断开连接(new Exception("异步接收错误 : " + ex.Message));
            }
        }

        private void 接收完成回调(IAsyncResult 异步参数)
        {
            try
            {
                if (this.正在断开 || 网络服务网关.网络服务停止 || this.当前连接.Client == null)
                {
                    return;
                }
                int num;
                num = this.当前连接.Client?.EndReceive(异步参数) ?? 0;
                if (num > 0)
                {
                    this.接收总数 += num;
                    网络服务网关.已接收字节数 += num;
                    byte[] src;
                    src = 异步参数.AsyncState as byte[];
                    byte[] dst;
                    dst = new byte[this.剩余数据.Length + num];
                    Buffer.BlockCopy(this.剩余数据, 0, dst, 0, this.剩余数据.Length);
                    Buffer.BlockCopy(src, 0, dst, this.剩余数据.Length, num);
                    this.剩余数据 = dst;
                    while (true)
                    {
                        try
                        {
                            游戏封包 游戏封包2;
                            游戏封包2 = 游戏封包.取封包(this, this.剩余数据, out this.剩余数据);
                            if (游戏封包2 == null)
                            {
                                break;
                            }
                            this.接收列表.Enqueue(游戏封包2);
                            continue;
                        }
                        catch (Exception e)
                        {
                            this.尝试断开连接(e);
                        }
                        break;
                    }
                    this.延迟掉线时间();
                    this.开始异步接收();
                }
                else
                {
                    this.尝试断开连接(new Exception("客户端断开连接."));
                }
            }
            catch (Exception ex)
            {
                this.尝试断开连接(new Exception("封包构建错误, 错误提示: " + ex.Message));
            }
        }

        private void 开始异步发送(List<byte> 数据)
        {
            try
            {
                this.正在发送 = true;
                this.当前连接.Client.BeginSend(数据.ToArray(), 0, 数据.Count, SocketFlags.None, 发送完成回调, null);
            }
            catch (Exception ex)
            {
                this.正在发送 = false;
                this.发送列表 = new ConcurrentQueue<游戏封包>();
                this.尝试断开连接(new Exception("异步发送错误 : " + ex.Message));
            }
        }

        private void 开始同步发送(List<byte> 数据)
        {
            try
            {
                this.正在发送 = true;
                this.当前连接.Client.Send(数据.ToArray(), 0, 数据.Count, SocketFlags.None);
                this.正在发送 = false;
            }
            catch (Exception ex)
            {
                this.正在发送 = false;
                this.发送列表 = new ConcurrentQueue<游戏封包>();
                this.尝试断开连接(new Exception("同步发送错误 : " + ex.Message));
            }
        }

        private void 发送完成回调(IAsyncResult 异步参数)
        {
            try
            {
                int num;
                num = this.当前连接.Client.EndSend(异步参数);
                this.发送总数 += num;
                网络服务网关.已发送字节数 += num;
                if (num == 0)
                {
                    this.发送列表 = new ConcurrentQueue<游戏封包>();
                    this.尝试断开连接(new Exception("发送回调错误!"));
                }
                this.正在发送 = false;
            }
            catch (Exception ex)
            {
                this.正在发送 = false;
                this.发送列表 = new ConcurrentQueue<游戏封包>();
                this.尝试断开连接(new Exception("发送回调错误 : " + ex.Message));
            }
        }


        public void 处理封包(玩家开始挖矿 P)
        {
            /*
            byte[] obj;
            obj = new byte[10] { 0, 0, 0, 0, 48, 107, 208, 36, 101, 3 };
            obj[0] = (byte)((uint)this.绑定角色.角色数据.角色编号 & 0xFFu);
            obj[1] = (byte)((uint)(this.绑定角色.角色数据.角色编号 >> 8) & 0xFFu);
            obj[2] = (byte)((uint)(this.绑定角色.角色数据.角色编号 >> 16) & 0xFFu);
            obj[3] = (byte)((uint)(this.绑定角色.角色数据.角色编号 >> 24) & 0xFFu);
            this.SendRaw(59, 12, obj);
			*/
            if (当前阶段 != 游戏阶段.正在游戏)
            {
                尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{当前阶段}"));
                return;
            }
            else
            {
                绑定角色.玩家开始挖矿(P.挖掘坐标);
            }

        }

        #region 挖矿
        /*
        public void 处理封包(玩家开始挖矿 P)
        {
            if (当前阶段 != 游戏阶段.正在游戏)
            {
                尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{当前阶段}"));
                return;
            }
            else
            {
                绑定角色.玩家开始挖矿(P.挖掘坐标);
            }
        }
		*/
        public void 处理封包(玩家挖矿成功 P)
        {
            if (当前阶段 != 游戏阶段.正在游戏)
            {
                尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{当前阶段}"));
                return;
            }
            else
            {
                绑定角色.玩家挖矿成功(P.编号, P.坐标, P.动作间隔);
            }
        }
        public void 处理封包(玩家挖矿失败 P)
        {
            if (当前阶段 != 游戏阶段.正在游戏)
            {
                尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{当前阶段}"));
                return;
            }
            else
            {
                绑定角色.玩家挖矿失败(P.玩家坐标, P.高度);
            }
        }
        #endregion
        // PROTO-04: 以下处理器原本完全无阶段守卫, 统一加 阶段+绑定角色 检查
        public void 处理封包(任务传送封包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.传送任务点(P.任务编号);
        }

        public void 处理封包(普通升级取回 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家取回装备(0);
        }

        public void 处理封包(普通快速取回 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家取回装备(100000);
        }

        public void 处理封包(高级升级取回 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家取回装备(0);
        }

        public void 处理封包(高级快速取回 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家取回装备(100000);
        }

        public void 处理封包(装备铭文刻印 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家铭文刻印(P.装备部位, P.物品编号, P.铭文索引);
        }

        public void 处理封包(角色外观易容 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家外观易容(P.角色发型, P.角色发色, P.角色脸型, P.未知参数);
        }

        public void 处理封包(玩家合无相石 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家合无相石((byte)P.物品位置, P.一键合成 == 1);
        }

        public void 处理封包(挑战无相秘境 P)
        {
        }

        public void 处理封包(鉴定无相钥石 P)
        {
            // PROTO-04 续: 原无认证 → 未登录客户端可触发服务端发包, 浪费带宽与统计.
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.SendRaw(130, 4, new byte[2] { 1, P.背包位置 });
            byte[] obj;
            obj = new byte[80]
            {
                14, 3, 48, 130, 2, 215, 162, 244, 100, 33,
                148, 30, 0, 1, 0, 1, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
                3, 0, 40, 0, 67, 23, 0, 0, 44, 0,
                196, 11, 0, 0, 201, 11, 0, 0, 197, 11,
                0, 0, 17, 39, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            obj[14] = P.背包位置;
            this.SendRaw(128, 0, obj);
            this.SendRaw(364, 3, new byte[1] { P.背包位置 });
        }

        public void 处理封包(请求武技签到 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.传永武技签到();
        }

        public void 处理封包(Unknown0D P)
        {
        }

        public void 处理封包(UnknownC266 P)
        {
        }

        public void 处理封包(获取真实IP地址 P)
        {
            // PROTO-08: 客户端可伪造此封包覆盖服务端记录的网络地址, 用于日志欺骗 / 绕过基于 IP 的限速封禁.
            // socket 的 RemoteEndPoint 才是权威来源, 这里直接丢弃客户端自报值.
            // 若存在 CDN/反向代理需求, 应由可信前端列表 + 协议层签名重新引入.
        }

        public void 处理封包(内挂物品过滤 P)
        {
            // PROTO-04 续: 原无认证 → 攻击者未登录就能让服务端构造 BitArray + 多次哈希查表, 轻量 DoS.
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色?.物品过滤.Clear();
            this.绑定角色?.物品极品提示.Clear();
            BitArray bitArray;
            bitArray = new BitArray(P.字节描述);
            ushort num;
            num = (ushort)(bitArray.Length / 2);
            for (ushort num2 = 0; num2 < bitArray.Length; num2++)
            {
                物品过滤 value2;
                if (num2 < num)
                {
                    if (bitArray[num2] && 物品过滤.数据表.TryGetValue(num2, out var value))
                    {
                        foreach (int item in value.物品编号)
                        {
                            this.绑定角色?.物品过滤.Add(item);
                        }
                    }
                }
                else if (bitArray[num2] && 物品过滤.数据表.TryGetValue((ushort)(num2 - num), out value2))
                {
                    foreach (int item2 in value2.物品编号)
                    {
                        this.绑定角色?.物品极品提示.Add(item2);
                    }
                }
            }
        }

        public void 处理封包(UnknownC642 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            this.绑定角色.玩家进入场景();
            this.当前阶段 = 游戏阶段.正在游戏;
        }

        public void 处理封包(UnknownC644 P)
        {
        }

        public void 处理封包(特权引导寻路 P)
        {
            // C17: 补阶段守卫. 原缺守卫, 阶段0(未登录)即可探测守卫NPC坐标并诱发查表/回包.
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (地图处理网关.地图实例表.TryGetValue(P.地图编号 * 16 + 1, out var value))
            {
                守卫刷新 守卫刷新;
                守卫刷新 = value.守卫区域.Where((守卫刷新 o) => o.守卫编号 == P.对象编号).FirstOrDefault();
                if (守卫刷新 != null)
                {
                    this.发送封包(new 查询NPC位置
                    {
                        地图编号 = P.地图编号,
                        对象编号 = P.对象编号,
                        状态标志 = 1,
                        X = 计算类.点阵坐标转协议坐标(守卫刷新.所处坐标.X),
                        Y = 计算类.点阵坐标转协议坐标(守卫刷新.所处坐标.Y),
                        Z = 计算类.点阵坐标转协议坐标(value.地形高度(守卫刷新.所处坐标))
                    });
                    return;
                }
            }
            this.发送封包(new 查询NPC位置
            {
                地图编号 = P.地图编号,
                对象编号 = P.对象编号,
                状态标志 = 0,
                X = 0,
                Y = 0,
                Z = 0
            });
        }

        public void 处理封包(敏感字符过滤 P)
        {
        }

        public void 处理封包(UnknownC155 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.网络连接?.发送封包(new 成就完成通知
            {
                U1 = P.U1,
                U2 = 19225
            });
        }

        public void 处理封包(Unknown027D P)
        {
        }

        // PROTO-04: 以下 8 个处理器原本无认证检查, 未登录客户端伪造封包就能触发业务逻辑
        // 或对 this.绑定角色 解引用 → 空指针崩溃. 统一加阶段 / 绑定角色 守卫.
        public void 处理封包(传奇之力激活 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.激活传奇之力();
        }

        public void 处理封包(UnknownC272 P)
        {
        }

        public void 处理封包(请求战功信息 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.发送战功详情();
        }

        public void 处理封包(请求战功任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.发送战功任务(P.任务类型);
        }

        public void 处理封包(领取战功奖励 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.领取战功奖励(P.领取类型);
        }

        public void 处理封包(购买战功军令 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.购买战功军令();
        }

        public void 处理封包(购买战功积分 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.购买战功积分(P.购买类型);
        }

        public void 处理封包(购买主题礼包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家购买主题礼包(P.日期序号, P.物品A_ID, P.物品B_ID, P.物品C_ID, P.物品D_ID);
        }

        public void 处理封包(请求主题礼包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.玩家请求主题礼包();
        }

        public void 处理封包(装备开启精炼 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.装备开启精炼(P.背包类型, P.背包位置);
        }

        public void 处理封包(装备重新精炼 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.装备重新精炼(P.背包类型, P.背包位置, P.材料类型, P.材料位置, P.特殊标记);
        }

        public void 处理封包(装备精炼替换 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.装备精炼替换(P.背包类型, P.背包位置);
        }

        public void 处理封包(装备转移精炼 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.装备转移精炼(P.背包类型, P.背包位置, P.材料类型, P.材料位置);
        }

        public void 处理封包(升级玛法特权 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.升级玛法特权(P.特权编号);
        }

        public void 处理封包(UnknownC255 P)
        {
        }

        public void 处理封包(打开坐骑面板 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else if (this.绑定角色.坐骑列表.Contains(P.编号))
            {
                this.发送封包(new 坐骑面板回执
                {
                    编号 = P.编号
                });
                this.绑定角色.当前坐骑 = P.编号;
            }
        }

        public void 处理封包(坐骑御兽拖动 P)
        {
            byte value;
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else if (this.绑定角色 != null && this.绑定角色.坐骑列表.Contains(P.坐骑编号) && 游戏坐骑.御兽之力栏数.TryGetValue((byte)this.绑定角色.御兽之力等级, out value) && value >= P.御兽栏位)
            {
                this.绑定角色.御兽列表[P.御兽栏位] = P.坐骑编号;
                this.发送封包(new 游戏错误提示
                {
                    错误代码 = 2056,
                    第一参数 = P.御兽栏位,
                    第二参数 = P.坐骑编号
                });
            }
        }

        public void 处理封包(注入天赋之力 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.注入天赋之力((byte)(P.天赋位置 * 5 + 30), P.未知参数);
            }
        }

        public void 处理封包(天赋突破升级 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.天赋突破升级((byte)(P.天赋位置 * 5 + 30));
            }
        }

        public void 处理封包(激活天赋刻印 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.激活天赋刻印((byte)(P.天赋位置 * 5 + 30), P.刻印位置);
            }
        }

        public void 处理封包(暂停自动战斗 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"Phase exception, disconnected.  Processing packet: {P.GetType()}, Current phase: {this.当前阶段}"));
                return;
            }
            else if (!Settings.开启自动战斗)
            {
                this.绑定角色.自动战斗 = false;
            }
            else
            {
                this.绑定角色.自动挂机状态变更(P);
                this.绑定角色.自动战斗 = P.自动战斗 == 1;
                this.发送封包(new 自动战斗回执
                {
                    开关状态 = P.自动战斗
                });
            }
        }

        public void 处理封包(开始自动战斗 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"Phase exception, disconnected.  Processing packet: {P.GetType()}, Current phase: {this.当前阶段}"));
                return;
            }
            if (!Settings.开启自动战斗)
            {
                this.绑定角色.发送顶部公告("正在进行技术调试，自动战斗功能暂不开放");
                return;
            }
            if (Settings.使用新版内挂)
            {
                this.绑定角色.玩家开始自动挂机(P);
                this.发送封包(new 自动战斗回执
                {
                    开关状态 = (P.自动战斗 ? 1 : 0)
                });
            }
            if (this.绑定角色.自动战斗 != P.自动战斗)
            {
                this.绑定角色.发送顶部公告(P.自动战斗 ? "<#T:1000100>" : "<#T:1000101>");
                this.绑定角色.攻击目标 = null;
                this.绑定角色.自动战斗 = P.自动战斗;
                this.绑定角色.启动位置 = this.绑定角色.当前坐标;
                this.绑定角色.挂机范围 = P.战斗范围;
                this.绑定角色.开启收益检测 = P.开启空闲使用道具;
                this.绑定角色.收益检测时间 = P.空闲时间;
                this.绑定角色.收益间隔 = 主程.当前时间.AddSeconds(this.绑定角色.收益检测时间);
                this.绑定角色.传送物品 = P.道具ID;
                this.绑定角色.释放技能 = P.技能ID;
                this.绑定角色.自动拾取 = P.开启自动拾取;
                this.绑定角色.拾取范围 = P.拾取范围;
                this.绑定角色.背包预留 = P.开启预留背包;
                this.绑定角色.预留格数 = P.预留格数;
                this.绑定角色.优先战斗 = P.优先战斗;
                this.绑定角色.不捡他人归属 = P.不捡取他人装备;
                this.绑定角色.不打他人归属 = P.不抢怪;
                this.发送封包(new 自动战斗回执
                {
                    开关状态 = (P.自动战斗 ? 1 : 0)
                });
            }
        }

        public void 处理封包(开始锻石炼药 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色?.开始锻石炼药(P.模版编号, P.基础材料容器, P.基础材料位置, P.额外材料容器, P.额外材料位置, P.额外材料二容器, P.额外材料二位置);
            }
        }

        public void 处理封包(觉醒之力积累开关 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.开启觉醒经验存储 = P.开关;
            }
        }

        public void 处理封包(觉醒技能升级 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.升级觉醒技能(P.技能编号);
            }
        }

        public void 处理封包(激活部位刻印 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"连接异常断开链接.  处理封包: {P.GetType()}, 当前阶段: {this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.激活部位刻印(P.激活部位);
            }
        }

        public void 处理封包(预留封包零一 P)
        {
        }

        public void 处理封包(预留封包零二 P)
        {
        }

        public void 处理封包(领取七天奖励 P)
        {
            // PROTO-04 续: 原无阶段守卫
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载) return;
            this.绑定角色?.领取七天奖励(P.未知参数, P.领取编号);
        }

        public void 处理封包(领取七天大奖 P)
        {
            // PROTO-04 续: 原无阶段守卫
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载) return;
            this.绑定角色?.领取七天大奖(P.领取天数);
        }

        public void 处理封包(请求七天详情 P)
        {
            if (!Settings.屏蔽七天活动)
            {
                if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载)
                {
                    this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                    return;
                }
                else
                {
                    this.绑定角色?.发送封包(new 同步七天信息
                    {
                        字节描述 = this.绑定角色.获取七天乐字节()
                    });
                }
            }
        }

        public void 处理封包(上传游戏设置 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家更改设置(P.字节描述);
            }
        }

        public void 处理封包(客户碰触法阵 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(客户进入法阵 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家进入法阵(P.法阵编号);
            }
        }

        public void 处理封包(点击Npcc对话 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            if (Settings.开启任务系统)
            {
                this.绑定角色.ProcessActionNPC(P.对象编号, P.任务编号);
            }
        }

        public void 处理封包(玩家完成任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Settings.开启任务系统)
            {
                this.绑定角色.CompleteQuest(P.任务编号, P.未知标识);
            }
        }

        public void 处理封包(AcceptRewardPacket P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Settings.开启成就系统)
            {
                this.绑定角色.AcceptReward(P.任务编号);
            }
        }

        public void 处理封包(请求对象数据 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.请求对象外观(P.对象编号, P.状态编号);
            }
        }

        public void 处理封包(客户网速测试 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.发送封包(new 网速测试应答
                {
                    当前时间 = P.客户时间
                });
            }
        }

        public void 处理封包(测试网关网速 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.发送封包(new 登陆查询应答
                {
                    当前时间 = P.客户时间
                });
            }
        }

        public void 处理封包(客户请求复活 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家请求复活();
            }
        }

        public void 处理封包(切换攻击模式 P)
        {
            攻击模式 result;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Enum.IsDefined(typeof(攻击模式), (int)P.攻击模式) && Enum.TryParse<攻击模式>(P.攻击模式.ToString(), out result))
            {
                this.绑定角色.更改攻击模式(result);
            }
            else
            {
                this.尝试断开连接(new Exception("更改攻击模式时提供错误的枚举参数.即将断开连接."));
            }
        }

        public void 处理封包(更改宠物模式 P)
        {
            宠物模式 result;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Enum.IsDefined(typeof(宠物模式), (int)P.宠物模式) && Enum.TryParse<宠物模式>(P.宠物模式.ToString(), out result))
            {
                this.绑定角色.更改宠物模式(result);
            }
            else
            {
                this.尝试断开连接(new Exception($"更改宠物模式时提供错误的枚举参数.即将断开连接. 参数 - {P.宠物模式}"));
            }
        }

        public void 处理封包(上传角色位置 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(客户角色转动 P)
        {
            游戏方向 result;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Enum.IsDefined(typeof(游戏方向), (int)P.转动方向) && Enum.TryParse<游戏方向>(P.转动方向.ToString(), out result))
            {
                this.绑定角色.玩家角色转动(result);
            }
            else
            {
                this.尝试断开连接(new Exception("玩家角色转动时提供错误的枚举参数.即将断开连接."));
            }
        }

        public void 处理封包(客户角色走动 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家角色走动(P.坐标);
            }
        }

        public void 处理封包(客户角色跑动 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家角色跑动(P.坐标);
            }
        }

        public void 处理封包(角色开关技能 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家开关技能(P.技能编号);
            }
        }

        public void 处理封包(角色装备技能 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (P.技能栏位 < 32)
            {
                this.绑定角色.玩家拖动技能(P.技能栏位, P.技能编号);
            }
            else
            {
                this.尝试断开连接(new Exception("玩家装配技能时提供错误的封包参数.即将断开连接."));
            }
        }

        public void 处理封包(角色释放技能 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家释放技能(P.技能编号, P.动作编号, P.目标编号, P.锚点坐标);
            }
        }

        public void 处理封包(战斗姿态切换 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家切换姿态(P.姿态编号, P.触发动作);
            }
        }

        public void 处理封包(客户更换角色 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.更换角色(this);
                this.当前阶段 = 游戏阶段.选择角色;
            }
        }

        public void 处理封包(场景加载完成 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家进入场景();
                this.当前阶段 = 游戏阶段.正在游戏;
            }
        }

        public void 处理封包(退出当前副本 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家退出副本();
            }
        }

        public void 处理封包(玩家退出登录 P)
        {
            if (this.当前阶段 == 游戏阶段.正在登录)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.返回登录(this);
            }
        }

        public void 处理封包(打开角色背包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.网络连接?.发送封包(new 同步元宝数量
            {
                元宝数量 = this.绑定角色.元宝数量
            });
        }

        public void 处理封包(角色拾取物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.拾取脚下物品(P.物品编号);
        }

        public void 处理封包(角色丢弃物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家丢弃物品(P.背包类型, P.物品位置, P.丢弃数量);
            }
        }

        public void 处理封包(角色转移物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (this.绑定角色 != null)
            {
                this.绑定角色.玩家转移物品(P.当前背包, P.原有位置, P.目标背包, P.目标位置);
            }
        }

        public void 处理封包(角色使用物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家使用物品(P.背包类型, P.物品位置);
            }
        }

        public void 处理封包(玩家喝修复油 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家喝修复油(P.背包类型, P.物品位置);
            }
        }

        public void 处理封包(玩家扩展背包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家扩展背包(P.背包类型, P.扩展大小);
            }
        }

        public void 处理封包(请求商店数据 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.请求商店数据(P.版本编号);
            }
        }

        public void 处理封包(角色购买物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家购买物品(P.商店编号, P.物品位置, P.购入数量);
            }
        }

        public void 处理封包(角色卖出物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家出售物品(P.背包类型, P.物品位置, P.卖出数量);
            }
        }

        public void 处理封包(查询回购列表 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.请求回购清单();
            }
        }

        public void 处理封包(角色回购物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (P.物品位置 < 100)
            {
                this.绑定角色.玩家回购物品(P.物品位置);
            }
            else
            {
                this.尝试断开连接(new Exception("玩家回购物品时提供错误的位置参数.即将断开连接."));
            }
        }

        public void 处理封包(商店修理单件 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.商店修理单件(P.背包类型, P.物品位置);
            }
        }

        public void 处理封包(商店修理全部 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.商店修理全部();
            }
        }

        public void 处理封包(商店特修单件 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.商店特修单件(P.物品容器, P.物品位置);
            }
        }

        public void 处理封包(随身修理单件 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.随身修理单件(P.物品容器, P.物品位置, P.物品编号);
            }
        }

        public void 处理封包(随身特修全部 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.随身修理全部();
            }
        }

        public void 处理封包(角色整理背包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家整理背包(P.背包类型);
            }
        }

        public void 处理封包(角色拆分物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家拆分物品(P.当前背包, P.物品位置, P.拆分数量, P.目标背包, P.目标位置);
            }
        }

        public void 处理封包(角色分解物品 P)
        {
            物品背包分类 result;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Enum.TryParse<物品背包分类>(P.背包类型.ToString(), out result) && Enum.IsDefined(typeof(物品背包分类), result))
            {
                this.绑定角色.玩家分解物品(P.背包类型, P.物品位置, P.分解数量);
            }
            else
            {
                this.尝试断开连接(new Exception("玩家分解物品时提供错误的枚举参数.即将断开连接."));
            }
        }

        public void 处理封包(角色兑换精粹 P)
        {
            物品背包分类 result;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (Enum.TryParse<物品背包分类>(P.背包类型.ToString(), out result) && Enum.IsDefined(typeof(物品背包分类), result))
            {
                this.绑定角色.玩家兑换精粹(P.物品位置);
            }
            else
            {
                this.尝试断开连接(new Exception("玩家兑换精粹时提供错误的枚举参数.即将断开连接."));
            }
        }

        public void 处理封包(角色合成物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家合成物品(P.物品编号);
            }
        }

        public void 处理封包(角色合成装备 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (this.绑定角色.玩家合成装备(合成勋章: false, P.合成模板, P.未知参数, P.合成参数) > 0)
            {
                this.发送封包(new 装备合成通知());
            }
        }

        public void 处理封包(角色合成勋章 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (this.绑定角色.玩家合成装备(合成勋章: true, P.合成模板, P.未知参数, P.合成参数) > 0)
            {
                this.发送封包(new 装备合成通知());
            }
        }

        public void 处理封包(角色重铸装备 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            int num;
            num = this.绑定角色.玩家重铸(P.装备部位, v: false);
            if (num == -1)
            {
                return;
            }
            if (num > 0)
            {
                if (P.装备部位 == 11)
                {
                    this.发送封包(new 玩家重铸技能
                    {
                        通知结果 = 1,
                        返回编号 = num
                    });
                    return;
                }
                if (游戏物品.数据表.TryGetValue(num, out var value) && value is 游戏装备 { 装备套装: > 游戏装备套装.无, 装备套装: < 游戏装备套装.神秘装备 })
                {
                    网络服务网关.发送公告($"<#P0:<#PN:{this.绑定角色.对象名字}>><#P1:<#I:{num}>><#T:MMOGame.DLG.ITEM.8>");
                }
                this.发送封包(new 玩家重铸装备
                {
                    通知结果 = 1,
                    返回编号 = num,
                    未知参数 = 8
                });
            }
            else
            {
                this.发送封包(new 玩家重铸装备
                {
                    通知结果 = 0,
                    返回编号 = 0,
                    未知参数 = 0
                });
            }
        }

        public void 处理封包(角色重铸技能 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            int num;
            num = this.绑定角色.玩家重铸(99, v: false);
            if (num > 0)
            {
                this.发送封包(new 玩家重铸技能
                {
                    通知结果 = 1,
                    返回编号 = num
                });
            }
        }

        public void 处理封包(角色勋章洗练 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家勋章洗炼(P.未知参数, P.物品位置);
            }
        }

        public void 处理封包(角色洗练替换 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.替换勋章洗炼();
            }
        }

        public void 处理封包(角色武器铸魂 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (this.绑定角色.玩家武器铸魂() > 0)
            {
                this.发送封包(new 装备铸魂通知
                {
                    返回结果 = 0
                });
            }
        }

        public void 处理封包(角色灵魂绑定 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                if (this.绑定角色.角色数据.角色装备[0] == null)
                {
                    return;
                }
                switch (this.绑定角色.角色职业)
                {
                    case 游戏对象职业.战士:
                        if (this.绑定角色.角色数据.角色装备[0].升级攻击.V >= 9)
                        {
                            this.绑定角色.角色数据.角色装备[0].灵魂绑定.V = true;
                        }
                        break;
                    case 游戏对象职业.法师:
                        if (this.绑定角色.角色数据.角色装备[0].升级魔法.V >= 9)
                        {
                            this.绑定角色.角色数据.角色装备[0].灵魂绑定.V = true;
                        }
                        break;
                    case 游戏对象职业.刺客:
                        if (this.绑定角色.角色数据.角色装备[0].升级刺术.V >= 9)
                        {
                            this.绑定角色.角色数据.角色装备[0].灵魂绑定.V = true;
                        }
                        break;
                    case 游戏对象职业.弓手:
                        if (this.绑定角色.角色数据.角色装备[0].升级弓术.V >= 9)
                        {
                            this.绑定角色.角色数据.角色装备[0].灵魂绑定.V = true;
                        }
                        break;
                    case 游戏对象职业.道士:
                        if (this.绑定角色.角色数据.角色装备[0].升级道术.V >= 9)
                        {
                            this.绑定角色.角色数据.角色装备[0].灵魂绑定.V = true;
                        }
                        break;
                    case 游戏对象职业.龙枪:
                        if (this.绑定角色.角色数据.角色装备[0].升级攻击.V >= 9)
                        {
                            this.绑定角色.角色数据.角色装备[0].灵魂绑定.V = true;
                        }
                        break;
                }
                if (this.绑定角色.角色数据.角色装备[0].灵魂绑定.V)
                {
                    this.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.绑定角色.角色数据.角色装备[0].字节描述()
                    });
                }
                if (Settings.开启成就系统)
                {
                    this.绑定角色.成就变量变更(AchievementVariables.SoulBindCount, 1);
                }
                this.发送封包(new 灵魂绑定通知
                {
                    返回结果 = 0
                });
                网络服务网关.发送公告($"<#P0:<#PN:{this.绑定角色.对象名字}>><#P1:<#I:{this.绑定角色.角色数据.角色装备[0].物品编号}>><#T:MMOGame.DLG.ITEM.33>");
            }
        }

        public void 处理封包(角色武器祈祷 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (this.绑定角色.玩家武器祈祷(P.未知参数) > 0)
            {
                this.发送封包(new 武器祈祷通知());
            }
        }

        public void 处理封包(角色防具升级 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.角色防具升级(P.装备部位);
            }
        }

        public void 处理封包(角色装备神佑 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                int 通知结果;
                通知结果 = this.绑定角色.玩家装备神佑((byte)P.装备部位);
                this.发送封包(new 装备神佑通知
                {
                    通知结果 = 通知结果
                });
            }
        }

        public void 处理封包(玩家装备打孔 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家装备打孔(P.物品位置);
            }
        }

        public void 处理封包(玩家装备雕色 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家装备雕色(P.物品位置, P.孔洞位置, P.孔洞颜色);
            }
        }

        public void 处理封包(物品锁定状态 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.物品锁定状态(P.背包类型, P.物品位置, P.锁定状态);
            }
        }

        public void 处理封包(仓库锁定状态 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.仓库锁定状态(P.锁定状态);
            }
        }

        public void 处理封包(玩家孔色传承 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家孔色传承(P.来源位置, P.传承位置);
            }
        }

        public void 处理封包(玩家合成灵石 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家合成灵石(P.物品编号, P.幸运符数);
            }
        }

        public void 处理封包(玩家镶嵌灵石 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家镶嵌灵石(P.装备类型, P.装备位置, P.装备孔位, P.灵石类型, P.灵石位置);
            }
        }

        public void 处理封包(玩家拆除灵石 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家拆除灵石(P.装备类型, P.装备位置, P.装备孔位);
            }
        }

        public void 处理封包(普通铭文洗练 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.普通铭文洗练(P.装备类型, P.装备位置, P.物品编号);
            }
        }

        public void 处理封包(高级铭文洗练 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.高级铭文洗练(P.装备类型, P.装备位置, P.物品编号);
            }
        }

        public void 处理封包(替换铭文洗练 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.替换铭文洗练(P.装备类型, P.装备位置, P.物品编号);
            }
        }

        public void 处理封包(替换高级铭文 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.高级洗练确认(P.装备类型, P.装备位置);
            }
        }

        public void 处理封包(替换低级铭文 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.替换洗练确认(P.装备类型, P.装备位置);
            }
        }

        public void 处理封包(放弃铭文替换 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.放弃替换铭文();
            }
        }

        public void 处理封包(解锁双铭文位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.解锁双铭文位(P.装备类型, P.装备位置, P.操作参数);
            }
        }

        public void 处理封包(切换双铭文位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.切换双铭文位(P.装备类型, P.装备位置, P.操作参数);
            }
        }

        public void 处理封包(传承武器铭文 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.传承武器铭文(P.来源类型, P.来源位置, P.目标类型, P.目标位置);
            }
        }

        public void 处理封包(升级武器普通 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.升级武器普通(P.首饰组, P.材料组);
            }
        }

        public void 处理封包(升级武器高级 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.升级武器高级(P.首饰组, P.材料组);
            }
        }

        public void 处理封包(角色选中目标 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家选中对象(P.对象编号);
            }
        }

        public void 处理封包(开始Npcc对话 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.开始Npcc对话(P.对象编号);
            }
        }

        public void 处理封包(继续Npcc对话 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.继续Npc对话(P.对话编号);
            }
        }

        public void 处理封包(查看玩家装备 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色?.查看对象装备(P.对象编号);
            }
        }

        public void 处理封包(请求龙卫数据 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色?.查看他人龙卫(P.对象编号);
        }

        public void 处理封包(扩展龙卫记录 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.扩展龙卫记录();
        }

        public void 处理封包(龙卫传承觉醒 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.龙卫传承觉醒(P.属性位置, P.当前阶段);
        }

        public void 处理封包(龙卫传承重塑 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.龙卫传承重塑(P.属性位置, P.模式, P.附加);
        }

        public void 处理封包(龙卫记录部位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.龙卫记录部位(P.记录部位, P.记录序号);
        }

        public void 处理封包(龙卫恢复部位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.龙卫恢复部位(P.记录部位, P.记录序号);
        }

        public void 处理封包(龙卫修改备注 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.龙卫修改备注(P.记录序号, P.文本信息);
        }

        public void 处理封包(龙卫全套恢复 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.龙卫全套恢复(P.恢复模式, P.记录序号);
        }

        public void 处理封包(请求魂石数据 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(查询奖励找回 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色?.查询奖励找回();
        }

        public void 处理封包(找回日程奖励 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色?.找回日程奖励(P.日程编号, P.找回次数);
            this.SendRaw(340, 14, new byte[12]
            {
                1, 0, 0, 0, 8, 0, 0, 0, 1, 0,
                0, 0
            });
        }

        public void 处理封包(同步角色战力 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询玩家战力(P.对象编号);
            }
        }

        public void 处理封包(查询问卷调查 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(领取玛法传说 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.领取玛法传说(P.领取编号);
        }

        public void 处理封包(玩家申请交易 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家申请交易(P.对象编号);
            }
        }

        public void 处理封包(玩家同意交易 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家同意交易(P.对象编号);
            }
        }

        public void 处理封包(玩家结束交易 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家结束交易();
            }
        }

        public void 处理封包(玩家放入金币 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家放入金币(P.金币数量);
            }
        }

        public void 处理封包(玩家放入物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家放入物品(P.放入位置, P.放入物品, P.物品容器, P.物品位置);
            }
        }

        public void 处理封包(玩家锁定交易 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家锁定交易();
            }
        }

        public void 处理封包(玩家解锁交易 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家解锁交易();
            }
        }

        public void 处理封包(玩家确认交易 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家确认交易();
            }
        }

        public void 处理封包(玩家准备摆摊 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家准备摆摊();
            }
        }

        public void 处理封包(玩家重整摊位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家重整摊位();
            }
        }

        public void 处理封包(玩家开始摆摊 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家开始摆摊();
            }
        }

        public void 处理封包(玩家收起摊位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家收起摊位();
            }
        }

        public void 处理封包(放入摊位物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.放入摊位物品(P.放入位置, P.物品容器, P.物品位置, P.物品数量, P.物品价格);
            }
        }

        public void 处理封包(取回摊位物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.取回摊位物品(P.取回位置);
            }
        }

        public void 处理封包(更改摊位名字 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.更改摊位名字(P.摊位名字);
            }
        }

        public void 处理封包(更改摊位外观 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.升级摊位外观(P.外观编号);
            }
        }

        public void 处理封包(打开角色摊位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家打开摊位(P.对象编号);
            }
        }

        public void 处理封包(购买摊位物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.购买摊位物品(P.对象编号, P.物品位置, P.购买数量);
            }
        }

        public void 处理封包(添加好友关注 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家添加关注(P.对象编号, P.对象名字);
            }
        }

        public void 处理封包(取消好友关注 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家取消关注(P.对象编号);
            }
        }

        public void 处理封包(新建好友分组 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(移动好友分组 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(发送好友聊天 P)
        {
            // 聊天/邮件/广播类字段虽已由 HIGH-02 限制 64KB, 但单条聊天合理上限是几百字节;
            // 64KB 全服广播会造成 O(连接数 * 字段长) 流量放大. 收紧到 2KB.
            const int 单条聊天最大字节 = 2048;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (P.字节数据.Length < 7 || P.字节数据.Length > 单条聊天最大字节)
            {
                this.尝试断开连接(new Exception($"数据长度越界,断开连接.  处理封包: {P.GetType()},  数据长度:{P.字节数据.Length}"));
            }
            else if (P.字节数据.Last() != 0)
            {
                this.尝试断开连接(new Exception($"数据错误,断开连接.  处理封包: {P.GetType()},  无结束符."));
            }
            else
            {
                this.绑定角色.玩家好友聊天(P.字节数据);
            }
        }

        public void 处理封包(玩家添加仇人 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家添加仇人(P.对象编号);
            }
        }

        public void 处理封包(玩家删除仇人 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家删除仇人(P.对象编号);
            }
        }

        public void 处理封包(玩家屏蔽对象 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家屏蔽目标(P.对象编号);
            }
        }

        public void 处理封包(玩家解除屏蔽 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家解除屏蔽(P.对象编号);
            }
        }

        public void 处理封包(玩家比较成就 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(发送聊天信息 P)
        {
            // 单条聊天上限 2KB, 防止 64KB 广播包被放大成全服洪水. 详见 [DEEP-04].
            const int 单条聊天最大字节 = 2048;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (P.字节数据.Length < 7 || P.字节数据.Length > 单条聊天最大字节)
            {
                this.尝试断开连接(new Exception($"数据长度越界,断开连接.  处理封包: {P.GetType()},  数据长度:{P.字节数据.Length}"));
            }
            else if (P.字节数据.Last() != 0)
            {
                this.尝试断开连接(new Exception($"数据错误,断开连接.  处理封包: {P.GetType()},  无结束符."));
            }
            else
            {
                this.绑定角色.玩家发送广播(P.字节数据);
            }
        }

        public void 处理封包(发送社交消息 P)
        {
            const int 单条聊天最大字节 = 2048;
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (P.字节数据.Length < 6 || P.字节数据.Length > 单条聊天最大字节)
            {
                this.尝试断开连接(new Exception($"数据长度越界,断开连接.  处理封包: {P.GetType()},  数据长度:{P.字节数据.Length}"));
            }
            else if (P.字节数据.Last() != 0)
            {
                this.尝试断开连接(new Exception($"数据错误,断开连接.  处理封包: {P.GetType()},  无结束符."));
            }
            else
            {
                this.绑定角色.玩家发送消息(P.字节数据);
            }
        }

        public void 处理封包(请求角色数据 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.请求角色资料(P.角色编号);
            }
        }

        public void 处理封包(上传社交信息 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(查询附近队伍 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询附近队伍();
            }
        }

        public void 处理封包(查询队伍信息 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询队伍信息(P.对象编号);
            }
        }

        public void 处理封包(申请创建队伍 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请创建队伍(P.对象编号, P.分配方式);
            }
        }

        public void 处理封包(发送组队请求 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.发送组队请求(P.对象编号);
            }
        }

        public void 处理封包(申请离开队伍 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请队员离队(P.对象编号);
            }
        }

        public void 处理封包(申请更改队伍 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请移交队长(P.队长编号);
            }
        }

        public void 处理封包(回应组队请求 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.回应组队请求(P.对象编号, P.组队方式, P.回应方式);
            }
        }

        public void 处理封包(玩家装配称号 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家使用称号(P.称号编号);
            }
        }

        public void 处理封包(玩家卸下称号 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家卸下称号();
            }
        }

        public void 处理封包(申请发送邮件 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请发送邮件(P.字节数据);
            }
        }

        public void 处理封包(查询邮箱内容 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询邮箱内容();
            }
        }

        public void 处理封包(查看邮件内容 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查看邮件内容(P.邮件编号);
            }
        }

        public void 处理封包(删除指定邮件 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.删除指定邮件(P.邮件编号);
            }
        }

        public void 处理封包(提取邮件附件 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.提取邮件附件(P.邮件编号);
            }
        }

        public void 处理封包(查询行会名字 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询行会信息(P.行会编号);
            }
        }

        public void 处理封包(更多行会信息 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.更多行会信息();
            }
        }

        public void 处理封包(查看行会列表 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查看行会列表(P.行会编号, P.查看方式);
            }
        }

        public void 处理封包(查找对应行会 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查找对应行会(P.行会编号, P.行会名字);
            }
        }

        public void 处理封包(申请加入行会 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请加入行会(P.行会编号, P.行会名字);
            }
        }

        public void 处理封包(查看申请列表 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查看申请列表();
            }
        }

        public void 处理封包(处理入会申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.处理入会申请(P.对象编号, P.处理类型);
            }
        }

        public void 处理封包(处理入会邀请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.处理入会邀请(P.对象编号, P.处理类型);
            }
        }

        public void 处理封包(邀请加入行会 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.邀请加入行会(P.对象名字);
            }
        }

        public void 处理封包(申请创建行会 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请创建行会(P.字节数据);
            }
        }

        public void 处理封包(申请解散行会 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请解散行会();
            }
        }

        public void 处理封包(捐献行会资金 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.捐献行会资金(P.金币数量);
            }
        }

        public void 处理封包(行会仓库刷新 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.行会仓库刷新(P.仓库页面);
            }
        }

        public void 处理封包(行会仓库转入 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.行会仓库转入(P.原来容器, P.原来位置, P.仓库页面, P.仓库位置);
            }
        }

        public void 处理封包(行会仓库移动 P)
        {
        }

        public void 处理封包(行会仓库转出 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.行会仓库转出(P.仓库页面, P.仓库位置, P.目标容器, P.目标位置);
            }
        }

        public void 处理封包(进入行会领地 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.进入行会领地(P.地图类型, P.行会编号);
            }
        }

        public void 处理封包(发放行会福利 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.发放行会福利();
            }
        }

        public void 处理封包(申请离开行会 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请离开行会();
            }
        }

        public void 处理封包(查询行会战史 P)
        {
        }

        public void 处理封包(更改行会公告 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.更改行会公告(P.行会公告);
            }
        }

        public void 处理封包(更改行会宣言 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.更改行会宣言(P.行会宣言);
            }
        }

        public void 处理封包(设置行会禁言 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.设置行会禁言(P.对象编号, P.禁言状态);
            }
        }

        public void 处理封包(变更会员职位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.变更会员职位(P.对象编号, P.对象职位);
            }
        }

        public void 处理封包(逐出行会成员 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.逐出行会成员(P.对象编号);
            }
        }

        public void 处理封包(转移会长职位 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.转移会长职位(P.对象编号);
            }
        }

        public void 处理封包(申请行会外交 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请行会外交(P.外交类型, P.外交时间, P.行会名字);
            }
        }

        public void 处理封包(申请行会敌对 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请行会敌对(P.敌对时间, P.行会名字);
            }
        }

        public void 处理封包(处理结盟申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.处理结盟申请(P.处理类型, P.行会编号);
            }
        }

        public void 处理封包(申请解除结盟 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请解除结盟(P.行会编号);
            }
        }

        public void 处理封包(申请解除敌对 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.申请解除敌对(P.行会编号);
            }
        }

        public void 处理封包(处理解敌申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.处理解除申请(P.行会编号, P.回应类型);
            }
        }

        public void 处理封包(更改存储权限 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            // C02: 仅会长可改存储权限. 原实现无职位鉴权, 任意会员可自授行会仓库取出权限盗空公会仓库, 或清空会长权限做 DoS.
            行会数据 行会 = this.绑定角色.所属行会;
            if (行会 == null || !行会.行会成员.ContainsKey(this.绑定角色.角色数据) || 行会.行会成员[this.绑定角色.角色数据] != 行会职位.会长)
            {
                return;
            }
            行会.更新行会权限(P.行会职位, (ushort)(P.权限标志 & 0x0FFF));
        }

        public void 处理封包(查看结盟申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查看结盟申请();
            }
        }

        public void 处理封包(更多行会事记 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.更多行会事记();
            }
        }

        public void 处理封包(查询行会成就 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(开启行会活动 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(发布通缉榜单 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(同步通缉榜单 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(发起行会战争 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(查询地图路线 P)
        {
            if (this.当前阶段 != 游戏阶段.场景加载 && this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询地图路线();
            }
        }

        public void 处理封包(切换地图路线 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.切换地图路线();
            }
        }

        public void 处理封包(跳过剧情动画 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(更改收徒推送 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.更改收徒推送(P.收徒推送);
            }
        }

        public void 处理封包(查询师门成员 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询师门成员();
            }
        }

        public void 处理封包(查询师门奖励 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询师门奖励();
            }
        }

        public void 处理封包(查询拜师名册 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询拜师名册();
            }
        }

        public void 处理封包(查询收徒名册 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询收徒名册();
            }
        }

        public void 处理封包(祝贺徒弟升级 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(玩家申请拜师 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家申请拜师(P.对象编号);
            }
        }

        public void 处理封包(同意拜师申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.同意拜师申请(P.对象编号);
            }
        }

        public void 处理封包(拒绝拜师申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.拒绝拜师申请(P.对象编号);
            }
        }

        public void 处理封包(玩家申请收徒 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.玩家申请收徒(P.对象编号);
            }
        }

        public void 处理封包(同意收徒申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.同意收徒申请(P.对象编号);
            }
        }

        public void 处理封包(拒绝收徒申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.拒绝收徒申请(P.对象编号);
            }
        }

        public void 处理封包(逐出师门申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.逐出师门申请(P.对象编号);
            }
        }

        public void 处理封包(离开师门申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.离开师门申请();
            }
        }

        public void 处理封包(提交出师申请 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.提交出师申请();
            }
        }

        public void 处理封包(验证动态密码 P)
        {
            // PROTO-04 续: 二级密码校验属于登录后流程, 拒绝未登录调用
            if (this.当前阶段 != 游戏阶段.正在游戏 && this.当前阶段 != 游戏阶段.场景加载) return;
            this.绑定角色?.验证动态密码(P.动态密码);
        }

        public void 处理封包(查询公会通缉 P)
        {
        }

        public void 处理封包(查询排名榜单 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询排名榜单(P.榜单类型, P.起始位置);
            }
        }

        public void 处理封包(查看演武排名 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(刷新演武挑战 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(开始战场演武 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(进入演武战场 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(跨服武道排名 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(登录寄售平台 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (P.操作标识 == 1)
            {
                this.发送封包(new 寄售登录账号
                {
                    消息类型 = 2,
                    登录账号 = this.绑定账号.账号名字.V
                });
            }
            else if (P.操作标识 == 2)
            {
                this.发送封包(new 寄售登录账号
                {
                    消息类型 = 5,
                    登录账号 = this.绑定账号.账号名字.V
                });
            }
        }

        public void 处理封包(查询平台商品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询寄售物品(P.过滤筛选);
            }
        }

        public void 处理封包(查询我的寄售 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询我的寄售();
            }
        }

        public void 处理封包(查询指定商品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询指定物品(P.物品编号);
            }
        }

        public void 处理封包(下架寄售物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.寄售下架物品(P.订单编号);
            }
        }

        public void 处理封包(购买平台物品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.寄售购买物品(P.订单编号);
            }
        }

        public void 处理封包(上架平台商品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.寄售上架物品(P.背包类型, P.背包位置, P.时间类型, P.上架价格);
            }
        }

        public void 处理封包(请求珍宝数据 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询珍宝商店(P.数据版本);
            }
        }

        public void 处理封包(查询出售信息 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.查询出售信息();
            }
        }

        public void 处理封包(购买珍宝商品 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.购买珍宝商品(P.物品编号, P.购买数量);
            }
        }

        public void 处理封包(购买每周特惠 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.购买每周特惠(P.礼包编号);
            }
        }

        public void 处理封包(购买玛法特权 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.购买玛法特权(P.特权类型, P.购买数量);
            }
        }

        public void 处理封包(预定玛法特权 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.预定玛法特权(P.特权类型);
            }
        }

        public void 处理封包(领取特权礼包 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.领取特权礼包(P.特权类型, P.礼包位置);
            }
        }

        public void 处理封包(玩家每日签到 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            this.绑定角色.领取每日签到();
        }

        public void 处理封包(客户账号登录 P)
        {

            门票信息 value;
            // C19: 网卡封禁原以客户端 1001 封包自报的 P.物理地址 作键, 该值客户端可控可任意伪造, 封禁形同虚设.
            // 不可伪造的封禁边界是 accept 层按 socket RemoteEndPoint 的 IP 封禁(网络服务网关.异步连接)与账号封禁.
            // 对齐 获取真实IP地址(PROTO-08) 的处置: 不再把自报 MAC 作访问控制依据.
            if (this.当前阶段 != 0)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else if (!网络服务网关.门票数据表.TryGetValue(P.登录门票, out value))
            {
                this.尝试断开连接(new Exception("登录的门票不存在." + P.登录门票));
            }
            else if (主程.当前时间 > value.有效时间)
            {
                this.尝试断开连接(new Exception("登录门票已经过期."));
            }
            else
            {
                账号数据 账号数据2;
                账号数据2 = ((!游戏数据网关.账号数据表.检索表.TryGetValue(value.登录账号, out var value2) || !(value2 is 账号数据 账号数据)) ? new 账号数据(value.登录账号) : 账号数据);
                if (账号数据2.网络连接 != null)
                {
                    账号数据2.网络连接?.发送封包(new 登陆错误提示
                    {
                        错误代码 = 260u
                    });
                    账号数据2.网络连接?.尝试断开连接(new Exception("账号重复登录, 被踢下线."));
                    this.尝试断开连接(new Exception("账号已经在线, 无法登录."));
                }
                else
                {
                    账号数据2.账号登录(this, this.网络地址);
                }
            }
            网络服务网关.门票数据表.Remove(P.登录门票);

        }

        public void 处理封包(客户创建角色 P)
        {
            if (this.当前阶段 != 游戏阶段.选择角色)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.创建角色(this, P);
            }
        }

        public void 处理封包(客户删除角色 P)
        {
            if (this.当前阶段 != 游戏阶段.选择角色)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.删除角色(this, P);
            }
        }

        public void 处理封包(彻底删除角色 P)
        {
            if (this.当前阶段 != 游戏阶段.选择角色)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.永久删除(this, P);
            }
        }

        public void 处理封包(客户进入游戏 P)
        {
            if (this.当前阶段 != 游戏阶段.选择角色)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.进入游戏(this, P);
            }
        }

        public void 处理封包(客户找回角色 P)
        {
            if (this.当前阶段 != 游戏阶段.选择角色)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定账号.找回角色(this, P);
            }
        }

        public void 处理封包(开启道具消息 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
            else
            {
                this.绑定角色.OpenChest(P.ObjectId);
            }
        }

        public void 处理封包(玩家放弃任务 P)
        {
            // PROTO-04 续: 原无阶段守卫
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (Settings.开启任务系统)
            {
                this.绑定角色?.玩家放弃任务(P.任务编号);
            }
        }

        public void 处理封包(组队拍卖竞价 P)
        {
            // PROTO-04 续: 原无阶段守卫, 攻击者未登录就能在他人队伍触发竞价
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色?.角色数据?.当前队伍?.竞价拍卖(this.绑定角色.角色数据, P.拍卖顺序, P.当前竞价);
        }

        public void 处理封包(组队拍卖放弃 P)
        {
            if (this.当前阶段 == 游戏阶段.正在游戏 && this.绑定角色.角色数据.当前队伍 != null)
            {
                this.绑定角色?.角色数据?.当前队伍?.放弃拍卖(this.绑定角色?.角色数据, P.拍卖顺序);
                return;
            }
            this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
            return;
        }

        public void 处理封包(历练兑换经验 P)
        {
        }

        public void 处理封包(灵符兑换历练 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏)
            {
                this.尝试断开连接(new Exception($"阶段异常,断开连接.  处理封包: {P.GetType()},  当前阶段:{this.当前阶段}"));
                return;
            }
        }

        public void 处理封包(开始狩猎任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.开始狩猎任务();
        }

        public void 处理封包(请求同步狩猎 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.发送封包(new 同步狩猎信息
            {
                未知标志 = 1,
                未知参数 = 39015,
                狩猎编号 = this.绑定角色.角色数据.已接狩猎.V,
                剩余秒数 = (int)((this.绑定角色.角色数据.已接狩猎.V > 0) ? Math.Max(0.0, (this.绑定角色.角色数据.狩猎完成.V - 主程.当前时间).TotalSeconds) : 0.0)
            });
        }

        public void 处理封包(领取狩猎奖励 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.领取狩猎奖励();
        }

        public void 处理封包(查询狩猎详情 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.发送狩猎详情();
        }

        public void 处理封包(刷新狩猎任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.刷新狩猎详情();
        }

        public void 处理封包(放弃狩猎任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.放弃狩猎任务();
        }

        public void 处理封包(请求悬赏任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (Settings.开启任务系统)
            {
                this.绑定角色.请求悬赏任务(P.任务类型);
            }
        }

        public void 处理封包(请求悬赏剩余 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (Settings.开启任务系统)
            {
                this.绑定角色.请求悬赏剩余(P.任务类型);
            }
        }

        public void 处理封包(刷新悬赏任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (Settings.开启任务系统)
            {
                this.绑定角色.刷新悬赏任务(P.对话编号);
            }
        }

        public void 处理封包(完成悬赏任务 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (Settings.开启任务系统)
            {
                this.绑定角色.完成悬赏任务(P.物品编号, P.物品容器, P.物品位置, P.任务编号);
            }
        }

        public void 处理封包(领取杀怪成就 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (Settings.开启成就系统)
            {
                this.绑定角色.领取杀怪成就(P.成就编号, P.进度编号);
            }
        }

        public void 处理封包(领取日程奖励 P)
        {
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            this.绑定角色.领取日程奖励(P.奖励进度);
        }

        public void 处理封包(自定义扩展封包 P)
        {
            // PROTO-04 续 + 日志注入: 原本未登录客户端可触发将任意 UTF-8 内容写入系统日志,
            // 攻击者可借此污染日志 / 注入 ANSI 控制字符 / 钓鱼管理员 / 撑爆日志磁盘.
            if (this.当前阶段 != 游戏阶段.正在游戏 || this.绑定角色 == null) return;
            if (P.字节数据.Length < 4 || P.字节数据.Length > 1024) return;
            BinaryReader binaryReader;
            binaryReader = new BinaryReader(new MemoryStream(P.字节数据));
            int num;
            num = binaryReader.ReadInt32();
            if (num == 1)
            {
                byte[] bytes;
                bytes = binaryReader.ReadBytes(P.字节数据.Length - 4);
                string raw;
                raw = Encoding.UTF8.GetString(bytes);
                // 过滤控制字符 (含 \r\n 与 ANSI 转义), 防日志注入伪造行
                System.Text.StringBuilder sb;
                sb = new System.Text.StringBuilder(raw.Length);
                foreach (char ch in raw)
                {
                    if (ch >= 0x20 && ch != 0x7F) sb.Append(ch);
                }
                主程.添加系统日志("收到自定义扩展协议ID 1:" + sb.ToString());
            }
            else
            {
                主程.添加系统日志("收到不支持的扩展协议ID:" + num);
            }
        }
    }
}
