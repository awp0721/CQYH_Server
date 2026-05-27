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
        public void 查询附近队伍()
        {
        }

        public void 查询队伍信息(int 对象编号)
        {
            if (对象编号 == this.地图编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3852
                });
                return;
            }
            客户网络 客户网络;
            查询队伍应答 查询队伍应答;
            object obj;
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                客户网络 = this.网络连接;
                if (客户网络 == null)
                {
                    return;
                }
                查询队伍应答 = new 查询队伍应答
                {
                    队伍编号 = (角色数据.当前队伍?.队伍编号 ?? 0),
                    队长编号 = (角色数据.当前队伍?.队长编号 ?? 0)
                };
                队伍数据 当前队伍;
                当前队伍 = 角色数据.当前队伍;
                if (当前队伍 == null)
                {
                    obj = null;
                }
                else
                {
                    obj = 当前队伍.队长名字;
                    if (obj != null)
                    {
                        goto IL_00b3;
                    }
                }
                obj = "";
                goto IL_00b3;
            }
            this.网络连接?.发送封包(new 社交错误提示
            {
                错误编号 = 6732
            });
            return;
        IL_00b3:
            查询队伍应答.队伍名字 = (string)obj;
            客户网络.发送封包(查询队伍应答);
        }

        public void 申请创建队伍(int 对象编号, byte 分配方式)
        {
            if (this.所属队伍 != null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3847
                });
                return;
            }
            if (this.地图编号 == 对象编号)
            {
                this.所属队伍 = new 队伍数据(this.角色数据, 分配方式);
                this.网络连接?.发送封包(new 玩家加入队伍
                {
                    字节描述 = this.所属队伍.队伍描述()
                });
                using MemoryStream memoryStream = new MemoryStream();
                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(this.地图编号);
                binaryWriter.Write(this.所属队伍.队伍编号);
                this.网络连接?.SendRaw(197, 10, memoryStream.ToArray());
                this.玩家加入队伍();
                return;
            }
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (角色数据.当前队伍 != null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3847
                    });
                    return;
                }
                if (角色数据.角色在线(out var 网络))
                {
                    this.所属队伍 = new 队伍数据(this.角色数据, 分配方式);
                    this.网络连接?.发送封包(new 玩家加入队伍
                    {
                        字节描述 = this.所属队伍.队伍描述()
                    });
                    using MemoryStream memoryStream2 = new MemoryStream();
                    using BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
                    binaryWriter2.Write(this.地图编号);
                    binaryWriter2.Write(this.所属队伍.队伍编号);
                    this.网络连接?.SendRaw(197, 10, memoryStream2.ToArray());
                    this.所属队伍.邀请列表[角色数据] = 主程.当前时间.AddMinutes(5.0);
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3842
                    });
                    网络.发送封包(new 发送组队申请
                    {
                        组队方式 = 0,
                        对象编号 = this.地图编号,
                        对象职业 = (byte)this.角色职业,
                        对象名字 = this.对象名字
                    });
                    this.玩家加入队伍();
                    return;
                }
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3844
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 发送组队请求(int 对象编号)
        {
            游戏数据 value;
            if (对象编号 == this.地图编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3852
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据)
            {
                客户网络 网络2;
                if (this.所属队伍 == null)
                {
                    客户网络 网络;
                    if (角色数据.当前队伍 == null)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3860
                        });
                    }
                    else if (角色数据.当前队伍.队员数量 >= 11)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3848
                        });
                    }
                    else if (角色数据.当前队伍.队长数据.角色在线(out 网络))
                    {
                        角色数据.当前队伍.申请列表[this.角色数据] = 主程.当前时间.AddMinutes(5.0);
                        网络.发送封包(new 发送组队申请
                        {
                            组队方式 = 1,
                            对象编号 = this.地图编号,
                            对象职业 = (byte)this.角色职业,
                            对象名字 = this.对象名字
                        });
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3842
                        });
                    }
                    else
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3844
                        });
                    }
                }
                else if (this.地图编号 != this.所属队伍.队长编号)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3850
                    });
                }
                else if (角色数据.当前队伍 != null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3847
                    });
                }
                else if (this.所属队伍.队员数量 >= 11)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3848
                    });
                }
                else if (角色数据.角色在线(out 网络2))
                {
                    this.所属队伍.邀请列表[角色数据] = 主程.当前时间.AddMinutes(5.0);
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3842
                    });
                    网络2.发送封包(new 发送组队申请
                    {
                        组队方式 = 0,
                        对象编号 = this.地图编号,
                        对象职业 = (byte)this.角色职业,
                        对象名字 = this.对象名字
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3844
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 回应组队请求(int 对象编号, byte 组队方式, byte 回应方式)
        {
            游戏数据 value;
            if (this.地图编号 == 对象编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3852
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据)
            {
                if (组队方式 == 0)
                {
                    if (回应方式 == 0)
                    {
                        if (角色数据.当前队伍 == null)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 3860
                            });
                            return;
                        }
                        if (this.所属队伍 != null)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 3847
                            });
                            return;
                        }
                        if (角色数据.当前队伍.队员数量 >= 11)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 3848
                            });
                            return;
                        }
                        if (!角色数据.当前队伍.邀请列表.ContainsKey(this.角色数据))
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 3860
                            });
                            return;
                        }
                        if (角色数据.当前队伍.邀请列表[this.角色数据] < 主程.当前时间)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 3860
                            });
                            return;
                        }
                        角色数据.当前队伍.发送封包(new 队伍增加成员
                        {
                            队伍编号 = 角色数据.当前队伍.队伍编号,
                            对象编号 = this.地图编号,
                            对象名字 = this.对象名字,
                            对象性别 = (byte)this.角色性别,
                            对象职业 = (byte)this.角色职业,
                            在线离线 = 0
                        });
                        this.所属队伍 = 角色数据.当前队伍;
                        角色数据.当前队伍.队伍成员.Add(this.角色数据);
                        this.玩家加入队伍();
                        this.网络连接?.发送封包(new 玩家加入队伍
                        {
                            字节描述 = this.所属队伍.队伍描述()
                        });
                    }
                    else
                    {
                        队伍数据 当前队伍;
                        当前队伍 = 角色数据.当前队伍;
                        if (当前队伍 != null && 当前队伍.邀请列表.Remove(this.角色数据) && 角色数据.角色在线(out var 网络))
                        {
                            网络.发送封包(new 社交错误提示
                            {
                                错误编号 = 3856
                            });
                        }
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3855
                        });
                    }
                }
                else if (回应方式 == 0)
                {
                    客户网络 网络2;
                    if (this.所属队伍 == null)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3860
                        });
                    }
                    else if (this.所属队伍.队员数量 >= 11)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3848
                        });
                    }
                    else if (this.地图编号 != this.所属队伍.队长编号)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3850
                        });
                    }
                    else if (!this.所属队伍.申请列表.ContainsKey(角色数据))
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3860
                        });
                    }
                    else if (this.所属队伍.申请列表[角色数据] < 主程.当前时间)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3860
                        });
                    }
                    else if (角色数据.当前队伍 != null)
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 3847
                        });
                    }
                    else if (角色数据.角色在线(out 网络2))
                    {
                        this.所属队伍.发送封包(new 队伍增加成员
                        {
                            队伍编号 = this.所属队伍.队伍编号,
                            对象编号 = 角色数据.角色编号,
                            对象名字 = 角色数据.角色名字.V,
                            对象性别 = (byte)角色数据.角色性别.V,
                            对象职业 = (byte)角色数据.角色职业.V,
                            在线离线 = 0
                        });
                        角色数据.当前队伍 = this.所属队伍;
                        this.所属队伍.队伍成员.Add(角色数据);
                        网络2.绑定角色?.玩家加入队伍();
                        网络2.发送封包(new 玩家加入队伍
                        {
                            字节描述 = this.所属队伍.队伍描述()
                        });
                    }
                }
                else
                {
                    队伍数据 队伍数据;
                    队伍数据 = this.所属队伍;
                    if (队伍数据 != null && 队伍数据.申请列表.Remove(角色数据) && 角色数据.角色在线(out var 网络3))
                    {
                        网络3.发送封包(new 社交错误提示
                        {
                            错误编号 = 3858
                        });
                    }
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3857
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 申请队员离队(int 对象编号)
        {
            游戏数据 value;
            if (this.所属队伍 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3854
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据)
            {
                if (this.角色数据 == 角色数据)
                {
                    this.所属队伍.放弃所有拍卖(角色数据);
                    this.所属队伍.队伍成员.Remove(this.角色数据);
                    this.所属队伍.发送封包(new 队伍成员离开
                    {
                        对象编号 = this.地图编号,
                        队伍编号 = this.所属队伍.数据索引.V
                    });
                    this.网络连接?.发送封包(new 玩家离开队伍
                    {
                        队伍编号 = this.所属队伍.数据索引.V
                    });
                    using MemoryStream memoryStream = new MemoryStream();
                    using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(this.地图编号);
                    binaryWriter.Write(0);
                    this.网络连接?.SendRaw(197, 10, memoryStream.ToArray());
                    if (this.角色数据 == this.所属队伍.队长数据)
                    {
                        角色数据 角色数据2;
                        角色数据2 = this.所属队伍.队伍成员.FirstOrDefault((角色数据 O) => O.网络连接 != null);
                        if (角色数据2 != null)
                        {
                            this.所属队伍.队长数据 = 角色数据2;
                            this.所属队伍.发送封包(new 队伍状态改变
                            {
                                成员上限 = 11,
                                队伍编号 = this.所属队伍.队伍编号,
                                队伍名字 = this.所属队伍.队长名字,
                                分配方式 = this.所属队伍.拾取方式,
                                队长编号 = this.所属队伍.队长编号
                            });
                        }
                        else
                        {
                            this.所属队伍.删除数据();
                        }
                    }
                    this.角色数据.当前队伍 = null;
                    return;
                }
                if (!this.所属队伍.队伍成员.Contains(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6732
                    });
                    return;
                }
                if (this.角色数据 == this.所属队伍.队长数据)
                {
                    // 与玩家自主离队对齐: 队长强制踢人时也要清理被踢者的未完成拍卖,
                    // 否则攻击者作为队长可以让队员竞价中突然被踢, 金币/物品状态卡死.
                    this.所属队伍.放弃所有拍卖(角色数据);
                    this.所属队伍.队伍成员.Remove(角色数据);
                    角色数据.当前队伍 = null;
                    this.所属队伍.发送封包(new 队伍成员离开
                    {
                        队伍编号 = this.所属队伍.数据索引.V,
                        对象编号 = 角色数据.角色编号
                    });
                    角色数据.网络连接?.发送封包(new 玩家离开队伍
                    {
                        队伍编号 = this.所属队伍.数据索引.V
                    });
                    using MemoryStream memoryStream2 = new MemoryStream();
                    using BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
                    binaryWriter2.Write(对象编号);
                    binaryWriter2.Write(0);
                    this.网络连接?.SendRaw(197, 10, memoryStream2.ToArray());
                    return;
                }
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3850
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 申请移交队长(int 对象编号)
        {
            游戏数据 value;
            if (this.所属队伍 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3854
                });
            }
            else if (this.角色数据 != this.所属队伍.队长数据)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 3850
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据)
            {
                if (角色数据 == this.角色数据)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 3852
                    });
                    return;
                }
                if (!this.所属队伍.队伍成员.Contains(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6732
                    });
                    return;
                }
                this.所属队伍.队长数据 = 角色数据;
                this.所属队伍.发送封包(new 队伍状态改变
                {
                    成员上限 = 11,
                    队伍编号 = this.所属队伍.队伍编号,
                    队伍名字 = this.所属队伍.队长名字,
                    分配方式 = this.所属队伍.拾取方式,
                    队长编号 = this.所属队伍.队长编号
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 玩家加入队伍()
        {
            if (this.开启七天乐)
            {
                this.修改七天进度(37, this.角色数据.七天进度[37] + 1);
                this.修改七天进度(42, this.角色数据.七天进度[42] + 1);
                this.修改七天进度(57, this.角色数据.七天进度[57] + 1);
            }
            if (Settings.开启成就系统)
            {
                this.成就变量变更(AchievementVariables.JoinTeamCount, 1);
            }
        }

        public void 查询邮箱内容()
        {
            this.网络连接?.发送封包(new 同步邮箱内容
            {
                字节数据 = this.全部邮件描述()
            });
        }

        public void 申请发送邮件(byte[] 数据)
        {
            if (数据.Length >= 94 && 数据.Length <= 839)
            {
                if (主程.当前时间 < this.邮件时间)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6151
                    });
                    return;
                }
                if (this.金币数量 < 1000)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6149
                    });
                    return;
                }
                byte[] array;
                array = 数据.Take(32).ToArray();
                byte[] array2;
                array2 = 数据.Skip(32).Take(61).ToArray();
                数据.Skip(93).Take(4).ToArray();
                byte[] array3;
                array3 = 数据.Skip(97).ToArray();
                if (array[0] != 0 && array2[0] != 0 && array3[0] != 0)
                {
                    string key;
                    key = Encoding.UTF8.GetString(array).Split(new char[1], StringSplitOptions.RemoveEmptyEntries)[0];
                    string 标题;
                    // LOW-A 续: 标题/正文 会保存到收件人邮件并展示在 UI, 同样需要过滤控制字符与双向覆写
                    标题 = 净化展示文本(Encoding.UTF8.GetString(array2).Split(new char[1], StringSplitOptions.RemoveEmptyEntries)[0]);
                    string 正文;
                    正文 = 净化展示文本(Encoding.UTF8.GetString(array3).Split(new char[1], StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (游戏数据网关.角色数据表.检索表.TryGetValue(key, out var value) && value is 角色数据 角色数据)
                    {
                        if (角色数据.角色邮件.Count >= 100)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6147
                            });
                            return;
                        }
                        this.金币数量 -= 1000u;
                        主程.添加货币日志(this, "发送邮件扣除", 游戏货币.金币, -1000);
                        角色数据.发送邮件(new 邮件数据(this.角色数据, 标题, 正文, null));
                        this.网络连接?.发送封包(new 成功发送邮件());
                    }
                    else
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 6146
                        });
                    }
                }
                else
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 申请发送邮件.  错误: 邮件文本错误."));
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 申请发送邮件.  错误: 数据长度错误."));
            }
        }

        public void 查看邮件内容(int 邮件编号)
        {
            if (游戏数据网关.邮件数据表.数据表.TryGetValue(邮件编号, out var value) && value is 邮件数据 邮件数据)
            {
                if (!this.全部邮件.Contains(邮件数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6148
                    });
                    return;
                }
                this.未读邮件.Remove(邮件数据);
                邮件数据.未读邮件.V = false;
                this.网络连接?.发送封包(new 同步邮件内容
                {
                    字节数据 = 邮件数据.邮件内容描述()
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6148
                });
            }
        }

        public void 删除指定邮件(int 邮件编号)
        {
            if (游戏数据网关.邮件数据表.数据表.TryGetValue(邮件编号, out var value) && value is 邮件数据 邮件数据)
            {
                if (!this.全部邮件.Contains(邮件数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6148
                    });
                    return;
                }
                this.网络连接?.发送封包(new 邮件删除成功
                {
                    邮件编号 = 邮件数据.邮件编号
                });
                this.未读邮件.Remove(邮件数据);
                this.全部邮件.Remove(邮件数据);
                邮件数据.邮件附件.V?.删除数据();
                邮件数据.删除数据();
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6148
                });
            }
        }

        public void 提取邮件附件(int 邮件编号)
        {
            if (游戏数据网关.邮件数据表.数据表.TryGetValue(邮件编号, out var value) && value is 邮件数据 邮件数据)
            {
                if (!this.全部邮件.Contains(邮件数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6148
                    });
                    return;
                }
                if (邮件数据.邮件附件.V == null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6150
                    });
                    return;
                }
                if (this.背包剩余 <= 0)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 1793
                    });
                    return;
                }
                if (this.根据物品编号获得货币(邮件数据.邮件附件.V.物品编号, 邮件数据.邮件附件.V.当前持久.V))
                {
                    this.网络连接?.发送封包(new 成功提取附件
                    {
                        邮件编号 = 邮件数据.邮件编号
                    });
                    邮件数据.邮件附件.V.删除数据();
                    邮件数据.邮件附件.V = null;
                    return;
                }
                int num;
                num = -1;
                byte b;
                b = 0;
                while (b < this.背包大小)
                {
                    if (this.角色背包.ContainsKey(b))
                    {
                        b++;
                        continue;
                    }
                    num = b;
                    break;
                }
                if (num == -1)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 1793
                    });
                    return;
                }
                this.网络连接?.发送封包(new 成功提取附件
                {
                    邮件编号 = 邮件数据.邮件编号
                });
                this.角色背包[(byte)num] = 邮件数据.邮件附件.V;
                主程.添加物品日志(this, "提取邮件附件", 邮件数据.邮件附件.V, 邮件数据.邮件附件.V.当前持久.V, $"发件人:{邮件数据.邮件作者?.V}");
                邮件数据.邮件附件.V.物品位置.V = (byte)num;
                邮件数据.邮件附件.V.物品容器.V = 1;
                邮件数据.邮件附件.V = null;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = this.角色数据.角色背包[(byte)num].字节描述()
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6148
                });
            }
        }

        public void 查询行会信息(int 行会编号)
        {
            if (游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out var value) && value is 行会数据 行会数据)
            {
                this.网络连接?.发送封包(new 行会名字应答
                {
                    行会编号 = 行会数据.数据索引.V,
                    行会名字 = 行会数据.行会名字.V,
                    创建时间 = 行会数据.创建日期.V,
                    会长编号 = 行会数据.行会会长.V.数据索引.V,
                    行会人数 = (byte)行会数据.行会成员.Count,
                    行会等级 = 行会数据.行会等级.V
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6669
                });
            }
        }

        public void 更多行会信息()
        {
        }

        public void 更多行会事记()
        {
        }

        public void 查看行会列表(int 行会编号, byte 查看方式)
        {
            int num;
            num = Math.Max(0, (游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out var value) && value is 行会数据 行会数据) ? (行会数据.行会排名.V - 1) : 0);
            int num2;
            num2 = ((查看方式 == 2) ? Math.Max(0, num) : Math.Max(0, num - 11));
            int num3;
            num3 = Math.Min(12, 系统数据.数据.行会人数排名.Count - num2);
            if (num3 > 0)
            {
                List<行会数据> range;
                range = 系统数据.数据.行会人数排名.GetRange(num2, num3);
                using MemoryStream memoryStream = new MemoryStream();
                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(查看方式);
                binaryWriter.Write((byte)num3);
                foreach (行会数据 item in range)
                {
                    binaryWriter.Write(item.行会检索描述());
                }
                this.网络连接?.发送封包(new 同步行会列表
                {
                    字节数据 = memoryStream.ToArray()
                });
                return;
            }
            using MemoryStream memoryStream2 = new MemoryStream();
            using BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
            binaryWriter2.Write(查看方式);
            binaryWriter2.Write((byte)0);
            this.网络连接?.发送封包(new 同步行会列表
            {
                字节数据 = memoryStream2.ToArray()
            });
        }

        public void 查找对应行会(int 行会编号, string 行会名字)
        {
            if ((游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out var value) || 游戏数据网关.行会数据表.检索表.TryGetValue(行会名字, out value)) && value is 行会数据 行会数据)
            {
                this.网络连接?.发送封包(new 查找行会应答
                {
                    字节数据 = 行会数据.行会检索描述()
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 申请解散行会()
        {
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] != 行会职位.会长)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (this.所属行会.结盟行会.Count != 0)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6739
                });
            }
            else if (this.所属行会.结盟行会.Count != 0)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6740
                });
            }
            else if (地图处理网关.攻城行会.Contains(this.所属行会))
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6819
                });
            }
            else if (this.所属行会 == 系统数据.数据.占领行会.V)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6819
                });
            }
            else
            {
                this.所属行会.解散行会();
            }
        }

        public void 申请创建行会(byte[] 数据)
        {
            物品数据 物品;
            if (this.打开界面 != "Guild")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 申请创建行会. 错误: 没有打开界面."));
            }
            else if (this.所属行会 != null)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6707
                });
            }
            else if (this.当前等级 < 12)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6699
                });
            }
            else if (this.金币数量 < 200000)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6699
                });
            }
            else if (!this.查找背包物品(80002, out 物品))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6664
                });
            }
            else if (数据.Length > 25 && 数据.Length < 128)
            {
                string[] array;
                array = Encoding.UTF8.GetString(数据.Take(25).ToArray()).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
                string[] array2;
                array2 = Encoding.UTF8.GetString(数据.Skip(25).ToArray()).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 0 && array2.Length != 0 && Encoding.UTF8.GetBytes(array[0]).Length < 25 && Encoding.UTF8.GetBytes(array2[0]).Length < 101)
                {
                    // LOW-A 续: 行会名会被全服公告 ($"[xxx]创建了行会[xxx]") 和加入聊天广播,
                    // 控制字符/双向覆写会污染所有玩家的 UI; 这里直接拒绝异常字符而非吞掉,
                    // 因为行会名永久存在数据库, 而且 ContainsKey 查询也应基于原字符串.
                    string 行会名 = 净化展示文本(array[0]);
                    string 行会宣言 = 净化展示文本(array2[0]);
                    if (string.IsNullOrEmpty(行会名) || string.IsNullOrEmpty(行会宣言))
                    {
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 申请创建行会. 错误: 名字含非法字符."));
                        return;
                    }
                    if (游戏数据网关.行会数据表.检索表.ContainsKey(行会名))
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 6697
                        });
                        return;
                    }
                    this.金币数量 -= 200000u;
                    主程.添加货币日志(this, "创建行会扣除", 游戏货币.金币, -200000);
                    this.消耗背包物品(1, 物品, "创建行会扣除");
                    this.所属行会 = new 行会数据(this, 行会名, 行会宣言);
                    this.网络连接?.发送封包(new 创建行会应答
                    {
                        行会名字 = this.所属行会.行会名字.V
                    });
                    this.网络连接?.发送封包(new 行会信息公告
                    {
                        字节数据 = this.所属行会.行会信息描述()
                    });
                    base.发送封包(new 同步对象行会
                    {
                        对象编号 = this.地图编号,
                        行会编号 = this.所属行会.行会编号
                    });
                    网络服务网关.发送公告($"[{this.对象名字}]创建了行会[{this.所属行会}]");
                }
                else
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 申请创建行会. 错误: 字符长度错误."));
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 申请创建行会. 错误: 数据长度错误."));
            }
        }

        public void 更改行会公告(byte[] 数据)
        {
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] > 行会职位.监事)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (数据.Length != 0 && 数据.Length < 255)
            {
                if (数据[0] == 0)
                {
                    this.所属行会.更改公告("");
                }
                else
                {
                    this.所属行会.更改公告(净化展示文本(Encoding.UTF8.GetString(数据).Split('\0')[0]));
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 更改行会公告. 错误: 数据长度错误"));
            }
        }

        // LOW-A: 行会公告/宣言/摊位名 等会广播到其他玩家 UI 的文本, 客户端可塞入
        // 控制字符 (\r \n \b ESC 等) 与 RTL/双向覆写 Unicode 字符, 在不同客户端
        // 显示效果不一致, 可被用于钓鱼/伪装系统消息. 入口统一过滤.
        private static string 净化展示文本(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;
            System.Text.StringBuilder sb;
            sb = new System.Text.StringBuilder(raw.Length);
            foreach (char c in raw)
            {
                // 丢弃 C0 控制字符 (\0~\x1F) + DEL (\x7F) + Unicode 双向覆写
                if (c < 0x20 || c == 0x7F) continue;
                // U+202A..202E + U+2066..2069 是 Unicode 双向控制符, 易被钓鱼
                if (c >= 0x202A && c <= 0x202E) continue;
                if (c >= 0x2066 && c <= 0x2069) continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        public void 更改行会宣言(byte[] 数据)
        {
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] > 行会职位.监事)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (数据.Length != 0 && 数据.Length < 101)
            {
                if (数据[0] == 0)
                {
                    this.所属行会.更改宣言(this.角色数据, "");
                }
                else
                {
                    this.所属行会.更改宣言(this.角色数据, 净化展示文本(Encoding.UTF8.GetString(数据).Split('\0')[0]));
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 更改行会公告. 错误: 数据长度错误"));
            }
        }

        public void 处理入会邀请(int 对象编号, byte 处理类型)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (角色数据.当前行会 != null && 角色数据.当前行会.邀请列表.Remove(this.角色数据))
                {
                    if (处理类型 == 2)
                    {
                        if (this.所属行会 != null)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 6707
                            });
                            return;
                        }
                        if (角色数据.所属行会.V.行会成员.Count >= 100)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6709
                            });
                            return;
                        }
                        角色数据.网络连接?.发送封包(new 行会邀请应答
                        {
                            对象名字 = this.对象名字,
                            应答类型 = 1
                        });
                        角色数据.当前行会.添加成员(this.角色数据);
                    }
                    else
                    {
                        角色数据.网络连接?.发送封包(new 行会邀请应答
                        {
                            对象名字 = this.对象名字,
                            应答类型 = 2
                        });
                    }
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6731
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 处理入会申请(int 对象编号, byte 处理类型)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if ((byte)this.所属行会.行会成员[this.角色数据] >= 6)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据)
            {
                if (!this.所属行会.申请列表.Remove(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6731
                    });
                }
                else if (处理类型 == 2)
                {
                    if (角色数据.当前行会 != null)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 6707
                        });
                    }
                    else
                    {
                        this.所属行会.添加成员(角色数据);
                        this.网络连接?.发送封包(new 入会申请应答
                        {
                            对象编号 = 角色数据.角色编号
                        });
                    }
                }
                else
                {
                    this.网络连接?.发送封包(new 入会申请应答
                    {
                        对象编号 = 角色数据.角色编号
                    });
                    角色数据.发送邮件(new 邮件数据(null, "入会申请被拒绝", "行会[" + this.所属行会.行会名字.V + "]拒绝了你的入会申请.", null));
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 申请加入行会(int 行会编号, string 行会名字)
        {
            if ((游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out var value) || 游戏数据网关.行会数据表.检索表.TryGetValue(行会名字, out value)) && value is 行会数据 行会数据)
            {
                if (this.所属行会 != null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6707
                    });
                    return;
                }
                if (this.当前等级 < 8)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6714
                    });
                    return;
                }
                if (行会数据.行会成员.Count >= 100)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6710
                    });
                    return;
                }
                if (行会数据.申请列表.Count > 20)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6703
                    });
                    return;
                }
                行会数据.申请列表[this.角色数据] = 主程.当前时间.AddHours(1.0);
                行会数据.行会提醒(行会职位.执事, 1);
                this.网络连接?.发送封包(new 加入行会应答
                {
                    行会编号 = 行会数据.行会编号
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 邀请加入行会(string 对象名字)
        {
            if (this.所属行会 != null)
            {
                foreach (KeyValuePair<角色数据, DateTime> item in this.所属行会.邀请列表.ToList())
                {
                    if (主程.当前时间 > item.Value)
                    {
                        this.所属行会.邀请列表.Remove(item.Key);
                    }
                }
            }
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] == 行会职位.会员)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (this.所属行会.行会成员.Count >= 100)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.角色数据表.检索表.TryGetValue(对象名字, out value) && value is 角色数据 角色数据)
            {
                if (!角色数据.角色在线(out var 网络))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6711
                    });
                    return;
                }
                if (角色数据.当前行会 != null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6707
                    });
                    return;
                }
                if (角色数据.角色等级 < 8)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6714
                    });
                    return;
                }
                this.所属行会.邀请列表[角色数据] = 主程.当前时间.AddHours(1.0);
                网络.发送封包(new 受邀加入行会
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字,
                    行会名字 = this.所属行会.行会名字.V
                });
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6713
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 查看申请列表()
        {
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else
            {
                this.网络连接?.发送封包(new 查看申请名单
                {
                    字节描述 = this.所属行会.入会申请描述()
                });
            }
        }

        public void 申请离开行会()
        {
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] == 行会职位.会长)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6718
                });
            }
            else
            {
                this.所属行会.退出行会(this.角色数据);
            }
        }

        public void 发放行会福利()
        {
        }

        public void 逐出行会成员(int 对象编号)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.地图编号 == 对象编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据 && this.所属行会 == 角色数据.当前行会)
            {
                if (this.所属行会.行会成员[this.角色数据] < 行会职位.长老 && this.所属行会.行会成员[this.角色数据] < this.所属行会.行会成员[角色数据])
                {
                    this.所属行会.逐出成员(this.角色数据, 角色数据);
                    角色数据.发送邮件(new 邮件数据(null, "你被逐出行会", "你被[" + this.所属行会.行会名字.V + "]的官员[" + this.对象名字 + "]逐出了行会.", null));
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6709
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 转移会长职位(int 对象编号)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] != 行会职位.会长)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6719
                });
            }
            else if (this.地图编号 == 对象编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6681
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据 && 角色数据.当前行会 == this.所属行会)
            {
                this.所属行会.转移会长(this.角色数据, 角色数据);
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 捐献行会资金(int 金币数量)
        {
        }

        public void 行会仓库刷新(int 仓库页面)
        {
            // 仅 0-5 合法, 越界查询直接返回空, 避免攻击者扫描非法页索引引起内部异常
            if (仓库页面 < 0 || 仓库页面 >= 6 || this.所属行会 == null) return;
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            List<KeyValuePair<int, 物品数据>> list;
            list = this.所属行会.行会仓库.Where((KeyValuePair<int, 物品数据> x) => x.Key >= 仓库页面 * 56 && x.Key < (仓库页面 + 1) * 56).ToList();
            binaryWriter.Write((byte)仓库页面);
            binaryWriter.Write((ushort)list.Count);
            binaryWriter.Write((byte)0);
            int num;
            num = 0;
            foreach (KeyValuePair<int, 物品数据> item in list)
            {
                memoryStream.Seek(num * 194 + 4, SeekOrigin.Begin);
                binaryWriter.Write((byte)(item.Key - 仓库页面 * 56));
                binaryWriter.Write(item.Value.字节描述());
                num++;
            }
            this.网络连接?.发送封包(new 仓库刷新应答
            {
                字节数据 = memoryStream.ToArray()
            });
        }

        public void 行会仓库转入(byte 原来容器, byte 原来位置, byte 仓库页面, byte 仓库位置)
        {
            if (this.所属行会 == null)
            {
                return;
            }
            // 仓库页面 必须在 0-5: 行会权限 enum 只定义 6 页(一存~六存 = bits 6-11).
            // 不限会让 `1 << (仓库页面+6)` 走 C# 移位的 `& 31` 截断, 在 仓库页面=26 时 mask 退化为 1,
            // 命中"取仓库一"权限位 → 任意成员凭"仓库一取"权限即可越权访问伪造页, 并污染行会仓库字典.
            if (原来容器 != 1 || 原来位置 >= this.背包大小 || 仓库位置 >= 56 || 仓库页面 >= 6)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 行会仓库转入, 错误: 参数越界"));
                return;
            }
            if (this.对话守卫 != null && this.当前地图 == this.对话守卫.当前地图 && base.网格距离(this.对话守卫.当前坐标) <= 12)
            {
                int num;
                num = 仓库页面 * 56 + 仓库位置;
                物品数据 v;
                if (((uint)this.所属行会.行会权限[this.所属行会.行会成员[this.角色数据]] & (uint)(1 << 仓库页面 + 6)) == 0)
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6709
                    });
                }
                else if (this.所属行会.行会仓库.ContainsKey(num))
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6736
                    });
                }
                else if (this.角色背包.TryGetValue(原来位置, out v))
                {
                    if (v.是否上锁)
                    {
                        base.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1890
                        });
                        return;
                    }
                    if (v.是否绑定)
                    {
                        base.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1804
                        });
                        return;
                    }
                    if (v is 装备数据 装备数据 && 装备数据.灵魂绑定.V)
                    {
                        base.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1804
                        });
                        return;
                    }
                    v.物品容器.V = 0;
                    v.物品位置.V = 0;
                    this.角色背包.Remove(原来位置);
                    this.所属行会.行会仓库.Add(num, v);
                    base.发送封包(new 删除玩家物品
                    {
                        背包类型 = 原来容器,
                        物品位置 = 原来位置
                    });
                    base.发送封包(new 转入行会仓库
                    {
                        仓库页面 = 仓库页面,
                        仓库位置 = 仓库位置,
                        物品详情 = v.字节描述()
                    });
                    主程.添加物品日志(this, "行会仓库转入", v, (v.持久类型 != 物品持久分类.堆叠) ? 1 : v.当前持久.V);
                }
            }
            else
            {
                this.对话守卫 = null;
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 3330
                });
            }
        }

        public void 行会仓库转出(byte 仓库页面, byte 仓库位置, byte 目标容器, byte 目标位置)
        {
            if (this.所属行会 == null)
            {
                return;
            }
            // 同 行会仓库转入: 仓库页面 必须在 0-5, 见 数据类/行会权限.cs (仓库一取~仓库六取 = bits 0-5)
            if (仓库位置 >= 56 || 目标容器 != 1 || 目标位置 >= this.背包大小 || 仓库页面 >= 6)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 行会仓库转出, 错误: 参数越界"));
                return;
            }
            if (this.对话守卫 != null && this.当前地图 == this.对话守卫.当前地图 && base.网格距离(this.对话守卫.当前坐标) <= 12)
            {
                int num;
                num = 仓库页面 * 56 + 仓库位置;
                物品数据 v;
                if (((uint)this.所属行会.行会权限[this.所属行会.行会成员[this.角色数据]] & (uint)(1 << (int)仓库页面)) == 0)
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6709
                    });
                }
                else if (this.角色背包.ContainsKey(目标位置))
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6736
                    });
                }
                else if (this.所属行会.行会仓库.TryGetValue(num, out v))
                {
                    v.物品容器.V = 目标容器;
                    v.物品位置.V = 目标位置;
                    this.所属行会.行会仓库.Remove(num);
                    this.角色背包.Add(目标位置, v);
                    base.发送封包(new 仓库转出应答
                    {
                        仓库页面 = 仓库页面,
                        仓库位置 = 仓库位置
                    });
                    base.发送封包(new 玩家物品变动
                    {
                        物品描述 = v.字节描述()
                    });
                    主程.添加物品日志(this, "行会仓库转出", v, (v.持久类型 != 物品持久分类.堆叠) ? 1 : v.当前持久.V);
                }
            }
            else
            {
                this.对话守卫 = null;
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 3330
                });
            }
        }

        public void 进入行会领地(byte 地图类型, int 行会编号)
        {
            if (this.所属行会 != null && this.所属行会.行会编号 == 行会编号)
            {
                this.所属行会.初始化公会领地();
                this.玩家切换地图(this.所属行会.公会属地, 地图区域类型.未知区域, new Point(967, 507));
            }
        }

        public void 设置行会禁言(int 对象编号, byte 禁言状态)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.地图编号 == 对象编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据 && 角色数据.当前行会 == this.所属行会)
            {
                if (this.所属行会.行会成员[this.角色数据] < 行会职位.理事 && this.所属行会.行会成员[this.角色数据] < this.所属行会.行会成员[角色数据])
                {
                    this.所属行会.成员禁言(this.角色数据, 角色数据, 禁言状态);
                    return;
                }
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 变更会员职位(int 对象编号, byte 对象职位)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.地图编号 == 对象编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6681
                });
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据 && 角色数据.当前行会 == this.所属行会)
            {
                if (this.所属行会.行会成员[this.角色数据] < 行会职位.理事 && this.所属行会.行会成员[this.角色数据] < this.所属行会.行会成员[角色数据])
                {
                    if (对象职位 > 1 && 对象职位 < 8 && 对象职位 != (byte)this.所属行会.行会成员[角色数据])
                    {
                        if (对象职位 == 2 && this.所属行会.行会成员.Values.Where((行会职位 O) => O == 行会职位.副长).Count() >= 2)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6717
                            });
                        }
                        else if (对象职位 == 3 && this.所属行会.行会成员.Values.Where((行会职位 O) => O == 行会职位.长老).Count() >= 4)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6717
                            });
                        }
                        else if (对象职位 == 4 && this.所属行会.行会成员.Values.Where((行会职位 O) => O == 行会职位.监事).Count() >= 4)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6717
                            });
                        }
                        else if (对象职位 == 5 && this.所属行会.行会成员.Values.Where((行会职位 O) => O == 行会职位.理事).Count() >= 4)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6717
                            });
                        }
                        else if (对象职位 == 6 && this.所属行会.行会成员.Values.Where((行会职位 O) => O == 行会职位.执事).Count() >= 4)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6717
                            });
                        }
                        else
                        {
                            this.所属行会.更改职位(this.角色数据, 角色数据, (行会职位)对象职位);
                        }
                    }
                    else
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 6704
                        });
                    }
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6709
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
            }
        }

        public void 申请行会外交(byte 外交类型, byte 外交时间, string 行会名字)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会名字.V == 行会名字)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6694
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] >= 行会职位.长老)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.行会数据表.检索表.TryGetValue(行会名字, out value) && value is 行会数据 行会数据)
            {
                if (this.所属行会.结盟行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6727
                    });
                }
                else if (this.所属行会.敌对行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6726
                    });
                }
                else if (外交时间 >= 1 && 外交时间 <= 3)
                {
                    switch (外交类型)
                    {
                        default:
                            this.网络连接?.尝试断开连接(new Exception("错误操作: 申请行会外交.  错误: 类型参数错误"));
                            break;
                        case 2:
                            this.所属行会.行会敌对(行会数据, 外交时间);
                            网络服务网关.发送公告($"[{this.所属行会}]和[{行会数据}]成为敌对行会.");
                            break;
                        case 1:
                            if (this.所属行会.结盟行会.Count >= 10)
                            {
                                this.网络连接?.发送封包(new 社交错误提示
                                {
                                    错误编号 = 6668
                                });
                            }
                            else if (行会数据.结盟行会.Count >= 10)
                            {
                                this.网络连接?.发送封包(new 社交错误提示
                                {
                                    错误编号 = 6668
                                });
                            }
                            else
                            {
                                this.所属行会.申请结盟(this.角色数据, 行会数据, 外交时间);
                            }
                            break;
                    }
                }
                else
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 申请行会外交.  错误: 时间参数错误"));
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 申请行会敌对(byte 敌对时间, string 行会名字)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会名字.V == 行会名字)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6694
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] >= 行会职位.长老)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.行会数据表.检索表.TryGetValue(行会名字, out value) && value is 行会数据 行会数据)
            {
                if (this.所属行会.结盟行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6727
                    });
                }
                else if (this.所属行会.敌对行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6726
                    });
                }
                else if (敌对时间 >= 1 && 敌对时间 <= 3)
                {
                    this.所属行会.行会敌对(行会数据, 敌对时间);
                    网络服务网关.发送公告($"[{this.所属行会}]和[{行会数据}]成为敌对行会.");
                }
                else
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 申请行会敌对.  错误: 时间参数错误"));
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 查看结盟申请()
        {
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else
            {
                this.网络连接?.发送封包(new 同步结盟申请
                {
                    字节描述 = this.所属行会.结盟申请描述()
                });
            }
        }

        public void 处理结盟申请(byte 处理类型, int 行会编号)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会编号 == 行会编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6694
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] >= 行会职位.长老)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out value) && value is 行会数据 行会数据)
            {
                if (this.所属行会.结盟行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6727
                    });
                    return;
                }
                if (this.所属行会.敌对行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6726
                    });
                    return;
                }
                if (!this.所属行会.结盟申请.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 6695
                    });
                    return;
                }
                switch (处理类型)
                {
                    default:
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 处理结盟申请.  错误: 处理类型错误."));
                        break;
                    case 2:
                        this.所属行会.行会结盟(行会数据);
                        网络服务网关.发送公告($"[{this.所属行会}]和[{行会数据}]成为结盟行会.");
                        this.所属行会.结盟申请.Remove(行会数据);
                        break;
                    case 1:
                        this.网络连接?.发送封包(new 结盟申请应答
                        {
                            行会编号 = 行会数据.行会编号
                        });
                        行会数据.发送邮件(行会职位.副长, "结盟申请被拒绝", "行会[" + this.所属行会.行会名字.V + "]拒绝了你所在行会的结盟申请.");
                        this.所属行会.结盟申请.Remove(行会数据);
                        break;
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 申请解除结盟(int 行会编号)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会编号 == 行会编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6694
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] >= 行会职位.长老)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out value) && value is 行会数据 行会数据)
            {
                if (!this.所属行会.结盟行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6728
                    });
                    return;
                }
                this.所属行会.解除结盟(this.角色数据, 行会数据);
                网络服务网关.发送公告($"[{this.所属行会}]解除了和[{行会数据}]的行会结盟.");
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 申请解除敌对(int 行会编号)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会编号 == 行会编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6694
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] >= 行会职位.长老)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out value) && value is 行会数据 行会数据)
            {
                if (!this.所属行会.敌对行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6826
                    });
                }
                else if (行会数据.解除申请.ContainsKey(this.所属行会))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6708
                    });
                }
                else
                {
                    this.所属行会.申请解敌(this.角色数据, 行会数据);
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 处理解除申请(int 行会编号, byte 应答类型)
        {
            游戏数据 value;
            if (this.所属行会 == null)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6668
                });
            }
            else if (this.所属行会.行会编号 == 行会编号)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6694
                });
            }
            else if (this.所属行会.行会成员[this.角色数据] >= 行会职位.长老)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6709
                });
            }
            else if (游戏数据网关.行会数据表.数据表.TryGetValue(行会编号, out value) && value is 行会数据 行会数据)
            {
                if (!this.所属行会.敌对行会.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6826
                    });
                }
                else if (!this.所属行会.解除申请.ContainsKey(行会数据))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5899
                    });
                }
                else if (应答类型 == 2)
                {
                    if (地图处理网关.沙城节点 >= 2 && ((this.所属行会 == 系统数据.数据.占领行会.V && 地图处理网关.攻城行会.Contains(行会数据)) || (行会数据 == 系统数据.数据.占领行会.V && 地图处理网关.攻城行会.Contains(this.所属行会))))
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 6800
                        });
                        return;
                    }
                    this.所属行会.解除敌对(行会数据);
                    网络服务网关.发送公告($"[{this.所属行会}]解除了和[{行会数据}]的行会敌对.");
                    this.所属行会.解除申请.Remove(行会数据);
                }
                else
                {
                    this.所属行会.发送封包(new 解除敌对列表
                    {
                        申请类型 = 2,
                        行会编号 = 行会数据.行会编号
                    });
                    this.所属行会.解除申请.Remove(行会数据);
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6669
                });
            }
        }

        public void 查询师门成员()
        {
            if (this.所属师门 != null)
            {
                this.网络连接?.发送封包(new 同步师门成员
                {
                    字节数据 = this.所属师门.成员数据()
                });
            }
        }

        public void 查询师门奖励()
        {
            if (this.所属师门 != null)
            {
                this.网络连接?.发送封包(new 同步师门奖励
                {
                    字节数据 = this.所属师门.奖励数据(this.角色数据)
                });
            }
        }

        public void 查询拜师名册()
        {
        }

        public void 查询收徒名册()
        {
        }

        public void 玩家申请拜师(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据 && 角色数据 != null)
            {
                客户网络 网络;
                if (this.所属师门 != null)
                {
                    客户网络 客户网络;
                    客户网络 = this.网络连接;
                    if (客户网络 != null)
                    {
                        客户网络?.发送封包(new 社交错误提示
                        {
                            错误编号 = 5895
                        });
                    }
                }
                else if (this.当前等级 >= 30)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5915
                    });
                }
                else if (角色数据.角色等级 < 30)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5894
                    });
                }
                else if (角色数据.当前师门 != null && 角色数据.角色编号 != 角色数据.当前师门.师父编号)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5890
                    });
                }
                else if (角色数据.当前师门 != null && 角色数据.当前师门.徒弟数量 >= 3)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5891
                    });
                }
                else if (角色数据.角色在线(out 网络))
                {
                    if (角色数据.当前师门 == null)
                    {
                        角色数据.当前师门 = new 师门数据(角色数据);
                    }
                    角色数据.当前师门.申请列表[this.地图编号] = 主程.当前时间;
                    this.网络连接?.发送封包(new 申请拜师应答
                    {
                        对象编号 = 角色数据.角色编号
                    });
                    网络.发送封包(new 申请拜师提示
                    {
                        对象编号 = this.地图编号
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5892
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 同意拜师申请(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                客户网络 网络;
                if (this.当前等级 < 30)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 同意拜师申请, 错误: 自身等级不够."));
                }
                else if (角色数据.角色等级 >= 30)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5894
                    });
                }
                else if (角色数据.当前师门 != null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5895
                    });
                }
                else if (this.所属师门 == null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 同意拜师申请, 错误: 尚未创建师门."));
                }
                else if (this.所属师门.师父编号 != this.地图编号)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 同意拜师申请, 错误: 自身尚未出师."));
                }
                else if (!this.所属师门.申请列表.ContainsKey(角色数据.角色编号))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5898
                    });
                }
                else if (this.所属师门.徒弟数量 >= 3)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5891
                    });
                }
                else if (角色数据.角色在线(out 网络))
                {
                    if (this.所属师门 == null)
                    {
                        this.所属师门 = new 师门数据(this.角色数据);
                    }
                    this.所属师门.添加徒弟(角色数据);
                    this.所属师门.发送封包(new 收徒成功提示
                    {
                        对象编号 = 角色数据.角色编号
                    });
                    this.网络连接?.发送封包(new 拜师申请通过
                    {
                        对象编号 = 角色数据.角色编号
                    });
                    this.网络连接?.发送封包(new 同步师门成员
                    {
                        字节数据 = this.所属师门.成员数据()
                    });
                    网络.发送封包(new 同步师门成员
                    {
                        字节数据 = this.所属师门.成员数据()
                    });
                    网络.发送封包(new 同步师门信息
                    {
                        师门参数 = 1
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5893
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 拒绝拜师申请(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (this.所属师门 == null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 拒绝拜师申请, 错误: 尚未创建师门."));
                    return;
                }
                if (this.所属师门.师父编号 != this.地图编号)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 拒绝拜师申请, 错误: 自身尚未出师."));
                    return;
                }
                if (!this.所属师门.申请列表.ContainsKey(角色数据.角色编号))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5898
                    });
                    return;
                }
                this.网络连接?.发送封包(new 拜师申请拒绝
                {
                    对象编号 = 角色数据.角色编号
                });
                if (this.所属师门.申请列表.Remove(角色数据.角色编号))
                {
                    角色数据.网络连接?.发送封包(new 拒绝拜师提示
                    {
                        对象编号 = this.地图编号
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 玩家申请收徒(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                客户网络 网络;
                if (this.当前等级 < 30)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家申请收徒, 错误: 自身等级不够."));
                }
                else if (角色数据.角色等级 >= 30)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5894
                    });
                }
                else if (角色数据.当前师门 != null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5895
                    });
                }
                else if (this.所属师门 != null && this.所属师门.师父编号 != this.地图编号)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家申请收徒, 错误: 自身尚未出师."));
                }
                else if (this.所属师门 != null && this.所属师门.徒弟数量 >= 3)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5891
                    });
                }
                else if (角色数据.角色在线(out 网络))
                {
                    if (this.所属师门 == null)
                    {
                        this.所属师门 = new 师门数据(this.角色数据);
                    }
                    this.所属师门.邀请列表[角色数据.角色编号] = 主程.当前时间;
                    this.网络连接?.发送封包(new 申请收徒应答
                    {
                        对象编号 = 角色数据.角色编号
                    });
                    网络.发送封包(new 申请收徒提示
                    {
                        对象编号 = this.地图编号,
                        对象等级 = this.当前等级,
                        对象声望 = this.师门声望
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5893
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 同意收徒申请(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                客户网络 网络;
                if (this.当前等级 > 30)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5915
                    });
                }
                else if (this.所属师门 != null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5895
                    });
                }
                else if (角色数据.角色等级 < 30)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 同意收徒申请, 错误: 对方等级不够."));
                }
                else if (角色数据.当前师门 == null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 同意收徒申请, 错误: 对方没有师门."));
                }
                else if (角色数据.当前师门.师父编号 != 角色数据.角色编号)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 同意收徒申请, 错误: 对方尚未出师."));
                }
                else if (!角色数据.当前师门.邀请列表.ContainsKey(this.地图编号))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5899
                    });
                }
                else if (角色数据.当前师门.徒弟数量 >= 3)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5891
                    });
                }
                else if (角色数据.角色在线(out 网络))
                {
                    this.网络连接?.发送封包(new 收徒申请同意
                    {
                        对象编号 = 角色数据.角色编号
                    });
                    if (角色数据.当前师门 == null)
                    {
                        角色数据.当前师门 = new 师门数据(角色数据);
                    }
                    网络.发送封包(new 收徒成功提示
                    {
                        对象编号 = this.地图编号
                    });
                    角色数据.当前师门.发送封包(new 收徒成功提示
                    {
                        对象编号 = this.地图编号
                    });
                    角色数据.当前师门.添加徒弟(this.角色数据);
                    this.网络连接?.发送封包(new 同步师门成员
                    {
                        字节数据 = 角色数据.当前师门.成员数据()
                    });
                    this.网络连接?.发送封包(new 同步师门信息
                    {
                        师门参数 = 1
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5892
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 拒绝收徒申请(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (角色数据.所属师门 == null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 拒绝收徒申请, 错误: 尚未创建师门."));
                    return;
                }
                if (角色数据.当前师门.师父编号 != 角色数据.角色编号)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 拒绝拜师申请, 错误: 自身尚未出师."));
                    return;
                }
                if (!角色数据.当前师门.邀请列表.ContainsKey(this.地图编号))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5899
                    });
                    return;
                }
                this.网络连接?.发送封包(new 收徒申请拒绝
                {
                    对象编号 = 角色数据.角色编号
                });
                if (角色数据.当前师门.邀请列表.Remove(this.地图编号))
                {
                    角色数据.网络连接?.发送封包(new 拒绝收徒提示
                    {
                        对象编号 = this.地图编号
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 逐出师门申请(int 对象编号)
        {
            游戏数据 value;
            if (this.所属师门 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 逐出师门申请, 错误: 自身没有师门."));
            }
            else if (this.所属师门.师父编号 != this.地图编号)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 逐出师门申请, 错误: 自己不是师父."));
            }
            else if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out value) && value is 角色数据 角色数据 && this.所属师门.师门成员.Contains(角色数据))
            {
                this.网络连接?.发送封包(new 逐出师门应答
                {
                    对象编号 = 角色数据.角色编号
                });
                this.所属师门.发送封包(new 逐出师门提示
                {
                    对象编号 = 角色数据.角色编号
                });
                uint num;
                num = (uint)this.所属师门.徒弟出师金币(角色数据);
                int num2;
                num2 = this.所属师门.徒弟出师经验(角色数据);
                if (地图处理网关.玩家对象表.TryGetValue(角色数据.角色编号, out var value2))
                {
                    value2.修改货币("+", 游戏货币.金币, num);
                    主程.添加货币日志(value2, "逐出师门处理", 游戏货币.金币, num);
                    value2.玩家增加经验(null, num2);
                }
                else
                {
                    角色数据.获得经验(num2);
                    角色数据.金币数量 += num;
                    主程.添加货币日志(角色数据, "逐出师门处理", 游戏货币.金币, num);
                }
                this.所属师门.移除徒弟(角色数据);
                角色数据.当前师门 = null;
                角色数据.网络连接?.发送封包(new 同步师门信息
                {
                    师门参数 = (byte)((角色数据.角色等级 >= 30) ? 2u : 0u)
                });
                角色数据.发送邮件(new 邮件数据(null, "你被逐出了师门", "你被[" + this.对象名字 + "]逐出了师门.", null));
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5913
                });
            }
        }

        public void 离开师门申请()
        {
            if (this.所属师门 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 离开师门申请, 错误: 自身没有师门."));
            }
            else if (this.所属师门.师门成员.Contains(this.角色数据))
            {
                this.网络连接?.发送封包(new 离开师门应答());
                this.所属师门.师父数据.网络连接?.发送封包(new 离开师门提示
                {
                    对象编号 = this.地图编号
                });
                this.所属师门.发送封包(new 离开师门提示
                {
                    对象编号 = this.地图编号
                });
                this.所属师门.师父数据.发送邮件(new 邮件数据(null, "徒弟叛离师门", "你的徒弟[" + this.对象名字 + "]已经叛离了师门.", null));
                uint num;
                num = (uint)this.所属师门.徒弟提供金币(this.角色数据);
                uint num2;
                num2 = (uint)this.所属师门.徒弟提供声望(this.角色数据);
                int num3;
                num3 = this.所属师门.徒弟提供经验(this.角色数据);
                if (地图处理网关.玩家对象表.TryGetValue(this.所属师门.师父数据.角色编号, out var value))
                {
                    value.修改货币("+", 游戏货币.金币, num);
                    主程.添加货币日志(value, "离开师门增加", 游戏货币.金币, num);
                    value.师门声望 += num2;
                    value.玩家增加经验(null, num3);
                }
                else
                {
                    this.所属师门.师父数据.获得经验(num3);
                    this.所属师门.师父数据.金币数量 += num;
                    主程.添加货币日志(this.所属师门.师父数据, "离开师门增加", 游戏货币.金币, num);
                    this.所属师门.师父数据.师门声望 += num2;
                }
                this.所属师门.移除徒弟(this.角色数据);
                this.角色数据.当前师门 = null;
                this.网络连接?.发送封包(new 同步师门信息
                {
                    师门参数 = this.师门参数
                });
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 离开师门申请, 错误: 自身不是徒弟."));
            }
        }

        public void 提交出师申请()
        {
            if (this.所属师门 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 提交出师申请, 错误: 自身没有师门."));
                return;
            }
            if (this.当前等级 < 30)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 提交出师申请, 错误: 自身等级不足."));
                return;
            }
            if (!this.所属师门.师门成员.Contains(this.角色数据))
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 提交出师申请, 错误: 自己不是徒弟."));
                return;
            }
            uint num;
            num = (uint)this.所属师门.徒弟提供金币(this.角色数据);
            uint num2;
            num2 = (uint)this.所属师门.徒弟提供声望(this.角色数据);
            int num3;
            num3 = this.所属师门.徒弟提供经验(this.角色数据);
            if (地图处理网关.玩家对象表.TryGetValue(this.所属师门.师父数据.角色编号, out var value))
            {
                value.修改货币("+", 游戏货币.金币, num);
                主程.添加货币日志(value, "提交出师申请", 游戏货币.金币, num);
                value.师门声望 += num2;
                value.玩家增加经验(null, num3);
            }
            else
            {
                this.所属师门.师父数据.获得经验(num3);
                this.所属师门.师父数据.金币数量 += num;
                主程.添加货币日志(this.所属师门.师父数据, "提交出师申请", 游戏货币.金币, num);
                this.所属师门.师父数据.师门声望 += num2;
            }
            this.修改货币("+", 游戏货币.金币, (uint)this.所属师门.徒弟出师金币(this.角色数据));
            主程.添加货币日志(this, "提交出师申请", 游戏货币.金币, (uint)this.所属师门.徒弟出师金币(this.角色数据));
            this.玩家增加经验(null, this.所属师门.徒弟出师经验(this.角色数据));
            this.所属师门.师父数据.网络连接?.发送封包(new 徒弟成功出师
            {
                对象编号 = this.地图编号
            });
            this.所属师门.移除徒弟(this.角色数据);
            this.角色数据.当前师门 = null;
            this.网络连接?.发送封包(new 徒弟成功出师
            {
                对象编号 = this.地图编号
            });
            this.网络连接?.发送封包(new 清空师门信息());
            this.网络连接?.发送封包(new 同步师门信息
            {
                师门参数 = this.师门参数
            });
        }

        public void 更改收徒推送(bool 收徒推送)
        {
        }

    }
}
