using DevExpress.XtraBars;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace 游戏服务器
{
    internal static class Settings
    {
        public delegate void SaveHandle();

        public delegate void LoadHandle();

        public static ushort 客户连接端口 = 8701;

        public static ushort 门票接收端口 = 6678;

        // PROTO-03: 门票接收 UDP 端口绑定 IPAddress.Any, 任何源 IP 都能注入伪造门票登录任意账号.
        // 此白名单声明可信账号服务器 IP, 以逗号分隔. 空字符串 = 不过滤 (兼容旧部署, 启动时打警告).
        // 建议生产环境设置为账号服务器实际地址, 如 "10.0.0.5,127.0.0.1".
        public static string 门票来源白名单 = "";

        public static ushort 封包限定数量 = 100;

        public static ushort 异常屏蔽时间 = 5;

        public static ushort 掉线判定时间 = 5;

        public static byte 游戏开放等级 = 40;

        public static decimal 装备特修折扣 = 1m;

        public static decimal 怪物额外爆率 = default(decimal);

        public static decimal 怪物经验倍率 = 1m;

        public static byte 减收益等级差 = 10;

        public static decimal 收益减少比率 = 0.1m;

        public static ushort 怪物诱惑时长 = 120;

        public static byte 物品清理时间 = 5;

        public static byte 物品归属时间 = 3;

        public static string 游戏数据目录 = ".\\Database";

        public static string 数据备份目录 = ".\\Backup";

        public static string 系统公告内容 = "";

        public static byte 新手扶持等级 = 0;

        public static byte 自动保存时间 = 5;

        public static byte 武斗场时间一 = 13;

        public static byte 武斗场时间二 = 21;

        public static int 武斗普通经验 = 500;

        public static int 武斗抢点经验 = 2500;

        public static DateTime 开服日期 = DateTime.Now;

        public static bool 开启线程发包 = true;

        public static bool 开启自动战斗 = true;

        public static bool 开启任务系统 = true;

        public static bool 开启成就系统 = true;

        public static bool 开启七天奖励 = true;

        public static bool 使用新版内挂 = false;

        public static bool 沙巴克掉装备 = false;

        public static bool 限制重要封包间隔时间 = true;

        public static bool 开启lua = true;

        public static bool 触发装备重铸 = false;

        public static bool 资源包只能放材料 = true;

        public static bool 可购买玛法特权 = true;

        public static int 玩家出生地图 = 142;

        public static string 游戏区服名称 = "开天辟地";

        public static string 统计UUID代码 = "";

        // HTTP 充值/管理回调接口的签名密钥. 必须由运营方配置为足够长且高熵的随机字符串.
        // 若保持为空, WebApi.WebApiService.Sign() 会拒绝处理所有签名请求.
        public static string 充值签名密钥 = "";

        public static byte 暴击特效ID = 0;

        public static bool BOSS自动死亡 = true;

        public static bool 普通强化不碎武器 = false;

        public static bool 屏蔽七天活动 = false;

        public static bool 屏蔽威望 = false;

        public static bool 屏蔽战功 = false;

        public static bool 屏蔽日程 = false;

        public static bool 屏蔽每周特惠 = false;

        public static bool 屏蔽每日签到 = false;

        public static bool 屏蔽传永武技 = false;

        public static int 充值货币类型 = 0;

        public static bool 达最高级后继续加经验 = true;

        public static int 神佑掉落ID = 90313;

        public static int 商人比例 = 110;

        public static string 充值公告 = "";

        public static string 商人发货公告 = "";

        public static uint 货币异常上限 = 4200000000u;

        public static uint 货币异常归位 = 10000u;

        public static int 祝福油几率0级 = 80;

        public static int 祝福油几率1级 = 10;

        public static int 祝福油几率2级 = 8;

        public static int 祝福油几率3级 = 8;

        public static int 祝福油几率4级 = 5;

        public static int 祝福油几率5级 = 3;

        public static int 祝福油几率6级 = 3;

        public static float 死亡掉落剑甲 = 0.01f;

        public static float 死亡掉落首饰 = 0.05f;

        public static float 死亡掉落背包 = 0.1f;

        public static int 单次死亡限量 = 3;

        public static float 红名掉落剑甲 = 0.01f;

        public static float 红名掉落首饰 = 0.05f;

        public static int 龙卫重铸费用 = 10000;

        public static int 锁单重铸费用 = 250000;

        public static int 锁半重铸费用 = 500000;

        public static int 行会最高人数 = 40;

        public static byte 幽冥海底节点开放天数 = 10;

        public static byte 白日赤月节点开放天数 = 21;

        public static byte 魔龙之城节点开放天数 = 45;

        public static byte 苍月惊变节点开放天数 = 75;

        public static byte 龙耀雪山节点开放天数 = 100;

        public static byte 聊天限制等级 = 20;

        public static int 玛法新秀价格 = 68;

        public static int 玛法名俊价格 = 128;

        public static int 玛法豪杰价格 = 288;

        public static int 玛法战将价格 = 288;

        public static int 玛法至尊价格 = 588;

        public static float 技巧项链倍数 = 2f;

        public static bool 金币自动入包 = true;

        public static bool 银币自动入包 = true;

        public static bool 物品自动入包 = false;

        public static bool 自动分解装备 = false;

        public static bool 不分解极品装备 = false;

        public static bool 安全区内满血满蓝 = true;

        public static string 默认皮肤 = "Office 2010 Blue";

        public static bool 下线宝宝不死 = false;

        public static bool 专用网关登录器 = true;

        public static bool[] 职业开放 = new bool[6] { true, true, true, true, true, true };

        public static string EnvirPath = Path.Combine(Settings.游戏数据目录, "Envir");

        public static string NameListPath = Path.Combine(Settings.EnvirPath, "NameLists");

        public static string NPCPath = Path.Combine(Settings.EnvirPath, "NPCs");

        public static string ValuePath = Path.Combine(Settings.EnvirPath, "Values");

        public static string DefaultNPCFilename = "00-QFunction";

        private static InIReader iniconfig;

        [CompilerGenerated]
        private static SaveHandle _0004_0002_0004_0008_0002_0006_0002_0005_0007_0002;

        [CompilerGenerated]
        private static SaveHandle _0013_0014_0013_0009_0016_0006;

        public static event SaveHandle OnSaved
        {
            [CompilerGenerated]
            add
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002, (SaveHandle)Delegate.Combine(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
            [CompilerGenerated]
            remove
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002, (SaveHandle)Delegate.Remove(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
        }

        public static event SaveHandle OnLoaded
        {
            [CompilerGenerated]
            add
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0013_0014_0013_0009_0016_0006;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0013_0014_0013_0009_0016_0006, (SaveHandle)Delegate.Combine(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
            [CompilerGenerated]
            remove
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0013_0014_0013_0009_0016_0006;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0013_0014_0013_0009_0016_0006, (SaveHandle)Delegate.Remove(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
        }

        public static void Load()
        {
            Settings.iniconfig = new InIReader(".\\Setup.ini");
            Settings.客户连接端口 = Settings.iniconfig.ReadUInt16("General", "客户连接端口", Settings.客户连接端口);
            Settings.门票接收端口 = Settings.iniconfig.ReadUInt16("General", "门票接收端口", Settings.门票接收端口);
            Settings.门票来源白名单 = Settings.iniconfig.ReadString("General", "门票来源白名单", Settings.门票来源白名单);
            Settings.封包限定数量 = Settings.iniconfig.ReadUInt16("General", "封包限定数量", Settings.封包限定数量);
            Settings.异常屏蔽时间 = Settings.iniconfig.ReadUInt16("General", "异常屏蔽时间", Settings.异常屏蔽时间);
            Settings.掉线判定时间 = Settings.iniconfig.ReadUInt16("General", "掉线判定时间", Settings.掉线判定时间);
            Settings.游戏开放等级 = Settings.iniconfig.ReadByte("General", "游戏开放等级", Settings.游戏开放等级);
            Settings.装备特修折扣 = Settings.iniconfig.ReadDecimal("General", "装备特修折扣", Settings.装备特修折扣);
            Settings.怪物额外爆率 = Settings.iniconfig.ReadDecimal("General", "怪物额外爆率", Settings.怪物额外爆率);
            Settings.怪物经验倍率 = Settings.iniconfig.ReadDecimal("General", "怪物经验倍率", Settings.怪物经验倍率);
            Settings.减收益等级差 = Settings.iniconfig.ReadByte("General", "减收益等级差", Settings.减收益等级差);
            Settings.收益减少比率 = Settings.iniconfig.ReadDecimal("General", "收益减少比率", Settings.收益减少比率);
            Settings.怪物诱惑时长 = Settings.iniconfig.ReadUInt16("General", "怪物诱惑时长", Settings.怪物诱惑时长);
            Settings.物品清理时间 = Settings.iniconfig.ReadByte("General", "物品清理时间", Settings.物品清理时间);
            Settings.物品归属时间 = Settings.iniconfig.ReadByte("General", "物品归属时间", Settings.物品归属时间);
            Settings.游戏数据目录 = Settings.iniconfig.ReadString("General", "游戏数据目录", Settings.游戏数据目录);
            Settings.数据备份目录 = Settings.iniconfig.ReadString("General", "数据备份目录", Settings.数据备份目录);
            Settings.系统公告内容 = Settings.iniconfig.ReadString("General", "系统公告内容", Settings.系统公告内容);
            Settings.新手扶持等级 = Settings.iniconfig.ReadByte("General", "新手扶持等级", Settings.新手扶持等级);
            Settings.自动保存时间 = Settings.iniconfig.ReadByte("General", "自动保存时间", Settings.自动保存时间);
            Settings.武斗场时间一 = Settings.iniconfig.ReadByte("General", "武斗场时间一", Settings.武斗场时间一);
            Settings.武斗场时间二 = Settings.iniconfig.ReadByte("General", "武斗场时间二", Settings.武斗场时间二);
            Settings.武斗普通经验 = Settings.iniconfig.ReadInt32("General", "武斗普通经验", Settings.武斗普通经验);
            Settings.武斗抢点经验 = Settings.iniconfig.ReadInt32("General", "武斗抢点经验", Settings.武斗抢点经验);
            Settings.开服日期 = 计算类.转换日期(Settings.iniconfig.ReadInt32("General", "开服日期", 0));
            Settings.开启线程发包 = Settings.iniconfig.ReadBoolean("General", "开启线程发包", Settings.开启线程发包);
            Settings.开启自动战斗 = Settings.iniconfig.ReadBoolean("General", "开启自动战斗", Settings.开启自动战斗);
            Settings.开启任务系统 = Settings.iniconfig.ReadBoolean("General", "开启任务系统", Settings.开启任务系统);
            Settings.开启成就系统 = Settings.iniconfig.ReadBoolean("General", "开启成就系统", Settings.开启成就系统);
            Settings.使用新版内挂 = Settings.iniconfig.ReadBoolean("General", "使用新版内挂", Settings.使用新版内挂);
            Settings.沙巴克掉装备 = Settings.iniconfig.ReadBoolean("General", "沙巴克掉装备", Settings.沙巴克掉装备);
            Settings.限制重要封包间隔时间 = Settings.iniconfig.ReadBoolean("General", "限制重要封包间隔时间", Settings.限制重要封包间隔时间);
            Settings.玩家出生地图 = Settings.iniconfig.ReadInt32("General", "玩家出生地图", Settings.玩家出生地图);
            Settings.游戏区服名称 = Settings.iniconfig.ReadString("General", "游戏区服名称", Settings.游戏区服名称);
            Settings.统计UUID代码 = Settings.iniconfig.ReadString("General", "统计UUID代码", Settings.统计UUID代码);
            Settings.开启lua = Settings.iniconfig.ReadBoolean("General", "开启lua", Settings.开启lua);
            Settings.触发装备重铸 = Settings.iniconfig.ReadBoolean("General", "触发装备重铸", Settings.触发装备重铸);
            Settings.资源包只能放材料 = Settings.iniconfig.ReadBoolean("General", "资源包只能放材料", Settings.资源包只能放材料);
            Settings.可购买玛法特权 = Settings.iniconfig.ReadBoolean("General", "可购买玛法特权", Settings.可购买玛法特权);
            Settings.暴击特效ID = Settings.iniconfig.ReadByte("General", "暴击特效ID", Settings.暴击特效ID);
            Settings.BOSS自动死亡 = Settings.iniconfig.ReadBoolean("General", "BOSS自动死亡", Settings.BOSS自动死亡);
            Settings.普通强化不碎武器 = Settings.iniconfig.ReadBoolean("General", "普通强化不碎武器", Settings.普通强化不碎武器);
            Settings.屏蔽七天活动 = Settings.iniconfig.ReadBoolean("General", "屏蔽七天活动", Settings.屏蔽七天活动);
            Settings.神佑掉落ID = Settings.iniconfig.ReadInt32("General", "神佑掉落ID", Settings.神佑掉落ID);
            Settings.商人比例 = Settings.iniconfig.ReadInt32("General", "商人比例", Settings.商人比例);
            Settings.充值公告 = Settings.iniconfig.ReadString("General", "充值公告", Settings.充值公告);
            Settings.商人发货公告 = Settings.iniconfig.ReadString("General", "商人发货公告", Settings.商人发货公告);
            Settings.屏蔽威望 = Settings.iniconfig.ReadBoolean("General", "屏蔽威望", Settings.屏蔽威望);
            Settings.屏蔽战功 = Settings.iniconfig.ReadBoolean("General", "屏蔽战功", Settings.屏蔽战功);
            Settings.屏蔽日程 = Settings.iniconfig.ReadBoolean("General", "屏蔽日程", Settings.屏蔽日程);
            Settings.屏蔽每周特惠 = Settings.iniconfig.ReadBoolean("General", "屏蔽每周特惠", Settings.屏蔽每周特惠);
            Settings.屏蔽每日签到 = Settings.iniconfig.ReadBoolean("General", "屏蔽每日签到", Settings.屏蔽每日签到);
            Settings.屏蔽传永武技 = Settings.iniconfig.ReadBoolean("General", "屏蔽传永武技", Settings.屏蔽传永武技);
            Settings.充值货币类型 = Settings.iniconfig.ReadInt32("General", "充值货币类型 ", Settings.充值货币类型);
            Settings.达最高级后继续加经验 = Settings.iniconfig.ReadBoolean("General", "达最高级后继续加经验", Settings.达最高级后继续加经验);
            Settings.EnvirPath = Path.Combine(Settings.游戏数据目录, "System\\Envir");
            Settings.NameListPath = Path.Combine(Settings.EnvirPath, "NameLists");
            Settings.NPCPath = Path.Combine(Settings.EnvirPath, "NPCs");
            Settings.ValuePath = Path.Combine(Settings.EnvirPath, "Values");
            Settings.祝福油几率0级 = Settings.iniconfig.ReadInt32("General", "祝福油几率0级 ", Settings.祝福油几率0级);
            Settings.祝福油几率1级 = Settings.iniconfig.ReadInt32("General", "祝福油几率1级 ", Settings.祝福油几率1级);
            Settings.祝福油几率2级 = Settings.iniconfig.ReadInt32("General", "祝福油几率2级 ", Settings.祝福油几率2级);
            Settings.祝福油几率3级 = Settings.iniconfig.ReadInt32("General", "祝福油几率3级 ", Settings.祝福油几率3级);
            Settings.祝福油几率4级 = Settings.iniconfig.ReadInt32("General", "祝福油几率4级 ", Settings.祝福油几率4级);
            Settings.祝福油几率5级 = Settings.iniconfig.ReadInt32("General", "祝福油几率5级 ", Settings.祝福油几率5级);
            Settings.祝福油几率6级 = Settings.iniconfig.ReadInt32("General", "祝福油几率6级 ", Settings.祝福油几率6级);
            Settings.死亡掉落剑甲 = Settings.iniconfig.ReadFloat("General", "死亡掉落剑甲 ", Settings.死亡掉落剑甲);
            Settings.死亡掉落首饰 = Settings.iniconfig.ReadFloat("General", "死亡掉落首饰 ", Settings.死亡掉落首饰);
            Settings.死亡掉落背包 = Settings.iniconfig.ReadFloat("General", "死亡掉落背包 ", Settings.死亡掉落背包);
            Settings.单次死亡限量 = Settings.iniconfig.ReadInt32("General", "单次死亡限量 ", Settings.单次死亡限量);
            Settings.红名掉落剑甲 = Settings.iniconfig.ReadFloat("General", "红名掉落剑甲 ", Settings.红名掉落剑甲);
            Settings.红名掉落首饰 = Settings.iniconfig.ReadFloat("General", "红名掉落首饰 ", Settings.红名掉落首饰);
            Settings.龙卫重铸费用 = Settings.iniconfig.ReadInt32("General", "龙卫重铸费用 ", Settings.龙卫重铸费用);
            Settings.锁单重铸费用 = Settings.iniconfig.ReadInt32("General", "锁单重铸费用 ", Settings.锁单重铸费用);
            Settings.锁半重铸费用 = Settings.iniconfig.ReadInt32("General", "锁半重铸费用 ", Settings.锁半重铸费用);
            Settings.行会最高人数 = Settings.iniconfig.ReadInt32("General", "行会最高人数 ", Settings.行会最高人数);
            Settings.幽冥海底节点开放天数 = Settings.iniconfig.ReadByte("General", "幽冥海底节点开放天数 ", Settings.幽冥海底节点开放天数);
            Settings.白日赤月节点开放天数 = Settings.iniconfig.ReadByte("General", "白日赤月节点开放天数 ", Settings.白日赤月节点开放天数);
            Settings.魔龙之城节点开放天数 = Settings.iniconfig.ReadByte("General", "魔龙之城节点开放天数 ", Settings.魔龙之城节点开放天数);
            Settings.苍月惊变节点开放天数 = Settings.iniconfig.ReadByte("General", "苍月惊变节点开放天数 ", Settings.苍月惊变节点开放天数);
            Settings.龙耀雪山节点开放天数 = Settings.iniconfig.ReadByte("General", "龙耀雪山节点开放天数 ", Settings.龙耀雪山节点开放天数);
            Settings.聊天限制等级 = Settings.iniconfig.ReadByte("General", "聊天限制等级 ", Settings.聊天限制等级);
            Settings.玛法新秀价格 = Settings.iniconfig.ReadInt32("General", "玛法新秀价格 ", Settings.玛法新秀价格);
            Settings.玛法名俊价格 = Settings.iniconfig.ReadInt32("General", "玛法名俊价格 ", Settings.玛法名俊价格);
            Settings.玛法豪杰价格 = Settings.iniconfig.ReadInt32("General", "玛法豪杰价格 ", Settings.玛法豪杰价格);
            Settings.玛法战将价格 = Settings.iniconfig.ReadInt32("General", "玛法战将价格 ", Settings.玛法战将价格);
            Settings.玛法至尊价格 = Settings.iniconfig.ReadInt32("General", "玛法至尊价格 ", Settings.玛法至尊价格);
            Settings.技巧项链倍数 = Settings.iniconfig.ReadFloat("General", "技巧项链倍数 ", Settings.技巧项链倍数);
            //新增
            Settings.金币自动入包 = Settings.iniconfig.ReadBoolean("General", "金币自动入包", Settings.金币自动入包);
            Settings.金币自动入包 = Settings.iniconfig.ReadBoolean("General", "银币自动入包", Settings.银币自动入包);
            Settings.物品自动入包 = Settings.iniconfig.ReadBoolean("General", "物品自动入包", Settings.物品自动入包);
            Settings.自动分解装备 = Settings.iniconfig.ReadBoolean("General", "自动分解装备", Settings.自动分解装备);
            Settings.不分解极品装备 = Settings.iniconfig.ReadBoolean("General", "不分解极品装备", Settings.不分解极品装备);

            Settings.安全区内满血满蓝 = Settings.iniconfig.ReadBoolean("General", "安全区内满血满蓝", Settings.安全区内满血满蓝);
            Settings.下线宝宝不死 = Settings.iniconfig.ReadBoolean("General", "下线宝宝不死", Settings.下线宝宝不死);

            Settings.专用网关登录器 = Settings.iniconfig.ReadBoolean("General", "专用网关登录器", Settings.专用网关登录器);

            for (int i = 0; i < 6; i++)
            {
                Settings.职业开放[i] = Settings.iniconfig.ReadBoolean("职业开放", "职业" + i, Settings.职业开放[i]);
            }
            Settings._0013_0014_0013_0009_0016_0006?.Invoke();
        }

        public static void Save()
        {
            Settings.iniconfig.Write("General", "客户连接端口", Settings.客户连接端口);
            Settings.iniconfig.Write("General", "门票接收端口", Settings.门票接收端口);
            Settings.iniconfig.Write("General", "门票来源白名单", Settings.门票来源白名单);
            Settings.iniconfig.Write("General", "封包限定数量", Settings.封包限定数量);
            Settings.iniconfig.Write("General", "异常屏蔽时间", Settings.异常屏蔽时间);
            Settings.iniconfig.Write("General", "掉线判定时间", Settings.掉线判定时间);
            Settings.iniconfig.Write("General", "游戏开放等级", Settings.游戏开放等级);
            Settings.iniconfig.Write("General", "装备特修折扣", Settings.装备特修折扣);
            Settings.iniconfig.Write("General", "怪物额外爆率", Settings.怪物额外爆率);
            Settings.iniconfig.Write("General", "怪物经验倍率", Settings.怪物经验倍率);
            Settings.iniconfig.Write("General", "减收益等级差", Settings.减收益等级差);
            Settings.iniconfig.Write("General", "收益减少比率", Settings.收益减少比率);
            Settings.iniconfig.Write("General", "怪物诱惑时长", Settings.怪物诱惑时长);
            Settings.iniconfig.Write("General", "物品清理时间", Settings.物品清理时间);
            Settings.iniconfig.Write("General", "物品归属时间", Settings.物品归属时间);
            Settings.iniconfig.Write("General", "游戏数据目录", Settings.游戏数据目录);
            Settings.iniconfig.Write("General", "数据备份目录", Settings.数据备份目录);
            Settings.iniconfig.Write("General", "系统公告内容", Settings.系统公告内容);
            Settings.iniconfig.Write("General", "新手扶持等级", Settings.新手扶持等级);
            Settings.iniconfig.Write("General", "自动保存时间", Settings.自动保存时间);
            Settings.iniconfig.Write("General", "武斗场时间一", Settings.武斗场时间一);
            Settings.iniconfig.Write("General", "武斗场时间二", Settings.武斗场时间二);
            Settings.iniconfig.Write("General", "武斗普通经验", Settings.武斗普通经验);
            Settings.iniconfig.Write("General", "武斗抢点经验", Settings.武斗抢点经验);
            Settings.iniconfig.Write("General", "开服日期", 计算类.日期转换(Settings.开服日期));
            Settings.iniconfig.Write("General", "开启线程发包", Settings.开启线程发包);
            Settings.iniconfig.Write("General", "开启自动战斗", Settings.开启自动战斗);
            Settings.iniconfig.Write("General", "使用新版内挂", Settings.使用新版内挂);
            Settings.iniconfig.Write("General", "沙巴克掉装备", Settings.沙巴克掉装备);
            Settings.iniconfig.Write("General", "限制重要封包间隔时间", Settings.限制重要封包间隔时间);
            Settings.iniconfig.Write("General", "开启任务系统", Settings.开启任务系统);
            Settings.iniconfig.Write("General", "开启成就系统", Settings.开启成就系统);
            Settings.iniconfig.Write("General", "玩家出生地图", Settings.玩家出生地图);
            Settings.iniconfig.Write("General", "游戏区服名称", Settings.游戏区服名称);
            Settings.iniconfig.Write("General", "统计UUID代码", Settings.统计UUID代码);
            Settings.iniconfig.Write("General", "开启lua", Settings.开启lua);
            Settings.iniconfig.Write("General", "触发装备重铸", Settings.触发装备重铸);
            Settings.iniconfig.Write("General", "资源包只能放材料", Settings.资源包只能放材料);
            Settings.iniconfig.Write("General", "可购买玛法特权", Settings.可购买玛法特权);
            Settings.iniconfig.Write("General", "暴击特效ID", Settings.暴击特效ID);
            Settings.iniconfig.Write("General", "BOSS自动死亡", Settings.BOSS自动死亡);
            Settings.iniconfig.Write("General", "普通强化不碎武器", Settings.普通强化不碎武器);
            Settings.iniconfig.Write("General", "屏蔽七天活动", Settings.屏蔽七天活动);
            Settings.iniconfig.Write("General", "神佑掉落ID", Settings.神佑掉落ID);
            Settings.iniconfig.Write("General", "商人比例", Settings.商人比例);
            Settings.iniconfig.Write("General", "充值公告", Settings.充值公告);
            Settings.iniconfig.Write("General", "商人发货公告", Settings.商人发货公告);
            Settings.iniconfig.Write("General", "屏蔽威望", Settings.屏蔽威望);
            Settings.iniconfig.Write("General", "屏蔽战功", Settings.屏蔽战功);
            Settings.iniconfig.Write("General", "屏蔽日程", Settings.屏蔽日程);
            Settings.iniconfig.Write("General", "屏蔽每周特惠", Settings.屏蔽每周特惠);
            Settings.iniconfig.Write("General", "屏蔽每日签到", Settings.屏蔽每日签到);
            Settings.iniconfig.Write("General", "屏蔽传永武技", Settings.屏蔽传永武技);
            Settings.iniconfig.Write("General", "充值货币类型 ", Settings.充值货币类型);
            Settings.iniconfig.Write("General", "达最高级后继续加经验", Settings.达最高级后继续加经验);
            Settings.iniconfig.Write("General", "祝福油几率0级 ", Settings.祝福油几率0级);
            Settings.iniconfig.Write("General", "祝福油几率1级 ", Settings.祝福油几率1级);
            Settings.iniconfig.Write("General", "祝福油几率2级 ", Settings.祝福油几率2级);
            Settings.iniconfig.Write("General", "祝福油几率3级 ", Settings.祝福油几率3级);
            Settings.iniconfig.Write("General", "祝福油几率4级 ", Settings.祝福油几率4级);
            Settings.iniconfig.Write("General", "祝福油几率5级 ", Settings.祝福油几率5级);
            Settings.iniconfig.Write("General", "祝福油几率6级 ", Settings.祝福油几率6级);
            Settings.iniconfig.Write("General", "死亡掉落剑甲 ", Settings.死亡掉落剑甲);
            Settings.iniconfig.Write("General", "死亡掉落首饰 ", Settings.死亡掉落首饰);
            Settings.iniconfig.Write("General", "死亡掉落背包 ", Settings.死亡掉落背包);
            Settings.iniconfig.Write("General", "单次死亡限量 ", Settings.单次死亡限量);
            Settings.iniconfig.Write("General", "红名掉落剑甲 ", Settings.红名掉落剑甲);
            Settings.iniconfig.Write("General", "红名掉落首饰 ", Settings.红名掉落首饰);
            Settings.iniconfig.Write("General", "龙卫重铸费用 ", Settings.龙卫重铸费用);
            Settings.iniconfig.Write("General", "锁单重铸费用 ", Settings.锁单重铸费用);
            Settings.iniconfig.Write("General", "锁半重铸费用 ", Settings.锁半重铸费用);
            Settings.iniconfig.Write("General", "行会最高人数 ", Settings.行会最高人数);
            Settings.iniconfig.Write("General", "幽冥海底节点开放天数 ", Settings.幽冥海底节点开放天数);
            Settings.iniconfig.Write("General", "白日赤月节点开放天数 ", Settings.白日赤月节点开放天数);
            Settings.iniconfig.Write("General", "魔龙之城节点开放天数 ", Settings.魔龙之城节点开放天数);
            Settings.iniconfig.Write("General", "苍月惊变节点开放天数 ", Settings.苍月惊变节点开放天数);
            Settings.iniconfig.Write("General", "龙耀雪山节点开放天数 ", Settings.龙耀雪山节点开放天数);
            Settings.iniconfig.Write("General", "聊天限制等级 ", Settings.聊天限制等级);
            Settings.iniconfig.Write("General", "玛法新秀价格 ", Settings.玛法新秀价格);
            Settings.iniconfig.Write("General", "玛法名俊价格 ", Settings.玛法名俊价格);
            Settings.iniconfig.Write("General", "玛法豪杰价格 ", Settings.玛法豪杰价格);
            Settings.iniconfig.Write("General", "玛法战将价格 ", Settings.玛法战将价格);
            Settings.iniconfig.Write("General", "玛法至尊价格 ", Settings.玛法至尊价格);
            Settings.iniconfig.Write("General", "技巧项链倍数 ", Settings.技巧项链倍数);
            //新增
            Settings.iniconfig.Write("General", "金币自动入包", Settings.金币自动入包);
            Settings.iniconfig.Write("General", "银币自动入包", Settings.银币自动入包);
            Settings.iniconfig.Write("General", "物品自动入包", Settings.物品自动入包);
            Settings.iniconfig.Write("General", "自动分解装备", Settings.自动分解装备);
            Settings.iniconfig.Write("General", "不分解极品装备", Settings.不分解极品装备);

            Settings.iniconfig.Write("General", "安全区内满血满蓝", Settings.安全区内满血满蓝);
            Settings.iniconfig.Write("General", "下线宝宝不死", Settings.下线宝宝不死);
            Settings.iniconfig.Write("General", "专用网关登录器", Settings.专用网关登录器);


            for (int i = 0; i < 6; i++)
            {
                Settings.iniconfig.Write("职业开放", "职业" + i, Settings.职业开放[i]);
            }
            Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002?.Invoke();
        }
    }
}
