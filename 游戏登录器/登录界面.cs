using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sunny.UI;
using 游戏登录器.Properties;

namespace 游戏登录器
{

    public partial class 登录界面 : Form
    {

        public static int 封包编号;
        public static string 登录账号;
        public static string 登录密码;
        public static Process 游戏进程;
        public static 登录界面 用户界面;
        public static Dictionary<string, IPEndPoint> 游戏区服;


        public 登录界面()
        {
            InitializeComponent();
            用户界面 = this;
            网络通信.开始通信();
            游戏区服 = new Dictionary<string, IPEndPoint>();
            启动_选中区服标签.Text = 是合法区服名(Settings.Default.保存区服) ? Settings.Default.保存区服 : "";
            // 持久化账号字段也可能被外部篡改，启动时做一次健壮性过滤
            登录_账号名字输入框.Text = 是合法用户输入(Settings.Default.保存账号, 32) ? Settings.Default.保存账号 : "";
            if (!File.Exists(".\\Binaries\\Win32\\MMOGame-Win32-Shipping.exe"))
            {
                MessageBox.Show("未找到游戏路径, 请确认登录器位置");
                Environment.Exit(0);
            }
            if (!File.Exists("./ServerCfg.txt"))
            {
                MessageBox.Show("请在./ServerCfg.txt文件中配置账号服务器IP和端口");
                Environment.Exit(0);
            }
            string 配置内容;
            try
            {
                配置内容 = File.ReadAllText("./ServerCfg.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取 ServerCfg.txt 失败: " + ex.Message);
                Environment.Exit(0);
                return;
            }
            string[] array = 配置内容.Trim('\r', '\n', '\t', ' ').Split(':');
            IPAddress 服务器地址 = null;
            int 服务器端口 = 0;
            if (array.Length != 2
                || !IPAddress.TryParse(array[0], out 服务器地址)
                || !int.TryParse(array[1], out 服务器端口)
                || 服务器端口 <= 0 || 服务器端口 > 65535)
            {
                MessageBox.Show("账号服务器配置错误");
                Environment.Exit(0);
                return;
            }
            网络通信.连接地址 = new IPEndPoint(服务器地址, 服务器端口);
        }


        public void 用户界面锁定()
        {
            主选项卡.Enabled = false;
            登录_错误提示标签.Visible = false;
            注册_错误提示标签.Visible = false;
            修改_错误提示标签.Visible = false;
        }

        // 限制账号/密码这类用户输入：去掉控制字符与协议分隔符（空格），限制最大长度
        private static bool 是合法用户输入(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s) || s.Length > maxLen)
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < 0x20 || c == 0x7F || c == ' ')
                {
                    return false;
                }
            }
            return true;
        }

        // 服务器回显字符串净化：截断 + 去除控制字符，避免恶意服务器用超长串/控制字符冲击 UI
        private static string 净化显示文本(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            if (s.Length > maxLen)
            {
                s = s.Substring(0, maxLen);
            }
            StringBuilder sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c >= 0x20 && c != 0x7F)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        // 门票来自服务器响应，会被拼进游戏进程命令行参数；只允许字母数字/连字符/下划线，限长 64
        private static bool 是合法门票(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length > 64)
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool ok = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '-' || c == '_';
                if (!ok)
                {
                    return false;
                }
            }
            return true;
        }

        // 区服名同样会被拼进命令行参数；禁止空格、引号、控制字符、路径分隔符与命令行特殊符号
        private static bool 是合法区服名(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length > 32)
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < 0x20 || c == ' ' || c == '"' || c == '\'' || c == '/' || c == '\\' || c == ',' || c == ':' || c == '\t')
                {
                    return false;
                }
            }
            // 不允许以 '-' 开头，防止被游戏当成命令行开关
            if (s[0] == '-')
            {
                return false;
            }
            return true;
        }



        public void 数据接收处理(object sender, EventArgs e)
        {
            try
            {
                数据接收处理_内部();
            }
            catch
            {
                // 单包处理失败不能让定时器停摆/进程崩溃
            }
        }

        private void 数据接收处理_内部()
        {
            if (网络通信.通信实例 == null || 网络通信.接收队列.IsEmpty || !网络通信.接收队列.TryDequeue(out var result))
            {
                return;
            }
            string[] array = Encoding.UTF8.GetString(result, 0, result.Length).Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length <= 2 || !int.TryParse(array[0], out var result2) || result2 != 封包编号)
            {
                return;
            }
            switch (array[1])
            {
                case "0":
                    {
                        if (array.Length != 5)
                        {
                            break;
                        }
                        用户界面解锁(null, null);
                        string text2 = (登录账号 = (启动_当前账号标签.Text = array[2]));
                        登录密码 = array[3];
                        启动_选择游戏区服.Items.Clear();
                        游戏区服.Clear();
                        string[] array2 = array[4].Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string[] array3 = array2[i].Split(new char[2] { ',', '/' }, StringSplitOptions.RemoveEmptyEntries);
                            if (array3.Length != 3
                                || !IPAddress.TryParse(array3[0], out var 区服地址)
                                || !int.TryParse(array3[1], out var 区服端口)
                                || 区服端口 <= 0 || 区服端口 > 65535
                                || !是合法区服名(array3[2]))
                            {
                                // 单条解析失败不能让恶意/异常服务器把客户端整个干掉，跳过即可
                                continue;
                            }
                            游戏区服[array3[2]] = new IPEndPoint(区服地址, 区服端口);
                            启动_选择游戏区服.Items.Add(array3[2]);
                        }
                        主选项卡.SelectedIndex = 3;
                        // 保存本地输入框的账号而非服务器回显，避免 MITM 篡改回显毒化本地配置
                        Settings.Default.保存账号 = 登录_账号名字输入框.Text;
                        Settings.Default.Save();
                        break;
                    }
                case "1":
                    if (array.Length == 3)
                    {
                        用户界面解锁(null, null);
                        登录_错误提示标签.Text = 净化显示文本(array[2], 64);
                        登录_错误提示标签.Visible = true;
                    }
                    break;
                case "2":
                    if (array.Length == 4)
                    {
                        用户界面解锁(null, null);
                        MessageBox.Show("账号注册成功");
                    }
                    break;
                case "3":
                    if (array.Length == 3)
                    {
                        用户界面解锁(null, null);
                        注册_错误提示标签.Text = 净化显示文本(array[2], 64);
                        注册_错误提示标签.Visible = true;
                    }
                    break;
                case "4":
                    if (array.Length == 4)
                    {
                        用户界面解锁(null, null);
                        MessageBox.Show("密码修改成功");
                    }
                    break;
                case "5":
                    if (array.Length == 3)
                    {
                        用户界面解锁(null, null);
                        修改_错误提示标签.Text = 净化显示文本(array[2], 64);
                        修改_错误提示标签.Visible = true;
                    }
                    break;
                case "6":
                    if (array.Length == 5)
                    {
                        IPEndPoint value;
                        if (!File.Exists(".\\Binaries\\Win32\\MMOGame-Win32-Shipping.exe"))
                        {
                            MessageBox.Show("未找到游戏路径, 请确认登录器位置");
                            用户界面计时.Enabled = false;
                            用户界面解锁(null, null);
                        }
                        else if (!是合法门票(array[4]))
                        {
                            // 服务器返回的门票字段不可信，必须严格白名单后才能拼进游戏进程命令行
                            用户界面解锁(null, null);
                            MessageBox.Show("启动游戏失败! 门票格式非法");
                        }
                        else if (!是合法区服名(启动_选中区服标签.Text))
                        {
                            用户界面解锁(null, null);
                            MessageBox.Show("启动游戏失败! 区服名非法");
                        }
                        else if (游戏区服.TryGetValue(启动_选中区服标签.Text, out value))
                        {
                            string 区服名 = 启动_选中区服标签.Text;
                            string 票据 = array[4];
                            string arguments = "-wegame=" + $"1,1,{value.Address},{value.Port}," + $"1,1,{value.Address},{value.Port}," + 区服名 + "  " + $"/ip:1,1,{value.Address} " + $"/port:{value.Port} " + "/ticket:" + 票据 + " /AreaName:" + 区服名;
                            Settings.Default.保存区服 = 区服名;
                            Settings.Default.Save();
                            游戏进程 = new Process();
                            游戏进程.StartInfo.FileName = ".\\Binaries\\Win32\\MMOGame-Win32-Shipping.exe";
                            游戏进程.StartInfo.Arguments = arguments;
                            游戏进程.StartInfo.UseShellExecute = false;
                            游戏进程.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                            游戏进程.Start();
                            游戏进程监测.Enabled = true;
                            托盘_隐藏到托盘区(null, null);
                            用户界面锁定();
                            用户界面计时.Enabled = false;
                            最小化到托盘.ShowBalloonTip(1000, "", "正在启动游戏, 请稍候...", ToolTipIcon.Info);
                        }
                    }
                    break;
                case "7":
                    if (array.Length == 3)
                    {
                        用户界面解锁(null, null);
                        MessageBox.Show("启动游戏失败! " + 净化显示文本(array[2], 128));
                    }
                    break;
            }
        }


        public void 用户界面解锁(object sender, EventArgs e)
        {
            主选项卡.Enabled = true;
            用户界面计时.Enabled = false;
        }

        public void 游戏进程检查(object sender, EventArgs e)
        {
            if (游戏进程 != null && 游戏进程.HasExited)
            {
                用户界面解锁(null, null);
                托盘_恢复到任务栏(null, null);
                游戏进程监测.Enabled = false;
            }
        }

        private void 登录_登录账号按钮_Click(object sender, EventArgs e)
        {
            if (登录_账号名字输入框.Text.Length <= 0)
            {
                登录_错误提示标签.Text = "用户名不能为空";
                登录_错误提示标签.Visible = true;
                return;
            }
            if (登录_账号名字输入框.Text.IndexOf(' ') >= 0)
            {
                登录_错误提示标签.Text = "用户名不能包含空格";
                登录_错误提示标签.Visible = true;
                return;
            }
            if (登录_账号密码输入框.Text.Length <= 0)
            {
                登录_错误提示标签.Text = "密码不能为空";
                登录_错误提示标签.Visible = true;
                return;
            }
            if (登录_账号密码输入框.Text.IndexOf(' ') >= 0)
            {
                登录_错误提示标签.Text = "密码不能包含空格";
                登录_错误提示标签.Visible = true;
                return;
            }
            // 登录通道与注册通道使用同样的字符集/长度约束，避免在登录路径塞控制字符或超长串
            if (!是合法用户输入(登录_账号名字输入框.Text, 32) || !是合法用户输入(登录_账号密码输入框.Text, 32))
            {
                登录_错误提示标签.Text = "用户名或密码包含非法字符或过长";
                登录_错误提示标签.Visible = true;
                return;
            }
            if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{++封包编号} 0 " + 登录_账号名字输入框.Text + " " + 登录_账号密码输入框.Text)))
            {
                用户界面锁定();
            }
            登录_账号密码输入框.Text = "";
            用户界面计时.Enabled = true;
        }

        private void 登录_忘记密码选项_Click(object sender, EventArgs e)
        {
            主选项卡.SelectedIndex = 2;
        }


        private void 登录_注册账号按钮_Click(object sender, EventArgs e)
        {
            主选项卡.SelectedIndex = 1;
        }



        public void 登录_账号登录成功(string 账号, string 密码)
        {
            用户界面计时.Enabled = false;
            登录_错误提示标签.Visible = false;
            登录_登录账号按钮.Enabled = false;
            登录账号 = 账号;
            登录密码 = 密码;
            启动_当前账号标签.Text = 账号;
            主选项卡.SelectedIndex = 3;
        }

        public void 登录_账号登录失败(string 错误)
        {
            主选项卡.SelectedIndex = 0;
            用户界面计时.Enabled = false;
            登录_错误提示标签.Visible = true;
            登录_错误提示标签.Text = 错误;
            登录_登录账号按钮.Enabled = true;
        }

        private void 托盘_隐藏到托盘区(object sender, FormClosingEventArgs e)
        {
            最小化到托盘.Visible = true;
            Hide();
            if (e != null)
            {
                e.Cancel = true;
            }
        }

        private void 托盘_恢复到任务栏(object sender, MouseEventArgs e)
        {
            if (e == null || e.Button == MouseButtons.Left)
            {
                base.Visible = true;
                最小化到托盘.Visible = false;
            }
        }

        private void 托盘_恢复到任务栏(object sender, EventArgs e)
        {
            base.Visible = true;
            最小化到托盘.Visible = false;
        }

        private void 托盘_彻底关闭应用(object sender, EventArgs e)
        {
            最小化到托盘.Visible = false;
            Environment.Exit(Environment.ExitCode);
        }

        private void 注册_注册账号按钮_Click(object sender, EventArgs e)
        {
            if (注册_账号名字输入框.Text.Length <= 0)
            {
                注册_错误提示标签.Text = "用户名不能为空";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_账号名字输入框.Text.IndexOf(' ') >= 0)
            {
                注册_错误提示标签.Text = "用户名不能包含空格";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_账号名字输入框.Text.Length <= 5 || 注册_账号名字输入框.Text.Length > 12)
            {
                注册_错误提示标签.Text = "用户名长度只能为6-12位";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (!Regex.IsMatch(注册_账号名字输入框.Text, "^[a-zA-Z]+.*$"))
            {
                注册_错误提示标签.Text = "用户名只能以字母开头";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (!Regex.IsMatch(注册_账号名字输入框.Text, "^[a-zA-Z_][A-Za-z0-9_]*$"))
            {
                注册_错误提示标签.Text = "用户名只能包含字母数字和下划线";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_账号密码输入框.Text.Length <= 0)
            {
                注册_错误提示标签.Text = "密码不能为空";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_账号密码输入框.Text.IndexOf(' ') >= 0)
            {
                注册_错误提示标签.Text = "密码不能包含空格";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_账号密码输入框.Text.Length <= 5 || 注册_账号密码输入框.Text.Length > 18)
            {
                注册_错误提示标签.Text = "密码长度只能为6-18位";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_密保问题输入框.Text.Length <= 0)
            {
                注册_错误提示标签.Text = "密保问题不能为空";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_密保问题输入框.Text.IndexOf(' ') >= 0)
            {
                注册_错误提示标签.Text = "密保问题不能包含空格";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_密保问题输入框.Text.Length <= 1 || 注册_密保问题输入框.Text.Length > 18)
            {
                注册_错误提示标签.Text = "密保问题只能为2-18位";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_密保答案输入框.Text.Length <= 0)
            {
                注册_错误提示标签.Text = "密保答案不能为空";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_密保答案输入框.Text.IndexOf(' ') >= 0)
            {
                注册_错误提示标签.Text = "密保答案不能包含空格";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (注册_密保答案输入框.Text.Length <= 1 || 注册_密保答案输入框.Text.Length > 18)
            {
                注册_错误提示标签.Text = "密保问题只能为2-18位";
                注册_错误提示标签.Visible = true;
                return;
            }
            if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{++封包编号} 1 " + 注册_账号名字输入框.Text + " " + 注册_账号密码输入框.Text + " " + 注册_密保问题输入框.Text + " " + 注册_密保答案输入框.Text)))
            {
                用户界面锁定();
            }
            string text3 = (注册_账号密码输入框.Text = (注册_密保答案输入框.Text = ""));
            用户界面计时.Enabled = true;
        }

        private void 注册_返回登录按钮_Click(object sender, EventArgs e)
        {
            主选项卡.SelectedIndex = 0;
        }
        private void 修改_修改密码按钮_Click(object sender, EventArgs e)
        {
            if (修改_账号名字输入框.Text.Length <= 0)
            {
                修改_错误提示标签.Text = "用户名不能为空";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_账号名字输入框.Text.IndexOf(' ') >= 0)
            {
                修改_错误提示标签.Text = "用户名不能包含空格";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_账号密码输入框.Text.Length <= 0)
            {
                修改_错误提示标签.Text = "密码不能为空";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_账号密码输入框.Text.IndexOf(' ') >= 0)
            {
                修改_错误提示标签.Text = "密码不能包含空格";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_账号密码输入框.Text.Length <= 5 || 修改_账号密码输入框.Text.Length > 18)
            {
                修改_错误提示标签.Text = "密码长度只能为6-18位";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_密保问题输入框.Text.Length <= 0)
            {
                修改_错误提示标签.Text = "密保问题不能为空";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_密保问题输入框.Text.IndexOf(' ') >= 0)
            {
                修改_错误提示标签.Text = "密保问题不能包含空格";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_密保答案输入框.Text.Length <= 0)
            {
                修改_错误提示标签.Text = "密保答案不能为空";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (修改_密保答案输入框.Text.IndexOf(' ') >= 0)
            {
                修改_错误提示标签.Text = "密保答案不能包含空格";
                修改_错误提示标签.Visible = true;
                return;
            }
            if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{++封包编号} 2 " + 修改_账号名字输入框.Text + " " + 修改_账号密码输入框.Text + " " + 修改_密保问题输入框.Text + " " + 修改_密保答案输入框.Text)))
            {
                用户界面锁定();
            }
            string text3 = (修改_账号密码输入框.Text = (修改_密保答案输入框.Text = ""));
            用户界面计时.Enabled = true;
        }

        private void 修改_返回登录按钮_Click(object sender, EventArgs e)
        {
            主选项卡.SelectedIndex = 0;
        }

        private void 启动_进入游戏按钮_Click(object sender, EventArgs e)
        {
            if (登录账号 == null || 登录账号 == "")
            {
                主选项卡.SelectedIndex = 0;
            }
            else if (启动_选中区服标签.Text == null || 启动_选中区服标签.Text == "")
            {
                MessageBox.Show("请选择服务器");
            }
            else if (!游戏区服.ContainsKey(启动_选中区服标签.Text))
            {
                MessageBox.Show("服务器选择错误");
            }
            else if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{++封包编号} 3 " + 登录账号 + " " + 登录密码 + " " + 启动_选中区服标签.Text + " v1.0")))
            {
                用户界面锁定();
                用户界面计时.Enabled = true;
            }
        }

        private void 启动_注销账号标签_Click(object sender, EventArgs e)
        {
            登录账号 = null;
            登录密码 = null;
            主选项卡.SelectedIndex = 0;
        }

        private void 启动_选择游戏区服_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(启动_选择游戏区服.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds, stringFormat);
        }

        private void 启动_选择游戏区服_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (启动_选择游戏区服.SelectedIndex < 0)
            {
                启动_选中区服标签.Text = "";
            }
            else
            {
                启动_选中区服标签.Text = 启动_选择游戏区服.Items[启动_选择游戏区服.SelectedIndex].ToString();
            }
        }

        private void 登录界面_Load(object sender, EventArgs e)
        {

        }
    }
}
