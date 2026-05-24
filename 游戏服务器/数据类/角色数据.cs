using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using 游戏服务器.地图类;
using 游戏服务器.模板类;
using 游戏服务器.网络类;

namespace 游戏服务器.数据类
{
    [数据快速检索(检索字段 = "角色名字")]
    public sealed class 角色数据 : 游戏数据
    {
        public readonly 数据监视器<string> 角色名字;

        public readonly 数据监视器<string> 网络地址;

        public readonly 数据监视器<string> 物理地址;

        public readonly 数据监视器<DateTime> 创建日期;

        public readonly 数据监视器<DateTime> 登录日期;

        public readonly 数据监视器<DateTime> 冻结日期;

        public readonly 数据监视器<DateTime> 删除日期;

        public readonly 数据监视器<DateTime> 离线日期;

        public readonly 数据监视器<DateTime> 监禁日期;

        public readonly 数据监视器<DateTime> 封禁日期;

        public readonly 数据监视器<TimeSpan> 灰名时间;

        public readonly 数据监视器<TimeSpan> 减PK时间;

        public readonly 数据监视器<DateTime> 武斗日期;

        public readonly 数据监视器<DateTime> 攻沙日期;

        public readonly 数据监视器<DateTime> 领奖日期;

        public readonly 数据监视器<DateTime> 屠魔大厅;

        public readonly 数据监视器<DateTime> 屠魔兑换;

        public readonly 数据监视器<int> 屠魔次数;

        public readonly 数据监视器<DateTime> 分解日期;

        public readonly 数据监视器<int> 分解经验;

        public readonly 数据监视器<游戏对象职业> 角色职业;

        public readonly 数据监视器<游戏对象性别> 角色性别;

        public readonly 数据监视器<对象发型分类> 角色发型;

        public readonly 数据监视器<对象发色分类> 角色发色;

        public readonly 数据监视器<对象脸型分类> 角色脸型;

        public readonly 数据监视器<int> 当前血量;

        public readonly 数据监视器<int> 当前蓝量;

        public readonly 数据监视器<byte> 当前等级;

        public readonly 数据监视器<long> 当前经验;

        public readonly 数据监视器<int> 双倍经验;

        public readonly 数据监视器<int> 当前战力;

        public readonly 数据监视器<int> 当前PK值;

        public readonly 数据监视器<int> 当前地图;

        public readonly 数据监视器<int> 重生地图;

        public readonly 数据监视器<Point> 当前坐标;

        public readonly 数据监视器<游戏方向> 当前朝向;

        public readonly 数据监视器<攻击模式> 攻击模式;

        public readonly 数据监视器<宠物模式> 宠物模式;

        public readonly 哈希监视器<宠物数据> 宠物数据;

        public readonly 数据监视器<byte> 背包大小;

        public readonly 数据监视器<byte> 仓库大小;

        public readonly 数据监视器<byte> 资源包大小;

        public readonly 数据监视器<long> 消耗元宝;

        public readonly 数据监视器<long> 转出金币;

        public readonly 列表监视器<uint> 玩家设置;

        public readonly 数据监视器<装备数据> 升级装备;

        public readonly 数据监视器<DateTime> 取回时间;

        public readonly 数据监视器<bool> 升级成功;

        public readonly 数据监视器<byte> 当前称号;

        public readonly 字典监视器<byte, int> 历史排名;

        public readonly 字典监视器<byte, int> 当前排名;

        public readonly 字典监视器<byte, DateTime> 称号列表;

        public readonly 字典监视器<游戏货币, uint> 角色货币;

        public readonly 字典监视器<byte, 物品数据> 角色背包;

        public readonly 字典监视器<byte, 物品数据> 角色仓库;

        public readonly 字典监视器<byte, 物品数据> 角色资源包;

        public readonly 字典监视器<byte, 装备数据> 角色装备;

        public readonly 字典监视器<byte, 技能数据> 快捷栏位;

        public readonly 字典监视器<ushort, Buff数据> Buff数据;

        public readonly 字典监视器<ushort, 技能数据> 技能数据;

        public readonly 字典监视器<int, DateTime> 冷却数据;

        public readonly 字典监视器<ushort, AchievementData> Achievements;

        public readonly 字典监视器<byte, int> AchievementVariables;

        public readonly 哈希监视器<邮件数据> 角色邮件;

        public readonly 哈希监视器<邮件数据> 未读邮件;

        public readonly 数据监视器<byte> 预定特权;

        public readonly 数据监视器<byte> 本期特权;

        public readonly 数据监视器<byte> 上期特权;

        public readonly 数据监视器<uint> 本期记录;

        public readonly 数据监视器<uint> 上期记录;

        public readonly 数据监视器<DateTime> 本期日期;

        public readonly 数据监视器<DateTime> 上期日期;

        public readonly 数据监视器<DateTime> 补给日期;

        public readonly 数据监视器<DateTime> 战备日期;

        public readonly 字典监视器<byte, int> 剩余特权;

        public readonly 数据监视器<账号数据> 所属账号;

        public readonly 数据监视器<队伍数据> 所属队伍;

        public readonly 数据监视器<行会数据> 所属行会;

        public readonly 数据监视器<师门数据> 所属师门;

        public readonly 哈希监视器<角色数据> 好友列表;

        public readonly 哈希监视器<角色数据> 偶像列表;

        public readonly 哈希监视器<角色数据> 粉丝列表;

        public readonly 哈希监视器<角色数据> 仇人列表;

        public readonly 哈希监视器<角色数据> 仇恨列表;

        public readonly 哈希监视器<角色数据> 黑名单表;

        public readonly 数据监视器<byte> 当前坐骑;

        public readonly 列表监视器<byte> 坐骑列表;

        public readonly 哈希监视器<龙卫数据> 龙卫属性;

        public readonly 哈希监视器<龙卫数据> 龙卫属性一;

        public readonly 哈希监视器<龙卫数据> 龙卫属性二;

        public readonly 哈希监视器<龙卫数据> 龙卫属性三;

        public readonly 哈希监视器<龙卫数据> 龙卫属性四;

        public readonly 哈希监视器<龙卫数据> 龙卫属性五;

        public readonly 数据监视器<string> 龙卫记录一;

        public readonly 数据监视器<string> 龙卫记录二;

        public readonly 数据监视器<string> 龙卫记录三;

        public readonly 数据监视器<string> 龙卫记录四;

        public readonly 数据监视器<string> 龙卫记录五;

        public readonly 数据监视器<long> 当前觉醒经验;

        public readonly 数据监视器<bool> 管理员角色;

        public readonly 数据监视器<bool> 商人角色;

        public readonly 哈希监视器<CharacterQuest> Quests;

        public readonly 字典监视器<byte, int> 御兽列表;

        public readonly 数据监视器<物品数据> 挂载物品;

        public readonly 字典监视器<byte, int> 天赋等级;

        public readonly 字典监视器<byte, int> 天赋经验;

        public readonly 字典监视器<byte, int> 天赋刻印;

        public readonly 字典监视器<int, int> 脚本变量;

        public readonly 字典监视器<int, int> 脚本数字;

        public readonly 字典监视器<int, string> 脚本字符;

        public readonly 字典监视器<int, int> 零点数字;

        public readonly 字典监视器<int, bool> 任务标识;

        public readonly 数据监视器<int> 传奇之力;

        public readonly 数据监视器<DateTime> 武技日期;

        public readonly 数据监视器<byte> 传永武技;

        public readonly 数据监视器<DateTime> 签到日期;

        public readonly 数据监视器<byte> 每日签到;

        public readonly 数据监视器<int> 狩猎编号;

        public readonly 数据监视器<int> 狩猎金币;

        public readonly 数据监视器<int> 已接狩猎;

        public readonly 数据监视器<DateTime> 狩猎完成;

        public readonly 数据监视器<DateTime> 狩猎刷新;

        public readonly 字典监视器<ushort, ushort> 节点数据;

        public readonly 列表监视器<int> 日常悬赏;

        public readonly 数据监视器<int> 日常悬赏完成次数;

        public readonly 数据监视器<int> 日常悬赏刷新次数;

        public readonly 数据监视器<DateTime> 日常悬赏任务刷新;

        public readonly 数据监视器<DateTime> 日常悬赏计次刷新;

        public readonly 列表监视器<int> 周常悬赏;

        public readonly 数据监视器<int> 周常悬赏完成次数;

        public readonly 数据监视器<int> 周常悬赏刷新次数;

        public readonly 数据监视器<DateTime> 周常悬赏任务刷新;

        public readonly 数据监视器<DateTime> 周常悬赏计次刷新;

        public readonly 数据监视器<bool> 龙卫觉醒;

        public readonly 字典监视器<int, int> 角色变量;

        public readonly 数据监视器<ushort> 日程进度;

        public readonly 数据监视器<byte> 日程奖励;

        public readonly 字典监视器<byte, int> 威望进度;

        public readonly 数据监视器<byte> 紧急任务;

        public readonly 数据监视器<bool> 锁定仓库;

        public readonly 数据监视器<string> 动态密码;

        public readonly 数据监视器<int> 七天积分;

        public readonly 数据监视器<long> 七天领取;

        public readonly 字典监视器<byte, int> 七天进度;

        public readonly 数据监视器<bool> 开启战令;

        public readonly 数据监视器<ushort> 战功进度;

        public readonly 数据监视器<ushort> 购买战功;

        public readonly 数据监视器<ushort> 战功奖励;

        public readonly 数据监视器<ushort> 军机奖励;

        public readonly 数据监视器<ushort> 战功次数;

        public readonly 字典监视器<ushort, ushort> 战功任务;

        public readonly 字典监视器<ushort, ushort> 找回奖励;

        public readonly 字典监视器<ushort, ushort> 杀怪成就;

        public readonly 字典监视器<ushort, byte> 杀怪领取;

        public readonly 列表监视器<int> 主题礼包_星期日_购买数据;

        public readonly 列表监视器<int> 主题礼包_星期六_购买数据;

        public readonly 列表监视器<int> 主题礼包_星期一_购买数据;

        public readonly 列表监视器<int> 主题礼包_星期二_购买数据;

        public readonly 列表监视器<int> 主题礼包_星期三_购买数据;

        public readonly 列表监视器<int> 主题礼包_星期四_购买数据;

        public readonly 列表监视器<int> 主题礼包_星期五_购买数据;

        public readonly 数据监视器<int> 累计充值;

        public readonly 数据监视器<int> 今日充值;

        public readonly 数据监视器<int> 脚本爆率;

        public readonly 字典监视器<byte, int> 累计充值奖励已领取;

        public readonly 数据监视器<DateTime> 禁言日期;

        public int 角色编号 => base.数据索引.V;

        public long 角色经验
        {
            get
            {
                return this.当前经验.V;
            }
            set
            {
                this.当前经验.V = value;
            }
        }

        public byte 角色等级
        {
            get
            {
                return this.当前等级.V;
            }
            set
            {
                if (this.当前等级.V != value)
                {
                    this.当前等级.V = value;
                    系统数据.数据.更新等级(this);
                }
            }
        }

        public int 角色战力
        {
            get
            {
                return this.当前战力.V;
            }
            set
            {
                if (this.当前战力.V != value)
                {
                    this.当前战力.V = value;
                    系统数据.数据.更新战力(this);
                }
            }
        }

        public int 角色PK值
        {
            get
            {
                return this.当前PK值.V;
            }
            set
            {
                if (this.当前PK值.V != value)
                {
                    this.当前PK值.V = value;
                    系统数据.数据.更新PK值(this);
                }
            }
        }

        public long 所需经验 => 角色成长.升级所需经验[this.角色等级];

        public uint 元宝数量
        {
            get
            {
                if (this.所属账号 == null)
                {
                    return 0u;
                }
                return this.所属账号.V.元宝数量.V;
            }
            set
            {
                if (this.所属账号 != null)
                {
                    this.所属账号.V.元宝数量.V = value;
                }
            }
        }

        public uint 银币数量
        {
            get
            {
                if (!this.角色货币.TryGetValue(游戏货币.银币, out var v))
                {
                    return 0u;
                }
                return v;
            }
            set
            {
                this.角色货币[游戏货币.银币] = value;
                主窗口.更新角色数据(this, "银币数量", value);
            }
        }

        public uint 金币数量
        {
            get
            {
                if (!this.角色货币.TryGetValue(游戏货币.金币, out var v))
                {
                    return 0u;
                }
                return v;
            }
            set
            {
                this.角色货币[游戏货币.金币] = value;
                主窗口.更新角色数据(this, "金币数量", value);
            }
        }

        public uint 师门声望
        {
            get
            {
                if (!this.角色货币.TryGetValue(游戏货币.名师声望, out var v))
                {
                    return 0u;
                }
                return v;
            }
            set
            {
                this.角色货币[游戏货币.名师声望] = value;
                系统数据.数据.更新声望(this);
                主窗口.更新角色数据(this, "师门声望", value);
            }
        }

        public byte 师门参数
        {
            get
            {
                if (this.当前师门 != null)
                {
                    if (this.当前师门.师父编号 == this.角色编号)
                    {
                        return 2;
                    }
                    return 1;
                }
                if (this.角色等级 < 30)
                {
                    return 0;
                }
                return 2;
            }
        }

        public 队伍数据 当前队伍
        {
            get
            {
                return this.所属队伍.V;
            }
            set
            {
                if (this.所属队伍.V != value)
                {
                    this.所属队伍.V = value;
                }
            }
        }

        public 师门数据 当前师门
        {
            get
            {
                return this.所属师门.V;
            }
            set
            {
                if (this.所属师门.V != value)
                {
                    this.所属师门.V = value;
                }
            }
        }

        public 行会数据 当前行会
        {
            get
            {
                return this.所属行会.V;
            }
            set
            {
                if (this.所属行会.V != value)
                {
                    if ((主程.当前时间.Date - this.创建日期.V.Date).TotalDays < 7.0)
                    {
                        this.七天进度[47] = this.七天进度[47] + 1;
                    }
                    this.所属行会.V = value;
                }
            }
        }

        public long 觉醒经验
        {
            get
            {
                return this.当前觉醒经验.V;
            }
            set
            {
                this.当前觉醒经验.V = value;
            }
        }

        public int 传奇之力等级
        {
            get
            {
                return this.传奇之力.V;
            }
            set
            {
                if (this.传奇之力.V != value)
                {
                    this.传奇之力.V = value;
                    this.网络连接?.发送封包(new 同步传奇之力
                    {
                        传奇之力 = value,
                        对象编号 = this.角色编号
                    });
                }
            }
        }

        public 客户网络 网络连接 { get; set; }

        public void 获得经验(int 经验值)
        {
            if ((this.角色等级 < Settings.游戏开放等级 || this.角色经验 < this.所需经验) && (this.角色经验 += 经验值) > this.所需经验 && this.角色等级 < Settings.游戏开放等级)
            {
                while (this.角色经验 >= this.所需经验)
                {
                    this.角色经验 -= this.所需经验;
                    this.角色等级++;
                }
            }
        }

        public void 角色下线()
        {
            if (this.网络连接 != null)
            {
                this.网络连接.绑定角色 = null;
            }
            this.网络连接 = null;
            网络服务网关.已上线连接数--;
            this.离线日期.V = 主程.当前时间;
            主窗口.更新角色数据(this, "离线日期", this.离线日期);
        }

        public void 角色上线(客户网络 网络)
        {
            if (this.网络连接 != null && this.网络连接 != 网络)
            {
                this.网络连接.绑定角色?.玩家角色下线();
                this.网络连接.尝试断开连接(new Exception("角色被新连接接管, 旧连接被踢"));
            }
            this.网络连接 = 网络;
            网络服务网关.已上线连接数++;
            this.物理地址.V = 网络?.物理地址;
            this.网络地址.V = 网络?.网络地址;
            this.离线日期.V = default(DateTime);
            主窗口.更新角色数据(this, "离线日期", null);
            if (网络 != null)
            {
                主程.添加系统日志($"玩家[{this.角色名字}][{this.当前等级}级]进入了游戏  {((IPEndPoint)(网络?.当前连接?.Client?.RemoteEndPoint))?.Address} {this.物理地址.V}");
            }
        }

        public void 发送邮件(邮件数据 邮件)
        {
            邮件.收件地址.V = this;
            this.角色邮件.Add(邮件);
            this.未读邮件.Add(邮件);
            this.网络连接?.发送封包(new 未读邮件提醒
            {
                邮件数量 = this.未读邮件.Count
            });
        }

        public void 发送邮件(角色数据 作者, string 标题, string 正文, 物品数据 附件)
        {
            this.发送邮件(new 邮件数据(作者, 标题, 正文, 附件));
        }

        public void 发送邮件(角色数据 作者, string 标题, string 正文, int 附件, int 数量 = 1, bool 是否绑定 = false)
        {
            物品数据 附件2;
            附件2 = null;
            if (游戏物品.数据表.TryGetValue(附件, out var value))
            {
                附件2 = ((!(value is 游戏装备 模板)) ? ((value.持久类型 == 物品持久分类.容器) ? new 物品数据(value, this, 0, 0, 0, 是否绑定) : ((value.持久类型 != 物品持久分类.堆叠) ? new 物品数据(value, this, 0, 0, value.物品持久, 是否绑定) : new 物品数据(value, this, 0, 0, 数量, 是否绑定))) : new 装备数据(模板, this, 0, 0, 随机生成: true, 是否绑定));
            }
            this.发送邮件(new 邮件数据(作者, 标题, 正文, 附件2));
        }

        public bool 角色在线(out 客户网络 网络)
        {
            网络 = this.网络连接;
            return 网络 != null;
        }

        public bool 尝试获取背包空余格子(out byte location)
        {
            byte b;
            b = 0;
            while (true)
            {
                if (b < this.背包大小.V)
                {
                    if (!this.角色背包.ContainsKey(b))
                    {
                        break;
                    }
                    b++;
                    continue;
                }
                location = byte.MaxValue;
                return false;
            }
            location = b;
            return true;
        }

        public bool 尝试获取背包空余格子(byte count, out byte[] location)
        {
            List<byte> list;
            list = new List<byte>();
            byte b;
            b = 0;
            while (b < this.背包大小.V && count > list.Count)
            {
                if (!this.角色背包.ContainsKey(b))
                {
                    list.Add(b);
                }
                b++;
            }
            location = list.ToArray();
            return list.Count == count;
        }

        public CharacterQuest[] GetInProgressQuests()
        {
            return this.Quests.Where((CharacterQuest x) => x.CompleteDate.V == DateTime.MinValue).ToArray();
        }

        public 角色数据()
        {
        }

        public 角色数据(账号数据 账号, string 名字, 游戏对象职业 职业, 游戏对象性别 性别, 对象发型分类 发型, 对象发色分类 发色, 对象脸型分类 脸型)
        {
            this.当前等级.V = 0;
            this.背包大小.V = 32;
            this.仓库大小.V = 16;
            this.资源包大小.V = 32;
            this.所属账号.V = 账号;
            this.角色名字.V = 名字;
            this.角色职业.V = 职业;
            this.角色性别.V = 性别;
            this.角色发型.V = 发型;
            this.角色发色.V = 发色;
            this.角色脸型.V = 脸型;
            this.创建日期.V = 主程.当前时间;
            this.当前血量.V = 角色成长.获取数据(职业, 1)[游戏对象属性.最大体力];
            this.当前蓝量.V = 角色成长.获取数据(职业, 1)[游戏对象属性.最大魔力];
            this.当前朝向.V = 计算类.随机方向();
            this.当前地图.V = Settings.玩家出生地图;
            this.重生地图.V = Settings.玩家出生地图;
            this.当前坐标.V = 地图处理网关.已分配地图(Settings.玩家出生地图).复活区域.随机坐标;
            for (int i = 0; i <= 19; i++)
            {
                this.角色货币[(游戏货币)i] = 0u;
            }
            this.玩家设置.SetValue(new uint[128].ToList());
            if (游戏物品.检索表.TryGetValue("新手药水(绑定)", out var value))
            {
                this.角色背包[0] = new 物品数据(value, this, 1, 0, 1);
                this.角色背包[1] = new 物品数据(value, this, 1, 1, 1);
                this.角色背包[2] = new 物品数据(value, this, 1, 2, 1);
                this.角色背包[3] = new 物品数据(value, this, 1, 3, 1);
                this.角色背包[4] = new 物品数据(value, this, 1, 4, 1);
            }
            游戏物品 value3;
            游戏物品 value4;
            游戏物品 value6;
            if (职业 == 游戏对象职业.龙枪 && 游戏物品.检索表.TryGetValue("木枪", out var value2) && value2 is 游戏装备 模板)
            {
                this.角色背包[5] = new 装备数据(模板, this, 1, 5, 随机生成: true);
            }
            else if (职业 == 游戏对象职业.刺客 && 游戏物品.检索表.TryGetValue("柴刀", out value3) && value3 is 游戏装备 模板2)
            {
                this.角色背包[5] = new 装备数据(模板2, this, 1, 5, 随机生成: true);
            }
            else if (职业 == 游戏对象职业.弓手 && 游戏物品.检索表.TryGetValue("新手木弓", out value4) && value4 is 游戏装备 模板3)
            {
                this.角色背包[5] = new 装备数据(模板3, this, 1, 5, 随机生成: true);
                if (游戏物品.检索表.TryGetValue("箭袋(大)", out var value5))
                {
                    this.角色背包[6] = new 物品数据(value5, this, 1, 6, value5.物品持久, 绑定: true);
                }
            }
            else if (游戏物品.检索表.TryGetValue("新手木剑", out value6) && value6 is 游戏装备 模板4)
            {
                this.角色背包[5] = new 装备数据(模板4, this, 1, 5, 随机生成: true);
                if (职业 == 游戏对象职业.道士 && 游戏物品.检索表.TryGetValue("超级护身符", out var value7))
                {
                    this.角色背包[6] = new 物品数据(value7, this, 1, 6, value7.物品持久, 绑定: true);
                }
            }
            if (游戏物品.数据表.TryGetValue((性别 == 游戏对象性别.男性) ? 99980113 : 99990113, out var value8) && value8 is 游戏装备 模板5)
            {
                this.角色装备[1] = new 装备数据(模板5, this, 0, 1);
            }
            if (铭文技能.数据表.TryGetValue(职业 switch
            {
                游戏对象职业.战士 => 10300,
                游戏对象职业.法师 => 25300,
                游戏对象职业.刺客 => 15300,
                游戏对象职业.弓手 => 20400,
                游戏对象职业.道士 => 30000,
                _ => 12000,
            }, out var value9))
            {
                技能数据 技能数据2;
                技能数据2 = new 技能数据(value9.技能编号, 0);
                this.技能数据.Add(技能数据2.技能编号.V, 技能数据2);
                this.快捷栏位[0] = 技能数据2;
                技能数据2.快捷栏位.V = 0;
            }
            if (铭文技能.数据表.TryGetValue(10480, out var value10))
            {
                技能数据 技能数据3;
                技能数据3 = new 技能数据(value10.技能编号, 0);
                this.技能数据.Add(技能数据3.技能编号.V, 技能数据3);
                this.快捷栏位[8] = 技能数据3;
                技能数据3.快捷栏位.V = 8;
            }
            if (铭文技能.数据表.TryGetValue(职业 switch
            {
                游戏对象职业.战士 => 10510,
                游戏对象职业.法师 => 25600,
                游戏对象职业.刺客 => 15490,
                游戏对象职业.弓手 => 20590,
                游戏对象职业.道士 => 30280,
                _ => 12190,
            }, out var value11))
            {
                技能数据 技能数据4;
                技能数据4 = new 技能数据(value11.技能编号, 0);
                this.技能数据.Add(技能数据4.技能编号.V, 技能数据4);
                this.快捷栏位[9] = 技能数据4;
                技能数据4.快捷栏位.V = 9;
            }
            this.龙卫记录一.V = "记录1";
            this.龙卫记录二.V = "记录2";
            this.龙卫记录三.V = "记录3";
            this.龙卫记录四.V = "记录4";
            this.龙卫记录五.V = string.Empty;
            foreach (KeyValuePair<byte, 游戏天赋> item in 游戏天赋.数据表)
            {
                this.天赋刻印.Add(item.Key, int.MaxValue);
                this.天赋等级.Add(item.Key, 0);
                this.天赋经验.Add(item.Key, 0);
            }
            foreach (int value12 in Enum.GetValues(typeof(日程找回)))
            {
                this.找回奖励.Add((ushort)value12, 常量类.找回奖励字典[(ushort)value12]);
            }
            游戏数据网关.角色数据表.添加数据(this, 分配索引: true);
            账号.角色列表.Add(this);
            this.加载完成();
            //主程.WebLog(LogDataType.CreateGameRole, (账号.所属UUID.V == string.Empty || 账号.所属UUID.V == null) ? Settings.统计UUID代码 : 账号.所属UUID.V, Settings.游戏区服名称, 账号.推荐人码.V, 账号.账号名字.V, 名字);
        }

        public override string ToString()
        {
            return this.角色名字?.V;
        }

        public void 订阅事件()
        {
            this.所属账号.更改事件 += delegate (账号数据 O)
            {
                主窗口.更新角色数据(this, "所属账号", O);
                主窗口.更新角色数据(this, "账号封禁", (O.封禁日期.V != default(DateTime)) ? O.封禁日期 : null);
            };
            this.所属账号.V.封禁日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "账号封禁", (O != default(DateTime)) ? ((object)O) : null);
            };
            this.角色名字.更改事件 += delegate (string O)
            {
                主窗口.更新角色数据(this, "角色名字", O);
            };
            this.封禁日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "角色封禁", (O != default(DateTime)) ? ((object)O) : null);
            };
            this.冻结日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "冻结日期", (O != default(DateTime)) ? ((object)O) : null);
            };
            this.删除日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "删除日期", (O != default(DateTime)) ? ((object)O) : null);
            };
            this.登录日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "登录日期", (O != default(DateTime)) ? ((object)O) : null);
            };
            this.离线日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "离线日期", (this.网络连接 == null) ? ((object)O) : null);
            };
            this.网络地址.更改事件 += delegate (string O)
            {
                主窗口.更新角色数据(this, "网络地址", O);
            };
            this.物理地址.更改事件 += delegate (string O)
            {
                主窗口.更新角色数据(this, "物理地址", O);
            };
            this.角色职业.更改事件 += delegate (游戏对象职业 O)
            {
                主窗口.更新角色数据(this, "角色职业", O);
            };
            this.角色性别.更改事件 += delegate (游戏对象性别 O)
            {
                主窗口.更新角色数据(this, "角色性别", O);
            };
            this.所属行会.更改事件 += delegate (行会数据 O)
            {
                主窗口.更新角色数据(this, "所属行会", O);
            };
            this.消耗元宝.更改事件 += delegate (long O)
            {
                主窗口.更新角色数据(this, "消耗元宝", O);
            };
            this.转出金币.更改事件 += delegate (long O)
            {
                主窗口.更新角色数据(this, "转出金币", O);
            };
            this.背包大小.更改事件 += delegate (byte O)
            {
                主窗口.更新角色数据(this, "背包大小", O);
            };
            this.仓库大小.更改事件 += delegate (byte O)
            {
                主窗口.更新角色数据(this, "仓库大小", O);
            };
            this.本期特权.更改事件 += delegate (byte O)
            {
                主窗口.更新角色数据(this, "本期特权", O);
            };
            this.本期日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "本期日期", O);
            };
            this.上期特权.更改事件 += delegate (byte O)
            {
                主窗口.更新角色数据(this, "上期特权", O);
            };
            this.上期日期.更改事件 += delegate (DateTime O)
            {
                主窗口.更新角色数据(this, "上期日期", O);
            };
            this.剩余特权.更改事件 += delegate (List<KeyValuePair<byte, int>> O)
            {
                主窗口.更新角色数据(this, "剩余特权", O.Sum((KeyValuePair<byte, int> X) => X.Value));
            };
            this.当前等级.更改事件 += delegate (byte O)
            {
                主窗口.更新角色数据(this, "当前等级", O);
            };
            this.当前经验.更改事件 += delegate (long O)
            {
                主窗口.更新角色数据(this, "当前经验", O);
            };
            this.双倍经验.更改事件 += delegate (int O)
            {
                主窗口.更新角色数据(this, "双倍经验", O);
            };
            this.当前战力.更改事件 += delegate (int O)
            {
                主窗口.更新角色数据(this, "当前战力", O);
            };
            this.当前地图.更改事件 += delegate (int O)
            {
                主窗口.更新角色数据(this, "当前地图", 游戏地图.数据表.TryGetValue((byte)O, out var value) ? value : ((object)O));
            };
            this.当前坐标.更改事件 += delegate (Point O)
            {
                主窗口.更新角色数据(this, "当前坐标", $"{O.X}, {O.Y}");
            };
            this.当前PK值.更改事件 += delegate (int O)
            {
                主窗口.更新角色数据(this, "当前PK值", O);
            };
            this.技能数据.更改事件 += delegate (List<KeyValuePair<ushort, 技能数据>> O)
            {
                主窗口.更新角色技能(this, O);
            };
            this.角色装备.更改事件 += delegate (List<KeyValuePair<byte, 装备数据>> O)
            {
                主窗口.更新角色装备(this, O);
            };
            this.角色背包.更改事件 += delegate (List<KeyValuePair<byte, 物品数据>> O)
            {
                if (this.角色在线(out var 网络) && 网络.绑定角色 != null)
                {
                    网络.绑定角色.UpdateAchievementProgress(sendMsg: false);
                }
                主窗口.更新角色背包(this, O);
            };
            this.角色仓库.更改事件 += delegate (List<KeyValuePair<byte, 物品数据>> O)
            {
                主窗口.更新角色仓库(this, O);
            };
        }

        public override void 加载完成()
        {
            this.订阅事件();
            主窗口.添加角色数据(this);
            主窗口.更新角色技能(this, this.技能数据.ToList());
            主窗口.更新角色装备(this, this.角色装备.ToList());
            主窗口.更新角色背包(this, this.角色背包.ToList());
            主窗口.更新角色仓库(this, this.角色仓库.ToList());
        }

        public override void 删除数据()
        {
            this.所属账号.V.角色列表.Remove(this);
            this.所属账号.V.冻结列表.Remove(this);
            this.所属账号.V.删除列表.Remove(this);
            this.升级装备.V?.删除数据();
            foreach (宠物数据 item in this.宠物数据)
            {
                item.删除数据();
            }
            foreach (邮件数据 item2 in this.角色邮件)
            {
                item2.删除数据();
            }
            foreach (KeyValuePair<byte, 物品数据> item3 in this.角色背包)
            {
                item3.Value.删除数据();
            }
            foreach (KeyValuePair<byte, 装备数据> item4 in this.角色装备)
            {
                item4.Value.删除数据();
            }
            foreach (KeyValuePair<byte, 物品数据> item5 in this.角色仓库)
            {
                item5.Value.删除数据();
            }
            foreach (KeyValuePair<ushort, 技能数据> item6 in this.技能数据)
            {
                item6.Value.删除数据();
            }
            foreach (KeyValuePair<ushort, Buff数据> item7 in this.Buff数据)
            {
                item7.Value.删除数据();
            }
            foreach (龙卫数据 item8 in this.龙卫属性)
            {
                item8.删除数据();
            }
            foreach (CharacterQuest quest in this.Quests)
            {
                quest.删除数据();
            }
            if (this.所属队伍.V != null)
            {
                if (this == this.所属队伍.V.队长数据)
                {
                    this.所属队伍.V.删除数据();
                }
                else
                {
                    this.所属队伍.V.队伍成员.Remove(this);
                }
            }
            if (this.所属师门.V != null)
            {
                if (this == this.所属师门.V.师父数据)
                {
                    this.所属师门.V.删除数据();
                }
                else
                {
                    this.所属师门.V.移除徒弟(this);
                }
            }
            if (this.所属行会.V != null)
            {
                this.所属行会.V.行会成员.Remove(this);
                this.所属行会.V.行会禁言.Remove(this);
            }
            foreach (角色数据 item9 in this.好友列表)
            {
                item9.好友列表.Remove(this);
            }
            foreach (角色数据 item10 in this.粉丝列表)
            {
                item10.偶像列表.Remove(this);
            }
            foreach (角色数据 item11 in this.仇恨列表)
            {
                item11.仇人列表.Remove(this);
            }
            base.删除数据();
        }

        public byte[] 角色描述()
        {
            using MemoryStream memoryStream = new MemoryStream(new byte[94]);
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(base.数据索引.V);
            binaryWriter.Write(this.名字描述());
            binaryWriter.Seek(61, SeekOrigin.Begin);
            binaryWriter.Write((byte)this.角色职业.V);
            binaryWriter.Write((byte)this.角色性别.V);
            binaryWriter.Write((byte)this.角色发型.V);
            binaryWriter.Write((byte)this.角色发色.V);
            binaryWriter.Write((byte)this.角色脸型.V);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this.角色等级);
            binaryWriter.Write(this.当前地图.V);
            binaryWriter.Write(this.角色装备[0]?.升级次数.V ?? 0);
            binaryWriter.Write((this.角色装备[0]?.对应模板.V?.物品编号).GetValueOrDefault());
            binaryWriter.Write((this.角色装备[1]?.对应模板.V?.物品编号).GetValueOrDefault());
            binaryWriter.Write((this.角色装备[2]?.对应模板.V?.物品编号).GetValueOrDefault());
            binaryWriter.Write(计算类.时间转换(this.离线日期.V));
            binaryWriter.Write((!this.冻结日期.V.Equals(default(DateTime))) ? 计算类.时间转换(this.冻结日期.V) : 0);
            return memoryStream.ToArray();
        }

        public byte[] 名字描述()
        {
            return Encoding.UTF8.GetBytes(this.角色名字.V);
        }

        public byte[] 角色设置()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (uint item in this.玩家设置)
            {
                binaryWriter.Write(item);
            }
            return memoryStream.ToArray();
        }

        public byte[] 邮箱描述()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((ushort)this.角色邮件.Count);
            foreach (邮件数据 item in this.角色邮件)
            {
                binaryWriter.Write(item.邮件检索描述());
            }
            return memoryStream.ToArray();
        }

        public 龙卫数据 获取龙卫数据(byte 属性位置, int 当前阶段, 龙卫词缀类型 词缀, byte 记录序号 = byte.MaxValue)
        {
            哈希监视器<龙卫数据> 哈希监视器2;
            哈希监视器2 = null;
            foreach (龙卫数据 item in 记录序号 switch
            {
                0 => this.龙卫属性一,
                1 => this.龙卫属性二,
                2 => this.龙卫属性三,
                3 => this.龙卫属性四,
                4 => this.龙卫属性五,
                _ => this.龙卫属性,
            })
            {
                if (item.当前阶段 == 当前阶段 && item.对应模板.V.词缀类型 == 词缀 && item.位置编号.V == 属性位置)
                {
                    return item;
                }
            }
            return null;
        }

        public byte[] 获取龙卫激活封包数据(byte 属性位置)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            for (int i = 1; i < 4; i++)
            {
                龙卫数据 龙卫数据2;
                龙卫数据2 = this.获取龙卫数据(属性位置, i, 龙卫词缀类型.攻击);
                if (龙卫数据2 != null && 龙卫数据2.是否激活)
                {
                    binaryWriter.Write(1);
                }
                else
                {
                    binaryWriter.Write(0);
                }
            }
            for (int j = 1; j < 4; j++)
            {
                龙卫数据 龙卫数据3;
                龙卫数据3 = this.获取龙卫数据(属性位置, j, 龙卫词缀类型.防御);
                if (龙卫数据3 != null && 龙卫数据3.是否激活)
                {
                    binaryWriter.Write(1);
                }
                else
                {
                    binaryWriter.Write(0);
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 获取龙卫属性封包数据(byte 属性位置, byte 记录序号 = byte.MaxValue)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            int value;
            value = 0;
            for (int i = 1; i < 4; i++)
            {
                龙卫数据 龙卫数据2;
                龙卫数据2 = this.获取龙卫数据(属性位置, i, 龙卫词缀类型.攻击, 记录序号);
                if (i == 2 || i == 3)
                {
                    龙卫数据 obj;
                    obj = this.获取龙卫数据(属性位置, i - 1, 龙卫词缀类型.攻击, 记录序号);
                    value = ((obj != null && obj.龙卫模板.占位数量 > 1) ? 1 : 0);
                }
                if (龙卫数据2 == null)
                {
                    binaryWriter.Write(value);
                    binaryWriter.Write(0);
                    binaryWriter.Write(0);
                }
                else
                {
                    binaryWriter.Write(龙卫数据2.龙卫模板.占位数量);
                    binaryWriter.Write(龙卫数据2.属性编号);
                    binaryWriter.Write(龙卫数据2.当前属性);
                }
            }
            for (int j = 1; j < 4; j++)
            {
                龙卫数据 龙卫数据3;
                龙卫数据3 = this.获取龙卫数据(属性位置, j, 龙卫词缀类型.防御, 记录序号);
                if (j == 2 || j == 3)
                {
                    龙卫数据 obj2;
                    obj2 = this.获取龙卫数据(属性位置, j - 1, 龙卫词缀类型.防御, 记录序号);
                    value = ((obj2 != null && obj2.龙卫模板.占位数量 > 1) ? 1 : 0);
                }
                if (龙卫数据3 == null)
                {
                    binaryWriter.Write(value);
                    binaryWriter.Write(0);
                    binaryWriter.Write(0);
                }
                else
                {
                    binaryWriter.Write(龙卫数据3.龙卫模板.占位数量);
                    binaryWriter.Write(龙卫数据3.属性编号);
                    binaryWriter.Write(龙卫数据3.当前属性);
                }
            }
            return memoryStream.ToArray();
        }

        public byte[] 获取龙卫记录封包数据(byte 属性位置, byte 记录序号 = byte.MaxValue)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            for (int i = 1; i < 4; i++)
            {
                龙卫数据 龙卫数据2;
                龙卫数据2 = this.获取龙卫数据(属性位置, i, 龙卫词缀类型.攻击, 记录序号);
                if (龙卫数据2 == null)
                {
                    binaryWriter.Write(0);
                    binaryWriter.Write(0);
                }
                else
                {
                    binaryWriter.Write(龙卫数据2.属性编号);
                    binaryWriter.Write(龙卫数据2.当前属性);
                }
            }
            for (int j = 1; j < 4; j++)
            {
                龙卫数据 龙卫数据3;
                龙卫数据3 = this.获取龙卫数据(属性位置, j, 龙卫词缀类型.防御, 记录序号);
                if (龙卫数据3 == null)
                {
                    binaryWriter.Write(0);
                    binaryWriter.Write(0);
                }
                else
                {
                    binaryWriter.Write(龙卫数据3.属性编号);
                    binaryWriter.Write(龙卫数据3.当前属性);
                }
            }
            return memoryStream.ToArray();
        }

        public bool 获取龙卫登录记录封包(byte 属性位置, byte 记录序号, out byte[] 返回值)
        {
            返回值 = null;
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            int num;
            num = 1;
            while (true)
            {
                if (num < 4)
                {
                    龙卫数据 龙卫数据2;
                    龙卫数据2 = this.获取龙卫数据(属性位置, num, 龙卫词缀类型.攻击, 记录序号);
                    if (龙卫数据2 == null)
                    {
                        if (num == 1)
                        {
                            break;
                        }
                        binaryWriter.Write(0);
                        binaryWriter.Write(0);
                    }
                    else if (龙卫数据2 != null)
                    {
                        binaryWriter.Write(龙卫数据2.属性编号);
                        binaryWriter.Write(龙卫数据2.当前属性);
                    }
                    num++;
                    continue;
                }
                int num2;
                num2 = 1;
                while (true)
                {
                    if (num2 < 4)
                    {
                        龙卫数据 龙卫数据3;
                        龙卫数据3 = this.获取龙卫数据(属性位置, num2, 龙卫词缀类型.防御, 记录序号);
                        if (龙卫数据3 == null)
                        {
                            if (num2 == 1)
                            {
                                break;
                            }
                            binaryWriter.Write(0);
                            binaryWriter.Write(0);
                        }
                        else if (龙卫数据3 != null)
                        {
                            binaryWriter.Write(龙卫数据3.属性编号);
                            binaryWriter.Write(龙卫数据3.当前属性);
                        }
                        num2++;
                        continue;
                    }
                    返回值 = memoryStream.ToArray();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
