using System;
using System.Threading;
using System.Windows.Forms;

namespace 账号服务器
{
	internal static class Program
	{
		private static Mutex myMutex;

		[STAThread]
		private static void Main()
		{
			// OO: Mutex 名加用户 SID, 防止同机器其他账户创建同名 Mutex 阻塞服务启动 (与客户端对齐)
			System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
			string mutexName = "Local\\CY_LoginServer_Mutex_" + (identity.User?.Value ?? "default");
			myMutex = new Mutex(initiallyOwned: false, mutexName, out var createdNew);
			if (createdNew)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(defaultValue: false);
				Application.Run(new 主窗口());
			}
			else
			{
				MessageBox.Show("服务器已经在运行中");
				Environment.Exit(0);
			}
		}
	}
}
