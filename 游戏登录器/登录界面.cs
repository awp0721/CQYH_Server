using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
            // Settings 中的账号 / 区服字段经 DPAPI 加密保存; 老明文格式继续兼容读取.
            string 解密账号 = 解密本地字符串(Settings.Default.保存账号);
            string 解密区服 = 解密本地字符串(Settings.Default.保存区服);
            启动_选中区服标签.Text = 是合法区服名(解密区服) ? 解密区服 : "";
            // 持久化账号字段也可能被外部篡改，启动时做一次健壮性过滤
            登录_账号名字输入框.Text = 是合法用户输入(解密账号, 32) ? 解密账号 : "";
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

        // MISC-02: 客户端在 hash 之前校验明文密码强度. 服务端只看到 hash, 无法在那侧校验.
        private static bool 是强密码(string pwd)
        {
            if (string.IsNullOrEmpty(pwd)) return false;
            int 类别 = 0;
            bool 有小写 = false, 有大写 = false, 有数字 = false, 有特殊 = false;
            foreach (char c in pwd)
            {
                if (!有小写 && c >= 'a' && c <= 'z') { 有小写 = true; 类别++; }
                else if (!有大写 && c >= 'A' && c <= 'Z') { 有大写 = true; 类别++; }
                else if (!有数字 && c >= '0' && c <= '9') { 有数字 = true; 类别++; }
                else if (!有特殊 && !char.IsLetterOrDigit(c)) { 有特殊 = true; 类别++; }
                if (类别 >= 2) return true;
            }
            return false;
        }

        // 协议 v2: 密码字段在传输前先 hash, 避免端到端明文密码 + 服务端不必持有可逆密码.
        // 域分隔字符串 "YH-Auth-v2" 防止跨用途碰撞 (如签名/HMAC 复用同口令).
        // 服务端: 见 账号服务器/网络通信.cs 同名实现, 两端必须保持一致.
        public static string 密码哈希(string 账号, string 明文密码)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("YH-Auth-v2:" + 账号 + ":" + 明文密码);
            byte[] hash = SHA256.HashData(bytes);
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        // 服务端响应的密码字段在 v2 下必然是 64 char 小写 hex
        private static bool 是合法密码哈希(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length != 64)
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool ok = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
                if (!ok)
                {
                    return false;
                }
            }
            return true;
        }

        // DPAPI 包装: 当前用户作用域加密 Settings 中的敏感字段 (账号名).
        // base64(0x01 || ciphertext) 与原明文 (无 0x01 前缀) 区分, 实现旧明文向密文的兼容读取.
        private const byte DPAPI头标记 = 0x01;

        public static string 加密本地字符串(string 明文)
        {
            if (string.IsNullOrEmpty(明文))
            {
                return string.Empty;
            }
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(明文);
                byte[] enc = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                byte[] withMarker = new byte[enc.Length + 1];
                withMarker[0] = DPAPI头标记;
                Buffer.BlockCopy(enc, 0, withMarker, 1, enc.Length);
                return Convert.ToBase64String(withMarker);
            }
            catch
            {
                // DPAPI 在罕见环境下不可用 — 退回明文, 不阻塞登录流程
                return 明文;
            }
        }

        public static string 解密本地字符串(string 存储值)
        {
            if (string.IsNullOrEmpty(存储值))
            {
                return string.Empty;
            }
            try
            {
                byte[] decoded = Convert.FromBase64String(存储值);
                if (decoded.Length > 0 && decoded[0] == DPAPI头标记)
                {
                    byte[] cipher = new byte[decoded.Length - 1];
                    Buffer.BlockCopy(decoded, 1, cipher, 0, cipher.Length);
                    byte[] plain = ProtectedData.Unprotect(cipher, null, DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(plain);
                }
            }
            catch
            {
                // base64 解析失败或 DPAPI 解密失败 — 当作旧明文处理
            }
            return 存储值;
        }

        // 启动游戏前对游戏可执行文件做 Authenticode 弱模式校验：
        // - 未签名 → 跳过（兼容未签名的开发构建，fail-open）
        // - 已签名但链不合法 → 询问用户是否继续
        // - 已签名且链合法 → 通过
        // 返回 true 表示可以继续启动游戏。
        private static bool 校验游戏可执行文件(string path)
        {
            try
            {
                using (var sig = X509Certificate.CreateFromSignedFile(path))
                using (var cert2 = new X509Certificate2(sig))
                using (var chain = new X509Chain())
                {
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    if (!chain.Build(cert2))
                    {
                        DialogResult r = MessageBox.Show(
                            "游戏可执行文件签名校验失败, 可能已被篡改. 是否继续启动?",
                            "安全警告",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2);
                        return r == DialogResult.Yes;
                    }
                    return true;
                }
            }
            catch (CryptographicException)
            {
                // 未签名：保持现状，允许启动（项目可能尚未配置发布签名）
                return true;
            }
            catch
            {
                // 校验过程意外错误不阻塞启动，仅当签名明确无效时才提示
                return true;
            }
        }

        // 包号: 之前 ++封包编号 从 0 起顺序递增, 可被预测; 改为每次发包前生成 [1, int.MaxValue) 随机数.
        // 服务端原样回显, 客户端 数据接收处理 用 == 校验, 不需要服务端改动.
        private static int 下一包号()
        {
            封包编号 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            return 封包编号;
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
                        // MED-E: 服务器不再回显密码, 登录成功响应长度由 5 → 4 (区服列表在 array[3]).
                        if (array.Length != 4)
                        {
                            break;
                        }
                        用户界面解锁(null, null);
                        // 服务器回显的账号必须与发包前缓存的一致, 否则视为篡改/会话错位
                        if (!是合法用户输入(array[2], 32)
                            || string.IsNullOrEmpty(登录账号)
                            || array[2] != 登录账号)
                        {
                            登录_错误提示标签.Text = "服务器响应格式异常";
                            登录_错误提示标签.Visible = true;
                            break;
                        }
                        启动_当前账号标签.Text = 登录账号;
                        启动_选择游戏区服.Items.Clear();
                        游戏区服.Clear();
                        string[] array2 = array[3].Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
                        // 保存本地输入框的账号而非服务器回显, 并通过 DPAPI 加密
                        Settings.Default.保存账号 = 加密本地字符串(登录_账号名字输入框.Text);
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
                    // MED-E: 服务器不再回显密码, 注册成功响应长度由 4 → 3.
                    if (array.Length == 3)
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
                    // MED-E: 服务器不再回显密码, 门票响应长度由 5 → 4, 票据在 array[3].
                    if (array.Length == 4)
                    {
                        IPEndPoint value;
                        if (!File.Exists(".\\Binaries\\Win32\\MMOGame-Win32-Shipping.exe"))
                        {
                            MessageBox.Show("未找到游戏路径, 请确认登录器位置");
                            用户界面计时.Enabled = false;
                            用户界面解锁(null, null);
                        }
                        else if (!是合法门票(array[3]))
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
                            const string 游戏路径 = ".\\Binaries\\Win32\\MMOGame-Win32-Shipping.exe";
                            // 登录器以管理员权限运行, 在拉起游戏前做一次签名校验, 降低本地权限提升攻击面
                            if (!校验游戏可执行文件(游戏路径))
                            {
                                用户界面解锁(null, null);
                                break;
                            }
                            string 区服名 = 启动_选中区服标签.Text;
                            string 票据 = array[3];
                            string arguments = "-wegame=" + $"1,1,{value.Address},{value.Port}," + $"1,1,{value.Address},{value.Port}," + 区服名 + "  " + $"/ip:1,1,{value.Address} " + $"/port:{value.Port} " + "/ticket:" + 票据 + " /AreaName:" + 区服名;
                            Settings.Default.保存区服 = 加密本地字符串(区服名);
                            Settings.Default.Save();
                            游戏进程 = new Process();
                            游戏进程.StartInfo.FileName = 游戏路径;
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
            try
            {
                if (游戏进程 != null && 游戏进程.HasExited)
                {
                    用户界面解锁(null, null);
                    托盘_恢复到任务栏(null, null);
                    游戏进程监测.Enabled = false;
                }
            }
            catch
            {
                // Process 句柄异常不能让监测定时器停摆
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
            // 发包前先把账号 + 哈希存到静态字段, 服务端不再回显这两个字段:
            // 收到 case "0" 时直接复用这里固化的凭据, 后续进入游戏请求也用 登录密码 (= hash).
            登录账号 = 登录_账号名字输入框.Text;
            登录密码 = 密码哈希(登录_账号名字输入框.Text, 登录_账号密码输入框.Text);
            if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{下一包号()} 0 " + 登录账号 + " " + 登录密码)))
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
            if (!是强密码(注册_账号密码输入框.Text))
            {
                注册_错误提示标签.Text = "密码须含至少 2 种字符类型 (字母/数字/符号)";
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
            // 防止 \t \n \r \0 等控制字符塞进密码/密保字段
            if (!是合法用户输入(注册_账号密码输入框.Text, 18)
                || !是合法用户输入(注册_密保问题输入框.Text, 18)
                || !是合法用户输入(注册_密保答案输入框.Text, 18))
            {
                注册_错误提示标签.Text = "包含非法字符";
                注册_错误提示标签.Visible = true;
                return;
            }
            string 注册_密码哈希 = 密码哈希(注册_账号名字输入框.Text, 注册_账号密码输入框.Text);
            if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{下一包号()} 1 " + 注册_账号名字输入框.Text + " " + 注册_密码哈希 + " " + 注册_密保问题输入框.Text + " " + 注册_密保答案输入框.Text)))
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
            if (!是强密码(修改_账号密码输入框.Text))
            {
                修改_错误提示标签.Text = "密码须含至少 2 种字符类型 (字母/数字/符号)";
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
            // 修改密码各字段同样过滤控制字符
            if (!是合法用户输入(修改_账号名字输入框.Text, 32)
                || !是合法用户输入(修改_账号密码输入框.Text, 18)
                || !是合法用户输入(修改_密保问题输入框.Text, 18)
                || !是合法用户输入(修改_密保答案输入框.Text, 18))
            {
                修改_错误提示标签.Text = "包含非法字符";
                修改_错误提示标签.Visible = true;
                return;
            }
            string 修改_密码哈希 = 密码哈希(修改_账号名字输入框.Text, 修改_账号密码输入框.Text);
            if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{下一包号()} 2 " + 修改_账号名字输入框.Text + " " + 修改_密码哈希 + " " + 修改_密保问题输入框.Text + " " + 修改_密保答案输入框.Text)))
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
            else if (网络通信.发送数据(Encoding.UTF8.GetBytes($"{下一包号()} 3 " + 登录账号 + " " + 登录密码 + " " + 启动_选中区服标签.Text + " v1.0")))
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
            if (e.Index < 0 || e.Index >= 启动_选择游戏区服.Items.Count)
            {
                return;
            }
            using (StringFormat stringFormat = new StringFormat())
            using (SolidBrush brush = new SolidBrush(e.ForeColor))
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(启动_选择游戏区服.Items[e.Index].ToString(), e.Font, brush, e.Bounds, stringFormat);
            }
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
