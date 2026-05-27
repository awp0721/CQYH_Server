using DevExpress.Mvvm.POCO;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 游戏服务器.数据类;
using 游戏服务器.窗口视图;

namespace 游戏服务器
{
    public partial class SMainR : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static SMainR MainR;
        public List<Control> Windows = new List<Control>();

        public SMainR()
        {
            InitializeComponent();
            SMainR.MainR = this;
            this.ShowView(typeof(SystemLogView));
            //skinDropDownButtonItem1.Initialize();
            //skinDropDownButtonItem1.DropDownGallery.LookAndFeel.SkinName = "Office 2010 Blue";
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(Settings.默认皮肤);//skinName为皮肤名
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
        private void LogNavButton_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            this.ShowView(typeof(SystemLogView));
        }

        private void skinDropDownButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
        private void ChatLog_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            this.ShowView(typeof(ChatLogView));
        }
        private void Gmcommand_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            this.ShowView(typeof(CommandLogView));
        }
        private void ConfigInfo_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            this.ShowView(typeof(ConfigInfoView));
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
    }
}