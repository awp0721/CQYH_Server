using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 游戏服务器.窗口视图;
using 游戏服务器.地图类;
using 游戏服务器.管理命令;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using System.Windows.Controls.Ribbon;
using RibbonControl = DevExpress.XtraBars.Ribbon.RibbonControl;

namespace 游戏服务器
{
    public partial class SMain : RibbonForm
    {

        private struct PeekMsg
        {
            private readonly IntPtr hWnd;

            private readonly Message msg;

            private readonly IntPtr wParam;

            private readonly IntPtr lParam;

            private readonly uint time;

            private readonly Point p;
        }

        public static SMain Main;
        public static DataTable 技能数据表;

        public static DataTable 装备数据表;

        public static DataTable 背包数据表;

        public static DataTable 仓库数据表;

        public static DataTable 地图数据表;

        public static DataTable 怪物数据表;

        public static DataTable 掉落数据表;

        public static DataTable 封禁数据表;

        public List<Control> Windows = new List<Control>();

        private static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !SMain.PeekMessage(out msg, IntPtr.Zero, 0u, 0u, 0u);
            }
        }

        public SMain()
        {
            InitializeComponent();
            SMain.Main = this;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            this.ShowView(typeof(SystemLogView));
            主程.添加系统日志("Elaina Engine (伊蕾娜引擎)");
            主程.添加系统日志("源码仓库: https://github.com/awp0721/CQYH_Server");
            ribbonControl1.Enabled = false;
            navBarControl1.Enabled = false;
            Task.Run(delegate
            {
                Thread.Sleep(100);
                主程.添加系统日志("正在加载系统数据...");
                系统数据网关.加载数据();
                主程.添加系统日志("系统数据加载完成");
                主程.添加系统日志("正在初始化脚本系统...");
                游戏脚本.初始化脚本系统();
                主程.添加系统日志("脚本系统初始化完成");
                主程.添加系统日志("正在加载客户数据...");
                游戏数据网关.加载数据();
                SMain.加载客户数据();
                主程.添加系统日志("客户数据加载完成");

                BeginInvoke(new MethodInvoker(delegate ()
                {
                    navBarControl1.Enabled = true;
                    ribbonControl1.Enabled = true;
                }));
                this.Loaded = true;
                this.UpdateInterface();
            });
            this.UpdateTitleText();
            Settings.OnSaved += delegate
            {
                this.UpdateTitleText();
            };
            Settings.OnLoaded += delegate
            {
                this.UpdateTitleText();
            };
        }

        private void UpdateTitleText()
        {
            if (this.TileText == null)
            {
                this.TileText = this.Text;
            }
            this.Text = this.TileText.Replace("%D%", DateTime.Parse("2000/1/1").AddDays(Assembly.GetExecutingAssembly().GetName().Version.Build).ToString("d") + " " + DateTime.Parse("2000/1/1").AddSeconds(Assembly.GetExecutingAssembly().GetName().Version.Revision * 2).ToString("T")) + " " + Settings.游戏区服名称;
        }

        public static void 加载客户数据()
        {
            SMain.技能数据表 = new DataTable("技能数据表");
            SMain.装备数据表 = new DataTable("装备数据表");
            SMain.背包数据表 = new DataTable("装备数据表");
            SMain.仓库数据表 = new DataTable("装备数据表");
            SMain.技能数据表.Columns.Add("技能名字", typeof(string));
            SMain.技能数据表.Columns.Add("技能编号", typeof(string));
            SMain.技能数据表.Columns.Add("当前等级", typeof(string));
            SMain.技能数据表.Columns.Add("当前经验", typeof(string));
            SMain.装备数据表.Columns.Add("穿戴部位", typeof(string));
            SMain.装备数据表.Columns.Add("穿戴装备", typeof(string));
            SMain.装备数据表.Columns.Add("持久", typeof(string));
            SMain.装备数据表.Columns.Add("物品归属", typeof(string));
            SMain.装备数据表.Columns.Add("掉落怪物", typeof(string));
            SMain.装备数据表.Columns.Add("掉落地图", typeof(string));
            SMain.装备数据表.Columns.Add("掉落时间", typeof(string));
            SMain.背包数据表.Columns.Add("背包位置", typeof(string));
            SMain.背包数据表.Columns.Add("背包物品", typeof(string));
            SMain.背包数据表.Columns.Add("数量", typeof(string));
            SMain.背包数据表.Columns.Add("物品归属", typeof(string));
            SMain.背包数据表.Columns.Add("掉落怪物", typeof(string));
            SMain.背包数据表.Columns.Add("掉落地图", typeof(string));
            SMain.背包数据表.Columns.Add("掉落时间", typeof(string));
            SMain.仓库数据表.Columns.Add("仓库位置", typeof(string));
            SMain.仓库数据表.Columns.Add("仓库物品", typeof(string));
            SMain.仓库数据表.Columns.Add("数量", typeof(string));
            SMain.仓库数据表.Columns.Add("物品归属", typeof(string));
            SMain.仓库数据表.Columns.Add("掉落怪物", typeof(string));
            SMain.仓库数据表.Columns.Add("掉落地图", typeof(string));
            SMain.仓库数据表.Columns.Add("掉落时间", typeof(string));
            SMain.封禁数据表 = new DataTable();
            SMain.封禁数据表.Columns.Add("网络地址", typeof(string));
            SMain.封禁数据表.Columns.Add("物理地址", typeof(string));
            SMain.封禁数据表.Columns.Add("到期时间", typeof(string));

        }

        private void SMain_Load(object sender, EventArgs e)
        {
            this.UpdateInterface();
            Application.Idle += Application_Idle;
            //navBarControl1.Enabled = false;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (XtraMessageBox.Show("是否确定关闭服务器？", "关闭服务器", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else if (主程.主线程 != null)
            {
                主程.停止服务();
                while (主程.主线程 != null)
                {
                    Thread.Sleep(1);
                }
                if (主程.已经启动)
                {
                    Thread.Sleep(5000);
                    游戏数据网关.强制保存();
                    Thread.Sleep(5000);
                    游戏数据网关.导出数据();
                }
            }
        }

        public void ShowView(Type type)
        {
            foreach (Control window in this.Windows)
            {
                if (window.GetType() == type)
                {
                    this.tabbedView1.ActivateDocument(window);
                    return;
                }
            }
            Form form;
            form = (Form)Activator.CreateInstance(type);
            form.MdiParent = this;
            form.Disposed += View_Disposed;
            form.Tag = type.Name;
            this.Windows.Add(form);
            form.Show();
        }

        private void View_Disposed(object sender, EventArgs e)
        {
            this.Windows.Remove((Control)sender);
        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateInterface();
            if (!主程.已经启动 && 主程.主线程 == null)
            {
                this.InterfaceTimer.Enabled = false;
            }
        }

        private void UpdateInterface()
        {
            this.StartServerButton.Enabled = 主程.主线程 == null && this.Loaded;
            this.StopServerButton.Enabled = 主程.已经启动 && this.Loaded;
            if (主程.已经启动)
            {
                this.ConnectionLabel.Caption = $"连接数: {网络服务网关.网络连接表.Count:#,##0}";
                this.ObjectLabel.Caption = $"玩家数: {地图处理网关.玩家对象表.Count:#,##0}";
                if ((decimal)网络服务网关.已接收字节数 > 1073741824m)
                {
                    this.TotalDownloadLabel.Caption = $"已下载: {(decimal)网络服务网关.已接收字节数 / 1073741824m:#,##0.0}GB";
                }
                else if ((decimal)网络服务网关.已接收字节数 > 1048576m)
                {
                    this.TotalDownloadLabel.Caption = $"已下载: {(decimal)网络服务网关.已接收字节数 / 1048576m:#,##0.0}MB";
                }
                else if ((decimal)网络服务网关.已接收字节数 > 1024m)
                {
                    this.TotalDownloadLabel.Caption = $"已下载: {(decimal)网络服务网关.已接收字节数 / 1024m:#,##0}KB";
                }
                else
                {
                    this.TotalDownloadLabel.Caption = $"已下载: {网络服务网关.已接收字节数:#,##0}B";
                }
                if ((decimal)网络服务网关.已发送字节数 > 1073741824m)
                {
                    this.TotalUploadLabel.Caption = $"已上传: {(decimal)网络服务网关.已发送字节数 / 1073741824m:#,##0.0}GB";
                }
                else if ((decimal)网络服务网关.已发送字节数 > 1048576m)
                {
                    this.TotalUploadLabel.Caption = $"已上传: {(decimal)网络服务网关.已发送字节数 / 1048576m:#,##0.0}MB";
                }
                else if ((decimal)网络服务网关.已发送字节数 > 1024m)
                {
                    this.TotalUploadLabel.Caption = $"已上传: {(decimal)网络服务网关.已发送字节数 / 1024m:#,##0}KB";
                }
                else
                {
                    this.TotalUploadLabel.Caption = $"已上传: {网络服务网关.已发送字节数:#,##0}B";
                }
                this.scriptMemory.Caption = "lua 内存:" + 游戏脚本.scriptMemoryUseage + " KB";
            }
        }

        private void StartServerButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.InterfaceTimer.Enabled = true;
                主程.启动服务();
                this.UpdateInterface();
            }
            catch (Exception ex)
            {
                主程.添加系统日志("例外: " + ex.ToString());
            }
        }

        private void StopServerButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SMain.GetHardDiskFreeSpace(Path.GetPathRoot(Application.ExecutablePath)) < 102400L)
            {
                MessageBox.Show("磁盘剩余空间不足,请先清理磁盘");
                return;
            }
            主程.停止服务();
            this.UpdateInterface();
        }

        public static void SetUpView(GridView view)
        {
            view.BestFitColumns();
            view.KeyPress += PasteData_KeyPress;
            view.KeyDown += DeleteRows_KeyDown;
            view.OptionsSelection.MultiSelect = true;
            view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }

        private static void DeleteRows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                MessageBox.Show("删除行", "确认", MessageBoxButtons.YesNo);
            }
        }

        public static void PasteData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\u0016')
            {
                return;
            }
            e.Handled = true;
            GridView gridView;
            gridView = (GridView)sender;
            string[] array;
            array = Clipboard.GetText().Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int[] selectedRows;
            selectedRows = gridView.GetSelectedRows();
            for (int i = 0; i < selectedRows.Length && i + 1 < array.Length; i++)
            {
                string[] array2;
                array2 = array[i + 1].Split('\t');
                if (gridView.GetSelectedCells(selectedRows[i]).Length != array2.Length)
                {
                    XtraMessageBox.Show("列计数与复制列计数不匹配");
                    break;
                }
            }
        }

        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            long result;
            result = 0L;
            if (!str_HardDiskName.EndsWith(":\\"))
            {
                str_HardDiskName += ":\\";
            }
            DriveInfo[] drives;
            drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                if (driveInfo.Name.Equals(str_HardDiskName, StringComparison.OrdinalIgnoreCase))
                {
                    result = driveInfo.TotalSize;
                }
            }
            return result;
        }

        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long result;
            result = 0L;
            if (!str_HardDiskName.EndsWith(":\\"))
            {
                str_HardDiskName += ":\\";
            }
            DriveInfo[] drives;
            drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                if (driveInfo.Name.Equals(str_HardDiskName, StringComparison.OrdinalIgnoreCase))
                {
                    result = driveInfo.TotalFreeSpace;
                }
            }
            return result;
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        private void LogNavButton_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            this.ShowView(typeof(SystemLogView));
        }

        private void navBarItem1_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            this.ShowView(typeof(ChatLogView));
        }

        private void navBarItem2_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(MagicInfoView));
            }
        }

        private void navBarItem4_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(BuffInfoView));
            }
        }

        private void navBarItem3_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(RuneInfoView));
            }
        }

        private void navBarItem5_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(TrapInfoView));
            }
        }

        private void navBarItem8_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(ItemInfoView));
            }
        }

        private void navBarItem7_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(MobInfoView));
            }
        }

        private void navBarItem13_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(CommandLogView));
            }
        }

        private void navBarItem15_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(ConfigInfoView));
            }
        }

        private void repositoryItemTextEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void barEditItem2_HiddenEditor(object sender, ItemClickEventArgs e)
        {
            if (((string)this.barEditItem2.EditValue).Length <= 0)
            {
                return;
            }
            主程.添加命令日志("=> " + (string)this.barEditItem2.EditValue);
            GM命令 命令;
            if (((string)this.barEditItem2.EditValue)[0] != '@')
            {
                主程.添加命令日志("<= 命令解析错误, GM命令必须以 '@' 开头. 输入 '@查看命令' 获取所有受支持的命令格式");
            }
            else if (((string)this.barEditItem2.EditValue).Trim('@', ' ').Length == 0)
            {
                主程.添加命令日志("<= 命令解析错误, GM命令不能为空. 输入 '@查看命令' 获取所有受支持的命令格式");
            }
            else if (GM命令.解析命令((string)this.barEditItem2.EditValue, out 命令))
            {
                if (命令.执行方式 == 执行方式.前台立即执行)
                {
                    命令.执行命令();
                }
                else if (命令.执行方式 == 执行方式.优先后台执行)
                {
                    if (主程.已经启动)
                    {
                        主程.外部命令.Enqueue(命令);
                    }
                    else
                    {
                        命令.执行命令();
                    }
                }
                else if (命令.执行方式 == 执行方式.只能后台执行)
                {
                    if (主程.已经启动)
                    {
                        主程.外部命令.Enqueue(命令);
                    }
                    else
                    {
                        主程.添加命令日志("<= 命令执行失败, 当前命令只能在服务器运行时执行, 请先启动服务器");
                    }
                }
                else if (命令.执行方式 == 执行方式.只能空闲执行)
                {
                    if (!主程.已经启动 && (主程.主线程 == null || !主程.主线程.IsAlive))
                    {
                        命令.执行命令();
                    }
                    else
                    {
                        主程.添加命令日志("<= 命令执行失败, 当前命令只能在服务器未运行时执行, 请先关闭服务器");
                    }
                }
            }
            this.barEditItem2.EditValue = "";
        }

        private void ReadLoadMapButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            地图处理网关.重载地图(完全更新: true);
        }

        private void navBarItem11_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(PlayerView));
            }
        }

        public static void UpdateDelay(int 激活对象, int 次要对象, int 对象总数)
        {
            SMain.Main?.BeginInvoke((MethodInvoker)delegate
            {
                SMain.Main.LoopLabel.Caption = $" 激活对象: {激活对象}  次要对象: {次要对象}  对象总数: {对象总数}  ";
            });
        }

        public static void UpdateTick(long elapsedMilliseconds)
        {
            SMain.Main?.BeginInvoke((MethodInvoker)delegate
            {
                SMain.Main.ProcessLabel.Caption = $"帧数:{elapsedMilliseconds}";
            });
        }

        private void navBarItem16_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(守卫视图窗口));
            }
        }

        public void 允许重载()
        {
            this.ribbonPageGroup3.Enabled = true;
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e) //重载守卫
        {
            this.ribbonPageGroup3.Enabled = false;
            系统数据网关.加载数据(int.Parse(e.Item.Tag.ToString()));
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ShowView(typeof(CommandLogView));
            this.barEditItem2_HiddenEditor(null, null);
        }

        private void navBarItem17_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(称号视图窗口));
            }
        }

        private void navBarItem18_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(刷怪视图窗口));
            }
        }

        private void navBarItem14_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (this.Loaded)
            {
                this.ShowView(typeof(公告视图窗口));
            }
        }

        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (主程.已经启动)
            {
                MessageBox.Show("合并数据只能在服务器未运行时执行");
                return;
            }
            Dictionary<Type, 数据表基类> 数据类型表;
            数据类型表 = 游戏数据网关.数据类型表;
            if (数据类型表 != null && 数据类型表.Count != 0)
            {
                if (!Directory.Exists(this.barEditItem3.EditValue.ToString()))
                {
                    MessageBox.Show("请选择有效的 Data.db 文件目录");
                }
                else if (!File.Exists(this.barEditItem3.EditValue.ToString() + "\\Data.db"))
                {
                    MessageBox.Show("选择的目录中没有找到 Data.db 文件");
                }
                else if (MessageBox.Show("即将执行数据合并操作\r\n\r\n此操作不可逆, 请做好数据备份\r\n\r\n确定要执行吗?", "危险操作", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    游戏数据网关.合并数据(this.barEditItem3.EditValue.ToString() + "\\Data.db", this.barEditItem4.EditValue.ToString());
                }
            }
            else
            {
                MessageBox.Show("需要先加载当前客户数据后才能与指定客户数据合并");
            }
        }

        private void barEditItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void barEditItem3_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
        {
        }

        private void barEditItem_ShowingEditor(object sender, ItemCancelEventArgs e)
        {
            if (sender is BarEditItem barEditItem)
            {
                FolderBrowserDialog folderBrowserDialog;
                folderBrowserDialog = new FolderBrowserDialog
                {
                    Description = "请选择文件夹"
                };
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    barEditItem.EditValue = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void barStaticItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start("explorer", "http://www.baidu.com/");
        }

        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {


        }

        private void ReLoadLuaButton_ItemClick(object sender, EventArgs e)
        {
            主程.重载任务列表.Enqueue(delegate
            {
                主程.ReloadNPCs();
            });
            this.ShowView(typeof(SystemLogView));
        }

        private void buttonEdit_ReloadQF_Click(object sender, EventArgs e)
        {
            if (主程.DefaultNPC != null)
            {
                主程.重载任务列表.Enqueue(delegate
                {
                    主程.ReloadNPCs(new int[1] { 主程.DefaultNPC.ScriptID });
                });
            }
            this.ShowView(typeof(SystemLogView));
        }

        private void buttonEdit_ReloadNpcById_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                string text;
                text = this.buttonEdit_ReloadNpcById.EditValue?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }
                string[] array;
                array = text.Split(new char[6] { ',', '|', '.', ';', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int[] arr;
                arr = (from t in array
                       where int.TryParse(t, out var _)
                       select int.Parse(t)).ToArray();
                if (arr.Length < array.Length)
                {
                    主程.添加系统日志("输入的NPC编号有错误,请输入整数");
                    return;
                }
                arr = (from n in 地图处理网关.守卫对象表.Values.ToList()
                       where arr.Contains(n.对象模板.守卫编号)
                       select n.ScriptID).ToArray();
                if (!arr.Any())
                {
                    主程.添加系统日志("没有找到需要重载的NPC,请确认NPC编号正确且已配置刷新");
                    return;
                }
                if (arr.Length < array.Length)
                {
                    主程.添加系统日志("输入的NPC编号有部分没有配置NPC刷新,具体请核对");
                }
                主程.重载任务列表.Enqueue(delegate
                {
                    主程.ReloadNPCs(arr);
                });
            }
            catch (Exception value)
            {
                主程.添加系统日志($"重载指定NPC异常:{value}");
            }
        }

        private void barButtonItem_ResetClientFeeHttp_ItemClick(object sender, ItemClickEventArgs e)
        {
            //网络服务网关.RestartHttpService();
        }

        private void barButtonItem_ResetRechargeHttp_ItemClick(object sender, ItemClickEventArgs e)
        {
            //网络服务网关.RestartHttpService();
        }

        private void barButtonItem_ItemClick(object sender, EventArgs e)
        {
            try
            {
                this.ShowView(typeof(SystemLogView));
                if (sender == this.游戏商店Button)
                {
                    游戏商店.载入数据();
                    珍宝商品.载入数据();
                    主程.添加系统日志("重载 游戏商店 珍宝商店 成功");

                }
                else if (sender == this.游戏称号Button)
                {
                    游戏称号.载入数据();
                    主程.添加系统日志("重载 称号 成功");
                }
                else if (sender == this.重载怪物Button)
                {
                    游戏怪物.载入数据();
                    this.RefeshMonster();
                    主程.添加系统日志("重载 怪物信息 成功");
                }
                else if (sender == this.坐骑数据Button)
                {
                    游戏坐骑.载入数据();
                    主程.添加系统日志("重载 坐骑 成功");
                }
            }
            catch (Exception ex)
            {
                主程.添加系统日志("重载失败:" + ex.Message);
            }
        }

        private void RefeshMonster()
        {
            foreach (怪物实例 item in 地图处理网关.怪物对象表.Values.ToList())
            {
                if (!游戏怪物.数据表.TryGetValue(item.对象模板.怪物名字, out var value))
                {
                    continue;
                }
                item.对象模板 = value;
                item.属性加成[item] = value.基础属性;
                item.更新对象属性();
                string 普通攻击技能;
                普通攻击技能 = value.普通攻击技能;
                if (普通攻击技能 != null && 普通攻击技能.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value.普通攻击技能, out item.普通攻击技能);
                }
                else
                {
                    item.普通攻击技能 = null;
                }
                List<游戏技能> list;
                list = new List<游戏技能>();
                string[] 概率触发技能;
                概率触发技能 = value.概率触发技能;
                foreach (string text in 概率触发技能)
                {
                    if (text != null && text.Length > 0)
                    {
                        游戏技能.数据表.TryGetValue(text, out var value2);
                        list.Add(value2);
                    }
                }
                item.概率触发技能 = list;
                string 进入战斗技能;
                进入战斗技能 = value.进入战斗技能;
                if (进入战斗技能 != null && 进入战斗技能.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value.进入战斗技能, out item.进入战斗技能);
                }
                else
                {
                    item.进入战斗技能 = null;
                }
                string 退出战斗技能;
                退出战斗技能 = value.退出战斗技能;
                if (退出战斗技能 != null && 退出战斗技能.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value.退出战斗技能, out item.退出战斗技能);
                }
                else
                {
                    item.退出战斗技能 = null;
                }
                string 死亡释放技能;
                死亡释放技能 = value.死亡释放技能;
                if (死亡释放技能 != null && 死亡释放技能.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value.死亡释放技能, out item.死亡释放技能);
                }
                else
                {
                    item.死亡释放技能 = null;
                }
                string 移动释放技能;
                移动释放技能 = value.移动释放技能;
                if (移动释放技能 != null && 移动释放技能.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value.移动释放技能, out item.移动释放技能);
                }
                else
                {
                    item.移动释放技能 = null;
                }
                string 出生释放技能;
                出生释放技能 = value.出生释放技能;
                if (出生释放技能 != null && 出生释放技能.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value.出生释放技能, out item.出生释放技能);
                }
                else
                {
                    item.出生释放技能 = null;
                }
            }
            foreach (宠物实例 item2 in 地图处理网关.宠物对象表.Values.ToList())
            {
                if (!游戏怪物.数据表.TryGetValue(item2.对象模板.怪物名字, out var value3))
                {
                    continue;
                }
                item2.对象模板 = value3;
                item2.属性加成[item2] = value3.成长属性[Math.Min(value3.成长属性.Length - 1, item2.宠物等级)];
                item2.更新对象属性();
                string 普通攻击技能2;
                普通攻击技能2 = value3.普通攻击技能;
                if (普通攻击技能2 != null && 普通攻击技能2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value3.普通攻击技能, out item2.普通攻击技能);
                }
                else
                {
                    item2.普通攻击技能 = null;
                }
                List<游戏技能> list2;
                list2 = new List<游戏技能>();
                string[] 概率触发技能;
                概率触发技能 = value3.概率触发技能;
                foreach (string text2 in 概率触发技能)
                {
                    if (text2 != null && text2.Length > 0)
                    {
                        游戏技能.数据表.TryGetValue(text2, out var value4);
                        list2.Add(value4);
                    }
                }
                item2.概率触发技能 = list2;
                string 进入战斗技能2;
                进入战斗技能2 = value3.进入战斗技能;
                if (进入战斗技能2 != null && 进入战斗技能2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value3.进入战斗技能, out item2.进入战斗技能);
                }
                else
                {
                    item2.进入战斗技能 = null;
                }
                string 退出战斗技能2;
                退出战斗技能2 = value3.退出战斗技能;
                if (退出战斗技能2 != null && 退出战斗技能2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value3.退出战斗技能, out item2.退出战斗技能);
                }
                else
                {
                    item2.退出战斗技能 = null;
                }
                string 死亡释放技能2;
                死亡释放技能2 = value3.死亡释放技能;
                if (死亡释放技能2 != null && 死亡释放技能2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value3.死亡释放技能, out item2.死亡释放技能);
                }
                else
                {
                    item2.死亡释放技能 = null;
                }
                string 移动释放技能2;
                移动释放技能2 = value3.移动释放技能;
                if (移动释放技能2 != null && 移动释放技能2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value3.移动释放技能, out item2.移动释放技能);
                }
                else
                {
                    item2.移动释放技能 = null;
                }
                string 出生释放技能2;
                出生释放技能2 = value3.出生释放技能;
                if (出生释放技能2 != null && 出生释放技能2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(value3.出生释放技能, out item2.出生释放技能);
                }
                else
                {
                    item2.出生释放技能 = null;
                }
            }
        }

        private void navBarItem12_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
        }

        private void barButtonItem_ReloadList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void navBarControl1_Click(object sender, EventArgs e)
        {

        }

        private void buttonEdit_ReloadAllNPC_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void buttonEdit_ReloadMonster_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void 重载怪物Button_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (主程.已经启动)
            {
                Task.Run(delegate ()
                {
                    Thread.Sleep(100);
                    BeginInvoke(new MethodInvoker(delegate ()
                    {
                        主程.添加系统日志("怪物信息 怪物刷新 重载中请稍候...");

                    }));
                    游戏怪物.载入数据();
                    this.RefeshMonster();
                    系统数据网关.加载数据(16); //怪物刷新重载
                    主程.添加系统日志("重载 怪物信息 怪物刷新 成功");
                });
            }
            else
            {
                MessageBox.Show("服务器未启动，无法重载怪物！");
            }

        }

        private void 游戏商店Button_ItemClick(object sender, ItemClickEventArgs e)
        {
            /*
            Task.Run(delegate ()
            {
                Thread.Sleep(100);
                BeginInvoke(new MethodInvoker(delegate ()
                {

                }));
            });
            */
            Task.Run(delegate ()
            {
                Thread.Sleep(100);
                BeginInvoke(new MethodInvoker(delegate ()
                {
                    主程.添加系统日志("游戏商店 珍宝商店 重载中请稍候...");

                }));
                游戏商店.载入数据();
                珍宝商品.载入数据();
                主程.添加系统日志("重载 游戏商店 珍宝商店 成功");
            });

        }

        private void 游戏称号Button_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(delegate ()
            {
                Thread.Sleep(100);
                BeginInvoke(new MethodInvoker(delegate ()
                {
                    主程.添加系统日志("称号 重载中请稍候...");

                }));

                游戏称号.载入数据();
                主程.添加系统日志("重载 称号 成功");
            });

        }

        private void 坐骑数据Button_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(delegate ()
            {
                Thread.Sleep(100);
                BeginInvoke(new MethodInvoker(delegate ()
                {
                    主程.添加系统日志("坐骑 重载中请稍候...");

                }));
                游戏坐骑.载入数据();
                主程.添加系统日志("重载 坐骑 成功");
            });

        }

        private void ReLoadLuaButton_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void 怪物爆率Button_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(delegate ()
            {
                Thread.Sleep(100);
                BeginInvoke(new MethodInvoker(delegate ()
                {
                    主程.添加系统日志("爆率 重载中请稍候...");

                }));
                this.barEditItem2.EditValue = "@重载爆率";
                this.barEditItem2_HiddenEditor(null, null);
            });

        }
    }
}
