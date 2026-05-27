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
        public void 玩家申请交易(int 对象编号)
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                玩家实例 value;
                if (this.当前等级 < 30 && this.本期特权 == 0)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 65538
                    });
                }
                else if (对象编号 == this.地图编号)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家申请交易. 错误: 不能交易自己"));
                }
                else if (!地图处理网关.玩家对象表.TryGetValue(对象编号, out value))
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5635
                    });
                }
                else if (this.当前地图 != value.当前地图)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5636
                    });
                }
                else if (base.网格距离(value) > 12)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5636
                    });
                }
                else if (!value.对象死亡 && value.摆摊状态 == 0 && value.交易状态 < 3)
                {
                    this.当前交易?.结束交易();
                    value.当前交易?.结束交易();
                    this.当前交易 = (value.当前交易 = new 玩家交易(this, value));
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5633
                    });
                }
                else
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5637
                    });
                }
            }
            else
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
        }

        public void 玩家同意交易(int 对象编号)
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (!this.对象死亡 && this.摆摊状态 == 0 && this.交易状态 == 2)
            {
                玩家实例 value;
                if (this.当前等级 < 30 && this.本期特权 == 0)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 65538
                    });
                }
                else if (对象编号 == this.地图编号)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家申请交易. 错误: 不能交易自己"));
                }
                else if (!地图处理网关.玩家对象表.TryGetValue(对象编号, out value))
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5635
                    });
                }
                else if (this.当前地图 != value.当前地图)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5636
                    });
                }
                else if (base.网格距离(value) > 12)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5636
                    });
                }
                else if (!value.对象死亡 && value.摆摊状态 == 0 && value.交易状态 == 1)
                {
                    if (value == this.当前交易.交易申请方 && this == value.当前交易.交易接收方)
                    {
                        this.当前交易.更改状态(3);
                        return;
                    }
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5634
                    });
                }
                else
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 5637
                    });
                }
            }
            else
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
        }

        public void 玩家结束交易()
        {
            this.当前交易?.结束交易();
        }

        public void 玩家放入金币(int 金币数量)
        {
            if (this.交易状态 != 3)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
            else if (this.当前地图 != this.当前交易.对方玩家(this).当前地图)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (base.网格距离(this.当前交易.对方玩家(this)) > 12)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (金币数量 > 0 && 金币数量 <= 1_000_000_000
                && this.金币数量 >= (uint)金币数量 + (uint)Math.Ceiling((float)金币数量 * 0.04f))
            {
                // 上限 10 亿 + 改 uint 加法, 防止 客户端 int 接近 MaxValue 时
                // 加手续费溢出成负数, 再被 uint vs 负 long 比较意外通过校验 → 凭空放入海量金币
                if (this.当前交易.金币重复(this))
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入金币. 错误: 重复放入金币"));
                }
                else
                {
                    this.当前交易.放入金币(this, 金币数量);
                }
            }
            else
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入金币. 错误: 金币数量错误"));
            }
        }

        public void 玩家放入物品(byte 放入位置, byte 放入物品, byte 背包类型, byte 物品位置)
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            物品数据 v;
            if (this.交易状态 != 3)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
            else if (this.当前地图 != this.当前交易.对方玩家(this).当前地图)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (base.网格距离(this.当前交易.对方玩家(this)) > 12)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (放入位置 >= 6)
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 放入位置错误"));
            }
            else if (this.当前交易.物品重复(this, 放入位置))
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 放入位置重复"));
            }
            else if (放入物品 != 1)
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 禁止取回物品"));
            }
            else if (背包类型 != 1)
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 背包类型错误"));
            }
            else if (!this.角色背包.TryGetValue(物品位置, out v))
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 物品数据错误"));
            }
            else if (v.是否绑定)
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 放入绑定物品"));
            }
            else if (this.当前交易.物品重复(this, v))
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品. 错误: 重复放入物品"));
            }
            else if (v.是否上锁)
            {
                this.当前交易?.结束交易();
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家放入物品, 错误: 放入上锁物品"));
            }
            else
            {
                this.当前交易.放入物品(this, v, 放入位置);
            }
        }

        public void 玩家锁定交易()
        {
            if (this.交易状态 != 3)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
            else if (this.当前地图 != this.当前交易.对方玩家(this).当前地图)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (base.网格距离(this.当前交易.对方玩家(this)) > 12)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else
            {
                this.当前交易.更改状态(4, this);
            }
        }

        public void 玩家解锁交易()
        {
            if (this.交易状态 < 4)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
            else if (this.当前地图 != this.当前交易.对方玩家(this).当前地图)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (base.网格距离(this.当前交易.对方玩家(this)) > 12)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else
            {
                this.当前交易.更改状态(3);
            }
        }

        public void 玩家确认交易()
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            玩家实例 玩家;
            if (this.交易状态 != 4)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5634
                });
            }
            else if (this.当前地图 != this.当前交易.对方玩家(this).当前地图)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (base.网格距离(this.当前交易.对方玩家(this)) > 12)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5636
                });
            }
            else if (this.当前交易.对方状态(this) != 5)
            {
                this.当前交易.更改状态(5, this);
            }
            else if (this.当前交易.背包已满(out 玩家))
            {
                this.当前交易?.结束交易();
                this.当前交易?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5639,
                    第一参数 = 玩家.地图编号
                });
            }
            else
            {
                this.当前交易.更改状态(5, this);
                this.当前交易.交换物品();
            }
        }

        public void 玩家准备摆摊()
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (!this.对象死亡 && this.交易状态 < 3)
            {
                if (this.当前等级 < 30 && this.本期特权 == 0)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 65538
                    });
                }
                else if (this.当前摊位 != null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 2825
                    });
                }
                else if (!this.当前地图.摆摊区内(this.当前坐标))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 2818
                    });
                }
                else if (this.当前地图[this.当前坐标].FirstOrDefault((地图对象 O) => O is 玩家实例 玩家实例2 && 玩家实例2.当前摊位 != null) != null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 2819
                    });
                }
                else
                {
                    this.当前摊位 = new 玩家摊位();
                    base.发送封包(new 摆摊状态改变
                    {
                        对象编号 = this.地图编号,
                        摊位状态 = 1
                    });
                }
            }
        }

        public void 玩家重整摊位()
        {
            if (this.摆摊状态 != 2)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2817
                });
                return;
            }
            this.当前摊位.摊位状态 = 1;
            base.发送封包(new 摆摊状态改变
            {
                对象编号 = this.地图编号,
                摊位状态 = this.摆摊状态
            });
        }

        public void 玩家开始摆摊()
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (this.摆摊状态 != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2817
                });
            }
            else if (this.当前等级 < 30 && this.本期特权 == 0)
            {
                this.当前交易?.结束交易();
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 65538
                });
            }
            else if (this.当前摊位.物品总价() + this.金币数量 > 2147483647L)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2827
                });
            }
            else
            {
                this.当前摊位.摊位状态 = 2;
                base.发送封包(new 摆摊状态改变
                {
                    对象编号 = this.地图编号,
                    摊位状态 = this.摆摊状态
                });
            }
        }

        public void 玩家收起摊位()
        {
            if (this.摆摊状态 == 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2817
                });
                return;
            }
            this.当前摊位 = null;
            base.发送封包(new 摆摊状态改变
            {
                对象编号 = this.地图编号,
                摊位状态 = this.摆摊状态
            });
        }

        public void 放入摊位物品(byte 放入位置, byte 背包类型, byte 物品位置, ushort 物品数量, int 物品价格)
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (this.摆摊状态 != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2817
                });
                return;
            }
            if (放入位置 >= 10)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 放入位置错误"));
                return;
            }
            if (this.当前摊位.摊位物品.ContainsKey(放入位置))
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 重复放入位置"));
                return;
            }
            if (背包类型 != 1)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 背包类型错误"));
                return;
            }
            if (物品价格 < 100 || 物品价格 > 1000000000)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 物品价格错误"));
                return;
            }
            if (物品数量 <= 0)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 物品数量错误"));
                return;
            }
            if (!this.角色背包.TryGetValue(物品位置, out var v))
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 选中物品为空"));
                return;
            }
            if (this.当前摊位.摊位物品.Values.FirstOrDefault((物品数据 O) => O.物品位置.V == 物品位置) != null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 重复放入物品"));
                return;
            }
            if (v.是否绑定)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 放入绑定物品"));
                return;
            }
            if (物品数量 > ((!v.能否堆叠) ? 1 : v.当前持久.V))
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 物品数量错误"));
                return;
            }
            if (v.是否上锁)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 放入摊位物品, 错误: 放入上锁物品"));
                return;
            }
            this.当前摊位.摊位物品.Add(放入位置, v);
            this.当前摊位.物品数量.Add(v, 物品数量);
            this.当前摊位.物品单价.Add(v, 物品价格);
            this.网络连接?.发送封包(new 添加摆摊物品
            {
                放入位置 = 放入位置,
                背包类型 = 背包类型,
                物品位置 = 物品位置,
                物品数量 = 物品数量,
                物品价格 = 物品价格
            });
        }

        public void 取回摊位物品(byte 取回位置)
        {
            if (this.摆摊状态 != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2817
                });
                return;
            }
            if (!this.当前摊位.摊位物品.TryGetValue(取回位置, out var value))
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 取回摊位物品, 错误: 选中物品为空"));
                return;
            }
            this.当前摊位.物品单价.Remove(value);
            this.当前摊位.物品数量.Remove(value);
            this.当前摊位.摊位物品.Remove(取回位置);
            this.网络连接?.发送封包(new 移除摆摊物品
            {
                取回位置 = 取回位置
            });
        }

        public void 更改摊位名字(string 摊位名字)
        {
            if (this.摆摊状态 != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2817
                });
                return;
            }
            // LOW-A 续: 摊位名字会广播到摆摊区其他玩家 UI, 复用公会师门 partial 中的 净化展示文本
            string 净化后 = 净化展示文本(摊位名字);
            this.当前摊位.摊位名字 = 净化后;
            base.发送封包(new 变更摊位名字
            {
                对象编号 = this.地图编号,
                摊位名字 = 净化后
            });
        }

        public void 升级摊位外观(byte 外观编号)
        {
        }

        public void 玩家打开摊位(int 对象编号)
        {
            if (!地图处理网关.玩家对象表.TryGetValue(对象编号, out var value))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2828
                });
            }
            else if (value.摆摊状态 != 2)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2828
                });
            }
            else
            {
                this.网络连接?.发送封包(new 同步摊位数据
                {
                    对象编号 = value.地图编号,
                    字节数据 = value.当前摊位.摊位描述()
                });
            }
        }

        public void 购买摊位物品(int 对象编号, byte 物品位置, ushort 购买数量)
        {
            if (购买数量 <= 0)
            {
                return;
            }
            if (!地图处理网关.玩家对象表.TryGetValue(对象编号, out var value))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2828
                });
                return;
            }
            if (value.摆摊状态 != 2)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2828
                });
                return;
            }
            if (!value.当前摊位.摊位物品.TryGetValue(物品位置, out var value2))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2824
                });
                return;
            }
            if (value.当前摊位.物品数量[value2] < 购买数量)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2830
                });
                return;
            }
            long 总价检查;
            总价检查 = (long)value.当前摊位.物品单价[value2] * (long)购买数量;
            if (总价检查 <= 0 || 总价检查 > int.MaxValue)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 购买摊位物品, 错误: 总价计算溢出"));
                return;
            }
            if ((long)this.金币数量 < 总价检查)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2561
                });
                return;
            }
            byte b;
            b = byte.MaxValue;
            byte b2;
            b2 = 0;
            while (b2 < this.背包大小)
            {
                if (this.角色背包.ContainsKey(b2))
                {
                    b2++;
                    continue;
                }
                b = b2;
                break;
            }
            if (b == byte.MaxValue)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return;
            }
            int num;
            num = (int)总价检查;
            this.扣金币((uint)num);
            主程.添加货币日志(this, "购买摊位物品->" + value2.物品名字, 游戏货币.金币, -num);
            this.角色数据.转出金币.V += num;
            value.修改货币("+", 游戏货币.金币, (uint)((float)num * 0.95f));
            主程.添加货币日志(value, "出售摊位物品->" + value2.物品名字, 游戏货币.金币, num);
            if ((value.当前摊位.物品数量[value2] -= 购买数量) <= 0)
            {
                主程.添加物品日志(value, "出售摊位物品", value2, 1, "购买者:" + this.对象名字);
                value.角色背包.Remove(value2.物品位置.V);
                value.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 1,
                    物品位置 = value2.物品位置.V
                });
            }
            else
            {
                value2.当前持久.V -= 购买数量;
                value.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = value2.字节描述()
                });
            }
            if (value.当前摊位.物品数量[value2] <= 0)
            {
                this.角色背包[b] = value2;
                value2.物品位置.V = b;
                value2.物品容器.V = 1;
            }
            else
            {
                this.角色背包[b] = new 物品数据(value2.物品模板, value2.生成来源.V, 1, b, 购买数量, 绑定: false, value2.掉落怪物.V);
                this.角色背包[b].掉落地图.V = value2.掉落地图.V;
            }
            主程.添加物品日志(this, "购买摊位物品", this.角色背包[b], 1, "摊主:" + value.对象名字);
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = this.角色背包[b].字节描述()
            });
            this.网络连接?.发送封包(new 购入摊位物品
            {
                对象编号 = value.地图编号,
                物品位置 = 物品位置,
                剩余数量 = value.当前摊位.物品数量[value2]
            });
            value.网络连接?.发送封包(new 售出摊位物品
            {
                物品位置 = 物品位置,
                售出数量 = 购买数量,
                售出收益 = (int)((float)num * 0.95f)
            });
            主程.添加系统日志($"[{this.对象名字}][{this.当前等级}级] 购买了 [{value.对象名字}][{value.当前等级}级] 的摊位物品[{this.角色背包[b]}] * {购买数量}, 花费金币[{num}]");
            if (value.当前摊位.物品数量[value2] <= 0)
            {
                value.当前摊位.摊位物品.Remove(物品位置);
                value.当前摊位.物品单价.Remove(value2);
                value.当前摊位.物品数量.Remove(value2);
            }
            if (value.当前摊位.物品数量.Count <= 0)
            {
                value.玩家收起摊位();
            }
        }
    }
}
