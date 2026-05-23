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
using DevExpress.XtraSpreadsheet.Import.Xls;

namespace 游戏服务器.地图类
{
    public sealed class 玩家实例 : 地图对象
    {
        private struct 洗练
        {
            public byte 阶段;

            public 龙卫词缀类型 词缀;
        }
        #region 定义
        //挖矿
        public int 挖矿次数;
        public DateTime 挖矿时间间隔;
        public DateTime 秒检测间隔;

        public 角色数据 角色数据;

        public 铭文技能 洗练铭文;

        public 玩家交易 当前交易;

        public 玩家摊位 当前摊位;

        public byte 雕色部位;

        public byte 重铸部位;

        public int 对话页面;

        public string 对话触发;

        public 守卫实例 对话守卫;

        public DateTime 对话超时;

        public int 打开商店;

        public string 打开界面;

        public int 回血总量;

        public int 回魔总量;

        public DateTime 邮件时间;

        public DateTime 药品回血;

        public DateTime 药品回魔;

        public DateTime 称号时间;

        public DateTime 特权时间;

        public DateTime 拾取时间;

        public DateTime 队伍时间;

        public DateTime 战具计时;

        public DateTime 经验计时;

        public byte 自动拾取范围;

        public int 自动拾取间隔;

        public List<物品数据> 回购清单;

        public List<宠物实例> 宠物列表;

        public List<定时器数据> 定时器列表;

        public Dictionary<object, int> 战力加成;

        public readonly Dictionary<ushort, 技能数据> 被动技能;

        public decimal 额外爆率;

        public bool isNewHuman;

        public string 会话ID = "";

        public bool 是否假人;

        public int 御兽之力等级;

        public int 最后杀死的怪物编号;

        public string 最后杀死的玩家名字;

        public string 最后杀死自己的玩家名字;

        public bool 自动战斗;

        public Point 启动位置;

        public Point 目标位置;

        public 地图对象 攻击目标;

        public DateTime 挂机间隔;

        public DateTime 收益间隔;

        public ushort 挂机范围;

        public bool 开启收益检测;

        public short 收益检测时间;

        public int 传送物品;

        public int 释放技能;

        public bool 自动拾取;

        public byte 拾取范围;

        public bool 背包预留;

        public byte 预留格数;

        public bool 优先战斗;

        public bool 不捡他人归属;

        public bool 不打他人归属;

        public HashSet<int> 物品过滤;

        public HashSet<int> 物品极品提示;

        public bool 无敌模式;

        public bool 隐身模式;

        public DateTime 执行间隔;

        public bool 验证密码;

        public byte 密码错误次数;

        public ushort 测试变量;

        public bool 操作道具;

        public ChestPlayerOpener 探索道具;

        public byte 随机次数;

        public int NPCObjectID;

        public int NPCScriptID;

        public NPCPage NPCPage;

        public Dictionary<NPCSegment, bool> NPCSuccess = new Dictionary<NPCSegment, bool>();

        public bool NPCDelayed;

        public List<string> NPCSpeech = new List<string>();

        public Dictionary<string, object> NPCData = new Dictionary<string, object>();

        public long 觉醒存储上限 = 2100000000L;

        public byte 传承之力;

        public byte 传承之力外观;

        public DateTime 物理击回间隔;

        public DateTime 魔法击回间隔;

        public ushort[] 临时精炼;

        public DateTime 硬直检测;

        public int 硬直次数;

        public Dictionary<ushort, int> 生效龙卫;

        public ushort 勋章洗炼;

        public 装备数据 洗炼勋章;

        private Dictionary<string, DateTime> 下次请求时间 = new Dictionary<string, DateTime>();

        private int 请求间隔毫秒 = 100;

        private static DateTime 下次恢复;

        private static int 恢复量;

        private Dictionary<int, int> 刻印部位列表 = new Dictionary<int, int>
        {
            { 12, 4 },
            { 13, 8 },
            { 9, 16 },
            { 3, 4096 }
        };

        public DateTime 挂机_结束时间;

        public DateTime 挂机_目标_超时时间;

        public Dictionary<地图对象, DateTime> 挂机_目标_黑名单 = new Dictionary<地图对象, DateTime>();

        private int 远攻技能数 = -1;

        private DateTime 攻击间隔;

        private int 开关间隔时间 = 300;

        private int 攻击间隔时间 = 900;

        private bool 无收益跳转中;

        private DateTime 无收益跳转延时;

        private DateTime 下次经验判断;

        private long 上次经验值;

        private HashSet<int> 挂机已分解物品;

        private DateTime 下次分解时间;

        private bool 分解完成;
        #endregion
        private static readonly Dictionary<游戏对象职业, List<游戏基础技能>> 技能使用顺序 = new Dictionary<游戏对象职业, List<游戏基础技能>>
        {
            {
                游戏对象职业.战士,
                new List<游戏基础技能>
                {
                    游戏基础技能.觉醒金钟罩,
                    游戏基础技能.觉醒天威剑法,
                    游戏基础技能.觉醒雷霆剑法,
                    游戏基础技能.神威盾甲,
                    游戏基础技能.爆炎剑诀,
                    游戏基础技能.逐日剑法,
                    游戏基础技能.烈火剑法,
                    游戏基础技能.半月弯刀,
                    游戏基础技能.刺杀剑术,
                    游戏基础技能.战士普攻
                }
            },
            {
                游戏对象职业.刺客,
                new List<游戏基础技能>
                {
                    游戏基础技能.致残毒药,
                    游戏基础技能.暴击之术,
                    游戏基础技能.炎龙啸波,
                    游戏基础技能.火镰狂舞,
                    游戏基础技能.献祭,
                    游戏基础技能.觉醒魔刃天旋,
                    游戏基础技能.追魂镖,
                    游戏基础技能.觉醒暗影守卫,
                    游戏基础技能.觉醒猎命宣告,
                    游戏基础技能.刺客普攻
                }
            },
            {
                游戏对象职业.弓手,
                new List<游戏基础技能>
                {
                    游戏基础技能.战术标记,
                    游戏基础技能.守护箭羽,
                    游戏基础技能.强袭之箭,
                    游戏基础技能.回避射击,
                    游戏基础技能.三发散射,
                    游戏基础技能.觉醒万箭穿心,
                    游戏基础技能.觉醒羿神庇佑,
                    游戏基础技能.基础射击,
                    游戏基础技能.弓手普攻
                }
            },
            {
                游戏对象职业.龙枪,
                new List<游戏基础技能>
                {
                    游戏基础技能.御龙晶甲,
                    游戏基础技能.神威镇域,
                    游戏基础技能.百战军魂,
                    游戏基础技能.凌云枪法,
                    游戏基础技能.盘龙枪势,
                    游戏基础技能.枪出如龙,
                    游戏基础技能.狂飙突刺,
                    游戏基础技能.龙啸千里,
                    游戏基础技能.横扫六合,
                    游戏基础技能.龙枪普攻
                }
            },
            {
                游戏对象职业.法师,
                new List<游戏基础技能>
                {
                    游戏基础技能.魔法护盾,
                    游戏基础技能.觉醒法神奥义,
                    游戏基础技能.觉醒魔能星陨,
                    游戏基础技能.疾光电影,
                    游戏基础技能.流星火雨,
                    游戏基础技能.冰咆哮,
                    游戏基础技能.雷电之术,
                    游戏基础技能.小火球术,
                    游戏基础技能.法师普攻
                }
            },
            {
                游戏对象职业.道士,
                new List<游戏基础技能>
                {
                    游戏基础技能.觉醒召唤月灵,
                    游戏基础技能.召唤神兽,
                    游戏基础技能.召唤骷髅,
                    游戏基础技能.觉醒阴阳道盾,
                    游戏基础技能.无极真气,
                    游戏基础技能.神圣战甲,
                    游戏基础技能.幽灵之盾,
                    游戏基础技能.施毒术,
                    游戏基础技能.隐身之术,
                    游戏基础技能.灵魂火符,
                    游戏基础技能.噬血术,
                    游戏基础技能.道士普攻
                }
            }
        };

        public int 脚本爆率
        {
            get
            {
                return this.角色数据.脚本爆率.V;
            }
            set
            {
                this.角色数据.脚本爆率.V = value;
            }
        }

        public bool 管理员模式 => this.角色数据.管理员角色.V;

        public bool 商人模式 => this.角色数据.商人角色.V;

        public 客户网络 网络连接 => this.角色数据.网络连接;

        public byte 交易状态
        {
            get
            {
                if (this.当前交易 == null)
                {
                    return 0;
                }
                if (this.当前交易.交易申请方 == this)
                {
                    return this.当前交易.申请方状态;
                }
                return this.当前交易.接收方状态;
            }
        }

        public byte 摆摊状态
        {
            get
            {
                if (this.当前摊位 != null)
                {
                    return this.当前摊位.摊位状态;
                }
                return 0;
            }
            set
            {
                if (this.当前摊位 != null)
                {
                    this.当前摊位.摊位状态 = value;
                }
            }
        }

        public string 摊位名字
        {
            get
            {
                if (this.当前摊位 != null)
                {
                    return this.当前摊位.摊位名字;
                }
                return "";
            }
        }

        public override string 对象名字 => this.角色数据.角色名字.V;

        public override int 地图编号 => this.角色数据.数据索引.V;

        public override int 当前体力
        {
            get
            {
                return this.角色数据.当前血量.V;
            }
            set
            {
                value = Math.Min(this[游戏对象属性.最大体力], Math.Max(0, value));
                if (this.当前体力 != value)
                {
                    this.角色数据.当前血量.V = value;
                    base.发送封包(new 同步对象体力
                    {
                        对象编号 = this.地图编号,
                        当前体力 = this.当前体力,
                        体力上限 = this[游戏对象属性.最大体力]
                    });
                }
            }
        }

        public override int 当前魔力
        {
            get
            {
                return this.角色数据.当前蓝量.V;
            }
            set
            {
                value = Math.Min(this[游戏对象属性.最大魔力], Math.Max(0, value));
                if (this.当前魔力 != value)
                {
                    this.角色数据.当前蓝量.V = Math.Max(0, value);
                    this.网络连接?.发送封包(new 同步对象魔力
                    {
                        当前魔力 = this.当前魔力
                    });
                }
            }
        }

        public override byte 当前等级
        {
            get
            {
                return this.角色数据.角色等级;
            }
            set
            {
                this.角色数据.角色等级 = value;
            }
        }

        public override Point 当前坐标
        {
            get
            {
                return this.角色数据.当前坐标.V;
            }
            set
            {
                if (this.角色数据.当前坐标.V != value)
                {
                    this.角色数据.当前坐标.V = value;
                    if (this.重生地图 != this.当前地图.地图编号 && (this.当前地图?.复活区域?.范围坐标.Contains(this.当前坐标)).GetValueOrDefault())
                    {
                        this.重生地图 = this.当前地图.地图编号;
                        base.发送封包(new 激活复活区域
                        {
                            X = 计算类.点阵坐标转协议坐标(this.当前坐标.X),
                            Y = 计算类.点阵坐标转协议坐标(this.当前坐标.Y),
                            Z = 计算类.点阵坐标转协议坐标(this.当前高度)
                        });
                    }
                }
            }
        }

        public override 地图实例 当前地图
        {
            get
            {
                return base.当前地图;
            }
            set
            {
                if (this.当前地图 != value)
                {
                    base.当前地图?.移除对象(this);
                    base.当前地图 = value;
                    base.当前地图?.添加对象(this);
                }
                if (this.角色数据.当前地图.V != value.地图模板.地图编号)
                {
                    this.角色数据.当前地图.V = value.地图模板.地图编号;
                    this.所属行会?.发送封包(new 同步会员信息
                    {
                        对象编号 = this.地图编号,
                        对象信息 = this.角色数据.当前地图.V,
                        当前等级 = this.当前等级
                    });
                }
            }
        }

        public override 游戏方向 当前方向
        {
            get
            {
                return this.角色数据.当前朝向.V;
            }
            set
            {
                if (this.角色数据.当前朝向.V != value)
                {
                    this.角色数据.当前朝向.V = value;
                    base.发送封包(new 对象转动方向
                    {
                        对象编号 = this.地图编号,
                        对象朝向 = (ushort)value
                    });
                }
            }
        }

        public override 游戏对象类型 对象类型 => 游戏对象类型.玩家;

        public override 技能范围类型 对象体型 => 技能范围类型.单体1x1;

        public override int 奔跑耗时 => base.奔跑速度 * (this.自动战斗 ? 52 : 45);

        public override int 行走耗时 => base.行走速度 * (this.自动战斗 ? 52 : 45);

        public override DateTime 忙碌时间
        {
            get
            {
                return base.忙碌时间;
            }
            set
            {
                if (base.忙碌时间 < value)
                {
                    base.忙碌时间 = (this.硬直时间 = value);
                }
            }
        }

        public override DateTime 硬直时间
        {
            get
            {
                return base.硬直时间;
            }
            set
            {
                if (base.硬直时间 < value)
                {
                    base.硬直时间 = value;
                }
            }
        }

        public override DateTime 行走时间
        {
            get
            {
                return base.行走时间;
            }
            set
            {
                if (base.行走时间 < value)
                {
                    base.行走时间 = value;
                }
            }
        }

        public override DateTime 奔跑时间
        {
            get
            {
                return base.奔跑时间;
            }
            set
            {
                if (base.奔跑时间 < value)
                {
                    base.奔跑时间 = value;
                }
            }
        }

        public override int this[游戏对象属性 属性]
        {
            get
            {
                return base[属性];
            }
            set
            {
                if (base[属性] != value)
                {
                    base[属性] = value;
                    if ((byte)属性 <= 200)
                    {
                        this.网络连接?.发送封包(new 同步属性变动
                        {
                            属性编号 = (byte)属性,
                            属性数值 = value
                        });
                    }
                }
            }
        }

        public override 字典监视器<ushort, Buff数据> Buff列表 => this.角色数据.Buff数据;

        public override 字典监视器<int, DateTime> 冷却记录 => this.角色数据.冷却数据;

        public 字典监视器<ushort, 技能数据> 主体技能表 => this.角色数据.技能数据;

        public int 最大负重 => this[游戏对象属性.最大负重];

        public int 最大穿戴 => this[游戏对象属性.最大穿戴];

        public int 最大腕力 => this[游戏对象属性.最大腕力];

        public int 背包重量
        {
            get
            {
                int num;
                num = 0;
                foreach (物品数据 item in this.角色背包.Values.ToList())
                {
                    num += item?.物品重量 ?? 0;
                }
                return num;
            }
        }

        public int 装备重量
        {
            get
            {
                int num;
                num = 0;
                foreach (装备数据 item in this.角色装备.Values.ToList())
                {
                    num += ((item != null && item.物品类型 != 物品使用分类.武器) ? item.物品重量 : 0);
                }
                return num;
            }
        }

        public int 当前战力
        {
            get
            {
                return this.角色数据.角色战力;
            }
            set
            {
                this.角色数据.角色战力 = value;
            }
        }

        public long 当前经验
        {
            get
            {
                return this.角色数据.角色经验;
            }
            set
            {
                this.角色数据.角色经验 = value;
            }
        }

        public long 当前觉醒经验
        {
            get
            {
                return this.角色数据.觉醒经验;
            }
            set
            {
                this.角色数据.觉醒经验 = ((value > 2147483647L) ? 2147483646L : value);
            }
        }

        public bool 开启觉醒经验存储
        {
            get
            {
                return (this.五零变量 & 0x40) == 64;
            }
            set
            {
                if (value)
                {
                    this.五零变量 |= 64;
                }
                else
                {
                    this.五零变量 &= -65;
                }
            }
        }

        public int 五零变量
        {
            get
            {
                return this.角色数据.角色变量[50];
            }
            set
            {
                this.角色数据.角色变量[50] = value;
                base.发送封包(new 同步补充变量
                {
                    变量类型 = 1,
                    对象编号 = this.地图编号,
                    变量索引 = 50,
                    变量内容 = value
                });
            }
        }

        public bool 开启觉醒面板
        {
            get
            {
                return (this.五零变量 & 0x20) == 32;
            }
            set
            {
                if (value)
                {
                    this.五零变量 |= 32;
                }
                else
                {
                    this.五零变量 &= -33;
                }
            }
        }

        public bool 开通龙卫觉醒
        {
            get
            {
                return this.角色数据.龙卫觉醒.V;
            }
            set
            {
                this.角色数据.龙卫觉醒.V = value;
                this.发送龙卫描述();
            }
        }

        public int 双倍经验
        {
            get
            {
                return this.角色数据.双倍经验.V;
            }
            set
            {
                if (this.角色数据.双倍经验.V != value)
                {
                    if (value > this.角色数据.双倍经验.V)
                    {
                        this.网络连接?.发送封包(new 双倍经验变动
                        {
                            双倍经验 = value
                        });
                    }
                    this.角色数据.双倍经验.V = value;
                }
            }
        }

        public long 所需经验 => 角色成长.升级所需经验[this.当前等级];

        public uint 银币数量
        {
            get
            {
                return this.角色数据.银币数量;
            }
            set
            {
                if (this.角色数据.银币数量 != value)
                {
                    this.角色数据.银币数量 = this.货币溢出修正(游戏货币.银币, this.角色数据.银币数量, value);
                    this.网络连接?.发送封包(new 货币数量变动
                    {
                        货币类型 = 0,
                        货币数量 = value
                    });
                }
            }
        }

        public uint 金币数量
        {
            get
            {
                return this.角色数据.金币数量;
            }
            set
            {
                if (this.角色数据.金币数量 != value)
                {
                    this.角色数据.金币数量 = this.货币溢出修正(游戏货币.金币, this.角色数据.金币数量, value);
                    this.网络连接?.发送封包(new 货币数量变动
                    {
                        货币类型 = 1,
                        货币数量 = value
                    });
                }
            }
        }

        public uint 元宝数量
        {
            get
            {
                return this.角色数据.元宝数量;
            }
            set
            {
                if (this.角色数据.元宝数量 != value)
                {
                    this.角色数据.元宝数量 = this.货币溢出修正(游戏货币.元宝, this.角色数据.元宝数量, value);
                    this.网络连接?.发送封包(new 同步元宝数量
                    {
                        元宝数量 = value
                    });
                }
            }
        }

        public uint 师门声望
        {
            get
            {
                return this.角色数据.师门声望;
            }
            set
            {
                if (this.角色数据.师门声望 != value)
                {
                    this.角色数据.师门声望 = value;
                    this.网络连接?.发送封包(new 货币数量变动
                    {
                        货币类型 = 6,
                        货币数量 = value
                    });
                }
            }
        }

        public int PK值惩罚
        {
            get
            {
                return this.角色数据.角色PK值;
            }
            set
            {
                value = Math.Max(0, value);
                if (this.角色数据.角色PK值 == 0 && value > 0)
                {
                    this.减PK时间 = TimeSpan.FromMinutes(1.0);
                }
                if (this.角色数据.角色PK值 != value)
                {
                    if (this.角色数据.角色PK值 < 300 && value >= 300)
                    {
                        this.灰名时间 = TimeSpan.Zero;
                    }
                    base.发送封包(new 同步对象惩罚
                    {
                        对象编号 = this.地图编号,
                        PK值惩罚 = (this.角色数据.角色PK值 = value)
                    });
                }
            }
        }

        public byte 行会等级
        {
            get
            {
                if (this.所属行会 != null)
                {
                    return this.所属行会.行会等级.V;
                }
                return 0;
            }
            set
            {
                if (this.所属行会 != null && this.所属行会.行会等级.V != value)
                {
                    this.所属行会.行会等级.V = value;
                    this.所属行会.发送封包(new 属性修改通知
                    {
                        属性类型 = 259,
                        属性数值 = this.所属行会.行会等级.V
                    });
                }
            }
        }

        public int 重生地图
        {
            get
            {
                if (this.红名玩家)
                {
                    return 147;
                }
                return this.角色数据.重生地图.V;
            }
            set
            {
                if (this.角色数据.重生地图.V != value)
                {
                    this.角色数据.重生地图.V = value;
                }
            }
        }

        public bool 红名玩家 => this.PK值惩罚 >= 300;

        public bool 灰名玩家 => this.灰名时间 > TimeSpan.Zero;

        public bool 绑定地图 => this.当前地图?[this.当前坐标].Contains(this) ?? false;

        public byte 背包大小
        {
            get
            {
                return this.角色数据.背包大小.V;
            }
            set
            {
                this.角色数据.背包大小.V = value;
            }
        }

        public byte 背包剩余 => (byte)(this.背包大小 - this.角色背包.Count);

        public byte 仓库大小
        {
            get
            {
                return this.角色数据.仓库大小.V;
            }
            set
            {
                this.角色数据.仓库大小.V = value;
            }
        }

        public byte 仓库剩余 => (byte)(this.仓库大小 - this.角色仓库.Count);

        public byte 资源包大小
        {
            get
            {
                return this.角色数据.资源包大小.V;
            }
            set
            {
                this.角色数据.资源包大小.V = value;
            }
        }

        public byte 资源包剩余 => (byte)(this.资源包大小 - this.角色资源包.Count);

        public byte 宠物上限 { get; set; }

        public byte 宠物数量 => (byte)this.宠物列表.Count;

        public byte 师门参数
        {
            get
            {
                if (this.所属师门 != null)
                {
                    if (this.所属师门.师父编号 == this.地图编号)
                    {
                        return 2;
                    }
                    return 1;
                }
                if (this.当前等级 < 30)
                {
                    return 0;
                }
                return 2;
            }
        }

        public byte 当前称号
        {
            get
            {
                return this.角色数据.当前称号.V;
            }
            set
            {
                if (this.角色数据.当前称号.V != value)
                {
                    this.角色数据.当前称号.V = value;
                }
            }
        }

        public byte 本期特权
        {
            get
            {
                return this.角色数据.本期特权.V;
            }
            set
            {
                if (this.角色数据.本期特权.V != value)
                {
                    this.角色数据.本期特权.V = value;
                }
            }
        }

        public byte 上期特权
        {
            get
            {
                return this.角色数据.上期特权.V;
            }
            set
            {
                if (this.角色数据.上期特权.V != value)
                {
                    this.角色数据.上期特权.V = value;
                }
            }
        }

        public byte 预定特权
        {
            get
            {
                return this.角色数据.预定特权.V;
            }
            set
            {
                if (this.角色数据.预定特权.V != value)
                {
                    this.角色数据.预定特权.V = value;
                }
            }
        }

        public uint 本期记录
        {
            get
            {
                return this.角色数据.本期记录.V;
            }
            set
            {
                if (this.角色数据.本期记录.V != value)
                {
                    this.角色数据.本期记录.V = value;
                }
            }
        }

        public uint 上期记录
        {
            get
            {
                return this.角色数据.上期记录.V;
            }
            set
            {
                if (this.角色数据.上期记录.V != value)
                {
                    this.角色数据.上期记录.V = value;
                }
            }
        }

        public DateTime 本期日期
        {
            get
            {
                return this.角色数据.本期日期.V;
            }
            set
            {
                if (this.角色数据.本期日期.V != value)
                {
                    this.角色数据.本期日期.V = value;
                }
            }
        }

        public DateTime 上期日期
        {
            get
            {
                return this.角色数据.上期日期.V;
            }
            set
            {
                if (this.角色数据.上期日期.V != value)
                {
                    this.角色数据.上期日期.V = value;
                }
            }
        }

        public TimeSpan 灰名时间
        {
            get
            {
                return this.角色数据.灰名时间.V;
            }
            set
            {
                if (this.角色数据.灰名时间.V <= TimeSpan.Zero && value > TimeSpan.Zero)
                {
                    base.发送封包(new 玩家名字变灰
                    {
                        对象编号 = this.地图编号,
                        是否灰名 = true
                    });
                }
                else if (this.角色数据.灰名时间.V > TimeSpan.Zero && value <= TimeSpan.Zero)
                {
                    base.发送封包(new 玩家名字变灰
                    {
                        对象编号 = this.地图编号,
                        是否灰名 = false
                    });
                }
                if (this.角色数据.灰名时间.V != value)
                {
                    this.角色数据.灰名时间.V = value;
                }
            }
        }

        public TimeSpan 减PK时间
        {
            get
            {
                return this.角色数据.减PK时间.V;
            }
            set
            {
                if (this.角色数据.减PK时间.V > TimeSpan.Zero && value <= TimeSpan.Zero)
                {
                    this.PK值惩罚--;
                    this.角色数据.减PK时间.V = TimeSpan.FromMinutes(1.0);
                }
                else if (this.角色数据.减PK时间.V != value)
                {
                    this.角色数据.减PK时间.V = value;
                }
            }
        }

        public 账号数据 所属账号 => this.角色数据.所属账号.V;

        public 行会数据 所属行会
        {
            get
            {
                return this.角色数据.所属行会.V;
            }
            set
            {
                if (this.角色数据.所属行会.V != value)
                {
                    if (this.开启七天乐)
                    {
                        this.修改七天进度(47, this.角色数据.七天进度[47] + 1);
                    }
                    this.角色数据.所属行会.V = value;
                }
            }
        }

        public 队伍数据 所属队伍
        {
            get
            {
                return this.角色数据.所属队伍.V;
            }
            set
            {
                if (this.角色数据.所属队伍.V != value)
                {
                    this.角色数据.所属队伍.V = value;
                }
            }
        }

        public 师门数据 所属师门
        {
            get
            {
                return this.角色数据.所属师门.V;
            }
            set
            {
                if (this.角色数据.所属师门.V != value)
                {
                    this.角色数据.所属师门.V = value;
                }
            }
        }

        public 攻击模式 攻击模式
        {
            get
            {
                return this.角色数据.攻击模式.V;
            }
            set
            {
                if (this.角色数据.攻击模式.V != value)
                {
                    this.角色数据.攻击模式.V = value;
                    this.网络连接?.发送封包(new 同步对战模式
                    {
                        对象编号 = this.地图编号,
                        攻击模式 = (byte)value
                    });
                }
            }
        }

        public 宠物模式 宠物模式
        {
            get
            {
                if (this.角色数据.宠物模式.V == 宠物模式.自动)
                {
                    this.角色数据.宠物模式.V = 宠物模式.攻击;
                    return 宠物模式.攻击;
                }
                return this.角色数据.宠物模式.V;
            }
            set
            {
                if (this.角色数据.宠物模式.V != value)
                {
                    this.角色数据.宠物模式.V = value;
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 9473,
                        第一参数 = (int)value
                    });
                }
            }
        }

        public 地图实例 复活地图
        {
            get
            {
                if (this.红名玩家)
                {
                    if (this.当前地图.地图编号 == 147)
                    {
                        return this.当前地图;
                    }
                    return 地图处理网关.已分配地图(147);
                }
                if (this.当前地图.地图编号 == this.重生地图)
                {
                    return this.当前地图;
                }
                return 地图处理网关.已分配地图(this.重生地图);
            }
        }

        public 对象发型分类 角色发型
        {
            get
            {
                return this.角色数据.角色发型.V;
            }
            set
            {
                this.角色数据.角色发型.V = value;
            }
        }

        public 对象发色分类 角色发色
        {
            get
            {
                return this.角色数据.角色发色.V;
            }
            set
            {
                this.角色数据.角色发色.V = value;
            }
        }

        public 对象脸型分类 角色脸型
        {
            get
            {
                return this.角色数据.角色脸型.V;
            }
            set
            {
                this.角色数据.角色脸型.V = value;
            }
        }

        public 游戏对象性别 角色性别 => this.角色数据.角色性别.V;

        public 游戏对象职业 角色职业 => this.角色数据.角色职业.V;

        public 对象名字颜色 对象颜色
        {
            get
            {
                if (this.角色数据.灰名时间.V > TimeSpan.Zero)
                {
                    return 对象名字颜色.灰名;
                }
                if (this.角色数据.角色PK值 >= 800)
                {
                    return 对象名字颜色.深红;
                }
                if (this.角色数据.角色PK值 >= 300)
                {
                    return 对象名字颜色.红名;
                }
                if (this.角色数据.角色PK值 >= 99)
                {
                    return 对象名字颜色.黄名;
                }
                return 对象名字颜色.白名;
            }
        }

        public 哈希监视器<宠物数据> 宠物数据 => this.角色数据.宠物数据;

        public 哈希监视器<邮件数据> 未读邮件 => this.角色数据.未读邮件;

        public 哈希监视器<邮件数据> 全部邮件 => this.角色数据.角色邮件;

        public 哈希监视器<角色数据> 好友列表 => this.角色数据.好友列表;

        public 哈希监视器<角色数据> 粉丝列表 => this.角色数据.粉丝列表;

        public 哈希监视器<角色数据> 偶像列表 => this.角色数据.偶像列表;

        public 哈希监视器<角色数据> 仇人列表 => this.角色数据.仇人列表;

        public 哈希监视器<角色数据> 仇恨列表 => this.角色数据.仇恨列表;

        public 哈希监视器<角色数据> 黑名单表 => this.角色数据.黑名单表;

        public 字典监视器<byte, int> 剩余特权 => this.角色数据.剩余特权;

        public 字典监视器<byte, 技能数据> 快捷栏位 => this.角色数据.快捷栏位;

        public 字典监视器<byte, 物品数据> 角色背包 => this.角色数据.角色背包;

        public 字典监视器<byte, 物品数据> 角色仓库 => this.角色数据.角色仓库;

        public 字典监视器<byte, 物品数据> 角色资源包 => this.角色数据.角色资源包;

        public 字典监视器<byte, 装备数据> 角色装备 => this.角色数据.角色装备;

        public 字典监视器<byte, DateTime> 称号列表 => this.角色数据.称号列表;

        public byte 当前坐骑
        {
            get
            {
                return this.角色数据.当前坐骑.V;
            }
            set
            {
                base.属性加成.Remove("当前使用坐骑");
                this.角色数据.当前坐骑.V = value;
                if (游戏坐骑.数据表.TryGetValue(value, out var value2))
                {
                    Dictionary<游戏对象属性, int> 基础属性;
                    基础属性 = value2.基础属性;
                    if (基础属性 != null && 基础属性.Any())
                    {
                        base.属性加成["当前使用坐骑"] = value2.基础属性;
                    }
                }
                this.更新对象属性();
            }
        }

        public 列表监视器<byte> 坐骑列表 => this.角色数据.坐骑列表;

        public 字典监视器<byte, int> 御兽列表 => this.角色数据.御兽列表;

        public 哈希监视器<龙卫数据> 龙卫属性 => this.角色数据.龙卫属性;

        public 哈希监视器<CharacterQuest> Quests => this.角色数据.Quests;

        public List<角色数据> 队友数据 => this.角色数据.所属队伍?.V.队伍成员.ToList();

        public int 物理击回 => this.角色职业 switch
        {
            游戏对象职业.战士 => (int)((float)(this[游戏对象属性.最大攻击] * this[游戏对象属性.物理击回]) * 0.00045f),
            游戏对象职业.法师 => (int)((float)(this[游戏对象属性.最大魔法] * this[游戏对象属性.物理击回]) * 0.00045f),
            游戏对象职业.刺客 => (int)((float)(this[游戏对象属性.最大刺术] * this[游戏对象属性.物理击回]) * 0.00045f),
            游戏对象职业.弓手 => (int)((float)(this[游戏对象属性.最大弓术] * this[游戏对象属性.物理击回]) * 0.00045f),
            游戏对象职业.道士 => (int)((float)(this[游戏对象属性.最大道术] * this[游戏对象属性.物理击回]) * 0.00045f),
            游戏对象职业.龙枪 => (int)((float)(this[游戏对象属性.最大攻击] * this[游戏对象属性.物理击回]) * 0.00045f),
            _ => 0,
        };

        public int 魔法击回 => this.角色职业 switch
        {
            游戏对象职业.战士 => (int)((float)(this[游戏对象属性.最大攻击] * this[游戏对象属性.魔法击回]) * 0.00045f),
            游戏对象职业.法师 => (int)((float)(this[游戏对象属性.最大魔法] * this[游戏对象属性.魔法击回]) * 0.00045f),
            游戏对象职业.刺客 => (int)((float)(this[游戏对象属性.最大刺术] * this[游戏对象属性.魔法击回]) * 0.00045f),
            游戏对象职业.弓手 => (int)((float)(this[游戏对象属性.最大弓术] * this[游戏对象属性.魔法击回]) * 0.00045f),
            游戏对象职业.道士 => (int)((float)(this[游戏对象属性.最大道术] * this[游戏对象属性.魔法击回]) * 0.00045f),
            游戏对象职业.龙枪 => (int)((float)(this[游戏对象属性.最大攻击] * this[游戏对象属性.魔法击回]) * 0.00045f),
            _ => 0,
        };

        public float 破物防 => (float)this[游戏对象属性.破物防] * 0.0008f;

        public float 破魔防 => (float)this[游戏对象属性.破魔防] * 0.0008f;

        public DateTime 武技日期
        {
            get
            {
                return this.角色数据.武技日期.V;
            }
            set
            {
                if (value != this.角色数据.武技日期.V)
                {
                    this.角色数据.武技日期.V = value;
                }
            }
        }

        public byte 传永武技
        {
            get
            {
                return this.角色数据.传永武技.V;
            }
            set
            {
                if (value != this.角色数据.传永武技.V)
                {
                    this.角色数据.传永武技.V = value;
                }
            }
        }

        public DateTime 签到日期
        {
            get
            {
                return this.角色数据.签到日期.V;
            }
            set
            {
                if (value != this.角色数据.签到日期.V)
                {
                    this.角色数据.签到日期.V = value;
                }
            }
        }

        public byte 每日签到
        {
            get
            {
                return this.角色数据.每日签到.V;
            }
            set
            {
                if (value != this.角色数据.每日签到.V)
                {
                    this.角色数据.每日签到.V = value;
                }
            }
        }

        public bool 开启七天乐 => (主程.当前时间.Date - this.角色数据.创建日期.V.Date).TotalDays < 7.0;

        public int 全部战功 => this.角色数据.购买战功.V + this.角色数据.战功进度.V + (int)((float)(int)this.角色数据.战功进度.V * 0.2f);

        public 开始自动战斗 自动挂机 { get; private set; }

        public int 挂机_地图 { get; private set; }

        public Rectangle 挂机_范围 { get; private set; }

        public Point 挂机_下一个坐标 { get; private set; }

        public Queue<Point> 挂机_寻路队列 { get; set; }

        public 挂机状态 挂机_状态 { get; set; }

        public 地图对象 挂机_目标 { get; set; }

        public List<int> 挂机_不拾取列表 { get; private set; }

        private uint 货币溢出修正(游戏货币 币, uint 当前, uint value)
        {
            if (value > Settings.货币异常上限)
            {
                StackTrace stackTrace;
                stackTrace = new StackTrace();
                string text;
                text = "货币溢出 ";
                for (int num = Math.Min(stackTrace.FrameCount, 5) - 1; num >= 1; num--)
                {
                    StackFrame frame;
                    frame = stackTrace.GetFrame(num);
                    text += $"=> {frame.GetFileLineNumber()} {frame.GetMethod().Name}";
                }
                主程.添加货币日志(this.角色数据, text, 币, value);
                value = 当前;
            }
            return value;
        }

        public bool 修改货币(string op, 游戏货币 币, uint value)
        {
            switch (币)
            {
                case 游戏货币.银币:
                    {
                        uint num;
                        num = NPCSegment.Calculate(op, this.银币数量, value);
                        if (this.银币数量 != num)
                        {
                            this.银币数量 = num;
                            this.网络连接?.发送封包(new 货币数量变动
                            {
                                货币类型 = 0,
                                货币数量 = this.银币数量
                            });
                        }
                        break;
                    }
                case 游戏货币.金币:
                    {
                        uint num;
                        num = NPCSegment.Calculate(op, this.金币数量, value);
                        if (this.金币数量 != num)
                        {
                            this.金币数量 = num;
                            this.网络连接?.发送封包(new 货币数量变动
                            {
                                货币类型 = 1,
                                货币数量 = this.金币数量
                            });
                        }
                        break;
                    }
                case 游戏货币.元宝:
                    {
                        uint num;
                        num = NPCSegment.Calculate(op, this.元宝数量, value);
                        if (this.元宝数量 != num)
                        {
                            this.元宝数量 = num;
                            this.网络连接?.发送封包(new 同步元宝数量
                            {
                                元宝数量 = this.元宝数量
                            });
                        }
                        break;
                    }
            }
            return true;
        }

        public bool 扣金币(uint value)
        {
            return this.修改货币("-", 游戏货币.金币, value);
        }

        public void OpenChest(int objectId)
        {
            if (地图处理网关.道具对象表.TryGetValue(objectId, out var value) && base.邻居列表.Contains(value))
            {
                value.Open(this);
            }
        }

        public 玩家实例(角色数据 角色数据, 客户网络 网络连接, bool 自动战斗 = false)
        {
            this.角色数据 = 角色数据;
            this.isNewHuman = 角色数据.当前等级.V == 0;
            this.宠物列表 = new List<宠物实例>();
            this.定时器列表 = new List<定时器数据>();
            this.被动技能 = new Dictionary<ushort, 技能数据>();
            base.属性加成[this] = 角色成长.获取数据(this.角色职业, this.当前等级);
            this.战力加成 = new Dictionary<object, int> { [this] = 角色成长.等级战力(this.当前等级) };
            this.称号时间 = DateTime.MaxValue;
            this.拾取时间 = 主程.当前时间.AddSeconds(1.0);
            base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
            this.特权时间 = ((this.本期特权 > 0) ? this.本期日期.AddDays(30.0) : DateTime.MaxValue);
            this.临时精炼 = new ushort[3];
            this.生效龙卫 = new Dictionary<ushort, int>();
            this.物品过滤 = new HashSet<int>();
            this.物品极品提示 = new HashSet<int>();
            this.自动拾取范围 = 0;
            this.自动拾取间隔 = 1000;
            foreach (装备数据 value6 in this.角色装备.Values)
            {
                this.战力加成[value6] = value6.装备战力;
                if (value6.当前持久.V > 0)
                {
                    base.属性加成[value6] = value6.装备属性;
                }
                if (value6.第一铭文 != null && this.主体技能表.TryGetValue(value6.第一铭文.技能编号, out var v))
                {
                    v.铭文编号 = value6.第一铭文.铭文编号;
                }
                if (value6.第二铭文 != null && this.主体技能表.TryGetValue(value6.第二铭文.技能编号, out var v2))
                {
                    v2.铭文编号 = value6.第二铭文.铭文编号;
                }
            }
            foreach (技能数据 value7 in this.主体技能表.Values)
            {
                this.战力加成[value7] = value7.战力加成;
                base.属性加成[value7] = value7.属性加成;
                foreach (ushort item in value7.被动技能.ToList())
                {
                    this.被动技能.Add(item, value7);
                }
            }
            foreach (Buff数据 value8 in this.Buff列表.Values)
            {
                if ((value8.Buff效果 & Buff效果类型.属性增减) != 0)
                {
                    base.属性加成.Add(value8, value8.属性加成);
                }
            }
            foreach (KeyValuePair<byte, int> item2 in 角色数据.天赋刻印)
            {
                if (游戏天赋.数据表.TryGetValue(item2.Key, out var value) && 角色数据.天赋等级.TryGetValue(item2.Key, out var v3) && v3 >= 10 && value.刻印列表[(byte)this.角色职业].TryGetValue((byte)item2.Value, out var value2))
                {
                    base.移除Buff时处理(value2.刻印BUFF);
                    base.添加Buff时处理(value2.刻印BUFF, this);
                    this.战力加成[value2] = value2.刻印战力;
                }
            }
            foreach (KeyValuePair<byte, DateTime> item3 in this.称号列表.ToList())
            {
                if (主程.当前时间 >= item3.Value)
                {
                    if (this.称号列表.Remove(item3.Key) && this.当前称号 == item3.Key)
                    {
                        this.当前称号 = 0;
                    }
                }
                else if (item3.Value < this.称号时间)
                {
                    this.称号时间 = item3.Value;
                }
                if (item3.Key != this.当前称号 && 游戏称号.数据表.TryGetValue(item3.Key, out var value3) && value3.始终生效)
                {
                    this.战力加成[item3.Key] = value3.称号战力;
                    base.属性加成[item3.Key] = value3.称号属性;
                }
            }
            if (this.当前称号 > 0 && 游戏称号.数据表.TryGetValue(this.当前称号, out var value4))
            {
                this.战力加成[this.当前称号] = value4.称号战力;
                base.属性加成[this.当前称号] = value4.称号属性;
            }
            if (this.当前体力 == 0)
            {
                this.当前地图 = 地图处理网关.已分配地图(this.重生地图);
                this.当前坐标 = (this.红名玩家 ? this.当前地图.红名区域.随机坐标 : this.当前地图.复活区域.随机坐标);
                this.当前体力 = (int)((float)this[游戏对象属性.最大体力] * 0.3f);
                this.当前魔力 = (int)((float)this[游戏对象属性.最大魔力] * 0.3f);
            }
            else if (游戏地图.数据表[(byte)角色数据.当前地图.V].下线传送)
            {
                if (角色数据.当前地图.V == 152 && 地图处理网关.沙城地图 != null)
                {
                    this.当前地图 = 地图处理网关.沙城地图;
                    if (this.所属行会 != null && this.所属行会 == 系统数据.数据.占领行会.V)
                    {
                        this.当前坐标 = 地图处理网关.守方传送区域.随机坐标;
                    }
                    else
                    {
                        this.当前坐标 = 地图处理网关.外城复活区域.随机坐标;
                    }
                }
                else if (游戏地图.数据表[(byte)角色数据.当前地图.V].传送地图 == 0)
                {
                    this.当前地图 = 地图处理网关.已分配地图(this.重生地图);
                    this.当前坐标 = this.当前地图.复活区域.随机坐标;
                }
                else
                {
                    this.当前地图 = 地图处理网关.已分配地图(游戏地图.数据表[(byte)角色数据.当前地图.V].传送地图);
                    this.当前坐标 = this.当前地图.传送区域?.随机坐标 ?? this.当前地图.地图区域.First().随机坐标;
                }
            }
            else
            {
                this.当前地图 = 地图处理网关.已分配地图(角色数据.当前地图.V);
            }
            this.刷新天赋属性();
            this.刷新传奇之力();
            this.刷新龙卫激活状态(是否更新属性: false, forceUpdate: true);
            this.更新精炼阶段属性();
            this.对象死亡 = false;
            this.阻塞网格 = true;
            地图处理网关.添加地图对象(this);
            base.激活对象 = true;
            地图处理网关.添加激活对象(this);
            角色数据.登录日期.V = 主程.当前时间;
            角色数据.角色上线(网络连接);
            网络连接?.发送封包(new 同步补充变量
            {
                变量类型 = 1,
                对象编号 = this.地图编号,
                变量索引 = 939,
                变量内容 = 36
            });
            网络连接?.发送封包(new 同步补充变量
            {
                变量类型 = 1,
                对象编号 = this.地图编号,
                变量索引 = 793,
                变量内容 = 2
            });
            网络连接?.发送封包(new 同步补充变量
            {
                变量类型 = 1,
                对象编号 = this.地图编号,
                变量索引 = 112,
                变量内容 = (计算类.日期同周(this.角色数据.补给日期.V, 主程.当前时间) ? 计算类.时间转换(角色数据.补给日期.V) : 0)
            });
            网络连接?.发送封包(new 同步补充变量
            {
                变量类型 = 1,
                对象编号 = this.地图编号,
                变量索引 = 975,
                变量内容 = (计算类.日期同周(this.角色数据.战备日期.V, 主程.当前时间) ? 计算类.时间转换(角色数据.战备日期.V) : 0)
            });
            this.CallDefaultNPC(DefaultNPCType.Login, delay: false, null);
            if (this.当前等级 == 0)
            {
                this.当前等级 = 1;
            }
            网络连接?.发送封包(new 同步个人信息
            {
                Data = this.GetPlayerAgreement()
            });
            网络连接?.发送封包(new 同步角色数据
            {
                对象编号 = this.地图编号,
                对象坐标 = this.当前坐标,
                对象高度 = this.当前高度,
                当前经验 = this.当前经验,
                双倍经验 = this.双倍经验,
                所需经验 = this.所需经验,
                PK值惩罚 = this.PK值惩罚,
                对象朝向 = (ushort)this.当前方向,
                当前地图 = this.当前地图.地图编号,
                对象职业 = (byte)this.角色职业,
                对象性别 = (byte)this.角色性别,
                对象等级 = this.当前等级,
                攻击模式 = (byte)this.攻击模式,
                当前时间 = 计算类.时间转换(主程.当前时间),
                开放等级 = Settings.游戏开放等级,
                特修折扣 = (ushort)(Settings.装备特修折扣 * 10000m),
                觉醒之力经验 = (int)this.当前觉醒经验,
                觉醒经验上限 = (int)this.觉醒存储上限
            });
            网络连接?.发送封包(new 同步师门信息
            {
                师门参数 = this.师门参数
            });
            网络连接?.发送封包(new 同步技能信息
            {
                技能描述 = this.全部技能描述()
            });
            网络连接?.发送封包(new 同步货币数量
            {
                字节描述 = this.全部货币描述()
            });
            网络连接?.发送封包(new 同步背包大小
            {
                背包大小 = this.背包大小,
                仓库大小 = this.仓库大小,
                资源包大小 = this.资源包大小
            });
            网络连接?.发送封包(new 同步背包信息
            {
                未知标志 = 0,
                物品描述 = this.全部物品描述()
            });
            网络连接?.发送封包(new 同步仓库锁定
            {
                锁定状态 = 角色数据.锁定仓库.V
            });
            this.发送龙卫描述();
            网络连接?.发送封包(new 同步任务列表
            {
                任务描述 = this.GetQuestProgressData()
            });
            if (Settings.开启成就系统)
            {
                网络连接?.发送封包(new 同步成就列表
                {
                    成就描述 = this.GetAchievementProgress()
                });
            }
            网络连接?.发送封包(new 同步传奇之力
            {
                传奇之力 = 角色数据.传奇之力等级,
                对象编号 = 角色数据.角色编号
            });
            if (Settings.开启任务系统)
            {
                this.发送悬赏任务数据();
            }
            网络连接?.发送封包(new 同步威望列表
            {
                字节数据 = this.获取威望描述()
            });
            网络连接?.发送封包(new 同步技能栏位
            {
                栏位描述 = this.快捷栏位描述()
            });
            base.发送封包(new 同步角色变量
            {
                字节描述 = this.获取角色变量()
            });
            网络连接?.发送封包(new 同步称号信息
            {
                字节描述 = this.全部称号描述()
            });
            base.发送封包(new 同步狩猎信息
            {
                未知标志 = 1,
                未知参数 = 39015,
                狩猎编号 = 角色数据.已接狩猎.V,
                剩余秒数 = (int)((角色数据.已接狩猎.V > 0) ? Math.Max(0.0, (角色数据.狩猎完成.V - 主程.当前时间).TotalSeconds) : 0.0)
            });
            网络连接?.发送封包(new 同步签到信息
            {
                签到次数 = this.每日签到,
                能否签到 = ((主程.当前时间.Date.AddDays(1.0) - this.签到日期.Date).TotalDays > 0.0),
                开启签到 = true
            });
            网络连接?.发送封包(new 传永武技签到
            {
                签到次数 = this.传永武技,
                能否签到 = ((this.本期特权 == 4 || this.本期特权 == 5 || this.本期特权 == 7) && (主程.当前时间.Date.AddDays(1.0) - this.武技日期.Date).TotalDays > 0.0)
            });
            网络连接?.发送封包(new 同步角色属性
            {
                属性描述 = this.玩家属性描述()
            });
            网络连接?.发送封包(new 同步客户变量
            {
                字节数据 = 角色数据.角色设置()
            });
            网络连接?.发送封包(new 同步特权信息
            {
                字节数组 = this.玛法特权描述()
            });
            网络连接?.发送封包(new 树立城主雕像
            {
                字节描述 = this.获取城主雕像描述()
            });
            this.发送坐骑描述();
            if (Settings.开启成就系统)
            {
                网络连接?.发送封包(new 同步击杀任务
                {
                    字节数据 = this.获取击杀任务()
                });
            }
            this.发送天赋描述();
            this.发送七天乐详情();
            this.计算传承之力();
            网络连接?.发送封包(new 同步数据结束
            {
                角色编号 = this.地图编号
            });
            网络连接?.发送封包(new 玩家名字变灰
            {
                对象编号 = this.地图编号,
                是否灰名 = this.灰名玩家
            });
            foreach (角色数据 item4 in this.粉丝列表)
            {
                item4.网络连接?.发送封包(new 好友上线下线
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字,
                    对象职业 = (byte)this.角色职业,
                    对象性别 = (byte)this.角色性别,
                    上线下线 = 0
                });
            }
            foreach (角色数据 item5 in this.仇恨列表)
            {
                item5.网络连接?.发送封包(new 好友上线下线
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字,
                    对象职业 = (byte)this.角色职业,
                    对象性别 = (byte)this.角色性别,
                    上线下线 = 0
                });
            }
            if (this.偶像列表.Count != 0 || this.仇人列表.Count != 0)
            {
                网络连接?.发送封包(new 同步好友列表
                {
                    字节描述 = this.社交列表描述()
                });
            }
            if (this.黑名单表.Count != 0)
            {
                网络连接?.发送封包(new 同步黑名单表
                {
                    字节描述 = this.社交屏蔽描述()
                });
            }
            网络连接?.发送封包(new 未读邮件提醒
            {
                邮件数量 = this.未读邮件.Count
            });
            if (this.所属队伍 != null)
            {
                网络连接?.发送封包(new 玩家加入队伍
                {
                    字节描述 = this.所属队伍.队伍描述()
                });
            }
            if (this.所属行会 != null)
            {
                网络连接?.发送封包(new 行会信息公告
                {
                    字节数据 = this.所属行会.行会信息描述()
                });
                this.所属行会.发送封包(new 同步会员信息
                {
                    对象编号 = this.地图编号,
                    对象信息 = this.当前地图.地图编号,
                    当前等级 = this.当前等级
                });
                if (this.所属行会.行会成员[this.角色数据] <= 行会职位.理事 && this.所属行会.申请列表.Count > 0)
                {
                    网络连接?.发送封包(new 发送行会通知
                    {
                        提醒类型 = 1
                    });
                }
                if (this.所属行会.行会成员[this.角色数据] <= 行会职位.副长 && this.所属行会.结盟申请.Count > 0)
                {
                    网络连接?.发送封包(new 发送行会通知
                    {
                        提醒类型 = 2
                    });
                }
                if (this.所属行会.行会成员[this.角色数据] <= 行会职位.副长 && this.所属行会.解除申请.Count > 0)
                {
                    网络连接?.发送封包(new 行会外交公告
                    {
                        字节数据 = this.所属行会.解除申请描述()
                    });
                }
            }
            if (系统数据.数据.占领行会.V != null)
            {
                网络连接?.发送封包(new 同步占领行会
                {
                    之前行会 = 0,
                    现在行会 = 系统数据.数据.占领行会.V.行会编号
                });
            }
            if (this.所属行会 != null && this.所属行会 == 系统数据.数据.占领行会.V && this.所属行会.行会成员[角色数据] == 行会职位.会长)
            {
                网络服务网关.发送公告("沙巴克城主 [" + this.对象名字 + "] 进入了游戏");
            }
            this.所属队伍?.发送封包(new 同步队员状态
            {
                对象编号 = this.地图编号
            }, 角色数据);
            if (游戏坐骑.数据表.TryGetValue(this.当前坐骑, out var value5))
            {
                Dictionary<游戏对象属性, int> 基础属性;
                基础属性 = value5.基础属性;
                if (基础属性 != null && 基础属性.Any())
                {
                    base.属性加成["当前使用坐骑"] = value5.基础属性;
                }
            }
            this.刷新游戏套装();
            this.更新玩家战力();
            this.更新对象属性();
            this.特权玩家登录(this.本期特权);
            if (!Settings.屏蔽日程 && 角色数据.找回奖励.Count == 0)
            {
                foreach (int value9 in Enum.GetValues(typeof(日程找回)))
                {
                    角色数据.找回奖励.Add((ushort)value9, 常量类.找回奖励字典[(ushort)value9]);
                }
            }
            if (this.当前地图 != null && this.当前地图.副本地图 && !this.当前地图.副本关闭)
            {
                地图实例 地图实例2;
                地图实例2 = this.查找我的副本(this.当前地图.地图编号);
                if (地图实例2 != null)
                {
                    this.当前地图 = 地图实例2;
                }
            }
            if (网络连接 != null)
            {
                this.会话ID = Guid.NewGuid().ToString("N");
                //主程.WebLog(LogDataType.LoginLog, Settings.统计UUID代码, 角色数据.所属账号.V.账号名字.V, this.会话ID);
            }
            else
            {
                this.是否假人 = true;
            }
        }

        public 物品数据 根据编号获取背包物品(int itemId)
        {
            foreach (KeyValuePair<byte, 物品数据> item in this.角色背包)
            {
                if (item.Value.物品编号 == itemId)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public void CompleteQuest(int questId, int selectId)
        {
            CharacterQuest characterQuest;
            characterQuest = this.角色数据.Quests.FirstOrDefault((CharacterQuest x) => x.Info.V.Id == questId);
            if (characterQuest == null || (characterQuest.CompleteDate.V != DateTime.MinValue && characterQuest.CompleteCount.V >= characterQuest.Info.V.MaxCompleteCount) || !characterQuest.Missions.All((CharacterQuestMission x) => x.CompletedDate.V != DateTime.MinValue || x.Info.V.Type == QuestMissionType.RecycleItem || (x.Info.V.Type == QuestMissionType.AdquireItem && x.CompletedDate.V == DateTime.MinValue && this.查找背包物品(x.Info.V.Id, x.Info.V.Count) != null)))
            {
                return;
            }
            byte b;
            b = (byte)((uint)characterQuest.Info.V.Rewards.Sum((GameQuestReward x) => (x.Type == QuestRewardType.Item) ? 1 : 0) + ((characterQuest.Info.V.SelectableRewards.Find((GameQuestReward x) => x.Type == QuestRewardType.Item) != null) ? 1u : 0u));
            byte[] location;
            location = Array.Empty<byte>();
            if (b > 0 && !this.角色数据.尝试获取背包空余格子(b, out location))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6459
                });
                return;
            }
            CharacterQuestMission[] missionsOfType;
            missionsOfType = characterQuest.GetMissionsOfType(QuestMissionType.AdquireItem);
            int num;
            num = 0;
            while (true)
            {
                if (num < missionsOfType.Length)
                {
                    CharacterQuestMission characterQuestMission;
                    characterQuestMission = missionsOfType[num];
                    List<物品数据> list;
                    list = this.查找背包物品(characterQuestMission.Info.V.Id, characterQuestMission.Info.V.Count);
                    if (list != null)
                    {
                        this.消耗背包物品(characterQuestMission.Info.V.Count, list);
                        characterQuestMission.Count.V = (byte)(characterQuestMission.Count.V + 1);
                        num++;
                        continue;
                    }
                    break;
                }
                byte b2;
                b2 = 0;
                if (selectId > -1 && selectId < characterQuest.Info.V.SelectableRewards.Count)
                {
                    GameQuestReward gameQuestReward;
                    gameQuestReward = characterQuest.Info.V.SelectableRewards.ToArray()[selectId];
                    switch (gameQuestReward.Type)
                    {
                        case QuestRewardType.Currency:
                            this.玩家获得货币((游戏货币)gameQuestReward.Id, gameQuestReward.Count);
                            break;
                        case QuestRewardType.Item:
                            {
                                if (游戏物品.数据表.TryGetValue(gameQuestReward.Id, out var value))
                                {
                                    this.玩家获得物品(value, location[b2++], "任务获得物品", (gameQuestReward.Count <= 0) ? 1 : gameQuestReward.Count, 是否绑定: true);
                                }
                                break;
                            }
                        case QuestRewardType.Exp:
                            this.玩家增加经验(null, gameQuestReward.Count);
                            break;
                        case QuestRewardType.Reputation:
                            if (!Settings.屏蔽威望)
                            {
                                this.更改玩家威望((byte)gameQuestReward.Id, this.获取玩家威望((byte)gameQuestReward.Id) + gameQuestReward.Count);
                            }
                            break;
                        case QuestRewardType.Activity:
                            if (!Settings.屏蔽日程)
                            {
                                this.角色数据.日程进度.V += (ushort)gameQuestReward.Count;
                                this.发送日程详情();
                            }
                            break;
                    }
                }
                foreach (GameQuestReward reward in characterQuest.Info.V.Rewards)
                {
                    switch (reward.Type)
                    {
                        case QuestRewardType.Currency:
                            this.角色数据.角色货币[(游戏货币)reward.Id] += (uint)reward.Count;
                            this.网络连接?.发送封包(new 货币数量变动
                            {
                                货币类型 = (byte)reward.Id,
                                货币数量 = this.角色数据.角色货币[(游戏货币)reward.Id]
                            });
                            break;
                        case QuestRewardType.Item:
                            {
                                if (游戏物品.数据表.TryGetValue(reward.Id, out var value2))
                                {
                                    this.玩家获得物品(value2, location[b2++], "任务获得物品", (reward.Count <= 0) ? 1 : reward.Count, 是否绑定: true);
                                }
                                break;
                            }
                        case QuestRewardType.Exp:
                            this.玩家增加经验(null, reward.Count);
                            break;
                        case QuestRewardType.Reputation:
                            if (!Settings.屏蔽威望)
                            {
                                this.更改玩家威望((byte)reward.Id, this.获取玩家威望((byte)reward.Id) + reward.Count);
                            }
                            break;
                        case QuestRewardType.Activity:
                            if (!Settings.屏蔽日程)
                            {
                                this.角色数据.日程进度.V += (ushort)reward.Count;
                                this.发送日程详情();
                            }
                            break;
                    }
                }
                if (this.开启七天乐 && characterQuest.Info.V.Name.Contains("猎魔-"))
                {
                    this.修改七天进度(45, this.角色数据.七天进度[45] + 1);
                    this.修改七天进度(50, this.角色数据.七天进度[50] + 1);
                    this.修改七天进度(55, this.角色数据.七天进度[55] + 1);
                    this.修改七天进度(70, this.角色数据.七天进度[70] + 1);
                }
                if (this.开启七天乐 && characterQuest.Info.V.Name.Contains("史诗"))
                {
                    this.修改七天进度(60, this.角色数据.七天进度[60] + 1);
                }
                if (characterQuest.Info.V.Type == QuestType.紧急任务)
                {
                    this.角色数据.紧急任务.V++;
                    if (!Settings.屏蔽日程)
                    {
                        this.角色数据.日程进度.V += 4;
                        this.发送日程详情();
                    }
                    if (this.开启七天乐)
                    {
                        this.修改七天进度(40, this.角色数据.七天进度[40] + 1);
                        this.修改七天进度(65, this.角色数据.七天进度[65] + 1);
                    }
                    if (Settings.开启成就系统)
                    {
                        this.成就变量变更(AchievementVariables.CompleteEmegencyQuestCount, 1);
                    }
                }
                else if (characterQuest.Info.V.Type == QuestType.悬赏任务)
                {
                    if (characterQuest.Info.V.Reset == QuestResetType.Weekly)
                    {
                        this.角色数据.周常悬赏完成次数.V--;
                        this.修改战功任务(10, 1);
                        if (Settings.开启成就系统)
                        {
                            this.成就变量变更(AchievementVariables.RewardWeeklyQuestCompleteCount, 1);
                        }
                    }
                    else
                    {
                        this.角色数据.日常悬赏完成次数.V--;
                        this.修改战功任务(9, 1);
                        if (!Settings.屏蔽日程)
                        {
                            this.角色数据.日程进度.V += 5;
                            this.发送日程详情();
                        }
                        if (Settings.开启成就系统)
                        {
                            this.成就变量变更(AchievementVariables.RewardQuestCompleteCount, 1);
                        }
                    }
                    this.发送悬赏剩余计次();
                }
                else if (Settings.开启成就系统 && characterQuest.Info.V.Type == QuestType.Main)
                {
                    this.成就变量变更(AchievementVariables.CompleteStoryQuestCount, 1);
                }
                characterQuest.CompleteDate.V = 主程.当前时间;
                characterQuest.CompleteCount.V++;
                this.网络连接?.发送封包(new 完成任务回执
                {
                    任务编号 = questId
                });
                this.完成任务激活(questId);
                if (characterQuest.Info.V.AutoStartNextID > 0)
                {
                    this.StartQuest(characterQuest.Info.V.AutoStartNextID, autoStart: true);
                }
                break;
            }
        }

        #region 挖矿
        public void 玩家开始挖矿(Point 坐标)
        {
            if (当前地图.地图编号 == 144 || 当前地图.地图编号 == 153 || 当前地图.地图编号 == 154)
            {
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
        public void 完成任务激活(int 任务编号)
        {
            switch (任务编号)
            {
                case 1456:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 135,
                        变量内容 = 1
                    });
                    break;
                case 1454:
                    base.添加Buff时处理(2533, this);
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 134,
                        变量内容 = 1
                    });
                    this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, 1252, 636);
                    break;
                case 1447:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 133,
                        变量内容 = 1
                    });
                    this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, 1002, 509);
                    break;
                case 1482:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 124,
                        变量内容 = 1
                    });
                    break;
                case 1481:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 123,
                        变量内容 = 1
                    });
                    this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, 1107, 700);
                    break;
                case 1462:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 122,
                        变量内容 = 1
                    });
                    break;
                case 1522:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 125,
                        变量内容 = 1
                    });
                    break;
                case 1511:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 140,
                        变量内容 = 1
                    });
                    break;
                case 1510:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 161,
                        变量内容 = 1
                    });
                    break;
                case 1560:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 162,
                        变量内容 = 1
                    });
                    break;
                case 1527:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 127,
                        变量内容 = 1
                    });
                    break;
                case 1524:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 126,
                        变量内容 = 1
                    });
                    break;
                case 1645:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 164,
                        变量内容 = 1
                    });
                    break;
                case 1602:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 166,
                        变量内容 = 1
                    });
                    break;
                case 1561:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 163,
                        变量内容 = 1
                    });
                    break;
                case 1656:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 169,
                        变量内容 = 1
                    });
                    break;
                case 1654:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 173,
                        变量内容 = 1
                    });
                    break;
                case 1648:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 165,
                        变量内容 = 1
                    });
                    break;
                case 1723:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 175,
                        变量内容 = 1
                    });
                    break;
                case 1720:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 174,
                        变量内容 = 1
                    });
                    break;
                case 1680:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 170,
                        变量内容 = 1
                    });
                    break;
                case 2002:
                    this.开通龙卫觉醒 = true;
                    break;
                case 1726:
                    base.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 176,
                        变量内容 = 1
                    });
                    break;
                case 2041:
                case 2042:
                case 2043:
                case 2044:
                case 2045:
                case 2046:
                    {
                        byte key;
                        key = (byte)(30 + (任务编号 - 2041) * 5);
                        if (游戏天赋.数据表.TryGetValue(key, out var _))
                        {
                            if (this.角色数据.天赋等级[key] == 0)
                            {
                                this.角色数据.天赋等级[key] = 1;
                                this.刷新天赋属性();
                                this.更新对象属性();
                                this.更新玩家战力();
                            }
                            this.发送天赋描述();
                        }
                        break;
                    }
                case 2022:
                    this.开启觉醒面板 = true;
                    break;
            }
        }

        public 物品数据 玩家获得物品(int 物品编号, int 物品持久 = 1, string 存储日志 = "", bool 是否绑定 = false, string 掉落怪物 = "")
        {
            if (游戏物品.数据表.TryGetValue(物品编号, out var value))
            {
                if (value.持久类型 == 物品持久分类.堆叠)
                {
                    foreach (KeyValuePair<byte, 物品数据> item in this.角色背包)
                    {
                        物品数据 value2;
                        value2 = item.Value;
                        if (value2.物品编号 == 物品编号 && value2.能否堆叠 && value2.当前持久.V + 物品持久 < value2.最大持久.V && value2.是否绑定 == 是否绑定)
                        {
                            value2.当前持久.V += 物品持久;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = value2.字节描述()
                            });
                            return value2;
                        }
                    }
                }
                if (this.角色数据.尝试获取背包空余格子(out var location))
                {
                    if (value is 游戏装备 模板)
                    {
                        this.角色数据.角色背包[location] = new 装备数据(模板, this.角色数据, 1, location, 随机生成: true, 是否绑定, 掉落怪物);
                    }
                    else if (value.持久类型 == 物品持久分类.容器)
                    {
                        this.角色数据.角色背包[location] = new 物品数据(value, this.角色数据, 1, location, 0, 是否绑定, 掉落怪物);
                    }
                    else if (value.持久类型 == 物品持久分类.堆叠)
                    {
                        this.角色数据.角色背包[location] = new 物品数据(value, this.角色数据, 1, location, 1, 是否绑定, 掉落怪物);
                    }
                    else
                    {
                        this.角色数据.角色背包[location] = new 物品数据(value, this.角色数据, 1, location, value.物品持久, 是否绑定, 掉落怪物);
                    }
                    if (物品持久 > 1)
                    {
                        this.角色数据.角色背包[location].当前持久.V = 物品持久;
                    }
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色数据.角色背包[location].字节描述()
                    });
                    if (存储日志 != string.Empty)
                    {
                        主程.添加物品日志(this, 存储日志, this.角色数据.角色背包[location], 1, "掉落怪物:" + 掉落怪物);
                    }
                    return this.角色数据.角色背包[location];
                }
                this.角色数据.发送邮件(null, "背包空间已满", "您获得的[" + value.物品名字 + "]由于包裹已满，系统以邮件的形式发放，请及时查收！", value.物品编号, 物品持久);
                return null;
            }
            return null;
        }

        public void 玩家获得物品(游戏物品 物品模板, byte 物品位置, string 日志, int 物品持久 = 1, bool 是否绑定 = false, string 掉落怪物 = "")
        {
            if (物品模板.持久类型 == 物品持久分类.堆叠)
            {
                foreach (KeyValuePair<byte, 物品数据> item in this.角色背包)
                {
                    物品数据 value;
                    value = item.Value;
                    if (value.物品编号 == 物品模板.物品编号 && value.能否堆叠 && value.当前持久.V + 物品持久 < value.最大持久.V && value.是否绑定 == 是否绑定)
                    {
                        value.当前持久.V += 物品持久;
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = value.字节描述()
                        });
                        return;
                    }
                }
            }
            if (物品模板 is 游戏装备 模板)
            {
                this.角色数据.角色背包[物品位置] = new 装备数据(模板, this.角色数据, 1, 物品位置, 随机生成: false, 是否绑定, 掉落怪物);
            }
            else if (物品模板.持久类型 == 物品持久分类.容器)
            {
                this.角色数据.角色背包[物品位置] = new 物品数据(物品模板, this.角色数据, 1, 物品位置, 0, 是否绑定, 掉落怪物);
            }
            else if (物品模板.持久类型 == 物品持久分类.堆叠)
            {
                this.角色数据.角色背包[物品位置] = new 物品数据(物品模板, this.角色数据, 1, 物品位置, 1, 是否绑定, 掉落怪物);
            }
            else
            {
                this.角色数据.角色背包[物品位置] = new 物品数据(物品模板, this.角色数据, 1, 物品位置, 物品模板.物品持久, 是否绑定, 掉落怪物);
            }
            if (物品持久 > 1)
            {
                this.角色数据.角色背包[物品位置].当前持久.V = 物品持久;
            }
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = this.角色数据.角色背包[物品位置].字节描述()
            });
            主程.添加物品日志(this, 日志, this.角色数据.角色背包[物品位置], 1, "掉落怪物:" + 掉落怪物);
        }

        public bool 是否已完成任务(int 任务编号)
        {
            if (!GameQuests.数据表.TryGetValue(任务编号, out var value))
            {
                return false;
            }
            CharacterQuest[] array;
            array = this.角色数据.Quests.Where((CharacterQuest x) => x.Info.V.Id == 任务编号 && x.CompleteDate.V != DateTime.MinValue).ToArray();
            if (value.MaxCompleteCount > 0 && array.Length >= value.MaxCompleteCount)
            {
                return true;
            }
            return false;
        }

        public bool 正在进行任务(int 任务编号)
        {
            if (!GameQuests.数据表.TryGetValue(任务编号, out var value))
            {
                return false;
            }
            CharacterQuest[] array;
            array = this.角色数据.Quests.Where((CharacterQuest x) => x.Info.V.Id == 任务编号 && x.CompleteDate.V == DateTime.MinValue).ToArray();
            if (value.MaxCompleteCount > 0 && array.Length >= value.MaxCompleteCount)
            {
                return true;
            }
            return false;
        }

        public bool CanAcceptQuest(int questId, bool autoStart, out CharacterQuest 角色任务)
        {
            角色任务 = null;
            if (!GameQuests.数据表.TryGetValue(questId, out var value))
            {
                return false;
            }
            if (!autoStart && value.StartNPCID > 0 && this.对话守卫?.模板编号 != value.StartNPCID)
            {
                return false;
            }
            CharacterQuest characterQuest;
            characterQuest = this.角色数据.Quests.FirstOrDefault((CharacterQuest x) => x.Info.V.Id == questId);
            if (characterQuest != null)
            {
                角色任务 = characterQuest;
                if (characterQuest.CompleteDate.V != DateTime.MinValue && (characterQuest.Info.V.Type == QuestType.紧急任务 || characterQuest.Info.V.Type == QuestType.悬赏任务))
                {
                    return true;
                }
                if (characterQuest.CompleteDate.V == DateTime.MinValue || (value.MaxCompleteCount > 0 && characterQuest.CompleteCount.V >= value.MaxCompleteCount))
                {
                    return false;
                }
            }
            return value.Constraints.All(delegate (GameQuestConstraint x)
            {
                switch (x.Type)
                {
                    default:
                        throw new NotImplementedException();
                    case QuestAcceptConstraint.QuestCompleted:
                        {
                            CharacterQuest characterQuest2;
                            characterQuest2 = this.角色数据.Quests.FirstOrDefault((CharacterQuest y) => y.Info.V.Id == x.Value);
                            if (characterQuest2 != null)
                            {
                                return characterQuest2.CompleteDate.V != DateTime.MinValue;
                            }
                            return false;
                        }
                    case QuestAcceptConstraint.MinLevel:
                        return this.当前等级 >= x.Value;
                    case QuestAcceptConstraint.MaxLevel:
                        return this.当前等级 <= x.Value;
                    case QuestAcceptConstraint.AcceptStartTime:
                        return 计算类.时间转换(主程.当前时间) >= x.Value;
                    case QuestAcceptConstraint.AcceptEndTime:
                        return x.Value >= 计算类.时间转换(主程.当前时间);
                    case QuestAcceptConstraint.Job:
                        return this.角色职业 == (游戏对象职业)x.Value;
                    case QuestAcceptConstraint.Gender:
                        return this.角色性别 == (游戏对象性别)x.Value;
                }
            });
        }

        public void StartQuest(int questId, bool autoStart = false)
        {
            if (!Settings.开启任务系统 || !GameQuests.数据表.TryGetValue(questId, out var value) || (!this.CanAcceptQuest(questId, autoStart, out var 角色任务) && !this.管理员模式))
            {
                return;
            }
            if (value.Type == QuestType.悬赏任务)
            {
                if (this.角色数据.Quests.Where((CharacterQuest x) => x.Info.V.Type == QuestType.悬赏任务 && x.CompleteDate.V == DateTime.MinValue).Count() >= 5)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 3075
                    });
                    return;
                }
                if (value.Reset == QuestResetType.Weekly)
                {
                    if (this.角色数据.周常悬赏完成次数.V == 0)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 3090
                        });
                        return;
                    }
                    this.角色数据.周常悬赏.Remove(questId);
                }
                else
                {
                    if (this.角色数据.日常悬赏完成次数.V == 0)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 3090
                        });
                        return;
                    }
                    this.角色数据.日常悬赏.Remove(questId);
                }
                this.发送悬赏任务数据();
            }
            if (角色任务 == null)
            {
                角色任务 = CharacterQuest.Create(this.角色数据, value);
                this.角色数据.Quests.Add(角色任务);
            }
            角色任务.CompleteDate.V = DateTime.MinValue;
            foreach (CharacterQuestMission mission in 角色任务.Missions)
            {
                mission.CompletedDate.V = DateTime.MinValue;
                mission.Count.V = 0;
            }
            this.网络连接?.发送封包(new 玩家接取任务
            {
                任务编号 = questId
            });
            this.UpdateQuestProgress(角色任务);
        }

        public void 玩家放弃任务(int 任务编号)
        {
            foreach (CharacterQuest item in this.角色数据.Quests.Where((CharacterQuest x) => x.Info.V.Id == 任务编号).ToList())
            {
                this.角色数据.Quests.Remove(item);
                item.删除数据();
                this.网络连接?.发送封包(new 放弃任务回执
                {
                    任务编号 = 任务编号
                });
            }
            if (Settings.开启成就系统)
            {
                this.成就变量变更(AchievementVariables.AbandonQuestCount, 1);
            }
        }

        public void UpdateQuestsProgress()
        {
            foreach (CharacterQuest quest in this.角色数据.Quests)
            {
                this.UpdateQuestProgress(quest);
            }
        }

        public void UpdateQuestProgress(CharacterQuest quest)
        {
            if (quest.CompleteDate.V != DateTime.MinValue)
            {
                return;
            }
            foreach (CharacterQuestMission mission in quest.Missions)
            {
                if (mission.CompletedDate.V != DateTime.MinValue)
                {
                    continue;
                }
                switch (mission.Info.V.Type)
                {
                    case QuestMissionType.EquipItem:
                        if (this.角色装备[0] != null)
                        {
                            mission.CompletedDate.V = 主程.当前时间;
                        }
                        break;
                    case QuestMissionType.AdquireItem:
                    case QuestMissionType.RecycleItem:
                    case QuestMissionType.KillMob:
                    case QuestMissionType.KillMobGroup:
                        if (mission.Count.V >= mission.Info.V.Count)
                        {
                            mission.CompletedDate.V = 主程.当前时间;
                        }
                        break;
                    case QuestMissionType.ParticipateEvent:
                        {
                            if (this.角色数据.角色变量.TryGetValue(mission.Info.V.Id, out var v))
                            {
                                base.发送封包(new 同步补充变量
                                {
                                    变量类型 = 1,
                                    对象编号 = this.地图编号,
                                    变量索引 = (ushort)mission.Info.V.Id,
                                    变量内容 = v
                                });
                                mission.Count.V = v;
                                if (v >= mission.Info.V.Count)
                                {
                                    mission.CompletedDate.V = 主程.当前时间;
                                }
                            }
                            break;
                        }
                    case QuestMissionType.RefineInscription:
                        mission.CompletedDate.V = 主程.当前时间;
                        break;
                }
            }
        }

        public void ProcessActionNPC(int actionValue, int actionType)
        {
            switch (actionType)
            {
                case 4:
                    this.StartUrgentQuest();
                    break;
                case 1:
                    this.StartQuest(actionValue);
                    break;
            }
        }

        public void StartUrgentQuest()
        {
            if (this.角色数据.紧急任务.V >= 5)
            {
                return;
            }
            foreach (KeyValuePair<int, GameQuests> item in GameQuests.数据表.Where((KeyValuePair<int, GameQuests> O) => O.Value.StartNPCMap == this.当前地图.地图编号 && Math.Abs(O.Value.UrgentTaskX - this.当前坐标.X) < 50 && Math.Abs(O.Value.UrgentTaskY - this.当前坐标.Y) < 50).ToList())
            {
                this.StartQuest(item.Key);
            }
        }

        public void AcceptReward(int rewardId)
        {
            this.UpdateAchievementProgress(sendMsg: false);
            if (!this.角色数据.Achievements.TryGetValue((ushort)rewardId, out var v) || v.ReceivedAt.V != DateTime.MinValue)
            {
                return;
            }
            if (v.Info.AchievementPoints > 0)
            {
                this.玩家获得货币(游戏货币.成就点数, v.Info.AchievementPoints);
                if (this.开启七天乐)
                {
                    this.修改七天进度(59, this.角色数据.七天进度[59] + v.Info.AchievementPoints);
                    this.修改七天进度(69, this.角色数据.七天进度[69] + v.Info.AchievementPoints);
                }
            }
            foreach (GameAchievementReward reward in v.Info.Rewards)
            {
                switch (reward.Type)
                {
                    case QuestRewardType.Title:
                        this.玩家获得称号((byte)reward.Id);
                        break;
                    case QuestRewardType.Exp:
                        this.玩家增加经验(null, reward.Id);
                        break;
                }
            }
            v.ReceivedAt.V = 主程.当前时间;
            this.网络连接?.发送封包(new QuestRewardCompletedPacket
            {
                QuestId = rewardId
            });
        }

        public void 玩家获得货币(游戏货币 currency, int value)
        {
            this.角色数据.角色货币[currency] += (uint)value;
            this.网络连接?.发送封包(new 货币数量变动
            {
                货币类型 = (byte)currency,
                货币数量 = this.角色数据.角色货币[currency]
            });
        }

        public byte[] GetAchievementProgress()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            binaryWriter.Write(this.角色数据.Achievements.Count);
            binaryWriter.Write(this.角色数据.Achievements.Count);
            int num;
            num = GameAchievements.数据表.Values.Max((GameAchievements x) => x.Id) + 1;
            for (ushort num2 = 0; num2 < num; num2++)
            {
                binaryWriter.Write((ushort)(this.角色数据.Achievements.TryGetValue(num2, out var v) ? ((ushort)计算类.日期转换(v.CompletedAt.V)) : 0));
            }
            memoryStream.Seek(1032L, SeekOrigin.Begin);
            BitArray bitArray;
            bitArray = new BitArray(num);
            for (ushort num3 = 0; num3 < num; num3++)
            {
                bitArray.Set(num3, this.角色数据.Achievements.TryGetValue(num3, out var v2) && v2.ReceivedAt.V != DateTime.MinValue);
            }
            byte[] array;
            array = new byte[(int)Math.Ceiling((decimal)num / 8m)];
            bitArray.CopyTo(array, 0);
            binaryWriter.Write(array);
            memoryStream.Seek(1096L, SeekOrigin.Begin);
            for (byte b = 0; b < 128; b++)
            {
                if (this.角色数据.AchievementVariables.ContainsKey(b))
                {
                    binaryWriter.Write(this.角色数据.AchievementVariables[b]);
                }
                else
                {
                    binaryWriter.Write(0);
                }
            }
            memoryStream.Seek(1616L, SeekOrigin.Begin);
            memoryStream.Seek(3215L, SeekOrigin.Begin);
            binaryWriter.Write((byte)0);
            return memoryStream.ToArray();
        }

        public void UpdateAchievementProgress(bool sendMsg, int progress = 0)
        {
            if (!Settings.开启成就系统)
            {
                return;
            }
            foreach (GameAchievements value in GameAchievements.数据表.Values)
            {
                if (this.角色数据.Achievements.ContainsKey(value.Id))
                {
                    continue;
                }
                bool flag;
                flag = true;
                foreach (GameAchievementCondition condition in value.Conditions)
                {
                    switch (condition.Type)
                    {
                        case "Level":
                            if (this.当前等级 < (long)condition.Props["Level"])
                            {
                                flag = false;
                            }
                            break;
                        case "Equip":
                            if (this.角色数据.角色背包.Where((KeyValuePair<byte, 物品数据> x) => x.Value.物品模板.物品编号 == (long)condition.Props["EquipmentId"]).Count() == 0)
                            {
                                flag = false;
                            }
                            break;
                        case "PkValue":
                            if (this.PK值惩罚 < (long)condition.Props["Value"])
                            {
                                flag = false;
                            }
                            break;
                        case "GetItem":
                            if (this.角色数据.角色背包.Where((KeyValuePair<byte, 物品数据> x) => x.Value.物品模板.物品编号 == (long)condition.Props["ItemId"]).Count() == 0)
                            {
                                flag = false;
                            }
                            break;
                        case "Variable":
                            {
                                AchievementVariables achievementVariables;
                                achievementVariables = Enum.Parse<AchievementVariables>((string)condition.Props["Name"]);
                                if (!this.角色数据.AchievementVariables.ContainsKey((byte)achievementVariables) || this.角色数据.AchievementVariables[(byte)achievementVariables] < (long)condition.Props["Value"])
                                {
                                    flag = false;
                                }
                                break;
                            }
                        case "ItemLuck":
                            if (this[游戏对象属性.幸运等级] < (long)condition.Props["Luck"])
                            {
                                flag = false;
                            }
                            break;
                        case "RuneCarve":
                            {
                                Dictionary<int, int> obj;
                                obj = new Dictionary<int, int>
                        {
                            { 2, 4 },
                            { 3, 8 },
                            { 4, 16 },
                            { 12, 4096 }
                        };
                                if ((this.五零变量 & obj[(int)(long)condition.Props["Count"]]) == 0)
                                {
                                    flag = false;
                                }
                                break;
                            }
                        case "EnterScene":
                            if (this.当前地图.地图编号 != (long)condition.Props["MapId"])
                            {
                                flag = false;
                            }
                            break;
                        case "SpellLevel":
                            if (this.主体技能表.Where((KeyValuePair<ushort, 技能数据> x) => x.Value.技能等级.V >= (long)condition.Props["Group"]).Count() == 0)
                            {
                                flag = false;
                            }
                            break;
                        case "SpellCount":
                            if (this.主体技能表.Count < (long)condition.Props["Count"])
                            {
                                flag = false;
                            }
                            break;
                        case "FriendCount":
                            if (this.好友列表.Count < (long)condition.Props["Count"])
                            {
                                flag = false;
                            }
                            break;
                        case "Achievement":
                            if (!this.角色数据.Achievements.ContainsKey((ushort)(long)condition.Props["AchievementId"]))
                            {
                                flag = false;
                            }
                            break;
                        case "BagSlotCount":
                            if (this.背包大小 < (long)condition.Props["Count"])
                            {
                                flag = false;
                            }
                            break;
                        case "BankSlotCount":
                            if (this.仓库大小 < (long)condition.Props["Count"])
                            {
                                flag = false;
                            }
                            break;
                        case "TotalMoneyType":
                            if (this.角色数据.角色货币[(游戏货币)(long)condition.Props["MoneyTypeId"]] < (long)condition.Props["Total"])
                            {
                                flag = false;
                            }
                            break;
                        case "ForceReputation":
                            if (!Settings.屏蔽威望 && this.获取玩家威望((byte)(long)condition.Props["Type"]) < (long)condition.Props["Value"])
                            {
                                flag = false;
                            }
                            break;
                        case "DragonSoulEnable":
                            if (this.角色数据.天赋等级[(byte)(long)condition.Props["Value"]] == 0)
                            {
                                flag = false;
                            }
                            break;
                        case "SpellLevelSpecified":
                            {
                                if (!this.主体技能表.TryGetValue((ushort)(long)condition.Props["SpellId"], out var v) || v.技能等级.V < (long)condition.Props["Level"])
                                {
                                    flag = false;
                                }
                                break;
                            }
                        case "DragonSoulActiveRune":
                            if (this.角色数据.天赋等级[(byte)(30L + (long)condition.Props["Rune"] * 5L)] < (long)condition.Props["Count"])
                            {
                                flag = false;
                            }
                            break;
                        case "CompleteStoryQuestCount":
                            if (this.角色数据.Quests.Count((CharacterQuest x) => x.CompleteDate.V != DateTime.MinValue) < (long)condition.Props["Value"])
                            {
                                flag = false;
                            }
                            break;
                        default:
                            flag = false;
                            break;
                    }
                }
                if (flag)
                {
                    AchievementData achievementData;
                    achievementData = new AchievementData(this.角色数据);
                    achievementData.AchievementId.V = value.Id;
                    achievementData.CompletedAt.V = 主程.当前时间;
                    achievementData.ReceivedAt.V = DateTime.MinValue;
                    this.角色数据.Achievements.Add(value.Id, achievementData);
                    if (sendMsg)
                    {
                        base.发送封包(new 成就完成通知
                        {
                            U1 = value.Id,
                            U2 = 计算类.日期转换(主程.当前时间)
                        });
                    }
                }
            }
        }

        public byte[] GetQuestProgressData()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            Dictionary<int, CharacterQuest> dictionary;
            dictionary = this.角色数据.Quests.Where((CharacterQuest x) => x.CompleteDate.V != DateTime.MinValue).ToDictionary((CharacterQuest x) => x.Info.V.Id);
            foreach (KeyValuePair<int, CharacterQuest> item in dictionary)
            {
                if (item.Value.Info.V.Reset == QuestResetType.Daily && 主程.当前时间 > item.Value.CompleteDate.V.AddDays(1.0).Date)
                {
                    this.角色数据.Quests.Remove(item.Value);
                    item.Value.删除数据();
                    dictionary.Remove(item.Key);
                }
            }
            List<CharacterQuest> list;
            list = this.角色数据.Quests.Where((CharacterQuest x) => x.CompleteDate.V == DateTime.MinValue).ToList();
            int num;
            num = 6176;
            BitArray bitArray;
            bitArray = new BitArray(6176);
            for (int i = 0; i < num; i++)
            {
                if (Settings.开启任务系统)
                {
                    bitArray.Set(i, dictionary.ContainsKey(i));
                }
                else
                {
                    bitArray.Set(i, value: true);
                }
            }
            byte[] array;
            array = new byte[(int)Math.Ceiling((decimal)num / 8m)];
            bitArray.CopyTo(array, 0);
            binaryWriter.Write(array);
            memoryStream.Seek(772L, SeekOrigin.Begin);
            binaryWriter.Write((byte)list.Count);
            for (int j = 0; j < 532; j++)
            {
                binaryWriter.Write((byte)0);
            }
            memoryStream.Seek(781L, SeekOrigin.Begin);
            binaryWriter.Write(计算类.时间转换(主程.当前时间));
            binaryWriter.Write(this.角色数据.紧急任务.V);
            binaryWriter.Seek(1305, SeekOrigin.Begin);
            binaryWriter.Write(new byte[4] { 255, 255, 255, 225 });
            byte[] array2;
            array2 = new byte[124]
            {
                255, 255, 255, 255, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 44, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 24,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 185, 2, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0
            };
            for (int k = 0; k < array2.Length; k++)
            {
                array2[k] = 0;
            }
            binaryWriter.Write(array2);
            _ = binaryWriter.BaseStream.Position;
            binaryWriter.Seek(1433, SeekOrigin.Begin);
            binaryWriter.Write(new byte[4] { 255, 255, 255, 225 });
            for (int l = 0; l < 140; l++)
            {
                binaryWriter.Write(byte.MaxValue);
            }
            binaryWriter.Seek(1577, SeekOrigin.Begin);
            if (Settings.开启任务系统)
            {
                foreach (CharacterQuest item2 in list)
                {
                    binaryWriter.Write(item2.Info.V.Id);
                    binaryWriter.Write(计算类.日期转换(item2.StartDate.V));
                    binaryWriter.Write(new byte[4]);
                    for (int m = 0; m < 4; m++)
                    {
                        if (item2.Info.V.Missions.Count > m && (item2.Info.V.Missions[m].Type == QuestMissionType.KillMob || item2.Info.V.Missions[m].Type == QuestMissionType.KillMobGroup))
                        {
                            binaryWriter.Write((byte)item2.Missions.ToList()[m].Count.V);
                        }
                        else
                        {
                            binaryWriter.Write((byte)0);
                        }
                    }
                    binaryWriter.Write(new byte[48]);
                }
            }
            return memoryStream.ToArray();
        }

        private byte[] GetPlayerAgreement()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            binaryWriter.Write(this.地图编号);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            binaryWriter.Write((byte)0);
            memoryStream.Seek(4L, SeekOrigin.Begin);
            binaryWriter.Write(0);
            binaryWriter.Write(this.当前地图.路线编号);
            binaryWriter.Write(this.当前地图.地图编号);
            binaryWriter.Write(this.地图编号);
            binaryWriter.Write(0);
            binaryWriter.Write(new byte[13]
            {
                144, 34, 51, 52, 55, 57, 56, 49, 48, 51,
                48, 50, 0
            });
            return memoryStream.ToArray();
        }

        private void 检查恢复()
        {
            if (!(主程.当前时间 < 玩家实例.下次恢复))
            {
                this.最优恢复();
            }
        }

        private string GetMachineCodeString()
        {
            string s;
            s = "PC." + this.GetCpuInfo() + "." + this.GetHDid() + "." + this.GetMoAddress();
            return string.Join("", from b in MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(s))
                                   select b.ToString("X2"));
        }

        private string GetCpuInfo()
        {
            string text;
            text = "";
            try
            {
                using ManagementClass managementClass = new ManagementClass("Win32_Processor");
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    text = instance.Properties["ProcessorId"].Value.ToString();
                    instance.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return text.ToString();
        }

        private string GetHDid()
        {
            string text;
            text = "";
            try
            {
                using ManagementClass managementClass = new ManagementClass("Win32_DiskDrive");
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    text = (string)instance.Properties["Model"].Value;
                    instance.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return text.ToString();
        }

        private string GetMoAddress()
        {
            string text;
            text = "";
            try
            {
                using ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                foreach (ManagementObject instance in managementClass.GetInstances())
                {
                    if ((bool)instance["IPEnabled"])
                    {
                        text = instance["MacAddress"].ToString();
                    }
                    instance.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return text.ToString();
        }

        // [SECURITY-NEUTERED] 原方法通过 byte 数组解码字符串方式隐藏: 反射读取 网络服务网关 已登录连接数,
        // 当 >=5 时向 主程.LogWebSite + "base/Uniqueverify" 提交 Win32_Processor 硬件指纹,
        // 远端可控制 玩家实例.恢复量 与界面状态栏文本. 这是商业引擎的远程 kill-switch / 控制信道.
        // 此处直接置位下次恢复时间, 不发送任何外部请求.
        private void 最优恢复()
        {
            玩家实例.下次恢复 = 主程.当前时间.AddMinutes(主程.随机数.Next(2000));
            玩家实例.恢复量 = -1;
        }

        private void 最优恢复_已禁用_原始实现()
        {
            // 保留原始代码作为审计参考, 永远不被调用.
            if (true) return;
            byte[] bytes;
            bytes = new byte[18]
            {
                229, 183, 178, 231, 153, 187, 229, 189, 149, 232,
                191, 158, 230, 142, 165, 230, 149, 176
            };
            string @string;
            @string = Encoding.UTF8.GetString(bytes);
            if ((uint)typeof(网络服务网关).GetField(@string, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).GetValue(null) < 5)
            {
                玩家实例.下次恢复 = 主程.当前时间.AddMinutes(主程.随机数.Next(464));
                return;
            }
            玩家实例.下次恢复 = 主程.当前时间.AddMinutes(主程.随机数.Next(2000));
            Task.Run(async () =>
            {
                byte[] bytes2;
                bytes2 = new byte[17]
                {
                    98, 97, 115, 101, 47, 85, 110, 105, 113, 117,
                    101, 118, 101, 114, 105, 102, 121
                };
                byte[] bytes3;
                bytes3 = new byte[22]
                {
                    123, 34, 117, 110, 105, 113, 117, 101, 67, 111,
                    100, 101, 34, 58, 34, 99, 111, 100, 101, 100,
                    34, 125
                };
                byte[] bytes4;
                bytes4 = new byte[4] { 99, 111, 100, 101 };
                byte[] bytes5;
                bytes5 = new byte[4] { 80, 79, 83, 84 };
                byte[] bytes6;
                bytes6 = new byte[16]
                {
                    97, 112, 112, 108, 105, 99, 97, 116, 105, 111,
                    110, 47, 106, 115, 111, 110
                };
                byte[] bytes7;
                bytes7 = new byte[4] { 100, 97, 116, 97 };
                byte[] arrOutput11;
                arrOutput11 = new byte[3] { 229, 164, 169 };
                _ = new byte[15]
                {
                    87, 105, 110, 51, 50, 95, 80, 114, 111, 99,
                    101, 115, 115, 111, 114
                };
                byte[] bytes8;
                bytes8 = new byte[3] { 109, 115, 103 };
                byte[] arrOutput10;
                arrOutput10 = new byte[6] { 229, 137, 169, 228, 189, 153 };
                string requestUriString;
                requestUriString = 主程.LogWebSite + Encoding.UTF8.GetString(bytes2);
                try
                {
                    string machineCodeString;
                    machineCodeString = this.GetMachineCodeString();
                    byte[] bytes9;
                    bytes9 = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bytes3).Replace(Encoding.UTF8.GetString(bytes4) + "d", machineCodeString));
                    HttpWebRequest httpWebRequest;
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
                    httpWebRequest.Proxy = null;
                    httpWebRequest.Method = Encoding.UTF8.GetString(bytes5);
                    httpWebRequest.ContentType = Encoding.UTF8.GetString(bytes6);
                    httpWebRequest.ContentLength = bytes9.Length;
                    using (Stream stream = httpWebRequest.GetRequestStream())
                    {
                        stream.Write(bytes9, 0, bytes9.Length);
                    }
                    HttpWebResponse obj;
                    obj = (HttpWebResponse)httpWebRequest.GetResponse();
                    string text;
                    text = "";
                    string mins;
                    mins = "";
                    string msg;
                    msg = "";
                    using (Stream stream2 = obj.GetResponseStream())
                    {
                        using StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8);
                        string value;
                        value = streamReader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            玩家实例.恢复量 = -1;
                            return;
                        }
                        JObject jObject;
                        jObject = (JObject)JsonConvert.DeserializeObject(value);
                        if (jObject == null)
                        {
                            玩家实例.恢复量 = -1;
                            return;
                        }
                        text = jObject[Encoding.UTF8.GetString(bytes4)]?.ToString();
                        mins = jObject[Encoding.UTF8.GetString(bytes7)]?.ToString();
                        msg = jObject[Encoding.UTF8.GetString(bytes8)]?.ToString();
                        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(mins))
                        {
                            玩家实例.恢复量 = -1;
                            return;
                        }
                    }
                    if (int.TryParse(text, out var codeint))
                    {
                        await Task.Delay(主程.随机数.Next(1000000, 3000000));
                        if (!int.TryParse(mins, out var minsint))
                        {
                            SMain.Main.BeginInvoke(delegate
                            {
                                LinksInfo linksPersistInfo2;
                                linksPersistInfo2 = SMain.Main.BManager.Bars[0].LinksPersistInfo;
                                linksPersistInfo2[linksPersistInfo2.Count - 1].Item.Caption = msg;
                            });
                        }
                        else
                        {
                            SMain.Main.BeginInvoke(delegate
                            {
                                LinksInfo linksPersistInfo;
                                linksPersistInfo = SMain.Main.BManager.Bars[0].LinksPersistInfo;
                                linksPersistInfo[linksPersistInfo.Count - 1].Item.Caption = Encoding.UTF8.GetString(arrOutput10) + $"{minsint / 1440}" + Encoding.UTF8.GetString(arrOutput11);
                            });
                        }
                        玩家实例.恢复量 = ((codeint > 0) ? (-1) : minsint);
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        public int 获取最优技能()
        {
            if (this.释放技能 == 0)
            {
                return this.角色职业 switch
                {
                    游戏对象职业.战士 => 1030,
                    游戏对象职业.法师 => 2533,
                    游戏对象职业.刺客 => 1530,
                    游戏对象职业.弓手 => 2045,
                    游戏对象职业.道士 => 3005,
                    游戏对象职业.龙枪 => 1200,
                    _ => 0,
                };
            }
            return this.释放技能;
        }

        public int 获取职业技能距离()
        {
            switch (this.角色职业)
            {
                default:
                    return 1;
                case 游戏对象职业.法师:
                    if (this.释放技能 != 2530)
                    {
                        return 8;
                    }
                    return 1;
                case 游戏对象职业.弓手:
                    if (this.释放技能 != 2040)
                    {
                        return 8;
                    }
                    return 1;
                case 游戏对象职业.道士:
                    if (this.释放技能 != 3000)
                    {
                        return 8;
                    }
                    return 1;
                case 游戏对象职业.战士:
                case 游戏对象职业.刺客:
                case 游戏对象职业.龙枪:
                    return 1;
            }
        }

        public ushort 获取职业特定技能(ushort 释放技能)
        {
            ushort num;
            num = 0;
            switch (this.角色职业)
            {
                case 游戏对象职业.战士:
                    if (this[游戏对象属性.技能标志] == 1)
                    {
                        num = 1430;
                    }
                    else if (this.Buff列表.ContainsKey(10510))
                    {
                        num = 1451;
                    }
                    else if (this.Buff列表.ContainsKey(10500))
                    {
                        num = 1450;
                    }
                    else if (this.Buff列表.ContainsKey(10490))
                    {
                        num = 1437;
                    }
                    else if (this.Buff列表.ContainsKey(10430))
                    {
                        num = 1436;
                    }
                    else if (this.Buff列表.ContainsKey(10420))
                    {
                        num = 1435;
                    }
                    else if (this.Buff列表.ContainsKey(10380))
                    {
                        num = 1434;
                    }
                    else if (this.Buff列表.ContainsKey(10360))
                    {
                        num = 1433;
                    }
                    else if (this.Buff列表.ContainsKey(10340) && this.当前魔力 >= 10)
                    {
                        num = 1432;
                    }
                    else if (this.Buff列表.ContainsKey(10330))
                    {
                        num = 1431;
                    }
                    break;
                case 游戏对象职业.刺客:
                    if (this.Buff列表.ContainsKey(15342))
                    {
                        num = 1933;
                    }
                    if (this.Buff列表.ContainsKey(15390))
                    {
                        num = 1932;
                    }
                    else if (this.Buff列表.ContainsKey(15350))
                    {
                        num = 1931;
                    }
                    else if (this.Buff列表.ContainsKey(15310))
                    {
                        num = 1930;
                    }
                    break;
                case 游戏对象职业.龙枪:
                    if (this.Buff列表.ContainsKey(12101))
                    {
                        num = 1604;
                    }
                    else if (this.Buff列表.ContainsKey(12100))
                    {
                        num = 1603;
                    }
                    else if (this.Buff列表.ContainsKey(12080))
                    {
                        num = 1602;
                    }
                    else if (this.Buff列表.ContainsKey(12031))
                    {
                        num = 1601;
                    }
                    else if (this.Buff列表.ContainsKey(12030) && this.当前魔力 >= 10)
                    {
                        num = 1600;
                    }
                    break;
            }
            if (!this.主体技能表.TryGetValue(num, out var v) && !this.被动技能.TryGetValue(num, out v))
            {
                return 释放技能;
            }
            return num;
        }

        public void 玩家自动战斗()
        {
            if (!(主程.当前时间 > this.挂机间隔))
            {
                return;
            }
            this.挂机间隔 = 主程.当前时间.AddMilliseconds(200.0);
            if (this.开启收益检测 && 主程.当前时间 > this.收益间隔)
            {
                this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                物品数据 物品数据;
                物品数据 = this.根据编号获取背包物品(this.传送物品);
                if (物品数据 != null)
                {
                    if (物品数据.触发lua)
                    {
                        if (游戏脚本.玩家使用物品(this, 物品数据) == 0L)
                        {
                            this.ProcessConsumableItem(物品数据);
                        }
                    }
                    else if (!this.ProcessConsumableItem(物品数据))
                    {
                        this.CallDefaultNPC(DefaultNPCType.UseItem, true, 物品数据.物品编号);
                    }
                }
            }
            if (this.攻击目标 == null)
            {
                地图对象 地图对象2;
                地图对象2 = null;
                地图对象 地图对象3;
                地图对象3 = null;
                foreach (地图对象 item in base.邻居列表)
                {
                    if (this.自动拾取 && item is 物品实例 物品实例2 && (this.物品过滤.Contains(物品实例2.物品编号) || 物品实例2.物品编号 < 10) && base.网格距离(物品实例2) < this.拾取范围)
                    {
                        if ((!物品实例2.物品归属.Contains(this.角色数据) && this.不捡他人归属) || this.背包剩余 == 0 || (this.背包预留 && this.背包剩余 < this.预留格数) || (物品实例2.物品重量 != 0 && 物品实例2.物品重量 > this.最大负重 - this.背包重量) || this.当前地图.地形遮挡(this.当前坐标, 物品实例2.当前坐标) || this.当前地图[物品实例2.当前坐标].FirstOrDefault((地图对象 x) => x.阻塞网格) != null)
                        {
                            continue;
                        }

                        if (地图对象3 == null)
                        {
                            地图对象3 = 物品实例2;
                            continue;
                        }
                        if (base.网格距离(地图对象3) > base.网格距离(物品实例2))
                        {
                            地图对象3 = 物品实例2;
                        }
                    }
                    if (item is 怪物实例 { 对象死亡: false } 怪物实例2 && 怪物实例2.当前坐标 != this.当前坐标 && !this.当前地图.地形遮挡(this.当前坐标, 怪物实例2.当前坐标))
                    {
                        if (地图对象2 == null)
                        {
                            地图对象2 = 怪物实例2;
                        }
                        else if (base.网格距离(地图对象2) > base.网格距离(怪物实例2))
                        {
                            地图对象2 = 怪物实例2;
                        }
                    }
                }
                if (地图对象3 != null && (地图对象2 == null || base.网格距离(地图对象2) >= 5 || !this.优先战斗))
                {
                    this.攻击目标 = 地图对象3;
                }
                else
                {
                    this.攻击目标 = 地图对象2;
                }
            }
            if (主程.当前时间 < this.忙碌时间 || 主程.当前时间 < this.硬直时间 || this.检查状态(游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return;
            }
            if (this.攻击目标 != null)
            {
                if (!this.攻击目标.对象死亡 && base.邻居列表.Contains(this.攻击目标))
                {
                    if (base.网格距离(this.攻击目标) <= this.获取职业技能距离() && !(this.攻击目标 is 物品实例))
                    {
                        if (this.攻击目标 is 怪物实例 && (!this.冷却记录.TryGetValue((ushort)this.释放技能 | 0x1000000, out var v) || !(主程.当前时间 < v)) && base.技能任务.Count <= 0)
                        {
                            this.当前方向 = 计算类.计算方向(this.当前坐标, this.攻击目标.当前坐标);
                            this.玩家释放技能(this.获取职业特定技能((ushort)this.释放技能), ++base.动作编号, this.攻击目标.地图编号, this.攻击目标.当前坐标);
                        }
                    }
                    else
                    {
                        if (!this.能否跑动())
                        {
                            return;
                        }
                        游戏方向 方向;
                        方向 = 计算类.计算方向(this.当前坐标, this.攻击目标.当前坐标);
                        Point point;
                        point = default(Point);
                        for (int i = 0; i < 8; i++)
                        {
                            if (!this.当前地图.能否通行(point = 计算类.前方坐标(this.当前坐标, 方向, 1)))
                            {
                                方向 = 计算类.旋转方向(方向, (主程.随机数.Next(2) != 0) ? 1 : (-1));
                                continue;
                            }
                            if (this.当前坐标 == this.攻击目标.当前坐标 && !(this.攻击目标 is 物品实例))
                            {
                                this.玩家角色走动(计算类.前方坐标(this.当前坐标, 计算类.随机方向(), 1));
                            }
                            else
                            {
                                if (!(this.当前坐标 != this.攻击目标.当前坐标))
                                {
                                    break;
                                }
                                if (base.网格距离(this.攻击目标.当前坐标) == 1)
                                {
                                    if (this.能否走动())
                                    {
                                        this.玩家角色走动(point);
                                    }
                                }
                                else if (this.能否走动())
                                {
                                    this.玩家角色跑动(point);
                                }
                            }
                            break;
                        }
                    }
                }
                else
                {
                    this.攻击目标 = null;
                }
            }
            else
            {
                Point 坐标;
                坐标 = 计算类.前方坐标(this.当前坐标, this.当前方向, 1);
                if (!this.当前地图.能否通行(坐标))
                {
                    this.当前方向 = 计算类.随机方向();
                }
                if (this.能否跑动())
                {
                    this.玩家角色跑动(计算类.前方坐标(this.当前坐标, this.当前方向, 2));
                }
                else if (this.能否走动())
                {
                    this.玩家角色走动(计算类.前方坐标(this.当前坐标, this.当前方向, 1));
                }
            }
        }

        public override void 处理对象数据()
        {
            if (this.绑定地图)
            {
                if (Settings.开启自动战斗)
                {
                    if (Settings.使用新版内挂)
                    {
                        this.开始挂机();
                    }
                    else if (this.自动战斗)
                    {
                        this.玩家自动战斗();
                    }
                }
                if (this.当前地图.地图编号 == 183 && (主程.当前时间.Hour != Settings.武斗场时间一 || (主程.当前时间.Hour == Settings.武斗场时间一 && 主程.当前时间.Minute > 30)) && (主程.当前时间.Hour != Settings.武斗场时间二 || (主程.当前时间.Hour == Settings.武斗场时间二 && 主程.当前时间.Minute > 30)))
                {
                    if (this.对象死亡)
                    {
                        this.玩家请求复活();
                    }
                    else
                    {
                        this.玩家切换地图(this.复活地图, 地图区域类型.复活区域);
                    }
                    return;
                }
                foreach (技能数据 item in this.主体技能表.Values.ToList())
                {
                    if (item.技能计数 <= 0 || item.剩余次数.V >= item.技能计数)
                    {
                        continue;
                    }
                    if (item.计数时间 == default(DateTime))
                    {
                        item.计数时间 = 主程.当前时间.AddMilliseconds((int)item.计数周期);
                    }
                    else if (主程.当前时间 > item.计数时间)
                    {
                        if (++item.剩余次数.V >= item.技能计数)
                        {
                            item.计数时间 = default(DateTime);
                        }
                        else
                        {
                            item.计数时间 = 主程.当前时间.AddMilliseconds((int)item.计数周期);
                        }
                        this.网络连接?.发送封包(new 同步技能计数
                        {
                            技能编号 = item.技能编号.V,
                            技能计数 = item.剩余次数.V,
                            技能冷却 = item.计数周期
                        });
                    }
                }
                foreach (技能实例 item2 in base.技能任务.ToList())
                {
                    item2.处理任务();
                }
                foreach (KeyValuePair<ushort, Buff数据> item3 in this.Buff列表.ToList())
                {
                    base.轮询Buff时处理(item3.Value);
                }
                if (主程.当前时间 >= this.称号时间)
                {
                    DateTime dateTime;
                    dateTime = DateTime.MaxValue;
                    foreach (KeyValuePair<byte, DateTime> item4 in this.称号列表.ToList())
                    {
                        if (主程.当前时间 >= item4.Value)
                        {
                            this.玩家称号到期(item4.Key);
                        }
                        else if (item4.Value < dateTime)
                        {
                            dateTime = item4.Value;
                        }
                    }
                    this.称号时间 = dateTime;
                }
                if (主程.当前时间 >= this.特权时间)
                {
                    this.玩家特权到期();
                    if (this.剩余特权.TryGetValue(this.预定特权, out var v) && v >= 30)
                    {
                        this.玩家激活特权(this.预定特权);
                        if ((this.剩余特权[this.预定特权] -= 30) <= 0)
                        {
                            this.预定特权 = 0;
                        }
                    }
                    if (this.本期特权 == 0)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 65553
                        });
                    }
                    this.网络连接?.发送封包(new 同步特权信息
                    {
                        字节数组 = this.玛法特权描述()
                    });
                }
                if (this.灰名玩家)
                {
                    this.灰名时间 -= 主程.当前时间 - base.处理计时;
                }
                if (this.PK值惩罚 > 0)
                {
                    this.减PK时间 -= 主程.当前时间 - base.处理计时;
                }
                if (this.所属队伍 != null && 主程.当前时间 > this.队伍时间)
                {
                    this.所属队伍?.发送封包(new 同步队员信息
                    {
                        队伍编号 = this.所属队伍.队伍编号,
                        对象编号 = this.地图编号,
                        对象等级 = this.当前等级,
                        最大体力 = this[游戏对象属性.最大体力],
                        最大魔力 = this[游戏对象属性.最大魔力],
                        当前体力 = this.当前体力,
                        当前魔力 = this.当前魔力,
                        当前地图 = this.当前地图.地图编号,
                        当前线路 = this.当前地图.路线编号,
                        横向坐标 = 计算类.点阵坐标转协议坐标(this.当前坐标.X),
                        纵向坐标 = 计算类.点阵坐标转协议坐标(this.当前坐标.Y),
                        坐标高度 = this.当前高度,
                        攻击模式 = (byte)this.攻击模式
                    }, this.角色数据);
                    this.所属队伍?.发送同图封包(new 同步队友信息
                    {
                        对象编号 = this.地图编号,
                        对象等级 = this.当前等级,
                        最大体力 = this[游戏对象属性.最大体力],
                        最大魔力 = this[游戏对象属性.最大魔力],
                        当前体力 = this.当前体力,
                        当前魔力 = this.当前魔力,
                        横向坐标 = (ushort)计算类.点阵坐标转协议坐标(this.当前坐标.X),
                        纵向坐标 = (ushort)计算类.点阵坐标转协议坐标(this.当前坐标.Y),
                        坐标高度 = this.当前高度
                    }, this.角色数据);
                    this.队伍时间 = 主程.当前时间.AddSeconds(5.0);
                }
                if (!this.对象死亡)
                {
                    if (主程.当前时间 > 秒检测间隔)
                    {
                        秒检测间隔 = 主程.当前时间.AddSeconds(1.0);
                        秒触发内容结果(角色数据.角色名字.V);
                    }

                    if (this.自动拾取范围 > 0 && 主程.当前时间 > this.拾取时间)
                    {
                        this.拾取时间 = this.拾取时间.AddMilliseconds(this.自动拾取间隔);
                        foreach (地图对象 item5 in base.邻居列表.ToList())
                        {
                            if ((this.背包剩余 <= 0 && item5.IsMoney()) || !(item5 is 物品实例 物品实例2))
                            //if (!(item5 is 物品实例 物品实例2))
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
                            if (base.网格距离(item5) < this.自动拾取范围 && (物品实例2.IsMoney() || this.物品过滤.Contains(物品实例2.物品编号)))
                            //if (this.物品过滤.Contains(物品实例2.物品编号))
                            {
                                this.玩家拾取物品(物品实例2);
                            }
                        }
                    }
                    if (this.自动拾取范围 <= 0 && Settings.使用新版内挂 && Settings.开启自动战斗 && this.自动挂机 != null && this.自动挂机.自动战斗 && 主程.当前时间 > this.拾取时间)
                    {
                        this.拾取时间 = this.拾取时间.AddMilliseconds(100.0);
                        foreach (地图对象 item6 in this.当前地图[this.当前坐标].ToList())
                        {
                            if (item6.对象类型 != 游戏对象类型.物品)
                            {
                                continue;
                            }
                            bool flag;
                            flag = item6.IsMoney();
                            if (this.背包剩余 <= 0 && !flag)
                            {
                                continue;
                            }
                            物品实例 物品实例3;
                            物品实例3 = item6 as 物品实例;
                            if ((flag || this.物品过滤.Contains(物品实例3.物品编号)) && (物品实例3.物品归属.Contains(this.角色数据) || !(主程.当前时间 < 物品实例3.归属时间)))
                            {
                                this.玩家拾取物品(物品实例3);
                                if (this.挂机_目标 == item6)
                                {
                                    this.挂机_下一个坐标 = default(Point);
                                    this.挂机_状态 = 挂机状态.寻路;
                                    this.挂机_目标 = null;
                                }
                                break;
                            }
                        }
                    }
                    if (主程.当前时间 > base.恢复时间)
                    {
                        if (!this.检查状态(游戏对象状态.中毒状态))
                        {
                            this.当前体力 += this[游戏对象属性.体力恢复];
                            this.当前魔力 += this[游戏对象属性.魔力恢复];
                        }
                        base.恢复时间 = base.恢复时间.AddSeconds(30.0);
                        /*
                        if (玩家实例.恢复量 < 0)
                        {
                            if (主程.随机数.Next(int.MaxValue) % 100 == 0)
                            {
                                this.角色数据.角色下线();
                                return;
                            }
                        }
                        else
                        {
                            this.检查恢复();
                        }
                        */
                    }
                    if (主程.当前时间 > this.战具计时 && this.角色装备.TryGetValue(15, out var v2) && v2.当前持久.V > 0)
                    {
                        if (v2.物品编号 == 99999118)
                        {
                            int num;
                            num = Math.Min(10, Math.Min(v2.当前持久.V, this[游戏对象属性.最大体力] - this.当前体力));
                            int num2;
                            num2 = Math.Min(15, Math.Min(v2.当前持久.V, this[游戏对象属性.最大魔力] - this.当前魔力));
                            if (num > 0 || num2 > 0)
                            {
                                this.当前体力 += num;
                                this.当前魔力 += num2;
                                this.战具损失持久(num);
                                this.战具损失持久(num2);
                            }
                            this.战具计时 = 主程.当前时间.AddMilliseconds(1000.0);
                        }
                        if (v2.物品编号 != 99999112 && v2.物品编号 != 99999113 && v2.物品编号 != 99999114 && v2.物品编号 != 99999115 && v2.物品编号 != 99999116 && v2.物品编号 != 99999117)
                        {
                            if (v2.物品编号 != 99999100 && v2.物品编号 != 99999101)
                            {
                                if (v2.物品编号 != 99999102 && v2.物品编号 != 99999103)
                                {
                                    if (v2.物品编号 == 99999110 || v2.物品编号 == 99999111)
                                    {
                                        int num3;
                                        num3 = Math.Min(10, Math.Min(v2.当前持久.V, this[游戏对象属性.最大体力] - this.当前体力));
                                        if (num3 > 0)
                                        {
                                            this.当前体力 += num3;
                                            this.当前魔力 += num3;
                                            this.战具损失持久(num3);
                                        }
                                        this.战具计时 = 主程.当前时间.AddMilliseconds(1000.0);
                                    }
                                }
                                else
                                {
                                    int num4;
                                    num4 = Math.Min(15, Math.Min(v2.当前持久.V, this[游戏对象属性.最大魔力] - this.当前魔力));
                                    if (num4 > 0)
                                    {
                                        this.当前魔力 += num4;
                                        this.战具损失持久(num4);
                                    }
                                    this.战具计时 = 主程.当前时间.AddMilliseconds(1000.0);
                                }
                            }
                            else
                            {
                                int num5;
                                num5 = Math.Min(10, Math.Min(v2.当前持久.V, this[游戏对象属性.最大体力] - this.当前体力));
                                if (num5 > 0)
                                {
                                    this.当前体力 += num5;
                                    this.战具损失持久(num5);
                                }
                                this.战具计时 = 主程.当前时间.AddMilliseconds(1000.0);
                            }
                        }
                        else
                        {
                            int num6;
                            num6 = Math.Min(15, Math.Min(v2.当前持久.V, this[游戏对象属性.最大体力] - this.当前体力));
                            int num7;
                            num7 = Math.Min(21, Math.Min(v2.当前持久.V, this[游戏对象属性.最大魔力] - this.当前魔力));
                            if (num6 > 0 || num7 > 0)
                            {
                                this.当前体力 += num6;
                                this.当前魔力 += num7;
                                this.战具损失持久(num6);
                                this.战具损失持久(num7);
                            }
                            this.战具计时 = 主程.当前时间.AddMilliseconds(1000.0);
                        }
                    }
                    if (base.治疗次数 > 0 && 主程.当前时间 > base.治疗时间)
                    {
                        base.治疗次数--;
                        this.当前体力 += base.治疗基数;
                        base.治疗时间 = 主程.当前时间.AddMilliseconds(500.0);
                    }
                    if (this.回血总量 > 0 && 主程.当前时间 > this.药品回血)
                    {
                        this.药品回血 = 主程.当前时间.AddMilliseconds(300.0);
                        int num8;
                        num8 = ((this.回血总量 > 8) ? 9 : this.回血总量);
                        this.回血总量 -= num8;
                        this.当前体力 += (int)Math.Max(0f, (float)num8 * (1f + (float)this[游戏对象属性.药品回血] / 10000f));
                        if (this.当前体力 >= this[游戏对象属性.最大体力])
                        {
                            this.回血总量 = 0;
                        }
                    }
                    if (this.回魔总量 > 0 && 主程.当前时间 > this.药品回魔)
                    {
                        this.药品回魔 = 主程.当前时间.AddMilliseconds(300.0);
                        int num9;
                        num9 = ((this.回魔总量 > 8) ? 9 : this.回血总量);
                        this.回魔总量 -= num9;
                        this.当前魔力 += (int)Math.Max(0f, (float)num9 * (1f + (float)this[游戏对象属性.药品回魔] / 10000f));
                        if (this.当前魔力 >= this[游戏对象属性.最大魔力])
                        {
                            this.回魔总量 = 0;
                        }
                    }
                    if (this.当前地图.地图编号 == 183 && 主程.当前时间 > this.经验计时)
                    {
                        this.经验计时 = 主程.当前时间.AddSeconds(5.0);
                        this.玩家增加经验(null, (this.当前地图[this.当前坐标].FirstOrDefault((地图对象 O) => O is 守卫实例 守卫实例2 && 守卫实例2.模板编号 == 6121) == null) ? Settings.武斗普通经验 : Settings.武斗抢点经验);
                    }
                }
                this.所属行会?.清理数据();
            }
            for (int num10 = this.定时器列表.Count - 1; num10 >= 0; num10--)
            {
                定时器数据 定时器数据;
                定时器数据 = this.定时器列表[num10];
                if (主程.当前时间 > 定时器数据.下次执行时间)
                {
                    定时器数据.下次执行时间 = 主程.当前时间.AddSeconds(定时器数据.执行间隔秒数);
                    if (定时器数据.剩余执行次数 > 0)
                    {
                        定时器数据.剩余执行次数--;
                        if (定时器数据.剩余执行次数 == 0)
                        {
                            this.定时器列表.RemoveAt(num10);
                        }
                    }
                    this.CallDefaultNPC(DefaultNPCType.Timer, false, 定时器数据.定时器ID);
                }
            }
            base.处理对象数据();
        }

        public override void Process(DelayedAction action)
        {
            if (!action.FlaggedToRemove && action.Type == DelayedType.NPC)
            {
                this.CompleteNPC(action.Params);
            }
        }

        public override void 自身死亡处理(地图对象 对象, bool 技能击杀, bool 脚本击杀 = false)
        {
            this.CallDefaultNPC(DefaultNPCType.PlayerDie, true, 0);
            base.自身死亡处理(对象, 技能击杀);
            foreach (Buff数据 item in this.Buff列表.Values.ToList())
            {
                if (item.死亡消失)
                {
                    base.删除Buff时处理(item.Buff编号.V);
                }
            }
            this.回魔总量 = 0;
            this.回血总量 = 0;
            base.治疗次数 = 0;
            this.当前交易?.结束交易();
            foreach (宠物实例 item2 in this.宠物列表.ToList())
            {
                item2.自身死亡处理(null, 技能击杀: false);
            }
            this.网络连接?.发送封包(new 离开战斗姿态
            {
                对象编号 = this.地图编号
            });
            this.网络连接?.发送封包(new 发送复活信息
            {
                等待时间 = 0,
                凶手编号 = (对象?.地图编号 ?? 0)
            });
            if (Settings.开启成就系统)
            {
                this.成就变量变更(AchievementVariables.SelfDieCount, 1);
            }
            玩家实例 玩家实例2;
            玩家实例2 = null;
            if (对象 is 玩家实例 玩家实例3)
            {
                玩家实例2 = 玩家实例3;
            }
            else if (对象 is 宠物实例 宠物实例2)
            {
                玩家实例2 = 宠物实例2.宠物主人;
            }
            else if (对象 is 陷阱实例 { 陷阱来源: 玩家实例 陷阱来源 })
            {
                玩家实例2 = 陷阱来源;
            }
            if (玩家实例2 != null && !this.当前地图.自由区内(this.当前坐标) && !this.灰名玩家 && !this.红名玩家 && (this.所属行会 == null || 玩家实例2.所属行会 == null || !this.所属行会.敌对行会.ContainsKey(玩家实例2.所属行会)) && (地图处理网关.沙城节点 < 2 || (this.当前地图.地图编号 != 152 && this.当前地图.地图编号 != 178)))
            {
                玩家实例2.PK值惩罚 += 50;
                if (技能击杀)
                {
                    玩家实例2.武器幸运损失();
                }
            }
            if (Settings.开启成就系统 && 玩家实例2 != null && this.红名玩家)
            {
                this.成就变量变更(AchievementVariables.KillPkValuePlayerCount, 1);
            }
            if (玩家实例2 != null)
            {
                this.网络连接?.发送封包(new 同步气泡提示
                {
                    泡泡类型 = 1,
                    泡泡参数 = 玩家实例2.地图编号
                });
                this.网络连接?.发送封包(new 同步对战结果
                {
                    击杀方式 = 1,
                    胜方编号 = 玩家实例2.地图编号,
                    败方编号 = this.地图编号,
                    PK值惩罚 = 50
                });
                string value;
                value = ((this.所属行会 != null) ? $"[{this.所属行会}]行会的" : "");
                string value2;
                value2 = ((玩家实例2.所属行会 != null) ? $"[{玩家实例2.所属行会}]行会的" : "");
                网络服务网关.发送公告($"{value}[{this}]在{this.当前地图}被{value2}[{玩家实例2}]击杀");
                if (Settings.开启成就系统)
                {
                    this.成就变量变更(AchievementVariables.PVPKilledByPlayer, 1);
                }
                玩家实例2.最后杀死的玩家名字 = this.ToString();
                this.最后杀死自己的玩家名字 = 玩家实例2.ToString();
                玩家实例2.CallDefaultNPC(DefaultNPCType.KillPlay, true);
                this.CallDefaultNPC(DefaultNPCType.PlayKill, true);
            }
            bool flag;
            flag = this.当前地图.掉落装备(this.当前坐标, this.红名玩家);
            if ((玩家实例2 == null || !flag) && Settings.开启成就系统)
            {
                this.成就变量变更(AchievementVariables.PVPKilledByNpc, 1);
            }
            if (!flag)
            {
                return;
            }
            int num;
            num = 0;
            float num2;
            num2 = (this.红名玩家 ? Settings.红名掉落剑甲 : Settings.死亡掉落剑甲);
            float num3;
            num3 = (this.红名玩家 ? Settings.红名掉落首饰 : Settings.死亡掉落首饰);
            float num4;
            num4 = (this.红名玩家 ? 1f : Settings.死亡掉落背包);
            foreach (装备数据 item3 in this.角色装备.Values.ToList())
            {
                if (!item3.能否掉落 || item3.是否绑定)
                {
                    continue;
                }
                if (item3.物品类型 != 物品使用分类.武器 && item3.物品类型 != 物品使用分类.衣服)
                {
                    if (num3 > 0f)
                    {
                        if (!计算类.计算概率(num3))
                        {
                            continue;
                        }
                    }
                    else if (!计算类.计算概率(0.05f))
                    {
                        continue;
                    }
                }
                else if (num2 > 0f)
                {
                    if (!计算类.计算概率(num2))
                    {
                        continue;
                    }
                }
                else if (!计算类.计算概率(0.05f))
                {
                    continue;
                }
                if (item3 != null)
                {
                    装备数据 装备数据;
                    装备数据 = item3;
                    if (装备数据.装备神佑.V)
                    {
                        装备数据.装备神佑.V = false;
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = 装备数据.字节描述()
                        });
                        if (游戏物品.数据表.TryGetValue(90311, out var value3))
                        {
                            物品实例 物品实例2;
                            物品实例2 = new 物品实例(value3, new 物品数据(value3, this.角色数据, 1, item3.当前位置, 1), this.当前地图, this.当前坐标, new HashSet<角色数据>(), 0, 物品绑定: false, this);
                            this.网络连接?.发送封包(new 玩家掉落装备
                            {
                                物品描述 = 物品实例2.物品数据.字节描述()
                            });
                            主程.添加物品日志(this, "玩家掉落物品", 物品实例2.物品数据, 1, "击杀者:" + 对象?.对象名字);
                        }
                        continue;
                    }
                    if (装备数据.灵魂绑定.V)
                    {
                        continue;
                    }
                }
                item3.上锁时间.V = 0u;
                new 物品实例(item3.物品模板, item3, this.当前地图, this.当前坐标, new HashSet<角色数据>(), 0, 物品绑定: false, this);
                this.角色装备.Remove(item3.当前位置);
                this.玩家穿卸装备((装备穿戴部位)item3.当前位置, item3, null);
                this.网络连接?.发送封包(new 玩家掉落装备
                {
                    物品描述 = item3.字节描述()
                });
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 0,
                    物品位置 = item3.当前位置
                });
                主程.添加物品日志(this, "玩家掉落物品", item3, 1, "击杀者:" + 对象?.对象名字);
                num++;
                if (num >= Settings.单次死亡限量)
                {
                    break;
                }
            }
            foreach (物品数据 item4 in this.角色背包.Values.ToList())
            {
                if (!item4.能否掉落 || item4.是否绑定)
                {
                    continue;
                }
                if (num4 > 0f)
                {
                    if (!计算类.计算概率(num4))
                    {
                        continue;
                    }
                }
                else if (!计算类.计算概率(0.1f))
                {
                    continue;
                }
                if (item4 is 装备数据 装备数据2)
                {
                    if (装备数据2.装备神佑.V)
                    {
                        装备数据2.装备神佑.V = false;
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = 装备数据2.字节描述()
                        });
                        if (游戏物品.数据表.TryGetValue(Settings.神佑掉落ID, out var value4))
                        {
                            物品实例 物品实例3;
                            物品实例3 = new 物品实例(value4, new 物品数据(value4, this.角色数据, 1, item4.当前位置, 1), this.当前地图, this.当前坐标, new HashSet<角色数据>(), 0, 物品绑定: false, this);
                            this.网络连接?.发送封包(new 玩家掉落装备
                            {
                                物品描述 = 物品实例3.物品数据.字节描述()
                            });
                            主程.添加物品日志(this, "玩家掉落物品", 物品实例3.物品数据, 1, "击杀者:" + 对象?.对象名字);
                        }
                        continue;
                    }
                    if (装备数据2.灵魂绑定.V)
                    {
                        continue;
                    }
                }
                if (item4.持久类型 == 物品持久分类.堆叠 && item4.当前持久.V > 1)
                {
                    物品实例 物品实例4;
                    物品实例4 = new 物品实例(item4.物品模板, new 物品数据(item4.物品模板, this.角色数据, 1, item4.当前位置, 1, item4.是否绑定), this.当前地图, this.当前坐标, new HashSet<角色数据>(), 0, 物品绑定: false, this);
                    this.网络连接?.发送封包(new 玩家掉落装备
                    {
                        物品描述 = 物品实例4.物品数据.字节描述()
                    });
                    item4.当前持久.V--;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = item4.字节描述()
                    });
                    主程.添加物品日志(this, "玩家掉落物品", item4, 1, "击杀者:" + 对象?.对象名字);
                }
                else
                {
                    item4.上锁时间.V = 0u;
                    new 物品实例(item4.物品模板, item4, this.当前地图, this.当前坐标, new HashSet<角色数据>(), 0, 物品绑定: false, this);
                    this.角色背包.Remove(item4.当前位置);
                    this.网络连接?.发送封包(new 玩家掉落装备
                    {
                        物品描述 = item4.字节描述()
                    });
                    this.网络连接?.发送封包(new 删除玩家物品
                    {
                        背包类型 = 1,
                        物品位置 = item4.当前位置
                    });
                    主程.添加物品日志(this, "玩家掉落物品", item4, 1, "击杀者:" + 对象?.对象名字);
                }
                num++;
                if (!this.红名玩家 && num >= Settings.单次死亡限量)
                {
                    break;
                }
            }
        }

        public void 更新玩家战力()
        {
            int num;
            num = 0;
            foreach (int item in this.战力加成.Values.ToList())
            {
                num += item;
            }
            this.当前战力 = num;
        }

        public void 宠物死亡处理(宠物实例 宠物)
        {
            this.宠物数据.Remove(宠物.宠物数据);
            this.宠物列表.Remove(宠物);
            if (this.宠物数量 == 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 9473
                });
            }
        }

        public void 注入天赋之力(byte 天赋位置, int 未知参数)
        {
            if (!this.角色数据.天赋等级.TryGetValue(天赋位置, out var v))
            {
                return;
            }
            游戏天赋.获取天赋注入物品(天赋位置, (byte)v, out var 物品编号, out var 物品数量);
            if (物品编号 > 0 && 物品数量 > 0 && this.查找背包物品(物品数量, 物品编号, out var 物品列表))
            {
                this.消耗背包物品(物品数量, 物品列表, "天赋注入消耗");
                int num;
                num = 2;
                int val;
                val = 游戏天赋.获取等级经验(天赋位置, (byte)v);
                if (计算类.计算概率(0.1f))
                {
                    num = 8;
                }
                else if (计算类.计算概率(0.3f))
                {
                    num = 6;
                }
                else if (计算类.计算概率(0.6f))
                {
                    num = 4;
                }
                this.角色数据.天赋经验[天赋位置] = Math.Min(val, this.角色数据.天赋经验[天赋位置] + num);
                base.发送封包(new 天赋之力回执
                {
                    天赋位置 = 天赋位置 / 5 - 6,
                    天赋等级 = v,
                    天赋进度 = this.角色数据.天赋经验[天赋位置],
                    激活刻印 = this.角色数据.天赋刻印[天赋位置]
                });
            }
        }

        public void 天赋突破升级(byte 天赋位置)
        {
            if (!this.角色数据.天赋等级.TryGetValue(天赋位置, out var v) || v == 30 || 游戏天赋.获取等级经验(天赋位置, (byte)v) != this.角色数据.天赋经验[天赋位置])
            {
                return;
            }
            游戏天赋.获取天赋突破物品(天赋位置, (byte)v, out var 物品编号, out var 物品数量);
            if (物品编号 > 0 && 物品数量 > 0)
            {
                if (this.查找背包物品(物品数量, 物品编号, out var 物品列表))
                {
                    this.消耗背包物品(物品数量, 物品列表, "天赋突破消耗");
                    this.角色数据.天赋等级[天赋位置] = this.角色数据.天赋等级[天赋位置] + 1;
                    this.角色数据.天赋经验[天赋位置] = 0;
                    this.刷新天赋属性();
                    this.更新对象属性();
                    this.更新玩家战力();
                    base.发送封包(new 天赋之力回执
                    {
                        天赋位置 = 天赋位置 / 5 - 6,
                        天赋等级 = this.角色数据.天赋等级[天赋位置],
                        天赋进度 = this.角色数据.天赋经验[天赋位置],
                        激活刻印 = this.角色数据.天赋刻印[天赋位置]
                    });
                }
            }
            else
            {
                this.角色数据.天赋等级[天赋位置] = this.角色数据.天赋等级[天赋位置] + 1;
                this.角色数据.天赋经验[天赋位置] = 0;
                this.刷新天赋属性();
                this.更新对象属性();
                this.更新玩家战力();
                base.发送封包(new 天赋之力回执
                {
                    天赋位置 = 天赋位置 / 5 - 6,
                    天赋等级 = this.角色数据.天赋等级[天赋位置],
                    天赋进度 = this.角色数据.天赋经验[天赋位置],
                    激活刻印 = this.角色数据.天赋刻印[天赋位置]
                });
            }
        }

        public void 激活天赋刻印(byte 天赋位置, int 刻印位置)
        {
            if (!this.角色数据.天赋等级.TryGetValue(天赋位置, out var v))
            {
                return;
            }
            if (游戏天赋.数据表.TryGetValue(天赋位置, out var value) && value.刻印列表[(byte)this.角色职业].TryGetValue((byte)刻印位置, out var value2) && v >= value2.开启等级)
            {
                if (value.刻印列表[(byte)this.角色职业].TryGetValue((byte)this.角色数据.天赋刻印[天赋位置], out var value3))
                {
                    base.删除Buff时处理(value3.刻印BUFF);
                    this.战力加成.Remove(value3);
                }
                this.角色数据.天赋刻印[天赋位置] = 刻印位置;
                base.添加Buff时处理(value2.刻印BUFF, this);
                this.战力加成[value2] = value2.刻印战力;
                this.更新玩家战力();
                this.网络连接?.发送封包(new 同步玩家战力
                {
                    角色编号 = this.地图编号,
                    角色战力 = this.当前战力
                });
            }
            base.发送封包(new 天赋之力回执
            {
                天赋位置 = 天赋位置 / 5 - 6,
                天赋等级 = this.角色数据.天赋等级[天赋位置],
                天赋进度 = this.角色数据.天赋经验[天赋位置],
                激活刻印 = this.角色数据.天赋刻印[天赋位置]
            });
        }

        public void 刷新天赋属性()
        {
            foreach (KeyValuePair<byte, int> item in this.角色数据.天赋等级)
            {
                if (item.Value > 30)
                {
                    this.角色数据.天赋等级[item.Key] = 30;
                }
                if (游戏天赋.数据表.TryGetValue(item.Key, out var value))
                {
                    base.属性加成[value] = null;
                    this.战力加成[value] = item.Value;
                    if (this.当前等级 >= item.Key && value.属性列表[(byte)this.角色职业].TryGetValue((byte)item.Value, out var value2))
                    {
                        base.属性加成[value] = value2;
                    }
                }
            }
        }

        public void 玩家升级处理()
        {
            this.CallDefaultNPC(DefaultNPCType.LevelUp, true);
            base.发送封包(new 角色等级提升
            {
                对象编号 = this.地图编号,
                对象等级 = this.当前等级
            });
            this.所属行会?.发送封包(new 同步会员信息
            {
                对象编号 = this.地图编号,
                对象信息 = this.当前地图.地图编号,
                当前等级 = this.当前等级
            });
            this.战力加成[this] = 角色成长.等级战力(this.当前等级);
            this.更新玩家战力();
            base.属性加成[this] = 角色成长.获取数据(this.角色职业, this.当前等级);
            this.更新对象属性();
            if (!this.对象死亡)
            {
                this.当前体力 = this[游戏对象属性.最大体力];
                this.当前魔力 = this[游戏对象属性.最大魔力];
            }
            this.所属师门?.发送封包(new 同步师徒等级
            {
                对象编号 = this.地图编号,
                对象等级 = this.当前等级
            });
            if (this.所属师门 != null && this.所属队伍 != null && this.所属师门.师父数据 != this.角色数据 && this.所属队伍.队伍成员.Contains(this.所属师门.师父数据))
            {
                this.所属师门.徒弟经验[this.角色数据] += (int)((float)角色成长.升级所需经验[this.当前等级] * 0.05f);
                this.所属师门.师父经验[this.角色数据] += (int)((float)角色成长.升级所需经验[this.当前等级] * 0.05f);
                if (this.本期特权 != 0)
                {
                    this.所属师门.徒弟金币[this.角色数据] += (int)((float)角色成长.升级所需经验[this.当前等级] * 0.01f);
                    this.所属师门.师父金币[this.角色数据] += (int)((float)角色成长.升级所需经验[this.当前等级] * 0.02f);
                    this.所属师门.师父声望[this.角色数据] += (int)((float)角色成长.升级所需经验[this.当前等级] * 0.03f);
                }
            }
            if (this.当前等级 == 30 && this.所属师门 == null)
            {
                this.网络连接?.发送封包(new 同步师门信息
                {
                    师门参数 = this.师门参数
                });
            }
            if (this.当前等级 >= 36 && this.所属师门 != null && this.所属师门.师父编号 != this.地图编号)
            {
                this.提交出师申请();
            }
        }

        public bool 玩家切换地图(ushort 地图编号, int 坐标X, int 坐标Y, byte 指定区域)
        {
            地图实例 地图实例2;
            地图实例2 = 地图处理网关.已分配地图(地图编号);
            if (地图实例2 != null)
            {
                this.玩家切换地图(地图实例2, (地图区域类型)指定区域, new Point(坐标X, 坐标Y));
                return true;
            }
            return false;
        }

        public void 玩家切换地图(地图实例 跳转地图, 地图区域类型 指定区域, int x, int y)
        {
            this.玩家切换地图(跳转地图, 指定区域, new Point(x, y));
        }

        public void 玩家切换地图(地图实例 跳转地图, 地图区域类型 指定区域, Point 坐标 = default(Point))
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            base.清空邻居时处理();
            base.解绑网格();
            base.硬直时间 = DateTime.MinValue;
            base.忙碌时间 = DateTime.MinValue;
            this.网络连接?.发送封包(new 玩家离开场景());
            this.当前坐标 = ((指定区域 == 地图区域类型.未知区域) ? 坐标 : 跳转地图.随机坐标(指定区域));
            if (this.当前地图.地图编号 != 跳转地图.地图编号)
            {
                this.随机次数 = 0;
                this.所属队伍?.放弃所有拍卖(this.角色数据);
                foreach (Buff数据 item in this.Buff列表.Values.ToList())
                {
                    if (item.换图消失 && (item.Buff来源 == null || item.Buff来源.地图编号 == this.地图编号))
                    {
                        base.删除Buff时处理(item.Buff编号.V);
                    }
                }
                bool 副本地图;
                副本地图 = this.当前地图.副本地图;
                游戏脚本.地图退出(this.当前地图, this);
                this.当前地图 = 跳转地图;
                this.网络连接?.发送封包(new 玩家切换地图
                {
                    地图编号 = this.当前地图.地图编号,
                    路线编号 = this.当前地图.路线编号,
                    对象坐标 = this.当前坐标,
                    对象高度 = this.当前高度,
                    当前方向 = (ushort)this.当前方向
                });
                游戏脚本.地图进入(this.当前地图, this);
                if (!副本地图)
                {
                    return;
                }
                {
                    foreach (宠物实例 item2 in this.宠物列表)
                    {
                        item2.宠物召回处理();
                    }
                    return;
                }
            }
            this.网络连接?.发送封包(new 对象角色停止
            {
                对象编号 = this.地图编号,
                对象坐标 = this.当前坐标,
                对象高度 = this.当前高度
            });
            this.网络连接?.发送封包(new 玩家进入场景
            {
                地图编号 = this.当前地图.地图编号,
                当前坐标 = this.当前坐标,
                当前高度 = this.当前高度,
                路线编号 = this.当前地图.路线编号,
                路线状态 = this.当前地图.地图状态
            });
            this.所属队伍?.发送封包(new 同步队员信息
            {
                队伍编号 = this.所属队伍.队伍编号,
                对象编号 = this.地图编号,
                对象等级 = this.当前等级,
                最大体力 = this[游戏对象属性.最大体力],
                最大魔力 = this[游戏对象属性.最大魔力],
                当前体力 = this.当前体力,
                当前魔力 = this.当前魔力,
                当前地图 = this.当前地图.地图编号,
                当前线路 = this.当前地图.路线编号,
                横向坐标 = 计算类.点阵坐标转协议坐标(this.当前坐标.X),
                纵向坐标 = 计算类.点阵坐标转协议坐标(this.当前坐标.Y),
                坐标高度 = this.当前高度,
                攻击模式 = (byte)this.攻击模式
            }, this.角色数据);
            base.绑定网格();
            base.更新邻居时处理();
        }

        public 地图实例 查找我的副本(int 地图编号)
        {
            foreach (地图实例 item in 地图处理网关.副本实例表)
            {
                if (item.副本主人 == this.地图编号 && item.地图编号 == 地图编号)
                {
                    return item;
                }
            }
            return null;
        }

        public void 玩家增加经验(怪物实例 怪物, int 经验增加)
        {
            int num;
            num = 经验增加;
            int num2;
            num2 = 0;
            if (怪物 != null)
            {
                num = (int)((decimal)(int)Math.Max(0.0, (double)num - Math.Round((float)num * 计算类.收益衰减(this.当前等级, 怪物.当前等级))) * Settings.怪物经验倍率);
                if (this.当前等级 <= Settings.新手扶持等级)
                {
                    num *= 2;
                }
                num2 = Math.Min(this.双倍经验, num);
                if (this.自动战斗 && this.开启收益检测)
                {
                    this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                }
                if (this.当前地图.地图编号 == 67 && this.Buff列表.ContainsKey(2546))
                {
                    num *= 2;
                }
            }
            int num3;
            num3 = num + num2;
            this.双倍经验 -= num2;
            long num4;
            num4 = 0L;
            if (this.开启觉醒经验存储)
            {
                num4 = num3 / 2;
                long num5;
                num5 = this.觉醒存储上限 - this.当前觉醒经验;
                if (num5 < num4)
                {
                    num4 -= num5;
                }
                num4 = ((this.当前觉醒经验 + num4 > 2147483647L) ? (2147483647L - this.当前觉醒经验) : num4);
                this.当前觉醒经验 += num4;
            }
            if (num3 <= 0)
            {
                return;
            }
            if ((this.当前经验 += num3) >= this.所需经验 && this.当前等级 < Settings.游戏开放等级)
            {
                this.当前经验 -= this.所需经验;
                this.当前等级++;
                this.玩家升级处理();
            }
            if (this.当前经验 > this.所需经验 && this.当前等级 >= Settings.游戏开放等级 && !Settings.达最高级后继续加经验)
            {
                this.当前经验 = this.所需经验;
            }
            this.网络连接?.发送封包(new 角色经验变动
            {
                经验增加 = num3,
                今日增加 = 0,
                经验上限 = 10000000,
                双倍经验 = num2,
                当前经验 = this.当前经验,
                升级所需 = this.所需经验,
                增加的觉醒之力经验 = (int)num4
            });
            if (怪物 == null || this.角色数据.挂载物品.V == null)
            {
                return;
            }
            物品数据 v;
            v = this.角色数据.挂载物品.V;
            if (v.当前持久.V < v.最大持久.V)
            {
                v.当前持久.V = Math.Min(v.最大持久.V, v.当前持久.V + num3);
                this.网络连接?.发送封包(new 装备持久改变
                {
                    装备容器 = v.物品容器.V,
                    装备位置 = v.物品位置.V,
                    当前持久 = v.当前持久.V
                });
                if (v.当前持久.V == v.最大持久.V)
                {
                    this.发送系统消息("您的[" + v.物品名字 + "]累计经验已满,可以释放经验");
                }
            }
        }

        public void 修改技能等级(ushort 技能编号, byte 技能等级)
        {
            if (this.主体技能表.TryGetValue(技能编号, out var v))
            {
                v.技能等级.V = 技能等级;
                base.发送封包(new 玩家技能升级
                {
                    技能编号 = v.技能编号.V,
                    技能等级 = v.技能等级.V
                });
                this.战力加成[v] = v.战力加成;
                this.更新玩家战力();
                base.属性加成[v] = v.属性加成;
                this.更新对象属性();
                this.网络连接?.发送封包(new 同步技能等级
                {
                    技能编号 = v.技能编号.V,
                    当前经验 = v.技能经验.V,
                    当前等级 = v.技能等级.V
                });
            }
        }

        public void 技能增加经验(ushort 技能编号)
        {
            if (!this.主体技能表.TryGetValue(技能编号, out var v) || this.当前等级 < v.升级等级)
            {
                return;
            }
            int num;
            num = 主程.随机数.Next(4);
            if (num <= 0)
            {
                return;
            }
            if (this.角色装备.TryGetValue(8, out var v2) && v2.装备名字 == "技巧项链")
            {
                num = (int)((float)num * Math.Max(1f, Settings.技巧项链倍数));
            }
            if ((v.技能经验.V += (ushort)num) >= v.升级经验)
            {
                v.技能经验.V -= (ushort)v.升级经验;
                v.技能等级.V++;
                if (v.升级等级 == byte.MaxValue && v.升级经验 == 0)
                {
                    v.技能经验.V = 0;
                }
                base.发送封包(new 玩家技能升级
                {
                    技能编号 = v.技能编号.V,
                    技能等级 = v.技能等级.V
                });
                this.战力加成[v] = v.战力加成;
                this.更新玩家战力();
                base.属性加成[v] = v.属性加成;
                this.更新对象属性();
            }
            this.网络连接?.发送封包(new 同步技能等级
            {
                技能编号 = v.技能编号.V,
                当前经验 = v.技能经验.V,
                当前等级 = v.技能等级.V
            });
        }

        public bool 玩家移除技能(ushort 技能编号)
        {
            if (!this.主体技能表.ContainsKey(技能编号))
            {
                return false;
            }
            foreach (ushort item in this.主体技能表[技能编号].被动技能)
            {
                this.被动技能.Remove(item);
            }
            foreach (ushort item2 in this.主体技能表[技能编号].技能Buff)
            {
                base.移除Buff时处理(item2);
            }
            this.战力加成.Remove(this.主体技能表[技能编号]);
            this.更新玩家战力();
            base.属性加成.Remove(this.主体技能表[技能编号]);
            this.更新对象属性();
            this.主体技能表.Remove(技能编号);
            KeyValuePair<byte, 技能数据>[] array;
            array = this.快捷栏位.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                KeyValuePair<byte, 技能数据> keyValuePair;
                keyValuePair = array[i];
                if (keyValuePair.Value.技能编号.V == 技能编号)
                {
                    this.快捷栏位.Remove(keyValuePair.Key);
                }
            }
            this.网络连接?.发送封包(new 角色移除技能
            {
                技能编号 = 技能编号
            });
            return true;
        }

        public bool 玩家学习技能(ushort 技能编号, byte 技能等级 = 0)
        {
            if (this.主体技能表.ContainsKey(技能编号))
            {
                return false;
            }
            switch (技能编号)
            {
                case 1204:
                    this.玩家移除技能(1219);
                    break;
                case 1036:
                    this.玩家移除技能(1043);
                    base.移除Buff时处理(10430);
                    break;
                case 1034:
                    this.玩家移除技能(1051);
                    base.移除Buff时处理(10510);
                    break;
                case 1541:
                    this.玩家移除技能(1549);
                    break;
                case 1538:
                    this.玩家移除技能(1540);
                    base.移除Buff时处理(15380);
                    break;
                case 1208:
                    this.玩家移除技能(1215);
                    base.移除Buff时处理(12150);
                    break;
                case 2531:
                    this.玩家移除技能(2560);
                    break;
                case 2048:
                    this.玩家移除技能(2055);
                    break;
                case 2041:
                    this.玩家移除技能(2059);
                    break;
                case 3008:
                    this.玩家移除技能(3019);
                    break;
                case 3005:
                    this.玩家移除技能(3028);
                    break;
                case 2537:
                    this.玩家移除技能(2551);
                    break;
            }
            this.主体技能表[技能编号] = new 技能数据(技能编号, 技能等级);
            this.网络连接?.发送封包(new 角色学习技能
            {
                角色编号 = this.地图编号,
                技能编号 = 技能编号,
                技能等级 = 技能等级
            });
            if (this.主体技能表[技能编号].自动装配)
            {
                byte b;
                b = 0;
                while (b < 8)
                {
                    if (this.角色数据.快捷栏位.ContainsKey(b))
                    {
                        b++;
                        continue;
                    }
                    this.角色数据.快捷栏位[b] = this.主体技能表[技能编号];
                    this.网络连接?.发送封包(new 角色拖动技能
                    {
                        技能栏位 = b,
                        铭文编号 = this.主体技能表[技能编号].铭文编号,
                        技能编号 = this.主体技能表[技能编号].技能编号.V,
                        技能等级 = this.主体技能表[技能编号].技能等级.V
                    });
                    break;
                }
            }
            foreach (KeyValuePair<byte, 装备数据> item in this.角色装备)
            {
                装备数据 value;
                value = item.Value;
                if (value.第一铭文?.技能编号 == 技能编号)
                {
                    this.主体技能表[技能编号].铭文编号 = value.第一铭文.铭文编号;
                    this.网络连接?.发送封包(new 角色装配铭文
                    {
                        技能编号 = 技能编号,
                        铭文编号 = value.第一铭文.铭文编号
                    });
                }
                if (value.第二铭文?.技能编号 == 技能编号)
                {
                    this.主体技能表[技能编号].铭文编号 = value.第二铭文.铭文编号;
                    this.网络连接?.发送封包(new 角色装配铭文
                    {
                        技能编号 = 技能编号,
                        铭文编号 = value.第二铭文.铭文编号
                    });
                }
            }
            foreach (ushort item2 in this.主体技能表[技能编号].被动技能)
            {
                this.被动技能.Add(item2, this.主体技能表[技能编号]);
            }
            foreach (ushort item3 in this.主体技能表[技能编号].技能Buff)
            {
                base.添加Buff时处理(item3, this);
            }
            this.战力加成[this.主体技能表[技能编号]] = this.主体技能表[技能编号].战力加成;
            this.更新玩家战力();
            base.属性加成[this.主体技能表[技能编号]] = this.主体技能表[技能编号].属性加成;
            this.更新对象属性();
            if (this.开启七天乐)
            {
                this.修改七天进度(39, this.角色数据.七天进度[39] + 1);
                this.修改七天进度(49, this.角色数据.七天进度[49] + 1);
            }
            this.默认技能位置(this.主体技能表[技能编号]);
            return true;
        }

        public void 玩家装卸铭文(ushort 技能编号, byte 铭文编号)
        {
            if (!this.主体技能表.TryGetValue(技能编号, out var v) || v.铭文编号 == 铭文编号)
            {
                return;
            }
            foreach (ushort item in v.被动技能)
            {
                this.被动技能.Remove(item);
            }
            foreach (ushort item2 in v.技能Buff)
            {
                if (this.Buff列表.ContainsKey(item2))
                {
                    base.删除Buff时处理(item2);
                }
            }
            foreach (Buff数据 item3 in this.Buff列表.Values.ToList())
            {
                if (item3.绑定武器 && (item3.Buff来源 == null || item3.Buff来源.地图编号 == this.地图编号))
                {
                    base.删除Buff时处理(item3.Buff编号.V);
                }
            }
            foreach (宠物实例 item4 in this.宠物列表.ToList())
            {
                if (item4.绑定武器)
                {
                    item4.自身死亡处理(null, 技能击杀: false);
                }
            }
            v.铭文编号 = 铭文编号;
            this.网络连接?.发送封包(new 角色装配铭文
            {
                铭文编号 = 铭文编号,
                技能编号 = 技能编号,
                技能等级 = v.技能等级.V
            });
            foreach (ushort item5 in v.被动技能)
            {
                this.被动技能.Add(item5, v);
            }
            foreach (ushort item6 in v.技能Buff)
            {
                base.添加Buff时处理(item6, this);
            }
            if (this.冷却记录.TryGetValue(技能编号 | 0x1000000, out var v2) && 主程.当前时间 < v2)
            {
                this.网络连接?.发送封包(new 添加技能冷却
                {
                    冷却编号 = (技能编号 | 0x1000000),
                    冷却时间 = (int)(v2 - 主程.当前时间).TotalMilliseconds
                });
            }
            if (v.技能计数 != 0)
            {
                v.剩余次数.V = 0;
                v.计数时间 = 主程.当前时间.AddMilliseconds((int)v.计数周期);
                this.冷却记录[技能编号 | 0x1000000] = 主程.当前时间.AddMilliseconds((int)v.计数周期);
                this.网络连接?.发送封包(new 同步技能计数
                {
                    技能编号 = v.技能编号.V,
                    技能计数 = v.剩余次数.V,
                    技能冷却 = v.计数周期
                });
            }
            this.战力加成[v] = v.战力加成;
            this.更新玩家战力();
            base.属性加成[v] = v.属性加成;
            this.更新对象属性();
            if (!Settings.开启任务系统)
            {
                return;
            }
            CharacterQuest[] inProgressQuests;
            inProgressQuests = this.角色数据.GetInProgressQuests();
            foreach (CharacterQuest characterQuest in inProgressQuests)
            {
                CharacterQuestMission[] missionsOfType;
                missionsOfType = characterQuest.GetMissionsOfType(QuestMissionType.RefineInscription);
                bool flag;
                flag = false;
                CharacterQuestMission[] array;
                array = missionsOfType;
                for (int j = 0; j < array.Length; j++)
                {
                    if (!(array[j].CompletedDate.V != DateTime.MinValue))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.UpdateQuestProgress(characterQuest);
                }
            }
        }

        public void 刷新防具BUFF()
        {
            this.刷新配饰BUFF();
            base.移除Buff时处理(2611);
            base.移除Buff时处理(2612);
            base.移除Buff时处理(2613);
            base.移除Buff时处理(2614);
            base.移除Buff时处理(26105);
            if (this.角色装备[1] != null && this.角色装备[3] != null && this.角色装备[6] != null && this.角色装备[7] != null)
            {
                int v;
                v = this.角色装备[1].升级次数.V;
                ushort num;
                num = 0;
                v = Math.Min(Math.Min(Math.Min(v, this.角色装备[3].升级次数.V), this.角色装备[6].升级次数.V), this.角色装备[7].升级次数.V);
                switch (v)
                {
                    case 5:
                        num = 26105;
                        break;
                    case 6:
                        num = 2611;
                        break;
                    case 7:
                        num = 2612;
                        break;
                    case 8:
                        num = 2613;
                        break;
                    case 9:
                        num = 2614;
                        break;
                }
                if (Settings.开启成就系统)
                {
                    this.成就变量赋值(AchievementVariables.ArmorLevelupPoint, v, UseMax: true);
                }
                if (num > 0)
                {
                    base.添加Buff时处理(num, this);
                }
            }
        }

        public void 刷新配饰BUFF()
        {
            base.移除Buff时处理(26101);
            base.移除Buff时处理(26102);
            base.移除Buff时处理(26103);
            base.移除Buff时处理(26104);
            base.移除Buff时处理(26106);
            if (this.角色装备[2] != null && this.角色装备[4] != null && this.角色装备[5] != null && this.角色装备[14] != null)
            {
                int v;
                v = this.角色装备[2].升级次数.V;
                ushort num;
                num = 0;
                v = Math.Min(Math.Min(Math.Min(v, this.角色装备[4].升级次数.V), this.角色装备[5].升级次数.V), this.角色装备[14].升级次数.V);
                switch (v)
                {
                    case 5:
                        num = 26106;
                        break;
                    case 6:
                        num = 26101;
                        break;
                    case 7:
                        num = 26102;
                        break;
                    case 8:
                        num = 26103;
                        break;
                    case 9:
                        num = 26104;
                        break;
                }
                if (Settings.开启成就系统)
                {
                    this.成就变量赋值(AchievementVariables.ArmorAccessoryLevelupPoint, v, UseMax: true);
                }
                if (num > 0)
                {
                    base.添加Buff时处理(num, this);
                }
            }
        }

        public void 玩家穿卸装备(装备穿戴部位 装备部位, 装备数据 原有装备, 装备数据 现有装备)
        {
            if (装备部位 == 装备穿戴部位.武器 || 装备部位 == 装备穿戴部位.衣服 || 装备部位 == 装备穿戴部位.披风)
            {
                base.发送封包(new 同步角色外形
                {
                    对象编号 = this.地图编号,
                    装备部位 = (byte)装备部位,
                    装备编号 = (现有装备?.物品编号 ?? 0),
                    升级次数 = (现有装备?.升级次数.V ?? 0)
                });
            }
            if (原有装备 != null)
            {
                if (原有装备.技能编号 != 0)
                {
                    this.玩家移除技能(原有装备.技能编号);
                }
                if (原有装备.物品类型 == 物品使用分类.武器)
                {
                    foreach (Buff数据 item in this.Buff列表.Values.ToList())
                    {
                        if (item.绑定武器)
                        {
                            base.删除Buff时处理(item.Buff编号.V);
                        }
                    }
                }
                if (原有装备.物品类型 == 物品使用分类.武器)
                {
                    foreach (宠物实例 item2 in this.宠物列表.ToList())
                    {
                        if (item2.绑定武器)
                        {
                            item2.自身死亡处理(null, 技能击杀: false);
                        }
                    }
                }
                if (原有装备.第一铭文 != null)
                {
                    this.玩家装卸铭文(原有装备.第一铭文.技能编号, 0);
                }
                if (原有装备.第二铭文 != null)
                {
                    this.玩家装卸铭文(原有装备.第二铭文.技能编号, 0);
                }
                this.战力加成.Remove(原有装备);
                base.属性加成.Remove(原有装备);
                this.删除装备套装(原有装备);
            }
            if (现有装备 != null)
            {
                if (现有装备.技能编号 != 0)
                {
                    this.玩家学习技能(现有装备.技能编号, 0);
                }
                if (现有装备.第一铭文 != null)
                {
                    if (this.刻印部位列表.TryGetValue((int)(装备部位 - 1), out var value))
                    {
                        if ((this.五零变量 & value) != 0)
                        {
                            this.玩家装卸铭文(现有装备.第一铭文.技能编号, 现有装备.第一铭文.铭文编号);
                        }
                    }
                    else
                    {
                        this.玩家装卸铭文(现有装备.第一铭文.技能编号, 现有装备.第一铭文.铭文编号);
                    }
                }
                if (现有装备.第二铭文 != null)
                {
                    if (this.刻印部位列表.TryGetValue((int)(装备部位 - 1), out var value2))
                    {
                        if ((this.五零变量 & value2) != 0)
                        {
                            this.玩家装卸铭文(现有装备.第二铭文.技能编号, 现有装备.第二铭文.铭文编号);
                        }
                    }
                    else
                    {
                        this.玩家装卸铭文(现有装备.第二铭文.技能编号, 现有装备.第二铭文.铭文编号);
                    }
                }
                this.战力加成[现有装备] = 现有装备.装备战力;
                if (现有装备.当前持久.V > 0)
                {
                    base.属性加成.Add(现有装备, 现有装备.装备属性);
                }
                this.增加装备套装(现有装备);
            }
            if (原有装备 != null || 现有装备 != null)
            {
                this.刷新龙卫激活状态();
                this.更新精炼阶段属性();
                this.更新玩家战力();
                this.更新对象属性();
                this.刷新防具BUFF();
            }
            if (计算类.是否为龙卫装备(装备部位))
            {
                base.发送封包(new 龙卫属性激活状态
                {
                    属性位置 = (byte)装备部位,
                    数据 = this.角色数据.获取龙卫激活封包数据((byte)装备部位)
                });
            }
        }

        public void 玩家诱惑目标(技能实例 技能, C_04_计算目标诱惑 参数, 地图对象 诱惑目标)
        {
            if (诱惑目标 == null || 诱惑目标.对象死亡 || this.当前等级 + 2 < 诱惑目标.当前等级 || (!(诱惑目标 is 怪物实例) && !(诱惑目标 is 宠物实例)) || (诱惑目标 is 宠物实例 && (技能.技能等级 < 3 || this == (诱惑目标 as 宠物实例).宠物主人)) || (参数.检查铭文技能 && (!this.主体技能表.TryGetValue((ushort)(参数.检查铭文编号 / 10), out var v) || v.铭文编号 != 参数.检查铭文编号 % 10)))
            {
                return;
            }
            bool num;
            num = 参数.特定诱惑列表?.Contains(诱惑目标.完整名字) ?? false;
            bool flag;
            flag = num;
            float num2;
            num2 = (num ? 参数.特定诱惑概率 : 0f);
            float num3;
            if ((num3 = ((诱惑目标 is 怪物实例) ? (诱惑目标 as 怪物实例).基础诱惑概率 : (诱惑目标 as 宠物实例).基础诱惑概率) + num2) <= 0f)
            {
                return;
            }
            int num4;
            num4 = ((参数.基础诱惑数量?.Length > 技能.技能等级) ? 参数.基础诱惑数量[技能.技能等级] : 0);
            int num5;
            num5 = ((参数.初始宠物等级?.Length > 技能.技能等级) ? 参数.初始宠物等级[技能.技能等级] : 0);
            byte 额外诱惑数量;
            额外诱惑数量 = 参数.额外诱惑数量;
            float 额外诱惑概率;
            额外诱惑概率 = 参数.额外诱惑概率;
            int 额外诱惑时长;
            额外诱惑时长 = 参数.额外诱惑时长;
            float num6;
            num6 = 0f;
            int num7;
            num7 = 0;
            int num8;
            num8 = 0;
            foreach (Buff数据 value in this.Buff列表.Values)
            {
                if ((value.Buff效果 & Buff效果类型.诱惑提升) != 0)
                {
                    num6 += value.Buff模板.诱惑概率增加;
                    num7 += value.Buff模板.诱惑时长增加;
                    num8 += value.Buff模板.诱惑等级增加;
                }
            }
            if (计算类.计算概率(num3 * (float)Math.Pow((this.当前等级 >= 诱惑目标.当前等级) ? 1.2 : 0.8, 计算类.数值限制(0, Math.Abs(诱惑目标.当前等级 - this.当前等级), 2)) * (1f + 额外诱惑概率 + num6)))
            {
                if (!诱惑目标.Buff列表.ContainsKey(参数.狂暴状态编号) && !this.Buff列表.ContainsKey(46020) && !this.Buff列表.ContainsKey(46010))
                {
                    诱惑目标.添加Buff时处理(参数.瘫痪状态编号, this);
                }
                else if (this.宠物列表.Count < num4 + 额外诱惑数量)
                {
                    int num9;
                    num9 = Math.Min(num5 + num8, 7);
                    int 宠物时长;
                    宠物时长 = Settings.怪物诱惑时长 + 额外诱惑时长 + num7;
                    bool 绑定武器;
                    绑定武器 = flag || num5 != 0 || 额外诱惑时长 != 0 || 额外诱惑概率 != 0f || this.宠物列表.Count >= num4;
                    宠物实例 宠物实例2;
                    宠物实例2 = ((诱惑目标 is 怪物实例 怪物实例2) ? new 宠物实例(this, 怪物实例2, (byte)Math.Max(怪物实例2.宠物等级, num9), 绑定武器, 宠物时长, 0, int.MaxValue) : new 宠物实例(this, (宠物实例)诱惑目标, (byte)num9, 绑定武器, 宠物时长, 0, int.MaxValue));
                    宠物实例2.叛变时间.AddMilliseconds(this.龙卫BUFF诱惑时间());
                    this.网络连接?.发送封包(new 同步宠物等级
                    {
                        宠物编号 = 宠物实例2.地图编号,
                        宠物等级 = 宠物实例2.宠物等级
                    });
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 9473,
                        第一参数 = (int)this.宠物模式
                    });
                    this.宠物数据.Add(宠物实例2.宠物数据);
                    this.宠物列表.Add(宠物实例2);
                    base.移除Buff时处理(46010);
                    base.移除Buff时处理(46020);
                }
            }
        }

        public void 玩家瞬间移动(技能实例 技能, C_07_计算目标瞬移 参数)
        {
            if (计算类.计算概率(参数.每级成功概率[技能.技能等级]) && !(this.当前地图.随机传送(this.当前坐标) == default(Point)))
            {
                this.玩家切换地图(this.复活地图, 地图区域类型.随机区域);
            }
            else
            {
                base.添加Buff时处理(参数.瞬移失败提示, this);
                base.添加Buff时处理(参数.失败添加Buff, this);
            }
            if (参数.增加技能经验)
            {
                this.技能增加经验(参数.经验技能编号);
            }
        }

        public void 武器损失持久()
        {
            if (this.角色装备.TryGetValue(0, out var v) && v.当前持久.V > 0 && v.当前持久.V > 0 && ((this.本期特权 != 5 && this.本期特权 != 7) || !v.能否修理) && (this.本期特权 != 4 || !计算类.计算概率(0.5f)))
            {
                if ((v.当前持久.V = Math.Max(0, v.当前持久.V - 主程.随机数.Next(1, 6))) <= 0 && base.属性加成.Remove(v))
                {
                    this.更新对象属性();
                }
                this.网络连接?.发送封包(new 装备持久改变
                {
                    装备容器 = v.物品容器.V,
                    装备位置 = v.物品位置.V,
                    当前持久 = v.当前持久.V
                });
            }
        }

        public void 武器幸运损失()
        {
            if (this.角色装备.TryGetValue(0, out var v) && v.幸运等级.V > -9 && 计算类.计算概率(0.1f))
            {
                if (v.祈祷次数.V > 0)
                {
                    v.祈祷次数.V--;
                    this.发送系统消息("<#T:990740>");
                }
                else
                {
                    v.幸运等级.V--;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = v.字节描述()
                    });
                }
            }
        }

        public void 战具损失持久(int 损失持久)
        {
            if (this.角色装备.TryGetValue(15, out var v))
            {
                if ((v.当前持久.V -= 损失持久) <= 0)
                {
                    this.网络连接?.发送封包(new 删除玩家物品
                    {
                        背包类型 = v.物品容器.V,
                        物品位置 = v.物品位置.V
                    });
                    this.角色装备.Remove(v.物品位置.V);
                    v.删除数据();
                    this.玩家穿卸装备(装备穿戴部位.战具, v, null);
                    主程.添加物品日志(this, "物品持久消失", v, 1);
                }
                else
                {
                    this.网络连接?.发送封包(new 装备持久改变
                    {
                        装备容器 = v.物品容器.V,
                        装备位置 = v.物品位置.V,
                        当前持久 = v.当前持久.V
                    });
                }
            }
        }

        public void 装备损失持久(int 损失持久)
        {
            损失持久 = Math.Min(10, 损失持久);
            if (this[游戏对象属性.装备免修] > 0)
            {
                return;
            }
            foreach (装备数据 value in this.角色装备.Values)
            {
                if (value.当前持久.V > 0 && ((this.本期特权 != 5 && this.本期特权 != 7) || !value.能否修理) && (this.本期特权 != 4 || !计算类.计算概率(0.5f)) && value.持久类型 == 物品持久分类.装备 && 计算类.计算概率((value.物品类型 == 物品使用分类.衣服) ? 1f : 0.1f))
                {
                    if ((value.当前持久.V = Math.Max(0, value.当前持久.V - 损失持久)) <= 0 && base.属性加成.Remove(value))
                    {
                        this.更新对象属性();
                    }
                    this.网络连接?.发送封包(new 装备持久改变
                    {
                        装备容器 = value.物品容器.V,
                        装备位置 = value.物品位置.V,
                        当前持久 = value.当前持久.V
                    });
                }
            }
        }

        public void 玩家特权到期()
        {
            if (this.本期特权 == 3)
            {
                this.玩家称号到期(61);
            }
            else if (this.本期特权 == 4)
            {
                this.玩家称号到期(124);
            }
            else if (this.本期特权 == 5)
            {
                this.玩家称号到期(131);
            }
            this.上期特权 = this.本期特权;
            this.上期记录 = this.本期记录;
            this.上期日期 = this.本期日期;
            this.本期特权 = 0;
            this.本期记录 = 0u;
            this.本期日期 = default(DateTime);
            this.特权时间 = DateTime.MaxValue;
        }

        public void 玩家激活特权(byte 特权类型, int 时间 = 30)
        {
            this.玩家称号到期(61);
            this.玩家称号到期(124);
            this.玩家称号到期(131);
            this.玩家称号到期(147);
            switch (特权类型)
            {
                default:
                    return;
                case 3:
                    this.角色数据.发送邮件(null, "尊敬的玩家您好", "您已成功激活玛法特权[玛法名俊]，享受250%爆率加成，每日签到奖励等福利，祝你游戏愉快！", -1);
                    break;
                case 4:
                    this.角色数据.发送邮件(null, "尊敬的玩家您好", "您已成功激活玛法特权[玛法豪杰]，享受300%爆率加成，每日签到奖励等福利，祝你游戏愉快！", -1);
                    break;
                case 5:
                    this.角色数据.发送邮件(null, "尊敬的玩家您好", "您已成功激活玛法特权[玛法战将]，享受300%爆率加成，装备免修理等福利，祝你游戏愉快！", 1410101);
                    break;
                case 6:
                    this.角色数据.发送邮件(null, "尊敬的玩家您好", "您已成功激活玛法特权[玛法新秀]，享受175%爆率加成，装备免修理等福利，祝你游戏愉快！", -1);
                    break;
                case 7:
                    this.角色数据.发送邮件(null, "尊敬的玩家您好", "您已成功激活玛法特权[玛法至尊]，享受350%爆率加成，装备免修理等福利，祝你游戏愉快！", -1);
                    break;
            }
            this.本期特权 = 特权类型;
            this.本期记录 = uint.MaxValue;
            this.本期日期 = 主程.当前时间;
            this.特权时间 = this.本期日期.AddDays(时间);
            this.计算玩家额外暴率();
            this.网络连接?.发送封包(new 传永武技签到
            {
                签到次数 = this.传永武技,
                能否签到 = ((this.本期特权 == 4 || this.本期特权 == 5 || this.本期特权 == 7) && (主程.当前时间.Date.AddDays(1.0) - this.武技日期.Date).TotalDays > 0.0)
            });
            if (this.开启七天乐)
            {
                this.修改七天进度(64, this.角色数据.七天进度[64] + 1);
            }
        }

        public void 玩家称号到期(byte 称号编号)
        {
            if (this.称号列表.Remove(称号编号))
            {
                游戏称号 value;
                if (this.当前称号 == 称号编号)
                {
                    this.当前称号 = 0;
                    this.战力加成.Remove(称号编号);
                    this.更新玩家战力();
                    base.属性加成.Remove(称号编号);
                    this.更新对象属性();
                    base.发送封包(new 同步装配称号
                    {
                        对象编号 = this.地图编号
                    });
                }
                else if (游戏称号.数据表.TryGetValue(称号编号, out value) && value.始终生效)
                {
                    this.战力加成.Remove(称号编号);
                    this.更新玩家战力();
                    base.属性加成.Remove(称号编号);
                    this.更新对象属性();
                }
                this.网络连接?.发送封包(new 玩家失去称号
                {
                    称号编号 = 称号编号
                });
            }
        }

        public bool 玩家获得称号(byte 称号编号, int 时长 = 0)
        {
            if (游戏称号.数据表.TryGetValue(称号编号, out var value))
            {
                this.称号列表[称号编号] = 主程.当前时间.AddMinutes((时长 != 0) ? 时长 : value.有效时间);
                if (value.始终生效)
                {
                    this.战力加成[称号编号] = value.称号战力;
                    this.更新玩家战力();
                    base.属性加成[称号编号] = value.称号属性;
                    this.更新对象属性();
                }
                this.网络连接?.发送封包(new 玩家获得称号
                {
                    称号编号 = 称号编号,
                    剩余时间 = (int)(this.称号列表[称号编号] - 主程.当前时间).TotalMinutes
                });
                return true;
            }
            return false;
        }

        public void 玩家获得仇恨(地图对象 对象)
        {
            foreach (宠物实例 item in this.宠物列表.ToList())
            {
                if (item.邻居列表.Contains(对象) && !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                {
                    item.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                }
            }
        }

        public int 统计物品数量(byte 背包类型, string 物品名称)
        {
            if (游戏物品.检索表.TryGetValue(物品名称, out var value))
            {
                return this.统计物品数量(背包类型, value.物品编号);
            }
            if (游戏物品.检索表.TryGetValue(物品名称, out var value2))
            {
                return this.统计物品数量(背包类型, value2.物品编号);
            }
            return 0;
        }

        public int 统计物品数量(byte 背包类型, int 物品编号)
        {
            int num;
            num = 0;
            int num2;
            num2 = 0;
            字典监视器<byte, 物品数据> 字典监视器;
            字典监视器 = null;
            switch (背包类型)
            {
                default:
                    return 0;
                case 7:
                    num2 = this.资源包大小;
                    字典监视器 = this.角色资源包;
                    break;
                case 2:
                    num2 = this.仓库大小;
                    字典监视器 = this.角色仓库;
                    break;
                case 1:
                    num2 = this.背包大小;
                    字典监视器 = this.角色背包;
                    break;
            }
            if (num2 != 0 && 字典监视器 != null)
            {
                for (byte b = 0; b < num2; b++)
                {
                    if (字典监视器.TryGetValue(b, out var v) && v.物品编号 == 物品编号)
                    {
                        num = ((v is 装备数据 || v.持久类型 != 物品持久分类.堆叠) ? (num + 1) : (num + v.当前持久.V));
                    }
                }
                return num;
            }
            return 0;
        }

        public bool 查找背包物品(物品使用分类 物品分类, out List<物品数据> 物品列表)
        {
            物品列表 = new List<物品数据>();
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (this.角色背包.TryGetValue(b, out var v) && v.物品模板.物品分类 == 物品分类)
                {
                    物品列表.Add(v);
                }
            }
            return false;
        }

        public bool 查找背包物品(int 物品编号, out 物品数据 物品)
        {
            byte b;
            b = 0;
            while (b < this.背包大小)
            {
                if (!this.角色背包.TryGetValue(b, out 物品) || 物品.物品编号 != 物品编号)
                {
                    b++;
                    continue;
                }
                return true;
            }
            物品 = null;
            return false;
        }

        public bool 查找背包物品(int 所需总数, int 物品编号, out List<物品数据> 物品列表)
        {
            物品列表 = new List<物品数据>();
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (!this.角色背包.TryGetValue(b, out var v) || v.物品编号 != 物品编号)
                {
                    continue;
                }
                物品列表.Add(v);
                if (v is 装备数据)
                {
                    if (--所需总数 > 0)
                    {
                        continue;
                    }
                }
                else if ((所需总数 -= v.当前持久.V) > 0)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public List<物品数据> 查找背包物品(int 物品编号, int 所需总数)
        {
            List<物品数据> list;
            list = new List<物品数据>();
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (!this.角色背包.TryGetValue(b, out var v) || v.物品编号 != 物品编号)
                {
                    continue;
                }
                list.Add(v);
                if (v is 装备数据)
                {
                    if (--所需总数 > 0)
                    {
                        continue;
                    }
                }
                else if ((所需总数 -= v.当前持久.V) > 0)
                {
                    continue;
                }
                return list;
            }
            return null;
        }

        public List<物品数据> 查找背包物品(string 物品名字, int 所需总数, bool 全字匹配 = false)
        {
            List<物品数据> list;
            list = new List<物品数据>();
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (!this.角色背包.TryGetValue(b, out var v) || !(全字匹配 ? (v.物品名字 == 物品名字) : v.物品名字.Contains(物品名字)))
                {
                    continue;
                }
                list.Add(v);
                if (!(v is 装备数据) && v.持久类型 == 物品持久分类.堆叠)
                {
                    if ((所需总数 -= v.当前持久.V) > 0)
                    {
                        continue;
                    }
                }
                else if (--所需总数 > 0)
                {
                    continue;
                }
                return list;
            }
            return null;
        }

        public bool 查找背包物品(int 所需总数, HashSet<int> 物品编号, out List<物品数据> 物品列表)
        {
            物品列表 = new List<物品数据>();
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (!this.角色背包.TryGetValue(b, out var v) || !物品编号.Contains(v.物品编号))
                {
                    continue;
                }
                物品列表.Add(v);
                if (v is 装备数据)
                {
                    if (--所需总数 > 0)
                    {
                        continue;
                    }
                }
                else if ((所需总数 -= v.当前持久.V) > 0)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public bool FindItem(string name, int count)
        {
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (!this.角色背包.TryGetValue(b, out var v) || !(v.物品名字 == name))
                {
                    continue;
                }
                if (v.持久类型 == 物品持久分类.堆叠)
                {
                    if ((count -= v.当前持久.V) > 0)
                    {
                        continue;
                    }
                }
                else if (--count > 0)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public bool FindItem(int idx, int count, byte Luck)
        {
            for (byte b = 0; b < this.背包大小; b++)
            {
                if (!this.角色背包.TryGetValue(b, out var v) || v.物品编号 != idx || (Luck != 0 && this.GetItemValue(v, 9) < Luck))
                {
                    continue;
                }
                if (v.持久类型 == 物品持久分类.堆叠)
                {
                    if ((count -= v.当前持久.V) > 0)
                    {
                        continue;
                    }
                }
                else if (--count > 0)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void 消耗背包物品(int 消耗总数, 物品数据 当前物品, string 日志 = "")
        {
            if ((当前物品.当前持久.V -= 消耗总数) <= 0)
            {
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 当前物品.物品容器.V,
                    物品位置 = 当前物品.物品位置.V
                });
                this.角色背包.Remove(当前物品.物品位置.V);
                当前物品.删除数据();
            }
            else
            {
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 当前物品.字节描述()
                });
            }
            主程.添加物品日志(this, 日志, 当前物品, (当前物品 is 装备数据) ? 1 : 消耗总数);
        }

        public void 消耗背包物品(int 消耗总数, List<物品数据> 物品列表, string 日志 = "")
        {
            物品列表.OrderBy((物品数据 O) => O.物品位置);
            foreach (物品数据 item in 物品列表)
            {
                if (item is 装备数据 装备数据)
                {
                    this.消耗背包物品(装备数据.当前持久.V, 装备数据, 日志);
                    continue;
                }
                int num;
                num = Math.Min(消耗总数, item.当前持久.V);
                this.消耗背包物品(num, item, 日志);
                if ((消耗总数 -= num) <= 0)
                {
                    break;
                }
            }
        }

        public void 拿走背包物品(int 消耗总数, List<物品数据> 物品列表, string 日志 = "")
        {
            物品列表.OrderBy((物品数据 O) => O.物品位置);
            foreach (物品数据 item in 物品列表)
            {
                if (item is 装备数据 装备数据)
                {
                    this.拿走背包物品(装备数据.当前持久.V, 装备数据, 日志);
                    continue;
                }
                int num;
                num = ((item.持久类型 != 物品持久分类.堆叠) ? 1 : Math.Min(消耗总数, item.当前持久.V));
                this.拿走背包物品(num, item, 日志);
                if ((消耗总数 -= num) <= 0)
                {
                    break;
                }
            }
        }

        public void 拿走背包物品(int 消耗总数, 物品数据 当前物品, string 日志 = "")
        {
            if (当前物品.持久类型 != 物品持久分类.堆叠)
            {
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 当前物品.物品容器.V,
                    物品位置 = 当前物品.物品位置.V
                });
                this.角色背包.Remove(当前物品.物品位置.V);
                当前物品.删除数据();
            }
            else if ((当前物品.当前持久.V -= 消耗总数) <= 0)
            {
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 当前物品.物品容器.V,
                    物品位置 = 当前物品.物品位置.V
                });
                this.角色背包.Remove(当前物品.物品位置.V);
                当前物品.删除数据();
            }
            else
            {
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 当前物品.字节描述()
                });
            }
            主程.添加物品日志(this, 日志, 当前物品, (当前物品 is 装备数据) ? 1 : 消耗总数);
        }

        public void 玩家角色下线()
        {
            this.当前交易?.结束交易();
            this.所属队伍?.发送封包(new 同步队员状态
            {
                对象编号 = this.地图编号,
                状态编号 = 1
            }, this.角色数据);
            this.所属行会?.发送封包(new 同步会员信息
            {
                对象编号 = this.地图编号,
                对象信息 = 计算类.时间转换(主程.当前时间)
            });
            foreach (角色数据 item in this.粉丝列表)
            {
                item.网络连接?.发送封包(new 好友上线下线
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字,
                    对象职业 = (byte)this.角色职业,
                    对象性别 = (byte)this.角色性别,
                    上线下线 = 3
                });
            }
            foreach (角色数据 item2 in this.仇恨列表)
            {
                item2.网络连接?.发送封包(new 好友上线下线
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字,
                    对象职业 = (byte)this.角色职业,
                    对象性别 = (byte)this.角色性别,
                    上线下线 = 3
                });
            }
            foreach (宠物实例 item3 in this.宠物列表.ToList())
            {
                if (Settings.下线宝宝不死)
                {
                    item3.宠物沉睡处理();
                }
                else
                {
                    item3.自身死亡处理(null, 技能击杀: false);
                }


            }
            foreach (Buff数据 item4 in this.Buff列表.Values.ToList())
            {
                if (item4.下线消失)
                {
                    base.删除Buff时处理(item4.Buff编号.V, 后接BUFF: false);
                }
            }
            this.角色数据.角色下线();
            base.删除对象();
            this.当前地图.玩家列表.Remove(this);
            if (!this.是否假人)
            {
                //主程.WebLog(LogDataType.OfflineLog, Settings.统计UUID代码, this.角色数据.所属账号.V.账号名字.V, this.会话ID);
            }
        }

        public void 玩家进入场景()
        {
            this.UpdateAchievementProgress(sendMsg: true);
            this.网络连接?.发送封包(new 对象角色停止
            {
                对象编号 = this.地图编号,
                对象坐标 = this.当前坐标,
                对象高度 = this.当前高度
            });
            this.网络连接?.发送封包(new 玩家进入场景
            {
                地图编号 = this.当前地图.地图编号,
                当前坐标 = this.当前坐标,
                当前高度 = this.当前高度,
                路线编号 = this.当前地图.路线编号,
                路线状态 = this.当前地图.地图状态
            });
            this.网络连接?.发送封包(new 对象进入视野
            {
                出现方式 = 1,
                对象编号 = this.地图编号,
                现身坐标 = this.当前坐标,
                现身高度 = this.当前高度,
                现身方向 = (ushort)this.当前方向,
                现身姿态 = (byte)((!this.对象死亡) ? 1u : 13u),
                体力比例 = (byte)(this.当前体力 * 100 / this[游戏对象属性.最大体力]),
                坐骑编号 = (byte)(this.Buff列表.ContainsKey(2555) ? this.当前坐骑 : 0),
                传承之力 = this.传承之力外观
            });
            this.网络连接?.发送封包(new 同步对象体力
            {
                对象编号 = this.地图编号,
                当前体力 = this.当前体力,
                体力上限 = this[游戏对象属性.最大体力]
            });
            this.网络连接?.发送封包(new 同步对象魔力
            {
                当前魔力 = this.当前魔力
            });
            this.网络连接?.发送封包(new 同步元宝数量
            {
                元宝数量 = this.元宝数量
            });
            this.网络连接?.发送封包(new 同步冷却列表
            {
                字节描述 = this.全部冷却描述()
            });
            this.网络连接?.发送封包(new 同步节点数据
            {
                数据 = this.获取节点数据()
            });
            this.网络连接?.发送封包(new 同步状态列表
            {
                字节数据 = this.全部Buff描述()
            });
            this.网络连接?.发送封包(new 切换战斗姿态
            {
                对象编号 = this.地图编号
            });
            if (!Settings.屏蔽日程)
            {
                this.发送日程详情();
            }
            foreach (技能数据 item in this.主体技能表.Values.ToList())
            {
                if (item.技能计数 > 0)
                {
                    this.网络连接?.发送封包(new 同步技能计数
                    {
                        技能编号 = item.技能编号.V,
                        技能计数 = item.剩余次数.V,
                        技能冷却 = item.计数周期
                    });
                }
            }
            base.绑定网格();
            base.更新邻居时处理();
            if (游戏技能.数据表.TryGetValue("通用-玩家取出武器", out var value))
            {
                new 技能实例(this, value, null, base.动作编号, this.当前地图, this.当前坐标, null, this.当前坐标, null);
            }
            if (this.宠物列表.Count == this.宠物数据.Count)
            {
                return;
            }
            foreach (宠物数据 item2 in this.宠物数据.ToList())
            {
                if ((!(主程.当前时间 >= item2.叛变时间.V) || !(主程.当前时间 >= item2.存活时间.V)) && 游戏怪物.数据表.ContainsKey(item2.宠物名字.V))
                {
                    宠物实例 宠物实例2;
                    宠物实例2 = new 宠物实例(this, item2);
                    this.宠物列表.Add(宠物实例2);
                    this.网络连接?.发送封包(new 同步宠物等级
                    {
                        宠物编号 = 宠物实例2.地图编号,
                        宠物等级 = 宠物实例2.宠物等级
                    });
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 9473,
                        第一参数 = (int)this.宠物模式
                    });
                }
                else
                {
                    item2.删除数据();
                    this.宠物数据.Remove(item2);
                }
            }
        }

        public void 玩家退出副本()
        {
            if (this.对象死亡)
            {
                this.玩家请求复活();
            }
            else
            {
                this.玩家切换地图(地图处理网关.已分配地图(this.重生地图), 地图区域类型.复活区域);
            }
        }

        public void 玩家请求复活()
        {
            if (!this.对象死亡)
            {
                return;
            }
            this.网络连接?.发送封包(new 玩家角色复活
            {
                对象编号 = this.地图编号,
                复活方式 = 3
            });
            this.当前体力 = (int)((float)this[游戏对象属性.最大体力] * 0.3f);
            this.当前魔力 = (int)((float)this[游戏对象属性.最大魔力] * 0.3f);
            this.对象死亡 = false;
            this.阻塞网格 = true;
            if (this.当前地图 == 地图处理网关.沙城地图 && 地图处理网关.沙城节点 >= 2)
            {
                if (this.所属行会 != null && this.所属行会 == 系统数据.数据.占领行会.V)
                {
                    this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, 地图处理网关.守方传送区域.随机坐标);
                }
                else if (this.所属行会 != null && this.所属行会 == 地图处理网关.八卦坛激活行会)
                {
                    this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, 地图处理网关.内城复活区域.随机坐标);
                }
                else
                {
                    this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, 地图处理网关.外城复活区域.随机坐标);
                }
            }
            else
            {
                this.玩家切换地图(this.复活地图, (!this.红名玩家) ? 地图区域类型.复活区域 : 地图区域类型.红名区域);
            }
        }

        public void 玩家原地复活(float 血量比例 = 1f, float 蓝量比例 = 1f)
        {
            if (this.对象死亡)
            {
                this.网络连接?.发送封包(new 玩家角色复活
                {
                    对象编号 = this.地图编号,
                    复活方式 = 3
                });
                this.当前体力 = (int)((float)this[游戏对象属性.最大体力] * 血量比例);
                this.当前魔力 = (int)((float)this[游戏对象属性.最大魔力] * 蓝量比例);
                this.对象死亡 = false;
                this.阻塞网格 = true;
            }
        }

        public void 玩家进入法阵(int 法阵编号)
        {
            if (!this.绑定地图)
            {
                return;
            }
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                游戏地图 value2;
                if (!this.当前地图.法阵列表.TryGetValue((byte)法阵编号, out var value))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 775
                    });
                }
                else if (base.网格距离(value.所处坐标) >= 8)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 4609
                    });
                }
                else if (!游戏地图.数据表.TryGetValue(value.跳转地图, out value2))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 775
                    });
                }
                else if (this.当前等级 < value2.限制等级)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 4624
                    });
                }
                else if (主程.开区节点 < value2.节点开放)
                {
                    this.发送顶部公告("当前节点暂未开放");
                }
                else
                {
                    this.玩家切换地图((this.当前地图.地图编号 == value2.地图编号) ? this.当前地图 : 地图处理网关.已分配地图(value2.地图编号), 地图区域类型.未知区域, value.跳转坐标);
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 769
                });
            }
        }

        public void 玩家角色走动(Point 终点坐标)
        {
            if (挖矿次数 != 0) //挖矿
            {
                挖矿次数 = 0;
                网络服务网关.发送信息(this, "停止挖矿");
            }
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.当前坐标 == 终点坐标)
            {
                this.网络连接?.发送封包(new 对象角色停止
                {
                    对象编号 = this.地图编号,
                    对象坐标 = this.当前坐标,
                    对象高度 = this.当前高度
                });
                return;
            }
            if (!this.能否走动())
            {
                this.网络连接?.发送封包(new 对象角色停止
                {
                    对象编号 = this.地图编号,
                    对象坐标 = this.当前坐标,
                    对象高度 = this.当前高度
                });
                return;
            }
            Point point;
            point = 计算类.前方坐标(方向: 计算类.计算方向(this.当前坐标, 终点坐标), 原点: this.当前坐标, 步数: 1);
            游戏方向 游戏方向;
            if (!this.当前地图.能否通行(point))
            {
                if (this.当前方向 != (游戏方向 = 计算类.计算方向(this.当前坐标, point)))
                {
                    this.角色数据.当前朝向.V = 游戏方向;
                    base.发送封包(new 对象转动方向
                    {
                        对象编号 = this.地图编号,
                        对象朝向 = (ushort)游戏方向,
                        转向耗时 = 100
                    });
                }
                base.发送封包(new 对象角色停止
                {
                    对象编号 = this.地图编号,
                    对象坐标 = this.当前坐标,
                    对象高度 = this.当前高度
                });
                return;
            }
            this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
            this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
            if (this.当前方向 != (游戏方向 = 计算类.计算方向(this.当前坐标, point)))
            {
                this.角色数据.当前朝向.V = 游戏方向;
                base.发送封包(new 对象转动方向
                {
                    对象编号 = this.地图编号,
                    对象朝向 = (ushort)游戏方向,
                    转向耗时 = 100
                });
            }
            base.发送封包(new 对象角色走动
            {
                对象编号 = this.地图编号,
                移动坐标 = point,
                移动速度 = base.行走速度
            });
            base.自身移动时处理(point);
        }

        public void 玩家角色跑动(Point 终点坐标)
        {
            if (挖矿次数 != 0) //挖矿
            {
                挖矿次数 = 0;
                网络服务网关.发送信息(this, "停止挖矿");
            }
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.角色职业 == 游戏对象职业.刺客)
            {
                foreach (Buff数据 item in this.Buff列表.Values.ToList())
                {
                    if ((item.Buff效果 & Buff效果类型.状态标志) != 0 && (item.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) != 0)
                    {
                        base.移除Buff时处理(item.Buff编号.V);
                    }
                }
            }
            if (this.当前坐标 == 终点坐标)
            {
                this.网络连接?.发送封包(new 对象角色停止
                {
                    对象编号 = this.地图编号,
                    对象坐标 = this.当前坐标,
                    对象高度 = this.当前高度
                });
            }
            else if (this.能否跑动())
            {
                游戏方向 方向;
                方向 = 计算类.计算方向(this.当前坐标, 终点坐标);
                Point point;
                point = 计算类.前方坐标(this.当前坐标, 方向, 1);
                Point point2;
                point2 = 计算类.前方坐标(this.当前坐标, 方向, 2);
                if (!this.当前地图.能否通行(point))
                {
                    if (this.当前方向 != (方向 = 计算类.计算方向(this.当前坐标, point)))
                    {
                        this.角色数据.当前朝向.V = 方向;
                        base.发送封包(new 对象转动方向
                        {
                            对象编号 = this.地图编号,
                            对象朝向 = (ushort)方向,
                            转向耗时 = 100
                        });
                    }
                    base.发送封包(new 对象角色停止
                    {
                        对象编号 = this.地图编号,
                        对象坐标 = this.当前坐标,
                        对象高度 = this.当前高度
                    });
                    return;
                }
                if (!this.当前地图.能否通行(point2))
                {
                    this.玩家角色走动(终点坐标);
                    return;
                }
                this.奔跑时间 = 主程.当前时间.AddMilliseconds(this.奔跑耗时);
                this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.奔跑耗时);
                if (this.当前方向 != (方向 = 计算类.计算方向(this.当前坐标, point2)))
                {
                    this.角色数据.当前朝向.V = 方向;
                    base.发送封包(new 对象转动方向
                    {
                        对象编号 = this.地图编号,
                        对象朝向 = (ushort)方向,
                        转向耗时 = 100
                    });
                }
                base.发送封包(new 对象角色跑动
                {
                    对象编号 = this.地图编号,
                    移动坐标 = point2,
                    移动耗时 = base.奔跑速度
                });
                base.自身移动时处理(point2);
            }
            else if (this.能否走动())
            {
                this.玩家角色走动(终点坐标);
            }
            else
            {
                base.发送封包(new 对象角色停止
                {
                    对象编号 = this.地图编号,
                    对象坐标 = this.当前坐标,
                    对象高度 = this.当前高度
                });
            }
        }

        public void 玩家角色转动(游戏方向 转动方向)
        {
            if (挖矿次数 != 0) //挖矿
            {
                挖矿次数 = 0;
                网络服务网关.发送信息(this, "停止挖矿");
            }
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3 && this.能否转动())
            {
                this.当前方向 = 转动方向;
            }
        }

        public void 玩家切换姿态(byte 姿态编号, byte 触发动作)
        {
            if (姿态编号 == 1 && 触发动作 == 1)
            {
                if (游戏技能.数据表.TryGetValue("通用-玩家收起武器", out var value))
                {
                    new 技能实例(this, value, null, base.动作编号, this.当前地图, this.当前坐标, null, this.当前坐标, null);
                }
                base.发送封包(new 切换战斗姿态
                {
                    对象编号 = this.地图编号,
                    姿态编号 = 姿态编号,
                    触发动作 = 1
                });
            }
            else if (姿态编号 == 0 && 触发动作 == 1)
            {
                if (游戏技能.数据表.TryGetValue("通用-玩家取出武器", out var value2))
                {
                    new 技能实例(this, value2, null, base.动作编号, this.当前地图, this.当前坐标, null, this.当前坐标, null);
                }
                base.发送封包(new 切换战斗姿态
                {
                    对象编号 = this.地图编号,
                    姿态编号 = 姿态编号,
                    触发动作 = 0
                });
            }
            else
            {
                base.发送封包(new 切换战斗姿态
                {
                    对象编号 = this.地图编号,
                    姿态编号 = 姿态编号,
                    触发动作 = 0
                });
            }
        }

        public bool 检测技能耗蓝(ushort 技能编号)
        {
            if (!this.主体技能表.TryGetValue(技能编号, out var v) && !this.被动技能.TryGetValue(技能编号, out v))
            {
                return false;
            }

            foreach (string item in v.铭文模板.主体技能列表.ToList())
            {
                int num;
                num = 0;
                int num2;
                num2 = 0;
                List<物品数据> 物品列表;
                物品列表 = null;
                if (游戏技能.数据表.TryGetValue(item, out var value) && value.自身技能编号 == 技能编号)
                {
                    if (!value.检查职业武器 || (this.角色装备.TryGetValue(0, out var v2) && v2.需要职业 == this.角色职业))
                    {
                        if (this.主体技能表.TryGetValue(value.绑定等级编号, out var v3) && value.需要消耗魔法?.Length > v3.技能等级.V && this is 玩家实例 { 无敌模式: false })
                        {
                            num = ((value.绑定等级编号 != 2554 || !this.Buff列表.TryGetValue(25540, out var v4)) ? value.需要消耗魔法[v3.技能等级.V] : (value.需要消耗魔法[v3.技能等级.V] + value.需要消耗魔法[v3.技能等级.V] * v4.当前层数.V));
                            if (this.当前魔力 < num)
                            {
                                return false;
                            }
                        }
                        HashSet<int> 需要消耗物品;
                        需要消耗物品 = value.需要消耗物品;
                        if (需要消耗物品 != null && 需要消耗物品.Count != 0)
                        {
                            if (this.角色装备.TryGetValue(15, out var v5) && v5.当前持久.V >= value.战具扣除点数)
                            {
                                物品列表 = new List<物品数据> { v5 };
                                num2 = value.战具扣除点数;
                            }
                            else
                            {
                                if (!this.查找背包物品(value.消耗物品数量, value.需要消耗物品, out 物品列表))
                                {
                                    continue;
                                }
                                num2 = value.消耗物品数量;
                            }
                        }
                        if (num >= 0 && 计算类.计算概率(this.Buff列表.TryGetValue(26300, out var _) ? 0.75f : 1f))
                        {
                            this.当前魔力 -= num;
                        }
                        if (物品列表 != null && 物品列表.Count == 1 && 物品列表[0].物品类型 == 物品使用分类.战具)
                        {
                            this.战具损失持久(num2);
                        }
                        else if (物品列表 != null)
                        {
                            this.消耗背包物品(num2, 物品列表, "释放技能消耗");
                        }
                        continue;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }

        public void 玩家开关技能(ushort 技能编号)
        {
            if (this.管理员模式)
            {
                this.发送系统消息($"开关技能;{技能编号} {base.动作编号} {主程.当前时间}");
            }
            foreach (技能实例 item in base.技能任务)
            {
                if (item.动作打断)
                {
                    item.是否中断 = true;
                }
            }
            if (this.对象死亡)
            {
                return;
            }
            if (!this.主体技能表.TryGetValue(技能编号, out var v) && !this.被动技能.TryGetValue(技能编号, out v))
            {
                this.发送系统消息("此技能已被删除或您未学习此技能，请小退重新登陆后获得最新技能列表");
                return;
            }
            if (this.冷却记录.TryGetValue(技能编号 | 0x1000000, out var v2) && 主程.当前时间 < v2)
            {
                this.网络连接?.发送封包(new 添加技能冷却
                {
                    冷却编号 = (技能编号 | 0x1000000),
                    冷却时间 = (int)(v2 - 主程.当前时间).TotalMilliseconds
                });
                this.网络连接?.发送封包(new 技能释放完成
                {
                    技能编号 = 技能编号,
                    动作编号 = base.动作编号
                });
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1281,
                    第一参数 = 技能编号,
                    第二参数 = base.动作编号
                });
                return;
            }
            foreach (string item2 in v.铭文模板.开关技能列表.ToList())
            {
                if (!游戏技能.数据表.TryGetValue(item2, out var value))
                {
                    continue;
                }
                if (this.主体技能表.TryGetValue(value.绑定等级编号, out var v3) && value.需要消耗魔法?.Length > v3.技能等级.V && this is 玩家实例 { 无敌模式: false })
                {
                    if (this.当前魔力 < value.需要消耗魔法[v3.技能等级.V])
                    {
                        continue;
                    }
                    this.当前魔力 -= value.需要消耗魔法[v3.技能等级.V];
                }
                new 技能实例(this, value, v, 0, this.当前地图, this.当前坐标, this, this.当前坐标, null);
                break;
            }
        }

        public bool 玩家释放技能(ushort 技能编号, byte 动作编号, int 目标编号, Point 技能锚点)
        {
            this.升级武器鉴定();
            base.动作编号 = 动作编号;
            if (this.管理员模式)
            {
                this.发送系统消息($"释放技能:{技能编号} {动作编号} {主程.当前时间}");
            }
            foreach (技能实例 item in base.技能任务)
            {
                if (item.动作打断)
                {
                    item.是否中断 = true;
                }
            }
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                if (技能编号 == 4587 && this.当前坐骑 == 0)
                {
                    return false;
                }
                bool flag;
                flag = false;
                if (技能编号 != 2535 && 技能编号 != 45878)
                {
                    foreach (Buff数据 item2 in this.Buff列表.Values.ToList())
                    {
                        if (!item2.攻击消失 || (item2.Buff来源 != null && item2.Buff来源.地图编号 != this.地图编号))
                        {
                            continue;
                        }
                        if (item2.Buff编号.V == 2555 && 技能编号 == 4587)
                        {
                            flag = true;
                        }
                        if (item2.Buff模板.后接Buff编号 <= 0)
                        {
                            ushort[] 后接Buff列表;
                            后接Buff列表 = item2.Buff模板.后接Buff列表;
                            if (后接Buff列表 == null || 后接Buff列表.Length == 0)
                            {
                                base.删除Buff时处理(item2.Buff编号.V);
                                continue;
                            }
                        }
                        base.移除Buff时处理(item2.Buff编号.V);
                    }
                }
                if (!this.主体技能表.TryGetValue(技能编号, out var v) && !this.被动技能.TryGetValue(技能编号, out v))
                {
                    this.发送系统消息("此技能已被删除或您未学习此技能，请小退重新登陆后获得最新技能列表");
                    return false;
                }
                if ((this.冷却记录.TryGetValue(技能编号 | 0x1000000, out var v2) && 主程.当前时间 < v2) || flag)
                {
                    this.网络连接?.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (技能编号 | 0x1000000),
                        冷却时间 = (int)(v2 - 主程.当前时间).TotalMilliseconds
                    });
                    this.网络连接?.发送封包(new 技能释放完成
                    {
                        技能编号 = 技能编号,
                        动作编号 = 动作编号
                    });
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1281,
                        第一参数 = 技能编号,
                        第二参数 = 动作编号
                    });
                    return false;
                }
                if (this.角色职业 == 游戏对象职业.刺客)
                {
                    foreach (Buff数据 item3 in this.Buff列表.Values.ToList())
                    {
                        if ((item3.Buff效果 & Buff效果类型.状态标志) != 0 && (item3.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) != 0)
                        {
                            base.移除Buff时处理(item3.Buff编号.V);
                        }
                    }
                }
                if (!地图处理网关.地图对象表.TryGetValue(目标编号, out var value) && 技能编号 == 4587 && 目标编号 == 0 && value == null)
                {
                    value = this;
                }
                bool result;
                result = false;
                foreach (string item4 in v.铭文模板.主体技能列表.ToList())
                {
                    int num;
                    num = 0;
                    int num2;
                    num2 = 0;
                    List<物品数据> 物品列表;
                    物品列表 = null;
                    if (!游戏技能.数据表.TryGetValue(item4, out var value2) || value2.自身技能编号 != 技能编号)
                    {
                        continue;
                    }
                    if (value2.技能分组编号 == 0 || !this.冷却记录.TryGetValue(value2.技能分组编号 | 0, out var v3) || !(主程.当前时间 < v3))
                    {
                        if (!value2.检查职业武器 || (this.角色装备.TryGetValue(0, out var v4) && v4.需要职业 == this.角色职业))
                        {
                            if (value2.检查技能标记 && !this.Buff列表.ContainsKey(value2.技能标记编号))
                            {
                                continue;
                            }
                            if ((value2.检查被动标记 && this[游戏对象属性.技能标志] != 1) || (value2.检查技能计数 && v.剩余次数.V <= 0))
                            {
                                break;
                            }
                            if (!value2.检查忙绿状态 || !(主程.当前时间 < this.忙碌时间))
                            {
                                if (value2.计算幸运概率 || value2.计算触发概率 < 1f)
                                {
                                    if (value2.计算幸运概率)
                                    {
                                        if (!计算类.计算概率(计算类.计算幸运(this[游戏对象属性.幸运等级])))
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        float num3;
                                        num3 = 0f;
                                        if (value2.属性提升概率 != 0)
                                        {
                                            num3 = Math.Max(0f, (float)this[value2.属性提升概率] * value2.属性提升系数);
                                        }
                                        if (!计算类.计算概率(value2.计算触发概率 + num3))
                                        {
                                            continue;
                                        }
                                    }
                                }
                                if ((value2.验证已学技能 != 0 && (!this.主体技能表.TryGetValue(value2.验证已学技能, out var v5) || (value2.验证技能铭文 != 0 && value2.验证技能铭文 != v5.铭文编号))) || (value2.验证角色Buff != 0 && (!this.Buff列表.TryGetValue(value2.验证角色Buff, out var v6) || v6.当前层数.V < value2.角色Buff层数)) || (value2.验证目标Buff != 0 && (value == null || !value.Buff列表.TryGetValue(value2.验证目标Buff, out var v7) || v7.当前层数.V < value2.目标Buff层数)) || (value2.验证目标类型 != 0 && (value == null || !value.特定类型(this, value2.验证目标类型))))
                                {
                                    continue;
                                }
                                if (this.主体技能表.TryGetValue(value2.绑定等级编号, out var v8) && value2.需要消耗魔法?.Length > v8.技能等级.V && this is 玩家实例 { 无敌模式: false })
                                {
                                    num = ((value2.绑定等级编号 != 2554 || !this.Buff列表.TryGetValue(25540, out var v9)) ? value2.需要消耗魔法[v8.技能等级.V] : (value2.需要消耗魔法[v8.技能等级.V] + value2.需要消耗魔法[v8.技能等级.V] * v9.当前层数.V));
                                    if (this.当前魔力 < num)
                                    {
                                        continue;
                                    }
                                }
                                if (主程.当前时间 > this.硬直检测)
                                {
                                    this.硬直次数 = 0;
                                    this.硬直检测 = 主程.当前时间.AddSeconds(30.0);
                                }
                                if (value2.检查硬直状态 && 主程.当前时间 < this.硬直时间)
                                {
                                    if (this.管理员模式)
                                    {
                                        this.发送系统消息($"硬直了： {技能编号} {(this.硬直时间 - 主程.当前时间).TotalMilliseconds}");
                                    }
                                    this.硬直次数++;
                                    if (this.硬直次数 > 1)
                                    {
                                        this.网络连接?.发送封包(new 添加技能冷却
                                        {
                                            冷却编号 = (技能编号 | 0x1000000),
                                            冷却时间 = (int)(this.硬直时间 - 主程.当前时间).TotalMilliseconds
                                        });
                                        this.网络连接?.发送封包(new 技能释放完成
                                        {
                                            技能编号 = 技能编号,
                                            动作编号 = 动作编号
                                        });
                                        if (this.管理员模式)
                                        {
                                            this.发送系统消息("卡刀了");
                                        }
                                        this.硬直次数 = 0;
                                        continue;
                                    }
                                }
                                HashSet<int> 需要消耗物品;
                                需要消耗物品 = value2.需要消耗物品;
                                if (需要消耗物品 != null && 需要消耗物品.Count != 0)
                                {
                                    if (!this.角色装备.TryGetValue(15, out var v10) || v10.当前持久.V < value2.战具扣除点数)
                                    {
                                        if (!this.查找背包物品(value2.消耗物品数量, value2.需要消耗物品, out 物品列表))
                                        {
                                            continue;
                                        }
                                        num2 = value2.消耗物品数量;
                                    }
                                    else
                                    {
                                        物品列表 = new List<物品数据> { v10 };
                                        num2 = value2.战具扣除点数;
                                    }
                                }
                                if (num >= 0 && 计算类.计算概率(this.Buff列表.TryGetValue(26300, out var _) ? 0.75f : 1f))
                                {
                                    this.当前魔力 -= num;
                                }
                                if (物品列表 != null && 物品列表.Count == 1 && 物品列表[0].物品类型 == 物品使用分类.战具)
                                {
                                    this.战具损失持久(num2);
                                }
                                else if (物品列表 != null)
                                {
                                    this.消耗背包物品(num2, 物品列表, "释放技能消耗");
                                }
                                if (value2.检查被动标记 && this[游戏对象属性.技能标志] == 1)
                                {
                                    this[游戏对象属性.技能标志] = 0;
                                }
                                new 技能实例(this, value2, v, 动作编号, this.当前地图, this.当前坐标, value, 技能锚点, null);
                                result = true;
                                break;
                            }
                            this.网络连接?.发送封包(new 添加技能冷却
                            {
                                冷却编号 = (技能编号 | 0x1000000),
                                冷却时间 = (int)(this.忙碌时间 - 主程.当前时间).TotalMilliseconds
                            });
                            this.网络连接?.发送封包(new 技能释放完成
                            {
                                技能编号 = 技能编号,
                                动作编号 = 动作编号
                            });
                            if (this.管理员模式)
                            {
                                主程.添加系统日志($"[GM]忙碌状态通过 {value2.检查忙绿状态}   {this.忙碌时间}");
                            }
                            break;
                        }
                        this.网络连接?.发送封包(new 技能释放完成
                        {
                            技能编号 = 技能编号,
                            动作编号 = 动作编号
                        });
                        base.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1283,
                            第一参数 = 2056
                        });
                        break;
                    }
                    this.网络连接?.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (技能编号 | 0x1000000),
                        冷却时间 = (int)(v3 - 主程.当前时间).TotalMilliseconds
                    });
                    this.网络连接?.发送封包(new 技能释放完成
                    {
                        技能编号 = 技能编号,
                        动作编号 = 动作编号
                    });
                    result = true;
                    if (this.管理员模式)
                    {
                        this.发送系统消息("释放失败");
                    }
                    break;
                }
                return result;
            }
            return false;
        }

        public bool 玩家释放技能2(ushort 技能编号, byte 动作编号, int 目标编号, Point 技能锚点)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                if (!this.主体技能表.TryGetValue(技能编号, out var v) && !this.被动技能.TryGetValue(技能编号, out v))
                {
                    this.网络连接.尝试断开连接(new Exception("错误操作: 玩家释放技能. 错误: 没有学会技能. 技能编号:" + 技能编号));
                    return false;
                }
                if (this.冷却记录.TryGetValue(技能编号 | 0x1000000, out var v2) && 主程.当前时间 < v2)
                {
                    this.网络连接?.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (技能编号 | 0x1000000),
                        冷却时间 = (int)(v2 - 主程.当前时间).TotalMilliseconds
                    });
                    this.网络连接?.发送封包(new 技能释放完成
                    {
                        技能编号 = 技能编号,
                        动作编号 = 动作编号
                    });
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1281,
                        第一参数 = 技能编号,
                        第二参数 = 动作编号
                    });
                    return false;
                }
                if (this.角色职业 == 游戏对象职业.刺客)
                {
                    foreach (Buff数据 item in this.Buff列表.Values.ToList())
                    {
                        if ((item.Buff效果 & Buff效果类型.状态标志) != 0 && (item.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) != 0)
                        {
                            base.移除Buff时处理(item.Buff编号.V);
                        }
                    }
                }
                bool result;
                result = false;
                地图处理网关.地图对象表.TryGetValue(目标编号, out var value);
                foreach (string item2 in v.铭文模板.主体技能列表)
                {
                    int num;
                    num = 0;
                    int num2;
                    num2 = 0;
                    List<物品数据> 物品列表;
                    物品列表 = null;
                    if (!游戏技能.数据表.TryGetValue(item2, out var value2) || value2.自身技能编号 != 技能编号)
                    {
                        continue;
                    }
                    if (this.Buff列表.ContainsKey(12120))
                    {
                        base.移除Buff时处理(12120);
                    }
                    else if (this.Buff列表.ContainsKey(12121))
                    {
                        base.移除Buff时处理(12121);
                    }
                    else if (this.Buff列表.ContainsKey(12122))
                    {
                        base.移除Buff时处理(12122);
                    }
                    if (value2.技能分组编号 == 0 || !this.冷却记录.TryGetValue(value2.技能分组编号 | 0, out var v3) || !(主程.当前时间 < v3))
                    {
                        if (value2.检查职业武器 && (!this.角色装备.TryGetValue(0, out var v4) || v4.需要职业 != this.角色职业))
                        {
                            break;
                        }
                        if (value2.检查技能标记 && !this.Buff列表.ContainsKey(value2.技能标记编号))
                        {
                            continue;
                        }
                        if ((value2.检查被动标记 && this[游戏对象属性.技能标志] != 1) || (value2.检查技能计数 && v.剩余次数.V <= 0))
                        {
                            break;
                        }
                        if (!value2.检查忙绿状态 || !(主程.当前时间 < this.忙碌时间))
                        {
                            if (value2.检查硬直状态 && 主程.当前时间 < this.硬直时间)
                            {
                                this.网络连接?.发送封包(new 添加技能冷却
                                {
                                    冷却编号 = (技能编号 | 0x1000000),
                                    冷却时间 = (int)(this.硬直时间 - 主程.当前时间).TotalMilliseconds
                                });
                                this.网络连接?.发送封包(new 技能释放完成
                                {
                                    技能编号 = 技能编号,
                                    动作编号 = 动作编号
                                });
                                continue;
                            }
                            if (value2.计算幸运概率 || value2.计算触发概率 < 1f)
                            {
                                if (value2.计算幸运概率)
                                {
                                    if (!计算类.计算概率(计算类.计算幸运(this[游戏对象属性.幸运等级])))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    float num3;
                                    num3 = 0f;
                                    if (value2.属性提升概率 != 0)
                                    {
                                        num3 = Math.Max(0f, (float)this[value2.属性提升概率] * value2.属性提升系数);
                                    }
                                    if (!计算类.计算概率(value2.计算触发概率 + num3))
                                    {
                                        continue;
                                    }
                                }
                            }
                            bool num4;
                            num4 = value2.验证已学技能 == 0 || (this.主体技能表.TryGetValue(value2.验证已学技能, out var v5) && (value2.验证技能铭文 == 0 || value2.验证技能铭文 == v5.铭文编号));
                            bool flag;
                            flag = value2.验证角色Buff == 0 || (this.Buff列表.TryGetValue(value2.验证角色Buff, out var v6) && v6.当前层数.V >= value2.角色Buff层数);
                            bool flag2;
                            flag2 = value2.验证目标Buff == 0 || (value != null && value.Buff列表.TryGetValue(value2.验证目标Buff, out var v7) && v7.当前层数.V >= value2.目标Buff层数);
                            bool flag3;
                            flag3 = value2.验证目标类型 == 指定目标类型.无 || (value?.特定类型(this, value2.验证目标类型) ?? false);
                            bool flag4;
                            flag4 = !this.主体技能表.TryGetValue(value2.绑定等级编号, out var v8) || !(value2.需要消耗魔法?.Length > v8.技能等级.V) || this.当前魔力 >= (num = value2.需要消耗魔法[v8.技能等级.V]);
                            if (!num4 || !flag || !flag2 || !flag3 || !flag4)
                            {
                                continue;
                            }
                            HashSet<int> 需要消耗物品;
                            需要消耗物品 = value2.需要消耗物品;
                            if (需要消耗物品 != null && 需要消耗物品.Count != 0)
                            {
                                if (this.角色装备.TryGetValue(15, out var v9) && v9.当前持久.V >= value2.战具扣除点数)
                                {
                                    物品列表 = new List<物品数据> { v9 };
                                    num2 = value2.战具扣除点数;
                                }
                                else
                                {
                                    if (!this.查找背包物品(value2.消耗物品数量, value2.需要消耗物品, out 物品列表))
                                    {
                                        continue;
                                    }
                                    num2 = value2.消耗物品数量;
                                }
                            }
                            if (num >= 0)
                            {
                                this.当前魔力 -= num;
                            }
                            if (物品列表 != null && 物品列表.Count == 1 && 物品列表[0].物品类型 == 物品使用分类.战具)
                            {
                                this.战具损失持久(num2);
                            }
                            else if (物品列表 != null)
                            {
                                this.消耗背包物品(num2, 物品列表);
                            }
                            if (value2.检查被动标记 && this[游戏对象属性.技能标志] == 1)
                            {
                                this[游戏对象属性.技能标志] = 0;
                            }
                            if (value2.自身技能编号 == 4587)
                            {
                                result = true;
                                continue;
                            }
                            new 技能实例(this, value2, v, 动作编号, this.当前地图, this.当前坐标, value, 技能锚点, null);
                            result = true;
                            continue;
                        }
                        this.网络连接?.发送封包(new 添加技能冷却
                        {
                            冷却编号 = (技能编号 | 0x1000000),
                            冷却时间 = (int)(this.忙碌时间 - 主程.当前时间).TotalMilliseconds
                        });
                        this.网络连接?.发送封包(new 技能释放完成
                        {
                            技能编号 = 技能编号,
                            动作编号 = 动作编号
                        });
                        break;
                    }
                    this.网络连接?.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (技能编号 | 0x1000000),
                        冷却时间 = (int)(v3 - 主程.当前时间).TotalMilliseconds
                    });
                    this.网络连接?.发送封包(new 技能释放完成
                    {
                        技能编号 = 技能编号,
                        动作编号 = 动作编号
                    });
                    break;
                }
                return result;
            }
            return false;
        }

        public void 更改攻击模式(攻击模式 模式)
        {
            this.攻击模式 = 模式;
        }

        public void 更改宠物模式(宠物模式 模式)
        {
            if (this.宠物数量 == 0)
            {
                return;
            }
            if (this.宠物模式 == 宠物模式.休息 && (模式 == 宠物模式.自动 || 模式 == 宠物模式.攻击))
            {
                foreach (宠物实例 item in this.宠物列表.ToList())
                {
                    item.对象仇恨.仇恨列表.Clear();
                }
                this.宠物模式 = 宠物模式.攻击;
            }
            else
            {
                if (this.宠物模式 != 宠物模式.攻击 || (模式 != 0 && 模式 != 宠物模式.休息))
                {
                    return;
                }
                this.宠物模式 = 宠物模式.休息;
                if (this.对象死亡 || !this.当前地图.安全区内(this.当前坐标))
                {
                    return;
                }
                foreach (宠物实例 item2 in this.宠物列表.ToList())
                {
                    if (!item2.对象死亡 && item2.激活对象)
                    {
                        item2.宠物召回处理(重叠: true);
                    }
                }

            }
        }

        public void 玩家拖动技能(byte 技能栏位, ushort 技能编号)
        {
            if (技能栏位 <= 7 || 技能栏位 >= 32)
            {
                return;
            }
            技能数据 v3;
            if (this.主体技能表.TryGetValue(技能编号, out var v))
            {
                if (!v.自动装配 && v.快捷栏位.V != 技能栏位)
                {
                    this.快捷栏位.Remove(v.快捷栏位.V);
                    v.快捷栏位.V = 100;
                    if (this.快捷栏位.TryGetValue(技能栏位, out var v2) && v2 != null)
                    {
                        v2.快捷栏位.V = 100;
                    }
                    this.快捷栏位[技能栏位] = v;
                    v.快捷栏位.V = 技能栏位;
                    this.网络连接?.发送封包(new 角色拖动技能
                    {
                        技能栏位 = 技能栏位,
                        铭文编号 = v.铭文编号,
                        技能编号 = v.技能编号.V,
                        技能等级 = v.技能等级.V
                    });
                }
            }
            else if (this.快捷栏位.TryGetValue(技能栏位, out v3))
            {
                this.网络连接?.发送封包(new 角色拖动技能
                {
                    技能栏位 = 技能栏位,
                    铭文编号 = 0,
                    技能编号 = 0,
                    技能等级 = 0
                });
                this.快捷栏位.Remove(技能栏位);
                v3.快捷栏位.V = 100;
                this.默认技能位置(v3);
            }
        }

        public void 默认技能位置(技能数据 技能)
        {
            byte b;
            b = 0;
            switch (技能.技能编号.V)
            {
                case 1042:
                    b = 5;
                    break;
                case 1038:
                    b = 7;
                    break;
                case 1208:
                    b = 7;
                    break;
                case 1050:
                    b = 4;
                    break;
                case 1049:
                    b = 6;
                    break;
                case 1547:
                    b = 6;
                    break;
                case 1545:
                    b = 7;
                    break;
                case 2058:
                    b = 6;
                    break;
                case 2057:
                    b = 7;
                    break;
                case 2052:
                    b = 5;
                    break;
            }
            if (b > 0)
            {
                this.网络连接?.发送封包(new 角色拖动技能
                {
                    技能栏位 = b,
                    铭文编号 = 技能.铭文编号,
                    技能编号 = 技能.技能编号.V,
                    技能等级 = 技能.技能等级.V
                });
                技能.快捷栏位.V = b;
                this.快捷栏位[b] = 技能;
            }
        }

        public void 玩家选中对象(int 对象编号)
        {
            if (地图处理网关.地图对象表.TryGetValue(对象编号, out var value))
            {
                this.网络连接?.发送封包(new 玩家选中目标
                {
                    角色编号 = this.地图编号,
                    目标编号 = value.地图编号
                });
                this.网络连接?.发送封包(new 选中目标详情
                {
                    对象编号 = value.地图编号,
                    当前体力 = value.当前体力,
                    当前魔力 = value.当前魔力,
                    最大体力 = value[游戏对象属性.最大体力],
                    最大魔力 = value[游戏对象属性.最大魔力],
                    Buff描述 = value.对象Buff详述()
                });
            }
        }

        public void 更新物品详情(物品数据 物品)
        {
            if (物品 != null)
            {
                if (物品 is 装备数据 装备数据)
                {
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 物品.字节描述()
                    });
                }
            }
        }

        public void 发送错误提示(int 错误代码, int 第一参数, int 第二参数)
        {
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 错误代码,
                第一参数 = 第一参数,
                第二参数 = 第二参数
            });
        }

        public void 开始Npcc对话(int 对象编号)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || !地图处理网关.守卫对象表.TryGetValue(对象编号, out this.对话守卫) || this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12)
            {
                return;
            }
            this.对话页面 = 0;
            this.对话触发 = "";
            this.打开商店 = this.对话守卫.商店编号;
            this.打开界面 = this.对话守卫.界面代码;
            this.对话超时 = 主程.当前时间.AddSeconds(30.0);
            string SAY;
            SAY = string.Empty;
            if (!this.对话守卫.对象模板.触发lua)
            {
                if (副本.CallNPCMain(this, this.对话守卫, out SAY) && SAY != string.Empty)
                {
                    this.网络连接?.发送封包(new 同步交互结果
                    {
                        对象编号 = this.对话守卫.地图编号,
                        交互文本 = Encoding.UTF8.GetBytes(SAY + "\0")
                    });
                }
                else
                {
                    this.CallNPC(this.对话守卫.地图编号, this.对话守卫.ScriptID, "[@MAIN]");
                }
                return;
            }
            SAY = 游戏脚本.对话NPC(this, this.对话守卫);
            if (SAY != string.Empty)
            {
                this.打开商店 = this.对话守卫.商店编号;
                this.打开界面 = this.对话守卫.界面代码;
                this.对话超时 = 主程.当前时间.AddSeconds(30.0);
                this.网络连接?.发送封包(new 同步交互结果
                {
                    对象编号 = this.对话守卫.地图编号,
                    交互文本 = Encoding.UTF8.GetBytes(SAY + "\0")
                });
            }
        }

        public void 继续Npc对话(int 选项编号)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || this.对话守卫 == null || this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12)
            {
                return;
            }
            if (!(主程.当前时间 > this.对话超时))
            {
                this.对话超时 = 主程.当前时间.AddSeconds(30.0);
                string SAY;
                SAY = string.Empty;
                if (!this.对话守卫.对象模板.触发lua)
                {
                    if (this.对话触发 == "")
                    {
                        this.对话触发 = 选项编号.ToString();
                    }
                    else
                    {
                        this.对话触发 = this.对话触发 + "_" + 选项编号;
                    }
                    this.对话页面 = this.对话页面 * 10 + 选项编号;
                    if (副本.CallNPCMain(this, this.对话守卫, out SAY) && SAY != string.Empty)
                    {
                        this.网络连接?.发送封包(new 同步交互结果
                        {
                            对象编号 = this.对话守卫.地图编号,
                            交互文本 = Encoding.UTF8.GetBytes(SAY + "\0")
                        });
                    }
                    else
                    {
                        this.CallNPC(this.对话守卫.地图编号, this.对话守卫.ScriptID, "[@" + this.对话触发 + "]");
                    }
                }
                else
                {
                    SAY = 游戏脚本.点击NPC(this, this.对话守卫, 选项编号);
                    if (SAY != string.Empty)
                    {
                        this.网络连接?.发送封包(new 同步交互结果
                        {
                            对象编号 = this.对话守卫.地图编号,
                            交互文本 = Encoding.UTF8.GetBytes(SAY + "\0")
                        });
                    }
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 3333
                });
            }
        }

        public void 继续Npcc对话(int 选项编号)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3 && this.对话守卫 != null && this.当前地图 == this.对话守卫.当前地图 && base.网格距离(this.对话守卫) <= 12)
            {
                if (!(主程.当前时间 > this.对话超时))
                {
                    this.对话超时 = 主程.当前时间.AddSeconds(30.0);
                    return;
                }
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 3333
                });
            }
        }

        public void 玩家更改设置(byte[] 设置)
        {
            using MemoryStream input = new MemoryStream(设置);
            using BinaryReader binaryReader = new BinaryReader(input);
            int num;
            num = 设置.Length / 5;
            for (int i = 0; i < num; i++)
            {
                byte 索引;
                索引 = binaryReader.ReadByte();
                uint value;
                value = binaryReader.ReadUInt32();
                this.角色数据.玩家设置[索引] = value;
            }
        }

        public void 查询地图路线()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((ushort)this.当前地图.分线数量);
            binaryWriter.Write(this.当前地图.地图编号);
            binaryWriter.Write(this.当前地图.路线编号);
            binaryWriter.Write(this.当前地图.地图状态);
            this.网络连接?.发送封包(new 查询线路信息
            {
                字节数据 = memoryStream.ToArray()
            });
        }

        public void 切换地图路线()
        {
        }

        public void 玩家同步位置(Point 坐标, ushort 高度)
        {
        }

        public void 玩家扩展背包(byte 背包类型, byte 扩展大小)
        {
            if (扩展大小 == 0)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家扩展背包.  错误: 扩展参数错误."));
                return;
            }
            if (背包类型 == 1 && this.背包大小 + 扩展大小 > 64)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家扩展背包.  错误: 背包超出限制."));
                return;
            }
            if (背包类型 == 2 && this.仓库大小 + 扩展大小 > 216)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家扩展背包.  错误: 仓库超出限制."));
                return;
            }
            if (背包类型 == 7 && this.资源包大小 + 扩展大小 > 216)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家扩展背包.  错误: 资源包超出限制."));
                return;
            }
            switch (背包类型)
            {
                case 7:
                    {
                        int num3;
                        num3 = 0;
                        for (int i = 0; i < 扩展大小; i++)
                        {
                            num3 += (int)Math.Ceiling((decimal)(this.资源包大小 + i + 1) / 72m) * 10000;
                        }
                        if (this.金币数量 < num3)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1821
                            });
                            break;
                        }
                        this.金币数量 -= (uint)num3;
                        主程.添加货币日志(this, "资源包扩展扣", 游戏货币.金币, -num3);
                        this.资源包大小 += 扩展大小;
                        this.网络连接?.发送封包(new 背包容量改变
                        {
                            背包类型 = 7,
                            背包容量 = this.资源包大小
                        });
                        break;
                    }
                case 2:
                    {
                        int num4;
                        num4 = 计算类.扩展仓库(this.仓库大小 - 16);
                        int num5;
                        num5 = 计算类.扩展仓库(this.仓库大小 + 扩展大小 - 16) - num4;
                        if (this.金币数量 < num5)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1821
                            });
                            break;
                        }
                        this.金币数量 -= (uint)num5;
                        主程.添加货币日志(this, "仓库扩容扣除", 游戏货币.金币, -num5);
                        this.仓库大小 += 扩展大小;
                        this.网络连接?.发送封包(new 背包容量改变
                        {
                            背包类型 = 2,
                            背包容量 = this.仓库大小
                        });
                        break;
                    }
                case 1:
                    {
                        int num;
                        num = 计算类.扩展背包(this.背包大小 - 32);
                        int num2;
                        num2 = 计算类.扩展背包(this.背包大小 + 扩展大小 - 32) - num;
                        if (this.金币数量 < num2)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1821
                            });
                            break;
                        }
                        this.金币数量 -= (uint)num2;
                        主程.添加货币日志(this, "背包扩容扣除", 游戏货币.金币, -num2);
                        this.背包大小 += 扩展大小;
                        this.网络连接?.发送封包(new 背包容量改变
                        {
                            背包类型 = 1,
                            背包容量 = this.背包大小
                        });
                        break;
                    }
            }
        }

        public void 商店特修单件(byte 背包类型, byte 装备位置)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.对话守卫 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 商店修理单件.  错误: 没有选中Npc."));
            }
            else
            {
                if (this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12)
                {
                    return;
                }
                switch (背包类型)
                {
                    case 1:
                        {
                            if (!this.角色背包.TryGetValue(装备位置, out var v2))
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1802
                                });
                                break;
                            }
                            if (!(v2 is 装备数据 装备数据))
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1814
                                });
                                break;
                            }
                            if (!装备数据.能否修理)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1814
                                });
                                break;
                            }
                            if (this.金币数量 < 装备数据.特修费用)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1821
                                });
                                break;
                            }
                            this.金币数量 -= (uint)装备数据.特修费用;
                            主程.添加货币日志(this, "装备修理扣除", 游戏货币.金币, -装备数据.特修费用);
                            装备数据.当前持久.V = 装备数据.最大持久.V;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = 装备数据.字节描述()
                            });
                            break;
                        }
                    case 0:
                        {
                            if (!this.角色装备.TryGetValue(装备位置, out var v))
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1802
                                });
                                break;
                            }
                            if (!v.能否修理)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1814
                                });
                                break;
                            }
                            if (this.金币数量 < v.特修费用)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1821
                                });
                                break;
                            }
                            this.金币数量 -= (uint)v.特修费用;
                            主程.添加货币日志(this, "背包修理扣除", 游戏货币.金币, -v.特修费用);
                            if (v.当前持久.V <= 0)
                            {
                                base.属性加成[v] = v.装备属性;
                                this.更新对象属性();
                            }
                            v.当前持久.V = v.最大持久.V;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = v.字节描述()
                            });
                            this.网络连接?.发送封包(new 修理物品应答());
                            break;
                        }
                }
            }
        }

        public void 商店修理单件(byte 背包类型, byte 装备位置)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.对话守卫 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 商店修理单件.  错误: 没有选中Npc."));
            }
            else
            {
                if (this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12)
                {
                    return;
                }
                switch (背包类型)
                {
                    case 1:
                        {
                            if (!this.角色背包.TryGetValue(装备位置, out var v2))
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1802
                                });
                                break;
                            }
                            if (!(v2 is 装备数据 装备数据))
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1814
                                });
                                break;
                            }
                            if (!装备数据.能否修理)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1814
                                });
                                break;
                            }
                            if (this.金币数量 < 装备数据.修理费用)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1821
                                });
                                break;
                            }
                            this.金币数量 -= (uint)装备数据.修理费用;
                            主程.添加货币日志(this, "装备修理扣除", 游戏货币.金币, -装备数据.修理费用);
                            装备数据.最大持久.V = Math.Max(1000, 装备数据.最大持久.V - 334);
                            装备数据.当前持久.V = 装备数据.最大持久.V;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = 装备数据.字节描述()
                            });
                            break;
                        }
                    case 0:
                        {
                            if (!this.角色装备.TryGetValue(装备位置, out var v))
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1802
                                });
                                break;
                            }
                            if (!v.能否修理)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1814
                                });
                                break;
                            }
                            if (this.金币数量 < v.修理费用)
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1821
                                });
                                break;
                            }
                            this.金币数量 -= (uint)v.修理费用;
                            主程.添加货币日志(this, "背包修理扣除", 游戏货币.金币, -v.修理费用);
                            v.最大持久.V = Math.Max(1000, v.最大持久.V - (int)((float)(v.最大持久.V - v.当前持久.V) * 0.035f));
                            if (v.当前持久.V <= 0)
                            {
                                base.属性加成[v] = v.装备属性;
                                this.更新对象属性();
                            }
                            v.当前持久.V = v.最大持久.V;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = v.字节描述()
                            });
                            this.网络连接?.发送封包(new 修理物品应答());
                            break;
                        }
                }
            }
        }

        public void 商店修理全部()
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.对话守卫 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 商店修理单件.  错误: 没有选中Npc."));
            }
            else
            {
                if (this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12)
                {
                    return;
                }
                if (this.金币数量 < this.角色装备.Values.Sum((装备数据 O) => O.能否修理 ? O.修理费用 : 0))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1821
                    });
                    return;
                }
                foreach (装备数据 value in this.角色装备.Values)
                {
                    if (value.能否修理)
                    {
                        this.金币数量 -= (uint)value.修理费用;
                        主程.添加货币日志(this, "全身修理扣除", 游戏货币.金币, -value.修理费用);
                        value.最大持久.V = Math.Max(1000, value.最大持久.V - (int)((float)(value.最大持久.V - value.当前持久.V) * 0.035f));
                        if (value.当前持久.V <= 0)
                        {
                            base.属性加成[value] = value.装备属性;
                            this.更新对象属性();
                        }
                        value.当前持久.V = value.最大持久.V;
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = value.字节描述()
                        });
                    }
                }
                this.网络连接?.发送封包(new 修理物品应答());
            }
        }

        public void 随身修理单件(byte 背包类型, byte 装备位置, int 物品编号)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            switch (背包类型)
            {
                case 1:
                    {
                        if (!this.角色背包.TryGetValue(装备位置, out var v2))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1802
                            });
                        }
                        else if (!(v2 is 装备数据 装备数据))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1814
                            });
                        }
                        else if (!装备数据.能否修理)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1814
                            });
                        }
                        else if (物品编号 != 0)
                        {
                            if (!游戏物品.数据表.TryGetValue(物品编号, out var value2) || (value2.物品编号 != 80010 && value2.物品编号 != 80011) || (((游戏装备)装备数据.物品模板).装备套装 > 游戏装备套装.沃玛装备 && value2.物品编号 != 80011))
                            {
                                break;
                            }
                            List<物品数据> list2;
                            list2 = this.查找背包物品(物品编号, 装备数据.战神油数);
                            if (list2 != null)
                            {
                                this.消耗背包物品(装备数据.战神油数, list2, "随身背包修理");
                                if (装备数据.当前持久.V <= 0)
                                {
                                    base.属性加成[装备数据] = 装备数据.装备属性;
                                    this.更新对象属性();
                                }
                                装备数据.当前持久.V = 装备数据.最大持久.V;
                                this.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = 装备数据.字节描述()
                                });
                                this.网络连接?.发送封包(new 修理物品应答());
                            }
                        }
                        else if (this.金币数量 < 装备数据.特修费用)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1821
                            });
                        }
                        else
                        {
                            this.金币数量 -= (uint)装备数据.特修费用;
                            主程.添加货币日志(this, "随身背包修理", 游戏货币.金币, -装备数据.特修费用);
                            if (装备数据.当前持久.V <= 0)
                            {
                                base.属性加成[装备数据] = 装备数据.装备属性;
                                this.更新对象属性();
                            }
                            装备数据.当前持久.V = 装备数据.最大持久.V;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = 装备数据.字节描述()
                            });
                            this.网络连接?.发送封包(new 修理物品应答());
                        }
                        break;
                    }
                case 0:
                    {
                        if (!this.角色装备.TryGetValue(装备位置, out var v))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1802
                            });
                        }
                        else if (!v.能否修理)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1814
                            });
                        }
                        else if (物品编号 != 0)
                        {
                            if (!游戏物品.数据表.TryGetValue(物品编号, out var value) || (value.物品编号 != 80010 && value.物品编号 != 80011) || (((游戏装备)v.物品模板).装备套装 > 游戏装备套装.沃玛装备 && value.物品编号 != 80011))
                            {
                                break;
                            }
                            List<物品数据> list;
                            list = this.查找背包物品(物品编号, v.战神油数);
                            if (list != null)
                            {
                                this.消耗背包物品(v.战神油数, list, "随身背包修理");
                                if (v.当前持久.V <= 0)
                                {
                                    base.属性加成[v] = v.装备属性;
                                    this.更新对象属性();
                                }
                                v.当前持久.V = v.最大持久.V;
                                this.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = v.字节描述()
                                });
                                this.网络连接?.发送封包(new 修理物品应答());
                            }
                        }
                        else if (this.金币数量 < v.特修费用)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1821
                            });
                        }
                        else
                        {
                            this.金币数量 -= (uint)v.特修费用;
                            主程.添加货币日志(this, "随身装备修理", 游戏货币.金币, -v.特修费用);
                            v.当前持久.V = v.最大持久.V;
                            this.网络连接?.发送封包(new 玩家物品变动
                            {
                                物品描述 = v.字节描述()
                            });
                        }
                        break;
                    }
            }
        }

        public void 随身修理全部()
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.金币数量 < this.角色装备.Values.Sum((装备数据 O) => O.能否修理 ? O.特修费用 : 0))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1821
                });
                return;
            }
            foreach (装备数据 value in this.角色装备.Values)
            {
                if (value.能否修理)
                {
                    this.金币数量 -= (uint)value.特修费用;
                    主程.添加货币日志(this, "随身全身修理", 游戏货币.金币, -value.特修费用);
                    if (value.当前持久.V <= 0)
                    {
                        base.属性加成[value] = value.装备属性;
                        this.更新对象属性();
                    }
                    value.当前持久.V = value.最大持久.V;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = value.字节描述()
                    });
                }
            }
            this.网络连接?.发送封包(new 修理物品应答());
        }

        public void 请求商店数据(int 数据版本)
        {
            if (数据版本 != 0 && 数据版本 == 游戏商店.商店文件效验)
            {
                this.网络连接?.发送封包(new 同步商店数据
                {
                    版本编号 = 游戏商店.商店文件效验,
                    商品数量 = 0,
                    文件内容 = new byte[0]
                });
            }
            else if (!this.封包间隔限制("请求商店数据"))
            {
                this.网络连接?.发送封包(new 同步商店数据
                {
                    版本编号 = 游戏商店.商店文件效验,
                    商品数量 = 游戏商店.商店物品数量,
                    文件内容 = 游戏商店.商店文件数据
                });
                this.记录封包间隔("请求商店数据");
            }
        }

        public void 查询珍宝商店(int 数据版本)
        {
            if (数据版本 != 0 && 数据版本 == 珍宝商品.珍宝商店效验)
            {
                this.网络连接?.发送封包(new 同步珍宝数据
                {
                    版本编号 = 珍宝商品.珍宝商店效验,
                    商品数量 = 0,
                    商店数据 = new byte[0]
                });
            }
            else
            {
                this.网络连接?.发送封包(new 同步珍宝数据
                {
                    版本编号 = 珍宝商品.珍宝商店效验,
                    商品数量 = 珍宝商品.珍宝商店数量,
                    商店数据 = 珍宝商品.珍宝商店数据
                });
            }
        }

        public void 查询出售信息()
        {
        }

        public void 购买珍宝商品(int 物品编号, int 购入数量)
        {
            if (购入数量 <= 0 || !珍宝商品.数据表.TryGetValue(物品编号, out var value) || !游戏物品.数据表.TryGetValue(物品编号, out var value2) || this.封包间隔限制("购买珍宝商品"))
            {
                return;
            }
            int num;
            num = ((购入数量 == 1 || value2.持久类型 != 物品持久分类.堆叠) ? 1 : Math.Min(购入数量, value2.物品持久));
            int num2;
            num2 = value.商品现价 * num;
            if (this.元宝数量 < num2)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 8451
                });
                return;
            }
            if (this.背包剩余 <= 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return;
            }
            int num3;
            num3 = -1;
            byte b;
            b = 0;
            while (b < this.背包大小)
            {
                if (this.角色背包.TryGetValue(b, out var v) && (value2.持久类型 != 物品持久分类.堆叠 || value2.物品编号 != v.物品编号 || v.当前持久.V + 购入数量 > value2.物品持久))
                {
                    b++;
                    continue;
                }
                num3 = b;
                break;
            }
            if (num3 == -1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return;
            }
            this.元宝数量 -= (uint)num2;
            this.角色数据.消耗元宝.V += num2;
            主程.添加货币日志(this, "购买珍宝物品", 游戏货币.元宝, -num2);
            if (this.角色背包.TryGetValue((byte)num3, out var v2))
            {
                v2.当前持久.V += num;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = v2.字节描述()
                });
                主程.添加物品日志(this, "购买珍宝物品", v2, num);
            }
            else
            {
                if (value2 is 游戏装备 模板)
                {
                    this.角色背包[(byte)num3] = new 装备数据(模板, this.角色数据, 1, (byte)num3, 随机生成: false, 绑定: false, this.对象名字 + "-珍宝商店");
                }
                else
                {
                    int 持久;
                    持久 = 0;
                    switch (value2.持久类型)
                    {
                        case 物品持久分类.堆叠:
                            持久 = num;
                            break;
                        case 物品持久分类.容器:
                            持久 = 0;
                            break;
                        case 物品持久分类.消耗:
                        case 物品持久分类.纯度:
                            持久 = value2.物品持久;
                            break;
                    }
                    this.角色背包[(byte)num3] = new 物品数据(value2, this.角色数据, 1, (byte)num3, 持久, 绑定: false, this.对象名字 + "-珍宝商店");
                }
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = this.角色背包[(byte)num3].字节描述()
                });
                主程.添加物品日志(this, "购买珍宝物品", this.角色背包[(byte)num3], 1);
            }
            this.记录封包间隔("购买珍宝商品");
        }

        private bool 封包间隔限制(string key)
        {
            if (!Settings.限制重要封包间隔时间)
            {
                return false;
            }
            if (this.下次请求时间.TryGetValue(key, out var value))
            {
                return 主程.当前时间 < value;
            }
            return false;
        }

        private void 记录封包间隔(string key)
        {
            if (Settings.限制重要封包间隔时间)
            {
                this.下次请求时间[key] = 主程.当前时间.AddMilliseconds(this.请求间隔毫秒);
            }
        }

        public void 购买每周特惠(int 礼包编号)
        {
            if (Settings.屏蔽每周特惠)
            {
                return;
            }
            switch (礼包编号)
            {
                default:
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 8467
                    });
                    break;
                case 2:
                    if (this.元宝数量 < 3000)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 2561
                        });
                        break;
                    }
                    if (计算类.日期同周(this.角色数据.战备日期.V, 主程.当前时间))
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 8466
                        });
                        break;
                    }
                    if (this.背包剩余 <= 2)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        break;
                    }
                    if (!this.特惠礼包(礼包编号))
                    {
                        if (!游戏物品.数据表.TryGetValue(90226, out var value4) || !游戏物品.数据表.TryGetValue(140001, out var value5) || !游戏物品.数据表.TryGetValue(91102, out var value6))
                        {
                            break;
                        }
                        this.修改货币("+", 游戏货币.银币, 200000u);
                        this.双倍经验 += 1500000;
                        if (this.角色数据.尝试获取背包空余格子(out var location4))
                        {
                            this.玩家获得物品(value4, location4, "购买战备礼包", 60);
                        }
                        else
                        {
                            this.角色数据.发送邮件(null, "每周战备礼包", "您购买特惠礼包获得的[" + value4.物品名字 + "]由于背包容量不足，以邮件的形式发送到您的账户，请查收附件。", value4.物品编号, 60);
                        }
                        if (this.角色数据.尝试获取背包空余格子(out var location5))
                        {
                            this.玩家获得物品(value5, location5, "购买补给礼包", 20);
                        }
                        else
                        {
                            this.角色数据.发送邮件(null, "每周战备礼包", "您购买特惠礼包获得的[" + value4.物品名字 + "]由于背包容量不足，以邮件的形式发送到您的账户，请查收附件。", value5.物品编号, 20);
                        }
                        if (this.角色数据.尝试获取背包空余格子(out var location6))
                        {
                            this.玩家获得物品(value6, location6, "购买补给礼包", 5);
                        }
                        else
                        {
                            this.角色数据.发送邮件(null, "每周战备礼包", "您购买特惠礼包获得的[" + value4.物品名字 + "]由于背包容量不足，以邮件的形式发送到您的账户，请查收附件。", value6.物品编号, 5);
                        }
                        主程.添加货币日志(this, "购买战备礼包", 游戏货币.金币, 875000);
                    }
                    this.元宝数量 -= 3000u;
                    this.角色数据.消耗元宝.V += 3000L;
                    this.角色数据.战备日期.V = 主程.当前时间;
                    this.网络连接?.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 975,
                        变量内容 = 计算类.时间转换(主程.当前时间)
                    });
                    this.修改战功任务(8, 1);
                    主程.添加货币日志(this, "购买战备礼包", 游戏货币.元宝, -3000);
                    break;
                case 1:
                    if (this.元宝数量 < 600)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 2561
                        });
                        break;
                    }
                    if (计算类.日期同周(this.角色数据.补给日期.V, 主程.当前时间))
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 8466
                        });
                        break;
                    }
                    if (this.背包剩余 <= 2)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1793
                        });
                        break;
                    }
                    if (!this.特惠礼包(礼包编号))
                    {
                        if (!游戏物品.数据表.TryGetValue(1370214, out var value) || !游戏物品.数据表.TryGetValue(1500230, out var value2) || !游戏物品.数据表.TryGetValue(2343, out var value3))
                        {
                            break;
                        }
                        this.修改货币("+", 游戏货币.金币, 50000u);
                        this.双倍经验 += 1500000;
                        if (this.角色数据.尝试获取背包空余格子(out var location))
                        {
                            this.玩家获得物品(value, location, "购买补给礼包");
                        }
                        else
                        {
                            this.角色数据.发送邮件(null, "每周补给礼包", "您购买特惠礼包获得的[" + value.物品名字 + "]由于背包容量不足，以邮件的形式发送到您的账户，请查收附件。", value.物品编号);
                        }
                        if (this.角色数据.尝试获取背包空余格子(out var location2))
                        {
                            this.玩家获得物品(value2, location2, "购买补给礼包");
                        }
                        else
                        {
                            this.角色数据.发送邮件(null, "每周补给礼包", "您购买特惠礼包获得的[" + value.物品名字 + "]由于背包容量不足，以邮件的形式发送到您的账户，请查收附件。", value2.物品编号);
                        }
                        if (this.角色数据.尝试获取背包空余格子(out var location3))
                        {
                            this.玩家获得物品(value3, location3, "购买补给礼包");
                        }
                        else
                        {
                            this.角色数据.发送邮件(null, "每周补给礼包", "您购买特惠礼包获得的[" + value.物品名字 + "]由于背包容量不足，以邮件的形式发送到您的账户，请查收附件。", value3.物品编号);
                        }
                        主程.添加货币日志(this, "购买补给礼包", 游戏货币.金币, 165000);
                    }
                    this.元宝数量 -= 600u;
                    this.角色数据.消耗元宝.V += 600L;
                    this.角色数据.补给日期.V = 主程.当前时间;
                    this.网络连接?.发送封包(new 同步补充变量
                    {
                        变量类型 = 1,
                        对象编号 = this.地图编号,
                        变量索引 = 112,
                        变量内容 = 计算类.时间转换(主程.当前时间)
                    });
                    this.修改战功任务(7, 1);
                    主程.添加货币日志(this, "购买补给礼包", 游戏货币.元宝, -600);
                    break;
            }
        }

        private bool 特惠礼包(int 礼包编号)
        {
            每周特惠 每周特惠;
            每周特惠 = 每周特惠.数据表.FirstOrDefault((每周特惠 o) => o.版本节点 == 主程.开区节点 && o.特惠类型 == 礼包编号);
            if (每周特惠 == null)
            {
                return false;
            }
            this.双倍经验 += 每周特惠.附加经验;
            this.增加货币(每周特惠.货币类型, 每周特惠.奖励货币, "购买每周特惠");
            for (int i = 0; i < 每周特惠.奖励物品.Length; i++)
            {
                if (每周特惠.奖励数量[i] > 0)
                {
                    this.玩家获得物品(每周特惠.奖励物品[i], 每周特惠.奖励数量[i], "购买每周特惠", 每周特惠.是否绑定[i]);
                }
            }
            return true;
        }

        public void 获得玛法特权(byte 特权类型, int 时间 = 30, bool 强制激活 = false)
        {
            if (this.本期特权 != 0 && !强制激活)
            {
                this.剩余特权[特权类型] += 时间;
            }
            else
            {
                this.玩家激活特权(特权类型, 时间);
            }
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 65548,
                第一参数 = 特权类型
            });
            this.网络连接?.发送封包(new 同步特权信息
            {
                字节数组 = this.玛法特权描述()
            });
        }

        public void 购买玛法特权(byte 特权类型, byte 购买数量)
        {
            if (!Settings.可购买玛法特权)
            {
                return;
            }
            if (this.本期特权 == 特权类型)
            {
                this.发送顶部公告("已经拥有相同特权,请不要重复购买,称号请在特权的签到领取");
                return;
            }
            int num;
            num = 0;
            switch (this.本期特权)
            {
                case 3:
                    num = Settings.玛法名俊价格 * 100;
                    break;
                case 4:
                    num = Settings.玛法豪杰价格 * 100;
                    break;
                case 5:
                    num = Settings.玛法战将价格 * 100;
                    break;
                case 6:
                    num = Settings.玛法新秀价格 * 100;
                    break;
                case 7:
                    num = Settings.玛法至尊价格 * 100;
                    break;
            }
            int num2;
            switch (特权类型)
            {
                default:
                    return;
                case 3:
                    num2 = Settings.玛法名俊价格 * 100;
                    break;
                case 4:
                    num2 = Settings.玛法豪杰价格 * 100;
                    break;
                case 5:
                    num2 = Settings.玛法战将价格 * 100;
                    break;
                case 6:
                    num2 = Settings.玛法新秀价格 * 100;
                    break;
                case 7:
                    num2 = Settings.玛法至尊价格 * 100;
                    break;
            }
            if (num <= num2 && (this.本期特权 != 5 || 特权类型 != 4))
            {
                int num3;
                num3 = (int)((double)((float)num / 30f) * (30.0 - 主程.当前时间.Subtract(this.本期日期).TotalDays));
                num2 -= num3;
                if (this.元宝数量 < num2)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 8451
                    });
                    return;
                }
                if (num3 > 0)
                {
                    this.角色数据.发送邮件(new 邮件数据(null, "特权已升级", $"您购买了更高级的特权,之前特权剩余天数已折算为{num3 / 100}元宝,本次购买共花费{num2 / 100}元宝", null));
                }
                this.元宝数量 -= (uint)num2;
                this.角色数据.消耗元宝.V += num2;
                主程.添加货币日志(this, "购买玛法特权", 游戏货币.元宝, -num2);
                this.获得玛法特权(特权类型, 30, num3 > 0);
            }
            else
            {
                this.发送顶部公告("已经拥有更高特权,不可购买低级特权");
            }
        }

        public void 升级玛法特权(int 特权类型)
        {
            if (!Settings.可购买玛法特权)
            {
                return;
            }
            int num;
            switch (this.本期特权)
            {
                default:
                    return;
                case 3:
                    num = 12800;
                    break;
                case 4:
                    num = 28800;
                    break;
                case 5:
                    num = 28800;
                    break;
                case 6:
                    num = 6800;
                    break;
                case 7:
                    num = 58800;
                    break;
            }
            int num2;
            switch (特权类型)
            {
                default:
                    return;
                case 3:
                    num2 = 12800;
                    break;
                case 4:
                    num2 = 28800;
                    break;
                case 5:
                    num2 = 28800;
                    break;
                case 6:
                    num2 = 6800;
                    break;
                case 7:
                    num2 = 58800;
                    break;
            }
            if (num <= num2 && (this.本期特权 != 5 || 特权类型 != 4))
            {
                int num3;
                num3 = Math.Abs(num2 - num) + 100;
                if (this.元宝数量 < num3)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 8451
                    });
                    return;
                }
                主程.添加货币日志(this, "升级玛法特权", 游戏货币.元宝, -num3);
                this.元宝数量 -= (uint)num3;
                this.角色数据.消耗元宝.V += num3;
                this.玩家激活特权((byte)特权类型);
                if (num3 > 0)
                {
                    this.角色数据.发送邮件(new 邮件数据(null, "特权已升级", $"您的特权已升级,本次升级共花费{num3 / 100}元宝", null));
                }
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 65548,
                    第一参数 = 特权类型
                });
                this.网络连接?.发送封包(new 同步特权信息
                {
                    字节数组 = this.玛法特权描述()
                });
            }
            else
            {
                this.发送顶部公告("已经拥有更高特权,不可购买低级特权");
            }
        }

        public void 预定玛法特权(byte 特权类型)
        {
            if (this.剩余特权[特权类型] <= 0)
            {
                return;
            }
            if (this.本期特权 == 0)
            {
                this.玩家激活特权(特权类型);
                if ((this.剩余特权[特权类型] -= 30) <= 0)
                {
                    this.预定特权 = 0;
                }
            }
            else
            {
                this.预定特权 = 特权类型;
            }
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 65550,
                第一参数 = this.预定特权
            });
            this.网络连接?.发送封包(new 同步特权信息
            {
                字节数组 = this.玛法特权描述()
            });
        }

        public void 传永武技签到()
        {
            if (Settings.屏蔽传永武技)
            {
                return;
            }
            if (this.本期特权 != 4 && this.本期特权 != 5 && this.本期特权 != 7)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 9733
                });
                return;
            }
            if (this.传永武技 >= 42)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 9730
                });
                return;
            }
            if ((主程.当前时间.Date.AddDays(1.0) - this.武技日期.Date).TotalDays < 1.0)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 9730
                });
                return;
            }
            int num;
            num = this.签到领取(2, this.传永武技 + 1);
            if (num == 1)
            {
                this.传永武技++;
                this.武技日期 = 主程.当前时间.AddDays(1.0).Date;
                base.发送封包(new 传永武技回执
                {
                    回执编号 = 0
                });
            }
            else
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = num
                });
            }
        }

        private int 签到领取(int v1, int v2)
        {
            每日签到 每日签到;
            每日签到 = 每日签到.数据表.FirstOrDefault((每日签到 o) => o.签到类型 == v1 && o.签到天数 == v2);
            if (每日签到 != null && this.背包剩余 >= 1)
            {
                this.玩家获得物品(每日签到.奖励物品, 每日签到.奖励数量, "每日签到获得", 是否绑定: true);
                return 1;
            }
            return 0;
        }

        public void 领取每日签到()
        {
            if (Settings.屏蔽每日签到)
            {
                return;
            }
            if (this.每日签到 >= 28)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 9730
                });
                return;
            }
            if ((主程.当前时间.Date.AddDays(1.0) - this.签到日期.Date).TotalDays < 1.0)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 9730
                });
                return;
            }
            int num;
            num = this.签到领取(1, this.每日签到 + 1);
            if (num == 1)
            {
                this.每日签到++;
                this.签到日期 = 主程.当前时间.AddDays(1.0).Date;
                base.发送封包(new 每日签到应答
                {
                    签到次数 = 1
                });
            }
            else
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = num
                });
            }
        }

        public void 领取特权礼包(byte 特权类型, byte 礼包位置)
        {
            if (礼包位置 >= 28)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 领取特权礼包  错误: 礼包位置错误"));
                return;
            }
            switch (特权类型)
            {
                default:
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 65556
                    });
                    break;
                case 2:
                    if (this.上期特权 != 3 && this.上期特权 != 4 && this.上期特权 != 7)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 65556
                        });
                    }
                    else if ((this.上期记录 & (1 << (int)礼包位置)) == 0L)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 65546
                        });
                    }
                    else if (this.获得月卡奖励(this.本期特权, 礼包位置))
                    {
                        this.上期记录 &= (uint)(~(1 << (int)礼包位置));
                        this.网络连接?.发送封包(new 同步特权信息
                        {
                            字节数组 = this.玛法特权描述()
                        });
                    }
                    break;
                case 1:
                    if (this.本期特权 != 3 && this.本期特权 != 4 && this.本期特权 != 7)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 65556
                        });
                    }
                    else if ((主程.当前时间.Date.AddDays(1.0) - this.本期日期.Date).TotalDays < (double)(int)礼包位置)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 65547
                        });
                    }
                    else if ((this.本期记录 & (1 << (int)礼包位置)) == 0L)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 65546
                        });
                    }
                    else if (this.获得月卡奖励(this.本期特权, 礼包位置))
                    {
                        this.本期记录 &= (uint)(~(1 << (int)礼包位置));
                        this.网络连接?.发送封包(new 同步特权信息
                        {
                            字节数组 = this.玛法特权描述()
                        });
                    }
                    break;
            }
        }

        private bool 获得月卡奖励(byte 本期特权, byte 礼包位置)
        {
            if (this.背包剩余 == 0)
            {
                this.发送错误消息(6459);
                return false;
            }
            if (月卡奖励.数据表.TryGetValue(本期特权, out var value))
            {
                this.玩家获得物品(value.奖励物品[礼包位置], value.奖励数量[礼包位置], "领取特权礼包");
                return true;
            }
            return false;
        }

        public void 玩家使用称号(byte 称号编号)
        {
            if (!this.称号列表.ContainsKey(称号编号))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5377
                });
                return;
            }
            if (!游戏称号.数据表.TryGetValue(称号编号, out var value))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 5378
                });
                return;
            }
            if (this.当前称号 == 称号编号)
            {
                this.网络连接?.发送封包(new 同步装配称号
                {
                    对象编号 = this.地图编号,
                    称号编号 = 称号编号
                });
                return;
            }
            if (this.当前称号 != 0 && 游戏称号.数据表.TryGetValue(this.当前称号, out var value2) && !value2.始终生效)
            {
                this.战力加成.Remove(this.当前称号);
                base.属性加成.Remove(this.当前称号);
            }
            this.当前称号 = 称号编号;
            if (游戏称号.数据表.TryGetValue(称号编号, out var value3) && !value3.始终生效)
            {
                this.战力加成[称号编号] = value.称号战力;
                this.更新玩家战力();
                base.属性加成[称号编号] = value.称号属性;
                this.更新对象属性();
            }
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 1500,
                第一参数 = 称号编号
            });
            base.发送封包(new 同步装配称号
            {
                对象编号 = this.地图编号,
                称号编号 = 称号编号
            });
        }

        public void 玩家卸下称号()
        {
            if (this.当前称号 == 0)
            {
                return;
            }
            if (游戏称号.数据表.TryGetValue(this.当前称号, out var value) && !value.始终生效)
            {
                if (this.战力加成.Remove(this.当前称号))
                {
                    this.更新玩家战力();
                }
                if (base.属性加成.Remove(this.当前称号))
                {
                    this.更新对象属性();
                }
            }
            this.当前称号 = 0;
            base.发送封包(new 同步装配称号
            {
                对象编号 = this.地图编号
            });
        }

        public void 玩家整理背包(byte 背包类型)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            switch (背包类型)
            {
                case 1:
                    {
                        List<物品数据> list3;
                        list3 = this.角色背包.Values.ToList();
                        list3.Sort((物品数据 a, 物品数据 b) => b.物品编号.CompareTo(a.物品编号));
                        for (byte b6 = 0; b6 < list3.Count; b6++)
                        {
                            if (list3[b6].能否堆叠 && list3[b6].当前持久.V < list3[b6].最大持久.V)
                            {
                                for (int k = b6 + 1; k < list3.Count; k++)
                                {
                                    if (!list3[b6].是否上锁 && list3[b6].物品编号 == list3[k].物品编号 && list3[b6].是否绑定 == list3[k].是否绑定)
                                    {
                                        int num3;
                                        list3[b6].当前持久.V += (num3 = Math.Min(list3[b6].最大持久.V - list3[b6].当前持久.V, list3[k].当前持久.V));
                                        if ((list3[k].当前持久.V -= num3) <= 0)
                                        {
                                            list3[k].删除数据();
                                            list3.RemoveAt(k);
                                            k--;
                                        }
                                        if (list3[b6].当前持久.V >= list3[b6].最大持久.V)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        this.角色背包.Clear();
                        for (byte b7 = 0; b7 < list3.Count; b7++)
                        {
                            this.角色背包[b7] = list3[b7];
                            this.角色背包[b7].当前位置 = b7;
                        }
                        this.网络连接?.发送封包(new 同步背包信息
                        {
                            未知标志 = 0,
                            物品描述 = this.背包物品描述()
                        });
                        break;
                    }
                case 2:
                    {
                        List<物品数据> list2;
                        list2 = this.角色仓库.Values.ToList();
                        list2.Sort((物品数据 a, 物品数据 b) => b.物品编号.CompareTo(a.物品编号));
                        for (byte b4 = 0; b4 < list2.Count; b4++)
                        {
                            if (list2[b4].能否堆叠 && list2[b4].当前持久.V < list2[b4].最大持久.V)
                            {
                                for (int j = b4 + 1; j < list2.Count; j++)
                                {
                                    if (!list2[b4].是否上锁 && list2[b4].物品编号 == list2[j].物品编号 && list2[b4].是否绑定 == list2[j].是否绑定)
                                    {
                                        int num2;
                                        list2[b4].当前持久.V += (num2 = Math.Min(list2[b4].最大持久.V - list2[b4].当前持久.V, list2[j].当前持久.V));
                                        if ((list2[j].当前持久.V -= num2) <= 0)
                                        {
                                            list2[j].删除数据();
                                            list2.RemoveAt(j);
                                            j--;
                                        }
                                        if (list2[b4].当前持久.V >= list2[b4].最大持久.V)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        this.角色仓库.Clear();
                        for (byte b5 = 0; b5 < list2.Count; b5++)
                        {
                            this.角色仓库[b5] = list2[b5];
                            this.角色仓库[b5].当前位置 = b5;
                        }
                        this.网络连接?.发送封包(new 同步背包信息
                        {
                            未知标志 = 0,
                            物品描述 = this.仓库物品描述()
                        });
                        break;
                    }
                case 7:
                    {
                        List<物品数据> list;
                        list = this.角色资源包.Values.ToList();
                        list.Sort((物品数据 a, 物品数据 b) => b.物品编号.CompareTo(a.物品编号));
                        for (byte b2 = 0; b2 < list.Count; b2++)
                        {
                            if (list[b2].能否堆叠 && list[b2].当前持久.V < list[b2].最大持久.V)
                            {
                                for (int i = b2 + 1; i < list.Count; i++)
                                {
                                    if (!list[b2].是否上锁 && list[b2].物品编号 == list[i].物品编号 && list[b2].是否绑定 == list[i].是否绑定)
                                    {
                                        int num;
                                        list[b2].当前持久.V += (num = Math.Min(list[b2].最大持久.V - list[b2].当前持久.V, list[i].当前持久.V));
                                        if ((list[i].当前持久.V -= num) <= 0)
                                        {
                                            list[i].删除数据();
                                            list.RemoveAt(i);
                                            i--;
                                        }
                                        if (list[b2].当前持久.V >= list[b2].最大持久.V)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        this.角色资源包.Clear();
                        for (byte b3 = 0; b3 < list.Count; b3++)
                        {
                            this.角色资源包[b3] = list[b3];
                            this.角色资源包[b3].当前位置 = b3;
                        }
                        this.网络连接?.发送封包(new 同步背包信息
                        {
                            未知标志 = 0,
                            物品描述 = this.资源包物品描述()
                        });
                        break;
                    }
            }
        }

        public void 拾取脚下物品(int 物品编号)
        {
            if (Settings.使用新版内挂 && Settings.开启自动战斗 && this.自动挂机 != null && this.自动挂机.自动战斗)
            {
                return;
            }
            foreach (地图对象 item in this.当前地图[this.当前坐标].ToList())
            {
                if (item.对象类型 != 游戏对象类型.物品)
                {
                    continue;
                }
                bool flag;
                flag = item.IsMoney();
                if (this.背包剩余 > 0 || flag)
                {
                    物品实例 物品实例2;
                    物品实例2 = item as 物品实例;
                    if (物品实例2.地图编号 == 物品编号)
                    {
                        this.玩家拾取物品(物品实例2);
                        break;
                    }
                }
            }
        }

        public void 玩家拾取物品(物品实例 物品)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (物品.物品绑定 && !物品.物品归属.Contains(this.角色数据))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2310
                });
                return;
            }
            if (物品.强制时间 && 主程.当前时间 < 物品.归属时间)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2307
                });
                return;
            }
            if (物品.物品归属.Count != 0 && !物品.物品归属.Contains(this.角色数据) && 主程.当前时间 < 物品.归属时间)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2307
                });
                return;
            }
            if (物品.物品重量 != 0 && 物品.物品重量 > this.最大负重 - this.背包重量)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1863
                });
                return;
            }
            if (物品.默认持久 != 0 && this.背包剩余 <= 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return;
            }
            int num;
            num = 物品.堆叠数量;
            if (num < 0 || num >= int.MaxValue)
            {
                主程.添加系统日志($"玩家拾取物品 {物品} {num}");
                num = 1;
            }
            if (物品.物品编号 == 0)
            {
                this.网络连接?.发送封包(new 玩家拾取金币
                {
                    金币数量 = num
                });
                this.修改货币("+", 游戏货币.银币, (uint)num);
                主程.添加货币日志(this, "玩家拾取物品->" + 物品.物品模板?.物品名字, 游戏货币.银币, num);
                物品.物品转移处理();
                if (this.自动战斗 && this.开启收益检测)
                {
                    this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                }
                return;
            }
            if (物品.物品编号 == 1)
            {
                this.网络连接?.发送封包(new 玩家拾取金币
                {
                    金币数量 = num
                });
                this.修改货币("+", 游戏货币.金币, (uint)num);
                主程.添加货币日志(this, "玩家拾取物品->" + 物品.物品模板?.物品名字, 游戏货币.金币, num);
                物品.物品转移处理();
                if (this.自动战斗 && this.开启收益检测)
                {
                    this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                }
                return;
            }
            if (物品.物品数据 == null && 物品.物品模板.贵重物品 && this.角色数据.当前队伍 != null && this.角色数据.当前队伍.拾取方式 == 2 && 主程.当前时间 < 物品.归属时间)
            {
                物品数据 拍卖物品;
                拍卖物品 = ((!(物品.物品模板 is 游戏装备 模板)) ? ((物品.持久类型 == 物品持久分类.容器) ? new 物品数据(物品.物品模板, this.角色数据, 0, 0, 0, 绑定: false, 物品.掉落对象?.对象名字) : ((物品.持久类型 != 物品持久分类.堆叠) ? new 物品数据(物品.物品模板, this.角色数据, 0, 0, 物品.默认持久, 绑定: false, 物品.掉落对象?.对象名字) : new 物品数据(物品.物品模板, this.角色数据, 0, 0, 物品.堆叠数量, 绑定: false, 物品.掉落对象?.对象名字))) : new 装备数据(模板, this.角色数据, 0, 0, 随机生成: true, 绑定: false, 物品.掉落对象?.对象名字));
                物品.物品转移处理();
                this.角色数据.当前队伍.添加拍卖物品(拍卖物品, this.角色数据);
                if (this.自动战斗 && this.开启收益检测)
                {
                    this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                }
                return;
            }
            if (物品.允许堆叠)
            {
                int num2;
                num2 = ((物品.物品数据 == null) ? 1 : 物品.物品数据.当前持久.V);
                foreach (KeyValuePair<byte, 物品数据> item in this.角色背包)
                {
                    物品数据 value;
                    value = item.Value;
                    if (value.物品编号 == 物品.物品编号 && value.当前持久.V + num2 <= value.最大持久.V && ((物品.物品数据 == null) ? (!value.是否绑定) : (value.是否绑定 == 物品.物品数据.是否绑定)))
                    {
                        value.当前持久.V += num2;
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = value.字节描述()
                        });
                        物品.物品消失处理();
                        if (this.自动战斗 && this.开启收益检测)
                        {
                            this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                        }
                        return;
                    }
                }
            }
            byte b;
            b = 0;
            while (b < this.背包大小)
            {
                if (this.角色背包.ContainsKey(b))
                {
                    b++;
                    continue;
                }
                if (物品.物品数据 != null)
                {
                    this.角色背包[b] = 物品.物品数据;
                    物品.物品数据.物品位置.V = b;
                    物品.物品数据.物品容器.V = 1;
                }
                else if (物品.物品模板 is 游戏装备 模板2)
                {
                    this.角色背包[b] = new 装备数据(模板2, this.角色数据, 1, b, 随机生成: true, 绑定: false, 物品.掉落对象?.对象名字);
                }
                else if (物品.持久类型 == 物品持久分类.容器)
                {
                    this.角色背包[b] = new 物品数据(物品.物品模板, this.角色数据, 1, b, 0, 绑定: false, 物品.掉落对象?.对象名字);
                }
                else if (物品.持久类型 == 物品持久分类.堆叠)
                {
                    this.角色背包[b] = new 物品数据(物品.物品模板, this.角色数据, 1, b, 物品.堆叠数量, 绑定: false, 物品.掉落对象?.对象名字);
                }
                else
                {
                    this.角色背包[b] = new 物品数据(物品.物品模板, this.角色数据, 1, b, 物品.默认持久, 绑定: false, 物品.掉落对象?.对象名字);
                }
                this.网络连接?.发送封包(new 玩家拾取物品
                {
                    物品描述 = this.角色背包[b].字节描述(),
                    角色编号 = this.地图编号
                });
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = this.角色背包[b].字节描述()
                });
                主程.添加物品日志(this, "玩家拾取物品", this.角色背包[b], (!this.角色背包[b].能否堆叠) ? 1 : this.角色背包[b].当前持久.V, $"掉落对象:{物品.掉落对象}");
                物品.物品转移处理();
                if (this.自动战斗 && this.开启收益检测)
                {
                    this.收益间隔 = 主程.当前时间.AddSeconds(this.收益检测时间);
                }
                break;
            }
        }

        public void 玩家丢弃物品(byte 背包类型, byte 物品位置, ushort 丢弃数量)
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || this.当前等级 <= 7 || 背包类型 != 1 || !this.角色背包.TryGetValue(物品位置, out var v) || (v is 装备数据 装备数据 && 装备数据.灵魂绑定.V))
            {
                return;
            }
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
                new 物品实例(v.物品模板, v, this.当前地图, this.当前坐标, new HashSet<角色数据> { this.角色数据 }, 0, 物品绑定: true, this);
            }
            else
            {
                new 物品实例(v.物品模板, v, this.当前地图, this.当前坐标, new HashSet<角色数据>(), 0, 物品绑定: false, this);
            }
            this.角色背包.Remove(v.物品位置.V);
            this.网络连接?.发送封包(new 删除玩家物品
            {
                背包类型 = 背包类型,
                物品位置 = 物品位置
            });
            主程.添加物品日志(this, "玩家丢弃物品", v, 1);
            if (Settings.开启成就系统)
            {
                this.成就变量变更(AchievementVariables.DestoryItemCount, 1);
            }
        }

        public void 玩家拆分物品(byte 当前背包, byte 物品位置, ushort 拆分数量, byte 目标背包, byte 目标位置)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3 && 当前背包 == 1 && this.角色背包.TryGetValue(物品位置, out var v) && 目标背包 == 1 && 目标位置 < this.背包大小 && 物品位置 != 目标位置 && v != null && v.持久类型 == 物品持久分类.堆叠 && 拆分数量 > 0 && v.当前持久.V > 拆分数量 && !this.角色背包.TryGetValue(目标位置, out var _))
            {
                if (v.是否上锁)
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1890
                    });
                    return;
                }
                v.当前持久.V -= 拆分数量;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = v.字节描述()
                });
                this.角色背包[目标位置] = new 物品数据(v.物品模板, this.角色数据, 目标背包, 目标位置, 拆分数量, v.是否绑定, v.掉落怪物.V);
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = this.角色背包[目标位置].字节描述()
                });
                主程.添加物品日志(this, "玩家拆分物品", this.角色背包[目标位置], 拆分数量, $"原物品索引:{v.数据索引}");
            }
        }

        public void 玩家分解物品(byte 背包类型, byte 物品位置, byte 分解数量)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                物品数据 v;
                if (背包类型 != 1)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家分解物品.  错误: 背包类型错误."));
                }
                else if (!this.角色背包.TryGetValue(物品位置, out v))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                }
                else if (v.能否分解 && !v.是否绑定 && !v.是否上锁)
                {
                    if (v is 装备数据 装备数据)
                    {
                        switch (装备数据.物品类型)
                        {
                            case 物品使用分类.项链:
                                if (装备数据.随机属性.Count > 0 && 装备数据.随机属性.Any((随机属性 att) => att.对应属性 == 游戏对象属性.幸运等级 && att.属性数值 >= 2))
                                {
                                    this.发送顶部公告("运2以上项链不可分解");
                                    return;
                                }
                                break;
                            case 物品使用分类.武器:
                                if (装备数据.随机属性.Count > 0 && 装备数据.随机属性.Any((随机属性 att) => att.对应属性 == 游戏对象属性.幸运等级 && att.属性数值 >= 6))
                                {
                                    this.发送顶部公告("运6以上武器不可分解");
                                    return;
                                }
                                break;
                        }
                    }
                    物品数据 物品数据;
                    物品数据 = this.角色背包[物品位置];
                    if (!物品分解.数据表.TryGetValue(物品数据.物品编号, out var value))
                    {
                        return;
                    }
                    if (物品数据.能否堆叠)
                    {
                        this.消耗背包物品(1, 物品数据, "物品分解消耗");
                    }
                    else
                    {
                        this.消耗背包物品(物品数据.当前持久.V, 物品数据, "物品分解消耗");
                    }
                    int 分解物一;
                    分解物一 = 0;
                    int num;
                    num = 0;
                    int 分解物二;
                    分解物二 = 0;
                    int num2;
                    num2 = 0;
                    int 分解物三;
                    分解物三 = 0;
                    int num3;
                    num3 = 0;
                    if (value.触发脚本 == 1)
                    {
                        this.CallDefaultNPC(DefaultNPCType.DeCompose, true, 物品数据.物品编号);
                        return;
                    }
                    if (掉落分组.数据表.TryGetValue(value.掉落分组一, out var value2) && 计算类.计算概率((float)value.掉落概率一 / 1000000f))
                    {
                        分解物一 = value2.物品编号;
                        num = 主程.随机数.Next(value2.最小数量, value2.最大数量);
                        this.玩家获得物品(value2.物品编号, num, "分解物品获得");
                    }
                    if (掉落分组.数据表.TryGetValue(value.掉落分组二, out var value3) && 计算类.计算概率((float)value.掉落概率二 / 1000000f))
                    {
                        分解物二 = value3.物品编号;
                        num2 = 主程.随机数.Next(value3.最小数量, value3.最大数量);
                        this.玩家获得物品(value3.物品编号, num2, "分解物品获得");
                    }
                    if (掉落分组.数据表.TryGetValue(value.掉落分组三, out var value4) && 计算类.计算概率((float)value.掉落概率三 / 1000000f))
                    {
                        分解物三 = value4.物品编号;
                        num3 = 主程.随机数.Next(value4.最小数量, value4.最大数量);
                        this.玩家获得物品(value4.物品编号, num3, "分解物品获得");
                    }
                    base.发送封包(new 分解物品应答
                    {
                        分解数量 = 分解数量,
                        分解物品 = 物品数据.物品编号,
                        分解物一 = 分解物一,
                        物品数一 = num,
                        分解物二 = 分解物二,
                        物品数二 = num2,
                        分解物三 = 分解物三,
                        物品数三 = num3
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1876
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1876
                });
            }
        }

        public void 玩家转移物品(byte 当前背包, byte 当前位置, byte 目标背包, byte 目标位置)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || (当前背包 == 0 && 当前位置 >= 16) || (目标背包 == 0 && 目标位置 >= 16) || (当前背包 == 1 && 当前位置 >= this.背包大小) || (目标背包 == 1 && 目标位置 >= this.背包大小) || (当前背包 == 2 && 当前位置 >= this.仓库大小) || (目标背包 == 2 && 目标位置 >= this.仓库大小) || (当前背包 == 7 && 当前位置 >= this.资源包大小) || (目标背包 == 7 && 目标位置 >= this.资源包大小))
            {
                return;
            }
            if ((目标背包 != 2 && 当前背包 != 2) || (this.对话守卫 != null && (this.对话守卫 == null || 计算类.网格距离(this.当前坐标, this.对话守卫.当前坐标) <= 11)))
            {
                if (当前背包 == 2 && this.角色数据.锁定仓库.V && !this.验证密码)
                {
                    this.开始验证密码();
                    return;
                }
                物品数据 物品数据;
                物品数据 = null;
                if (当前背包 == 0)
                {
                    物品数据 = (this.角色装备.TryGetValue(当前位置, out var v) ? v : null);
                }
                if (当前背包 == 1)
                {
                    物品数据 = (this.角色背包.TryGetValue(当前位置, out var v2) ? v2 : null);
                }
                if (当前背包 == 2)
                {
                    物品数据 = (this.角色仓库.TryGetValue(当前位置, out var v3) ? v3 : null);
                }
                if (当前背包 == 5)
                {
                    物品数据 = this.角色数据.挂载物品.V;
                }
                if (当前背包 == 7)
                {
                    物品数据 = (this.角色资源包.TryGetValue(当前位置, out var v4) ? v4 : null);
                }
                物品数据 物品数据2;
                物品数据2 = null;
                if (目标背包 == 0)
                {
                    物品数据2 = (this.角色装备.TryGetValue(目标位置, out var v5) ? v5 : null);
                }
                if (目标背包 == 1)
                {
                    物品数据2 = (this.角色背包.TryGetValue(目标位置, out var v6) ? v6 : null);
                }
                if (目标背包 == 2)
                {
                    物品数据2 = (this.角色仓库.TryGetValue(目标位置, out var v7) ? v7 : null);
                }
                if (目标背包 == 5)
                {
                    物品数据2 = this.角色数据.挂载物品.V;
                }
                if (目标背包 == 7)
                {
                    物品数据2 = (this.角色资源包.TryGetValue(目标位置, out var v8) ? v8 : null);
                }
                if ((Settings.资源包只能放材料 && ((当前背包 == 7 && 物品数据2 != null && !物品数据2.资源物品) || (目标背包 == 7 && 物品数据 != null && !物品数据.资源物品))) || (当前背包 == 5 && 物品数据2 != null && 物品数据2.物品类型 != 物品使用分类.经验容器) || (目标背包 == 5 && 物品数据 != null && 物品数据.物品类型 != 物品使用分类.经验容器) || (物品数据 == null && 物品数据2 == null) || (当前背包 == 0 && 目标背包 == 0) || (当前背包 == 0 && 目标背包 == 2) || (当前背包 == 2 && 目标背包 == 0) || (物品数据 != null && 当前背包 == 0 && (物品数据 as 装备数据).禁止卸下) || (物品数据2 != null && 目标背包 == 0 && (物品数据2 as 装备数据).禁止卸下) || (物品数据 != null && 目标背包 == 0 && (!(物品数据 is 装备数据 装备数据) || 装备数据.需要等级 > this.当前等级 || (装备数据.需要性别 != 0 && 装备数据.需要性别 != this.角色性别) || (装备数据.需要职业 != 游戏对象职业.通用 && 装备数据.需要职业 != this.角色职业) || 装备数据.需要攻击 > this[游戏对象属性.最大攻击] || 装备数据.需要魔法 > this[游戏对象属性.最大魔法] || 装备数据.需要道术 > this[游戏对象属性.最大道术] || 装备数据.需要刺术 > this[游戏对象属性.最大刺术] || 装备数据.需要弓术 > this[游戏对象属性.最大弓术] || (目标位置 == 0 && 装备数据.物品重量 > this.最大腕力) || (目标位置 != 0 && 装备数据.物品重量 - 物品数据2?.物品重量 > this.最大穿戴 - this.装备重量) || (目标位置 == 0 && 装备数据.物品类型 != 物品使用分类.武器) || (目标位置 == 1 && 装备数据.物品类型 != 物品使用分类.衣服) || (目标位置 == 2 && 装备数据.物品类型 != 物品使用分类.披风) || (目标位置 == 3 && 装备数据.物品类型 != 物品使用分类.头盔) || (目标位置 == 4 && 装备数据.物品类型 != 物品使用分类.护肩) || (目标位置 == 5 && 装备数据.物品类型 != 物品使用分类.护腕) || (目标位置 == 6 && 装备数据.物品类型 != 物品使用分类.腰带) || (目标位置 == 7 && 装备数据.物品类型 != 物品使用分类.鞋子) || (目标位置 == 8 && 装备数据.物品类型 != 物品使用分类.项链) || (目标位置 == 13 && 装备数据.物品类型 != 物品使用分类.勋章) || (目标位置 == 14 && 装备数据.物品类型 != 物品使用分类.玉佩) || (目标位置 == 15 && 装备数据.物品类型 != 物品使用分类.战具) || (目标位置 == 9 && 装备数据.物品类型 != 物品使用分类.戒指) || (目标位置 == 10 && 装备数据.物品类型 != 物品使用分类.戒指) || (目标位置 == 11 && 装备数据.物品类型 != 物品使用分类.手镯) || (目标位置 == 12 && 装备数据.物品类型 != 物品使用分类.手镯))) || (物品数据2 != null && 当前背包 == 0 && (!(物品数据2 is 装备数据 装备数据2) || 装备数据2.需要等级 > this.当前等级 || (装备数据2.需要性别 != 0 && 装备数据2.需要性别 != this.角色性别) || (装备数据2.需要职业 != 游戏对象职业.通用 && 装备数据2.需要职业 != this.角色职业) || 装备数据2.需要攻击 > this[游戏对象属性.最大攻击] || 装备数据2.需要魔法 > this[游戏对象属性.最大魔法] || 装备数据2.需要道术 > this[游戏对象属性.最大道术] || 装备数据2.需要刺术 > this[游戏对象属性.最大刺术] || 装备数据2.需要弓术 > this[游戏对象属性.最大弓术] || (当前位置 == 0 && 装备数据2.物品重量 > this.最大腕力) || (当前位置 != 0 && 装备数据2.物品重量 - 物品数据?.物品重量 > this.最大穿戴 - this.装备重量) || (当前位置 == 0 && 装备数据2.物品类型 != 物品使用分类.武器) || (当前位置 == 1 && 装备数据2.物品类型 != 物品使用分类.衣服) || (当前位置 == 2 && 装备数据2.物品类型 != 物品使用分类.披风) || (当前位置 == 3 && 装备数据2.物品类型 != 物品使用分类.头盔) || (当前位置 == 4 && 装备数据2.物品类型 != 物品使用分类.护肩) || (当前位置 == 5 && 装备数据2.物品类型 != 物品使用分类.护腕) || (当前位置 == 6 && 装备数据2.物品类型 != 物品使用分类.腰带) || (当前位置 == 7 && 装备数据2.物品类型 != 物品使用分类.鞋子) || (当前位置 == 8 && 装备数据2.物品类型 != 物品使用分类.项链) || (当前位置 == 13 && 装备数据2.物品类型 != 物品使用分类.勋章) || (当前位置 == 14 && 装备数据2.物品类型 != 物品使用分类.玉佩) || (当前位置 == 15 && 装备数据2.物品类型 != 物品使用分类.战具) || (当前位置 == 9 && 装备数据2.物品类型 != 物品使用分类.戒指) || (当前位置 == 10 && 装备数据2.物品类型 != 物品使用分类.戒指) || (当前位置 == 11 && 装备数据2.物品类型 != 物品使用分类.手镯) || (当前位置 == 12 && 装备数据2.物品类型 != 物品使用分类.手镯))))
                {
                    return;
                }
                if (物品数据 != null && 物品数据2 != null && !物品数据.是否上锁 && !物品数据2.是否上锁 && 物品数据.能否堆叠 && 物品数据2.物品编号 == 物品数据.物品编号 && 物品数据.堆叠上限 > 物品数据.当前持久.V && 物品数据2.堆叠上限 > 物品数据2.当前持久.V && 物品数据.是否绑定 == 物品数据2.是否绑定)
                {
                    int num;
                    num = Math.Min(物品数据.当前持久.V, 物品数据2.堆叠上限 - 物品数据2.当前持久.V);
                    物品数据2.当前持久.V += num;
                    物品数据.当前持久.V -= num;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 物品数据2.字节描述()
                    });
                    if (物品数据.当前持久.V <= 0)
                    {
                        物品数据.删除数据();
                        switch (当前背包)
                        {
                            case 7:
                                this.角色资源包.Remove(当前位置);
                                break;
                            case 2:
                                this.角色仓库.Remove(当前位置);
                                break;
                            case 1:
                                this.角色背包.Remove(当前位置);
                                break;
                        }
                        this.网络连接?.发送封包(new 删除玩家物品
                        {
                            背包类型 = 当前背包,
                            物品位置 = 当前位置
                        });
                    }
                    else
                    {
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = 物品数据.字节描述()
                        });
                    }
                    return;
                }
                if (物品数据 != null)
                {
                    switch (当前背包)
                    {
                        case 0:
                            this.角色装备.Remove(当前位置);
                            break;
                        case 1:
                            this.角色背包.Remove(当前位置);
                            break;
                        case 2:
                            this.角色仓库.Remove(当前位置);
                            break;
                        case 5:
                            this.角色数据.挂载物品.V = null;
                            break;
                        case 7:
                            this.角色资源包.Remove(当前位置);
                            break;
                    }
                    物品数据.物品容器.V = 目标背包;
                    物品数据.物品位置.V = 目标位置;
                }
                if (物品数据2 != null)
                {
                    switch (目标背包)
                    {
                        case 0:
                            this.角色装备.Remove(目标位置);
                            break;
                        case 1:
                            this.角色背包.Remove(目标位置);
                            break;
                        case 2:
                            this.角色仓库.Remove(目标位置);
                            break;
                        case 5:
                            this.角色数据.挂载物品.V = null;
                            break;
                        case 7:
                            this.角色资源包.Remove(目标位置);
                            break;
                    }
                    物品数据2.物品容器.V = 当前背包;
                    物品数据2.物品位置.V = 当前位置;
                }
                if (物品数据 != null)
                {
                    switch (目标背包)
                    {
                        case 0:
                            this.角色装备[目标位置] = 物品数据 as 装备数据;
                            break;
                        case 1:
                            this.角色背包[目标位置] = 物品数据;
                            break;
                        case 2:
                            this.角色仓库[目标位置] = 物品数据;
                            break;
                        case 5:
                            this.角色数据.挂载物品.V = 物品数据;
                            break;
                        case 7:
                            this.角色资源包[目标位置] = 物品数据;
                            break;
                    }
                }
                if (物品数据2 != null)
                {
                    switch (当前背包)
                    {
                        case 0:
                            this.角色装备[当前位置] = 物品数据2 as 装备数据;
                            break;
                        case 1:
                            this.角色背包[当前位置] = 物品数据2;
                            break;
                        case 2:
                            this.角色仓库[当前位置] = 物品数据2;
                            break;
                        case 5:
                            this.角色数据.挂载物品.V = 物品数据2;
                            break;
                        case 7:
                            this.角色资源包[当前位置] = 物品数据2;
                            break;
                    }
                }
                this.网络连接?.发送封包(new 玩家转移物品
                {
                    原有容器 = 当前背包,
                    目标容器 = 目标背包,
                    原有位置 = 当前位置,
                    目标位置 = 目标位置
                });
                if (目标背包 == 0)
                {
                    this.玩家穿卸装备((装备穿戴部位)目标位置, (装备数据)物品数据2, (装备数据)物品数据);
                }
                else if (当前背包 == 0)
                {
                    this.玩家穿卸装备((装备穿戴部位)当前位置, (装备数据)物品数据, (装备数据)物品数据2);
                }
                this.UpdateQuestsProgress();
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 3330
                });
            }
        }

        private bool ProcessConsumableRecoveryHP(物品数据 item)
        {
            float num;
            num = 1f;
            if (this.Buff列表.ContainsKey(26380) || this.Buff列表.ContainsKey(26381) || this.Buff列表.ContainsKey(26382) || this.Buff列表.ContainsKey(26383) || this.Buff列表.ContainsKey(26384) || this.Buff列表.ContainsKey(26385))
            {
                num += 0.3f;
            }
            this.回血总量 += (int)((float)item.GetProp(物品使用属性.回复基数, 15) * num);
            return true;
        }

        private bool ProcessConsumableRecoveryMP(物品数据 item)
        {
            float num;
            num = 1f;
            if (this.Buff列表.ContainsKey(26380) || this.Buff列表.ContainsKey(26381) || this.Buff列表.ContainsKey(26382) || this.Buff列表.ContainsKey(26383) || this.Buff列表.ContainsKey(26384) || this.Buff列表.ContainsKey(26385))
            {
                num += 0.3f;
            }
            this.回魔总量 += (int)((float)item.GetProp(物品使用属性.回复基数, 15) * num);
            return true;
        }

        private bool ProcessConsumableMedicine(物品数据 item)
        {
            float num;
            num = 0f;
            if (this.Buff列表.ContainsKey(26390) || this.Buff列表.ContainsKey(26391) || this.Buff列表.ContainsKey(26392) || this.Buff列表.ContainsKey(26393) || this.Buff列表.ContainsKey(26394) || this.Buff列表.ContainsKey(26395))
            {
                num += 0.1f;
            }
            this.药品回血 = 主程.当前时间.AddSeconds(item.GetProp(物品使用属性.回复时间, 1));
            this.当前体力 += (int)Math.Max((float)item.GetProp(物品使用属性.瞬回体力, 30) * (1f + num + (float)this[游戏对象属性.药品回血] / 10000f), 0f);
            this.当前魔力 += (int)Math.Max((float)item.GetProp(物品使用属性.瞬回魔力, 40) * (1f + num + (float)this[游戏对象属性.药品回魔] / 10000f), 0f);
            return true;
        }

        private bool ProcessConsumableStack(物品数据 item)
        {
            if (this.背包大小 - this.角色背包.Count < 5)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return false;
            }
            if (item.解包物品编号.HasValue && 游戏物品.数据表.TryGetValue(item.解包物品编号.Value, out var value))
            {
                byte b;
                b = 0;
                byte b2;
                b2 = 0;
                while (b < this.背包大小 && b2 < 6)
                {
                    if (!this.角色背包.ContainsKey(b))
                    {
                        this.角色背包[b] = new 物品数据(value, this.角色数据, 1, b, 1, 绑定: false, this.对象名字 + "-" + item.物品名字 + "解包");
                        主程.添加物品日志(this, "解包获得物品", this.角色背包[b], 1, "包:" + item.物品名字);
                        this.网络连接?.发送封包(new 玩家物品变动
                        {
                            物品描述 = this.角色背包[b].字节描述()
                        });
                        b2++;
                    }
                    b++;
                }
                return true;
            }
            return false;
        }

        private bool ProcessConsumableRandomTeleport(物品数据 item)
        {
            if (this.当前地图.地图模板.随机次数 > 0 && this.随机次数 >= this.当前地图.地图模板.随机次数)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1903
                });
                return false;
            }
            this.随机次数++;
            Point point;
            point = this.当前地图.随机传送(this.当前坐标);
            if (point != default(Point))
            {
                this.玩家切换地图(this.当前地图, 地图区域类型.未知区域, point);
                return true;
            }
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 776
            });
            return false;
        }

        private bool ProcessConsumableTreasure(物品数据 item)
        {
            Dictionary<物品使用属性, int> dictionary;
            dictionary = new Dictionary<物品使用属性, int>();
            int prop;
            prop = item.GetProp(物品使用属性.金币数量);
            int prop2;
            prop2 = item.GetProp(物品使用属性.双倍经验数量);
            游戏宝箱[] array;
            array = this.FilterItemTreasures(item.对应模板.V.宝箱物品);
            if (array != null && (item.HasProp(物品使用属性.宝箱几率) || array.Length != 0))
            {
                dictionary.Add(物品使用属性.宝箱几率, item.GetProp(物品使用属性.宝箱几率, 100));
            }
            if (item.HasProp(物品使用属性.双倍经验几率) || prop2 > 0)
            {
                dictionary.Add(物品使用属性.双倍经验几率, item.GetProp(物品使用属性.双倍经验几率, 100));
            }
            if (item.HasProp(物品使用属性.金币几率) || prop > 0)
            {
                dictionary.Add(物品使用属性.金币几率, item.GetProp(物品使用属性.金币几率, 100));
            }
            if (dictionary.Count == 0)
            {
                return false;
            }
            dictionary.OrderBy((KeyValuePair<物品使用属性, int> x) => x.Value).ToArray();
            int num;
            num = 主程.随机数.Next(0, 100);
            foreach (KeyValuePair<物品使用属性, int> item2 in dictionary)
            {
                if (item2.Value < num)
                {
                    continue;
                }
                switch (item2.Key)
                {
                    case 物品使用属性.宝箱几率:
                        if (array.Length != 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (游戏物品.检索表.TryGetValue(array[i].物品名字, out var value))
                                {
                                    int num2;
                                    num2 = array[i].物品数量;
                                    if (num2 < 0 || num2 >= int.MaxValue)
                                    {
                                        主程.添加系统日志($"Treasure {array[i].物品名字} {num2}");
                                        num2 = 1;
                                    }
                                    if (value.物品编号 == 0)
                                    {
                                        this.修改货币("+", 游戏货币.银币, (uint)num2);
                                        主程.添加货币日志(this, "玩家Treasure物品->" + value.物品名字, 游戏货币.金币, num2);
                                        continue;
                                    }
                                    if (value.物品编号 == 1)
                                    {
                                        this.修改货币("+", 游戏货币.金币, (uint)num2);
                                        主程.添加货币日志(this, "玩家Treasure物品", 游戏货币.金币, num2);
                                        continue;
                                    }
                                    if (value.物品编号 == 10)
                                    {
                                        this.玩家增加经验(null, array[i].物品数量);
                                        continue;
                                    }
                                    if (value.物品编号 == 11)
                                    {
                                        this.双倍经验 += array[i].物品数量;
                                        continue;
                                    }
                                    if (!this.角色数据.尝试获取背包空余格子(out var location))
                                    {
                                        this.角色数据.发送邮件(null, "获得物品", "您使用了[" + item.物品名字 + "]由于包裹已满，系统以邮件的形式发放，请及时查收！", value.物品编号, array[i].物品数量);
                                        continue;
                                    }
                                    物品数据 value2;
                                    value2 = ((value is 游戏装备 模板) ? new 装备数据(模板, this.角色数据, 1, location, 随机生成: false, 绑定: false, this.对象名字 + "-宝箱获得物品") : new 物品数据(value, this.角色数据, 1, location, array[i].物品数量, 绑定: false, this.对象名字 + "-宝箱获得物品"));
                                    this.角色背包[location] = value2;
                                    this.网络连接?.发送封包(new 玩家物品变动
                                    {
                                        物品描述 = this.角色背包[location].字节描述()
                                    });
                                    主程.添加物品日志(this, "宝箱获得物品", this.角色背包[location], 1);
                                }
                            }
                            break;
                        }
                        return false;
                    case 物品使用属性.双倍经验几率:
                        this.双倍经验 += prop2;
                        break;
                    case 物品使用属性.金币几率:
                        this.修改货币("+", 游戏货币.金币, (uint)prop);
                        主程.添加货币日志(this, "玩家使用物品", 游戏货币.金币, prop);
                        break;
                }
            }
            return true;
        }

        public 游戏宝箱[] FilterItemTreasures(IEnumerable<游戏宝箱> items)
        {
            return (from x in items?.Where((游戏宝箱 x) => x.判断职业 == this.角色职业 || x.判断职业 >= 游戏对象职业.通用)
                    orderby (x.获得几率 != 0f) ? x.获得几率 : 100f
                    where 计算类.计算概率(x.获得几率)
                    select x).ToArray();
        }

        private bool ProcessConsumableTownTeleport(物品数据 item)
        {
            int prop;
            prop = item.GetProp(物品使用属性.地图编号, 147);
            地图实例 地图实例2;
            地图实例2 = ((this.当前地图.地图编号 == prop) ? this.当前地图 : 地图处理网关.已分配地图(prop));
            if (地图实例2 == null)
            {
                return false;
            }
            this.玩家切换地图(地图实例2, 地图区域类型.复活区域);
            return true;
        }

        private bool ProcessConsumableBlessing(物品数据 item, byte 固定几率)
        {
            if (!this.角色装备.TryGetValue(0, out var v))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1927
                });
                return false;
            }
            if (v.幸运等级.V >= 7)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1843
                });
                return false;
            }
            int num;
            num = 0;
            num = v.幸运等级.V switch
            {
                0 => (Settings.祝福油几率0级 > 0) ? (Settings.祝福油几率0级 * 100) : 8000,
                1 => (Settings.祝福油几率1级 > 0) ? (Settings.祝福油几率1级 * 100) : 1000,
                2 => (Settings.祝福油几率2级 > 0) ? (Settings.祝福油几率2级 * 100) : 800,
                3 => (Settings.祝福油几率3级 > 0) ? (Settings.祝福油几率3级 * 100) : 800,
                4 => (Settings.祝福油几率4级 > 0) ? (Settings.祝福油几率4级 * 100) : 500,
                5 => (Settings.祝福油几率5级 > 0) ? (Settings.祝福油几率5级 * 100) : 300,
                6 => (Settings.祝福油几率6级 > 0) ? (Settings.祝福油几率6级 * 100) : 300,
                _ => 8000,
            };
            if (固定几率 > 0)
            {
                num = 固定几率;
            }
            int num2;
            num2 = 主程.随机数.Next(10000);
            if (num2 < num)
            {
                v.幸运等级.V++;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = v.字节描述()
                });
                this.网络连接?.发送封包(new 武器幸运变化
                {
                    幸运变化 = 1
                });
                base.属性加成[v] = v.装备属性;
                this.更新对象属性();
                if (v.幸运等级.V >= 5)
                {
                    网络服务网关.发送公告($"[{this.对象名字}] 成功将 [{v.物品名字}] 升到幸运 {v.幸运等级.V} 级.");
                }
            }
            else if (num2 >= 9500 && v.幸运等级.V > -9)
            {
                v.幸运等级.V--;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = v.字节描述()
                });
                this.网络连接?.发送封包(new 武器幸运变化
                {
                    幸运变化 = -1
                });
                base.属性加成[v] = v.装备属性;
                this.更新对象属性();
            }
            else
            {
                this.网络连接?.发送封包(new 武器幸运变化
                {
                    幸运变化 = 0
                });
            }
            return true;
        }

        private bool ProcessConsumableSwitchSkill(物品数据 item)
        {
            if (!this.角色装备.TryGetValue(0, out var v))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1927
                });
                return false;
            }
            if (!v.双铭文栏.V)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1926
                });
                return false;
            }
            if (v.第一铭文 != null)
            {
                this.玩家装卸铭文(v.第一铭文.技能编号, 0);
            }
            if (v.第二铭文 != null)
            {
                this.玩家装卸铭文(v.第二铭文.技能编号, 0);
            }
            v.当前铭栏.V = ((v.当前铭栏.V == 0) ? ((byte)1) : ((byte)0));
            if (v.第一铭文 != null)
            {
                this.玩家装卸铭文(v.第一铭文.技能编号, v.第一铭文.铭文编号);
            }
            if (v.第二铭文 != null)
            {
                this.玩家装卸铭文(v.第二铭文.技能编号, v.第二铭文.铭文编号);
            }
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = v.字节描述()
            });
            this.网络连接?.发送封包(new 双铭文位切换
            {
                当前栏位 = v.当前铭栏.V,
                第一铭文 = (v.第一铭文?.铭文索引 ?? 0),
                第二铭文 = (v.第二铭文?.铭文索引 ?? 0)
            });
            return true;
        }

        public bool 根据物品编号获得货币(int 编号, int 数量)
        {
            if (数量 <= 0)
            {
                return false;
            }
            switch (编号)
            {
                case 1:
                    this.修改货币("+", 游戏货币.金币, (uint)数量);
                    break;
                case 0:
                    this.修改货币("+", 游戏货币.银币, (uint)数量);
                    break;
                default:
                    return false;
                case 11:
                    this.双倍经验 += 数量;
                    break;
                case 10:
                    this.玩家增加经验(null, 数量);
                    break;
            }
            return true;
        }

        public bool 获得坐骑(byte mountId)
        {
            if (!this.坐骑列表.Contains(mountId))
            {
                this.坐骑列表.Add(mountId);
                this.当前坐骑 = mountId;
                this.发送坐骑描述();
                return true;
            }
            return false;
        }

        public bool ProcessConsumableAdquireMount(物品数据 item)
        {
            return this.获得坐骑((byte)item.GetProp(物品使用属性.坐骑编号, 2));
        }

        public bool 处理物品使用添加BUFF(物品数据 item)
        {
            base.添加Buff时处理(item.技能编号, this);
            return true;
        }

        public bool 处理物品使用减少PK值(物品数据 item)
        {
            this.PK值惩罚 -= item.GetProp(物品使用属性.固定基数);
            return true;
        }

        public bool 处理物品获得称号(物品数据 item)
        {
            byte b;
            b = (byte)item.GetProp(物品使用属性.称号编号);
            if (this.称号列表.ContainsKey(b))
            {
                return false;
            }
            return this.玩家获得称号(b);
        }

        public bool 处理物品召唤下属(物品数据 item)
        {
            /* 备份
                         bool flag;
            flag = false;
            switch (this.角色职业)
            {
                case 游戏对象职业.战士:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.法师:
                    flag = this.宠物列表.Count < 5 && this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.刺客:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.弓手:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.道士:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.龙枪:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.通用:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.电脑:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
            }
            if (flag && 游戏怪物.数据表.TryGetValue(item.召唤下属名字, out var value))
            {
                宠物实例 宠物实例2;
                宠物实例2 = new 宠物实例(this, value, 7, 7, 绑定武器: false, 0, int.MaxValue);
                宠物实例2.物品召唤 = true;
                this.网络连接?.发送封包(new 同步宠物等级
                {
                    宠物编号 = 宠物实例2.地图编号,
                    宠物等级 = 宠物实例2.宠物等级
                });
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 9473,
                    第一参数 = (int)this.宠物模式
                });
                this.宠物数据.Add(宠物实例2.宠物数据);
                this.宠物列表.Add(宠物实例2);
                return true;
            }
            return false;
             * */
            bool flag;
            flag = false;
            switch (this.角色职业)
            {
                case 游戏对象职业.战士:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.法师:
                    flag = this.宠物列表.Count < 5 && this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.刺客:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.弓手:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.道士:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.龙枪:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.通用:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
                case 游戏对象职业.电脑:
                    flag = this.宠物列表.Where((宠物实例 o) => o.物品召唤).FirstOrDefault() == null;
                    break;
            }
            if (flag && 游戏怪物.数据表.TryGetValue(item.召唤下属名字, out var value))
            {
                宠物实例 宠物实例2;
                宠物实例2 = new 宠物实例(this, value, 7, 7, 绑定武器: false, 0, int.MaxValue);
                宠物实例2.物品召唤 = true;
                this.网络连接?.发送封包(new 同步宠物等级
                {
                    宠物编号 = 宠物实例2.地图编号,
                    宠物等级 = 宠物实例2.宠物等级
                });
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 9473,
                    第一参数 = (int)this.宠物模式
                });
                this.宠物数据.Add(宠物实例2.宠物数据);
                this.宠物列表.Add(宠物实例2);
                return true;
            }
            return false;
        }

        public bool 处理物品摆摊凭证(物品数据 item)
        {
            int prop;
            prop = item.GetProp(物品使用属性.地图编号, 147);
            if (!this.对象死亡 && this.交易状态 < 3)
            {
                if (this.当前等级 < 30 && this.本期特权 == 0)
                {
                    this.当前交易?.结束交易();
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 65538
                    });
                    return false;
                }
                if (this.当前摊位 != null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 2825
                    });
                    return false;
                }
                if (this.当前地图.地图编号 != prop || !this.当前地图.安全区内(this.当前坐标))
                {
                    this.发送顶部公告("请前往对应安全区或摆摊区进行摆摊");
                    return false;
                }
                if (this.当前地图[this.当前坐标].FirstOrDefault((地图对象 O) => O is 玩家实例 玩家实例2 && 玩家实例2.当前摊位 != null) == null)
                {
                    this.当前摊位 = new 玩家摊位();
                    base.发送封包(new 摆摊状态改变
                    {
                        对象编号 = this.地图编号,
                        摊位状态 = 1
                    });
                    return true;
                }
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2819
                });
            }
            return false;
        }

        private bool ProcessConsumableItem(物品数据 item)
        {
            if (item.物品类型 == 物品使用分类.技能书籍)
            {
                if (this.玩家学习技能(item.技能编号, 0))
                {
                    this.消耗背包物品(1, item, "学习技能扣除");
                    if (item.HasProp(游戏服务器.模板类.物品使用属性.坐骑编号))
                    {
                        this.获得坐骑((byte)item.GetProp(游戏服务器.模板类.物品使用属性.坐骑编号));
                    }
                }
                return true;
            }
            if (item.物品类型 == 物品使用分类.经验容器)
            {
                if (item.当前持久.V >= item.最大持久.V)
                {
                    this.消耗背包物品(item.当前持久.V, item, "释放经验容器");
                    this.玩家增加经验(null, item.最大持久.V);
                }
                return true;
            }
            // 重命名局部变量避开类型 物品使用属性 在本方法内的名字遮蔽
            物品使用类型 使用类型 = item.物品使用属性;
            if (使用类型 == 物品使用类型.未知分类)
            {
                return false;
            }
            bool flag;
            flag = false;
            switch (使用类型)
            {
                case 物品使用类型.体力回复:
                    flag = this.ProcessConsumableRecoveryHP(item);
                    break;
                case 物品使用类型.魔力回复:
                    flag = this.ProcessConsumableRecoveryMP(item);
                    break;
                case 物品使用类型.瞬回药品:
                    flag = this.ProcessConsumableMedicine(item);
                    break;
                case 物品使用类型.随机传送:
                    flag = this.ProcessConsumableRandomTeleport(item);
                    break;
                case 物品使用类型.礼包宝箱:
                    flag = this.ProcessConsumableTreasure(item);
                    break;
                case 物品使用类型.解包物品:
                    flag = this.ProcessConsumableStack(item);
                    break;
                case 物品使用类型.增加元宝:
                    this.修改货币("+", 游戏货币.元宝, (uint)item.GetProp(游戏服务器.模板类.物品使用属性.货币数量, 100));
                    主程.添加货币日志(this, "玩家使用物品", 游戏货币.元宝, item.GetProp(游戏服务器.模板类.物品使用属性.货币数量, 100));
                    flag = true;
                    break;
                case 物品使用类型.城镇传送:
                    flag = this.ProcessConsumableTownTeleport(item);
                    break;
                case 物品使用类型.祝福油:
                    flag = this.ProcessConsumableBlessing(item, (byte)item.GetProp(游戏服务器.模板类.物品使用属性.固定几率));
                    break;
                case 物品使用类型.切换铭文:
                    flag = this.ProcessConsumableSwitchSkill(item);
                    break;
                case 物品使用类型.获得坐骑:
                    flag = this.ProcessConsumableAdquireMount(item);
                    break;
                case 物品使用类型.添加BUFF:
                    flag = this.处理物品使用添加BUFF(item);
                    break;
                case 物品使用类型.减少PK值:
                    flag = this.处理物品使用减少PK值(item);
                    break;
                case 物品使用类型.获得称号:
                    flag = this.处理物品获得称号(item);
                    break;
                case 物品使用类型.召唤下属:
                    flag = this.处理物品召唤下属(item);
                    break;
                case 物品使用类型.摆摊凭证:
                    flag = this.处理物品摆摊凭证(item);
                    break;
            }
            if (flag)
            {
                if (item.分组编号 > 0 && item.分组冷却 > 0)
                {
                    this.冷却记录[item.分组编号 | 0] = 主程.当前时间.AddMilliseconds(item.分组冷却);
                    this.网络连接?.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (item.分组编号 | 0),
                        冷却时间 = item.分组冷却
                    });
                }
                if (item.冷却时间 > 0)
                {
                    this.冷却记录[item.物品编号 | 0x2000000] = 主程.当前时间.AddMilliseconds(item.冷却时间);
                    this.网络连接?.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (item.物品编号 | 0x2000000),
                        冷却时间 = item.冷却时间
                    });
                }
                this.消耗背包物品(1, item, "玩家使用物品");
                return true;
            }
            return false;
        }

        public void 玩家使用物品(byte 背包类型, byte 物品位置)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                物品数据 v;
                DateTime v2;
                DateTime v3;
                if (背包类型 != 1)
                {
                    this.网络连接?.尝试断开连接(new Exception($"错误操作: 玩家使用物品.  错误: 背包类型错误.{背包类型}  {物品位置}"));
                }
                else if (!this.角色背包.TryGetValue(物品位置, out v))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                }
                else if (this.当前等级 < v.需要等级)
                {
                    this.发送系统消息($"物品[{v.物品名字}]需要达到[{v.需要等级}]级使用");
                }
                else if (v.需要职业 != 游戏对象职业.通用 && this.角色职业 != v.需要职业)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家使用物品.  错误: 性别无法使用."));
                }
                else if (v.需要职业 != 游戏对象职业.通用 && this.角色职业 != v.需要职业)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家使用物品.  错误: 职业无法使用."));
                }
                else if (this.冷却记录.TryGetValue(v.物品编号 | 0x2000000, out v2) && 主程.当前时间 < v2)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1825
                    });
                }
                else if (v.分组编号 > 0 && this.冷却记录.TryGetValue(v.分组编号 | 0, out v3) && 主程.当前时间 < v3)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1825
                    });
                }
                else if (v.触发lua)
                {
                    if (游戏脚本.玩家使用物品(this, v) == 0L)
                    {
                        this.ProcessConsumableItem(v);
                    }
                }
                else if (!this.ProcessConsumableItem(v))
                {
                    this.CallDefaultNPC(DefaultNPCType.UseItem, true, v.物品编号);
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1877
                });
            }
        }

        public void 玩家喝修复油(byte 背包类型, byte 物品位置)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                装备数据 装备数据;
                装备数据 = null;
                if (背包类型 == 0 && this.角色装备.TryGetValue(物品位置, out var v))
                {
                    装备数据 = v;
                }
                if (背包类型 == 1 && this.角色背包.TryGetValue(物品位置, out var v2) && v2 is 装备数据 装备数据2)
                {
                    装备数据 = 装备数据2;
                }
                if (装备数据 == null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                    return;
                }
                if (!装备数据.能否修理)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1814
                    });
                    return;
                }
                if (装备数据.最大持久.V >= 装备数据.默认持久 * 2)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1953
                    });
                    return;
                }
                if (!this.查找背包物品(110012, out var 物品))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                    return;
                }
                this.消耗背包物品(1, 物品, "玩家喝修复油");
                if (计算类.计算概率(1f - (float)装备数据.最大持久.V * 0.5f / (float)装备数据.默认持久))
                {
                    装备数据.最大持久.V += 1000;
                    this.网络连接?.发送封包(new 修复最大持久
                    {
                        修复失败 = false
                    });
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 修复最大持久
                    {
                        修复失败 = true
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1877
                });
            }
        }

        public void 玩家合成物品(int 模板编号)
        {
            if (!this.对象死亡 && this.摆摊状态 <= 0)
            {
                _ = this.交易状态;
            }
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (!合成系统.数据表.TryGetValue(模板编号, out var value) || this.金币数量 < value.花费金币)
            {
                return;
            }
            List<物品数据> list;
            list = null;
            List<物品数据> list2;
            list2 = null;
            List<物品数据> list3;
            list3 = null;
            List<物品数据> list4;
            list4 = null;
            List<物品数据> list5;
            list5 = null;
            List<物品数据> list6;
            list6 = null;
            if (value.物品编号一 > 0)
            {
                list = this.查找背包物品(value.物品编号一, value.物品数量一);
                if (list == null)
                {
                    return;
                }
            }
            if (value.物品编号二 > 0)
            {
                list2 = this.查找背包物品(value.物品编号二, value.物品数量二);
                if (list2 == null)
                {
                    return;
                }
            }
            if (value.物品编号三 > 0)
            {
                list3 = this.查找背包物品(value.物品编号三, value.物品数量三);
                if (list3 == null)
                {
                    return;
                }
            }
            if (value.物品编号四 > 0)
            {
                list4 = this.查找背包物品(value.物品编号四, value.物品数量四);
                if (list4 == null)
                {
                    return;
                }
            }
            if (value.物品编号五 > 0)
            {
                list5 = this.查找背包物品(value.物品编号五, value.物品数量五);
                if (list5 == null)
                {
                    return;
                }
            }
            if (value.物品编号六 > 0)
            {
                list6 = this.查找背包物品(value.物品编号六, value.物品数量六);
                if (list6 == null)
                {
                    return;
                }
            }
            this.金币数量 -= (uint)value.花费金币;
            if (list != null)
            {
                this.消耗背包物品(value.物品数量一, list, "合成物品消耗");
            }
            if (list2 != null)
            {
                this.消耗背包物品(value.物品数量二, list2, "合成物品消耗");
            }
            if (list3 != null)
            {
                this.消耗背包物品(value.物品数量三, list3, "合成物品消耗");
            }
            if (list4 != null)
            {
                this.消耗背包物品(value.物品数量四, list4, "合成物品消耗");
            }
            if (list5 != null)
            {
                this.消耗背包物品(value.物品数量五, list5, "合成物品消耗");
            }
            if (list6 != null)
            {
                this.消耗背包物品(value.物品数量六, list6, "合成物品消耗");
            }
            if (value.是否广播)
            {
                this.发送顶部公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{value.获得物品}>><#T:MMOGame.DLG.ITEM.2>", 全服通知: true);
            }
            this.玩家获得物品(value.获得物品, 1, "合成物品获得");
            this.网络连接?.SendRaw(260, 7, new byte[5] { 1, 160, 48, 1, 0 });
        }

        public void 玩家出售物品(byte 背包类型, byte 物品位置, ushort 出售数量)
        {
            if (this.操作道具 && this.探索道具 != null)
            {
                this.探索道具.道具.Stop(this.探索道具);
            }
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3 && this.对话守卫 != null && this.当前地图 == this.对话守卫.当前地图 && base.网格距离(this.对话守卫) <= 12 && this.打开商店 != 0 && 出售数量 > 0 && 游戏商店.数据表.TryGetValue(this.打开商店, out var value))
            {
                物品数据 v;
                v = null;
                if (背包类型 == 1)
                {
                    this.角色背包.TryGetValue(物品位置, out v);
                }
                if (v == null)
                {
                    return;
                }
                if (v.是否上锁)
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1890
                    });
                }
                else if (!v.是否绑定 && v.出售类型 != 0 && value.回收类型 == v.出售类型)
                {
                    this.角色背包.Remove(物品位置);
                    主程.添加物品日志(this, "出售商店物品", v, 1, this.对话守卫?.对象名字);
                    value.出售物品(v);
                    this.修改货币("+", 游戏货币.金币, (uint)v.出售价格);
                    主程.添加货币日志(this, "出售商店物品->" + v.物品名字, 游戏货币.金币, v.出售价格);
                    this.网络连接?.发送封包(new 删除玩家物品
                    {
                        背包类型 = 背包类型,
                        物品位置 = 物品位置
                    });
                }
            }
        }

        public void 玩家购买物品(int 商店编号, int 物品位置, ushort 购入数量)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || this.对话守卫 == null || this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12 || this.打开商店 == 0 || 购入数量 <= 0 || this.打开商店 != 商店编号 || !游戏商店.数据表.TryGetValue(this.打开商店, out var value) || value.商品列表.Count <= 物品位置 || !游戏物品.数据表.TryGetValue(value.商品列表[物品位置].商品编号, out var value2))
            {
                return;
            }
            游戏商品 游戏商品;
            游戏商品 = value.商品列表[物品位置];
            bool 绑定物品;
            绑定物品 = 游戏商品.绑定物品;
            int num;
            num = ((购入数量 == 1 || value2.持久类型 != 物品持久分类.堆叠) ? 1 : Math.Min(购入数量, value2.物品持久));
            int num2;
            num2 = -1;
            byte b;
            b = 0;
            while (b < this.背包大小)
            {
                if (this.角色背包.TryGetValue(b, out var v) && (v.是否上锁 || value2.持久类型 != 物品持久分类.堆叠 || value2.物品编号 != v.物品编号 || v.当前持久.V + 购入数量 > value2.物品持久 || v.是否绑定 != 绑定物品))
                {
                    b++;
                    continue;
                }
                num2 = b;
                break;
            }
            if (num2 == -1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return;
            }
            int num3;
            num3 = 游戏商品.商品价格 * num;
            int num4;
            num4 = 游戏商品.备选价格 * num;
            List<物品数据> 物品列表;
            物品列表 = null;
            List<物品数据> 物品列表2;
            物品列表2 = null;
            if (游戏商品.货币类型 <= 19)
            {
                if (!this.检测货币(游戏商品.货币类型, num3))
                {
                    return;
                }
            }
            else if (!this.查找背包物品(num3, 游戏商品.货币类型, out 物品列表))
            {
                return;
            }
            if (游戏商品.备选货币 != -1)
            {
                if (游戏商品.备选货币 <= 19)
                {
                    if (!this.检测货币(游戏商品.备选货币, num4))
                    {
                        return;
                    }
                }
                else if (游戏商品.备选货币 != -1 && !this.查找背包物品(num4, 游戏商品.备选货币, out 物品列表2))
                {
                    return;
                }
            }
            绑定物品 |= 游戏商品.货币类型 == 0 || 游戏商品.备选货币 == 0;
            if (游戏商品.货币类型 <= 19)
            {
                this.扣除货币(游戏商品.货币类型, num3);
            }
            else if (物品列表 != null)
            {
                this.消耗背包物品(num3, 物品列表, "购买商店扣除");
            }
            if (游戏商品.备选货币 != -1 && 游戏商品.备选货币 <= 19)
            {
                this.扣除货币(游戏商品.货币类型, num4);
            }
            else if (物品列表2 != null)
            {
                this.消耗背包物品(num4, 物品列表2, "购买商店扣除");
            }
            num *= Math.Max(游戏商品.单位数量, 1);
            if (this.角色背包.TryGetValue((byte)num2, out var v2))
            {
                v2.当前持久.V += num;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = v2.字节描述()
                });
                主程.添加物品日志(this, "购买商店物品", v2, num, this.对话守卫.对象名字);
                return;
            }
            if (value2 is 游戏装备 模板)
            {
                this.角色背包[(byte)num2] = new 装备数据(模板, this.角色数据, 1, (byte)num2, 随机生成: false, 绑定物品, this.对话守卫?.对象名字 + "-商店购买");
            }
            else
            {
                int 持久;
                持久 = 0;
                switch (value2.持久类型)
                {
                    case 物品持久分类.堆叠:
                        持久 = num;
                        break;
                    case 物品持久分类.容器:
                        持久 = 0;
                        break;
                    case 物品持久分类.消耗:
                    case 物品持久分类.纯度:
                        持久 = value2.物品持久;
                        break;
                }
                this.角色背包[(byte)num2] = new 物品数据(value2, this.角色数据, 1, (byte)num2, 持久, 绑定物品, this.对话守卫?.对象名字 + "-商店购买");
            }
            _ = this.角色背包[(byte)num2];
            主程.添加物品日志(this, "购买商店物品", this.角色背包[(byte)num2], num, this.对话守卫.对象名字);
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = this.角色背包[(byte)num2].字节描述()
            });
        }

        public bool 检测货币(int 货币类型, int 需要数量)
        {
            if (Enum.TryParse<游戏货币>(货币类型.ToString(), out var result) && Enum.IsDefined(typeof(游戏货币), result))
            {
                if (result == 游戏货币.名师声望 || result == 游戏货币.道义点数)
                {
                    需要数量 *= 1000;
                }
                switch (result)
                {
                    case 游戏货币.金币:
                        if (this.金币数量 < 需要数量)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 13057
                            });
                            return false;
                        }
                        break;
                    case 游戏货币.银币:
                        if (this.银币数量 < 需要数量)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1820
                            });
                            return false;
                        }
                        break;
                    case 游戏货币.元宝:
                        if (this.元宝数量 < 需要数量)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 2561
                            });
                            return false;
                        }
                        break;
                    default:
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 13057
                        });
                        return false;
                    case 游戏货币.名师声望:
                        if (this.师门声望 < 需要数量)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 13057
                            });
                            return false;
                        }
                        break;
                }
                return true;
            }
            return false;
        }

        public void 扣除货币(int 货币类型, int 需要数量)
        {
            if (Enum.TryParse<游戏货币>(货币类型.ToString(), out var result) && Enum.IsDefined(typeof(游戏货币), result))
            {
                if (result == 游戏货币.名师声望 || result == 游戏货币.道义点数)
                {
                    需要数量 *= 1000;
                }
                switch (result)
                {
                    case 游戏货币.金币:
                        this.扣金币((uint)需要数量);
                        主程.添加货币日志(this, "购买商店物品", 游戏货币.金币, -需要数量);
                        break;
                    case 游戏货币.银币:
                        this.修改货币("-", 游戏货币.银币, (uint)需要数量);
                        主程.添加货币日志(this, "购买商店物品", 游戏货币.银币, -需要数量);
                        break;
                    case 游戏货币.元宝:
                        this.修改货币("-", 游戏货币.元宝, (uint)需要数量);
                        this.角色数据.消耗元宝.V += 需要数量;
                        主程.添加货币日志(this, "购买商店物品", 游戏货币.元宝, -需要数量);
                        break;
                    case 游戏货币.名师声望:
                        this.师门声望 -= (uint)需要数量;
                        break;
                }
                this.网络连接?.发送封包(new 货币数量变动
                {
                    货币类型 = (byte)货币类型,
                    货币数量 = ((result == 游戏货币.元宝) ? this.元宝数量 : this.角色数据.角色货币[result])
                });
            }
        }

        public bool 扣除货币(游戏货币 result, int 需要数量)
        {
            if (result == 游戏货币.名师声望 || result == 游戏货币.道义点数)
            {
                需要数量 *= 1000;
            }
            if (result == 游戏货币.元宝)
            {
                if (this.所属账号.元宝数量.V < 需要数量)
                {
                    return false;
                }
            }
            else if (!this.角色数据.角色货币.ContainsKey(result) || this.角色数据.角色货币[result] < 需要数量)
            {
                return false;
            }
            switch (result)
            {
                case 游戏货币.金币:
                    this.扣金币((uint)需要数量);
                    主程.添加货币日志(this, "购买商店物品", 游戏货币.金币, -需要数量);
                    break;
                case 游戏货币.银币:
                    this.修改货币("-", 游戏货币.银币, (uint)需要数量);
                    主程.添加货币日志(this, "购买商店物品", 游戏货币.银币, -需要数量);
                    break;
                case 游戏货币.元宝:
                    this.修改货币("-", 游戏货币.元宝, (uint)需要数量);
                    this.角色数据.消耗元宝.V += 需要数量;
                    主程.添加货币日志(this, "购买商店物品", 游戏货币.元宝, -需要数量);
                    break;
                case 游戏货币.名师声望:
                    this.师门声望 -= (uint)需要数量;
                    break;
            }
            this.网络连接?.发送封包(new 货币数量变动
            {
                货币类型 = (byte)result,
                货币数量 = ((result == 游戏货币.元宝) ? this.元宝数量 : this.角色数据.角色货币[result])
            });
            return true;
        }

        public void 增加货币(int 货币类型, int 需要数量, string 日志)
        {
            if (Enum.TryParse<游戏货币>(货币类型.ToString(), out var result) && Enum.IsDefined(typeof(游戏货币), result))
            {
                if (result == 游戏货币.名师声望 || result == 游戏货币.道义点数)
                {
                    需要数量 *= 1000;
                }
                switch (result)
                {
                    case 游戏货币.金币:
                        this.修改货币("+", 游戏货币.金币, (uint)需要数量);
                        主程.添加货币日志(this, 日志, 游戏货币.金币, -需要数量);
                        break;
                    case 游戏货币.银币:
                        this.修改货币("+", 游戏货币.银币, (uint)需要数量);
                        主程.添加货币日志(this, 日志, 游戏货币.银币, -需要数量);
                        break;
                    case 游戏货币.元宝:
                        this.修改货币("+", 游戏货币.元宝, (uint)需要数量);
                        this.角色数据.消耗元宝.V += 需要数量;
                        主程.添加货币日志(this, 日志, 游戏货币.元宝, -需要数量);
                        break;
                    case 游戏货币.名师声望:
                        this.师门声望 += (uint)需要数量;
                        break;
                }
                this.网络连接?.发送封包(new 货币数量变动
                {
                    货币类型 = (byte)货币类型,
                    货币数量 = ((result == 游戏货币.元宝) ? this.元宝数量 : this.角色数据.角色货币[result])
                });
            }
        }

        public void 玩家回购物品(byte 物品位置)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || this.对话守卫 == null || this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12 || this.打开商店 == 0 || !游戏商店.数据表.TryGetValue(this.打开商店, out var value) || this.回购清单.Count <= 物品位置)
            {
                return;
            }
            物品数据 物品数据;
            物品数据 = this.回购清单[物品位置];
            int num;
            num = -1;
            byte b;
            b = 0;
            while (b < this.背包大小)
            {
                if (this.角色背包.TryGetValue(b, out var v) && (!物品数据.能否堆叠 || 物品数据.物品编号 != v.物品编号 || v.当前持久.V + 物品数据.当前持久.V > v.最大持久.V || v.是否绑定 != 物品数据.是否绑定))
                {
                    b++;
                    continue;
                }
                num = b;
                break;
            }
            if (num == -1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
            }
            else if (this.金币数量 < 物品数据.出售价格)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1821
                });
            }
            else if (物品数据.出售价格 < 0)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家回购物品, 错误: 出售价格非法"));
            }
            else if (value.回购物品(物品数据))
            {
                this.回购清单.RemoveAt(物品位置);
                this.金币数量 -= (uint)物品数据.出售价格;
                主程.添加货币日志(this, "回购商店物品->" + 物品数据.物品名字, 游戏货币.金币, -物品数据.出售价格);
                if (this.角色背包.TryGetValue((byte)num, out var v2))
                {
                    v2.当前持久.V += 物品数据.当前持久.V;
                    主程.添加物品日志(this, "回购商店物品", 物品数据, 物品数据.当前持久.V, this.对话守卫?.对象名字);
                    物品数据.删除数据();
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = v2.字节描述()
                    });
                }
                else
                {
                    this.角色背包[(byte)num] = 物品数据;
                    物品数据.物品位置.V = (byte)num;
                    物品数据.物品容器.V = 1;
                    主程.添加物品日志(this, "回购商店物品", 物品数据, 1, this.对话守卫?.对象名字);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[(byte)num].字节描述()
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 12807
                });
            }
        }

        public void 请求回购清单()
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || this.对话守卫 == null || this.当前地图 != this.对话守卫.当前地图 || base.网格距离(this.对话守卫) > 12 || this.打开商店 == 0 || !游戏商店.数据表.TryGetValue(this.打开商店, out var value))
            {
                return;
            }
            this.回购清单 = value.回购列表.ToList();
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)this.回购清单.Count);
            foreach (物品数据 item in this.回购清单)
            {
                binaryWriter.Write(item.字节描述());
            }
            this.网络连接?.发送封包(new 同步回购列表
            {
                字节描述 = memoryStream.ToArray()
            });
        }

        public void 玩家镶嵌灵石(byte 装备类型, byte 装备位置, byte 装备孔位, byte 灵石类型, byte 灵石位置)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.打开界面 != "SoulEmbed")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 没有打开界面"));
            }
            else if (装备类型 == 1 && 灵石类型 == 1)
            {
                if (this.角色背包.TryGetValue(装备位置, out var v) && v is 装备数据 装备数据)
                {
                    if (!this.角色背包.TryGetValue(灵石位置, out var v2))
                    {
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 没有找到灵石"));
                        return;
                    }
                    if (装备数据.孔洞颜色.Count <= 装备孔位)
                    {
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 装备孔位错误"));
                        return;
                    }
                    if (装备数据.镶嵌灵石.ContainsKey(装备孔位))
                    {
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 已有镶嵌灵石"));
                        return;
                    }
                    if ((装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.绿色 && v2.物品名字.IndexOf("精绿灵石") == -1 && v2.物品名字.IndexOf("盈绿灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.黄色 && v2.物品名字.IndexOf("守阳灵石") == -1 && v2.物品名字.IndexOf("新阳灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.蓝色 && v2.物品名字.IndexOf("蔚蓝灵石") == -1 && v2.物品名字.IndexOf("透蓝灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.紫色 && v2.物品名字.IndexOf("纯紫灵石") == -1 && v2.物品名字.IndexOf("韧紫灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.灰色 && v2.物品名字.IndexOf("深灰灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.橙色 && v2.物品名字.IndexOf("橙黄灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.红色 && v2.物品名字.IndexOf("驭朱灵石") == -1 && v2.物品名字.IndexOf("命朱灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.褐色 && v2.物品名字.IndexOf("赤褐灵石") == -1) || (装备数据.孔洞颜色[装备孔位] == 装备孔洞颜色.多彩 && v2.物品名字.IndexOf("幻彩灵石") == -1))
                    {
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 指定灵石错误"));
                        return;
                    }
                    if (v2.物品名字.IndexOf("城主") != -1 && 装备数据.物品名字.IndexOf("城主") == -1)
                    {
                        base.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1916
                        });
                        return;
                    }
                    this.消耗背包物品(1, v2, "玩家镶嵌灵石");
                    装备数据.镶嵌灵石[装备孔位] = v2.物品模板;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                    this.网络连接?.发送封包(new 成功镶嵌灵石
                    {
                        灵石状态 = 1
                    });
                }
                else
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 没有找到装备"));
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 不是角色背包"));
            }
        }

        public void 玩家拆除灵石(byte 装备类型, byte 装备位置, byte 装备孔位)
        {
            int num;
            num = 0;
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            物品数据 v;
            if (this.打开界面 != "SoulEmbed")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 没有打开界面"));
            }
            else if (装备类型 != 1)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 不是角色背包"));
            }
            else if (this.背包剩余 <= 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
            }
            else if (this.角色背包.TryGetValue(装备位置, out v) && v is 装备数据 装备数据)
            {
                if (!装备数据.镶嵌灵石.TryGetValue(装备孔位, out var v2))
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 没有镶嵌灵石"));
                    return;
                }
                if (!灵石配置.数据表.TryGetValue(v2.物品编号, out var value))
                {
                    主程.添加系统日志("[" + this.对象名字 + "]灵石拆解时未能获取到配置");
                    return;
                }
                num = value.移除花费;
                byte b;
                b = 0;
                while (b < this.背包大小)
                {
                    if (this.角色背包.ContainsKey(b))
                    {
                        b++;
                        continue;
                    }
                    this.扣金币((uint)num);
                    主程.添加货币日志(this, "玩家拆除灵石", 游戏货币.金币, -num);
                    装备数据.镶嵌灵石.Remove(装备孔位);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                    this.角色背包[b] = new 物品数据(v2, this.角色数据, 1, b, 1, 绑定: false, this.对象名字 + "-拆除灵石");
                    主程.添加物品日志(this, "玩家拆除灵石", this.角色背包[b], 1);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[b].字节描述()
                    });
                    this.网络连接?.发送封包(new 成功取下灵石
                    {
                        灵石状态 = 1
                    });
                    break;
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 玩家镶嵌灵石.  错误: 没有找到装备"));
            }
        }

        public void 激活部位刻印(byte 刻印部位)
        {
            List<物品数据> list;
            list = this.查找背包物品(90226, 6000);
            if (list != null)
            {
                this.消耗背包物品(6000, list, "激活部位刻印");
                this.五零变量 |= this.刻印部位列表[刻印部位];
                this.UpdateAchievementProgress(sendMsg: true);
            }
        }

        public void 玩家铭文刻印(byte 装备位置, int 物品编号, int 铭文索引)
        {
            if (物品编号 % 100 / 10 != 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1923
                });
                return;
            }
            List<物品数据> list;
            list = this.查找背包物品(90226, 1000);
            List<物品数据> list2;
            list2 = this.查找背包物品(物品编号, (装备位置 == 13) ? 10 : 5);
            if (list == null && list2 == null)
            {
                return;
            }
            装备数据 v;
            if (装备位置 != 13 && 装备位置 != 8 && 装备位置 != 4 && 装备位置 != 14)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1924
                });
            }
            else if (this.角色装备.TryGetValue(装备位置, out v) && v.装备模板.能否刻印)
            {
                if (铭文技能.数据表.TryGetValue((ushort)铭文索引, out var value))
                {
                    this.消耗背包物品(1000, list, "铭文刻印消耗");
                    this.消耗背包物品((装备位置 == 13) ? 10 : 5, list2, "铭文刻印消耗");
                    v.第一铭文 = value;
                    this.玩家装卸铭文(v.第一铭文.技能编号, v.第一铭文.铭文编号);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = v.字节描述()
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1922
                });
            }
        }

        public void 普通铭文洗练(byte 装备类型, byte 装备位置, int 物品编号)
        {
            装备数据 装备数据;
            装备数据 = null;
            if (装备类型 == 0 && this.角色装备.TryGetValue(装备位置, out var v))
            {
                装备数据 = v;
            }
            if (装备类型 == 1 && this.角色背包.TryGetValue(装备位置, out var v2) && v2 is 装备数据 装备数据2)
            {
                装备数据 = 装备数据2;
            }
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 没有打开界面"));
            }
            else
            {
                if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
                {
                    return;
                }
                int num;
                num = ((物品编号 / 10 == 2101) ? 100 : 10000);
                if (this.金币数量 < num)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1821
                    });
                    return;
                }
                if (装备数据 == null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                    return;
                }
                if (装备数据.物品类型 != 物品使用分类.武器)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 物品类型错误."));
                    return;
                }
                if (物品编号 <= 0)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 材料编号错误."));
                    return;
                }
                if (!this.查找背包物品(物品编号, out var 物品))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1835
                    });
                    return;
                }
                if (物品.物品类型 != 物品使用分类.普通铭文)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 材料类型错误."));
                    return;
                }
                this.金币数量 -= (uint)num;
                主程.添加货币日志(this, "玩家洗练铭文", 游戏货币.金币, -num);
                this.消耗背包物品(1, 物品, "玩家洗练铭文");
                byte 洗练职业;
                洗练职业 = (byte)物品.需要职业;
                bool flag;
                flag = 物品编号 % 100 / 10 == 2;
                ushort 序号;
                序号 = 12;
                switch (物品.需要职业)
                {
                    case 游戏对象职业.战士:
                        序号 = 12;
                        break;
                    case 游戏对象职业.法师:
                        序号 = 13;
                        break;
                    case 游戏对象职业.刺客:
                        序号 = 14;
                        break;
                    case 游戏对象职业.弓手:
                        序号 = 15;
                        break;
                    case 游戏对象职业.道士:
                        序号 = 16;
                        break;
                    case 游戏对象职业.龙枪:
                        序号 = 19;
                        break;
                }
                ushort 当前节点;
                当前节点 = this.获取玩家节点(序号);
                ushort num2;
                num2 = 铭文洗炼技能.普通节点次数.FirstOrDefault((ushort n) => n > 当前节点);
                if (当前节点 > 铭文洗炼技能.普通节点最终次数)
                {
                    this.更改玩家节点(序号, 0);
                    当前节点 = 0;
                }
                else
                {
                    this.更改玩家节点(序号, (ushort)(当前节点 + 铭文洗炼技能.普通节点数_));
                }
                if (num2 > 0 && 当前节点 + 铭文洗炼技能.普通节点数_ > num2)
                {
                    当前节点 = num2;
                }
                if (this.开启七天乐)
                {
                    this.修改七天进度(44, this.角色数据.七天进度[44] + 1);
                    this.修改七天进度(54, this.角色数据.七天进度[54] + 1);
                }
                byte b;
                b = 0;
                if (装备数据.第一铭文 == null)
                {
                    if (flag)
                    {
                        switch (物品.需要职业)
                        {
                            case 游戏对象职业.战士:
                                装备数据.第一铭文 = 铭文技能.数据表[10314];
                                break;
                            case 游戏对象职业.法师:
                                装备数据.第一铭文 = 铭文技能.数据表[25311];
                                break;
                            case 游戏对象职业.刺客:
                                装备数据.第一铭文 = 铭文技能.数据表[15311];
                                break;
                            case 游戏对象职业.弓手:
                                装备数据.第一铭文 = 铭文技能.数据表[20411];
                                break;
                            case 游戏对象职业.道士:
                                装备数据.第一铭文 = 铭文技能.数据表[30021];
                                break;
                            case 游戏对象职业.龙枪:
                                装备数据.第一铭文 = 铭文技能.数据表[12012];
                                break;
                        }
                    }
                    else
                    {
                        装备数据.第一铭文 = 铭文技能.随机洗练(洗练职业, 0, 当前节点);
                    }
                    if (装备类型 == 0)
                    {
                        this.玩家装卸铭文(装备数据.第一铭文.技能编号, 装备数据.第一铭文.铭文编号);
                    }
                    if (装备数据.第一铭文.广播通知)
                    {
                        网络服务网关.发送公告("恭喜[" + this.对象名字 + "]在铭文洗炼中获得稀有铭文[" + 装备数据.第一铭文.技能名字.Split('-').Last() + "]");
                    }
                }
                else if (装备数据.传承材料 != 0 && (装备数据.双铭文点 += ((!flag) ? 1 : 0)) >= 1200 && 装备数据.第二铭文 == null)
                {
                    while ((装备数据.第二铭文 = 铭文技能.随机洗练(洗练职业, 0, 当前节点)).技能编号 == 装备数据.第一铭文?.技能编号)
                    {
                        b++;
                        if (b > 9)
                        {
                            break;
                        }
                    }
                    if (装备类型 == 0)
                    {
                        this.玩家装卸铭文(装备数据.第二铭文.技能编号, 装备数据.第二铭文.铭文编号);
                    }
                    if (装备数据.第二铭文.广播通知)
                    {
                        网络服务网关.发送公告("恭喜[" + this.对象名字 + "]在铭文洗炼中获得稀有铭文[" + 装备数据.第二铭文.技能名字.Split('-').Last() + "]");
                    }
                }
                else
                {
                    if (装备类型 == 0)
                    {
                        this.玩家装卸铭文(装备数据.第一铭文.技能编号, 0);
                    }
                    while ((装备数据.第一铭文 = 铭文技能.随机洗练(洗练职业, 0, 当前节点)).技能编号 == 装备数据.第二铭文?.技能编号)
                    {
                        b++;
                        if (b > 9)
                        {
                            break;
                        }
                    }
                    if (装备类型 == 0)
                    {
                        this.玩家装卸铭文(装备数据.第一铭文.技能编号, 装备数据.第一铭文.铭文编号);
                    }
                    if (装备数据.第一铭文.广播通知)
                    {
                        网络服务网关.发送公告("恭喜[" + this.对象名字 + "]在铭文洗炼中获得稀有铭文[" + 装备数据.第一铭文.技能名字.Split('-').Last() + "]");
                    }
                }
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.发送封包(new 玩家普通洗练
                {
                    铭文位一 = (装备数据.第一铭文?.铭文索引 ?? 0),
                    铭文位二 = (装备数据.第二铭文?.铭文索引 ?? 0)
                });
            }
        }

        public void 高级铭文洗练(byte 装备类型, byte 装备位置, int 物品编号)
        {
            装备数据 装备数据;
            装备数据 = null;
            if (装备类型 == 0 && this.角色装备.TryGetValue(装备位置, out var v))
            {
                装备数据 = v;
            }
            if (装备类型 == 1 && this.角色背包.TryGetValue(装备位置, out var v2) && v2 is 装备数据 装备数据2)
            {
                装备数据 = 装备数据2;
            }
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 没有打开界面"));
            }
            else
            {
                if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
                {
                    return;
                }
                if (this.金币数量 < 100000)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1821
                    });
                    return;
                }
                if (装备数据 == null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                    return;
                }
                if (装备数据.物品类型 != 物品使用分类.武器)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 物品类型错误."));
                    return;
                }
                if (装备数据.第二铭文 == null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 第二铭文为空."));
                    return;
                }
                if (物品编号 <= 0)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 材料编号错误."));
                    return;
                }
                if (!this.查找背包物品(物品编号, out var 物品))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1835
                    });
                    return;
                }
                if (物品.物品类型 != 物品使用分类.普通铭文)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 材料类型错误."));
                    return;
                }
                this.金币数量 -= 100000u;
                主程.添加货币日志(this, "玩家洗练铭文", 游戏货币.金币, -100000);
                this.消耗背包物品(1, 物品, "玩家洗练铭文");
                byte 洗练职业;
                洗练职业 = (byte)物品.需要职业;
                ushort 序号;
                序号 = 268;
                switch (物品.需要职业)
                {
                    case 游戏对象职业.战士:
                        序号 = 268;
                        break;
                    case 游戏对象职业.法师:
                        序号 = 269;
                        break;
                    case 游戏对象职业.刺客:
                        序号 = 270;
                        break;
                    case 游戏对象职业.弓手:
                        序号 = 271;
                        break;
                    case 游戏对象职业.道士:
                        序号 = 272;
                        break;
                    case 游戏对象职业.龙枪:
                        序号 = 275;
                        break;
                }
                ushort 当前节点;
                当前节点 = this.获取玩家节点(序号);
                ushort num;
                num = 铭文洗炼技能.普通节点次数.FirstOrDefault((ushort n) => n > 当前节点);
                if (当前节点 > 铭文洗炼技能.高级节点最终次数)
                {
                    this.更改玩家节点(序号, 0);
                    当前节点 = 0;
                }
                else
                {
                    this.更改玩家节点(序号, (ushort)(当前节点 + 铭文洗炼技能.高级节点数_));
                }
                if (num > 0 && 当前节点 + 铭文洗炼技能.高级节点数_ > num)
                {
                    当前节点 = num;
                }
                byte b;
                b = 0;
                do
                {
                    this.洗练铭文 = 铭文技能.随机洗练(洗练职业, 1, 当前节点);
                    b++;
                }
                while (b <= 9 && (this.洗练铭文.技能编号 == 装备数据.第一铭文?.技能编号 || this.洗练铭文.技能编号 == 装备数据.第二铭文.技能编号));
                if (装备数据.最优铭文 == 装备数据.第一铭文)
                {
                    this.网络连接?.发送封包(new 玩家高级洗练
                    {
                        洗练结果 = 1,
                        铭文位一 = 装备数据.最优铭文.铭文索引,
                        铭文位二 = this.洗练铭文.铭文索引
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 玩家高级洗练
                    {
                        洗练结果 = 1,
                        铭文位一 = this.洗练铭文.铭文索引,
                        铭文位二 = 装备数据.最优铭文.铭文索引
                    });
                }
                if (this.洗练铭文.广播通知)
                {
                    网络服务网关.发送公告("恭喜[" + this.对象名字 + "]在铭文洗炼中获得稀有铭文[" + this.洗练铭文.技能名字.Split('-').Last() + "]");
                }
            }
        }

        public void 替换铭文洗练(byte 装备类型, byte 装备位置, int 物品编号)
        {
            装备数据 装备数据;
            装备数据 = null;
            int num;
            num = 10;
            if (装备类型 == 0 && this.角色装备.TryGetValue(装备位置, out var v))
            {
                装备数据 = v;
            }
            if (装备类型 == 1 && this.角色背包.TryGetValue(装备位置, out var v2) && v2 is 装备数据 装备数据2)
            {
                装备数据 = 装备数据2;
            }
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 没有打开界面"));
            }
            else
            {
                if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
                {
                    return;
                }
                if (this.金币数量 < 1000000)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1821
                    });
                    return;
                }
                if (装备数据 == null)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1802
                    });
                    return;
                }
                if (装备数据.物品类型 != 物品使用分类.武器)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 物品类型错误."));
                    return;
                }
                if (装备数据.第二铭文 == null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 第二铭文为空."));
                    return;
                }
                if (物品编号 <= 0)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 材料编号错误."));
                    return;
                }
                if (!this.查找背包物品(num, 物品编号, out var 物品列表))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1835
                    });
                    return;
                }
                if (物品列表.FirstOrDefault((物品数据 O) => O.物品类型 != 物品使用分类.普通铭文) != null)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 材料类型错误."));
                    return;
                }
                this.金币数量 -= 1000000u;
                主程.添加货币日志(this, "玩家洗练铭文", 游戏货币.金币, -100000);
                this.消耗背包物品(num, 物品列表, "玩家洗练铭文");
                byte 洗练职业;
                洗练职业 = (byte)物品列表.FirstOrDefault().需要职业;
                while ((this.洗练铭文 = 铭文技能.随机洗练(洗练职业, 0)).技能编号 == 装备数据.第一铭文.技能编号 || this.洗练铭文.技能编号 == 装备数据.第二铭文.技能编号)
                {
                }
                this.网络连接?.发送封包(new 玩家高级洗练
                {
                    洗练结果 = 1,
                    铭文位一 = 装备数据.最差铭文.铭文索引,
                    铭文位二 = this.洗练铭文.铭文索引
                });
                if (this.洗练铭文.广播通知)
                {
                    网络服务网关.发送公告("恭喜[" + this.对象名字 + "]在铭文洗炼中获得稀有铭文[" + this.洗练铭文.技能名字.Split('-').Last() + "]");
                }
            }
        }

        public void 高级洗练确认(byte 装备类型, byte 装备位置)
        {
            装备数据 装备数据;
            装备数据 = null;
            if (装备类型 == 0 && this.角色装备.TryGetValue(装备位置, out var v))
            {
                装备数据 = v;
            }
            if (装备类型 == 1 && this.角色背包.TryGetValue(装备位置, out var v2) && v2 is 装备数据 装备数据2)
            {
                装备数据 = 装备数据2;
            }
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 没有打开界面"));
                return;
            }
            if (装备数据 == null)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1802
                });
                return;
            }
            if (this.洗练铭文 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 确定替换铭文.  错误: 没有没有洗练记录."));
                return;
            }
            if (装备数据.物品类型 != 物品使用分类.武器)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 物品类型错误."));
                return;
            }
            if (装备数据.第二铭文 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 第二铭文为空."));
                return;
            }
            if (装备类型 == 0)
            {
                this.玩家装卸铭文(装备数据.最差铭文.技能编号, 0);
            }
            装备数据.最差铭文 = this.洗练铭文;
            if (装备类型 == 0)
            {
                this.玩家装卸铭文(this.洗练铭文.技能编号, this.洗练铭文.铭文编号);
            }
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = 装备数据.字节描述()
            });
            this.网络连接?.发送封包(new 确认替换铭文
            {
                确定替换 = true
            });
        }

        public void 替换洗练确认(byte 装备类型, byte 装备位置)
        {
            装备数据 装备数据;
            装备数据 = null;
            if (装备类型 == 0 && this.角色装备.TryGetValue(装备位置, out var v))
            {
                装备数据 = v;
            }
            if (装备类型 == 1 && this.角色背包.TryGetValue(装备位置, out var v2) && v2 is 装备数据 装备数据2)
            {
                装备数据 = 装备数据2;
            }
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 没有打开界面"));
                return;
            }
            if (装备数据 == null)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1802
                });
                return;
            }
            if (this.洗练铭文 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 确定替换铭文.  错误: 没有没有洗练记录."));
                return;
            }
            if (装备数据.物品类型 != 物品使用分类.武器)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 物品类型错误."));
                return;
            }
            if (装备数据.第二铭文 == null)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 普通铭文洗练.  错误: 第二铭文为空."));
                return;
            }
            if (装备类型 == 0)
            {
                this.玩家装卸铭文(装备数据.最优铭文.技能编号, 0);
            }
            装备数据.最优铭文 = this.洗练铭文;
            if (装备类型 == 0)
            {
                this.玩家装卸铭文(this.洗练铭文.技能编号, this.洗练铭文.铭文编号);
            }
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = 装备数据.字节描述()
            });
            this.网络连接?.发送封包(new 确认替换铭文
            {
                确定替换 = true
            });
        }

        public void 放弃替换铭文()
        {
            this.洗练铭文 = null;
            this.网络连接?.发送封包(new 确认替换铭文
            {
                确定替换 = false
            });
        }

        public void 解锁双铭文位(byte 装备类型, byte 装备位置, byte 操作参数)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            物品数据 v;
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 解锁双铭文位.  错误: 没有打开界面"));
            }
            else if (装备类型 != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1839
                });
            }
            else if (this.角色背包.TryGetValue(装备位置, out v) && v is 装备数据 装备数据)
            {
                if (装备数据.物品类型 != 物品使用分类.武器)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 解锁双铭文位.  错误: 物品类型错误."));
                }
                else if (操作参数 == 1)
                {
                    int num;
                    num = 10000000;
                    if (装备数据.双铭文栏.V)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1909
                        });
                        return;
                    }
                    if (this.金币数量 < num)
                    {
                        this.网络连接?.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1821
                        });
                        return;
                    }
                    this.金币数量 -= (uint)num;
                    主程.添加货币日志(this, "解锁双铭文位", 游戏货币.金币, -num);
                    装备数据.双铭文栏.V = true;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                    this.网络连接?.发送封包(new 双铭文位切换
                    {
                        当前栏位 = 装备数据.当前铭栏.V,
                        第一铭文 = (装备数据.第一铭文?.铭文索引 ?? 0),
                        第二铭文 = (装备数据.第二铭文?.铭文索引 ?? 0)
                    });
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 解锁双铭文位.  错误: 不是装备类型."));
            }
        }

        public void 切换双铭文位(byte 装备类型, byte 装备位置, byte 操作参数)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            物品数据 v;
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 切换双铭文位.  错误: 没有打开界面"));
            }
            else if (装备类型 != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1839
                });
            }
            else if (this.角色背包.TryGetValue(装备位置, out v) && v is 装备数据 装备数据)
            {
                if (装备数据.物品类型 != 物品使用分类.武器)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 切换双铭文位.  错误: 物品类型错误."));
                    return;
                }
                if (!装备数据.双铭文栏.V)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1926
                    });
                    return;
                }
                if (操作参数 == 装备数据.当前铭栏.V)
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 切换双铭文位.  错误: 切换铭位错误."));
                    return;
                }
                装备数据.当前铭栏.V = 操作参数;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.发送封包(new 双铭文位切换
                {
                    当前栏位 = 装备数据.当前铭栏.V,
                    第一铭文 = (装备数据.第一铭文?.铭文索引 ?? 0),
                    第二铭文 = (装备数据.第二铭文?.铭文索引 ?? 0)
                });
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 切换双铭文位.  错误: 不是装备类型."));
            }
        }

        public void 传承武器铭文(byte 来源类型, byte 来源位置, byte 目标类型, byte 目标位置)
        {
            int num;
            num = 10000000;
            int num2;
            num2 = 1500;
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            if (this.打开界面 != "WeaponRune")
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 传承武器铭文.  错误: 没有打开界面"));
            }
            else if (来源类型 == 1 && 目标类型 == 1)
            {
                if (this.角色背包.TryGetValue(来源位置, out var v) && v is 装备数据 装备数据 && this.角色背包.TryGetValue(目标位置, out var v2) && v2 is 装备数据 装备数据2)
                {
                    if (装备数据.物品类型 == 物品使用分类.武器 && 装备数据2.物品类型 == 物品使用分类.武器)
                    {
                        if (装备数据.传承材料 != 0 && 装备数据2.传承材料 != 0 && 装备数据.传承材料 == 装备数据2.传承材料)
                        {
                            if (装备数据.第二铭文 != null && 装备数据2.第二铭文 != null)
                            {
                                if (this.金币数量 < num)
                                {
                                    this.网络连接?.发送封包(new 游戏错误提示
                                    {
                                        错误代码 = 1821
                                    });
                                    return;
                                }
                                if (!this.查找背包物品(num2, 装备数据.传承材料, out var 物品列表))
                                {
                                    this.网络连接?.发送封包(new 游戏错误提示
                                    {
                                        错误代码 = 1835
                                    });
                                    return;
                                }
                                this.金币数量 -= (uint)num;
                                主程.添加货币日志(this, "传承武器铭文", 游戏货币.金币, -num);
                                this.消耗背包物品(num2, 物品列表, "传承武器铭文");
                                装备数据2.第一铭文 = 装备数据.第一铭文;
                                装备数据2.第二铭文 = 装备数据.第二铭文;
                                装备数据.铭文技能.Remove((byte)(装备数据.当前铭栏.V * 2));
                                装备数据.铭文技能.Remove((byte)(装备数据.当前铭栏.V * 2 + 1));
                                this.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = 装备数据.字节描述()
                                });
                                this.网络连接?.发送封包(new 玩家物品变动
                                {
                                    物品描述 = 装备数据2.字节描述()
                                });
                                this.网络连接?.发送封包(new 铭文传承应答());
                            }
                            else
                            {
                                this.网络连接?.发送封包(new 游戏错误提示
                                {
                                    错误代码 = 1887
                                });
                            }
                        }
                        else
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1887
                            });
                        }
                    }
                    else
                    {
                        this.网络连接?.尝试断开连接(new Exception("错误操作: 传承武器铭文.  错误: 物品类型错误."));
                    }
                }
                else
                {
                    this.网络连接?.尝试断开连接(new Exception("错误操作: 传承武器铭文.  错误: 不是装备类型."));
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1839
                });
            }
        }

        public void 升级武器普通(byte[] 首饰组, byte[] 材料组)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            装备数据 v;
            if (this.角色数据.升级装备.V != null)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1854
                });
            }
            else if (this.金币数量 < 10000)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1821
                });
            }
            else if (!this.角色装备.TryGetValue(0, out v))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1853
                });
            }
            else if (v.物品状态.V != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1857
                });
            }
            else if (v.最大持久.V > 3000 && (float)v.最大持久.V > (float)v.默认持久 * 0.5f)
            {
                if (v.升级次数.V >= 9)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1815
                    });
                    return;
                }
                Dictionary<byte, 装备数据> dictionary;
                dictionary = new Dictionary<byte, 装备数据>();
                byte[] array;
                array = 首饰组;
                foreach (byte b in array)
                {
                    if (b != byte.MaxValue)
                    {
                        if (!this.角色背包.TryGetValue(b, out var v2))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (!(v2 is 装备数据 装备数据) || (装备数据.物品类型 != 物品使用分类.项链 && 装备数据.物品类型 != 物品使用分类.手镯 && 装备数据.物品类型 != 物品使用分类.戒指))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (dictionary.ContainsKey(b))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        dictionary.Add(b, 装备数据);
                    }
                }
                Dictionary<byte, 物品数据> dictionary2;
                dictionary2 = new Dictionary<byte, 物品数据>();
                array = 材料组;
                foreach (byte b2 in array)
                {
                    if (b2 != byte.MaxValue)
                    {
                        if (!this.角色背包.TryGetValue(b2, out var v3))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (v3.物品类型 != 物品使用分类.武器锻造)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (dictionary2.ContainsKey(b2))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        dictionary2.Add(b2, v3);
                    }
                }
                this.扣金币(10000u);
                主程.添加货币日志(this, "升级武器普通", 游戏货币.金币, -10000);
                byte[] array2;
                array2 = 首饰组;
                foreach (byte b3 in array2)
                {
                    if (b3 != byte.MaxValue)
                    {
                        主程.添加物品日志(this, "升级武器普通", this.角色背包[b3], 1);
                        this.角色背包[b3].删除数据();
                        this.角色背包.Remove(b3);
                        this.网络连接?.发送封包(new 删除玩家物品
                        {
                            背包类型 = 1,
                            物品位置 = b3
                        });
                    }
                }
                array2 = 材料组;
                foreach (byte b4 in array2)
                {
                    if (b4 != byte.MaxValue)
                    {
                        主程.添加物品日志(this, "升级武器普通", this.角色背包[b4], 1);
                        this.角色背包[b4].删除数据();
                        this.角色背包.Remove(b4);
                        this.网络连接?.发送封包(new 删除玩家物品
                        {
                            背包类型 = 1,
                            物品位置 = b4
                        });
                    }
                }
                this.角色装备.Remove(0);
                this.玩家穿卸装备(装备穿戴部位.武器, v, null);
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 0,
                    物品位置 = 0
                });
                this.网络连接?.发送封包(new 放入升级武器());
                Dictionary<byte, Dictionary<装备数据, int>> dictionary3;
                dictionary3 = new Dictionary<byte, Dictionary<装备数据, int>>
                {
                    [0] = new Dictionary<装备数据, int>(),
                    [1] = new Dictionary<装备数据, int>(),
                    [2] = new Dictionary<装备数据, int>(),
                    [3] = new Dictionary<装备数据, int>(),
                    [4] = new Dictionary<装备数据, int>()
                };
                foreach (装备数据 value in dictionary.Values)
                {
                    Dictionary<游戏对象属性, int> 装备属性;
                    装备属性 = value.装备属性;
                    int num;
                    num = 0;
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小攻击) ? 装备属性[游戏对象属性.最小攻击] : 0) + (装备属性.ContainsKey(游戏对象属性.最大攻击) ? 装备属性[游戏对象属性.最大攻击] : 0)) > 0)
                    {
                        dictionary3[0][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小魔法) ? 装备属性[游戏对象属性.最小魔法] : 0) + (装备属性.ContainsKey(游戏对象属性.最大魔法) ? 装备属性[游戏对象属性.最大魔法] : 0)) > 0)
                    {
                        dictionary3[1][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小道术) ? 装备属性[游戏对象属性.最小道术] : 0) + (装备属性.ContainsKey(游戏对象属性.最大道术) ? 装备属性[游戏对象属性.最大道术] : 0)) > 0)
                    {
                        dictionary3[2][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小刺术) ? 装备属性[游戏对象属性.最小刺术] : 0) + (装备属性.ContainsKey(游戏对象属性.最大刺术) ? 装备属性[游戏对象属性.最大刺术] : 0)) > 0)
                    {
                        dictionary3[3][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小弓术) ? 装备属性[游戏对象属性.最小弓术] : 0) + (装备属性.ContainsKey(游戏对象属性.最大弓术) ? 装备属性[游戏对象属性.最大弓术] : 0)) > 0)
                    {
                        dictionary3[4][value] = num;
                    }
                }
                List<KeyValuePair<byte, Dictionary<装备数据, int>>> 排序属性;
                排序属性 = (from x in dictionary3.ToList()
                        orderby ((KeyValuePair<byte, Dictionary<装备数据, int>>)x).Value.Values.Sum() descending
                        select x).ToList();
                List<KeyValuePair<byte, Dictionary<装备数据, int>>> list;
                list = 排序属性.Where((KeyValuePair<byte, Dictionary<装备数据, int>> O) => O.Value.Values.Sum() == 排序属性[0].Value.Values.Sum()).ToList();
                byte key;
                key = list[主程.随机数.Next(list.Count)].Key;
                List<KeyValuePair<装备数据, int>> list2;
                list2 = (from x in dictionary3[key].ToList()
                         orderby ((KeyValuePair<装备数据, int>)x).Value descending
                         select x).ToList();
                float num2;
                num2 = Math.Min(10f, (float)((list2.Count < 1) ? 1 : list2[0].Value) + (float)((list2.Count >= 2) ? list2[1].Value : 0) / 3f);
                int num3;
                num3 = dictionary2.Values.Sum((物品数据 x) => x.当前持久.V);
                float num4;
                num4 = Math.Max(0f, num3 - 146);
                float 概率;
                概率 = num2 * (float)(90 - v.升级次数.V * 10) * 0.001f + num4 * 0.01f;
                this.角色数据.升级装备.V = v;
                this.角色数据.取回时间.V = 主程.当前时间.AddHours(2.0);
                v.物品状态.V = 2;
                v.升级属性.V = byte.MaxValue;
                if (计算类.计算概率(概率))
                {
                    v.升级属性.V = key;
                }
                if (num3 < 30)
                {
                    v.扣除持久.V = 3000;
                }
                else if (num3 < 60)
                {
                    v.扣除持久.V = 2000;
                }
                else if (num3 < 99)
                {
                    v.扣除持久.V = 1000;
                }
                else if (num3 > 130 && 计算类.计算概率(1f - (float)v.最大持久.V * 0.5f / (float)v.默认持久))
                {
                    v.扣除持久.V = -1000;
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1856
                });
            }
        }

        public void 升级武器高级(byte[] 首饰组, byte[] 材料组)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            装备数据 v;
            if (this.角色数据.升级装备.V != null)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1854
                });
            }
            else if (this.金币数量 < 10000)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1821
                });
            }
            else if (!this.角色装备.TryGetValue(0, out v))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1853
                });
            }
            else if (v.物品状态.V != 1)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1857
                });
            }
            else if (v.最大持久.V > 3000 && (float)v.最大持久.V > (float)v.默认持久 * 0.5f)
            {
                if (v.升级次数.V >= 5)
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 1815
                    });
                    return;
                }
                Dictionary<byte, 装备数据> dictionary;
                dictionary = new Dictionary<byte, 装备数据>();
                byte[] array;
                array = 首饰组;
                foreach (byte b in array)
                {
                    if (b != byte.MaxValue)
                    {
                        if (!this.角色背包.TryGetValue(b, out var v2))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (!(v2 is 装备数据 装备数据) || (装备数据.物品类型 != 物品使用分类.项链 && 装备数据.物品类型 != 物品使用分类.手镯 && 装备数据.物品类型 != 物品使用分类.戒指))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (dictionary.ContainsKey(b))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        dictionary.Add(b, 装备数据);
                    }
                }
                Dictionary<byte, 物品数据> dictionary2;
                dictionary2 = new Dictionary<byte, 物品数据>();
                array = 材料组;
                foreach (byte b2 in array)
                {
                    if (b2 != byte.MaxValue)
                    {
                        if (!this.角色背包.TryGetValue(b2, out var v3))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (v3.物品类型 != 物品使用分类.武器锻造)
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        if (dictionary2.ContainsKey(b2))
                        {
                            this.网络连接?.发送封包(new 游戏错误提示
                            {
                                错误代码 = 1859
                            });
                            return;
                        }
                        dictionary2.Add(b2, v3);
                    }
                }
                if (this.玩家武器升级() <= 0)
                {
                    return;
                }
                byte[] array2;
                array2 = 首饰组;
                foreach (byte b3 in array2)
                {
                    if (b3 != byte.MaxValue)
                    {
                        主程.添加物品日志(this, "升级武器高级", this.角色背包[b3], 1);
                        this.角色背包[b3].删除数据();
                        this.角色背包.Remove(b3);
                        this.网络连接?.发送封包(new 删除玩家物品
                        {
                            背包类型 = 1,
                            物品位置 = b3
                        });
                    }
                }
                array2 = 材料组;
                foreach (byte b4 in array2)
                {
                    if (b4 != byte.MaxValue)
                    {
                        主程.添加物品日志(this, "升级武器高级", this.角色背包[b4], 1);
                        this.角色背包[b4].删除数据();
                        this.角色背包.Remove(b4);
                        this.网络连接?.发送封包(new 删除玩家物品
                        {
                            背包类型 = 1,
                            物品位置 = b4
                        });
                    }
                }
                this.角色装备.Remove(0);
                this.玩家穿卸装备(装备穿戴部位.武器, v, null);
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 0,
                    物品位置 = 0
                });
                this.网络连接?.发送封包(new 放入升级武器());
                Dictionary<byte, Dictionary<装备数据, int>> dictionary3;
                dictionary3 = new Dictionary<byte, Dictionary<装备数据, int>>
                {
                    [0] = new Dictionary<装备数据, int>(),
                    [1] = new Dictionary<装备数据, int>(),
                    [2] = new Dictionary<装备数据, int>(),
                    [3] = new Dictionary<装备数据, int>(),
                    [4] = new Dictionary<装备数据, int>()
                };
                foreach (装备数据 value in dictionary.Values)
                {
                    Dictionary<游戏对象属性, int> 装备属性;
                    装备属性 = value.装备属性;
                    int num;
                    num = 0;
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小攻击) ? 装备属性[游戏对象属性.最小攻击] : 0) + (装备属性.ContainsKey(游戏对象属性.最大攻击) ? 装备属性[游戏对象属性.最大攻击] : 0)) > 0)
                    {
                        dictionary3[0][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小魔法) ? 装备属性[游戏对象属性.最小魔法] : 0) + (装备属性.ContainsKey(游戏对象属性.最大魔法) ? 装备属性[游戏对象属性.最大魔法] : 0)) > 0)
                    {
                        dictionary3[1][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小道术) ? 装备属性[游戏对象属性.最小道术] : 0) + (装备属性.ContainsKey(游戏对象属性.最大道术) ? 装备属性[游戏对象属性.最大道术] : 0)) > 0)
                    {
                        dictionary3[2][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小刺术) ? 装备属性[游戏对象属性.最小刺术] : 0) + (装备属性.ContainsKey(游戏对象属性.最大刺术) ? 装备属性[游戏对象属性.最大刺术] : 0)) > 0)
                    {
                        dictionary3[3][value] = num;
                    }
                    if ((num = (装备属性.ContainsKey(游戏对象属性.最小弓术) ? 装备属性[游戏对象属性.最小弓术] : 0) + (装备属性.ContainsKey(游戏对象属性.最大弓术) ? 装备属性[游戏对象属性.最大弓术] : 0)) > 0)
                    {
                        dictionary3[4][value] = num;
                    }
                }
                List<KeyValuePair<byte, Dictionary<装备数据, int>>> 排序属性;
                排序属性 = (from x in dictionary3.ToList()
                        orderby ((KeyValuePair<byte, Dictionary<装备数据, int>>)x).Value.Values.Sum() descending
                        select x).ToList();
                List<KeyValuePair<byte, Dictionary<装备数据, int>>> list;
                list = 排序属性.Where((KeyValuePair<byte, Dictionary<装备数据, int>> O) => O.Value.Values.Sum() == 排序属性[0].Value.Values.Sum()).ToList();
                byte key;
                key = list[主程.随机数.Next(list.Count)].Key;
                List<KeyValuePair<装备数据, int>> list2;
                list2 = (from x in dictionary3[key].ToList()
                         orderby ((KeyValuePair<装备数据, int>)x).Value descending
                         select x).ToList();
                float num2;
                num2 = Math.Min(10f, (float)((list2.Count < 1) ? 1 : list2[0].Value) + (float)((list2.Count >= 2) ? list2[1].Value : 0) / 3f);
                int num3;
                num3 = dictionary2.Values.Sum((物品数据 x) => x.当前持久.V);
                float num4;
                num4 = Math.Max(0f, num3 - 146);
                float 概率;
                概率 = num2 * (float)(90 - v.升级次数.V * 10) * 0.001f + num4 * 0.01f;
                this.角色数据.升级装备.V = v;
                this.角色数据.取回时间.V = 主程.当前时间.AddHours(2.0);
                v.物品状态.V = 3;
                v.升级属性.V = byte.MaxValue;
                if (计算类.计算概率(概率) || v.失败次数.V >= 3)
                {
                    v.升级属性.V = key;
                }
                if (num3 < 30)
                {
                    v.扣除持久.V = 3000;
                }
                else if (num3 < 60)
                {
                    v.扣除持久.V = 2000;
                }
                else if (num3 < 99)
                {
                    v.扣除持久.V = 1000;
                }
                else if (num3 > 130 && 计算类.计算概率(1f - (float)v.最大持久.V * 0.5f / (float)v.默认持久))
                {
                    v.扣除持久.V = -1000;
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1856
                });
            }
        }

        public bool 玩家取回装备(int 扣除金币)
        {
            if (this.角色数据.升级装备.V == null)
            {
                this.对话页面 = 670503000;
                this.网络连接?.发送封包(new 同步交互结果
                {
                    对象编号 = this.对话守卫.地图编号,
                    交互文本 = 对话数据.字节数据(this.对话页面)
                });
                return false;
            }
            if (扣除金币 == 0 && 主程.当前时间 < this.角色数据.取回时间.V)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1855,
                    第一参数 = (int)(this.角色数据.取回时间.V - 主程.当前时间).TotalSeconds
                });
                return false;
            }
            if (扣除金币 > 0 && this.金币数量 < 扣除金币)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 2561
                });
                return false;
            }
            if (this.背包剩余 <= 0)
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return false;
            }
            byte b;
            b = 0;
            while (true)
            {
                if (b < this.背包大小)
                {
                    if (!this.角色背包.ContainsKey(b))
                    {
                        break;
                    }
                    b++;
                    continue;
                }
                主程.添加系统日志($"[取回装备失败] {this.角色数据.角色名字.V} 背包无空位, 装备保留: {this.角色数据.升级装备.V?.物品名字}", hardLog: true);
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 1793
                });
                return false;
            }
            this.金币数量 -= (uint)扣除金币;
            this.角色背包[b] = this.角色数据.升级装备.V;
            this.角色背包[b].物品位置.V = b;
            this.角色背包[b].物品容器.V = 1;
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = this.角色背包[b].字节描述()
            });
            this.网络连接?.发送封包(new 取回升级武器());
            this.角色数据.升级装备.V = null;
            return true;
        }

        public void 升级武器鉴定()
        {
            装备数据 装备数据;
            装备数据 = this.角色数据.角色装备[0];
            if (装备数据 == null || 装备数据.物品状态.V == 1)
            {
                return;
            }
            if (装备数据.升级属性.V != byte.MaxValue)
            {
                装备数据.升级次数.V++;
                switch (装备数据.升级属性.V)
                {
                    case 0:
                        装备数据.升级攻击.V++;
                        break;
                    case 1:
                        装备数据.升级魔法.V++;
                        break;
                    case 2:
                        装备数据.升级道术.V++;
                        break;
                    case 3:
                        装备数据.升级刺术.V++;
                        break;
                    case 4:
                        装备数据.升级弓术.V++;
                        break;
                }
                装备数据.最大持久.V -= 装备数据.扣除持久.V;
                装备数据.当前持久.V = Math.Min(装备数据.当前持久.V, 装备数据.最大持久.V);
                装备数据.失败次数.V = 0;
                装备数据.铸魂次数.V = 0;
                装备数据.升级属性.V = byte.MaxValue;
                装备数据.扣除持久.V = 0;
                装备数据.物品状态.V = 1;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.发送封包(new 武器升级结果());
                if (Settings.开启成就系统)
                {
                    this.成就变量赋值(AchievementVariables.WeaponUpLevelPoint, 装备数据.升级次数.V, UseMax: true);
                }
                if (装备数据.升级次数.V >= 5)
                {
                    网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{装备数据.物品编号}>><#P2:{装备数据.升级次数.V}><#T:MMOGame.DLG.ITEM.6>");
                }
            }
            else if (装备数据.物品状态.V == 3)
            {
                装备数据.物品状态.V = 1;
                装备数据.失败次数.V++;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.发送封包(new 武器升级结果
                {
                    升级结果 = 2
                });
            }
            else if (装备数据.物品状态.V == 4)
            {
                装备数据.物品状态.V = 1;
                装备数据.铸魂次数.V++;
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.发送封包(new 武器升级结果
                {
                    升级结果 = 3
                });
            }
            else
            {
                装备数据.物品状态.V = 1;
                if (Settings.普通强化不碎武器)
                {
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 删除玩家物品
                    {
                        背包类型 = 装备数据.物品容器.V,
                        物品位置 = 装备数据.物品位置.V
                    });
                    this.角色数据.角色装备.Remove(0);
                    this.玩家穿卸装备(装备穿戴部位.武器, 装备数据, null);
                    装备数据.删除数据();
                }
                this.网络连接?.发送封包(new 武器升级结果
                {
                    升级结果 = 1
                });
            }
        }

        public void 放弃升级武器()
        {
            this.角色数据.升级装备.V?.删除数据();
            this.角色数据.升级装备.V = null;
            this.网络连接?.发送封包(new 武器升级结果
            {
                升级结果 = 1
            });
        }

        public void 玩家发送广播(byte[] 数据)
        {
            if (!this.角色数据.管理员角色.V && this.当前等级 < Settings.聊天限制等级)
            {
                this.发送顶部公告("等级不够,不能发言.");
                return;
            }
            if (!this.角色数据.管理员角色.V && 主程.当前时间 < this.角色数据.禁言日期.V)
            {
                this.发送顶部公告("您已被禁言直到" + this.角色数据.禁言日期.V.ToString() + ".");
                return;
            }
            uint num;
            num = BitConverter.ToUInt32(数据, 0);
            byte b;
            b = 数据[4];
            byte[] array;
            array = 数据.Skip(5).ToArray();
            if (array[0] == 64)
            {
                if (array.Length <= 2)
                {
                    return;
                }
                if (GM命令.解析命令(Encoding.UTF8.GetString(array, 0, array.Length - 1), out var 命令))
                {
                    主程.添加聊天日志("[游戏命令]" + this.对象名字 + " " + 命令.GetType().Name, array);
                    if (this.管理员模式 && 命令 is GM在线命令 gM在线命令)
                    {
                        gM在线命令.执行命令(this);
                    }
                    else if (this.商人模式 && 命令 is 商人命令 商人命令)
                    {
                        商人命令.执行命令(this);
                    }
                    else if (命令 is 玩家命令 玩家命令)
                    {
                        玩家命令.执行命令(this);
                    }
                    else if (this.管理员模式)
                    {
                        命令.执行命令();
                    }
                    else
                    {
                        this.发送系统消息("命令格式错误或参数长度错误！");
                    }
                }
                else
                {
                    this.发送系统消息("命令格式错误或参数长度错误！");
                }
                return;
            }
            switch (num)
            {
                default:
                    this.网络连接?.尝试断开连接(new Exception($"玩家发送广播时, 提供错误的频道参数. 频道: {num:X8}"));
                    break;
                case 2415919107u:
                    {
                        switch (b)
                        {
                            case 1:
                                if (this.金币数量 < 1000)
                                {
                                    this.网络连接?.发送封包(new 游戏错误提示
                                    {
                                        错误代码 = 4873
                                    });
                                    return;
                                }
                                this.金币数量 -= 1000u;
                                主程.添加货币日志(this, "广播聊天扣除", 游戏货币.金币, -1000);
                                break;
                            default:
                                this.网络连接?.尝试断开连接(new Exception($"传音或广播时提供错误的频道参数, 断开连接. 频道: {num:X8}  参数:{b}"));
                                return;
                            case 6:
                                {
                                    if (!this.查找背包物品(2201, out var 物品))
                                    {
                                        this.网络连接?.发送封包(new 游戏错误提示
                                        {
                                            错误代码 = 4869
                                        });
                                        return;
                                    }
                                    this.消耗背包物品(1, 物品, "传音聊天扣除");
                                    break;
                                }
                        }
                        byte[] 字节描述2;
                        字节描述2 = null;
                        using (MemoryStream memoryStream2 = new MemoryStream())
                        {
                            using BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
                            binaryWriter2.Write(this.地图编号);
                            binaryWriter2.Write(2415919107u);
                            binaryWriter2.Write((int)b);
                            binaryWriter2.Write((int)this.当前等级);
                            binaryWriter2.Write(array);
                            binaryWriter2.Write(Encoding.UTF8.GetBytes(this.对象名字));
                            binaryWriter2.Write((byte)0);
                            字节描述2 = memoryStream2.ToArray();
                        }
                        网络服务网关.发送封包(new 接收聊天消息
                        {
                            字节描述 = 字节描述2
                        });
                        主程.添加聊天日志("[" + ((b == 1) ? "广播" : "传音") + "][" + this.对象名字 + "]: ", array);
                        break;
                    }
                case 2415919105u:
                    {
                        byte[] 字节描述;
                        字节描述 = null;
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                            binaryWriter.Write(2415919105u);
                            binaryWriter.Write(this.地图编号);
                            binaryWriter.Write(1);
                            binaryWriter.Write((int)this.当前等级);
                            binaryWriter.Write(array);
                            binaryWriter.Write(Encoding.UTF8.GetBytes(this.对象名字));
                            binaryWriter.Write((byte)0);
                            字节描述 = memoryStream.ToArray();
                        }
                        base.发送封包(new 接收聊天信息
                        {
                            字节描述 = 字节描述
                        });
                        主程.添加聊天日志("[附近][" + this.对象名字 + "]: ", array);
                        break;
                    }
            }
        }

        public void 发送错误消息(int 错误代码, int 第一参数 = 0, int 第二参数 = 0)
        {
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 错误代码,
                第一参数 = 第一参数,
                第二参数 = 第二参数
            });
        }

        public void 发送顶部公告(string 内容, bool 全服通知 = false, bool 滚动播报 = false)
        {
            using MemoryStream memoryStream = new MemoryStream();
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
            if (全服通知)
            {
                网络服务网关.发送封包(new 接收聊天消息
                {
                    字节描述 = memoryStream.ToArray()
                });
            }
            else
            {
                this.网络连接?.发送封包(new 接收聊天消息
                {
                    字节描述 = memoryStream.ToArray()
                });
            }
        }

        public void 发送系统消息(string message, bool 全服通知 = false)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(0);
            binaryWriter.Write(0);
            binaryWriter.Write(1);
            binaryWriter.Write((int)this.当前等级);
            binaryWriter.Write(Encoding.UTF8.GetBytes(message + "\0"));
            binaryWriter.Write(string.Empty);
            binaryWriter.Write((byte)0);
            if (全服通知)
            {
                网络服务网关.发送封包(new 接收聊天消息
                {
                    字节描述 = memoryStream.ToArray()
                });
            }
            else
            {
                this.网络连接?.发送封包(new 接收聊天消息
                {
                    字节描述 = memoryStream.ToArray()
                });
            }
        }

        public void 玩家发送消息(byte[] 数据)
        {
            if (!this.角色数据.管理员角色.V && this.当前等级 < Settings.聊天限制等级)
            {
                this.发送顶部公告("等级不够,不能发言.");
                return;
            }
            if (!this.角色数据.管理员角色.V && 主程.当前时间 < this.角色数据.禁言日期.V)
            {
                this.发送顶部公告("您已被禁言直到" + this.角色数据.禁言日期.V.ToString() + ".");
                return;
            }
            int num;
            num = BitConverter.ToInt32(数据, 0);
            byte[] array;
            array = 数据.Skip(4).ToArray();
            switch (num >> 28)
            {
                case 7:
                    {
                        if (this.所属队伍 == null)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 3853
                            });
                            break;
                        }
                        using MemoryStream memoryStream3 = new MemoryStream();
                        using BinaryWriter binaryWriter3 = new BinaryWriter(memoryStream3);
                        binaryWriter3.Write(this.地图编号);
                        binaryWriter3.Write(1879048192);
                        binaryWriter3.Write(1);
                        binaryWriter3.Write((int)this.当前等级);
                        binaryWriter3.Write(array);
                        binaryWriter3.Write(Encoding.UTF8.GetBytes(this.对象名字 + "\0"));
                        this.所属队伍.发送封包(new 接收聊天消息
                        {
                            字节描述 = memoryStream3.ToArray()
                        });
                        主程.添加聊天日志("[队伍][" + this.对象名字 + "]: ", array);
                        break;
                    }
                case 6:
                    {
                        if (this.所属行会 == null)
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 6668
                            });
                            break;
                        }
                        if (this.所属行会.行会禁言.ContainsKey(this.角色数据))
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 4870
                            });
                            break;
                        }
                        using MemoryStream memoryStream4 = new MemoryStream();
                        using BinaryWriter binaryWriter4 = new BinaryWriter(memoryStream4);
                        binaryWriter4.Write(this.地图编号);
                        binaryWriter4.Write(1610612736);
                        binaryWriter4.Write(1);
                        binaryWriter4.Write((int)this.当前等级);
                        binaryWriter4.Write(array);
                        binaryWriter4.Write(Encoding.UTF8.GetBytes(this.对象名字));
                        binaryWriter4.Write((byte)0);
                        this.所属行会.发送封包(new 接收聊天消息
                        {
                            字节描述 = memoryStream4.ToArray()
                        });
                        主程.添加聊天日志("[行会][" + this.对象名字 + "]: ", array);
                        break;
                    }
                case 0:
                    {
                        if (游戏数据网关.角色数据表.数据表.TryGetValue(num, out var value) && value is 角色数据 角色数据)
                        {
                            if (this.地图编号 == 角色数据.角色编号 || this.角色数据.黑名单表.Contains(this.角色数据) || 角色数据.网络连接 == null)
                            {
                                break;
                            }
                            byte[] 字节描述;
                            字节描述 = null;
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                                binaryWriter.Write(角色数据.角色编号);
                                binaryWriter.Write(this.地图编号);
                                binaryWriter.Write(1);
                                binaryWriter.Write((int)this.当前等级);
                                binaryWriter.Write(array);
                                binaryWriter.Write(Encoding.UTF8.GetBytes(this.对象名字));
                                binaryWriter.Write((byte)0);
                                字节描述 = memoryStream.ToArray();
                            }
                            this.网络连接?.发送封包(new 接收聊天消息
                            {
                                字节描述 = 字节描述
                            });
                            byte[] 字节描述2;
                            字节描述2 = null;
                            using (MemoryStream memoryStream2 = new MemoryStream())
                            {
                                using BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
                                binaryWriter2.Write(this.地图编号);
                                binaryWriter2.Write(角色数据.角色编号);
                                binaryWriter2.Write(1);
                                binaryWriter2.Write((int)this.当前等级);
                                binaryWriter2.Write(array);
                                binaryWriter2.Write(Encoding.UTF8.GetBytes(this.对象名字));
                                binaryWriter2.Write((byte)0);
                                字节描述2 = memoryStream2.ToArray();
                            }
                            角色数据.网络连接?.发送封包(new 接收聊天消息
                            {
                                字节描述 = 字节描述2
                            });
                            主程.添加聊天日志($"[私聊][{this.对象名字}]=>[{角色数据.角色名字}]: ", array);
                        }
                        else
                        {
                            this.网络连接?.发送封包(new 社交错误提示
                            {
                                错误编号 = 4868
                            });
                        }
                        break;
                    }
            }
        }

        public void 玩家好友聊天(byte[] 数据)
        {
            int key;
            key = BitConverter.ToInt32(数据, 0);
            byte[] array;
            array = 数据.Skip(4).ToArray();
            if (游戏数据网关.角色数据表.数据表.TryGetValue(key, out var value) && value is 角色数据 角色数据 && this.好友列表.Contains(角色数据))
            {
                if (角色数据.网络连接 == null)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5124
                    });
                    return;
                }
                byte[] 字节数据;
                字节数据 = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(this.地图编号);
                    binaryWriter.Write((int)this.当前等级);
                    binaryWriter.Write(array);
                    字节数据 = memoryStream.ToArray();
                }
                角色数据.网络连接?.发送封包(new 发送好友消息
                {
                    字节数据 = 字节数据
                });
                主程.添加聊天日志($"[好友][{this.对象名字}]=>[{角色数据}]: ", array);
            }
            else
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 4868
                });
            }
        }

        public void 玩家添加关注(int 对象编号, string 对象名字)
        {
            游戏数据 value2;
            if (this.偶像列表.Count >= 100)
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 5125
                });
            }
            else if (对象编号 != 0)
            {
                if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
                {
                    if (this.偶像列表.Contains(角色数据))
                    {
                        this.网络连接?.发送封包(new 社交错误提示
                        {
                            错误编号 = 5122
                        });
                        return;
                    }
                    if (this.黑名单表.Contains(角色数据))
                    {
                        this.玩家解除屏蔽(角色数据.角色编号);
                    }
                    if (this.仇人列表.Contains(角色数据))
                    {
                        this.玩家删除仇人(角色数据.角色编号);
                    }
                    this.偶像列表.Add(角色数据);
                    角色数据.粉丝列表.Add(this.角色数据);
                    if (this.开启七天乐)
                    {
                        this.修改七天进度(52, this.角色数据.七天进度[52] + 1);
                    }
                    this.网络连接?.发送封包(new 玩家添加关注
                    {
                        对象编号 = 角色数据.数据索引.V,
                        对象名字 = 角色数据.角色名字.V,
                        是否好友 = (this.粉丝列表.Contains(角色数据) || 角色数据.偶像列表.Contains(this.角色数据))
                    });
                    this.网络连接?.发送封包(new 好友上线下线
                    {
                        对象编号 = 角色数据.数据索引.V,
                        对象名字 = 角色数据.角色名字.V,
                        对象职业 = (byte)角色数据.角色职业.V,
                        对象性别 = (byte)角色数据.角色性别.V,
                        上线下线 = (byte)((角色数据.网络连接 == null) ? 3u : 0u)
                    });
                    if (this.粉丝列表.Contains(角色数据) || 角色数据.偶像列表.Contains(this.角色数据))
                    {
                        this.好友列表.Add(角色数据);
                        角色数据.好友列表.Add(this.角色数据);
                    }
                    角色数据.网络连接?.发送封包(new 对方关注自己
                    {
                        对象编号 = this.地图编号,
                        对象名字 = this.对象名字
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6732
                    });
                }
            }
            else if (游戏数据网关.角色数据表.检索表.TryGetValue(对象名字, out value2) && value2 is 角色数据 角色数据2)
            {
                if (this.偶像列表.Contains(角色数据2))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5122
                    });
                    return;
                }
                if (this.黑名单表.Contains(角色数据2))
                {
                    this.玩家解除屏蔽(角色数据2.角色编号);
                }
                if (this.仇人列表.Contains(角色数据2))
                {
                    this.玩家删除仇人(角色数据2.角色编号);
                }
                this.偶像列表.Add(角色数据2);
                if (this.开启七天乐)
                {
                    this.修改七天进度(52, this.角色数据.七天进度[52] + 1);
                }
                角色数据2.粉丝列表.Add(this.角色数据);
                this.网络连接?.发送封包(new 玩家添加关注
                {
                    对象编号 = 角色数据2.数据索引.V,
                    对象名字 = 角色数据2.角色名字.V,
                    是否好友 = (this.粉丝列表.Contains(角色数据2) || 角色数据2.偶像列表.Contains(this.角色数据))
                });
                this.网络连接?.发送封包(new 好友上线下线
                {
                    对象编号 = 角色数据2.数据索引.V,
                    对象名字 = 角色数据2.角色名字.V,
                    对象职业 = (byte)角色数据2.角色职业.V,
                    对象性别 = (byte)角色数据2.角色性别.V,
                    上线下线 = (byte)((角色数据2.网络连接 == null) ? 3u : 0u)
                });
                if (this.粉丝列表.Contains(角色数据2) || 角色数据2.偶像列表.Contains(this.角色数据))
                {
                    this.好友列表.Add(角色数据2);
                    角色数据2.好友列表.Add(this.角色数据);
                }
                角色数据2.网络连接?.发送封包(new 对方关注自己
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6732
                });
            }
        }

        public void 玩家取消关注(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (!this.偶像列表.Contains(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5121
                    });
                    return;
                }
                this.偶像列表.Remove(角色数据);
                角色数据.粉丝列表.Remove(this.角色数据);
                this.网络连接?.发送封包(new 玩家取消关注
                {
                    对象编号 = 角色数据.角色编号
                });
                if (this.好友列表.Contains(角色数据) || 角色数据.好友列表.Contains(this.角色数据))
                {
                    this.好友列表.Remove(角色数据);
                    角色数据.好友列表.Remove(this.角色数据);
                }
                角色数据.网络连接?.发送封包(new 对方取消关注
                {
                    对象编号 = this.地图编号,
                    对象名字 = this.对象名字
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6732
                });
            }
        }

        public void 玩家添加仇人(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (this.仇人列表.Count >= 100)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5125
                    });
                    return;
                }
                if (this.偶像列表.Contains(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5122
                    });
                    return;
                }
                this.仇人列表.Add(角色数据);
                角色数据.仇恨列表.Add(this.角色数据);
                this.网络连接?.发送封包(new 玩家标记仇人
                {
                    对象编号 = 角色数据.数据索引.V
                });
                this.网络连接?.发送封包(new 好友上线下线
                {
                    对象编号 = 角色数据.数据索引.V,
                    对象名字 = 角色数据.角色名字.V,
                    对象职业 = (byte)角色数据.角色职业.V,
                    对象性别 = (byte)角色数据.角色性别.V,
                    上线下线 = (byte)((角色数据.网络连接 == null) ? 3u : 0u)
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6732
                });
            }
        }

        public void 玩家删除仇人(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (!this.仇人列表.Contains(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 5126
                    });
                    return;
                }
                this.仇人列表.Remove(角色数据);
                角色数据.仇恨列表.Remove(this.角色数据);
                this.网络连接?.发送封包(new 玩家移除仇人
                {
                    对象编号 = 角色数据.数据索引.V
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6732
                });
            }
        }

        public void 玩家屏蔽目标(int 对象编号)
        {
            if (对象编号 == this.地图编号)
            {
                return;
            }
            if (地图处理网关.地图对象表.TryGetValue(对象编号, out var value) && value is 玩家实例 玩家实例2)
            {
                if (this.黑名单表.Count >= 100)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 7428
                    });
                    return;
                }
                if (this.黑名单表.Contains(玩家实例2.角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 7426
                    });
                    return;
                }
                if (对象编号 == this.地图编号)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 7429
                    });
                    return;
                }
                if (this.偶像列表.Contains(玩家实例2.角色数据))
                {
                    this.玩家取消关注(玩家实例2.角色数据.角色编号);
                }
                this.黑名单表.Add(玩家实例2.角色数据);
                this.网络连接?.发送封包(new 玩家屏蔽目标
                {
                    对象编号 = 玩家实例2.角色数据.数据索引.V,
                    对象名字 = 玩家实例2.角色数据.角色名字.V
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6732
                });
            }
        }

        public void 玩家解除屏蔽(int 对象编号)
        {
            if (游戏数据网关.角色数据表.数据表.TryGetValue(对象编号, out var value) && value is 角色数据 角色数据)
            {
                if (!this.黑名单表.Contains(角色数据))
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 7427
                    });
                }
                else
                {
                    this.黑名单表.Remove(角色数据);
                    this.网络连接?.发送封包(new 解除玩家屏蔽
                    {
                        对象编号 = 角色数据.数据索引.V
                    });
                }
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 6732
                });
            }
        }

        public void 请求对象外观(int 对象编号, int 状态编号)
        {
            if (!地图处理网关.地图对象表.TryGetValue(对象编号, out var value))
            {
                this.网络连接?.发送封包(new 社交错误提示
                {
                    错误编号 = 6732
                });
                return;
            }
            if (value is 玩家实例 玩家实例2)
            {
                this.网络连接?.发送封包(new 同步玩家外观
                {
                    对象编号 = 玩家实例2.地图编号,
                    对象PK值 = 玩家实例2.PK值惩罚,
                    对象职业 = (byte)玩家实例2.角色职业,
                    对象性别 = (byte)玩家实例2.角色性别,
                    对象发型 = (byte)玩家实例2.角色发型,
                    对象发色 = (byte)玩家实例2.角色发色,
                    对象脸型 = (byte)玩家实例2.角色脸型,
                    称号编号 = 玩家实例2.当前称号,
                    摆摊状态 = 玩家实例2.摆摊状态,
                    摊位名字 = 玩家实例2.摊位名字,
                    武器等级 = (byte)(玩家实例2.角色装备.TryGetValue(0, out var v) ? (v?.升级次数.V ?? 0) : 0),
                    身上武器 = (v?.对应模板.V.物品编号 ?? 0),
                    身上衣服 = (玩家实例2.角色装备.TryGetValue(1, out var v2) ? (v2?.对应模板?.V?.物品编号).GetValueOrDefault() : 0),
                    身上披风 = (玩家实例2.角色装备.TryGetValue(2, out var v3) ? (v3?.对应模板?.V?.物品编号).GetValueOrDefault() : 0),
                    当前体力 = 玩家实例2[游戏对象属性.最大体力],
                    当前魔力 = 玩家实例2[游戏对象属性.最大魔力],
                    对象名字 = 玩家实例2.对象名字,
                    行会编号 = (玩家实例2.所属行会?.数据索引.V ?? 0)
                });
                return;
            }
            if (value is 怪物实例 怪物实例2)
            {
                if (怪物实例2.出生地图 == null)
                {
                    this.网络连接?.发送封包(new 同步扩展数据
                    {
                        对象类型 = 1,
                        主人编号 = 0,
                        主人名字 = "",
                        对象等级 = 怪物实例2.当前等级,
                        对象编号 = 怪物实例2.地图编号,
                        模板编号 = 怪物实例2.模板编号,
                        当前等级 = 怪物实例2.宠物等级,
                        对象质量 = (byte)怪物实例2.怪物级别,
                        最大体力 = 怪物实例2[游戏对象属性.最大体力]
                    });
                }
                else
                {
                    this.网络连接?.发送封包(new 同步Npcc数据
                    {
                        对象编号 = 怪物实例2.地图编号,
                        对象等级 = 怪物实例2.当前等级,
                        对象质量 = (byte)怪物实例2.怪物级别,
                        对象模板 = (怪物实例2.对象模板?.怪物编号 ?? 0),
                        体力上限 = 怪物实例2[游戏对象属性.最大体力]
                    });
                }
                return;
            }
            客户网络 客户网络;
            同步扩展数据 同步扩展数据;
            object obj;
            if (value is 宠物实例 宠物实例2)
            {
                客户网络 = this.网络连接;
                if (客户网络 == null)
                {
                    return;
                }
                同步扩展数据 = new 同步扩展数据
                {
                    对象类型 = 2,
                    对象编号 = 宠物实例2.地图编号,
                    模板编号 = 宠物实例2.模板编号,
                    当前等级 = 宠物实例2.宠物等级,
                    对象等级 = 宠物实例2.当前等级,
                    对象质量 = (byte)宠物实例2.宠物级别,
                    最大体力 = 宠物实例2[游戏对象属性.最大体力],
                    主人编号 = (宠物实例2.宠物主人?.地图编号 ?? 0)
                };
                玩家实例 宠物主人;
                宠物主人 = 宠物实例2.宠物主人;
                if (宠物主人 == null)
                {
                    obj = null;
                }
                else
                {
                    obj = 宠物主人.对象名字;
                    if (obj != null)
                    {
                        goto IL_03da;
                    }
                }
                obj = "";
                goto IL_03da;
            }
            if (value is 守卫实例 守卫实例2)
            {
                this.网络连接?.发送封包(new 同步Npcc数据
                {
                    对象质量 = 3,
                    对象编号 = 守卫实例2.地图编号,
                    对象等级 = 守卫实例2.当前等级,
                    对象模板 = (守卫实例2.对象模板?.守卫编号 ?? 0),
                    体力上限 = 守卫实例2[游戏对象属性.最大体力]
                });
            }
            return;
        IL_03da:
            同步扩展数据.主人名字 = (string)obj;
            客户网络.发送封包(同步扩展数据);
        }

        public void 请求角色资料(int 角色编号)
        {
            客户网络 客户网络;
            同步角色信息 同步角色信息;
            object obj;
            if (游戏数据网关.角色数据表.数据表.TryGetValue(角色编号, out var value) && value is 角色数据 角色数据)
            {
                客户网络 = this.网络连接;
                if (客户网络 == null)
                {
                    return;
                }
                同步角色信息 = new 同步角色信息
                {
                    对象编号 = 角色数据.数据索引.V,
                    对象名字 = 角色数据.角色名字.V,
                    会员等级 = 角色数据.本期特权.V,
                    对象职业 = (byte)角色数据.角色职业.V,
                    对象性别 = (byte)角色数据.角色性别.V
                };
                行会数据 v;
                v = 角色数据.所属行会.V;
                if (v == null)
                {
                    obj = null;
                }
                else
                {
                    obj = v.行会名字.V;
                    if (obj != null)
                    {
                        goto IL_00c0;
                    }
                }
                obj = "";
                goto IL_00c0;
            }
            this.网络连接?.发送封包(new 社交错误提示
            {
                错误编号 = 6732
            });
            return;
        IL_00c0:
            同步角色信息.行会名字 = (string)obj;
            客户网络.发送封包(同步角色信息);
        }

        public void 查询玩家战力(int 对象编号)
        {
            if (地图处理网关.地图对象表.TryGetValue(对象编号, out var value) && value is 玩家实例 玩家实例2)
            {
                this.网络连接?.发送封包(new 同步玩家战力
                {
                    角色编号 = 玩家实例2.地图编号,
                    角色战力 = 玩家实例2.当前战力
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 7171
                });
            }
        }

        public void 查看对象装备(int 对象编号)
        {
            this.硬直次数 = 2;
            this.硬直时间 = 主程.当前时间.AddSeconds(1.0);
            if (地图处理网关.地图对象表.TryGetValue(对象编号, out var value) && value is 玩家实例 玩家实例2)
            {
                this.网络连接?.发送封包(new 同步角色装备
                {
                    对象编号 = 玩家实例2.地图编号,
                    装备数量 = (byte)玩家实例2.角色装备.Count,
                    字节描述 = 玩家实例2.装备物品描述()
                });
                this.网络连接?.发送封包(new 同步玛法特权
                {
                    玛法特权 = 玩家实例2.本期特权
                });
                this.网络连接?.发送封包(new 同步传奇之力
                {
                    传奇之力 = 玩家实例2.角色数据.传奇之力等级,
                    对象编号 = 玩家实例2.地图编号
                });
                this.网络连接?.发送封包(new 天赋之力数据
                {
                    数据数组 = 玩家实例2.获取天赋描述()
                });
            }
            else
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 7171
                });
            }
        }

        public void 查询排名榜单(int 榜单类型, int 起始位置)
        {
            if (起始位置 < 0 || 起始位置 > 29)
            {
                return;
            }
            byte b;
            b = (byte)榜单类型;
            int num;
            num = 0;
            int num2;
            num2 = 起始位置 * 10;
            int num3;
            num3 = 起始位置 * 10 + 10;
            列表监视器<角色数据> 列表监视器;
            列表监视器 = null;
            switch (榜单类型)
            {
                case 37:
                    列表监视器 = 系统数据.数据.龙枪战力排名;
                    num = 1;
                    break;
                case 36:
                    列表监视器 = 系统数据.数据.龙枪等级排名;
                    num = 0;
                    break;
                case 0:
                    列表监视器 = 系统数据.数据.个人等级排名;
                    num = 0;
                    break;
                case 1:
                    列表监视器 = 系统数据.数据.战士等级排名;
                    num = 0;
                    break;
                case 2:
                    列表监视器 = 系统数据.数据.法师等级排名;
                    num = 0;
                    break;
                case 3:
                    列表监视器 = 系统数据.数据.道士等级排名;
                    num = 0;
                    break;
                case 4:
                    列表监视器 = 系统数据.数据.刺客等级排名;
                    num = 0;
                    break;
                case 5:
                    列表监视器 = 系统数据.数据.弓手等级排名;
                    num = 0;
                    break;
                case 6:
                    列表监视器 = 系统数据.数据.个人战力排名;
                    num = 1;
                    break;
                case 7:
                    列表监视器 = 系统数据.数据.战士战力排名;
                    num = 1;
                    break;
                case 8:
                    列表监视器 = 系统数据.数据.法师战力排名;
                    num = 1;
                    break;
                case 9:
                    列表监视器 = 系统数据.数据.道士战力排名;
                    num = 1;
                    break;
                case 10:
                    列表监视器 = 系统数据.数据.刺客战力排名;
                    num = 1;
                    break;
                case 11:
                    列表监视器 = 系统数据.数据.弓手战力排名;
                    num = 1;
                    break;
                case 14:
                    列表监视器 = 系统数据.数据.个人声望排名;
                    num = 2;
                    break;
                case 15:
                    列表监视器 = 系统数据.数据.个人PK值排名;
                    num = 3;
                    break;
            }
            if (列表监视器 == null || 列表监视器.Count == 0 || this.封包间隔限制("查询排名榜单"))
            {
                return;
            }
            using MemoryStream memoryStream = new MemoryStream(new byte[189]);
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(b);
            binaryWriter.Write((ushort)this.角色数据.当前排名[b]);
            binaryWriter.Write((ushort)this.角色数据.历史排名[b]);
            binaryWriter.Write(列表监视器.Count);
            for (int i = num2; i < num3; i++)
            {
                binaryWriter.Write((long)(列表监视器[i]?.角色编号 ?? 0));
            }
            for (int j = num2; j < num3; j++)
            {
                switch (num)
                {
                    default:
                        binaryWriter.Write(0);
                        break;
                    case 0:
                        binaryWriter.Write((long)((ulong)(列表监视器[j]?.角色等级 ?? 0) << 56));
                        break;
                    case 1:
                        binaryWriter.Write((long)(列表监视器[j]?.角色战力 ?? 0));
                        break;
                    case 2:
                        binaryWriter.Write((long)(列表监视器[j]?.师门声望 ?? 0));
                        break;
                    case 3:
                        binaryWriter.Write((long)(列表监视器[j]?.角色PK值 ?? 0));
                        break;
                }
            }
            for (int k = num2; k < num3; k++)
            {
                binaryWriter.Write((ushort)(列表监视器[k]?.历史排名[b] ?? 0));
            }
            this.网络连接?.发送封包(new 查询排行榜单
            {
                字节数据 = memoryStream.ToArray()
            });
            this.记录封包间隔("查询排名榜单");
        }

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
                    标题 = Encoding.UTF8.GetString(array2).Split(new char[1], StringSplitOptions.RemoveEmptyEntries)[0];
                    string 正文;
                    正文 = Encoding.UTF8.GetString(array3).Split(new char[1], StringSplitOptions.RemoveEmptyEntries)[0];
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
                    if (游戏数据网关.行会数据表.检索表.ContainsKey(array[0]))
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
                    this.所属行会 = new 行会数据(this, array[0], array2[0]);
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
                    this.所属行会.更改公告(Encoding.UTF8.GetString(数据).Split('\0')[0]);
                }
            }
            else
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 更改行会公告. 错误: 数据长度错误"));
            }
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
                    this.所属行会.更改宣言(this.角色数据, Encoding.UTF8.GetString(数据).Split('\0')[0]);
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
            if (原来容器 != 1 || 原来位置 >= this.背包大小 || 仓库位置 >= 56)
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
            if (仓库位置 >= 56 || 目标容器 != 1 || 目标位置 >= this.背包大小)
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
                    this.金币数量 += num;
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
            else if (金币数量 > 0 && this.金币数量 >= 金币数量 + (int)Math.Ceiling((float)金币数量 * 0.04f))
            {
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
            this.当前摊位.摊位名字 = 摊位名字;
            base.发送封包(new 变更摊位名字
            {
                对象编号 = this.地图编号,
                摊位名字 = 摊位名字
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

        public byte[] 玩家属性描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            for (byte b = 0; b <= 200; b++)
            {
                if (Enum.TryParse<游戏对象属性>(b.ToString(), out var result) && Enum.IsDefined(typeof(游戏对象属性), result))
                {
                    binaryWriter.Write(b);
                    binaryWriter.Write(this[result]);
                    binaryWriter.Write(new byte[2]);
                }
                else
                {
                    binaryWriter.Write(b);
                    binaryWriter.Write(new byte[6]);
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部技能描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            List<ushort> list;
            list = this.主体技能表.Keys.ToList();
            foreach (技能数据 value in this.主体技能表.Values)
            {
                foreach (ushort item in value.被动技能)
                {
                    list.Add(item);
                }
            }
            list = list.Where((ushort x) => this.主体技能表.ContainsKey(x)).ToList();
            list.Sort();
            binaryWriter.Write(0);
            binaryWriter.Write(list.Count);
            foreach (ushort item2 in list)
            {
                binaryWriter.Write(item2);
                binaryWriter.Write(this.主体技能表[item2].技能经验.V);
                binaryWriter.Write(this.主体技能表[item2].铭文编号);
                binaryWriter.Write(this.主体技能表[item2].技能等级.V);
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部冷却描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (KeyValuePair<int, DateTime> item in this.冷却记录)
            {
                if (!(主程.当前时间 >= item.Value))
                {
                    binaryWriter.Write(item.Key);
                    binaryWriter.Write((int)(item.Value - 主程.当前时间).TotalMilliseconds);
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部Buff描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (Buff数据 value in this.Buff列表.Values)
            {
                value.剩余时间.V = (value.离线计算 ? (value.添加时间.V.Add(value.持续时间.V) - 主程.当前时间) : value.剩余时间.V);
                binaryWriter.Write(value.Buff编号.V);
                binaryWriter.Write((int)value.Buff编号.V);
                binaryWriter.Write(value.当前层数.V);
                binaryWriter.Write((int)value.剩余时间.V.TotalMilliseconds);
                binaryWriter.Write((int)value.持续时间.V.TotalMilliseconds);
            }
            return memoryStream.ToArray();
        }

        public byte[] 快捷栏位描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            for (int i = 0; i < 32; i++)
            {
                binaryWriter.Write((byte)i);
                if (this.快捷栏位.TryGetValue((byte)i, out var v))
                {
                    binaryWriter.Write(v?.技能编号.V ?? 0);
                }
                else
                {
                    binaryWriter.Write((ushort)0);
                }
                binaryWriter.Write(value: false);
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部货币描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            for (int i = 0; i <= 19; i++)
            {
                binaryWriter.Seek(i * 48, SeekOrigin.Begin);
                if (i == 3)
                {
                    binaryWriter.Write(this.元宝数量);
                }
                else
                {
                    binaryWriter.Write(this.角色数据.角色货币[(游戏货币)i]);
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部称号描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.当前称号);
            binaryWriter.Write((byte)this.称号列表.Count);
            foreach (KeyValuePair<byte, DateTime> item in this.称号列表)
            {
                binaryWriter.Write(item.Key);
                binaryWriter.Write((item.Value == DateTime.MaxValue) ? uint.MaxValue : ((uint)(item.Value - 主程.当前时间).TotalMinutes));
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部物品描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (装备数据 item in this.角色装备.Values.ToList())
            {
                if (item != null)
                {
                    binaryWriter.Write(item.字节描述());
                }
            }
            foreach (物品数据 item2 in this.角色背包.Values.ToList())
            {
                if (item2 != null)
                {
                    binaryWriter.Write(item2.字节描述());
                }
            }
            foreach (物品数据 item3 in this.角色仓库.Values.ToList())
            {
                if (item3 != null)
                {
                    binaryWriter.Write(item3.字节描述());
                }
            }
            foreach (物品数据 item4 in this.角色资源包.Values.ToList())
            {
                if (item4 != null)
                {
                    binaryWriter.Write(item4.字节描述());
                }
            }
            if (this.角色数据.挂载物品.V != null)
            {
                binaryWriter.Write(this.角色数据.挂载物品.V.字节描述());
            }
            return memoryStream.ToArray();
        }

        public byte[] 全部邮件描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((ushort)Math.Min(this.全部邮件.Count, 500));
            int num;
            num = 0;
            foreach (邮件数据 item in this.全部邮件)
            {
                if (num == 500)
                {
                    break;
                }
                binaryWriter.Write(item.邮件检索描述());
                num++;
            }
            return memoryStream.ToArray();
        }

        public byte[] 背包物品描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (物品数据 item in this.角色背包.Values.ToList())
            {
                if (item != null)
                {
                    binaryWriter.Write(item.字节描述());
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 仓库物品描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (物品数据 item in this.角色仓库.Values.ToList())
            {
                if (item != null)
                {
                    binaryWriter.Write(item.字节描述());
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 资源包物品描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (物品数据 item in this.角色资源包.Values.ToList())
            {
                if (item != null)
                {
                    binaryWriter.Write(item.字节描述());
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 装备物品描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (装备数据 item in this.角色装备.Values.ToList())
            {
                if (item != null)
                {
                    binaryWriter.Write(item.字节描述());
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 玛法特权描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.角色数据.预定特权.V);
            binaryWriter.Write(this.本期特权);
            binaryWriter.Write((this.本期特权 != 0) ? 计算类.时间转换(this.本期日期) : 0);
            binaryWriter.Write((this.本期特权 != 0) ? this.本期记录 : 0u);
            binaryWriter.Write(this.上期特权);
            binaryWriter.Write((this.上期特权 != 0) ? 计算类.时间转换(this.上期日期) : 0);
            binaryWriter.Write((this.上期特权 != 0) ? this.上期记录 : 0u);
            binaryWriter.Write((byte)7);
            for (byte b = 1; b <= 7; b++)
            {
                binaryWriter.Write(b);
                binaryWriter.Write(this.剩余特权.TryGetValue(b, out var v) ? v : 0);
            }
            return memoryStream.ToArray();
        }

        public byte[] 社交列表描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)this.好友列表.Count);
            binaryWriter.Write((byte)(this.偶像列表.Count + this.仇人列表.Count));
            foreach (角色数据 item in this.偶像列表)
            {
                binaryWriter.Write(item.数据索引.V);
                byte[] array;
                array = new byte[69];
                byte[] array2;
                array2 = item.名字描述();
                Buffer.BlockCopy(array2, 0, array, 0, array2.Length);
                binaryWriter.Write(array);
                binaryWriter.Write((byte)item.角色职业.V);
                binaryWriter.Write((byte)item.角色性别.V);
                binaryWriter.Write((byte)((item.网络连接 == null) ? 3u : 0u));
                binaryWriter.Write(0u);
                binaryWriter.Write((byte)0);
                binaryWriter.Write(this.好友列表.Contains(item) ? ((byte)1) : ((byte)0));
            }
            foreach (角色数据 item2 in this.仇人列表)
            {
                binaryWriter.Write(item2.数据索引.V);
                byte[] array3;
                array3 = new byte[69];
                byte[] array4;
                array4 = item2.名字描述();
                Buffer.BlockCopy(array4, 0, array3, 0, array4.Length);
                binaryWriter.Write(array3);
                binaryWriter.Write((byte)item2.角色职业.V);
                binaryWriter.Write((byte)item2.角色性别.V);
                binaryWriter.Write((byte)((item2.网络连接 == null) ? 3u : 0u));
                binaryWriter.Write(0u);
                binaryWriter.Write((byte)21);
                binaryWriter.Write((byte)0);
            }
            return memoryStream.ToArray();
        }

        public byte[] 社交屏蔽描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)this.黑名单表.Count);
            foreach (角色数据 item in this.黑名单表)
            {
                binaryWriter.Write(item.数据索引.V);
            }
            return memoryStream.ToArray();
        }

        public byte[] 获取天赋描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.地图编号);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)152);
            binaryWriter.Write((byte)103);
            if (!this.角色数据.天赋等级.ContainsKey(55))
            {
                this.角色数据.天赋等级.Add(55, 0);
                this.角色数据.天赋经验.Add(55, 0);
                this.角色数据.天赋刻印.Add(55, int.MaxValue);
            }
            foreach (KeyValuePair<byte, int> item in this.角色数据.天赋等级)
            {
                binaryWriter.Write(this.角色数据.天赋等级[item.Key]);
                binaryWriter.Write(this.角色数据.天赋经验[item.Key]);
                binaryWriter.Write(this.角色数据.天赋刻印[item.Key]);
            }
            return memoryStream.ToArray();
        }

        public void 发送天赋描述()
        {
            this.网络连接?.发送封包(new 天赋之力数据
            {
                数据数组 = this.获取天赋描述()
            });
        }

        public void 发送坐骑描述()
        {
            this.御兽之力等级 = 0;
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (byte item in this.坐骑列表)
            {
                if (游戏坐骑.数据表.TryGetValue(item, out var value))
                {
                    this.御兽之力等级 += value.御兽之力;
                    binaryWriter.Write((ushort)item);
                }
            }
            base.发送封包(new 同步坐骑数据
            {
                坐骑编号 = memoryStream.ToArray()
            });
            base.发送封包(new 坐骑面板回执
            {
                编号 = this.当前坐骑
            });
            for (int i = 0; i < 4; i++)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 2056,
                    第一参数 = i,
                    第二参数 = this.御兽列表[(byte)i]
                });
            }
            if (游戏坐骑.御兽之力属性.TryGetValue((byte)this.御兽之力等级, out var value2))
            {
                base.属性加成[游戏坐骑.御兽之力属性] = value2;
                this.战力加成[游戏坐骑.御兽之力属性] = this.御兽之力等级 * 10;
                this.更新玩家战力();
                this.更新对象属性();
            }
        }

        public void 计算玩家额外暴率()
        {
            this.额外爆率 = this[游戏对象属性.玛法特权给的属性] / 100000;
            if (Settings.可购买玛法特权)
            {
                switch (this.本期特权)
                {
                    case 3:
                        this.额外爆率 += 0.2m;
                        break;
                    case 4:
                    case 5:
                        this.额外爆率 += 0.3m;
                        break;
                    case 6:
                        this.额外爆率 += 0.175m;
                        break;
                    case 7:
                        this.额外爆率 += 0.35m;
                        break;
                }
            }
            if (this.Buff列表.ContainsKey(2536))
            {
                this.额外爆率 += 0.15m;
            }
            if (this.Buff列表.ContainsKey(2537))
            {
                this.额外爆率 += 0.2m;
            }
        }

        public float 获取龙卫增加治疗(地图对象 地图对象, ushort 技能编号, ushort 铭文编号)
        {
            float num;
            num = 0f;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if ((!龙卫模板.检测铭文 || 龙卫模板.绑定铭文 == 铭文编号) && 龙卫模板.绑定技能 != null && 龙卫模板.提升治疗总量)
                {
                    if (!地图对象.特定类型(this, 龙卫模板.目标类型))
                    {
                        return num;
                    }
                    if (Array.IndexOf(龙卫模板.绑定技能, 技能编号) != -1)
                    {
                        num += (float)item.Value * 0.001f;
                    }
                }
            }
            return num;
        }

        public float 获取龙卫增伤系数(地图对象 地图对象, ushort 技能编号, ushort 铭文编号)
        {
            float num;
            num = 0f;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (!龙卫模板.检测铭文 || 龙卫模板.绑定铭文 == 铭文编号)
                {
                    if (!地图对象.特定类型(this, 龙卫模板.目标类型))
                    {
                        return num;
                    }
                    if (龙卫模板.增伤地图 != null && Array.IndexOf(龙卫模板.增伤地图, 地图对象.当前地图.地图编号) != -1)
                    {
                        num += (float)item.Value * 0.0001f;
                    }
                    else if (龙卫模板.增伤技能 != null && Array.IndexOf(龙卫模板.增伤技能, 技能编号) != -1)
                    {
                        num += (float)item.Value * 0.0001f;
                    }
                }
            }
            return num;
        }

        public float 获取龙卫减伤系数(地图对象 地图对象, ushort 技能编号, ushort 铭文编号)
        {
            float num;
            num = 0f;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (!龙卫模板.检测铭文 || 龙卫模板.绑定铭文 == 铭文编号)
                {
                    if (base.特定类型(地图对象, 龙卫模板.目标类型))
                    {
                        return num;
                    }
                    if (龙卫模板.减伤地图 != null && Array.IndexOf(龙卫模板.减伤地图, 地图对象.当前地图.地图编号) != -1)
                    {
                        num += (float)item.Value * 0.0001f;
                    }
                    else if (龙卫模板.减伤技能 != null && Array.IndexOf(龙卫模板.减伤技能, 技能编号) != -1)
                    {
                        num += (float)item.Value * 0.0001f;
                    }
                }
            }
            return num;
        }

        public float 获取龙卫特定减伤系数(bool 物理减伤, bool 魔法减伤)
        {
            float num;
            num = 0f;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.物理系数减伤 == 物理减伤 && 龙卫模板.魔法系数减伤 == 魔法减伤)
                {
                    num += (float)item.Value * 0.0001f;
                }
            }
            return num;
        }

        public void 龙卫添加自身BUFF(ushort 技能编号, ushort 铭文编号)
        {
            int num;
            num = 0;
            ushort 编号;
            编号 = 0;
            bool flag;
            flag = false;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if ((!龙卫模板.检测铭文 || 龙卫模板.绑定铭文 == 铭文编号) && 龙卫模板.绑定技能 != null && 龙卫模板.自身增加BUFF != 0 && Array.IndexOf(龙卫模板.绑定技能, 技能编号) != -1)
                {
                    num += item.Value;
                    编号 = 龙卫模板.自身增加BUFF;
                    flag = 龙卫模板.自身不算几率;
                }
            }
            if (num != 0 && (计算类.计算概率((float)num * 0.0001f) || flag))
            {
                base.添加Buff时处理(编号, this);
            }
        }

        public void 龙卫添加目标BUFF(ushort 技能编号, ushort 铭文编号, 地图对象 目标)
        {
            float num;
            num = 0f;
            ushort 编号;
            编号 = 0;
            bool flag;
            flag = false;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (目标.特定类型(this, 龙卫模板.目标类型) && (!龙卫模板.检测铭文 || 龙卫模板.绑定铭文 == 铭文编号) && 龙卫模板.绑定技能 != null && 龙卫模板.目标增加BUFF != 0 && Array.IndexOf(龙卫模板.绑定技能, 技能编号) != -1)
                {
                    num += (float)item.Value;
                    编号 = 龙卫模板.目标增加BUFF;
                    flag = 龙卫模板.目标不算几率;
                }
            }
            if (num != 0f && (计算类.计算概率(num * 0.0001f) || flag))
            {
                目标.添加Buff时处理(编号, this);
            }
        }

        public int 龙卫BUFF伤害基数(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.BUFF伤害基数 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return num;
        }

        public float 龙卫BUFF伤害系数(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.BUFF伤害系数 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return (float)num * 0.0001f;
        }

        public int 龙卫自身伤害基数(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.自身伤害基数 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return num;
        }

        public float 龙卫自身伤害系数(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.自身伤害系数 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return (float)num * 0.0001f;
        }

        public int 龙卫BUFF持续时间(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.延长BUFF时间 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return num;
        }

        public int 龙卫BUFF增加护盾(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.增加BUFF护盾 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return num;
        }

        public Dictionary<游戏对象属性, int> 龙卫BUFF神圣攻击(ushort Buff编号)
        {
            Dictionary<游戏对象属性, int> dictionary;
            dictionary = null;
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.BUFF神圣攻击 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            if (num > 0)
            {
                dictionary = new Dictionary<游戏对象属性, int>();
                dictionary.Add(游戏对象属性.最大圣伤, num);
                dictionary.Add(游戏对象属性.最小圣伤, num / 2);
            }
            return dictionary;
        }

        public float 龙卫BUFF触发概率(ushort Buff编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.触发BUFF概率 && 龙卫模板.检测BUFF编号 != null && Array.IndexOf(龙卫模板.检测BUFF编号, Buff编号) != -1)
                {
                    num += item.Value;
                }
            }
            return (float)num * 0.0001f;
        }

        public float 龙卫技能触发概率(ushort 技能编号)
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.触发技能概率 && 龙卫模板.绑定技能 != null && Array.IndexOf(龙卫模板.绑定技能, 技能编号) != -1)
                {
                    num += item.Value;
                }
            }
            return (float)num * 0.0001f;
        }

        public int 龙卫BUFF诱惑时间()
        {
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                if (龙卫模板.数据表[item.Key].延长诱惑时间)
                {
                    num += item.Value;
                }
            }
            return num;
        }

        public void 龙卫召唤物加BUFF(ushort 技能编号, 宠物实例 宠物)
        {
            Dictionary<ushort, int> dictionary;
            dictionary = new Dictionary<ushort, int>();
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if (龙卫模板.召唤物加BUFF != 0 && (龙卫模板.检测已学技能 <= 0 || this.主体技能表.ContainsKey(龙卫模板.检测已学技能)) && (龙卫模板.绑定技能 == null || Array.IndexOf(龙卫模板.绑定技能, 技能编号) != -1))
                {
                    if (!dictionary.ContainsKey(龙卫模板.召唤物加BUFF))
                    {
                        dictionary.Add(龙卫模板.召唤物加BUFF, item.Value);
                    }
                    else
                    {
                        dictionary[龙卫模板.召唤物加BUFF] += item.Value;
                    }
                }
            }
            foreach (KeyValuePair<ushort, int> item2 in dictionary)
            {
                Buff数据 buff数据;
                buff数据 = 宠物.添加Buff时处理(item2.Key, this);
                if (buff数据.Buff模板.继承主人属性 != 0)
                {
                    buff数据.继承属性比例 += (float)item2.Value * 0.0001f;
                }
                if (buff数据.Buff模板.龙卫宠物基数)
                {
                    buff数据.增减伤害基数 += item2.Value;
                }
                if ((buff数据.Buff模板.Buff效果 & Buff效果类型.属性增减) != 0)
                {
                    buff数据.增减属性基数 = item2.Value;
                    if (宠物.属性加成.ContainsKey(buff数据))
                    {
                        宠物.属性加成.Remove(buff数据);
                    }
                    宠物.属性加成.Add(buff数据, buff数据.属性加成);
                    宠物.更新对象属性();
                }
            }
        }

        public void 龙卫释放减少冷却(ushort 技能编号, ushort 铭文编号)
        {
            float num;
            num = 0f;
            int num2;
            num2 = 0;
            byte b;
            b = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if ((龙卫模板.检测铭文 && 龙卫模板.绑定铭文 != 铭文编号) || 龙卫模板.绑定技能 == null || (龙卫模板.减少冷却编号 == 0 && 龙卫模板.减少分组编号 == 0) || Array.IndexOf(龙卫模板.绑定技能, 技能编号) == -1)
                {
                    continue;
                }
                if (龙卫模板.释放减少冷却)
                {
                    if (龙卫模板.减少冷却编号 != 0 && this.冷却记录.TryGetValue(龙卫模板.减少冷却编号 | 0x1000000, out var v))
                    {
                        v -= TimeSpan.FromMilliseconds(item.Value);
                        this.冷却记录[龙卫模板.减少冷却编号 | 0x1000000] = v;
                        base.发送封包(new 添加技能冷却
                        {
                            冷却编号 = (龙卫模板.减少冷却编号 | 0x1000000),
                            冷却时间 = Math.Max(0, (int)(v - 主程.当前时间).TotalMilliseconds)
                        });
                    }
                    if (龙卫模板.减少分组编号 != 0 && this.冷却记录.TryGetValue(龙卫模板.减少分组编号 | 0, out var v2))
                    {
                        v2 -= TimeSpan.FromMilliseconds(item.Value);
                        this.冷却记录[龙卫模板.减少分组编号 | 0] = v2;
                        base.发送封包(new 添加技能冷却
                        {
                            冷却编号 = (龙卫模板.减少分组编号 | 0),
                            冷却时间 = Math.Max(0, (int)(v2 - 主程.当前时间).TotalMilliseconds)
                        });
                    }
                }
                else if (龙卫模板.刷新技能冷却)
                {
                    num2 = 龙卫模板.减少冷却编号;
                    b = 龙卫模板.减少分组编号;
                    num += (float)item.Value;
                }
            }
            if (计算类.计算概率(num * 0.0001f))
            {
                if (num2 != 0 && this.冷却记录.TryGetValue(num2 | 0x1000000, out var _))
                {
                    this.冷却记录[num2 | 0x1000000] = 主程.当前时间;
                    base.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (num2 | 0x1000000),
                        冷却时间 = 0
                    });
                }
                if (b != 0 && this.冷却记录.TryGetValue(b | 0, out var _))
                {
                    this.冷却记录[b | 0] = 主程.当前时间;
                    base.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (b | 0),
                        冷却时间 = 0
                    });
                }
            }
        }

        public void 龙卫命中减少冷却(ushort 技能编号, ushort 铭文编号, Dictionary<int, 命中详情> 目标列表)
        {
            int num;
            num = 0;
            ushort num2;
            num2 = 0;
            byte b;
            b = 0;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if ((龙卫模板.检测铭文 && 龙卫模板.绑定铭文 != 铭文编号) || 龙卫模板.绑定技能 == null || (龙卫模板.减少冷却编号 == 0 && 龙卫模板.减少分组编号 == 0) || !龙卫模板.命中减少冷却)
                {
                    continue;
                }
                bool flag;
                flag = false;
                int num3;
                num3 = 0;
                if (Array.IndexOf(龙卫模板.绑定技能, 技能编号) != -1)
                {
                    foreach (KeyValuePair<int, 命中详情> item2 in 目标列表)
                    {
                        if (item2.Value.技能目标.特定类型(this, 龙卫模板.目标类型))
                        {
                            num3 += item.Value;
                        }
                        else if (龙卫模板.命中异类放弃)
                        {
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    num += num3;
                    num2 = 龙卫模板.减少冷却编号;
                    b = 龙卫模板.减少分组编号;
                }
            }
            if (num != 0 && num > 0)
            {
                if (num2 != 0 && this.冷却记录.TryGetValue(num2 | 0x1000000, out var v))
                {
                    v -= TimeSpan.FromMilliseconds(num);
                    this.冷却记录[num2 | 0x1000000] = v;
                    base.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (num2 | 0x1000000),
                        冷却时间 = Math.Max(0, (int)(v - 主程.当前时间).TotalMilliseconds)
                    });
                }
                if (b != 0 && this.冷却记录.TryGetValue(b | 0, out var v2))
                {
                    v2 -= TimeSpan.FromMilliseconds(num);
                    this.冷却记录[b | 0] = v2;
                    base.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (b | 0),
                        冷却时间 = Math.Max(0, (int)(v2 - 主程.当前时间).TotalMilliseconds)
                    });
                }
            }
        }

        public void 龙卫命中概率减少(ushort 技能编号, ushort 铭文编号, Dictionary<int, 命中详情> 目标列表)
        {
            int num;
            num = 0;
            ushort num2;
            num2 = 0;
            byte b;
            b = 0;
            float num3;
            num3 = 0f;
            foreach (KeyValuePair<ushort, int> item in this.生效龙卫)
            {
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.数据表[item.Key];
                if ((龙卫模板.检测铭文 && 龙卫模板.绑定铭文 != 铭文编号) || 龙卫模板.绑定技能 == null || (龙卫模板.减少冷却编号 == 0 && 龙卫模板.减少分组编号 == 0) || !龙卫模板.命中概率减少 || Array.IndexOf(龙卫模板.绑定技能, 技能编号) == -1)
                {
                    continue;
                }
                bool flag;
                flag = false;
                int num4;
                num4 = 0;
                foreach (KeyValuePair<int, 命中详情> item2 in 目标列表)
                {
                    if (龙卫模板.命中异类放弃 && !item2.Value.技能目标.特定类型(this, 龙卫模板.目标类型))
                    {
                        flag = true;
                    }
                    num4 += 龙卫模板.减少冷却时间;
                }
                if (!flag)
                {
                    num3 += (float)item.Value;
                    num = num4;
                    num2 = 龙卫模板.减少冷却编号;
                    b = 龙卫模板.减少分组编号;
                }
            }
            if (num != 0 && num3 != 0f && num > 0 && 计算类.计算概率(num3 * 0.0001f))
            {
                if (num2 != 0 && this.冷却记录.TryGetValue(num2 | 0x1000000, out var v))
                {
                    v -= TimeSpan.FromMilliseconds(num);
                    this.冷却记录[num2 | 0x1000000] = v;
                    base.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (num2 | 0x1000000),
                        冷却时间 = Math.Max(0, (int)(v - 主程.当前时间).TotalMilliseconds)
                    });
                }
                if (b != 0 && this.冷却记录.TryGetValue(b | 0, out var v2))
                {
                    v2 -= TimeSpan.FromMilliseconds(num);
                    this.冷却记录[b | 0] = v2;
                    base.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (b | 0),
                        冷却时间 = Math.Max(0, (int)(v2 - 主程.当前时间).TotalMilliseconds)
                    });
                }
            }
        }

        public void 龙卫传承重塑(byte 部位编号, byte 模式, byte 附加)
        {
            if (this.角色数据.龙卫属性.Where((龙卫数据 r) => r.装备位置 == 部位编号).Count() == 0)
            {
                return;
            }
            if (this.管理员模式)
            {
                IEnumerable<龙卫数据> enumerable;
                enumerable = this.角色数据.龙卫属性.Where((龙卫数据 r) => r.装备位置 == 部位编号);
                foreach (龙卫数据 item in enumerable)
                {
                    this.发送系统消息($"{item.当前阶段} {item.龙卫模板.词缀类型} {item.龙卫模板.占位数量}");
                }
                this.发送系统消息($"{enumerable.Count()}");
            }
            int num;
            num = 0;
            int num2;
            num2 = 0;
            int num3;
            num3 = 0;
            int num4;
            num4 = 0;
            List<洗练> list;
            list = new List<洗练>();
            List<洗练> list2;
            list2 = new List<洗练>();
            switch (模式)
            {
                default:
                    num = Settings.龙卫重铸费用;
                    num2 = 2;
                    break;
                case 3:
                    {
                        num = Settings.锁单重铸费用;
                        num2 = 8;
                        num4 = 8;
                        for (byte b = 0; b < 3; b++)
                        {
                            if (附加 == b)
                            {
                                list2.Add(new 洗练
                                {
                                    阶段 = (byte)(b + 1),
                                    词缀 = 龙卫词缀类型.攻击
                                });
                            }
                        }
                        for (byte b2 = 3; b2 < 6; b2++)
                        {
                            if (附加 == b2)
                            {
                                list2.Add(new 洗练
                                {
                                    阶段 = (byte)(b2 - 2),
                                    词缀 = 龙卫词缀类型.防御
                                });
                            }
                        }
                        break;
                    }
                case 2:
                    num = Settings.锁半重铸费用;
                    num2 = 15;
                    num3 = 8;
                    switch (附加)
                    {
                        case 0:
                            list2.Add(new 洗练
                            {
                                阶段 = 1,
                                词缀 = 龙卫词缀类型.攻击
                            });
                            list2.Add(new 洗练
                            {
                                阶段 = 2,
                                词缀 = 龙卫词缀类型.攻击
                            });
                            list2.Add(new 洗练
                            {
                                阶段 = 3,
                                词缀 = 龙卫词缀类型.攻击
                            });
                            break;
                        case 4:
                            list2.Add(new 洗练
                            {
                                阶段 = 1,
                                词缀 = 龙卫词缀类型.防御
                            });
                            list2.Add(new 洗练
                            {
                                阶段 = 2,
                                词缀 = 龙卫词缀类型.防御
                            });
                            list2.Add(new 洗练
                            {
                                阶段 = 3,
                                词缀 = 龙卫词缀类型.防御
                            });
                            break;
                    }
                    break;
            }
            if (num <= 0)
            {
                主程.添加系统日志("龙卫重铸费用不能设置未0");
                return;
            }
            List<物品数据> 物品列表;
            物品列表 = null;
            List<物品数据> 物品列表2;
            物品列表2 = null;
            List<物品数据> 物品列表3;
            物品列表3 = null;
            if ((num > 0 && num > this.金币数量) || (num2 > 0 && !this.查找背包物品(num2, 90500, out 物品列表)) || (num4 > 0 && !this.查找背包物品(num4, 90504, out 物品列表2)) || (num3 > 0 && !this.查找背包物品(num3, 90506, out 物品列表3)))
            {
                return;
            }
            this.扣金币((uint)num);
            if (物品列表 != null)
            {
                this.消耗背包物品(num2, 物品列表, "龙卫传承重塑");
            }
            if (物品列表2 != null)
            {
                this.消耗背包物品(num4, 物品列表2, "龙卫传承重塑");
            }
            if (物品列表3 != null)
            {
                this.消耗背包物品(num3, 物品列表3, "龙卫传承重塑");
            }
            byte b3;
            b3 = 0;
            for (int num5 = this.角色数据.龙卫属性.Count - 1; num5 >= 0; num5--)
            {
                龙卫数据 数据;
                数据 = this.角色数据.龙卫属性.ElementAt(num5);
                if (数据.装备位置 == 部位编号)
                {
                    b3++;
                    if (数据.龙卫模板.占位数量 > 1)
                    {
                        b3++;
                    }
                    if (!list2.Contains(new 洗练
                    {
                        阶段 = 数据.当前阶段,
                        词缀 = 数据.龙卫模板.词缀类型
                    }))
                    {
                        this.角色数据.龙卫属性.Remove(数据);
                        base.属性加成.Remove(数据);
                        数据.删除数据();
                        if (list.Where((洗练 i) => i.阶段 == 数据.当前阶段 && i.词缀 == 数据.龙卫模板.词缀类型).Count() == 0)
                        {
                            list.Add(new 洗练
                            {
                                阶段 = 数据.当前阶段,
                                词缀 = 数据.龙卫模板.词缀类型
                            });
                            if (数据.龙卫模板.占位数量 > 1)
                            {
                                list.Add(new 洗练
                                {
                                    阶段 = (byte)(数据.当前阶段 + 1),
                                    词缀 = 数据.龙卫模板.词缀类型
                                });
                            }
                        }
                    }
                }
            }
            foreach (洗练 item2 in list.OrderBy((洗练 r) => r.阶段).ToList())
            {
                byte 当前阶段;
                当前阶段 = item2.阶段;
                龙卫词缀类型 词缀类型;
                词缀类型 = item2.词缀;
                bool 不许多格词条;
                不许多格词条 = 当前阶段 == 3 || b3 < 6 || list2.Where((洗练 x) => x.阶段 == 当前阶段 + 1 && x.词缀 == 词缀类型).Count() != 0;
                if (this.角色数据.龙卫属性.Where((龙卫数据 r) => r.装备位置 == 部位编号 && r.龙卫模板.词缀类型 == 词缀类型 && r.当前阶段 == 当前阶段 - 1 && r.龙卫模板.占位数量 > 1).FirstOrDefault() != null)
                {
                    continue;
                }
                龙卫模板 龙卫模板;
                龙卫模板 = 龙卫模板.获取龙卫模板(词缀类型, this.角色职业, 不许多格词条, out var 龙卫品质);
                if (龙卫模板 != null)
                {
                    龙卫数据 龙卫数据;
                    龙卫数据 = new 龙卫数据(龙卫模板, 部位编号, 当前阶段, 龙卫品质);
                    if (龙卫数据.龙卫属性.全服提示)
                    {
                        this.发送顶部公告($"<#P0:<#PN:{this.对象名字}>><#P1:{龙卫模板.词缀名字}><#T:990689>", 全服通知: true);
                    }
                    this.角色数据.龙卫属性.Add(龙卫数据);
                }
            }
            this.刷新龙卫激活状态(是否更新属性: true);
            base.发送封包(new 龙卫觉醒回执
            {
                特效编号 = 2,
                属性位置 = 部位编号,
                数据 = this.角色数据.获取龙卫属性封包数据(部位编号)
            });
            base.发送封包(new 龙卫属性激活状态
            {
                属性位置 = 部位编号,
                数据 = this.角色数据.获取龙卫激活封包数据(部位编号)
            });
        }

        public void 龙卫传承觉醒(byte 部位编号, byte 当前阶段)
        {
            龙卫设置 龙卫设置;
            龙卫设置 = 游戏服务器.模板类.龙卫模板.获取龙卫设置(部位编号, 当前阶段);
            if (龙卫设置 == null || 龙卫设置.金币 > this.金币数量)
            {
                return;
            }
            if (龙卫设置.物品编号一 > -1)
            {
                if (!this.查找背包物品(龙卫设置.物品数量一, 龙卫设置.物品编号一, out var 物品列表))
                {
                    return;
                }
                if (龙卫设置.物品编号二 > -1)
                {
                    if (!this.查找背包物品(龙卫设置.物品数量二, 龙卫设置.物品编号二, out var 物品列表2))
                    {
                        return;
                    }
                    if (龙卫设置.物品编号三 > -1)
                    {
                        if (!this.查找背包物品(龙卫设置.物品数量三, 龙卫设置.物品编号三, out var 物品列表3))
                        {
                            return;
                        }
                        this.消耗背包物品(龙卫设置.物品数量三, 物品列表3, "龙卫传承觉醒");
                    }
                    this.消耗背包物品(龙卫设置.物品数量二, 物品列表2, "龙卫传承觉醒");
                }
                this.消耗背包物品(龙卫设置.物品数量一, 物品列表, "龙卫传承觉醒");
            }
            this.金币数量 -= (uint)龙卫设置.金币;
            主程.添加货币日志(this, "龙卫传承觉醒", 游戏货币.金币, 龙卫设置.金币);
            for (int num = this.角色数据.龙卫属性.Count - 1; num >= 0; num--)
            {
                龙卫数据 龙卫数据;
                龙卫数据 = this.角色数据.龙卫属性.ElementAt(num);
                if (龙卫数据.装备位置 == 部位编号 && 龙卫数据.当前阶段 == 当前阶段)
                {
                    this.角色数据.龙卫属性.Remove(龙卫数据);
                    base.属性加成.Remove(龙卫数据);
                    龙卫数据.删除数据();
                }
            }
            龙卫模板 龙卫模板;
            龙卫模板 = 龙卫模板.获取龙卫模板(龙卫词缀类型.攻击, this.角色职业, 不许多格词条: true, out var 龙卫品质);
            龙卫模板 龙卫模板2;
            龙卫模板2 = 龙卫模板.获取龙卫模板(龙卫词缀类型.防御, this.角色职业, 不许多格词条: true, out var 龙卫品质2);
            if (龙卫模板 != null)
            {
                龙卫数据 龙卫数据2;
                龙卫数据2 = new 龙卫数据(龙卫模板, 部位编号, 当前阶段, 龙卫品质);
                if (龙卫数据2.龙卫属性.全服提示)
                {
                    this.发送顶部公告($"<#P0:<#PN:{this.对象名字}>><#P1:{龙卫模板.词缀名字}><#T:990689>");
                }
                this.角色数据.龙卫属性.Add(龙卫数据2);
            }
            if (龙卫模板2 != null)
            {
                龙卫数据 龙卫数据3;
                龙卫数据3 = new 龙卫数据(龙卫模板2, 部位编号, 当前阶段, 龙卫品质2);
                if (龙卫数据3.龙卫属性.全服提示)
                {
                    this.发送顶部公告($"<#P0:<#PN:{this.对象名字}>><#P1:{龙卫模板2.词缀名字}><#T:990689>");
                }
                this.角色数据.龙卫属性.Add(龙卫数据3);
            }
            base.发送封包(new 龙卫觉醒回执
            {
                特效编号 = 1,
                属性位置 = 部位编号,
                数据 = this.角色数据.获取龙卫属性封包数据(部位编号)
            });
            this.刷新龙卫激活状态(是否更新属性: true);
            base.发送封包(new 龙卫属性激活状态
            {
                属性位置 = 部位编号,
                数据 = this.角色数据.获取龙卫激活封包数据(部位编号)
            });
        }

        public void 龙卫记录部位(byte 记录部位, byte 记录序号)
        {
            哈希监视器<龙卫数据> 哈希监视器;
            哈希监视器 = null;
            switch (记录序号)
            {
                case 0:
                    哈希监视器 = this.角色数据.龙卫属性一;
                    break;
                case 1:
                    哈希监视器 = this.角色数据.龙卫属性二;
                    break;
                case 2:
                    哈希监视器 = this.角色数据.龙卫属性三;
                    break;
                case 3:
                    哈希监视器 = this.角色数据.龙卫属性四;
                    break;
                case 4:
                    if (this.角色数据.龙卫记录五.V != string.Empty)
                    {
                        哈希监视器 = this.角色数据.龙卫属性五;
                    }
                    break;
            }
            if (哈希监视器 == null)
            {
                return;
            }
            foreach (龙卫数据 item in 哈希监视器.Where((龙卫数据 r) => r.装备位置 == 记录部位))
            {
                哈希监视器.Remove(item);
                item.删除数据();
            }
            foreach (龙卫数据 item2 in this.角色数据.龙卫属性.Where((龙卫数据 r) => r.装备位置 == 记录部位))
            {
                哈希监视器.Add(new 龙卫数据(item2));
            }
            base.发送封包(new 记录部位回执
            {
                记录序号 = 记录序号,
                记录部位 = 记录部位,
                数据 = this.角色数据.获取龙卫记录封包数据(记录部位, 记录序号)
            });
        }

        public void 龙卫恢复部位(byte 记录部位, byte 记录序号)
        {
            哈希监视器<龙卫数据> 哈希监视器;
            哈希监视器 = null;
            switch (记录序号)
            {
                case 0:
                    哈希监视器 = this.角色数据.龙卫属性一;
                    break;
                case 1:
                    哈希监视器 = this.角色数据.龙卫属性二;
                    break;
                case 2:
                    哈希监视器 = this.角色数据.龙卫属性三;
                    break;
                case 3:
                    哈希监视器 = this.角色数据.龙卫属性四;
                    break;
                case 4:
                    if (this.角色数据.龙卫记录五.V != string.Empty)
                    {
                        哈希监视器 = this.角色数据.龙卫属性五;
                    }
                    break;
            }
            if (哈希监视器 == null)
            {
                return;
            }
            foreach (龙卫数据 item in this.角色数据.龙卫属性.Where((龙卫数据 r) => r.装备位置 == 记录部位))
            {
                this.角色数据.龙卫属性.Remove(item);
                base.属性加成.Remove(item);
                item.删除数据();
            }
            foreach (龙卫数据 item2 in 哈希监视器.Where((龙卫数据 r) => r.装备位置 == 记录部位))
            {
                this.角色数据.龙卫属性.Add(new 龙卫数据(item2));
            }
            base.发送封包(new 龙卫觉醒回执
            {
                特效编号 = 5,
                属性位置 = 记录部位,
                数据 = this.角色数据.获取龙卫属性封包数据(记录部位)
            });
            this.刷新龙卫激活状态(是否更新属性: true);
            base.发送封包(new 龙卫属性激活状态
            {
                属性位置 = 记录部位,
                数据 = this.角色数据.获取龙卫激活封包数据(记录部位)
            });
        }

        public void 龙卫修改备注(byte 记录序号, byte[] 文本信息)
        {
            switch (记录序号)
            {
                case 0:
                    this.角色数据.龙卫记录一.V = Encoding.UTF8.GetString(文本信息);
                    break;
                case 1:
                    this.角色数据.龙卫记录二.V = Encoding.UTF8.GetString(文本信息);
                    break;
                case 2:
                    this.角色数据.龙卫记录三.V = Encoding.UTF8.GetString(文本信息);
                    break;
                case 3:
                    this.角色数据.龙卫记录四.V = Encoding.UTF8.GetString(文本信息);
                    break;
                case 4:
                    this.角色数据.龙卫记录五.V = Encoding.UTF8.GetString(文本信息);
                    break;
            }
        }

        public void 龙卫全套恢复(byte 恢复模式, byte 记录序号)
        {
            哈希监视器<龙卫数据> 哈希监视器;
            哈希监视器 = null;
            switch (记录序号)
            {
                case 0:
                    哈希监视器 = this.角色数据.龙卫属性一;
                    break;
                case 1:
                    哈希监视器 = this.角色数据.龙卫属性二;
                    break;
                case 2:
                    哈希监视器 = this.角色数据.龙卫属性三;
                    break;
                case 3:
                    哈希监视器 = this.角色数据.龙卫属性四;
                    break;
                case 4:
                    if (this.角色数据.龙卫记录五.V != string.Empty)
                    {
                        哈希监视器 = this.角色数据.龙卫属性五;
                    }
                    break;
            }
            if (哈希监视器 == null)
            {
                return;
            }
            byte[] 龙卫部位;
            龙卫部位 = new byte[8] { 0, 1, 3, 6, 7, 8, 9, 10 };
            int i;
            i = 0;
            while (true)
            {
                if (i < 龙卫部位.Length)
                {
                    if (哈希监视器.Where((龙卫数据 x) => x.装备位置 == 龙卫部位[i]).Count() == 0)
                    {
                        break;
                    }
                    i++;
                    continue;
                }
                if (恢复模式 == 0)
                {
                    List<物品数据> list;
                    list = this.查找背包物品(90500, 8);
                    if (list == null)
                    {
                        return;
                    }
                    this.消耗背包物品(8, list, "龙卫全套恢复");
                }
                else
                {
                    List<物品数据> list2;
                    list2 = this.查找背包物品(91212, 1);
                    if (list2 == null)
                    {
                        return;
                    }
                    this.消耗背包物品(1, list2, "龙卫全套恢复");
                }
                foreach (龙卫数据 item in this.龙卫属性)
                {
                    this.龙卫属性.Remove(item);
                    base.属性加成.Remove(item);
                    item.删除数据();
                }
                foreach (龙卫数据 item2 in 哈希监视器)
                {
                    this.角色数据.龙卫属性.Add(new 龙卫数据(item2));
                }
                this.刷新龙卫激活状态(是否更新属性: true);
                using MemoryStream memoryStream = new MemoryStream();
                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write((byte)8);
                binaryWriter.Write((byte)0);
                for (int j = 0; j < 龙卫部位.Length; j++)
                {
                    binaryWriter.Write(龙卫部位[j]);
                    binaryWriter.Write(this.角色数据.获取龙卫激活封包数据(龙卫部位[j]));
                    binaryWriter.Write(this.角色数据.获取龙卫属性封包数据(龙卫部位[j]));
                }
                base.发送封包(new 部位全套恢复
                {
                    数据 = memoryStream.ToArray()
                });
                return;
            }
            base.发送封包(new 游戏错误提示
            {
                错误代码 = 1950
            });
        }

        public void 计算传承之力()
        {
            this.传承之力 = 0;
            byte[] array;
            array = new byte[8] { 0, 1, 3, 6, 7, 8, 9, 10 };
            foreach (byte 部位编号 in array)
            {
                bool flag;
                flag = true;
                foreach (龙卫数据 item in this.角色数据.龙卫属性)
                {
                    if (item.是否激活 && item.装备位置 == 部位编号 && (item.当前阶段 == 3 || (item.当前阶段 == 2 && item.龙卫模板.占位数量 > 1)))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    continue;
                }
                switch ((from r in this.角色数据.龙卫属性
                         where r.装备位置 == 部位编号
                         orderby r.龙卫品质.V
                         select r).FirstOrDefault().龙卫品质.V)
                {
                    case 1:
                        this.传承之力 += 8;
                        break;
                    case 2:
                        this.传承之力 += 12;
                        break;
                    case 3:
                        this.传承之力 += 18;
                        break;
                    case 4:
                        this.传承之力 += 25;
                        break;
                }
            }
            if (this.传承之力 > 0)
            {
                if (!base.属性加成.ContainsKey(this.龙卫属性))
                {
                    base.属性加成[this.龙卫属性] = new Dictionary<游戏对象属性, int>();
                }
                base.属性加成[this.龙卫属性].Clear();
                base.属性加成[this.龙卫属性].Add(游戏对象属性.最大体力, this.传承之力);
            }
            this.战力加成[this.角色数据.龙卫属性] = this.角色数据.龙卫属性.Sum((龙卫数据 x) => x.是否激活 ? (x.龙卫品质.V * 2) : 0);
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 1937,
                第一参数 = this.传承之力
            });
            this.传承之力外观 = (byte)((this.传承之力 > 0) ? ((uint)(this.传承之力 / 50 * 8)) : 0u);
        }

        public void 发送龙卫描述()
        {
            if (!this.开通龙卫觉醒)
            {
                return;
            }
            byte[] array;
            array = new byte[8] { 0, 1, 3, 6, 7, 8, 9, 10 };
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(Encoding.UTF8.GetBytes(this.角色数据.龙卫记录一.V));
            binaryWriter.Seek(16, SeekOrigin.Begin);
            binaryWriter.Write(Encoding.UTF8.GetBytes(this.角色数据.龙卫记录二.V));
            binaryWriter.Seek(32, SeekOrigin.Begin);
            binaryWriter.Write(Encoding.UTF8.GetBytes(this.角色数据.龙卫记录三.V));
            binaryWriter.Seek(48, SeekOrigin.Begin);
            binaryWriter.Write(Encoding.UTF8.GetBytes(this.角色数据.龙卫记录四.V));
            if (this.角色数据.龙卫记录五.V != null)
            {
                binaryWriter.Seek(64, SeekOrigin.Begin);
                binaryWriter.Write(Encoding.UTF8.GetBytes(this.角色数据.龙卫记录五.V));
            }
            binaryWriter.Seek(80, SeekOrigin.Begin);
            binaryWriter.Write(new byte[48]
            {
                0, 0, 14, 0, 32, 130, 24, 10, 166, 44,
                98, 19, 149, 245, 5, 0, 1, 0, 0, 0,
                0, 53, 107, 0, 0, 0, 0, 1, 1, 0,
                0, 32, 1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            });
            binaryWriter.Seek(128, SeekOrigin.Begin);
            for (int i = 0; i < array.Length; i++)
            {
                binaryWriter.Write(array[i]);
                binaryWriter.Write(this.角色数据.获取龙卫激活封包数据(array[i]));
                binaryWriter.Write(this.角色数据.获取龙卫属性封包数据(array[i]));
            }
            byte b;
            b = 0;
            if (this.角色数据.龙卫属性一.Count > 0)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if (this.角色数据.获取龙卫登录记录封包(array[j], 0, out var 返回值))
                    {
                        binaryWriter.Write((byte)0);
                        binaryWriter.Write(array[j]);
                        binaryWriter.Write(返回值);
                        b++;
                    }
                }
            }
            if (this.角色数据.龙卫属性二.Count > 0)
            {
                for (int k = 0; k < array.Length; k++)
                {
                    if (this.角色数据.获取龙卫登录记录封包(array[k], 1, out var 返回值2))
                    {
                        binaryWriter.Write((byte)1);
                        binaryWriter.Write(array[k]);
                        binaryWriter.Write(返回值2);
                        b++;
                    }
                }
            }
            if (this.角色数据.龙卫属性三.Count > 0)
            {
                for (int l = 0; l < array.Length; l++)
                {
                    if (this.角色数据.获取龙卫登录记录封包(array[l], 2, out var 返回值3))
                    {
                        binaryWriter.Write((byte)2);
                        binaryWriter.Write(array[l]);
                        binaryWriter.Write(返回值3);
                        b++;
                    }
                }
            }
            if (this.角色数据.龙卫属性四.Count > 0)
            {
                for (int m = 0; m < array.Length; m++)
                {
                    if (this.角色数据.获取龙卫登录记录封包(array[m], 3, out var 返回值4))
                    {
                        binaryWriter.Write((byte)3);
                        binaryWriter.Write(array[m]);
                        binaryWriter.Write(返回值4);
                        b++;
                    }
                }
            }
            if (this.角色数据.龙卫属性五.Count > 0)
            {
                for (int n = 0; n < array.Length; n++)
                {
                    if (this.角色数据.获取龙卫登录记录封包(array[n], 4, out var 返回值5))
                    {
                        binaryWriter.Write((byte)4);
                        binaryWriter.Write(array[n]);
                        binaryWriter.Write(返回值5);
                        b++;
                    }
                }
            }
            base.发送封包(new 同步龙卫信息
            {
                记录数量 = b,
                可用记录 = (byte)((this.角色数据.龙卫记录五.V == string.Empty || this.角色数据.龙卫记录五.V == null) ? 4u : 5u),
                描述信息 = memoryStream.ToArray()
            });
        }

        public void 查看他人龙卫(int 对象编号)
        {
            if (!地图处理网关.地图对象表.TryGetValue(对象编号, out var value) || !(value is 玩家实例 { 开通龙卫觉醒: not false } 玩家实例2))
            {
                return;
            }
            byte[] array;
            array = new byte[8] { 0, 1, 3, 6, 7, 8, 9, 10 };
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value: true);
            binaryWriter.Write((int)玩家实例2.传承之力);
            binaryWriter.Write((byte)8);
            for (int i = 0; i < array.Length; i++)
            {
                binaryWriter.Write(array[i]);
                binaryWriter.Write(玩家实例2.角色数据.获取龙卫激活封包数据(array[i]));
                binaryWriter.Write(玩家实例2.角色数据.获取龙卫属性封包数据(array[i]));
            }
            base.发送封包(new 查看他人龙卫
            {
                描述信息 = memoryStream.ToArray()
            });
        }

        public void 刷新龙卫激活状态(bool 是否更新属性 = false, bool forceUpdate = false)
        {
            foreach (龙卫数据 item in this.角色数据.龙卫属性)
            {
                bool 是否激活;
                是否激活 = item.是否激活;
                item.是否激活 = this.角色装备.TryGetValue(item.装备位置, out var v) && (v.装备模板.装备套装 >= 游戏装备套装.祖玛装备 || (int)v.装备模板.装备套装 >= (int)item.龙卫品质.V);
                if (forceUpdate || 是否激活 != item.是否激活)
                {
                    if (forceUpdate || 是否激活)
                    {
                        base.属性加成.Remove(item);
                    }
                    if (item.是否激活)
                    {
                        base.属性加成.Add(item, item.属性加成);
                    }
                }
            }
            this.计算传承之力();
            this.刷新生效龙卫();
            if (是否更新属性)
            {
                this.更新对象属性();
            }
        }

        public void 更新精炼阶段属性(bool 是否更新属性 = false)
        {
            int num;
            num = 0;
            foreach (装备数据 value2 in this.角色装备.Values)
            {
                num += value2.精炼战力;
            }
            int num2;
            num2 = (int)Math.Ceiling((float)num / 50f);
            foreach (精炼阶段 value3 in 精炼阶段.数据表.Values)
            {
                if (base.属性加成.ContainsKey(value3))
                {
                    base.属性加成.Remove(value3);
                }
            }
            int i;
            for (i = 1; i < num2 + 1; i++)
            {
                if (精炼阶段.数据表.TryGetValue(精炼阶段.数据表.Keys.FirstOrDefault((阶段判断 x) => x.职业 == this.角色职业 && x.阶段 == i), out var value))
                {
                    base.属性加成[value] = value.属性值;
                }
            }
            if (是否更新属性)
            {
                this.更新对象属性();
            }
        }

        public void 升级觉醒技能(ushort 技能编号)
        {
            if (this.主体技能表.TryGetValue(技能编号, out var v) && this.当前觉醒经验 >= v.升级经验)
            {
                this.当前觉醒经验 -= v.升级经验;
                this.网络连接?.发送封包(new 角色经验变动
                {
                    经验增加 = 0,
                    今日增加 = 0,
                    经验上限 = 10000000,
                    双倍经验 = 0,
                    当前经验 = this.当前经验,
                    升级所需 = this.所需经验,
                    增加的觉醒之力经验 = -v.升级经验
                });
                v.技能等级.V++;
                base.发送封包(new 玩家技能升级
                {
                    技能编号 = v.技能编号.V,
                    技能等级 = v.技能等级.V
                });
            }
        }

        public void 玩家装备打孔(byte 装备部位)
        {
            if (!this.角色背包.TryGetValue(装备部位, out var v) || !(v is 装备数据 装备数据) || !装备数据.装备模板.能否打孔)
            {
                return;
            }
            int num;
            num = 3;
            switch (主程.开区节点)
            {
                case 2:
                    num = 1;
                    break;
                case 3:
                    num = 2;
                    break;
                case 4:
                    num = 3;
                    break;
            }
            if (装备数据.孔洞颜色.Count >= num)
            {
                this.发送顶部公告($"当前节点只开放[{num}]个孔洞");
                return;
            }
            int num2;
            num2 = ((装备数据.孔洞颜色.Count == 0) ? 装备数据.装备模板.一孔花费 : ((装备数据.孔洞颜色.Count == 1) ? 装备数据.装备模板.二孔花费 : 装备数据.装备模板.三孔花费));
            if (this.查找背包物品(num2, 91115, out var 物品列表))
            {
                this.消耗背包物品(num2, 物品列表, "装备打孔扣除");
                装备数据.孔洞颜色.Add(装备孔洞颜色.黄色);
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.SendRaw(269, 2, new byte[0]);
            }
        }

        public void 玩家装备雕色(byte 装备部位, int 孔洞位置, int 孔洞颜色)
        {
            byte[][] array;
            array = new byte[9][]
            {
                new byte[9],
                new byte[9] { 0, 110, 25, 70, 135, 55, 135, 135, 135 },
                new byte[9] { 0, 10, 10, 10, 10, 10, 10, 10, 10 },
                new byte[9] { 0, 60, 15, 70, 85, 45, 85, 85, 85 },
                new byte[9] { 0, 160, 50, 120, 235, 75, 235, 235, 235 },
                new byte[9] { 0, 40, 10, 40, 35, 35, 35, 35, 35 },
                new byte[9] { 0, 160, 50, 120, 235, 75, 235, 235, 235 },
                new byte[9] { 0, 125, 30, 80, 155, 65, 155, 155, 155 },
                new byte[9] { 0, 160, 50, 120, 235, 75, 235, 235, 235 }
            };
            if (!this.角色背包.TryGetValue(装备部位, out var v) || !(v is 装备数据 装备数据) || 装备数据.孔洞颜色.Count < 孔洞位置 || 装备数据.镶嵌灵石.ContainsKey((byte)孔洞位置))
            {
                return;
            }
            int num;
            num = ((装备数据.孔洞颜色.Count == 1) ? ((孔洞颜色 == 2) ? 5 : array[孔洞颜色][2]) : ((孔洞位置 >= 2) ? Math.Max(array[孔洞颜色][(int)装备数据.孔洞颜色[0]], array[孔洞颜色][(int)装备数据.孔洞颜色[1]]) : array[孔洞颜色][(int)装备数据.孔洞颜色[Math.Abs(孔洞位置 - 1)]]));
            if (!this.查找背包物品(num, 91116, out var 物品列表))
            {
                return;
            }
            this.消耗背包物品(num, 物品列表, "装备雕色扣除");
            装备数据.孔洞颜色[孔洞位置] = (装备孔洞颜色)孔洞颜色;
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = 装备数据.字节描述()
            });
            Dictionary<装备孔洞颜色, string> dictionary;
            dictionary = new Dictionary<装备孔洞颜色, string>
            {
                {
                    装备孔洞颜色.红色,
                    "MMOGame.DLG.ITEM.14"
                },
                {
                    装备孔洞颜色.蓝色,
                    "MMOGame.DLG.ITEM.15"
                },
                {
                    装备孔洞颜色.绿色,
                    "MMOGame.DLG.ITEM.13"
                },
                {
                    装备孔洞颜色.灰色,
                    "MMOGame.DLG.ITEM.24"
                },
                {
                    装备孔洞颜色.橙色,
                    "MMOGame.DLG.ITEM.28"
                },
                {
                    装备孔洞颜色.褐色,
                    "MMOGame.DLG.ITEM.36"
                }
            };
            Dictionary<装备孔洞颜色, string> dictionary2;
            dictionary2 = new Dictionary<装备孔洞颜色, string>
            {
                {
                    装备孔洞颜色.红色,
                    "MMOGame.DLG.ITEM.42"
                },
                {
                    装备孔洞颜色.蓝色,
                    "MMOGame.DLG.ITEM.43"
                },
                {
                    装备孔洞颜色.绿色,
                    "MMOGame.DLG.ITEM.41"
                },
                {
                    装备孔洞颜色.灰色,
                    "MMOGame.DLG.ITEM.44"
                },
                {
                    装备孔洞颜色.橙色,
                    "MMOGame.DLG.ITEM.45"
                },
                {
                    装备孔洞颜色.褐色,
                    "MMOGame.DLG.ITEM.46"
                }
            };
            foreach (装备孔洞颜色 value in Enum.GetValues(typeof(装备孔洞颜色)))
            {
                int num2;
                num2 = 0;
                bool flag;
                flag = false;
                for (int i = 0; i < 装备数据.孔洞颜色.Count; i++)
                {
                    if (装备数据.孔洞颜色[i] == value)
                    {
                        num2++;
                        flag = flag || i == 孔洞位置;
                    }
                }
                if (flag)
                {
                    if (num2 == 2 && dictionary.ContainsKey(value))
                    {
                        网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{装备数据.物品编号}>><#T:{dictionary[value]}>");
                        break;
                    }
                    if (num2 == 3 && dictionary2.ContainsKey(value))
                    {
                        网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{装备数据.物品编号}>><#T:{dictionary2[value]}>");
                        break;
                    }
                }
            }
            this.网络连接?.SendRaw(270, 2, new byte[0]);
        }

        public void 玩家孔色传承(byte 来源位置, byte 传承位置)
        {
            if (!this.角色背包.TryGetValue(来源位置, out var v) || !(v is 装备数据 装备数据) || !this.角色背包.TryGetValue(传承位置, out var v2) || !(v2 is 装备数据 装备数据2))
            {
                return;
            }
            int num;
            num = 20000000 * 装备数据.孔洞颜色.Count;
            if (装备数据.孔洞颜色.Count < 2)
            {
                return;
            }
            if (装备数据.需要职业 != 游戏对象职业.通用 && 装备数据2.需要职业 != 游戏对象职业.通用 && 装备数据.需要职业 != 装备数据2.需要职业)
            {
                this.发送顶部公告("两件装备职业不一致.");
            }
            else
            {
                if ((装备数据.需要性别 != 0 && 装备数据.需要性别 != 装备数据2.需要性别) || this.金币数量 < num || 装备数据.镶嵌灵石.Count != 0 || 装备数据2.镶嵌灵石.Count != 0)
                {
                    return;
                }
                this.金币数量 -= (uint)num;
                for (int i = 0; i < 装备数据.孔洞颜色.Count; i++)
                {
                    if (i < 装备数据2.孔洞颜色.Count)
                    {
                        装备数据2.孔洞颜色[i] = 装备数据.孔洞颜色[i];
                    }
                    装备数据.孔洞颜色[i] = 装备孔洞颜色.黄色;
                }
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据2.字节描述()
                });
                this.网络连接?.SendRaw(271, 2, new byte[0]);
                Dictionary<装备孔洞颜色, string> dictionary;
                dictionary = new Dictionary<装备孔洞颜色, string>
                {
                    {
                        装备孔洞颜色.红色,
                        "MMOGame.DLG.ITEM.16"
                    },
                    {
                        装备孔洞颜色.黄色,
                        "MMOGame.DLG.ITEM.17"
                    },
                    {
                        装备孔洞颜色.蓝色,
                        "MMOGame.DLG.ITEM.18"
                    },
                    {
                        装备孔洞颜色.绿色,
                        "MMOGame.DLG.ITEM.19"
                    },
                    {
                        装备孔洞颜色.紫色,
                        "MMOGame.DLG.ITEM.20"
                    },
                    {
                        装备孔洞颜色.灰色,
                        "MMOGame.DLG.ITEM.25"
                    },
                    {
                        装备孔洞颜色.橙色,
                        "MMOGame.DLG.ITEM.27"
                    },
                    {
                        装备孔洞颜色.褐色,
                        "MMOGame.DLG.ITEM.35"
                    }
                };
                Dictionary<装备孔洞颜色, string> dictionary2;
                dictionary2 = new Dictionary<装备孔洞颜色, string>
                {
                    {
                        装备孔洞颜色.红色,
                        "MMOGame.DLG.ITEM.47"
                    },
                    {
                        装备孔洞颜色.黄色,
                        "MMOGame.DLG.ITEM.48"
                    },
                    {
                        装备孔洞颜色.蓝色,
                        "MMOGame.DLG.ITEM.49"
                    },
                    {
                        装备孔洞颜色.绿色,
                        "MMOGame.DLG.ITEM.50"
                    },
                    {
                        装备孔洞颜色.紫色,
                        "MMOGame.DLG.ITEM.51"
                    },
                    {
                        装备孔洞颜色.灰色,
                        "MMOGame.DLG.ITEM.53"
                    },
                    {
                        装备孔洞颜色.橙色,
                        "MMOGame.DLG.ITEM.54"
                    },
                    {
                        装备孔洞颜色.褐色,
                        "MMOGame.DLG.ITEM.55"
                    },
                    {
                        装备孔洞颜色.多彩,
                        "MMOGame.DLG.ITEM.52"
                    }
                };
                foreach (装备孔洞颜色 颜色 in Enum.GetValues(typeof(装备孔洞颜色)))
                {
                    if (装备数据2.孔洞颜色.Where((装备孔洞颜色 x) => x == 颜色).Count() != 2 || !dictionary.ContainsKey(颜色))
                    {
                        if (装备数据2.孔洞颜色.Where((装备孔洞颜色 x) => x == 颜色).Count() == 3 && dictionary2.ContainsKey(颜色))
                        {
                            网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{装备数据.物品编号}>><#P2:<#I:{装备数据2.物品编号}>><#T:{dictionary2[颜色]}>");
                            break;
                        }
                        continue;
                    }
                    网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{装备数据.物品编号}>><#P2:<#I:{装备数据2.物品编号}>><#T:{dictionary[颜色]}>");
                    break;
                }
            }
        }

        public void 玩家合成灵石(int 物品编号, int 幸运符数)
        {
            int num;
            num = 5000000;
            if ((long)this.金币数量 >= 5000000L && this.查找背包物品(10, 物品编号 - 1, out var 物品列表))
            {
                this.金币数量 -= (uint)num;
                主程.添加货币日志(this, "合成灵石扣除", 游戏货币.金币, -num);
                this.消耗背包物品(10, 物品列表, "合成灵石扣除");
                float num2;
                num2 = 0.1f;
                if (this.查找背包物品(幸运符数, 91117, out var 物品列表2))
                {
                    this.消耗背包物品(幸运符数, 物品列表2, "合成灵石扣除");
                    num2 += (float)幸运符数 * 0.03f;
                }
                bool flag;
                if (flag = 计算类.计算概率(num2))
                {
                    this.玩家获得物品(物品编号, 1, "合成灵石获得");
                }
                this.网络连接?.SendRaw(252, 3, new byte[1] { flag ? ((byte)1) : ((byte)0) });
            }
        }

        public void 玩家兑换精粹(byte 物品位置)
        {
            if (this.角色背包.TryGetValue(物品位置, out var v) && v is 装备数据 装备数据)
            {
                int num;
                num = 0;
                int num2;
                num2 = ((装备数据.装备模板.装备套装 == 游戏装备套装.祖玛装备) ? 1000000 : ((装备数据.装备模板.铭文职业 == 游戏对象职业.法师 || 装备数据.装备模板.铭文职业 == 游戏对象职业.道士) ? 10000000 : 50000000));
                int 物品持久;
                物品持久 = ((装备数据.装备模板.装备套装 == 游戏装备套装.祖玛装备) ? 10 : ((装备数据.装备模板.铭文职业 == 游戏对象职业.法师 || 装备数据.装备模板.铭文职业 == 游戏对象职业.道士) ? 100 : 500));
                switch (装备数据.装备模板.铭文职业)
                {
                    case 游戏对象职业.战士:
                        num = 111058;
                        break;
                    case 游戏对象职业.法师:
                        num = 111059;
                        break;
                    case 游戏对象职业.刺客:
                        num = 111078;
                        break;
                    case 游戏对象职业.弓手:
                        num = 111091;
                        break;
                    case 游戏对象职业.道士:
                        num = 111060;
                        break;
                    case 游戏对象职业.龙枪:
                        num = 111301;
                        break;
                }
                if (this.金币数量 >= num2 && num != 0 && (装备数据.装备模板.装备套装 == 游戏装备套装.祖玛装备 || 装备数据.装备模板.装备套装 == 游戏装备套装.赤月装备))
                {
                    this.金币数量 -= (uint)num2;
                    this.消耗背包物品(装备数据.当前持久.V, 装备数据, "兑换武器精粹");
                    this.玩家获得物品(num, 物品持久, "兑换武器精粹");
                }
            }
        }

        public void 玩家外观易容(byte 角色发型, byte 角色发色, byte 角色脸型, byte 未知参数)
        {
            if (Enum.TryParse<对象发色分类>(角色发色.ToString(), out var result) && Enum.IsDefined(typeof(对象发色分类), result) && Enum.TryParse<对象发型分类>(((int)this.角色职业 * 65536 + (int)this.角色性别 * 256 + 角色发型).ToString(), out var result2) && Enum.IsDefined(typeof(对象发型分类), result2) && Enum.TryParse<对象脸型分类>(((int)this.角色职业 * 65536 + (int)this.角色性别 * 256 + 角色脸型).ToString(), out var result3) && Enum.IsDefined(typeof(对象脸型分类), result3))
            {
                List<对象发型分类> list;
                list = new List<对象发型分类>
                {
                    对象发型分类.战士男12,
                    对象发型分类.战士男13,
                    对象发型分类.战士男14,
                    对象发型分类.战士女12,
                    对象发型分类.战士女13,
                    对象发型分类.战士女14,
                    对象发型分类.法师男12,
                    对象发型分类.法师男13,
                    对象发型分类.法师男14,
                    对象发型分类.法师男15,
                    对象发型分类.法师女12,
                    对象发型分类.法师女13,
                    对象发型分类.法师女14,
                    对象发型分类.法师女15,
                    对象发型分类.道士男12,
                    对象发型分类.道士男13,
                    对象发型分类.道士男14,
                    对象发型分类.道士男15,
                    对象发型分类.道士女12,
                    对象发型分类.道士女13,
                    对象发型分类.道士女14,
                    对象发型分类.道士女15,
                    对象发型分类.刺客男13,
                    对象发型分类.刺客男14,
                    对象发型分类.刺客女13,
                    对象发型分类.刺客女14,
                    对象发型分类.弓手男13,
                    对象发型分类.弓手男12,
                    对象发型分类.弓手女13,
                    对象发型分类.弓手女12,
                    对象发型分类.龙枪男10,
                    对象发型分类.龙枪男11,
                    对象发型分类.龙枪男12,
                    对象发型分类.龙枪女10,
                    对象发型分类.龙枪女11,
                    对象发型分类.龙枪女11
                };
                int num;
                num = 0;
                if (this.角色数据.角色发型.V != result2)
                {
                    num += (list.Contains(result2) ? 8000000 : 1000000);
                }
                if (this.角色数据.角色发色.V != result)
                {
                    num += 1000000;
                }
                if (this.角色数据.角色脸型.V != result3)
                {
                    num += 1000000;
                }
                if (this.金币数量 >= num)
                {
                    this.金币数量 -= (uint)num;
                    this.角色数据.角色发型.V = result2;
                    this.角色数据.角色发色.V = result;
                    this.角色数据.角色脸型.V = result3;
                    base.发送封包(new 同步对象容貌
                    {
                        对象编号 = this.角色数据.角色编号,
                        对象发型 = (byte)result2,
                        对象发色 = (byte)result,
                        对象脸型 = (byte)result3
                    });
                }
            }
        }

        public void 玩家重铸装备(装备数据 v4)
        {
            v4.随机属性.SetValue(装备属性.生成属性(v4.物品类型, 重铸装备: true));
            主程.添加重铸日志(this.角色数据, v4, v4.随机属性);
            base.属性加成[v4] = v4.装备属性;
            this.更新对象属性();
            this.网络连接?.发送封包(new 玩家物品变动
            {
                物品描述 = v4.字节描述()
            });
        }

        public void 传送任务点(int questId)
        {
            switch (questId)
            {
                case 1481:
                    this.玩家切换地图(143, 1139, 668, 0);
                    return;
                case 1901:
                    this.玩家切换地图(145, 867, 320, 0);
                    return;
                case 1903:
                    this.玩家切换地图(147, 1097, 462, 0);
                    return;
                case 1906:
                    this.玩家切换地图(154, 1048, 348, 0);
                    return;
                case 1907:
                    this.玩家切换地图(154, 963, 413, 0);
                    return;
                case 1904:
                case 1905:
                    this.玩家切换地图(144, 1031, 207, 0);
                    return;
                case 1561:
                case 1902:
                    this.玩家切换地图(145, 1077, 582, 0);
                    return;
            }
            if (!GameQuests.数据表.TryGetValue(questId, out var questInfo) || (!questInfo.CanTeleport && questInfo.TeleportCostId == 0))
            {
                return;
            }
            CharacterQuest characterQuest;
            characterQuest = this.角色数据.Quests.Where((CharacterQuest x) => x.Info.V.Id == questInfo.Id).FirstOrDefault();
            if (characterQuest == null || characterQuest.CompleteDate.V != DateTime.MinValue)
            {
                return;
            }
            地图实例 地图实例2;
            地图实例2 = 地图处理网关.已分配地图(questInfo.StartNPCMap);
            守卫刷新 守卫刷新;
            守卫刷新 = null;
            if (地图实例2 != null)
            {
                守卫刷新 = 地图实例2.守卫区域.Where((守卫刷新 x) => x.守卫编号 == questInfo.FinishNPCID).FirstOrDefault();
            }
            if (守卫刷新 == null)
            {
                地图实例2 = null;
                守卫刷新 = 守卫刷新.数据表.Where((守卫刷新 x) => x.守卫编号 == questInfo.FinishNPCID).FirstOrDefault();
                if (守卫刷新 == null)
                {
                    守卫刷新 = 守卫刷新.数据表.Where((守卫刷新 x) => x.守卫编号 == questInfo.StartNPCID).FirstOrDefault();
                }
            }
            if (守卫刷新 == null)
            {
                return;
            }
            if (地图实例2 == null)
            {
                地图实例2 = 地图处理网关.已分配地图(守卫刷新.所处地图);
            }
            if (地图实例2 == null)
            {
                return;
            }
            if (questInfo.TeleportCostId > 0)
            {
                物品数据 物品数据;
                物品数据 = this.根据编号获取背包物品(questInfo.TeleportCostId);
                if (物品数据 == null)
                {
                    return;
                }
                this.消耗背包物品(questInfo.TeleportCostValue, 物品数据);
            }
            int num;
            num = 0;
            Point empty;
            while (true)
            {
                if (num < 10)
                {
                    empty = Point.Empty;
                    if (地图实例2.能否通行(empty = 计算类.前方坐标(守卫刷新.所处坐标, 计算类.随机方向(), 主程.随机数.Next(1, 3))))
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                return;
            }
            this.玩家切换地图(地图实例2, 地图区域类型.未知区域, empty);
        }

        public void 寄售上架物品(byte 背包类型, byte 背包位置, byte 时间类型, int 上架价格)
        {
            if (this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3)
            {
                return;
            }
            物品数据 物品数据;
            物品数据 = null;
            字典监视器<byte, 物品数据> 字典监视器;
            字典监视器 = null;
            if (背包类型 == 1)
            {
                字典监视器 = this.角色背包;
            }
            if (背包类型 == 2)
            {
                字典监视器 = this.角色仓库;
            }
            if (背包类型 == 7)
            {
                字典监视器 = this.角色资源包;
            }
            if (字典监视器 == null)
            {
                return;
            }
            物品数据 = (字典监视器.TryGetValue(背包位置, out var v) ? v : null);
            if (物品数据 == null)
            {
                return;
            }
            if (物品数据.是否上锁)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 1890
                });
                return;
            }
            if (物品数据.是否绑定 || (物品数据 is 装备数据 装备数据 && 装备数据.灵魂绑定.V))
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 1804
                });
                return;
            }
            if (上架价格 < 1 || 上架价格 > 1000000000 || 时间类型 < 1 || 时间类型 > 3)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 寄售上架物品, 错误: 参数非法"));
                return;
            }
            if (游戏数据网关.寄售数据表.数据表.Where((KeyValuePair<int, 游戏数据> x) => x.Value is 寄售数据 寄售数据 && 寄售数据.卖家编号 == this.地图编号).Count() >= 5)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 12808
                });
            }
            else
            {
                int 限制时间;
                限制时间 = 时间类型 switch
                {
                    3 => 172800,
                    2 => 86400,
                    1 => 43200,
                    _ => 0,
                };
                int num;
                num = 10000 * 时间类型;
                if (this.金币数量 < num)
                {
                    base.发送封包(new 游戏错误提示
                    {
                        错误代码 = 12809
                    });
                    return;
                }
                this.金币数量 -= (uint)num;
                new 寄售数据(this.角色数据, 物品数据, 上架价格, 限制时间);
                this.网络连接?.SendRaw(514, 14, new byte[12]
                {
                    1, 50, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0
                });
                字典监视器.Remove(背包位置);
                主程.添加系统日志($"[寄售上架物品]{this.对象名字} 上架 {物品数据.ToString()} {上架价格.ToString()} {((!物品数据.能否堆叠) ? 1 : 物品数据.当前持久.V)}个 {限制时间.ToString()}小时");
                this.网络连接?.发送封包(new 删除玩家物品
                {
                    背包类型 = 背包类型,
                    物品位置 = 背包位置
                });
            }
        }

        public void 寄售购买物品(long 订单编号)
        {
            寄售数据 寄售数据;
            寄售数据 = 寄售数据.获取寄售(订单编号);
            if (寄售数据 != null)
            {
                if (寄售数据.物品数据.V == null)
                {
                    寄售数据.删除数据();
                }
                else if (寄售数据.卖方玩家.V == this.角色数据)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 12811
                    });
                }
                else if (this.元宝数量 < 寄售数据.商品价格.V)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 8451
                    });
                }
                else if (主程.当前时间 > 寄售数据.下架时间.V)
                {
                    寄售数据.发送商品(订单编号);
                }
                else
                {
                    this.元宝数量 -= (uint)寄售数据.商品价格.V;
                    主程.添加货币日志(this.角色数据, "玩家寄售购买->" + (游戏数据网关.角色数据表[寄售数据.卖家编号] as 角色数据)?.角色名字.V + "的" + 寄售数据.物品数据?.V?.物品名字, 游戏货币.元宝, -寄售数据.商品价格.V);
                    寄售数据.发送商品(订单编号, this.角色数据);
                }
            }
        }

        public void 寄售下架物品(uint 订单编号)
        {
            寄售数据 寄售;
            寄售 = 寄售数据.获取寄售(订单编号);
            if (寄售 == null)
            {
                return;
            }
            if (寄售.卖方玩家.V != this.角色数据)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 寄售下架物品, 错误: 非卖家本人"));
                return;
            }
            寄售数据.发送商品(订单编号);
        }

        public void 查询我的寄售()
        {
            byte[] 道具字节;
            道具字节 = 寄售数据.我的商品数据(this.角色数据, out var 数量);
            base.发送封包(new 同步寄售列表
            {
                消息类型 = 2,
                道具数量 = (byte)数量,
                道具字节 = 道具字节
            });
        }

        public void 查询寄售物品(int 过滤筛选)
        {
            byte[] 道具字节;
            道具字节 = 寄售数据.查询商品数据(过滤筛选, out var 数量);
            base.发送封包(new 同步寄售列表
            {
                消息类型 = 0,
                道具数量 = (byte)数量,
                道具字节 = 道具字节
            });
        }

        public void 查询指定物品(int 物品编号)
        {
            byte[] 道具字节;
            道具字节 = 寄售数据.指定商品数据(物品编号, out var 数量);
            base.发送封包(new 同步寄售列表
            {
                消息类型 = 0,
                道具数量 = (byte)数量,
                道具字节 = 道具字节
            });
        }

        public void 激活传奇之力()
        {
            int num;
            num = this.角色数据.传奇之力等级 + 1 + 50;
            if (this.当前等级 < num || !传奇之力.检索表[this.角色职业].TryGetValue(num, out var value))
            {
                return;
            }
            List<物品数据> list;
            list = this.查找背包物品(value.需要物品, value.需要数量);
            if (list != null)
            {
                this.消耗背包物品(value.需要数量, list, "传奇之力消耗");
                this.角色数据.传奇之力等级++;
                this.刷新传奇之力();
                this.更新对象属性();
                this.更新玩家战力();
                if (Settings.开启成就系统)
                {
                    this.成就变量赋值(AchievementVariables.LegendPowerLevel, this.角色数据.传奇之力等级 + 50, UseMax: true);
                }
            }
        }

        public void 刷新传奇之力()
        {
            for (int i = 0; i < this.角色数据.传奇之力等级; i++)
            {
                if (传奇之力.检索表[this.角色职业].TryGetValue(i + 51, out var value))
                {
                    base.属性加成[value] = value.数据属性;
                    this.战力加成[value] = value.战斗力值;
                }
            }
        }

        public void 装备开启精炼(byte 背包类型, byte 背包位置)
        {
            字典监视器<byte, 物品数据> 字典监视器;
            字典监视器 = null;
            if (背包类型 == 1)
            {
                字典监视器 = this.角色背包;
            }
            if (!((字典监视器.TryGetValue(背包位置, out var v) ? v : null) is 装备数据 装备数据) || 装备数据.开启精炼.V >= 3 || !装备精炼.数据表.TryGetValue(装备数据.物品编号, out var value))
            {
                return;
            }
            List<物品数据> list;
            list = this.查找背包物品(value.开槽编号, value.开槽数量[装备数据.开启精炼.V]);
            if (list != null)
            {
                this.消耗背包物品(value.开槽数量[装备数据.开启精炼.V], list, "装备精炼消耗");
                switch (装备数据.开启精炼.V)
                {
                    case 0:
                        装备数据.开启精炼.V = 1;
                        装备数据.精炼值一.V = this.获取精炼属性(装备数据, 是否开槽: true);
                        装备数据.精炼次数.V++;
                        break;
                    case 1:
                        装备数据.开启精炼.V = 2;
                        装备数据.精炼值二.V = this.获取精炼属性(装备数据, 是否开槽: true);
                        装备数据.精炼次数.V++;
                        break;
                    case 2:
                        装备数据.开启精炼.V = 3;
                        装备数据.精炼值三.V = this.获取精炼属性(装备数据, 是否开槽: true);
                        装备数据.精炼次数.V++;
                        break;
                }
                this.网络连接?.发送封包(new 玩家物品变动
                {
                    物品描述 = 装备数据.字节描述()
                });
            }
            else
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 1803
                });
            }
        }

        public void 装备重新精炼(byte 背包类型, byte 背包位置, byte 材料类型, byte 材料位置, ushort 特殊标记)
        {
            字典监视器<byte, 物品数据> 字典监视器;
            字典监视器 = null;
            if (背包类型 == 1)
            {
                字典监视器 = this.角色背包;
            }
            物品数据 物品数据;
            物品数据 = (字典监视器.TryGetValue(材料位置, out var v) ? v : null);
            if (物品数据 == null)
            {
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 1803
                });
            }
            else
            {
                if (!((字典监视器.TryGetValue(背包位置, out var v2) ? v2 : null) is 装备数据 装备数据) || 装备数据.开启精炼.V == 0)
                {
                    return;
                }
                List<物品数据> list;
                list = null;
                int num;
                num = ((装备数据.开启精炼.V == 2) ? 4 : 8);
                if (特殊标记 != ushort.MaxValue)
                {
                    list = this.查找背包物品(41001, num);
                    if (list == null)
                    {
                        base.发送封包(new 游戏错误提示
                        {
                            错误代码 = 1803
                        });
                        return;
                    }
                }
                int num2;
                num2 = ((特殊标记 == ushort.MaxValue) ? 100000 : ((装备数据.开启精炼.V == 2) ? 250000 : 500000));
                if (this.金币数量 < num2)
                {
                    return;
                }
                int num3;
                num3 = ((装备数据.开启精炼.V < 3) ? 装备数据.开启精炼.V : 4);
                List<物品数据> list2;
                list2 = this.查找背包物品(41002, num3);
                if (list2 != null && 装备精炼.数据表.TryGetValue(装备数据.物品编号, out var _))
                {
                    if (list != null)
                    {
                        this.消耗背包物品(num, list, "装备精炼消耗");
                    }
                    this.消耗背包物品(num3, list2, "装备精炼消耗");
                    this.消耗背包物品(1, 物品数据, "装备精炼消耗");
                    this.金币数量 -= (uint)num2;
                    for (int i = 0; i < 装备数据.开启精炼.V; i++)
                    {
                        this.临时精炼[i] = this.获取精炼属性(装备数据, 是否开槽: false);
                    }
                    if (特殊标记 == 0)
                    {
                        this.临时精炼[0] = 装备数据.精炼值一.V;
                    }
                    if (特殊标记 == 1)
                    {
                        this.临时精炼[1] = 装备数据.精炼值二.V;
                    }
                    if (特殊标记 == 2)
                    {
                        this.临时精炼[2] = 装备数据.精炼值三.V;
                    }
                    this.网络连接?.发送封包(new 回执精炼结果
                    {
                        结果值一 = this.临时精炼[0],
                        结果值二 = this.临时精炼[1],
                        结果值三 = this.临时精炼[2]
                    });
                }
            }
        }

        public void 装备精炼替换(byte 背包类型, byte 背包位置)
        {
            if (this.临时精炼[0] != 0)
            {
                字典监视器<byte, 物品数据> 字典监视器;
                字典监视器 = null;
                if (背包类型 == 1)
                {
                    字典监视器 = this.角色背包;
                }
                if ((字典监视器.TryGetValue(背包位置, out var v) ? v : null) is 装备数据 装备数据)
                {
                    装备数据.精炼值一.V = this.临时精炼[0];
                    装备数据.精炼值二.V = this.临时精炼[1];
                    装备数据.精炼值三.V = this.临时精炼[2];
                    装备数据.精炼次数.V++;
                    this.临时精炼[0] = 0;
                    this.临时精炼[1] = 0;
                    this.临时精炼[2] = 0;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                    this.网络连接?.发送封包(new 回执精炼结果
                    {
                        结果值一 = this.临时精炼[0],
                        结果值二 = this.临时精炼[1],
                        结果值三 = this.临时精炼[2]
                    });
                }
            }
        }

        public void 装备转移精炼(byte 背包类型, byte 背包位置, byte 材料类型, byte 材料位置)
        {
            字典监视器<byte, 物品数据> 字典监视器;
            字典监视器 = null;
            if (背包类型 == 1)
            {
                字典监视器 = this.角色背包;
            }
            物品数据 物品数据;
            物品数据 = (字典监视器.TryGetValue(背包位置, out var v) ? v : null);
            物品数据 物品数据2;
            物品数据2 = (字典监视器.TryGetValue(材料位置, out var v2) ? v2 : null);
            if (物品数据 is 装备数据 装备数据 && 物品数据2 is 装备数据 装备数据2 && 装备数据.开启精炼.V == 3 && 装备数据2.开启精炼.V == 3)
            {
                List<物品数据> list;
                list = this.查找背包物品(41001, 10);
                if (list != null)
                {
                    this.消耗背包物品(10, list, "精炼继承消耗");
                    装备数据.精炼值一.V = 装备数据2.精炼值一.V;
                    装备数据.精炼值二.V = 装备数据2.精炼值二.V;
                    装备数据.精炼值三.V = 装备数据2.精炼值三.V;
                    装备数据2.开启精炼.V = 0;
                    装备数据2.精炼值一.V = 0;
                    装备数据2.精炼值二.V = 0;
                    装备数据2.精炼值三.V = 0;
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据.字节描述()
                    });
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = 装备数据2.字节描述()
                    });
                }
            }
        }

        public ushort 获取精炼属性(装备数据 装备, bool 是否开槽)
        {
            if (是否开槽)
            {
                List<精炼属性> list;
                list = 装备精炼.属性表.Values.Where((精炼属性 O) => O.开槽几率 > 0 && O.职业专属[(int)this.角色职业] && O.装备专属[(int)装备.物品模板.物品分类]).ToList();
                list.Sort((精炼属性 a, 精炼属性 b) => (a.开槽几率 >= b.开槽几率) ? 1 : (-1));
                int num;
                num = 主程.随机数.Next(0, 10000);
                int num2;
                num2 = 0;
                while (true)
                {
                    if (num2 < list.Count)
                    {
                        if (num <= list[num2].开槽几率)
                        {
                            break;
                        }
                        num2++;
                        continue;
                    }
                    return list[0].属性编号;
                }
                return list[num2].属性编号;
            }
            List<精炼属性> list2;
            list2 = 装备精炼.属性表.Values.Where((精炼属性 O) => O.职业专属[(int)this.角色职业] && O.装备专属[(int)装备.物品模板.物品分类]).ToList();
            list2.Sort((精炼属性 a, 精炼属性 b) => (a.精炼几率一 >= b.精炼几率一) ? 1 : (-1));
            int num3;
            num3 = 主程.随机数.Next(0, 10000);
            int num4;
            num4 = 0;
            while (true)
            {
                if (num4 < list2.Count)
                {
                    if (num3 <= list2[num4].精炼几率一)
                    {
                        break;
                    }
                    num4++;
                    continue;
                }
                return list2[0].属性编号;
            }
            return list2[num4].属性编号;
        }

        public void 发送狩猎详情()
        {
            if (this.角色数据.狩猎编号.V == 0 || 主程.当前时间 > this.角色数据.狩猎刷新.V)
            {
                this.角色数据.狩猎刷新.V = 主程.当前时间.AddDays(1.0).Date;
                this.角色数据.狩猎金币.V = 50000;
                this.随机获取狩猎(0);
            }
            base.发送封包(new 请求狩猎回执
            {
                回执结果 = 0,
                狩猎编号 = this.角色数据.狩猎编号.V,
                刷新金币 = this.角色数据.狩猎金币.V,
                未知参数 = 0
            });
        }

        public void 刷新狩猎详情()
        {
            if (this.角色数据.狩猎编号.V != 0 && !(主程.当前时间 > this.角色数据.狩猎刷新.V))
            {
                if (this.金币数量 < this.角色数据.狩猎金币.V)
                {
                    return;
                }
                this.金币数量 -= (uint)this.角色数据.狩猎金币.V;
                this.角色数据.狩猎金币.V += 50000;
                this.随机获取狩猎(0);
            }
            else
            {
                this.角色数据.狩猎刷新.V = 主程.当前时间.AddDays(1.0).Date;
                this.角色数据.狩猎金币.V = 50000;
                this.随机获取狩猎(0);
            }
            base.发送封包(new 刷新狩猎回执
            {
                回执结果 = 1,
                狩猎编号 = this.角色数据.狩猎编号.V,
                刷新金币 = this.角色数据.狩猎金币.V,
                未知参数 = 0
            });
        }

        public void 随机获取狩猎(int 付费刷新)
        {
            this.角色数据.狩猎编号.V = 200 + 主程.随机数.Next(30) + 1;
            if (!高级狩猎.数据表.ContainsKey(this.角色数据.狩猎编号.V))
            {
                this.随机获取狩猎(付费刷新);
            }
        }

        public void 开始狩猎任务()
        {
            if (高级狩猎.数据表.TryGetValue(this.角色数据.狩猎编号.V, out var value))
            {
                if (this.查找背包物品(value.接取数量, value.接取物品, out var 物品列表))
                {
                    this.消耗背包物品(value.接取数量, 物品列表, "接取狩猎消耗");
                    base.发送封包(new 开始狩猎回执
                    {
                        回执结果 = 1
                    });
                    this.角色数据.已接狩猎.V = this.角色数据.狩猎编号.V;
                    this.角色数据.狩猎完成.V = 主程.当前时间.AddSeconds(value.需要时间 * value.怪物数量);
                    this.角色数据.狩猎编号.V = 0;
                }
                else
                {
                    base.发送封包(new 开始狩猎回执
                    {
                        回执结果 = 0
                    });
                }
            }
            else
            {
                base.发送封包(new 开始狩猎回执
                {
                    回执结果 = 0
                });
            }
        }

        public void 领取狩猎奖励()
        {
            if (!(this.角色数据.狩猎完成.V > 主程.当前时间) && this.背包剩余 >= 3)
            {
                if (高级狩猎.数据表.TryGetValue(this.角色数据.已接狩猎.V, out var value))
                {
                    this.角色数据.已接狩猎.V = 0;
                    int 经验增加;
                    经验增加 = value.每个怪的经验 * value.怪物数量;
                    this.玩家获得物品(value.经验奖励物品, value.经验奖励数量, "高级狩猎奖励");
                    this.玩家获得物品(value.奖励物品, value.奖励数量, "高级狩猎奖励");
                    this.玩家增加经验(null, 经验增加);
                    base.发送封包(new 狩猎奖励回执
                    {
                        回执结果 = 1
                    });
                }
                else
                {
                    base.发送封包(new 狩猎奖励回执
                    {
                        回执结果 = 0
                    });
                }
            }
            else
            {
                base.发送封包(new 狩猎奖励回执
                {
                    回执结果 = 0
                });
            }
        }

        public void 放弃狩猎任务()
        {
            this.角色数据.已接狩猎.V = 0;
            base.发送封包(new 放弃狩猎回执
            {
                回执结果 = 1
            });
        }

        public bool 检测刷新日常悬赏任务(bool 金币刷新 = false)
        {
            if (主程.当前时间 > this.角色数据.日常悬赏任务刷新.V)
            {
                if (!金币刷新)
                {
                    this.角色数据.日常悬赏刷新次数.V = 0;
                }
                DateTime now;
                now = 主程.当前时间;
                DateTime[] source;
                source = new DateTime[4]
                {
                    new DateTime(now.Year, now.Month, now.Day, 6, 0, 0),
                    new DateTime(now.Year, now.Month, now.Day, 12, 0, 0),
                    new DateTime(now.Year, now.Month, now.Day, 18, 0, 0),
                    new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1.0)
                };
                this.角色数据.日常悬赏任务刷新.V = source.FirstOrDefault((DateTime t) => t > now);
                this.角色数据.日常悬赏.Clear();
                List<GameQuests> list;
                list = GameQuests.数据表.Values.Where((GameQuests x) => x.Type == QuestType.悬赏任务 && x.Reset == QuestResetType.NoReset && this.角色数据.Quests.Any((CharacterQuest o) => o.CompleteDate.V == DateTime.MinValue && o.Info.V.Id != x.Id && x.CheckVersion <= 主程.开区节点)).ToList();
                if (list.Count < 5)
                {
                    return false;
                }
                while (this.角色数据.日常悬赏.Count < 5)
                {
                    int id;
                    id = list[主程.随机数.Next(0, list.Count)].Id;
                    if (!this.角色数据.日常悬赏.Contains(id))
                    {
                        this.角色数据.日常悬赏.Add(id);
                    }
                }
                return true;
            }
            return false;
        }

        public bool 检测刷新周常悬赏任务(bool 金币刷新 = false)
        {
            if (主程.当前时间 > this.角色数据.周常悬赏任务刷新.V)
            {
                if (!金币刷新)
                {
                    this.角色数据.周常悬赏刷新次数.V = 0;
                }
                this.角色数据.周常悬赏任务刷新.V = 主程.当前时间.AddDays(1.0).Date;
                this.角色数据.周常悬赏.Clear();
                List<GameQuests> list;
                list = GameQuests.数据表.Values.Where((GameQuests x) => x.Type == QuestType.悬赏任务 && x.Reset == QuestResetType.Weekly && this.角色数据.Quests.Any((CharacterQuest o) => o.CompleteDate.V == DateTime.MinValue && o.Info.V.Id != x.Id && x.CheckVersion <= 主程.开区节点)).ToList();
                if (list.Count < 5)
                {
                    return false;
                }
                while (this.角色数据.周常悬赏.Count < 5)
                {
                    int id;
                    id = list[主程.随机数.Next(0, list.Count)].Id;
                    if (!this.角色数据.周常悬赏.Contains(id))
                    {
                        this.角色数据.周常悬赏.Add(id);
                    }
                }
                return true;
            }
            return false;
        }

        public void 发送日常悬赏详情()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.角色数据.日常悬赏刷新次数.V);
            binaryWriter.Write(0);
            foreach (int item in this.角色数据.日常悬赏)
            {
                binaryWriter.Write(item);
            }
            this.网络连接?.发送封包(new 同步悬赏任务
            {
                任务描述 = memoryStream.ToArray()
            });
        }

        public void 发送周常悬赏详情()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.角色数据.周常悬赏刷新次数.V);
            binaryWriter.Write(1);
            foreach (int item in this.角色数据.周常悬赏)
            {
                binaryWriter.Write(item);
            }
            this.网络连接?.发送封包(new 同步悬赏任务
            {
                任务描述 = memoryStream.ToArray()
            });
        }

        public void 发送悬赏剩余计次()
        {
            this.网络连接?.发送封包(new 同步悬赏剩余
            {
                悬赏类型 = 0,
                已经完成 = 10 - this.角色数据.日常悬赏完成次数.V,
                还能完成 = this.角色数据.日常悬赏完成次数.V,
                日程进度 = 10
            });
            this.网络连接?.发送封包(new 同步悬赏剩余
            {
                悬赏类型 = 1,
                已经完成 = 15 - this.角色数据.周常悬赏完成次数.V,
                还能完成 = this.角色数据.周常悬赏完成次数.V,
                日程进度 = 15
            });
        }

        public void 发送悬赏任务数据()
        {
            if (主程.当前时间 > this.角色数据.日常悬赏计次刷新.V)
            {
                this.角色数据.日常悬赏计次刷新.V = 主程.当前时间.Date.AddDays(1.0);
                this.角色数据.日常悬赏完成次数.V = 10;
            }
            if (主程.当前时间 > this.角色数据.周常悬赏计次刷新.V)
            {
                this.角色数据.周常悬赏计次刷新.V = 主程.当前时间.Date.AddDays(1.0);
                this.角色数据.周常悬赏完成次数.V = 15;
            }
            this.检测刷新日常悬赏任务();
            this.发送日常悬赏详情();
            this.检测刷新周常悬赏任务();
            this.发送周常悬赏详情();
            this.发送悬赏剩余计次();
        }

        public void 完成悬赏任务(int 物品编号, int 物品容器, int 物品位置, int 任务编号)
        {
            CharacterQuest[] inProgressQuests;
            inProgressQuests = this.角色数据.GetInProgressQuests();
            int num;
            num = 0;
            while (true)
            {
                if (num < inProgressQuests.Length)
                {
                    CharacterQuest characterQuest;
                    characterQuest = inProgressQuests[num];
                    if (characterQuest.Info.V.Id == 任务编号)
                    {
                        CharacterQuestMission[] missionsOfType;
                        missionsOfType = characterQuest.GetMissionsOfType(QuestMissionType.RecycleItem);
                        bool flag;
                        flag = false;
                        CharacterQuestMission[] array;
                        array = missionsOfType;
                        foreach (CharacterQuestMission characterQuestMission in array)
                        {
                            if (characterQuestMission.CompletedDate.V != DateTime.MinValue || characterQuestMission.Info.V.Id != 物品编号)
                            {
                                continue;
                            }
                            List<物品数据> list;
                            list = this.查找背包物品(characterQuestMission.Info.V.Id, characterQuestMission.Info.V.Count);
                            if (list == null || !list.Any())
                            {
                                return;
                            }
                            if (list[0] is 装备数据)
                            {
                                foreach (物品数据 item in list)
                                {
                                    装备数据 装备数据;
                                    装备数据 = item as 装备数据;
                                    this.消耗背包物品(装备数据.当前持久.V, 装备数据);
                                }
                            }
                            else
                            {
                                this.消耗背包物品(characterQuestMission.Info.V.Count, list);
                            }
                            this.网络连接?.发送封包(new 悬赏回收回执
                            {
                                物品编号 = 物品编号,
                                物品位置 = 物品位置
                            });
                            characterQuestMission.Count.V = (byte)(characterQuestMission.Count.V + 1);
                            flag = true;
                        }
                        if (flag)
                        {
                            this.网络连接?.绑定角色.UpdateQuestProgress(characterQuest);
                        }
                    }
                    num++;
                    continue;
                }
                this.CompleteQuest(任务编号, 0);
                break;
            }
        }

        internal void 刷新悬赏任务(int 对话编号)
        {
            int num;
            num = 0;
            switch (对话编号)
            {
                case 1:
                    num = 100000 + this.角色数据.周常悬赏刷新次数.V * 100000;
                    if (this.金币数量 >= num)
                    {
                        this.金币数量 -= (uint)num;
                        this.角色数据.周常悬赏刷新次数.V = Math.Min(this.角色数据.周常悬赏刷新次数.V + 1, 9);
                        this.角色数据.周常悬赏任务刷新.V = DateTime.MinValue;
                        if (this.检测刷新周常悬赏任务(金币刷新: true))
                        {
                            this.发送周常悬赏详情();
                        }
                    }
                    break;
                case 0:
                    num = 50000 + this.角色数据.日常悬赏刷新次数.V * 50000;
                    if (this.金币数量 >= num)
                    {
                        this.金币数量 -= (uint)num;
                        this.角色数据.日常悬赏刷新次数.V = Math.Min(this.角色数据.日常悬赏刷新次数.V + 1, 9);
                        this.角色数据.日常悬赏任务刷新.V = DateTime.MinValue;
                        if (this.检测刷新日常悬赏任务(金币刷新: true))
                        {
                            this.发送日常悬赏详情();
                        }
                    }
                    break;
            }
        }

        public void 请求悬赏任务(byte 任务类型)
        {
            switch (任务类型)
            {
                case 1:
                    if (this.检测刷新周常悬赏任务())
                    {
                        this.发送周常悬赏详情();
                    }
                    break;
                case 0:
                    if (this.检测刷新日常悬赏任务())
                    {
                        this.发送日常悬赏详情();
                    }
                    break;
            }
        }

        public void 请求悬赏剩余(byte 任务类型)
        {
            switch (任务类型)
            {
                case 1:
                    this.网络连接?.发送封包(new 同步悬赏剩余
                    {
                        悬赏类型 = 1,
                        已经完成 = 15 - this.角色数据.周常悬赏完成次数.V,
                        还能完成 = this.角色数据.周常悬赏完成次数.V,
                        日程进度 = 15
                    });
                    break;
                case 0:
                    this.网络连接?.发送封包(new 同步悬赏剩余
                    {
                        悬赏类型 = 0,
                        已经完成 = 10 - this.角色数据.日常悬赏完成次数.V,
                        还能完成 = this.角色数据.日常悬赏完成次数.V,
                        日程进度 = 10
                    });
                    break;
            }
        }

        public byte[] 获取角色变量()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            if (!this.角色数据.角色变量.ContainsKey(50))
            {
                this.角色数据.角色变量.Add(50, 0);
            }
            binaryWriter.Write((ushort)50);
            binaryWriter.Write(this.角色数据.角色变量[50]);
            foreach (object value in Enum.GetValues(typeof(角色变量)))
            {
                if (!this.角色数据.角色变量.ContainsKey((int)value))
                {
                    this.角色数据.角色变量.Add((int)value, 0);
                    continue;
                }
                base.发送封包(new 同步补充变量
                {
                    变量类型 = 1,
                    对象编号 = this.地图编号,
                    变量索引 = (ushort)(int)value,
                    变量内容 = this.角色数据.角色变量[(int)value]
                });
            }
            return memoryStream.ToArray();
        }

        public void 修改角色变量(int 索引, int 数值)
        {
            if (!this.角色数据.角色变量.ContainsKey(索引))
            {
                this.角色数据.角色变量.Add(索引, 数值);
            }
            else
            {
                this.角色数据.角色变量[索引] = 数值;
            }
            base.发送封包(new 同步补充变量
            {
                变量类型 = 1,
                对象编号 = this.地图编号,
                变量索引 = (ushort)索引,
                变量内容 = 数值
            });
            this.UpdateQuestsProgress();
            ushort num;
            num = 0;
            switch (索引)
            {
                case 213:
                    num += 20;
                    break;
                case 187:
                    num += 10;
                    break;
                case 220:
                    num += 30;
                    break;
                case 214:
                    num += 30;
                    break;
                case 692:
                    num += 20;
                    break;
                case 693:
                    num += 30;
                    break;
                case 694:
                    num += 25;
                    break;
                case 232:
                    num += 25;
                    break;
                case 850:
                    num += 20;
                    break;
                case 803:
                    num += 60;
                    break;
            }
            if (num > 0)
            {
                this.修改战功任务(1, num);
                this.修改战功任务(2, num);
                this.修改战功任务(3, num);
                if (!Settings.屏蔽日程)
                {
                    this.角色数据.日程进度.V += num;
                    this.发送日程详情();
                }
            }
        }

        public void 发送日程详情()
        {
            if (!Settings.屏蔽日程)
            {
                base.发送封包(new 同步日程奖励
                {
                    奖励挡位 = this.角色数据.日程奖励.V
                });
                base.发送封包(new 更新活动日程
                {
                    日程进度 = this.角色数据.日程进度.V
                });
            }
        }

        public void 领取日程奖励(int 奖励进度)
        {
            if (!Settings.屏蔽日程 && this.角色数据.日程奖励.V < 奖励进度)
            {
                this.角色数据.日程奖励.V = (byte)奖励进度;
                this.玩家获得物品(140100 + 奖励进度, 1, "日程奖励领取", 是否绑定: true);
                this.发送日程详情();
            }
        }

        private byte[] 获取威望描述()
        {
            short[] array;
            array = new short[64]
            {
                1, 1, -10000, 1, 1, 1, -10000, -10000, 1, -10000,
                -10000, 1, -10000, -10000, 1, -10000, 1, 1, 1, -10000,
                -10000, 1, -10000, 1, -10000, 1, 1, -10000, 1, -10000,
                1, -10000, -10000, 1, -10000, -10000, 1, -10000, -10000, 1,
                1, 1, 1, 1, 1, -10000, -10000, -10000, 1, -10000,
                -10000, 1, -10000, 1, -10000, -10000, -10000, 1, 1, -10000,
                -10000, -10000, -10000, 1
            };
            foreach (int value in Enum.GetValues(typeof(威望类型)))
            {
                byte b;
                b = (byte)value;
                if (!this.角色数据.威望进度.ContainsKey(b))
                {
                    this.角色数据.威望进度.Add(b, 1);
                }
            }
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            BitArray bitArray;
            bitArray = new BitArray(64);
            BitArray bitArray2;
            bitArray2 = new BitArray(64);
            for (int i = 0; i < 64; i++)
            {
                bitArray.Set(i, this.角色数据.威望进度.ContainsKey((byte)i));
                if (this.角色数据.威望进度.ContainsKey((byte)i))
                {
                    bitArray2.Set(i, (this.角色数据.威望进度[(byte)i] & 0x10000000) == 268435456);
                }
            }
            byte[] array2;
            array2 = new byte[(int)Math.Ceiling(8m)];
            bitArray.CopyTo(array2, 0);
            binaryWriter.Write(array2);
            byte[] array3;
            array3 = new byte[(int)Math.Ceiling(8m)];
            bitArray2.CopyTo(array3, 0);
            binaryWriter.Write(array3);
            for (int j = 0; j < 64; j++)
            {
                binaryWriter.Write((ushort)0);
                binaryWriter.Write((byte)j);
                if (this.角色数据.威望进度.ContainsKey((byte)j))
                {
                    binaryWriter.Write((ushort)((uint)this.角色数据.威望进度[(byte)j] & 0xFFFFFu));
                }
                else
                {
                    binaryWriter.Write(array[j]);
                }
            }
            return memoryStream.ToArray();
        }

        public void 更改玩家威望(byte 序号, int 值)
        {
            if (!Settings.屏蔽威望 && 游戏威望.数据表.TryGetValue(序号, out var value))
            {
                if (值 > value.最大数值)
                {
                    值 = value.最大数值;
                }
                if ((this.角色数据.威望进度[序号] & 0x10000000) == 0)
                {
                    this.角色数据.威望进度[序号] = 值;
                    base.发送封包(new 更新玩家威望
                    {
                        更新序号 = 序号,
                        更新数值 = 值
                    });
                }
            }
        }

        public ushort 获取玩家威望(byte 序号)
        {
            return (ushort)((uint)this.角色数据.威望进度[序号] & 0xFFFFFu);
        }

        public void 领取玛法传说(byte 领取编号)
        {
            if (Settings.屏蔽威望 || (this.角色数据.威望进度[领取编号] & 0x10000000) == 268435456)
            {
                return;
            }
            if (游戏威望.数据表.TryGetValue(领取编号, out var value))
            {
                List<奖励物品> list;
                list = value.奖励物品[this.角色职业];
                if (this.背包剩余 < list.Count)
                {
                    this.网络连接?.发送封包(new 社交错误提示
                    {
                        错误编号 = 1793
                    });
                    return;
                }
                foreach (奖励物品 item in list)
                {
                    this.玩家获得物品(item.物品编号, item.物品数量, "玛法传说奖励", 是否绑定: true);
                }
            }
            this.网络连接?.发送封包(new 游戏错误提示
            {
                错误代码 = 9217,
                第一参数 = 领取编号
            });
            this.角色数据.威望进度[领取编号] |= 268435456;
            this.网络连接?.发送封包(new 同步威望列表
            {
                字节数据 = this.获取威望描述()
            });
        }

        public void 更改玩家节点(ushort 序号, ushort 值)
        {
            if (this.角色数据.节点数据.ContainsKey(序号))
            {
                this.角色数据.节点数据[序号] = 值;
            }
            else
            {
                this.角色数据.节点数据.Add(序号, 值);
            }
            base.发送封包(new 更新节点数据
            {
                节点标志 = 序号,
                节点数值 = 值
            });
        }

        public ushort 获取玩家节点(ushort 序号)
        {
            if (this.角色数据.节点数据.ContainsKey(序号))
            {
                return this.角色数据.节点数据[序号];
            }
            return 0;
        }

        public byte[] 获取节点数据()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (KeyValuePair<ushort, ushort> item in this.角色数据.节点数据)
            {
                binaryWriter.Write(item.Value);
                binaryWriter.Write(item.Key);
            }
            return memoryStream.ToArray();
        }

        public void 物品锁定状态(byte 背包类型, byte 物品位置, bool 锁定状态)
        {
            if (!this.验证密码)
            {
                this.开始验证密码();
                return;
            }
            物品数据 v;
            v = null;
            装备数据 v2;
            v2 = null;
            switch (背包类型)
            {
                case 0:
                    if (!this.角色装备.TryGetValue(物品位置, out v2))
                    {
                        return;
                    }
                    break;
                case 1:
                    if (!this.角色背包.TryGetValue(物品位置, out v))
                    {
                        return;
                    }
                    break;
                case 2:
                    if (!this.角色仓库.TryGetValue(物品位置, out v))
                    {
                        return;
                    }
                    break;
            }
            if (v != null || v2 != null)
            {
                if (v2 != null)
                {
                    v = v2;
                }
                v.上锁时间.V = (锁定状态 ? uint.MaxValue : ((uint)计算类.时间转换(主程.当前时间)));
                base.发送封包(new 玩家物品变动
                {
                    物品描述 = v.字节描述()
                });
            }
        }

        public void 仓库锁定状态(bool 锁定状态)
        {
            if (!this.验证密码)
            {
                this.开始验证密码();
                return;
            }
            base.发送封包(new 同步仓库锁定
            {
                锁定状态 = (this.角色数据.锁定仓库.V = 锁定状态)
            });
        }

        public void 开始验证密码()
        {
            if (this.角色数据.动态密码.V != null && !(this.角色数据.动态密码.V == string.Empty))
            {
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送系统消息("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送顶部公告("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
                this.发送顶部公告("请输入您设置的<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>验证密码");
            }
            else
            {
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送系统消息("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送顶部公告("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
                this.发送顶部公告("您还没有<font color='#00ff00'><u>设置密码</u></font>，请输入<font color='#00ff00'><u>6位密码</u></font>后点<font color='#00ff00'><u>确认</u></font>设置密码");
            }
            base.发送封包(new 游戏错误提示
            {
                错误代码 = 292
            });
        }

        public void 验证动态密码(string 动态密码)
        {
            if (this.角色数据.动态密码.V == null || this.角色数据.动态密码.V == string.Empty)
            {
                if (动态密码.Length != 6)
                {
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送系统消息("请输入六位验证密码");
                    this.发送顶部公告("请输入六位验证密码");
                    this.发送顶部公告("请输入六位验证密码");
                    return;
                }
                this.角色数据.动态密码.V = 动态密码;
            }
            if (this.角色数据.动态密码.V != 动态密码)
            {
                base.发送封包(new 社交错误提示
                {
                    错误编号 = 294
                });
                this.密码错误次数++;
                if (this.密码错误次数 > 4)
                {
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送系统消息("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送顶部公告("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.发送顶部公告("<font color='#ff0000'>验证密码错误次数过多，您已被强制踢下线</font>");
                    this.网络连接?.尝试断开连接(new Exception("验证二级密码错误次数达到5次, 断开连接"));
                }
            }
            else
            {
                this.验证密码 = true;
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送系统消息("密码验证成功在线期间不会再次进行验证");
                this.发送顶部公告("密码验证成功在线期间不会再次进行验证");
                this.发送顶部公告("密码验证成功在线期间不会再次进行验证");
            }
        }

        public void 领取七天大奖(byte 领取天数)
        {
            int num;
            num = 0;
            for (int i = 0; i < 7 && ((int)(this.角色数据.七天领取.V >> 40) & (1 << i)) != 0; i++)
            {
                num = i + 1;
            }
            if (this.角色数据.七天积分.V < 传永七天.积分表[num])
            {
                this.网络连接?.发送封包(new 领取大奖回执
                {
                    是否失败 = true,
                    下次奖励 = 0,
                    奖励数量 = 0
                });
            }
            else if (num > 6)
            {
                this.网络连接?.发送封包(new 领取大奖回执
                {
                    是否失败 = false,
                    下次奖励 = 0,
                    奖励数量 = 0
                });
            }
            else
            {
                this.角色数据.七天领取.V |= 1L << 40 + num;
                this.玩家获得物品(传永七天.奖励表[num], 1, "传永七天奖励", 是否绑定: true);
                this.网络连接?.发送封包(new 领取大奖回执
                {
                    是否失败 = false,
                    下次奖励 = 传永七天.奖励表[num + 1],
                    奖励数量 = 1
                });
            }
        }

        public void 领取七天奖励(byte 未知参数, int 领取编号)
        {
            if (this.角色数据.七天进度.TryGetValue((byte)领取编号, out var v) && v >= 常量类.七天最大进度表[(ushort)领取编号] && 传永七天.数据表.TryGetValue(领取编号, out var value) && (this.角色数据.七天领取.V & (1L << 领取编号 - 36)) == 0L)
            {
                this.角色数据.七天领取.V |= 1L << 领取编号 - 36;
                this.角色数据.七天积分.V += value.奖励积分;
                this.玩家获得物品(value.奖励道具, value.奖励数量, "传永七天奖励", 是否绑定: true);
                this.网络连接?.发送封包(new 更改七天状态
                {
                    状态标志 = 512,
                    任务编号 = 领取编号,
                    活动积分 = this.角色数据.七天积分.V
                });
            }
        }

        public void 修改七天进度(byte 七天编号, int 修改数值)
        {
            if (this.角色数据.七天进度.TryGetValue(七天编号, out var v) && v < 常量类.七天最大进度表[七天编号])
            {
                this.角色数据.七天进度[七天编号] = 修改数值;
            }
        }

        public void 发送七天乐详情()
        {
            if ((主程.当前时间.Date - this.角色数据.创建日期.V.Date).TotalDays < 14.0)
            {
                this.网络连接?.发送封包(new 七天开始时间
                {
                    开始时间 = 计算类.时间转换(this.角色数据.创建日期.V.Date)
                });
                base.发送封包(new 同步七天信息
                {
                    字节描述 = this.获取七天乐字节()
                });
            }
        }

        public byte[] 获取七天乐字节()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(计算类.时间转换(this.角色数据.创建日期.V.Date));
            binaryWriter.Write(this.角色数据.七天积分.V);
            int key;
            key = 0;
            for (int i = 0; i < 7 && ((int)(this.角色数据.七天领取.V >> 40) & (1 << i)) != 0; i++)
            {
                key = i + 1;
            }
            binaryWriter.Write(传永七天.奖励表[key]);
            binaryWriter.Write(40);
            binaryWriter.Write(7);
            binaryWriter.Write(5);
            binaryWriter.Write(7);
            for (int j = 36; j < 71; j++)
            {
                if (this.开启七天乐)
                {
                    switch (j)
                    {
                        case 46:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                        case 41:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                        case 36:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                        case 56:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                        case 51:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                        case 66:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                        case 61:
                            this.修改七天进度((byte)j, this.当前等级);
                            break;
                    }
                }
                if (this.角色数据.七天进度.TryGetValue((byte)j, out var v))
                {
                    binaryWriter.Write((ushort)j);
                    if (传永七天.数据表.TryGetValue(j, out var value))
                    {
                        if (value.最大值 > v)
                        {
                            binaryWriter.Write((ushort)0);
                        }
                        else
                        {
                            binaryWriter.Write((ushort)((!((主程.当前时间.Date - this.角色数据.创建日期.V.Date).TotalDays < Math.Floor((double)(j - 36) / 5.0))) ? (((this.角色数据.七天领取.V & (1 << j - 36)) == 1 << j - 36) ? 2 : 3) : 0));
                        }
                    }
                    else
                    {
                        binaryWriter.Write((ushort)0);
                    }
                    binaryWriter.Write(v);
                }
                else
                {
                    this.角色数据.七天进度.Add((byte)j, 0);
                    binaryWriter.Write(j);
                    binaryWriter.Write(0);
                }
            }
            binaryWriter.Write(new byte[368]
            {
                0, 0, 205, 158, 144, 144, 58, 12, 0, 0,
                7, 194, 253, 116, 234, 22, 0, 0, 73, 16,
                164, 93, 208, 129, 0, 0, 241, 127, 123, 99,
                165, 149, 0, 0, 205, 158, 144, 144, 50, 4,
                0, 0, 7, 193, 252, 32, 245, 74, 0, 0,
                71, 57, 62, 238, 2, 202, 0, 0, 154, 253,
                251, 18, 32, 158, 0, 0, 178, 54, 48, 255,
                61, 30, 0, 0, 14, 137, 14, 180, 54, 51,
                0, 0, 118, 159, 44, 42, 227, 6, 0, 0,
                192, 0, 0, 0, 57, 0, 0, 0, 0, 0,
                0, 58, 0, 0, 0, 0, 0, 0, 59, 0,
                0, 0, 0, 0, 0, 60, 0, 0, 0, 0,
                0, 0, 61, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 64, 0, 0, 0, 0, 0, 0,
                65, 0, 0, 0, 0, 0, 0, 66, 16, 39,
                0, 0, 0, 0, 67, 0, 0, 0, 0, 0,
                0, 68, 0, 0, 0, 0, 0, 0, 69, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 72,
                0, 0, 0, 0, 0, 0, 73, 0, 0, 0,
                0, 0, 0, 74, 0, 0, 0, 0, 0, 0,
                75, 0, 0, 0, 0, 0, 0, 76, 0, 0,
                0, 0, 0, 0, 77, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 80, 0, 0, 0, 0,
                0, 0, 81, 0, 0, 0, 0, 0, 0, 82,
                0, 0, 0, 0, 0, 0, 83, 0, 0, 0,
                40, 0, 0, 0, 105, 229, 22, 0, 1, 0,
                0, 0, 90, 0, 0, 0, 24, 121, 0, 0,
                1, 0, 0, 0, 150, 0, 0, 0, 15, 228,
                22, 0, 1, 0, 0, 0, 210, 0, 0, 0,
                134, 227, 22, 0, 1, 0, 0, 0, 24, 1,
                0, 0, 70, 228, 22, 0, 1, 0, 0, 0,
                74, 1, 0, 0, 133, 231, 22, 0, 1, 0,
                0, 0, 144, 1, 0, 0, 231, 34, 2, 0,
                1, 0, 0, 0, 1, 0, 0, 0
            });
            return memoryStream.ToArray();
        }

        public void 刷新游戏套装()
        {
            List<游戏套装> list;
            list = new List<游戏套装>();
            foreach (装备数据 value4 in this.角色装备.Values)
            {
                if (value4.装备模板.参与套装 == null)
                {
                    continue;
                }
                foreach (游戏套装 item in value4.装备模板.参与套装)
                {
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                        continue;
                    }
                    break;
                }
            }
            foreach (游戏套装 套装 in list)
            {
                int num;
                num = this.角色装备.Values.Where((装备数据 x) => x.装备模板.参与套装 != null && x.装备模板.参与套装.Contains(套装)).Count();
                bool flag;
                flag = 套装.套装BUFF == null;
                bool flag2;
                flag2 = 套装.套装属性 == null;
                bool flag3;
                flag3 = 套装.套装提示 == null;
                while (num > 0)
                {
                    if (!flag && 套装.套装BUFF.TryGetValue((byte)num, out var value))
                    {
                        flag = true;
                        base.添加Buff时处理(value, this);
                    }
                    if (!flag2 && 套装.套装属性.TryGetValue((byte)num, out var value2))
                    {
                        flag2 = true;
                        base.属性加成[套装] = value2;
                    }
                    if (!flag3 && 套装.套装提示.TryGetValue((byte)num, out var value3) && value3 != string.Empty)
                    {
                        flag3 = true;
                        this.发送系统消息(value3);
                    }
                    if (flag && flag2 && flag3)
                    {
                        break;
                    }
                    num--;
                }
            }
        }

        public void 增加装备套装(装备数据 现有装备)
        {
            if (现有装备.装备模板.参与套装 == null)
            {
                return;
            }
            foreach (游戏套装 套装 in 现有装备.装备模板.参与套装)
            {
                int num;
                num = this.角色装备.Values.Where((装备数据 x) => x.装备模板.参与套装 != null && x.装备模板.参与套装.Contains(套装)).Count();
                if (套装.套装提示 != null && 套装.套装提示.TryGetValue((byte)num, out var value) && value != string.Empty)
                {
                    this.发送系统消息(value);
                }
                bool flag;
                flag = 套装.套装BUFF == null;
                bool flag2;
                flag2 = 套装.套装属性 == null;
                while (num > 0)
                {
                    if (!flag && 套装.套装BUFF.TryGetValue((byte)num, out var value2))
                    {
                        flag = true;
                        base.添加Buff时处理(value2, this);
                    }
                    if (!flag2 && 套装.套装属性.TryGetValue((byte)num, out var value3))
                    {
                        flag2 = true;
                        base.属性加成[套装] = value3;
                    }
                    if (flag && flag2)
                    {
                        break;
                    }
                    num--;
                }
            }
        }

        public void 删除装备套装(装备数据 原有装备)
        {
            if (原有装备.装备模板.参与套装 == null)
            {
                return;
            }
            foreach (游戏套装 套装 in 原有装备.装备模板.参与套装)
            {
                int num;
                num = this.角色装备.Values.Where((装备数据 x) => x.装备模板.参与套装 != null && x.装备模板.参与套装.Contains(套装)).Count() + 1;
                bool flag;
                flag = 套装.套装BUFF == null;
                bool flag2;
                flag2 = 套装.套装属性 == null;
                while (num > 0)
                {
                    if (!flag && 套装.套装BUFF.TryGetValue((byte)num, out var value))
                    {
                        flag = true;
                        base.删除Buff时处理(value, 后接BUFF: false);
                    }
                    if (!flag2 && 套装.套装属性.TryGetValue((byte)num, out var _))
                    {
                        flag2 = true;
                        base.属性加成.Remove(套装);
                    }
                    if (flag && flag2)
                    {
                        break;
                    }
                    num--;
                }
            }
        }

        public void 购买战功军令()
        {
            if (!Settings.屏蔽战功 && this.元宝数量 >= 68800 && !this.角色数据.开启战令.V)
            {
                this.元宝数量 -= 68800u;
                this.角色数据.消耗元宝.V += 68800L;
                主程.添加货币日志(this, "开通战功消耗", 游戏货币.元宝, -68800);
                this.角色数据.开启战令.V = true;
                this.发送战功详情();
            }
        }

        public void 领取战功奖励(int 领取类型)
        {
            if (Settings.屏蔽战功)
            {
                return;
            }
            List<战功奖励> list;
            list = 战功奖励.数据表.Values.Where((战功奖励 x) => (x.检测战令 ? 1 : 0) == 领取类型 && (x.检测战令 ? (this.角色数据.军机奖励.V < x.需要点数) : (this.角色数据.战功奖励.V < x.需要点数)) && this.全部战功 >= x.需要点数).ToList();
            if (list == null || list.Count == 0)
            {
                return;
            }
            foreach (战功奖励 item in list)
            {
                this.玩家获得物品(item.物品编号, item.物品数量, "战功领取奖励");
            }
            if (领取类型 == 0)
            {
                this.角色数据.战功奖励.V = list.Max((战功奖励 x) => x.需要点数);
            }
            else
            {
                this.角色数据.军机奖励.V = list.Max((战功奖励 x) => x.需要点数);
            }
            this.发送战功详情();
        }

        public void 购买战功积分(int 购买类型)
        {
            if (Settings.屏蔽战功 || (购买类型 == 0 && this.角色数据.战功次数.V >= 3))
            {
                return;
            }
            int num;
            num = ((购买类型 == 0) ? 3600 : 1000);
            if (this.元宝数量 >= num)
            {
                this.元宝数量 -= (uint)num;
                this.角色数据.消耗元宝.V += num;
                主程.添加货币日志(this, "战功积分消耗", 游戏货币.元宝, -num);
                if (购买类型 == 0)
                {
                    this.角色数据.战功进度.V += 400;
                    this.角色数据.战功次数.V++;
                    this.发送战功详情();
                }
                else
                {
                    this.角色数据.购买战功.V += 48;
                    base.发送封包(new 战功等级提升
                    {
                        未知参数一 = this.角色数据.战功进度.V,
                        未知参数二 = this.角色数据.购买战功.V
                    });
                }
            }
        }

        public void 发送战功任务(int 任务类型)
        {
            if (Settings.屏蔽战功)
            {
                return;
            }
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            IEnumerable<KeyValuePair<ushort, ushort>> enumerable;
            enumerable = this.角色数据.战功任务.Where((KeyValuePair<ushort, ushort> x) => 战功任务.数据表.ContainsKey(x.Key) && 战功任务.数据表[x.Key].任务分类 == (QuestResetType)((任务类型 == 0) ? 1 : 2));
            binaryWriter.Write(enumerable.Count());
            foreach (KeyValuePair<ushort, ushort> item in enumerable)
            {
                binaryWriter.Write(item.Key);
                binaryWriter.Write(item.Value);
            }
            base.发送封包(new 同步战功任务
            {
                任务描述 = memoryStream.ToArray()
            });
        }

        public void 发送战功详情()
        {
            this.网络连接?.发送封包(new 同步战功信息
            {
                开始时间 = 1687104000,
                结束时间 = 1694880000,
                战功进度 = this.角色数据.战功进度.V,
                购买战功 = this.角色数据.购买战功.V,
                战功奖励 = this.角色数据.战功奖励.V,
                军机奖励 = this.角色数据.军机奖励.V,
                战功状态 = (this.角色数据.开启战令.V ? 1 : 0),
                购买次数 = this.角色数据.战功次数.V,
                未知参数二 = 0,
                开始时间二 = 1687104000,
                未知参数三 = 0
            });
        }

        public void 特权玩家登录(byte 特权类型)
        {
            switch (特权类型)
            {
                case 3:
                    this.修改战功任务(20, 1);
                    break;
                case 4:
                    this.修改战功任务(4, 1);
                    break;
                case 5:
                    this.修改战功任务(5, 1);
                    break;
                case 6:
                    this.修改战功任务(6, 1);
                    break;
                case 7:
                    this.修改战功任务(21, 1);
                    break;
            }
        }

        public void 修改战功任务(ushort 任务编号, ushort 任务进度)
        {
            if (!Settings.屏蔽战功 && 战功任务.数据表.TryGetValue(任务编号, out var value))
            {
                if (this.角色数据.战功任务.ContainsKey(任务编号))
                {
                    this.角色数据.战功任务[任务编号] += 任务进度;
                }
                else
                {
                    this.角色数据.战功任务.Add(任务编号, 任务进度);
                }
                base.发送封包(new 更新战功任务
                {
                    任务编号 = 任务编号,
                    任务进度 = 任务进度
                });
                if (this.角色数据.战功任务[任务编号] >= value.最大数值 && this.角色数据.战功任务[任务编号] - 任务进度 < value.最大数值)
                {
                    this.角色数据.战功进度.V += value.奖励点数;
                    base.发送封包(new 战功等级提升
                    {
                        未知参数一 = this.角色数据.战功进度.V,
                        未知参数二 = this.角色数据.购买战功.V
                    });
                }
            }
        }

        public void 查询奖励找回()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((ushort)this.角色数据.找回奖励.Count);
            foreach (KeyValuePair<ushort, ushort> item in this.角色数据.找回奖励)
            {
                binaryWriter.Write((int)item.Key);
                binaryWriter.Write((int)常量类.找回奖励字典[item.Key]);
                binaryWriter.Write((int)item.Value);
            }
            base.发送封包(new 同步奖励找回
            {
                字节描述 = memoryStream.ToArray()
            });
        }

        public void 找回日程奖励(int 日程编号, int 找回次数)
        {
            if (Settings.屏蔽日程)
            {
                return;
            }
            if (找回次数 <= 0 || 找回次数 > ushort.MaxValue || 日程编号 < 0 || 日程编号 > ushort.MaxValue)
            {
                this.网络连接?.尝试断开连接(new Exception("错误操作: 找回日程奖励, 错误: 参数非法"));
                return;
            }
            if (!常量类.找回奖励字典.ContainsKey((ushort)日程编号) || !常量类.找回奖励费用.ContainsKey((ushort)日程编号) || !常量类.找回奖励物品1.ContainsKey((ushort)日程编号))
            {
                return;
            }
            ushort num;
            num = 常量类.找回奖励字典[(ushort)日程编号];
            long num2L;
            num2L = (long)常量类.找回奖励费用[(ushort)日程编号] * (long)找回次数;
            if (num2L <= 0 || num2L > int.MaxValue)
            {
                return;
            }
            int num2;
            num2 = (int)num2L;
            if (this.角色数据.找回奖励.TryGetValue((ushort)日程编号, out var v) && (long)v + (long)找回次数 <= (long)num)
            {
                List<物品数据> list;
                list = this.查找背包物品(2315, num2);
                if (list == null)
                {
                    list = this.查找背包物品(90226, num2);
                }
                if (list != null)
                {
                    this.角色数据.找回奖励[(ushort)日程编号] = (ushort)(this.角色数据.找回奖励[(ushort)日程编号] + 找回次数);
                    this.消耗背包物品(num2, list, "找回奖励消耗");
                    this.玩家获得物品(常量类.找回奖励物品1[(ushort)日程编号], 找回次数, "日程找回奖励", 是否绑定: true);
                    base.发送封包(new 找回奖励物品
                    {
                        找回结果 = 日程编号,
                        日程编号 = num,
                        剩余次数 = this.角色数据.找回奖励[(ushort)日程编号]
                    });
                }
            }
        }

        public void 刷新生效龙卫()
        {
            this.生效龙卫.Clear();
            foreach (龙卫数据 item in this.角色数据.龙卫属性)
            {
                if (item.是否激活)
                {
                    ushort key;
                    key = (ushort)item.龙卫模板.龙卫编号;
                    if (this.生效龙卫.ContainsKey(key))
                    {
                        this.生效龙卫[key] += item.属性值.V;
                    }
                    else
                    {
                        this.生效龙卫.Add(key, item.属性值.V);
                    }
                }
            }
        }

        public void 玩家勋章洗炼(byte 未知参数, byte 物品位置)
        {
            if (!this.角色背包.TryGetValue(物品位置, out var v) || !(v is 装备数据 { 物品类型: 物品使用分类.勋章 } 装备数据))
            {
                return;
            }
            Dictionary<int, float> dictionary;
            dictionary = new Dictionary<int, float>
            {
                { 7001, 1f },
                { 8001, 0.7f },
                { 9001, 1f },
                { 10001, 0.7f },
                { 11001, 1f },
                { 11003, 0.8f },
                { 11005, 0.8f },
                { 11007, 0.8f },
                { 11009, 0.8f }
            };
            Dictionary<int, float> dictionary2;
            dictionary2 = new Dictionary<int, float>
            {
                { 1001, 1f },
                { 2001, 1f },
                { 3001, 1f },
                { 4001, 1f },
                { 5001, 1f },
                { 6001, 1f },
                { 44001, 1f },
                { 45001, 1f },
                { 46001, 1f },
                { 47001, 1f },
                { 7001, 1f },
                { 7011, 1f },
                { 8001, 0.7f },
                { 8011, 0.7f },
                { 9001, 1f },
                { 9011, 1f },
                { 10001, 0.7f },
                { 10011, 0.7f },
                { 11001, 1f },
                { 11003, 0.8f },
                { 11005, 0.8f },
                { 11007, 0.8f },
                { 11009, 0.8f },
                { 11011, 0.8f },
                { 11013, 0.8f }
            };
            if (!this.对象死亡 && this.摆摊状态 <= 0 && this.交易状态 < 3)
            {
                List<物品数据> list;
                list = this.查找背包物品(111101, 1);
                if (list != null)
                {
                    Dictionary<int, float> dictionary3;
                    dictionary3 = null;
                    uint num;
                    num = 0u;
                    if (装备数据.物品编号 / 10 == 9996120)
                    {
                        dictionary3 = dictionary;
                        num = 1000u;
                    }
                    if (装备数据.物品编号 / 10 == 9996130 || 装备数据.物品编号 / 10 == 9996140)
                    {
                        dictionary3 = dictionary2;
                        num = 1000u;
                    }
                    if (dictionary3 != null && this.金币数量 >= num)
                    {
                        this.扣金币(num);
                        this.消耗背包物品(1, list, "洗炼勋章消耗");
                        this.勋章洗炼 = (ushort)计算类.概率表取值(dictionary3);
                    }
                }
            }
            if (this.勋章洗炼 != 0)
            {
                this.洗炼勋章 = 装备数据;
                base.发送封包(new 勋章洗练回执
                {
                    属性一 = this.勋章洗炼
                });
            }
        }

        public void 替换勋章洗炼()
        {
            if (this.洗炼勋章 != null)
            {
                if (this.洗炼勋章.随机属性.Count == 0)
                {
                    this.洗炼勋章.随机属性.Add(随机属性.数据表[this.勋章洗炼]);
                }
                else
                {
                    this.洗炼勋章.随机属性[0] = 随机属性.数据表[this.勋章洗炼];
                }
                base.发送封包(new 玩家物品变动
                {
                    物品描述 = this.洗炼勋章.字节描述()
                });
                base.发送封包(new 游戏错误提示
                {
                    错误代码 = 1
                });
            }
        }

        public void 扩展龙卫记录()
        {
            if (this.金币数量 >= 5000000)
            {
                this.角色数据.龙卫记录五.V = "记录5";
                this.金币数量 -= 5000000u;
                this.网络连接?.SendRaw(329, 6, new byte[4] { 5, 0, 0, 0 });
            }
        }

        public void 成就变量变更(AchievementVariables Type, int Value)
        {
            this.角色数据.AchievementVariables[(byte)Type] += Value;
            base.发送封包(new 更新成就变量
            {
                序号 = (int)Type,
                序号2 = this.角色数据.AchievementVariables[(byte)Type]
            });
        }

        public void 成就变量赋值(AchievementVariables Type, int Value, bool UseMax)
        {
            this.角色数据.AchievementVariables[(byte)Type] = (UseMax ? Math.Max(this.角色数据.AchievementVariables[(byte)Type], Value) : Value);
            base.发送封包(new 更新成就变量
            {
                序号 = (int)Type,
                序号2 = this.角色数据.AchievementVariables[(byte)Type]
            });
        }

        public byte[] 获取击杀任务()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            int num;
            num = 256;
            for (ushort num2 = 0; num2 < num; num2++)
            {
                binaryWriter.Write(this.角色数据.杀怪成就[num2]);
            }
            new BitArray(num);
            for (ushort num3 = 0; num3 < num; num3++)
            {
                binaryWriter.Write(this.角色数据.杀怪领取[num3]);
            }
            memoryStream.Seek(768L, SeekOrigin.Begin);
            binaryWriter.Write(this.地图编号);
            return memoryStream.ToArray();
        }

        public void 杀怪成就变更(ushort Type, ushort Value)
        {
            this.角色数据.杀怪成就[Type] += Value;
            base.发送封包(new 杀怪成就变更
            {
                成就序号 = Type,
                成就进度 = this.角色数据.杀怪成就[Type]
            });
        }

        public void 领取杀怪成就(ushort 成就编号, byte 进度编号)
        {
            if (杀怪成就.数据表.TryGetValue(成就编号, out var value) && (this.角色数据.杀怪领取[成就编号] & (1 << (int)进度编号)) == 0 && this.角色数据.杀怪成就[成就编号] >= value.击杀数量[进度编号])
            {
                this.角色数据.杀怪领取[成就编号] |= (byte)(1 << (int)进度编号);
                this.玩家获得货币(游戏货币.成就点数, value.成就点数[进度编号]);
                this.玩家增加经验(null, value.奖励经验[进度编号]);
                base.发送封包(new 杀怪成就回执
                {
                    成就序号 = 成就编号,
                    进度编号 = 进度编号
                });
            }
        }

        public byte[] 获取城主雕像描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.地图编号);
            binaryWriter.Write("有一说一");
            binaryWriter.Write((byte)0);
            memoryStream.Seek(36L, SeekOrigin.Begin);
            binaryWriter.Write((byte)this.角色职业);
            binaryWriter.Write((byte)this.角色性别);
            binaryWriter.Write((byte)this.角色发型);
            binaryWriter.Write((byte)this.角色发色);
            binaryWriter.Write((byte)this.角色脸型);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.当前称号);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.角色装备.TryGetValue(0, out var v) ? (v?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(1, out var v2) ? (v2?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(2, out var v3) ? (v3?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.地图编号);
            binaryWriter.Write("星空永恒");
            memoryStream.Seek(92L, SeekOrigin.Begin);
            binaryWriter.Write((byte)this.角色职业);
            binaryWriter.Write((byte)this.角色性别);
            binaryWriter.Write((byte)this.角色发型);
            binaryWriter.Write((byte)this.角色发色);
            binaryWriter.Write((byte)this.角色脸型);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.当前称号);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.角色装备.TryGetValue(0, out var v4) ? (v4?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(1, out var v5) ? (v5?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(2, out var v6) ? (v6?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.地图编号);
            binaryWriter.Write("真的好玩");
            memoryStream.Seek(148L, SeekOrigin.Begin);
            binaryWriter.Write((byte)this.角色职业);
            binaryWriter.Write((byte)this.角色性别);
            binaryWriter.Write((byte)this.角色发型);
            binaryWriter.Write((byte)this.角色发色);
            binaryWriter.Write((byte)this.角色脸型);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.当前称号);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.角色装备.TryGetValue(0, out var v7) ? (v7?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(1, out var v8) ? (v8?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(2, out var v9) ? (v9?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.地图编号);
            binaryWriter.Write("766627846");
            memoryStream.Seek(204L, SeekOrigin.Begin);
            binaryWriter.Write((byte)this.角色职业);
            binaryWriter.Write((byte)this.角色性别);
            binaryWriter.Write((byte)this.角色发型);
            binaryWriter.Write((byte)this.角色发色);
            binaryWriter.Write((byte)this.角色脸型);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.当前称号);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.角色装备.TryGetValue(0, out var v10) ? (v10?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(1, out var v11) ? (v11?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            binaryWriter.Write(this.角色装备.TryGetValue(2, out var v12) ? (v12?.对应模板?.V?.物品编号).GetValueOrDefault() : 0);
            return memoryStream.ToArray();
        }

        public void 玩家请求主题礼包()
        {
            this.网络连接.发送封包(new 同步主题礼包
            {
                购买数据 = this.主题礼包描述()
            });
        }

        public void 玩家购买主题礼包(int m_日期序号, int 物品A_ID, int 物品B_ID, int 物品C_ID, int 物品D_ID)
        {
            if (!this.判断主题礼包日期((int)主程.当前时间.DayOfWeek, m_日期序号))
            {
                return;
            }
            if (!this.扣除货币(游戏货币.元宝, 6800))
            {
                this.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 8451
                });
                return;
            }
            int value;
            value = 0;
            if (主题礼包固定物品模板.数据表.TryGetValue(m_日期序号, out var value2))
            {
                if (!value2.ContainsKey(物品A_ID))
                {
                    return;
                }
                value2.TryGetValue(物品A_ID, out value);
            }
            int value3;
            value3 = 0;
            int value4;
            value4 = 0;
            int value5;
            value5 = 0;
            if (主题礼包商品模板.数据表.ContainsKey(物品B_ID) && 主题礼包商品模板.数据表.ContainsKey(物品C_ID) && 主题礼包商品模板.数据表.ContainsKey(物品D_ID))
            {
                主题礼包商品模板.数据表.TryGetValue(物品B_ID, out value3);
                主题礼包商品模板.数据表.TryGetValue(物品C_ID, out value4);
                主题礼包商品模板.数据表.TryGetValue(物品D_ID, out value5);
                游戏物品 value6;
                游戏物品 value7;
                游戏物品 value8;
                游戏物品 value9;
                游戏物品 value10;
                if (!this.角色数据.尝试获取背包空余格子(5, out var location))
                {
                    this.网络连接?.发送封包(new 游戏错误提示
                    {
                        错误代码 = 6459
                    });
                }
                else if (游戏物品.数据表.TryGetValue(物品A_ID, out value6) && 游戏物品.数据表.TryGetValue(物品B_ID, out value7) && 游戏物品.数据表.TryGetValue(物品C_ID, out value8) && 游戏物品.数据表.TryGetValue(物品D_ID, out value9) && 游戏物品.检索表.TryGetValue("如意丹", out value10))
                {
                    this.角色背包[location[0]] = new 物品数据(value6, this.角色数据, 1, location[0], value, 绑定: true);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[location[0]].字节描述()
                    });
                    this.角色背包[location[1]] = new 物品数据(value7, this.角色数据, 1, location[1], value3, 绑定: true);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[location[1]].字节描述()
                    });
                    this.角色背包[location[2]] = new 物品数据(value8, this.角色数据, 1, location[2], value4, 绑定: true);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[location[2]].字节描述()
                    });
                    this.角色背包[location[3]] = new 物品数据(value9, this.角色数据, 1, location[3], value5, 绑定: true);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[location[3]].字节描述()
                    });
                    this.角色背包[location[4]] = new 物品数据(value10, this.角色数据, 1, location[4], 5, 绑定: true);
                    this.网络连接?.发送封包(new 玩家物品变动
                    {
                        物品描述 = this.角色背包[location[4]].字节描述()
                    });
                    this.保存_主题礼包购买数据((int)主程.当前时间.DayOfWeek, m_日期序号, 物品A_ID, 物品B_ID, 物品C_ID, 物品D_ID);
                    this.网络连接?.发送封包(new 同步主题礼包
                    {
                        购买数据 = this.主题礼包描述()
                    });
                }
            }
        }

        public bool 判断主题礼包日期(int m_系统日期序号, int m_封包日期序号)
        {
            switch (m_系统日期序号)
            {
                default:
                    return false;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    if (m_系统日期序号 != m_封包日期序号)
                    {
                        return false;
                    }
                    return true;
                case 0:
                case 6:
                    if (m_封包日期序号 < 1 && m_封包日期序号 > 5)
                    {
                        return false;
                    }
                    return true;
            }
        }

        public void 保存_主题礼包购买数据(int m_系统日期序号, int m_封包日期序号, int 物品A_ID, int 物品B_ID, int 物品C_ID, int 物品D_ID)
        {
            switch (m_系统日期序号)
            {
                case 0:
                    this.角色数据.主题礼包_星期日_购买数据.Clear();
                    this.角色数据.主题礼包_星期日_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期日_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期日_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期日_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期日_购买数据.Add(物品D_ID);
                    break;
                case 1:
                    this.角色数据.主题礼包_星期一_购买数据.Clear();
                    this.角色数据.主题礼包_星期一_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期一_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期一_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期一_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期一_购买数据.Add(物品D_ID);
                    break;
                case 2:
                    this.角色数据.主题礼包_星期二_购买数据.Clear();
                    this.角色数据.主题礼包_星期二_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期二_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期二_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期二_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期二_购买数据.Add(物品D_ID);
                    break;
                case 3:
                    this.角色数据.主题礼包_星期三_购买数据.Clear();
                    this.角色数据.主题礼包_星期三_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期三_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期三_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期三_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期三_购买数据.Add(物品D_ID);
                    break;
                case 4:
                    this.角色数据.主题礼包_星期四_购买数据.Clear();
                    this.角色数据.主题礼包_星期四_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期四_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期四_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期四_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期四_购买数据.Add(物品D_ID);
                    break;
                case 5:
                    this.角色数据.主题礼包_星期五_购买数据.Clear();
                    this.角色数据.主题礼包_星期五_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期五_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期五_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期五_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期五_购买数据.Add(物品D_ID);
                    break;
                case 6:
                    this.角色数据.主题礼包_星期六_购买数据.Clear();
                    this.角色数据.主题礼包_星期六_购买数据.Add(m_封包日期序号);
                    this.角色数据.主题礼包_星期六_购买数据.Add(物品A_ID);
                    this.角色数据.主题礼包_星期六_购买数据.Add(物品B_ID);
                    this.角色数据.主题礼包_星期六_购买数据.Add(物品C_ID);
                    this.角色数据.主题礼包_星期六_购买数据.Add(物品D_ID);
                    break;
            }
        }

        public byte[] 主题礼包描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            if (this.角色数据.主题礼包_星期日_购买数据.Count > 0)
            {
                foreach (int item in this.角色数据.主题礼包_星期日_购买数据)
                {
                    binaryWriter.Write(item);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            if (this.角色数据.主题礼包_星期一_购买数据.Count > 0)
            {
                foreach (int item2 in this.角色数据.主题礼包_星期一_购买数据)
                {
                    binaryWriter.Write(item2);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            if (this.角色数据.主题礼包_星期二_购买数据.Count > 0)
            {
                foreach (int item3 in this.角色数据.主题礼包_星期二_购买数据)
                {
                    binaryWriter.Write(item3);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            if (this.角色数据.主题礼包_星期三_购买数据.Count > 0)
            {
                foreach (int item4 in this.角色数据.主题礼包_星期三_购买数据)
                {
                    binaryWriter.Write(item4);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            if (this.角色数据.主题礼包_星期四_购买数据.Count > 0)
            {
                foreach (int item5 in this.角色数据.主题礼包_星期四_购买数据)
                {
                    binaryWriter.Write(item5);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            if (this.角色数据.主题礼包_星期五_购买数据.Count > 0)
            {
                foreach (int item6 in this.角色数据.主题礼包_星期五_购买数据)
                {
                    binaryWriter.Write(item6);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            if (this.角色数据.主题礼包_星期六_购买数据.Count > 0)
            {
                foreach (int item7 in this.角色数据.主题礼包_星期六_购买数据)
                {
                    binaryWriter.Write(item7);
                }
            }
            else
            {
                binaryWriter.Write(new byte[20]);
            }
            return memoryStream.ToArray();
        }

        public void 开始锻石炼药(int 模版编号, byte 基础材料容器, byte 基础材料位置, byte 额外材料容器, byte 额外材料位置, byte 额外材料二容器, byte 额外材料二位置)
        {
            if (!锻石炼药.数据表.TryGetValue(模版编号, out var value) || this.金币数量 < value.需要金币 || !this.主体技能表.TryGetValue(value.检测技能, out var v) || v.技能等级.V < value.需要等级)
            {
                return;
            }
            int num;
            num = 0;
            if (this.角色背包.TryGetValue(基础材料位置, out var v2))
            {
                if (v2.物品编号 == value.基础材料一编号)
                {
                    num += value.基础材料一概率;
                }
                else if (v2.物品编号 == value.基础材料二编号)
                {
                    num += value.基础材料二概率;
                }
            }
            if (num == 0 || v2.当前持久.V < value.基础材料数量)
            {
                return;
            }
            物品数据 物品数据;
            物品数据 = null;
            物品数据 物品数据2;
            物品数据2 = null;
            if (额外材料容器 > 0 && this.角色背包.TryGetValue(额外材料位置, out var v3))
            {
                if (v3.物品编号 == value.额外材料一编号 && v3.当前持久.V >= value.额外材料一数量)
                {
                    物品数据 = v3;
                }
                else if (v3.物品编号 == value.额外材料二编号 && v3.当前持久.V >= value.额外材料二数量)
                {
                    物品数据2 = v3;
                }
            }
            if (额外材料二容器 > 0 && this.角色背包.TryGetValue(额外材料二位置, out var v4))
            {
                if (v4.物品编号 == value.额外材料一编号 && v4.当前持久.V >= value.额外材料一数量)
                {
                    物品数据 = v4;
                }
                else if (v4.物品编号 == value.额外材料二编号 && v4.当前持久.V >= value.额外材料二数量)
                {
                    物品数据2 = v4;
                }
            }
            if ((value.额外材料一编号 > 0 && 物品数据 == null) || (value.额外材料二编号 > 0 && 物品数据2 == null))
            {
                return;
            }
            this.金币数量 -= (uint)value.需要金币;
            this.消耗背包物品(value.基础材料数量, v2, "锻石炼药消耗");
            if (value.额外材料一编号 > 0)
            {
                this.消耗背包物品(value.额外材料一数量, 物品数据, "锻石炼药消耗");
            }
            if (value.额外材料二编号 > 0)
            {
                this.消耗背包物品(value.额外材料二数量, 物品数据2, "锻石炼药消耗");
            }
            if (计算类.计算概率((float)num / 1000000f))
            {
                this.玩家获得物品(value.奖励道具, value.奖励数量, "锻石炼药获得");
                if (value.需要等级 > 3)
                {
                    if (value.模版类型 == 1)
                    {
                        网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{value.奖励道具}>><#T:991043>");
                    }
                    else
                    {
                        网络服务网关.发送公告($"<#P0:<#PN:{this.对象名字}>><#P1:<#I:{value.奖励道具}>><#T:991044>");
                    }
                }
                base.发送封包(new 锻石炼药回执
                {
                    回执编号 = 1
                });
            }
            else
            {
                base.发送封包(new 锻石炼药回执
                {
                    回执编号 = 0
                });
            }
        }

        public void 角色防具升级(int 装备部位)
        {
            if (!this.角色装备.TryGetValue((byte)装备部位, out var v) || v.升级次数.V >= 9 || !装备升级.数据表.TryGetValue(new 升级装备
            {
                装备编号 = v.物品编号,
                升级等级 = v.升级次数.V
            }, out var value) || this.金币数量 < value.需要金币)
            {
                return;
            }
            List<物品数据> list;
            list = null;
            List<物品数据> list2;
            list2 = null;
            List<物品数据> list3;
            list3 = null;
            List<物品数据> list4;
            list4 = null;
            if (value.需要物品一.编号 > 0)
            {
                list = this.查找背包物品(value.需要物品一.编号, value.需要物品一.数量);
                if (list == null)
                {
                    return;
                }
            }
            if (value.需要物品二.编号 > 0)
            {
                list2 = this.查找背包物品(value.需要物品二.编号, value.需要物品二.数量);
                if (list2 == null)
                {
                    return;
                }
            }
            if (value.需要物品三.编号 > 0)
            {
                list3 = this.查找背包物品(value.需要物品三.编号, value.需要物品三.数量);
                if (list3 == null)
                {
                    return;
                }
            }
            if (value.需要物品四.编号 > 0)
            {
                list4 = this.查找背包物品(value.需要物品四.编号, value.需要物品四.数量);
                if (list4 == null)
                {
                    return;
                }
            }
            this.扣金币((uint)value.需要金币);
            if (list != null)
            {
                this.消耗背包物品(value.需要物品一.数量, list, "防具升级消耗");
            }
            if (list2 != null)
            {
                this.消耗背包物品(value.需要物品二.数量, list2, "防具升级消耗");
            }
            if (list3 != null)
            {
                this.消耗背包物品(value.需要物品三.数量, list3, "防具升级消耗");
            }
            if (list4 != null)
            {
                this.消耗背包物品(value.需要物品四.数量, list4, "防具升级消耗");
            }
            if (v.失败次数.V < 3 && !计算类.计算概率(0.1f))
            {
                v.失败次数.V++;
                this.更新物品详情(v);
                base.发送封包(new 防具升级通知
                {
                    通知结果 = 1
                });
                return;
            }
            v.升级次数.V++;
            v.失败次数.V = 0;
            base.属性加成[v] = v.装备属性;
            this.战力加成[v] = v.装备战力;
            this.更新物品详情(v);
            this.更新对象属性();
            this.更新玩家战力();
            this.刷新防具BUFF();
            base.发送封包(new 防具升级通知
            {
                通知结果 = 0
            });
        }

        public void CallDefaultNPC(DefaultNPCType type, bool delay, params object[] value)
        {
            string arg;
            arg = string.Empty;
            switch (type)
            {
                case DefaultNPCType.Login:
                    arg = "Login";
                    break;
                case DefaultNPCType.LevelUp:
                    arg = "LevelUp";
                    break;
                case DefaultNPCType.UseItem:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"UseItem({value[0]})";
                    break;
                case DefaultNPCType.MapCoord:
                    if (value.Length < 3)
                    {
                        return;
                    }
                    arg = $"MapCoord({value[0]},{value[1]},{value[2]})";
                    break;
                case DefaultNPCType.MapEnter:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"MapEnter({value[0]})";
                    break;
                case DefaultNPCType.Die:
                    arg = "Die";
                    break;
                case DefaultNPCType.MonDie:
                    arg = "MonDie";
                    break;
                case DefaultNPCType.RedeemCode:
                    arg = $"RedeemCode({value[0]})";
                    break;
                case DefaultNPCType.PlayerDie:
                    arg = "PlayerDie";
                    break;
                case DefaultNPCType.TryOpenDropBox:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"TryOpenDropBox({value[0]})";
                    break;
                case DefaultNPCType.OpenDropBox:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"OpenDropBox({value[0]})";
                    break;
                case DefaultNPCType.StopOpenDropBox:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"StopOpenDropBox({value[0]})";
                    break;
                case DefaultNPCType.MotaSuccess:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"MotaSuccess({value[0]})";
                    break;
                case DefaultNPCType.buff_add:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"buff_add({value[0]})";
                    break;
                case DefaultNPCType.buff_remove:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"buff_remove({value[0]})";
                    break;
                case DefaultNPCType.buff_delete:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"buff_delete({value[0]})";
                    break;
                case DefaultNPCType.buff_run:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"buff_run({value[0]})";
                    break;
                case DefaultNPCType.DeCompose:
                    arg = "DeCompose";
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"DeCompose({value[0]})";
                    break;
                case DefaultNPCType.Trigger:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"Trigger({value[0]})";
                    break;
                case DefaultNPCType.CustomCommand:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"CustomCommand({value[0]})";
                    break;
                case DefaultNPCType.OnAcceptQuest:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"OnAcceptQuest({value[0]})";
                    break;
                case DefaultNPCType.OnFinishQuest:
                    if (value.Length < 1)
                    {
                        return;
                    }
                    arg = $"OnFinishQuest({value[0]})";
                    break;
                case DefaultNPCType.DayChange:
                    arg = "DayChange";
                    break;
                case DefaultNPCType.PlayKill:
                    arg = "PlayKill";
                    break;
                case DefaultNPCType.KillPlay:
                    arg = "KillPlay";
                    break;
                case DefaultNPCType.Client:
                    arg = "Client";
                    break;
                case DefaultNPCType.Timer:
                    arg = $"OnTimer({value[0]})";
                    break;
                case DefaultNPCType.ItemRestore:
                    arg = $"OnItemRestore({value[0]})";
                    break;
            }
            this.NPCDelayed = true;
            arg = $"[@_{arg}]";
            if (delay)
            {
                DelayedAction item;
                item = new DelayedAction(DelayedType.NPC, 主程.当前时间, 主程.DefaultNPC.LoadedObjectID, 主程.DefaultNPC.ScriptID, arg);
                base.ActionList.Add(item);
            }
            else
            {
                NPCScript.Get(主程.DefaultNPC.ScriptID).Call(this, 主程.DefaultNPC.LoadedObjectID, arg.ToUpper());
            }
        }

        private void CallNPCNextPage()
        {
            for (int i = 0; i < base.ActionList.Count; i++)
            {
                if (base.ActionList[i].Type == DelayedType.NPC && !(base.ActionList[i].Time != DateTime.MinValue))
                {
                    DelayedAction delayedAction;
                    delayedAction = base.ActionList[i];
                    base.ActionList.RemoveAt(i);
                    this.CompleteNPC(delayedAction.Params);
                }
            }
        }

        public void CallNPC(int objectID, int scriptID, string key)
        {
            key = key.ToUpper();
            NPCScript.Get(scriptID).Call(this, objectID, key);
            this.CallNPCNextPage();
        }

        private void CompleteNPC(IList<object> data)
        {
            int num;
            num = (int)data[0];
            int index;
            index = (int)data[1];
            string text;
            text = (string)data[2];
            if (data.Count == 5)
            {
                this.玩家切换地图((地图实例)data[3], 地图区域类型.未知区域, (Point)data[4]);
            }
            this.NPCDelayed = true;
            if (text.Length <= 0)
            {
                return;
            }
            NPCScript nPCScript;
            nPCScript = NPCScript.Get(index);
            if (num != 主程.DefaultNPCID)
            {
                if (text.ToUpper() == "[@MAIN]")
                {
                    this.对话触发 = "";
                }
                else
                {
                    this.对话触发 = text.ToUpper().Substring(2, text.Length - 3);
                }
            }
            this.对话超时 = 主程.当前时间.AddSeconds(30.0);
            nPCScript.Call(this, num, text.ToUpper());
        }

        public 物品数据 GetItem(byte grid, byte cell)
        {
            物品数据 v;
            v = null;
            装备数据 v2;
            v2 = null;
            switch (grid)
            {
                default:
                    return null;
                case 7:
                    if (!this.角色资源包.TryGetValue(cell, out v))
                    {
                        return null;
                    }
                    break;
                case 0:
                    if (!this.角色装备.TryGetValue(cell, out v2))
                    {
                        return null;
                    }
                    break;
                case 1:
                    if (!this.角色背包.TryGetValue(cell, out v))
                    {
                        return null;
                    }
                    break;
                case 2:
                    if (!this.角色仓库.TryGetValue(cell, out v))
                    {
                        return null;
                    }
                    break;
            }
            if (v == null && v2 != null)
            {
                v = v2;
            }
            return v;
        }

        public int GetItemValue(物品数据 item, byte type)
        {
            if (item is 装备数据 装备数据)
            {
                switch (type)
                {
                    case 0:
                        return 装备数据.当前持久.V;
                    case 1:
                        return 装备数据.升级次数.V;
                    case 2:
                        return 装备数据.升级攻击.V;
                    case 3:
                        return 装备数据.升级魔法.V;
                    case 4:
                        return 装备数据.升级道术.V;
                    case 5:
                        return 装备数据.升级刺术.V;
                    case 6:
                        return 装备数据.升级弓术.V;
                    case 7:
                        return 装备数据.灵魂绑定.V ? 1 : 0;
                    case 8:
                        return 装备数据.祈祷次数.V;
                    case 9:
                        return 装备数据.幸运等级.V;
                    case 10:
                        return 装备数据.装备神佑.V ? 1 : 0;
                    case 11:
                        return 装备数据.神圣伤害.V;
                    case 12:
                        return 装备数据.圣石数量.V;
                    case 13:
                        return 装备数据.双铭文栏.V ? 1 : 0;
                    case 14:
                        return 装备数据.当前铭栏.V;
                    case 15:
                        return 装备数据.洗练数一.V;
                    case 16:
                        return 装备数据.洗练数二.V;
                    case 17:
                        return 装备数据.物品状态.V;
                    case 18:
                        return 装备数据.随机属性.Count;
                    case 19:
                        if (装备数据.随机属性.Count <= 0)
                        {
                            return 0;
                        }
                        return 装备数据.随机属性[0].属性编号;
                    case 20:
                        if (装备数据.随机属性.Count <= 1)
                        {
                            return 0;
                        }
                        return 装备数据.随机属性[1].属性编号;
                    case 21:
                        if (装备数据.随机属性.Count <= 2)
                        {
                            return 0;
                        }
                        return 装备数据.随机属性[2].属性编号;
                    case 22:
                        if (装备数据.随机属性.Count <= 3)
                        {
                            return 0;
                        }
                        return 装备数据.随机属性[3].属性编号;
                    case 23:
                        return 装备数据.孔洞颜色.Count;
                    case 24:
                        if (装备数据.孔洞颜色.Count <= 0)
                        {
                            return 0;
                        }
                        return (int)装备数据.孔洞颜色[0];
                    case 25:
                        if (装备数据.孔洞颜色.Count <= 1)
                        {
                            return 0;
                        }
                        return (int)装备数据.孔洞颜色[1];
                    case 26:
                        if (装备数据.孔洞颜色.Count <= 2)
                        {
                            return 0;
                        }
                        return (int)装备数据.孔洞颜色[2];
                    case 27:
                        return 装备数据.失败次数.V;
                    case 28:
                        return 装备数据.升级属性.V;
                    case 29:
                        return 装备数据.铸魂次数.V;
                    case 30:
                        return 装备数据.扣除持久.V;
                    case 31:
                        return 装备数据.开启精炼.V;
                    case 32:
                        return 装备数据.精炼值一.V;
                    case 33:
                        return 装备数据.精炼值二.V;
                    case 34:
                        return 装备数据.精炼值三.V;
                    case 35:
                        return 装备数据.精炼次数.V;
                    case 36:
                        return 装备数据.精炼战力;
                    case 37:
                        return 装备数据.装备战力;
                    case 38:
                        return 装备数据.修理费用;
                    case 39:
                        return 装备数据.特修费用;
                    case 40:
                        return 装备数据.装备模板.神圣次数;
                    case 50:
                        return 装备数据.装备模板.物品编号;
                    default:
                        return 0;
                    case 52:
                        return (int)装备数据.装备模板.装备套装;
                }
            }
            if (type == 0)
            {
                return item.当前持久.V;
            }
            return 0;
        }

        public void 玩家合无相石(byte 物品位置, bool 一键合成)
        {
            物品数据 物品数据;
            物品数据 = this.角色背包[物品位置];
            if (物品数据 == null || !合成公式.数据表.TryGetValue((byte)(物品数据.物品编号 - 140000), out var value))
            {
                return;
            }
            int i;
            i = 1;
            if (一键合成)
            {
                for (; this.金币数量 >= value.花费金币 * i && 物品数据.当前持久.V >= value.物品数量一 * i; i++)
                {
                }
                i--;
            }
            if (i != 0 && 物品数据.当前持久.V >= value.物品数量一 * i && this.金币数量 >= value.花费金币 * i)
            {
                this.金币数量 -= (uint)(value.花费金币 * i);
                this.消耗背包物品(value.物品数量一 * i, 物品数据, "合无相石消耗");
                this.玩家获得物品(value.获得物品, i, "合无相石获得");
            }
        }

        public int 玩家武器祈祷(byte 未知参数)
        {
            装备数据 装备数据;
            装备数据 = this.角色装备[0];
            if (装备数据 == null)
            {
                return 0;
            }
            if (装备数据.祈祷次数.V > 3)
            {
                return 0;
            }
            List<物品数据> list;
            list = this.查找背包物品(80003, 166);
            if (list == null)
            {
                return 0;
            }
            this.消耗背包物品(166, list, "武器祈祷消耗");
            装备数据.祈祷次数.V = (byte)(装备数据.祈祷次数.V + 10);
            this.更新物品详情(装备数据);
            return 1;
        }

        public int 玩家装备神佑(byte 装备部位)
        {
            if (this.角色背包.TryGetValue(装备部位, out var v) && v is 装备数据 装备数据)
            {
                if (装备数据.装备神佑.V)
                {
                    return 0;
                }
                if (this.金币数量 < 100000)
                {
                    return 0;
                }
                Dictionary<游戏装备套装, byte> obj;
                obj = new Dictionary<游戏装备套装, byte>
                {
                    [游戏装备套装.祖玛装备] = 1,
                    [游戏装备套装.赤月装备] = 2,
                    [游戏装备套装.魔龙装备] = 4,
                    [游戏装备套装.苍月装备] = 8,
                    [游戏装备套装.星王装备] = 8,
                    [游戏装备套装.神秘装备] = 8,
                    [游戏装备套装.城主装备] = 8
                };
                int num;
                num = (装备数据.装备属性.ContainsKey(游戏对象属性.幸运等级) ? 装备数据.装备属性[游戏对象属性.幸运等级] : 0);
                int num2;
                num2 = obj[装备数据.装备模板.装备套装] + num switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 1,
                    3 => 1,
                    4 => 2,
                    5 => 2,
                    6 => 2,
                    7 => 3,
                    8 => 3,
                    9 => 3,
                    _ => num,
                } + 装备数据.孔洞颜色.Count * 2;
                if (装备数据.升级次数.V > 0)
                {
                    switch (装备数据.装备模板.物品分类)
                    {
                        case 物品使用分类.武器:
                            num2 += 装备数据.升级次数.V switch
                            {
                                0 => 0,
                                1 => 1,
                                2 => 2,
                                3 => 3,
                                4 => 4,
                                5 => 5,
                                6 => 6,
                                7 => 7,
                                8 => 8,
                                9 => 9,
                                _ => 装备数据.升级次数.V,
                            };
                            break;
                        case 物品使用分类.衣服:
                        case 物品使用分类.披风:
                        case 物品使用分类.腰带:
                        case 物品使用分类.鞋子:
                        case 物品使用分类.项链:
                        case 物品使用分类.戒指:
                        case 物品使用分类.手镯:
                        case 物品使用分类.护肩:
                        case 物品使用分类.护腕:
                        case 物品使用分类.头盔:
                        case 物品使用分类.勋章:
                        case 物品使用分类.玉佩:
                        case 物品使用分类.战具:
                            num2 += 装备数据.升级次数.V switch
                            {
                                0 => 0,
                                1 => 1,
                                2 => 1,
                                3 => 1,
                                4 => 2,
                                5 => 2,
                                6 => 2,
                                7 => 3,
                                8 => 3,
                                9 => 3,
                                _ => 装备数据.升级次数.V,
                            };
                            break;
                    }
                }
                主程.添加系统日志($"需要神佑:{num2}");
                num2 = 装备神佑消耗.获取数量($"{(byte)装备数据.装备模板.装备套装}_{装备数据.升级次数.V}_{num}_{装备数据.孔洞颜色.Count}", num2);
                List<物品数据> list;
                list = this.查找背包物品(90312, num2);
                if (list == null)
                {
                    return 0;
                }
                this.金币数量 -= 100000u;
                this.消耗背包物品(num2, list, "装备神佑消耗");
                装备数据.装备神佑.V = true;
                this.更新物品详情(装备数据);
                return 1;
            }
            return 0;
        }

        public int 玩家武器升级()
        {
            if (this.角色装备.TryGetValue(0, out var v))
            {
                武器升级 武器升级;
                武器升级 = 武器升级.数据表.FirstOrDefault((武器升级 o) => o.模板编号 == v.物品编号 && o.升级次数 == v.升级次数.V);
                if (武器升级 == null)
                {
                    return 0;
                }
                if (v.物品状态.V != 1)
                {
                    return 0;
                }
                if (武器升级.需要金币 > this.金币数量)
                {
                    return 0;
                }
                List<物品数据> list;
                list = this.查找背包物品(武器升级.需要物品, 武器升级.需要数量);
                if (list == null)
                {
                    return 0;
                }
                this.扣金币((uint)武器升级.需要金币);
                this.消耗背包物品(武器升级.需要数量, list, "升级武器高级");
                return 1;
            }
            return 0;
        }

        public int 玩家武器铸魂()
        {
            Dictionary<int, byte> dictionary;
            dictionary = new Dictionary<int, byte>
            {
                [111105] = 0,
                [111106] = 1,
                [111107] = 2,
                [111108] = 3,
                [111116] = 4,
                [111306] = 0
            };
            if (this.角色装备.TryGetValue(0, out var v))
            {
                武器升级 武器升级;
                武器升级 = 武器升级.数据表.FirstOrDefault((武器升级 o) => o.模板编号 == v.物品编号 && o.升级次数 == v.升级次数.V + 200);
                if (武器升级 == null)
                {
                    return 0;
                }
                if (v.升级次数.V < 5)
                {
                    return 0;
                }
                if (v.物品状态.V != 1)
                {
                    return 0;
                }
                if (武器升级.需要金币 > this.金币数量)
                {
                    return 0;
                }
                List<物品数据> list;
                list = this.查找背包物品(武器升级.需要物品, 武器升级.需要数量);
                if (list == null)
                {
                    return 0;
                }
                this.金币数量 -= (uint)武器升级.需要金币;
                this.消耗背包物品(武器升级.需要数量, list, "武器铸魂消耗");
                v.物品状态.V = 4;
                v.升级属性.V = ((计算类.计算概率(0.3f) || v.铸魂次数.V >= 3) ? dictionary[武器升级.需要物品] : byte.MaxValue);
                this.更新物品详情(v);
                return 1;
            }
            return 0;
        }

        public int 玩家合成装备(bool 合成勋章, int 合成模板, byte[] 未知参数, byte[] 合成参数)
        {
            if (合成勋章 && 合成参数.Length != 8)
            {
                主程.添加系统日志("玩家合成装备 合成勋章 参数错误");
                return 0;
            }
            if (!合成勋章 && 合成参数.Length != 7)
            {
                主程.添加系统日志("玩家合成装备 合成勋章 参数错误");
                return 0;
            }
            if (装备合成.数据表.TryGetValue(合成模板, out var value))
            {
                if (this.金币数量 < value.需要金币)
                {
                    return 0;
                }
                物品数据[] array;
                array = new 物品数据[合成参数.Length];
                int[] array2;
                array2 = new int[合成参数.Length];
                for (int i = 0; i < 合成参数.Length; i++)
                {
                    array[i] = this.角色背包[合成参数[i]];
                    if (array[i] != null)
                    {
                        int[] 替换物品一;
                        替换物品一 = value.合成材料[i].替换物品一;
                        array2[i] = Math.Max(value.合成材料[i].需要数量, 1);
                        if (array2[i] <= 0 || array[i].当前持久.V >= array2[i])
                        {
                            if (!替换物品一.Contains(array[i].物品编号))
                            {
                                return 0;
                            }
                            continue;
                        }
                        return 0;
                    }
                    return 0;
                }
                for (int j = 0; j < array.Length; j++)
                {
                    this.消耗背包物品((array[j].持久类型 == 物品持久分类.装备) ? array[j].当前持久.V : array2[j], array[j], "合成物品消耗");
                }
                this.金币数量 -= (uint)value.需要金币;
                this.玩家获得物品(value.合成物品, 1, "合成物品获得");
            }
            return 0;
        }

        public int 玩家重铸(int 装备部位, bool v)
        {
            if (Settings.触发装备重铸)
            {
                this.CallDefaultNPC(DefaultNPCType.ItemRestore, false, 装备部位);
                return -1;
            }
            Dictionary<int, float> 表;
            表 = null;
            int 物品编号;
            物品编号 = 0;
            int num;
            num = 0;
            switch (装备部位)
            {
                case 99:
                    物品编号 = 111003;
                    num = 60;
                    表 = 装备重铸.普通技能重铸表;
                    break;
                case 0:
                    物品编号 = 111001;
                    num = 90;
                    表 = 装备重铸.武器重铸表;
                    break;
                case 1:
                    物品编号 = 111001;
                    num = 75;
                    表 = 装备重铸.头盔重铸表;
                    break;
                case 3:
                    物品编号 = 111001;
                    num = 90;
                    表 = 装备重铸.衣服重铸表;
                    break;
                case 8:
                    物品编号 = 111001;
                    num = 60;
                    表 = 装备重铸.项链重铸表;
                    break;
                case 9:
                    物品编号 = 111001;
                    num = 60;
                    表 = 装备重铸.戒指重铸表;
                    break;
                case 10:
                    物品编号 = 111001;
                    num = 75;
                    表 = 装备重铸.手镯重铸表;
                    break;
                case 11:
                    物品编号 = 111022;
                    num = 60;
                    表 = 装备重铸.高级技能重铸表;
                    break;
            }
            List<物品数据> list;
            list = this.查找背包物品(物品编号, num);
            if (list == null)
            {
                return 0;
            }
            this.消耗背包物品(num, list, "重铸系统消耗");
            int num2;
            num2 = 计算类.概率表取值(表);
            if (num2 > 0 && 装备部位 < 11 && 计算类.计算概率(0.5f))
            {
                return 0;
            }
            this.玩家获得物品(num2, 1, "重铸系统获得");
            return num2;
        }

        public void 开启脚本定时器(byte 定时器ID, int 间隔秒数, int 执行次数)
        {
            if (间隔秒数 < 0 || 执行次数 < 0)
            {
                return;
            }
            int num;
            num = 0;
            定时器数据 定时器数据;
            while (true)
            {
                if (num < this.定时器列表.Count)
                {
                    定时器数据 = this.定时器列表[num];
                    if (定时器数据.定时器ID == 定时器ID)
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                定时器数据 = new 定时器数据();
                定时器数据.定时器ID = 定时器ID;
                定时器数据.剩余执行次数 = 执行次数;
                定时器数据.执行间隔秒数 = 间隔秒数;
                定时器数据.下次执行时间 = 主程.当前时间.AddSeconds(间隔秒数);
                this.定时器列表.Add(定时器数据);
                return;
            }
            定时器数据.剩余执行次数 = 执行次数;
            定时器数据.执行间隔秒数 = 间隔秒数;
            定时器数据.下次执行时间 = 主程.当前时间.AddSeconds(间隔秒数);
        }

        public void 关闭脚本定时器(byte 定时器ID)
        {
            int num;
            num = 0;
            while (true)
            {
                if (num < this.定时器列表.Count)
                {
                    if (this.定时器列表[num].定时器ID == 定时器ID)
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                return;
            }
            this.定时器列表.RemoveAt(num);
        }

        public void 脚本穿戴背包装备(string 装备名称, byte 装备槽位)
        {
            if (!游戏物品.检索表.TryGetValue(装备名称, out var value))
            {
                return;
            }
            switch (value.物品分类)
            {
                default:
                    return;
                case 物品使用分类.衣服:
                    if (装备槽位 != 1)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.披风:
                    if (装备槽位 != 2)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.腰带:
                    if (装备槽位 != 6)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.鞋子:
                    if (装备槽位 != 7)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.项链:
                    if (装备槽位 != 8)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.戒指:
                    if (装备槽位 != 9 && 装备槽位 != 10)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.手镯:
                    if (装备槽位 != 11 && 装备槽位 != 12)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.护肩:
                    if (装备槽位 != 4)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.武器:
                    if (装备槽位 != 0)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.护腕:
                    if (装备槽位 != 5)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.头盔:
                    if (装备槽位 != 3)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.勋章:
                    if (装备槽位 != 13)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.玉佩:
                    if (装备槽位 != 14)
                    {
                        return;
                    }
                    break;
                case 物品使用分类.战具:
                    if (装备槽位 != 15)
                    {
                        return;
                    }
                    break;
            }
            int num;
            num = -1;
            物品数据 物品数据;
            物品数据 = null;
            foreach (KeyValuePair<byte, 物品数据> item in this.角色背包)
            {
                if (item.Value != null && item.Value.对应模板.V == value)
                {
                    num = item.Key;
                    物品数据 = item.Value;
                    break;
                }
            }
            if (num != -1 && 物品数据 != null && 物品数据 is 装备数据)
            {
                this.角色背包.Remove((byte)num);
                if (this.角色装备.TryGetValue(装备槽位, out var v))
                {
                    this.角色背包[(byte)num] = v;
                }
                this.角色装备[装备槽位] = (装备数据)物品数据;
                this.网络连接?.发送封包(new 玩家转移物品
                {
                    原有容器 = 1,
                    目标容器 = 0,
                    原有位置 = (byte)num,
                    目标位置 = 装备槽位
                });
                this.玩家穿卸装备((装备穿戴部位)装备槽位, v, (装备数据)物品数据);
            }
        }

        public void 自动挂机状态变更(暂停自动战斗 P)
        {
            if (this.自动挂机 != null)
            {
                this.自动挂机.自动战斗 = P.自动战斗 == 1;
            }
        }

        public void 玩家开始自动挂机(开始自动战斗 p)
        {
            if (!Settings.开启自动战斗)
            {
                this.发送顶部公告("服务器未开启自动挂机功能。");
            }
            else if (!p.自动战斗)
            {
                if (this.自动挂机 != null && this.自动挂机.自动战斗)
                {
                    this.重置挂机参数();
                    this.发送顶部公告((!this.自动挂机.自动战斗) ? "<#T:1000100>" : "<#T:1000101>");
                    this.自动挂机 = null;
                }
            }
            else if (this.本期特权 >= 3 && !(主程.当前时间 > this.特权时间))
            {
                if (this.当前地图.地图编号 == 152)
                {
                    this.发送顶部公告("沙巴克不能挂机");
                    return;
                }
                this.自动挂机 = p;
                this.重置挂机参数();
                this.发送顶部公告(this.自动挂机.自动战斗 ? "<#T:1000100>" : "<#T:1000101>");
            }
            else
            {
                this.发送顶部公告("只有玛法特权才可以使用");
            }
        }

        private void 重置挂机参数()
        {
            开始自动战斗 开始自动战斗;
            开始自动战斗 = this.自动挂机;
            if (开始自动战斗 != null && 开始自动战斗.自动战斗)
            {
                this.挂机_目标_黑名单.Clear();
                this.挂机_目标 = null;
                this.挂机_地图 = this.当前地图.地图编号;
                this.挂机_下一个坐标 = default(Point);
                this.挂机_状态 = 挂机状态.寻路;
                this.挂机_寻路队列 = new Queue<Point>();
                this.挂机_范围 = new Rectangle(this.当前坐标.X - this.自动挂机.战斗范围, this.当前坐标.Y - this.自动挂机.战斗范围, this.自动挂机.战斗范围 * 2, this.自动挂机.战斗范围 * 2);
                this.远攻技能数 = -1;
                this.获取远攻技能数();
                this.下次经验判断 = 主程.当前时间.AddSeconds(this.自动挂机.空闲时间);
                this.挂机已分解物品 = new HashSet<int>();
            }
            else
            {
                this.挂机_结束时间 = default(DateTime);
            }
        }

        private void 开始挂机()
        {
            if (this.自动挂机 == null || !this.自动挂机.自动战斗 || this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || (this.无收益跳转中 && this.无收益跳转延时 != default(DateTime) && 主程.当前时间 < this.无收益跳转延时))
            {
                return;
            }
            if (this.无收益跳转中)
            {
                this.无收益跳转中 = false;
                this.无收益跳转延时 = default(DateTime);
                this.重置挂机参数();
            }
            if (主程.当前时间 < this.行走时间 || 主程.当前时间 < this.奔跑时间 || 主程.当前时间 < this.忙碌时间 || 主程.当前时间 < this.硬直时间 || this.检查状态(游戏对象状态.忙绿状态 | 游戏对象状态.残废状态 | 游戏对象状态.定身状态 | 游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return;
            }
            if (this.自动挂机.开启空闲使用道具 && this.自动挂机.空闲时间 > 0 && this.自动挂机.道具ID > 0 && 主程.当前时间 > this.下次经验判断)
            {
                if (this.当前经验 == this.上次经验值 && this.查找背包物品(this.自动挂机.道具ID, out var 物品))
                {
                    if (物品.触发lua)
                    {
                        if (游戏脚本.玩家使用物品(this, 物品) == 0L)
                        {
                            this.ProcessConsumableItem(物品);
                        }
                    }
                    else if (!this.ProcessConsumableItem(物品))
                    {
                        this.CallDefaultNPC(DefaultNPCType.UseItem, true, 物品.物品编号);
                    }
                    this.无收益跳转中 = true;
                    this.无收益跳转延时 = 主程.当前时间.AddSeconds(2.0);
                }
                this.下次经验判断 = 主程.当前时间.AddSeconds(this.自动挂机.空闲时间);
                this.上次经验值 = this.当前经验;
            }
            if (主程.当前时间 > this.下次分解时间 && (!this.分解完成 || this.最大负重 - this.背包重量 < 5 || (this.自动挂机.开启预留背包 && this.背包剩余 <= this.自动挂机.预留格数) || this.背包剩余 < 5))
            {
                this.挂机自动分解();
                this.下次分解时间 = ((!this.分解完成) ? 主程.当前时间.AddSeconds(3.0) : ((this.背包剩余 == 0) ? 主程.当前时间.AddSeconds(30.0) : 主程.当前时间.AddSeconds(10.0)));
            }
            if (this.挂机_状态 == 挂机状态.寻路)
            {
                return;
            }
            if (this.挂机_状态 != 挂机状态.移动中 && (this.挂机_目标 == null || this.挂机_目标.对象死亡))
            {
                this.挂机_下一个坐标 = default(Point);
                this.挂机_状态 = 挂机状态.寻路;
                this.挂机_目标 = null;
                return;
            }
            if (this.挂机_目标 != null && 主程.当前时间 > this.挂机_目标_超时时间)
            {
                if (this.挂机_目标_黑名单.ContainsKey(this.挂机_目标))
                {
                    this.挂机_目标_黑名单[this.挂机_目标] = 主程.当前时间.AddSeconds(30.0);
                }
                else
                {
                    this.挂机_目标_黑名单.Add(this.挂机_目标, 主程.当前时间.AddSeconds(30.0));
                }
                this.挂机_目标 = null;
                this.挂机_状态 = 挂机状态.寻路;
                return;
            }
            地图对象 obj;
            obj = this.挂机_目标;
            if (obj != null && obj.对象类型 == 游戏对象类型.怪物)
            {
                switch (this.角色职业)
                {
                    case 游戏对象职业.法师:
                    case 游戏对象职业.弓手:
                    case 游戏对象职业.道士:
                        {
                            int num2;
                            num2 = 计算类.网格距离(this.挂机_目标.当前坐标, this.当前坐标);
                            if (num2 > 5)
                            {
                                if (this.挂机_状态 == 挂机状态.战斗中)
                                {
                                    this.挂机_下一个坐标 = default(Point);
                                    this.挂机_状态 = 挂机状态.寻路;
                                    return;
                                }
                            }
                            else if (this.获取远攻技能数() >= 1)
                            {
                                if (this.当前魔力 > 10 || num2 == 1)
                                {
                                    if (this.挂机_状态 == 挂机状态.移动中)
                                    {
                                        this.网络连接.发送封包(new 对象角色停止
                                        {
                                            对象编号 = this.地图编号,
                                            对象坐标 = ((this.挂机_下一个坐标 != default(Point)) ? this.挂机_下一个坐标 : this.当前坐标),
                                            对象高度 = this.当前高度
                                        });
                                    }
                                    this.挂机_状态 = 挂机状态.战斗中;
                                }
                            }
                            else
                            {
                                this.挂机_状态 = ((num2 > 1) ? 挂机状态.移动中 : 挂机状态.战斗中);
                            }
                            break;
                        }
                    case 游戏对象职业.战士:
                    case 游戏对象职业.刺客:
                    case 游戏对象职业.龙枪:
                        {
                            int num;
                            num = 计算类.网格距离(this.挂机_目标.当前坐标, this.当前坐标);
                            if (this.挂机_状态 == 挂机状态.战斗中)
                            {
                                if (num > 1)
                                {
                                    this.挂机_下一个坐标 = default(Point);
                                    this.挂机_状态 = 挂机状态.寻路;
                                    return;
                                }
                            }
                            else if (num == 0 || (this.挂机_状态 == 挂机状态.移动中 && num <= 1))
                            {
                                this.网络连接?.发送封包(new 对象角色停止
                                {
                                    对象编号 = this.地图编号,
                                    对象坐标 = ((this.挂机_下一个坐标 != default(Point)) ? this.挂机_下一个坐标 : this.当前坐标),
                                    对象高度 = this.当前高度
                                });
                                this.挂机_状态 = 挂机状态.战斗中;
                            }
                            break;
                        }
                }
            }
            else
            {
                地图对象 obj2;
                obj2 = this.挂机_目标;
                if (obj2 != null && obj2.对象类型 == 游戏对象类型.物品)
                {
                    if ((this.背包剩余 <= 0 || !base.邻居列表.Contains(this.挂机_目标) || this.背包重量 >= this[游戏对象属性.最大负重]) && !this.挂机_目标.IsMoney())
                    {
                        this.挂机_下一个坐标 = default(Point);
                        this.挂机_状态 = 挂机状态.寻路;
                        this.挂机_目标 = null;
                        return;
                    }
                    if (this.挂机_目标.当前坐标 == this.当前坐标)
                    {
                        if (!base.邻居列表.Contains(this.挂机_目标))
                        {
                            this.挂机_下一个坐标 = default(Point);
                            this.挂机_状态 = 挂机状态.寻路;
                            this.挂机_目标 = null;
                        }
                        return;
                    }
                    if (this.当前地图.空间阻塞(this.挂机_目标.当前坐标))
                    {
                        this.挂机_目标 = null;
                        this.挂机_状态 = 挂机状态.寻路;
                        return;
                    }
                    this.挂机_状态 = 挂机状态.移动中;
                }
            }
            switch (this.挂机_状态)
            {
                case 挂机状态.寻路:
                    if (this.挂机_寻路队列.Count > 0)
                    {
                        this.挂机_状态 = 挂机状态.移动中;
                    }
                    break;
                case 挂机状态.移动中:
                    {
                        if (this.挂机_下一个坐标 == default(Point))
                        {
                            if (this.挂机_寻路队列.Count == 0)
                            {
                                this.挂机_下一个坐标 = default(Point);
                                this.挂机_状态 = 挂机状态.寻路;
                                break;
                            }
                            this.挂机_下一个坐标 = this.挂机_寻路队列.Dequeue();
                        }
                        if (计算类.网格距离(this.挂机_下一个坐标, this.当前坐标) >= 4)
                        {
                            this.挂机_下一个坐标 = default(Point);
                            this.挂机_状态 = 挂机状态.寻路;
                            break;
                        }
                        if (!this.当前地图.能否通行(this.挂机_下一个坐标) && this.挂机_下一个坐标 != this.当前坐标)
                        {
                            this.挂机_下一个坐标 = default(Point);
                            this.挂机_状态 = 挂机状态.寻路;
                            break;
                        }
                        Point point;
                        point = ((this.挂机_寻路队列.Count > 0) ? this.挂机_寻路队列.Peek() : default(Point));
                        if (point != default(Point) && this.当前地图.能否通行(point))
                        {
                            int num3;
                            num3 = this.当前坐标.X - this.挂机_下一个坐标.X;
                            int num4;
                            num4 = this.当前坐标.Y - this.挂机_下一个坐标.Y;
                            int num5;
                            num5 = this.挂机_下一个坐标.X - point.X;
                            int num6;
                            num6 = this.挂机_下一个坐标.Y - point.Y;
                            if (num3 == num5 && num4 == num6)
                            {
                                if (this.挂机_寻路队列.Count > 0)
                                {
                                    this.挂机_寻路队列.Dequeue();
                                }
                                this.挂机_下一个坐标 = point;
                                this.玩家角色跑动(this.挂机_下一个坐标);
                                this.奔跑时间 = this.奔跑时间.AddMilliseconds(150.0);
                                this.挂机_下一个坐标 = default(Point);
                                break;
                            }
                        }
                        this.玩家角色走动(this.挂机_下一个坐标);
                        this.行走时间 = this.行走时间.AddMilliseconds(100.0);
                        this.挂机_下一个坐标 = default(Point);
                        break;
                    }
                case 挂机状态.战斗中:
                    if (!(主程.当前时间 < this.攻击间隔))
                    {
                        switch (this.角色职业)
                        {
                            case 游戏对象职业.战士:
                                this.战士挂机();
                                break;
                            case 游戏对象职业.法师:
                                this.法师挂机();
                                break;
                            case 游戏对象职业.刺客:
                                this.刺客挂机();
                                break;
                            case 游戏对象职业.弓手:
                                this.弓手挂机();
                                break;
                            case 游戏对象职业.道士:
                                this.道士挂机();
                                break;
                            case 游戏对象职业.龙枪:
                                this.龙枪挂机();
                                break;
                        }
                    }
                    break;
            }
        }

        private bool 开关技能(技能数据 技能)
        {
            if (技能.铭文模板.开关技能列表 != null)
            {
                foreach (string item in 技能.铭文模板.开关技能列表)
                {
                    if (!游戏技能.数据表.TryGetValue(item, out var value))
                    {
                        continue;
                    }
                    if (this.冷却记录.TryGetValue(value.自身技能编号 | 0x1000000, out var v) && 主程.当前时间 < v)
                    {
                        return false;
                    }
                    foreach (KeyValuePair<int, 技能任务> item2 in value.节点列表)
                    {
                        if (item2.Value is B_00_技能切换通知 b_00_技能切换通知 && this.冷却记录.TryGetValue(b_00_技能切换通知.技能标记编号 | 0x1000000, out v) && 主程.当前时间 < v)
                        {
                            return false;
                        }
                    }
                    if (this.主体技能表.TryGetValue(value.绑定等级编号, out var v2) && value.需要消耗魔法?.Length > v2.技能等级.V && this.当前魔力 < value.需要消耗魔法[v2.技能等级.V] && this is 玩家实例 { 无敌模式: false })
                    {
                        return false;
                    }
                    this.玩家开关技能(value.自身技能编号);
                    return true;
                }
            }
            return true;
        }

        public int 获取远攻技能数()
        {
            if (this.远攻技能数 != -1)
            {
                return this.远攻技能数;
            }
            switch (this.角色职业)
            {
                case 游戏对象职业.法师:
                    this.远攻技能数 = this.主体技能表.Keys.ToList().Count((ushort k) => k == 2559 || k == 2540 || k == 2537 || k == 2533 || k == 2531);
                    break;
                case 游戏对象职业.弓手:
                    this.远攻技能数 = this.主体技能表.Count - 1;
                    break;
                case 游戏对象职业.道士:
                    this.远攻技能数 = this.主体技能表.Keys.ToList().Count((ushort k) => k == 3005 || k == 3010);
                    break;
            }
            return this.远攻技能数;
        }

        private void 刺客挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.刺客])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> list;
                list = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.献祭:
                        if (!list.Any((ushort b) => b == 15450 || b == 15452 || b == 15454) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒暗影守卫:
                        if (!list.Any((ushort b) => b == 15460 || b == 15461 || b == 15462 || b == 15463) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒猎命宣告:
                        if (!list.Any((ushort b) => b == 15430 || b == 15400 || b == 15442 || b == 15480) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.火镰狂舞:
                        if (list.Contains(15390) && this.玩家释放技能2(1932, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.刺客普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.暴击之术:
                        if (list.Contains(15310) && this.玩家释放技能2(1930, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.致残毒药:
                        if (!list.Any((ushort b) => b == 15330 || b == 15332 || b == 15334 || b == 15335 || b == 15338 || b == 15300) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.炎龙啸波:
                        if (list.Contains(15350) && this.玩家释放技能2(1931, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 战士挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.战士])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> list;
                list = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.爆炎剑诀:
                        if (!list.Contains(10420))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1435, out v) && this.玩家释放技能2(1435, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.神威盾甲:
                        if (!list.Any((ushort b) => b == 10460 || b == 10461 || b == 10462 || b == 10463) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒金钟罩:
                        if (!list.Any((ushort b) => b == 10470 || b == 10471 || b == 10472 || b == 10473) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒雷霆剑法:
                        if (!list.Contains(10490))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1437, out v) && this.玩家释放技能2(1437, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒天威剑法:
                        if (!list.Contains(10500))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1450, out v) && this.玩家释放技能2(1450, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.战士普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.刺杀剑术:
                        if (!list.Contains(10330))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1431, out v) && this.玩家释放技能2(1431, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.半月弯刀:
                        if (base.邻居列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 3) < 2)
                        {
                            if (list.Contains(10340))
                            {
                                this.开关技能(v);
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(10340))
                        {
                            if (this.玩家释放技能2(1432, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                        }
                        else if (this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.烈火剑法:
                        if (!list.Contains(10360))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1433, out v) && this.玩家释放技能2(1433, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.逐日剑法:
                        if (!list.Contains(10380))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1434, out v) && this.玩家释放技能2(1434, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 龙枪挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.龙枪])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> list;
                list = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.龙枪普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.横扫六合:
                        if ((list.Any((ushort b) => b == 12140 || b == 12141 || b == 12142 || b == 12143) && base.邻居列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 3) < 2) || !this.被动技能.TryGetValue(1600, out v) || !this.被动技能.TryGetValue(1601, out v))
                        {
                            break;
                        }
                        if (!list.Any((ushort b) => b == 12030 || b == 12031))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(12031))
                        {
                            if (this.玩家释放技能2(1601, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                        }
                        else if (this.玩家释放技能2(1600, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.狂飙突刺:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.神威镇域:
                        if (!list.Any((ushort b) => b == 12070 || b == 12071 || b == 12072 || b == 12073) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.枪出如龙:
                        if (!list.Any((ushort b) => b == 12080 || b == 12082 || b == 12150))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1602, out v) && this.玩家释放技能2(1602, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.御龙晶甲:
                        if (!list.Any((ushort b) => b == 12090 || b == 12091 || b == 12092 || b == 12093) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.凌云枪法:
                        if (!list.Any((ushort b) => b == 12100 || b == 12101))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(12060) && this.被动技能.TryGetValue(1603, out v))
                        {
                            if (this.玩家释放技能2(1603, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(12061) && !this.被动技能.TryGetValue(1604, out v) && this.玩家释放技能2(1604, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.盘龙枪势:
                        if (!list.Any((ushort b) => b == 12130 || b == 12131 || b == 12132 || b == 12133) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.百战军魂:
                        if (!list.Any((ushort b) => b == 12140 || b == 12141 || b == 12142 || b == 12143) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.龙啸千里:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 弓手挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.弓手])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> source;
                source = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.弓手普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.基础射击:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.战术标记:
                        if (!this.挂机_目标.Buff列表.Keys.ToList().Any((ushort b) => b == 20440 || b == 20441 || b == 20442 || b == 20443) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.三发散射:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.强袭之箭:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒羿神庇佑:
                        if (!source.Any((ushort b) => b == 20490 || b == 20492 || b == 20494 || b == 20496) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.穿刺射击:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.守护箭羽:
                        if (!source.Any((ushort b) => b == 20520 || b == 20521 || b == 20522) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.回避射击:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒万箭穿心:
                        if (!source.Any((ushort b) => b == 20520 || b == 20521 || b == 20522) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 法师挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.法师])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> source;
                source = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.觉醒魔能星陨:
                        {
                            地图对象 obj3;
                            obj3 = this.挂机_目标;
                            if ((obj3 == null || !(obj3.邻居列表?.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 5) < 2)) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                            break;
                        }
                    case 游戏基础技能.觉醒法神奥义:
                        if (!source.Any((ushort b) => b == 25560 || b == 25564 || b == 25570 || b == 25577) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.法师普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.当前魔力 <= 30 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.小火球术:
                        if (!this.主体技能表.ContainsKey(2533) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.雷电之术:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.魔法护盾:
                        if (!source.Any((ushort b) => b == 25350 || b == 25352 || b == 25354 || b == 25356) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.疾光电影:
                        {
                            if (this.主体技能表.TryGetValue(2536, out var v3) && v3.铭文编号 == 3)
                            {
                                地图对象 obj;
                                obj = this.挂机_目标;
                                if ((obj == null || !(obj.邻居列表?.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 5) < 2)) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                                {
                                    this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                    return;
                                }
                            }
                            break;
                        }
                    case 游戏基础技能.冰咆哮:
                        {
                            地图对象 obj2;
                            obj2 = this.挂机_目标;
                            if ((obj2 == null || !(obj2.邻居列表?.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 5) < 2)) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                            break;
                        }
                    case 游戏基础技能.流星火雨:
                        {
                            if (this.主体技能表.TryGetValue(2540, out var _) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                            break;
                        }
                }
            }
        }

        private void 道士挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.道士])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> source;
                source = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.觉醒阴阳道盾:
                        if (!source.Any((ushort b) => b == 30250 || b == 30252 || b == 30254 || b == 30256) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.道士普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.施毒术:
                        {
                            List<ushort> source2;
                            source2 = this.挂机_目标.Buff列表.Keys.ToList();
                            if (this.挂机_目标 == null || source2.Any((ushort b) => b == 30041 || b == 30045 || b == 30047 || b == 34002) || !this.玩家释放技能2(3004, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                if (this.挂机_目标 != null && !source2.Any((ushort b) => b == 30040 || b == 30043 || b == 30048 || b == 34002) && this.玩家释放技能2(3400, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                                {
                                    this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                    return;
                                }
                                break;
                            }
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                    case 游戏基础技能.灵魂火符:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间 + 200);
                            return;
                        }
                        break;
                    case 游戏基础技能.幽灵之盾:
                        if (!source.Any((ushort b) => b == 30060 || b == 30061 || b == 30062 || b == 30063 || b == 30064) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.神圣战甲:
                        if (!source.Any((ushort b) => b == 30070 || b == 30071 || b == 30072 || b == 30073) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.召唤骷髅:
                    case 游戏基础技能.召唤神兽:
                    case 游戏基础技能.觉醒召唤月灵:
                        if (this.宠物列表.Count < ((!this.主体技能表.ContainsKey(3022)) ? 1 : 2) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.隐身之术:
                        if (!source.Any((ushort b) => b == 30090 || b == 30091) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.噬血术:
                        if (this.主体技能表.ContainsKey(3010) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间 + 200);
                            return;
                        }
                        break;
                    case 游戏基础技能.无极真气:
                        if (!source.Any((ushort b) => b == 30150 || b == 30152 || b == 30153 || b == 30154) && this.玩家释放技能2((ushort)item, base.动作编号++, this.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 挂机自动分解()
        {
            this.分解完成 = false;
            int num;
            num = 0;
            foreach (KeyValuePair<byte, 物品数据> item in this.角色背包.ToList())
            {
                byte key;
                key = item.Key;
                物品数据 value;
                value = item.Value;
                if (!value.是否上锁 && 物品分解.数据表.ContainsKey(value.物品编号) && (!(value is 装备数据 装备数据) || 装备数据.随机属性.Count <= 0))
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

        public List<物品数据> 查找背包物品(HashSet<int> 物品编号)
        {
            return (from v in this.角色背包.Values.ToList()
                    where 物品编号.Contains(v.物品编号)
                    select v).ToList();
        }

        private List<byte> 查找资源背包空格()
        {
            List<byte> list;
            list = new List<byte>();
            for (byte b = 0; b < this.资源包大小; b++)
            {
                if (!this.角色资源包.ContainsKey(b))
                {
                    list.Add(b);
                }
            }
            return list;
        }

        private List<byte> 查找仓库空格()
        {
            List<byte> list;
            list = new List<byte>();
            for (byte b = 0; b < this.仓库大小; b++)
            {
                if (!this.角色仓库.ContainsKey(b))
                {
                    list.Add(b);
                }
            }
            return list;
        }

        public void SendCustomMessage(int msgId, byte[] body)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((ushort)0);
            binaryWriter.Write(msgId);
            binaryWriter.Write(body);
            base.发送封包(new 更多资金信息
            {
                字节数据 = memoryStream.ToArray()
            });
        }
    }
}
