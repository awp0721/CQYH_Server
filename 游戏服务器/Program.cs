using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace 游戏服务器
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] str1)
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.SetCompatibleTextRenderingDefault(defaultValue: false);
            Application.ThreadException += _0010_0013_0005_0007_0001_0009;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += _0001_0003_0009_000B_000D_0008_000E_000D;
            Settings.Load();
            if (str1.Length != 0 && str1[0] == "old")
            {
                主程.OldForm = true;
                Application.Run(new 主窗口());
            }
            else
            {
                Application.Run(new SMain());
            }
        }

        private static void _0010_0013_0005_0007_0001_0009(object _000C_000A_000E_0008_0003_000A_0002_0003, ThreadExceptionEventArgs _0005_0008_0005_000B_0009_0003_000B_0002)
        {
            Program._0005_0007_0012_0013_0006_0007_0003_0003("Form1_UIThreadException:\r\n" + _0005_0008_0005_000B_0009_0003_000B_0002.Exception.ToString() + "\r\n");
            MessageBox.Show("An application error occurred.\r\n" + _0005_0008_0005_000B_0009_0003_000B_0002.Exception.ToString(), "UIThreadException", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private static void _0001_0003_0009_000B_000D_0008_000E_000D(object _000C_000A_000E_0008_0003_000A_0002_0003, UnhandledExceptionEventArgs _000F_0004_000E_0003_0014_000A)
        {
            string text;
            text = _000F_0004_000E_0003_0014_000A.ExceptionObject.ToString();
            Program._0005_0007_0012_0013_0006_0007_0003_0003("CurrentDomain_UnhandledException:\r\n" + text + "\r\n");
            MessageBox.Show("An application error occurred.Terminating=" + _000F_0004_000E_0003_0014_000A.IsTerminating + "\r\n" + text, "UnhandledException", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private static void _0005_0007_0012_0013_0006_0007_0003_0003(string _000F_0011_0001_000F_000C_0012)
        {
            try
            {
                string text;
                text = Path.Combine(Application.StartupPath, "Logs");
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                string path;
                path = Path.Combine(text, "unhandleEx.txt");
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "");
                }
                File.AppendAllText(path, _000F_0011_0001_000F_000C_0012);
            }
            catch (Exception)
            {
            }
        }
    }
}
