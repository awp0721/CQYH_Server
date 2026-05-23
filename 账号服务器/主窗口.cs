using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using 账号服务器.Properties;

namespace 账号服务器
{

    public partial class 主窗口 : Form
    {
        public static uint 新注册账号数;

        public static uint 生成门票总数;

        public static long 已接收字节数;

        public static long 已发送字节数;

        public static 主窗口 主界面;

        public static string 游戏区服 = "";

        public static string 数据目录 = ".\\Accounts";

        // 账号数据 由多个网络工作线程读写, UI 线程也会读 .Count, 必须线程安全.
        public static ConcurrentDictionary<string, 账号数据> 账号数据;

        public static Dictionary<string, IPEndPoint> 区服数据;


        public 主窗口()
        {
            InitializeComponent();
            主界面 = this;
            本地监听端口.Value = Settings.Default.本地监听端口;
            门票发送端口.Value = Settings.Default.门票发送端口;
            if (!File.Exists(".\\server.txt"))
            {
                日志文本框.AppendText("未找到服务器配置文件, 请注意配置\r\n");
            }
            if (!Directory.Exists(数据目录))
            {
                日志文本框.AppendText("未找到账号配置文件夹, 请注意导入\r\n");
            }
        }

        public static void 更新已注册账号数()
        {
            主界面?.BeginInvoke((MethodInvoker)delegate
            {
                主界面.已注册账号.Text = $"已注册账号: {账号数据.Count}";
            });
        }

        public static void 更新新注册账号数()
        {
            更新已注册账号数();
            主界面?.BeginInvoke((MethodInvoker)delegate
            {
                主界面.新注册账号.Text = $"新注册账号: {新注册账号数}";
            });
        }

        public static void 更新已生成门票数()
        {
            主界面?.BeginInvoke((MethodInvoker)delegate
            {
                主界面.生成门票数.Text = $"生成门票数: {生成门票总数}";
            });
        }

        // DD: 字节数更新走 UDP 热路径, 每包调用 BeginInvoke 会让 UI 消息队列在洪水攻击下堆积爆炸.
        // 节流到 100ms 一次, 数值本身仍每包累加, 仅刷新频率受限.
        private static long 已接收字节上次刷新Ticks;
        private static long 已发送字节上次刷新Ticks;
        private const long 字节数刷新间隔Ticks = 100 * TimeSpan.TicksPerMillisecond;

        public static void 更新已接收字节数()
        {
            long now = DateTime.UtcNow.Ticks;
            long prev = Interlocked.Read(ref 已接收字节上次刷新Ticks);
            if (now - prev < 字节数刷新间隔Ticks) return;
            if (Interlocked.CompareExchange(ref 已接收字节上次刷新Ticks, now, prev) != prev) return;
            主界面?.BeginInvoke((MethodInvoker)delegate
            {
                主界面.已接收字节.Text = $"已接收字节: {已接收字节数}";
            });
        }

        public static void 更新已发送字节数()
        {
            long now = DateTime.UtcNow.Ticks;
            long prev = Interlocked.Read(ref 已发送字节上次刷新Ticks);
            if (now - prev < 字节数刷新间隔Ticks) return;
            if (Interlocked.CompareExchange(ref 已发送字节上次刷新Ticks, now, prev) != prev) return;
            主界面?.BeginInvoke((MethodInvoker)delegate
            {
                主界面.已发送字节.Text = $"已发送字节: {已发送字节数}";
            });
        }

        // 日志文本框 行数上限. 超过后丢弃前一半, 防止长时间运行(尤其遭遇 IP 伪造攻击
        // 大量打印封禁日志时) UI 内存无限增长 (MED-L).
        private const int 日志最大行数 = 5000;

        public static void 添加日志(string 内容)
        {
            主界面?.BeginInvoke((MethodInvoker)delegate
            {
                if (主界面.日志文本框.Lines.Length >= 日志最大行数)
                {
                    string[] lines = 主界面.日志文本框.Lines;
                    string[] kept = new string[lines.Length / 2];
                    Array.Copy(lines, lines.Length - kept.Length, kept, 0, kept.Length);
                    主界面.日志文本框.Lines = kept;
                }
                主界面.日志文本框.AppendText(内容 + "\r\n");
                主界面.日志文本框.ScrollToCaret();
            });
        }

        public static void 添加账号(账号数据 账号)
        {
            // TryAdd 原子地保证不会被并发覆盖, 同时避免 ContainsKey -> Add 的 TOCTOU.
            if (账号数据.TryAdd(账号.账号名字, 账号))
            {
                保存账号(账号);
            }
        }

        // 全局写盘节流: 每秒最多 20 次磁盘写入, 防止伪造 IP 绕过 per-IP 注册限速
        // 后对磁盘形成 IO 放大攻击 (MED-K). 超额请求需要等待或被上层拒绝.
        private const int 写盘每秒上限 = 20;
        private static readonly object 写盘节流锁 = new object();
        private static DateTime 写盘窗口起点 = DateTime.UtcNow;
        private static int 写盘窗口计数 = 0;

        public static bool 写盘许可()
        {
            lock (写盘节流锁)
            {
                DateTime now = DateTime.UtcNow;
                if ((now - 写盘窗口起点).TotalSeconds >= 1.0)
                {
                    写盘窗口起点 = now;
                    写盘窗口计数 = 0;
                }
                if (写盘窗口计数 >= 写盘每秒上限) return false;
                写盘窗口计数++;
                return true;
            }
        }

        public static void 保存账号(账号数据 账号)
        {
            if (!是合法账号名(账号.账号名字))
            {
                throw new ArgumentException("非法账号名: " + 账号.账号名字);
            }
            string 根目录 = Path.GetFullPath(数据目录);
            string 文件路径 = Path.GetFullPath(Path.Combine(根目录, 账号.账号名字 + ".txt"));
            if (!文件路径.StartsWith(根目录 + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !文件路径.StartsWith(根目录 + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("路径越权: " + 文件路径);
            }
            File.WriteAllText(文件路径, 序列化类.序列化(账号));
        }

        private static bool 是合法账号名(string 名字)
        {
            if (string.IsNullOrEmpty(名字) || 名字.Length > 64) return false;
            foreach (char c in 名字)
            {
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-')) return false;
            }
            return true;
        }

        private void 启动服务_Click(object sender, EventArgs e)
        {
            if (!File.Exists(".\\Server.txt"))
            {
                添加日志("配置文件不存在, 已自动创建");
                File.WriteAllBytes(".\\Server.txt", new byte[0]);
                string text = "127.0.0.1,8701/传奇永恒";
                File.WriteAllText(@".\\Server.txt", text);
            }

            if (区服数据 == null || 区服数据.Count == 0)
            {
                加载配置按钮_Click(sender, e);
            }
            if (区服数据 == null || 区服数据.Count == 0)
            {
                添加日志("服务器配置为空, 启动失败");
                return;
            }
            if (账号数据 == null || 账号数据.Count == 0)
            {
                加载账号按钮_Click(sender, e);
            }
            if (网络通信.启动服务())
            {
                停止服务按钮.Enabled = true;
                Button button = 加载配置按钮;
                bool enabled = (加载账号按钮.Enabled = false);
                button.Enabled = enabled;
                启动服务按钮.Enabled = false;
                本地监听端口.Enabled = false;
                门票发送端口.Enabled = false;
                Settings.Default.本地监听端口 = (ushort)本地监听端口.Value;
                Settings.Default.门票发送端口 = (ushort)门票发送端口.Value;
                Settings.Default.Save();
            }
        }

        private void 停止服务_Click(object sender, EventArgs e)
        {
            网络通信.结束服务();
            停止服务按钮.Enabled = false;
            Button button = 加载配置按钮;
            bool enabled = (加载账号按钮.Enabled = true);
            button.Enabled = enabled;
            Button button2 = 启动服务按钮;
            NumericUpDown numericUpDown = 本地监听端口;
            bool flag3 = (门票发送端口.Enabled = true);
            enabled = (numericUpDown.Enabled = flag3);
            button2.Enabled = enabled;
        }

        private void 隐藏窗口_Click(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定关闭服务器?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                // LL: 与"结束进程"路径对齐, 先优雅关闭 UDP socket 与限速表定时器再退出.
                网络通信.结束服务();
                最小化到托盘.Visible = false;
                Environment.Exit(0);
                return;
            }
            最小化到托盘.Visible = true;
            Hide();
            if (e != null)
            {
                e.Cancel = true;
            }
            最小化到托盘.ShowBalloonTip(1000, "", "服务器已转为后台运行.", ToolTipIcon.Info);
        }

        private void 恢复窗口_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.Visible = true;
                最小化到托盘.Visible = false;
            }
        }

        private void 恢复窗口_Click(object sender, EventArgs e)
        {
            base.Visible = true;
            最小化到托盘.Visible = false;
        }

        private void 结束进程_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定关闭服务器?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                网络通信.结束服务();
                最小化到托盘.Visible = false;
                Environment.Exit(0);
            }
        }

        private void 打开配置按钮_Click(object sender, EventArgs e)
        {
            if (!File.Exists(".\\Server.txt"))
            {
                添加日志("配置文件不存在, 已自动创建");
                File.WriteAllBytes(".\\Server.txt", new byte[0]);
                string text = "127.0.0.1,8701/传奇永恒";
                File.WriteAllText(@".\\Server.txt", text);
            }
            Process.Start("notepad.exe", ".\\Server.txt");
        }

        private void 加载配置按钮_Click(object sender, EventArgs e)
        {
            if (!File.Exists(".\\Server.txt"))
            {
                return;
            }
            区服数据 = new Dictionary<string, IPEndPoint>();
            try
            {
                游戏区服 = File.ReadAllText(".\\Server.txt", Encoding.UTF8).Trim('\r', '\n', ' ');
            }
            catch (Exception 读取异常)
            {
                添加日志("读取 Server.txt 失败: " + 读取异常.Message);
                return;
            }
            string[] array = 游戏区服.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string text in array)
            {
                string[] array2 = text.Split(new char[2] { ',', '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (array2.Length != 3
                    || !IPAddress.TryParse(array2[0], out var 区服地址)
                    || !int.TryParse(array2[1], out var 区服端口)
                    || 区服端口 <= 0 || 区服端口 > 65535)
                {
                    添加日志("Server.txt 配置错误, 跳过该行: " + text);
                    continue;
                }
                if (!区服数据.TryAdd(array2[2], new IPEndPoint(区服地址, 区服端口)))
                {
                    添加日志("Server.txt 重复区服名, 已忽略: " + array2[2]);
                }
            }
            添加日志("网络配置已加载, 当前配置单\r\n" + 游戏区服);
        }

        private void 查看账号按钮_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(数据目录))
            {
                添加日志("账号目录不存在, 已自动创建");
                Directory.CreateDirectory(数据目录);
            }
            else
            {
                Process.Start("explorer.exe", 数据目录);
            }
        }

        private void 加载账号按钮_Click(object sender, EventArgs e)
        {
            账号数据 = new ConcurrentDictionary<string, 账号数据>();
            if (!Directory.Exists(数据目录))
            {
                添加日志("账号目录不存在, 已自动创建");
                Directory.CreateDirectory(数据目录);
                return;
            }
            object[] array = 序列化类.反序列化(数据目录, typeof(账号数据));
            int 丢弃数 = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is 账号数据 账号数据2)
                {
                    // W: 反序列化结果的 账号名字 可能被外部篡改, 加载时复用统一的合法性校验;
                    // 重名不再 silent overwrite, 改为 TryAdd 让后到者落空 + 计数.
                    if (!是合法账号名(账号数据2.账号名字) || !账号数据.TryAdd(账号数据2.账号名字, 账号数据2))
                    {
                        丢弃数++;
                        continue;
                    }
                }
            }
            添加日志($"账号数据已加载, 当前账号数: {账号数据.Count}, 丢弃异常账号: {丢弃数}");
            已注册账号.Text = $"已注册账号: {账号数据.Count}";
        }

        private void 主窗口_Load(object sender, EventArgs e)
        {

        }
    }
}
