using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using _001D_000F_0007_0013_0011_0015;
using 游戏服务器.副本类;
using 游戏服务器.管理命令;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using DevExpress.XtraBars;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace 游戏服务器.地图类
{
    public sealed partial class 玩家实例 : 地图对象
    {
        #region 挖矿
        public void 玩家开始挖矿(Point 坐标)
        {
            if (当前地图.地图编号 == 144 || 当前地图.地图编号 == 153 || 当前地图.地图编号 == 154)
            {
                // D02: 挖矿必须装备武器(0号位)且耐久>0, 否则 武器损失持久() 取不到武器将零成本无损耗刷矿.
                if (!角色装备.TryGetValue(0, out var 挖矿武器) || 挖矿武器.当前持久.V <= 0)
                {
                    挖矿次数 = 0;
                    网络服务网关.发送信息(this, "请先装备一把可用的工具再挖矿");
                    return;
                }
                网络服务网关.发送信息(this, "开始挖矿");
                if (游戏技能.数据表.TryGetValue("通用-挖矿动作0", out var value))
                {
                    new 技能实例(this, value, null, base.动作编号, 当前地图, 当前坐标, null, 当前坐标, null);
                }
                发送封包(new 切换战斗姿态
                {
                    //对象编号 = 角色数据.数据索引.V,
                    对象编号 = 地图编号,
                    姿态编号 = base.动作编号,
                    触发动作 = 1
                });
                挖矿次数 = 1000;
            }
            else
            {
                网络服务网关.发送信息(this, "此地图不允许挖矿");
            }
        }
        public void 玩家挖矿成功(int 编号, Point 坐标, ushort 动作间隔)
        {
        }
        public void 玩家挖矿失败(Point 玩家坐标, ushort 高度)
        {
        }
        public void 挖矿奖励给予(string 玩家姓名)
        {
            if (this.当前等级 > 0)
            {
                int num2 = 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000) + 主程.随机数.Next(1, 10000);
                if (num2 > 50) //黑铁矿概率
                {
                    int key6 = 主程.随机数.Next(114000, 114026);
                    if (!游戏物品.数据表.TryGetValue(key6, out var value6))
                    {
                        return;
                    }
                    byte b11 = byte.MaxValue;
                    byte b12 = 0;
                    while (b12 < 资源包大小)
                    {
                        if (角色资源包.ContainsKey(b12))
                        {
                            b12 = (byte)(b12 + 1);
                            continue;
                        }
                        b11 = b12;
                        break;
                    }
                    if (b11 == byte.MaxValue)
                    {
                        网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    角色资源包[b11] = new 物品数据(value6, 角色数据, 7, b11, 1);
                    角色数据.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.角色资源包[b11].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得一块黑铁矿");
                    return;
                }
                if (num2 > 100) //金矿概率
                {
                    int key7 = 主程.随机数.Next(118000, 118026);
                    if (!游戏物品.数据表.TryGetValue(key7, out var value7))
                    {
                        return;
                    }
                    byte b13 = byte.MaxValue;
                    byte b14 = 0;
                    while (b14 < 资源包大小)
                    {
                        if (角色资源包.ContainsKey(b14))
                        {
                            b14 = (byte)(b14 + 1);
                            continue;
                        }
                        b13 = b14;
                        break;
                    }
                    if (b13 == byte.MaxValue)
                    {
                        网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    角色资源包[b13] = new 物品数据(value7, 角色数据, 7, b13, 1);
                    角色数据.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.角色资源包[b13].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得一块金矿");
                    return;
                }
                if (num2 > 200) //银矿概率
                {
                    int key8 = 主程.随机数.Next(117000, 117026);
                    if (!游戏物品.数据表.TryGetValue(key8, out var value8))
                    {
                        return;
                    }
                    byte b15 = byte.MaxValue;
                    byte b16 = 0;
                    while (b16 < 资源包大小)
                    {
                        if (角色资源包.ContainsKey(b16))
                        {
                            b16 = (byte)(b16 + 1);
                            continue;
                        }
                        b15 = b16;
                        break;
                    }
                    if (b15 == byte.MaxValue)
                    {
                        网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    角色资源包[b15] = new 物品数据(value8, 角色数据, 7, b15, 1);
                    角色数据.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.角色资源包[b15].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得一块银矿");
                    return;
                }
                if (num2 > 500) //铁矿概率
                {
                    int key9 = 主程.随机数.Next(116000, 116026);
                    if (!游戏物品.数据表.TryGetValue(key9, out var value9))
                    {
                        return;
                    }
                    byte b17 = byte.MaxValue;
                    byte b18 = 0;
                    while (b18 < 资源包大小)
                    {
                        if (角色资源包.ContainsKey(b18))
                        {
                            b18 = (byte)(b18 + 1);
                            continue;
                        }
                        b17 = b18;
                        break;
                    }
                    if (b17 == byte.MaxValue)
                    {
                        网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    角色资源包[b17] = new 物品数据(value9, 角色数据, 7, b17, 1);
                    角色数据.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.角色资源包[b17].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得一块铁矿");
                    return;
                }
                if (num2 > 1000) //铜矿概率
                {
                    int key10 = 主程.随机数.Next(115000, 115026);
                    if (!游戏物品.数据表.TryGetValue(key10, out var value10))
                    {
                        return;
                    }
                    byte b19 = byte.MaxValue;
                    byte b20 = 0;
                    while (b20 < 资源包大小)
                    {
                        if (角色资源包.ContainsKey(b20))
                        {
                            b20 = (byte)(b20 + 1);
                            continue;
                        }
                        b19 = b20;
                        break;
                    }
                    if (b19 == byte.MaxValue)
                    {
                        网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    角色资源包[b19] = new 物品数据(value10, 角色数据, 7, b19, 1);
                    角色数据.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.角色资源包[b19].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得一块铜矿");
                    return;
                }
            }
            /*
            if (挖矿数据模板.DataSheet.TryGetValue(当前地图.地图编号, out var value11))
            {
                int num4 = 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000)
                     + 主程.RandomNumber.Next(1, 10000);
                if (num4 > value11.材料概率1)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型1, out var value20))
                    {
                        return;
                    }
                    byte b37 = byte.MaxValue;
                    byte b38 = 0;
                    while (b38 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b38))
                        {
                            b38 = (byte)(b38 + 1);
                            continue;
                        }
                        b37 = b38;
                        break;
                    }
                    if (b37 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b37] = new 物品数据(value20, 角色数据, 7, b37, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b37].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value20.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率2)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型2, out var value21))
                    {
                        return;
                    }
                    byte b39 = byte.MaxValue;
                    byte b40 = 0;
                    while (b40 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b40))
                        {
                            b40 = (byte)(b40 + 1);
                            continue;
                        }
                        b39 = b40;
                        break;
                    }
                    if (b39 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b39] = new 物品数据(value21, 角色数据, 7, b39, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b39].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value21.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率3)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型3, out var value22))
                    {
                        return;
                    }
                    byte b41 = byte.MaxValue;
                    byte b42 = 0;
                    while (b42 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b42))
                        {
                            b42 = (byte)(b42 + 1);
                            continue;
                        }
                        b41 = b42;
                        break;
                    }
                    if (b41 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b41] = new 物品数据(value22, 角色数据, 7, b41, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b41].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value22.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率4)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型4, out var value23))
                    {
                        return;
                    }
                    byte b43 = byte.MaxValue;
                    byte b44 = 0;
                    while (b44 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b44))
                        {
                            b44 = (byte)(b44 + 1);
                            continue;
                        }
                        b43 = b44;
                        break;
                    }
                    if (b43 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b43] = new 物品数据(value23, 角色数据, 7, b43, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b43].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value23.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率5)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型5, out var value24))
                    {
                        return;
                    }
                    byte b45 = byte.MaxValue;
                    byte b46 = 0;
                    while (b46 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b46))
                        {
                            b46 = (byte)(b46 + 1);
                            continue;
                        }
                        b45 = b46;
                        break;
                    }
                    if (b45 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b45] = new 物品数据(value24, 角色数据, 7, b45, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b45].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value24.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率6)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型6, out var value25))
                    {
                        return;
                    }
                    byte b47 = byte.MaxValue;
                    byte b48 = 0;
                    while (b48 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b48))
                        {
                            b48 = (byte)(b48 + 1);
                            continue;
                        }
                        b47 = b48;
                        break;
                    }
                    if (b47 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b47] = new 物品数据(value25, 角色数据, 7, b47, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b47].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value25.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率7)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型7, out var value26))
                    {
                        return;
                    }
                    byte b49 = byte.MaxValue;
                    byte b50 = 0;
                    while (b50 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b50))
                        {
                            b50 = (byte)(b50 + 1);
                            continue;
                        }
                        b49 = b50;
                        break;
                    }
                    if (b49 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b49] = new 物品数据(value26, 角色数据, 7, b49, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b49].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value26.物品名字 + "]");
                    return;
                }
                if (num4 > value11.材料概率8)
                {
                    if (!游戏物品.数据表.TryGetValue(value11.材料类型8, out var value27))
                    {
                        return;
                    }
                    byte b51 = byte.MaxValue;
                    byte b52 = 0;
                    while (b52 < ExtraBackpackSize)
                    {
                        if (ExtraBackpack.ContainsKey(b52))
                        {
                            b52 = (byte)(b52 + 1);
                            continue;
                        }
                        b51 = b52;
                        break;
                    }
                    if (b51 == byte.MaxValue)
                    {
                        ActiveConnection?.发送封包(new GameErrorMessagePacket
                        {
                            错误代码 = 1793
                        });
                        return;
                    }
                    ExtraBackpack[b51] = new 物品数据(value27, 角色数据, 7, b51, 1);
                    角色数据.ActiveConnection?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 角色数据.ExtraBackPack[b51].字节描述()
                    });
                    网络服务网关.发送信息(this, "恭喜您获得[" + value27.物品名字 + "]");
                    return;
                }
            }
            */
            if (角色装备.TryGetValue(0, out var v) && v.当前持久.V == 0 && 挖矿次数 != 0)
            {
                挖矿次数 = 0;
            }
        }
        public void 秒触发内容结果(string 玩家姓名)
        {
            //挖矿
            // C18: 每 tick 复核当前地图, 离开矿区(走动/跑动/转动以外的传送/换图)即停挖, 防离图后续发矿白嫖.
            if (挖矿次数 != 0 && 当前地图.地图编号 != 144 && 当前地图.地图编号 != 153 && 当前地图.地图编号 != 154)
            {
                挖矿次数 = 0;
                网络服务网关.发送信息(this, "停止挖矿");
            }
            if (挖矿次数 != 0 && 主程.当前时间 > 挖矿时间间隔)
            {
                if (游戏技能.数据表.TryGetValue("通用-挖矿动作0", out var value2))
                {
                    new 技能实例(this, value2, null, base.动作编号, 当前地图, 当前坐标, null, 当前坐标, null);
                }
                发送封包(new 切换战斗姿态
                {
                    //对象编号 = 角色数据.数据索引.V,
                    对象编号 = 地图编号,
                    姿态编号 = base.动作编号,
                    触发动作 = 1
                });
                挖矿次数--;
                挖矿时间间隔 = 主程.当前时间.AddSeconds(1);
                武器损失持久();
                挖矿奖励给予(玩家姓名);
            }
            //金币银币自动入包
            foreach (地图对象 item5 in base.邻居列表.ToList())
            {
                //if ((this.背包剩余 <= 0 && item5.IsMoney()) || !(item5 is 物品实例 物品实例2))
                if (!(item5 is 物品实例 物品实例2))
                {
                    continue;
                }

                if (物品实例2.掉落对象 != null)
                {
                    if (物品实例2.掉落对象 == null)
                    {
                        continue;
                    }
                    地图对象 掉落对象;
                    掉落对象 = 物品实例2.掉落对象;
                    if (掉落对象 != null && 掉落对象.对象类型 == 游戏对象类型.玩家)
                    {
                        continue;
                    }
                }
                int num;
                num = 物品实例2.堆叠数量;
                if (num < 0 || num >= int.MaxValue)
                {
                    主程.添加系统日志($"玩家拾取物品 {物品实例2} {num}");
                    num = 1;
                }
                if (物品实例2.物品编号 == 0 && Settings.银币自动入包 == true)
                {
                    this.网络连接?.发送封包(new 玩家拾取金币
                    {
                        金币数量 = num
                    });
                    this.修改货币("+", 游戏货币.银币, (uint)num);
                    主程.添加货币日志(this, "玩家拾取物品->" + 物品实例2.物品模板?.物品名字, 游戏货币.银币, num);
                    物品实例2.物品转移处理();
                }
                if (物品实例2.物品编号 == 1 && Settings.金币自动入包 == true)
                {
                    this.网络连接?.发送封包(new 玩家拾取金币
                    {
                        金币数量 = num
                    });
                    this.修改货币("+", 游戏货币.金币, (uint)num);
                    主程.添加货币日志(this, "玩家拾取物品->" + 物品实例2.物品模板?.物品名字, 游戏货币.金币, num);
                    物品实例2.物品转移处理();
                }
                //物品自动入包
                if (Settings.物品自动入包 == true)
                {
                    if (物品实例2.物品重量 != 0 && 物品实例2.物品重量 > this.最大负重 - this.背包重量)
                    {
                        continue;
                    }
                    if (物品实例2.物品归属.Count != 0 && !物品实例2.物品归属.Contains(this.角色数据) && 主程.当前时间 < 物品实例2.归属时间)
                    {
                        continue;
                    }

                    if (背包剩余 <= 0)
                    {
                        continue;
                    }
                    if (物品实例2.掉落对象 != null)
                    {
                        if (物品实例2.掉落对象 == null)
                        {
                            continue;
                        }
                        地图对象 掉落对象;
                        掉落对象 = 物品实例2.掉落对象;
                        if (掉落对象 != null && 掉落对象.对象类型 == 游戏对象类型.玩家)
                        {
                            continue;
                        }
                    }
                    //if (base.网格距离(item5) < this.自动拾取范围 && (物品实例2.IsMoney() || this.物品过滤.Contains(物品实例2.物品编号)))
                    if (this.物品过滤.Contains(物品实例2.物品编号))
                    {
                        this.玩家拾取物品(物品实例2);
                    }


                }
                if (Settings.自动分解装备 == true && Settings.不分解极品装备 == true)
                {
                    this.挂机自动分解();

                }
                if (Settings.自动分解装备 == true && Settings.不分解极品装备 == false)
                {

                    this.分解完成 = false;
                    foreach (KeyValuePair<byte, 物品数据> item in this.角色背包.ToList())
                    {
                        byte key;
                        key = item.Key;
                        物品数据 value;
                        value = item.Value;
                        if (!value.是否上锁 && 物品分解.数据表.ContainsKey(value.物品编号))
                        {
                            this.玩家分解物品(1, key, 1);
                            num++;
                            if (num > 5)
                            {
                                return;
                            }
                        }
                    }
                    this.分解完成 = true;
                }

            }

            //GM无敌时血量和魔法量变满
            if (无敌模式 && 当前体力 < this[游戏对象属性.最大体力])
            {
                this.当前体力 = this[游戏对象属性.最大体力];

            }
            if (无敌模式 && 当前魔力 < this[游戏对象属性.最大魔力])
            {
                this.当前魔力 = this[游戏对象属性.最大魔力];
            }
            if (当前地图.安全区内(this.当前坐标) && Settings.安全区内满血满蓝)
            {
                this.当前体力 = this[游戏对象属性.最大体力];
                this.当前魔力 = this[游戏对象属性.最大魔力];

            }


        }

        #endregion
    }
}
