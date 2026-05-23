using System;
using System.Threading;
using System.Windows.Forms;

namespace 游戏登录器
{
    internal static class Program
    {
        private static Mutex myMutex;

        [STAThread]
        private static void Main()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                // Mutex 名加当前用户 SID，避免不同账户/恶意进程抢占同名 Mutex 阻塞登录器启动
                string mutexName = "Local\\CY_Launcher_Mutex_" + (identity.User?.Value ?? "default");
                myMutex = new Mutex(initiallyOwned: false, mutexName, out var createdNew);
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(defaultValue: false);
                    Application.Run(new 登录界面());
                }
                else
                {
                    MessageBox.Show("登录器已经在运行中");
                    Environment.Exit(0);
                }
            }
            else
            {

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;

                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch
                {
                    return;
                }

                Application.Exit();
            }

        }
    }
}
