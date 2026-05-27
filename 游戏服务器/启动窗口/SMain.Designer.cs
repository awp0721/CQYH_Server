using DevExpress.DataAccess.Native.Data;
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
using DevExpress.XtraNavBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 游戏服务器
{
    partial class SMain
    {

        public bool Loaded;

        private string TileText;

        private IContainer components;

        private DefaultLookAndFeel DLookAndFeel;

        private RibbonControl ribbonControl1;

        private RibbonPage ribbonPage1;

        private RibbonPageGroup ribbonPageGroup1;

        private RibbonPage ribbonPage2;

        private RibbonPageGroup ribbonPageGroup2;

        private NavBarControl navBarControl1;

        private NavBarGroup navBarGroup1;

        private DocumentManager DManager;

        private TabbedView tabbedView1;

        private BarButtonItem StartServerButton;

        private BarButtonItem StopServerButton;

        private NavBarItem LogNavButton;

        private NavBarItem navBarItem1;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlTop;

        private Bar bar3;

        private BarStaticItem ConnectionLabel;

        private System.Windows.Forms.Timer InterfaceTimer;

        private BarStaticItem ObjectLabel;

        private BarStaticItem ProcessLabel;

        private BarStaticItem LoopLabel;

        private BarStaticItem TotalDownloadLabel;

        private BarStaticItem TotalUploadLabel;

        private NavBarGroup navBarGroup2;

        private NavBarGroup navBarGroup3;

        private BarStaticItem ConDelay;

        private NavBarItem navBarItem6;

        private NavBarItem navBarItem2;

        private NavBarItem navBarItem3;

        private NavBarItem navBarItem4;

        private NavBarItem navBarItem5;

        private NavBarItem navBarItem7;

        private NavBarItem navBarItem8;

        private NavBarItem navBarItem10;

        private NavBarItem navBarItem11;

        private NavBarItem navBarItem12;

        private NavBarItem navBarItem13;

        private NavBarItem navBarItem14;

        private BarEditItem barEditItem1;

        private RepositoryItemButtonEdit repositoryItemButtonEdit1;

        private BarEditItem barEditItem2;

        private RepositoryItemTextEdit repositoryItemTextEdit1;

        private BarButtonItem barButtonItem1;

        private NavBarItem navBarItem15;

        private BarButtonItem ReLoadLuaButton;

        private BarButtonItem ReadLoadMapButton;

        private NavBarItem navBarItem16;

        private RibbonPage 重载页;

        private RibbonPageGroup ribbonPageGroup3;

        private BarButtonItem barButtonItem2;

        private BarButtonItem barButtonItem3;

        private BarButtonItem barButtonItem4;

        private BarButtonItem barButtonItem6;

        private BarButtonItem barButtonItem7;

        private BarButtonItem barButtonItem8;

        private BarButtonItem barButtonItem9;

        private BarButtonItem barButtonItem10;

        private BarButtonItem barButtonItem5;

        private BarButtonItem 怪物爆率Button;

        private NavBarItem navBarItem17;

        private NavBarItem navBarItem18;

        private BarStaticItem scriptMemory;

        private BarButtonItem barButtonItem12;

        private BarButtonItem barButtonItem13;

        private RibbonPage ribbonPage3;

        private RibbonPageGroup ribbonPageGroup4;

        private RibbonPage ribbonPage4;

        private BarEditItem barEditItem3;

        private RepositoryItemButtonEdit repositoryItemButtonEdit2;

        private BarButtonItem barButtonItem14;

        private RibbonPageCategory ribbonPageCategory1;

        private BarButtonItem barButtonItem15;

        private BarButtonItem barButtonItem16;

        private BarButtonItem barButtonItem17;

        private BarButtonItem barButtonItem18;

        private BarEditItem barEditItem4;

        private RepositoryItemButtonEdit repositoryItemButtonEdit3;

        private RepositoryItemButtonEdit repositoryItemButtonEdit4;

        private BarButtonItem barButtonItem20;

        private RepositoryItemRibbonSearchEdit repositoryItemRibbonSearchEdit1;

        private PopupControlContainer popupControlContainer3;

        private ButtonEdit buttonEdit_ReloadNpcById;

        private ButtonEdit buttonEdit_ReloadQF;

        private ButtonEdit buttonEdit_ReloadAllNPC;

        private BarButtonItem barButtonItem_ResetRechargeHttp;

        private BarButtonItem barButtonItem_ResetClientFeeHttp;

        public BarStaticItem barStaticItem2;

        public BarManager BManager;

        private RibbonPageCategory ribbonPageCategory2;
        private RibbonPageCategory ribbonPageCategory3;
        private RibbonPageCategory ribbonPageCategory4;
        private RibbonPageCategory ribbonPageCategory5;
        private RibbonPageCategory ribbonPageCategory6;
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SMain));
            EditorButtonImageOptions editorButtonImageOptions1 = new EditorButtonImageOptions();
            SerializableAppearanceObject serializableAppearanceObject1 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject2 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject3 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject4 = new SerializableAppearanceObject();
            EditorButtonImageOptions editorButtonImageOptions2 = new EditorButtonImageOptions();
            SerializableAppearanceObject serializableAppearanceObject5 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject6 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject7 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject8 = new SerializableAppearanceObject();
            EditorButtonImageOptions editorButtonImageOptions3 = new EditorButtonImageOptions();
            SerializableAppearanceObject serializableAppearanceObject9 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject10 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject11 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject12 = new SerializableAppearanceObject();
            EditorButtonImageOptions editorButtonImageOptions4 = new EditorButtonImageOptions();
            SerializableAppearanceObject serializableAppearanceObject13 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject14 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject15 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject16 = new SerializableAppearanceObject();
            EditorButtonImageOptions editorButtonImageOptions5 = new EditorButtonImageOptions();
            SerializableAppearanceObject serializableAppearanceObject17 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject18 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject19 = new SerializableAppearanceObject();
            SerializableAppearanceObject serializableAppearanceObject20 = new SerializableAppearanceObject();
            ribbonControl1 = new RibbonControl();
            StartServerButton = new BarButtonItem();
            StopServerButton = new BarButtonItem();
            ReLoadLuaButton = new BarButtonItem();
            popupControlContainer3 = new PopupControlContainer(components);
            buttonEdit_ReloadAllNPC = new ButtonEdit();
            buttonEdit_ReloadQF = new ButtonEdit();
            buttonEdit_ReloadNpcById = new ButtonEdit();
            ReadLoadMapButton = new BarButtonItem();
            barButtonItem2 = new BarButtonItem();
            barButtonItem3 = new BarButtonItem();
            barButtonItem4 = new BarButtonItem();
            barButtonItem6 = new BarButtonItem();
            barButtonItem7 = new BarButtonItem();
            barButtonItem8 = new BarButtonItem();
            barButtonItem9 = new BarButtonItem();
            barButtonItem10 = new BarButtonItem();
            barButtonItem12 = new BarButtonItem();
            barButtonItem13 = new BarButtonItem();
            barEditItem3 = new BarEditItem();
            repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
            barButtonItem14 = new BarButtonItem();
            barButtonItem15 = new BarButtonItem();
            barButtonItem16 = new BarButtonItem();
            barButtonItem18 = new BarButtonItem();
            barEditItem4 = new BarEditItem();
            repositoryItemButtonEdit3 = new RepositoryItemButtonEdit();
            barButtonItem20 = new BarButtonItem();
            barButtonItem_ResetRechargeHttp = new BarButtonItem();
            barButtonItem_ResetClientFeeHttp = new BarButtonItem();
            skinPaletteRibbonGalleryBarItem1 = new SkinPaletteRibbonGalleryBarItem();
            skinRibbonGalleryBarItem1 = new SkinRibbonGalleryBarItem();
            skinRibbonGalleryBarItem2 = new SkinRibbonGalleryBarItem();
            重载怪物Button = new BarButtonItem();
            游戏商店Button = new BarButtonItem();
            游戏称号Button = new BarButtonItem();
            坐骑数据Button = new BarButtonItem();
            怪物爆率Button = new BarButtonItem();
            ribbonPageCategory1 = new RibbonPageCategory();
            ribbonPageCategory2 = new RibbonPageCategory();
            ribbonPageCategory3 = new RibbonPageCategory();
            ribbonPageCategory4 = new RibbonPageCategory();
            ribbonPageCategory5 = new RibbonPageCategory();
            ribbonPageCategory6 = new RibbonPageCategory();
            ribbonPageCategory7 = new RibbonPageCategory();
            ribbonPageCategory8 = new RibbonPageCategory();
            ribbonPage1 = new RibbonPage();
            ribbonPageGroup1 = new RibbonPageGroup();
            重载页 = new RibbonPage();
            ribbonPageGroup3 = new RibbonPageGroup();
            ribbonPage3 = new RibbonPage();
            ribbonPageGroup4 = new RibbonPageGroup();
            ribbonPage2 = new RibbonPage();
            ribbonPageGroup2 = new RibbonPageGroup();
            repositoryItemButtonEdit4 = new RepositoryItemButtonEdit();
            navBarControl1 = new NavBarControl();
            navBarGroup1 = new NavBarGroup();
            LogNavButton = new NavBarItem();
            navBarItem1 = new NavBarItem();
            navBarItem13 = new NavBarItem();
            navBarItem15 = new NavBarItem();
            navBarGroup2 = new NavBarGroup();
            navBarItem2 = new NavBarItem();
            navBarItem3 = new NavBarItem();
            navBarItem4 = new NavBarItem();
            navBarItem5 = new NavBarItem();
            navBarItem7 = new NavBarItem();
            navBarItem8 = new NavBarItem();
            navBarItem10 = new NavBarItem();
            navBarItem14 = new NavBarItem();
            navBarItem16 = new NavBarItem();
            navBarItem17 = new NavBarItem();
            navBarItem18 = new NavBarItem();
            navBarGroup3 = new NavBarGroup();
            navBarItem11 = new NavBarItem();
            navBarItem12 = new NavBarItem();
            DManager = new DocumentManager(components);
            tabbedView1 = new TabbedView(components);
            BManager = new BarManager(components);
            bar3 = new Bar();
            barEditItem2 = new BarEditItem();
            repositoryItemTextEdit1 = new RepositoryItemTextEdit();
            barButtonItem1 = new BarButtonItem();
            ConnectionLabel = new BarStaticItem();
            ObjectLabel = new BarStaticItem();
            ProcessLabel = new BarStaticItem();
            LoopLabel = new BarStaticItem();
            ConDelay = new BarStaticItem();
            TotalDownloadLabel = new BarStaticItem();
            TotalUploadLabel = new BarStaticItem();
            scriptMemory = new BarStaticItem();
            barStaticItem2 = new BarStaticItem();
            barDockControlTop = new BarDockControl();
            barDockControlBottom = new BarDockControl();
            barDockControlLeft = new BarDockControl();
            barDockControlRight = new BarDockControl();
            barEditItem1 = new BarEditItem();
            repositoryItemButtonEdit1 = new RepositoryItemButtonEdit();
            repositoryItemTextEdit2 = new RepositoryItemTextEdit();
            InterfaceTimer = new Timer(components);
            navBarItem6 = new NavBarItem();
            barButtonItem5 = new BarButtonItem();
            ribbonPage4 = new RibbonPage();
            barButtonItem17 = new BarButtonItem();
            repositoryItemRibbonSearchEdit1 = new RepositoryItemRibbonSearchEdit();
            galleryDropDown1 = new GalleryDropDown(components);
            barButtonItem19 = new BarButtonItem();
            ((ISupportInitialize)ribbonControl1).BeginInit();
            ((ISupportInitialize)popupControlContainer3).BeginInit();
            popupControlContainer3.SuspendLayout();
            ((ISupportInitialize)buttonEdit_ReloadAllNPC.Properties).BeginInit();
            ((ISupportInitialize)buttonEdit_ReloadQF.Properties).BeginInit();
            ((ISupportInitialize)buttonEdit_ReloadNpcById.Properties).BeginInit();
            ((ISupportInitialize)repositoryItemButtonEdit2).BeginInit();
            ((ISupportInitialize)repositoryItemButtonEdit3).BeginInit();
            ((ISupportInitialize)repositoryItemButtonEdit4).BeginInit();
            ((ISupportInitialize)navBarControl1).BeginInit();
            ((ISupportInitialize)DManager).BeginInit();
            ((ISupportInitialize)tabbedView1).BeginInit();
            ((ISupportInitialize)BManager).BeginInit();
            ((ISupportInitialize)repositoryItemTextEdit1).BeginInit();
            ((ISupportInitialize)repositoryItemButtonEdit1).BeginInit();
            ((ISupportInitialize)repositoryItemTextEdit2).BeginInit();
            ((ISupportInitialize)repositoryItemRibbonSearchEdit1).BeginInit();
            ((ISupportInitialize)galleryDropDown1).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.EmptyAreaImageOptions.ImagePadding = new Padding(35, 39, 35, 39);
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new BarItem[] { ribbonControl1.ExpandCollapseItem, StartServerButton, StopServerButton, ReLoadLuaButton, ReadLoadMapButton, barButtonItem2, barButtonItem3, barButtonItem4, barButtonItem6, barButtonItem7, barButtonItem8, barButtonItem9, barButtonItem10, barButtonItem12, barButtonItem13, barEditItem3, barButtonItem14, barButtonItem15, barButtonItem16, barButtonItem18, barEditItem4, barButtonItem20, barButtonItem_ResetRechargeHttp, barButtonItem_ResetClientFeeHttp, skinPaletteRibbonGalleryBarItem1, skinRibbonGalleryBarItem1, skinRibbonGalleryBarItem2, 重载怪物Button, 游戏商店Button, 游戏称号Button, 坐骑数据Button, 怪物爆率Button });
            ribbonControl1.Location = new Point(0, 0);
            ribbonControl1.Margin = new Padding(4);
            ribbonControl1.MaxItemId = 65;
            ribbonControl1.MdiMergeStyle = RibbonMdiMergeStyle.Always;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.OptionsMenuMinWidth = 385;
            ribbonControl1.OptionsPageCategories.ShowCaptions = false;
            ribbonControl1.PageCategories.AddRange(new RibbonPageCategory[] { ribbonPageCategory1, ribbonPageCategory2, ribbonPageCategory3, ribbonPageCategory4, ribbonPageCategory5, ribbonPageCategory6, ribbonPageCategory7, ribbonPageCategory8 });
            ribbonControl1.Pages.AddRange(new RibbonPage[] { ribbonPage1, 重载页, ribbonPage3, ribbonPage2 });
            ribbonControl1.RepositoryItems.AddRange(new RepositoryItem[] { repositoryItemButtonEdit2, repositoryItemButtonEdit3, repositoryItemButtonEdit4 });
            ribbonControl1.ShowApplicationButton = DefaultBoolean.False;
            ribbonControl1.ShowExpandCollapseButton = DefaultBoolean.True;
            ribbonControl1.ShowQatLocationSelector = false;
            ribbonControl1.ShowToolbarCustomizeItem = false;
            ribbonControl1.Size = new Size(1368, 160);
            ribbonControl1.Toolbar.ShowCustomizeItem = false;
            ribbonControl1.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
            // 
            // StartServerButton
            // 
            StartServerButton.Caption = "开启服务器";
            StartServerButton.Id = 2;
            StartServerButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("StartServerButton.ImageOptions.SvgImage");
            StartServerButton.LargeWidth = 70;
            StartServerButton.Name = "StartServerButton";
            StartServerButton.ItemClick += StartServerButton_ItemClick;
            // 
            // StopServerButton
            // 
            StopServerButton.Caption = "停止服务器";
            StopServerButton.Id = 3;
            StopServerButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("StopServerButton.ImageOptions.SvgImage");
            StopServerButton.LargeWidth = 70;
            StopServerButton.Name = "StopServerButton";
            StopServerButton.ItemClick += StopServerButton_ItemClick;
            // 
            // ReLoadLuaButton
            // 
            ReLoadLuaButton.ActAsDropDown = true;
            ReLoadLuaButton.ButtonStyle = BarButtonStyle.DropDown;
            ReLoadLuaButton.Caption = "重载NPC";
            ReLoadLuaButton.DropDownControl = popupControlContainer3;
            ReLoadLuaButton.Id = 9;
            ReLoadLuaButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("ReLoadLuaButton.ImageOptions.SvgImage");
            ReLoadLuaButton.LargeWidth = 70;
            ReLoadLuaButton.Name = "ReLoadLuaButton";
            ReLoadLuaButton.ItemClick += ReLoadLuaButton_ItemClick;
            // 
            // popupControlContainer3
            // 
            popupControlContainer3.BorderStyle = BorderStyles.NoBorder;
            popupControlContainer3.Controls.Add(buttonEdit_ReloadAllNPC);
            popupControlContainer3.Controls.Add(buttonEdit_ReloadQF);
            popupControlContainer3.Controls.Add(buttonEdit_ReloadNpcById);
            popupControlContainer3.Location = new Point(199, 174);
            popupControlContainer3.Name = "popupControlContainer3";
            popupControlContainer3.Ribbon = ribbonControl1;
            popupControlContainer3.Size = new Size(250, 85);
            popupControlContainer3.TabIndex = 9;
            popupControlContainer3.Visible = false;
            // 
            // buttonEdit_ReloadAllNPC
            // 
            buttonEdit_ReloadAllNPC.Location = new Point(133, 9);
            buttonEdit_ReloadAllNPC.MenuManager = ribbonControl1;
            buttonEdit_ReloadAllNPC.Name = "buttonEdit_ReloadAllNPC";
            editorButtonImageOptions1.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            editorButtonImageOptions1.ImageUri.Uri = "Refresh";
            buttonEdit_ReloadAllNPC.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Glyph, "全部重载", -1, true, true, true, editorButtonImageOptions1, new KeyShortcut(Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default) });
            buttonEdit_ReloadAllNPC.Properties.TextEditStyle = TextEditStyles.HideTextEditor;
            buttonEdit_ReloadAllNPC.Size = new Size(100, 40);
            buttonEdit_ReloadAllNPC.TabIndex = 6;
            buttonEdit_ReloadAllNPC.EditValueChanged += buttonEdit_ReloadAllNPC_EditValueChanged;
            buttonEdit_ReloadAllNPC.Click += ReLoadLuaButton_ItemClick;
            // 
            // buttonEdit_ReloadQF
            // 
            buttonEdit_ReloadQF.Location = new Point(13, 9);
            buttonEdit_ReloadQF.MenuManager = ribbonControl1;
            buttonEdit_ReloadQF.Name = "buttonEdit_ReloadQF";
            editorButtonImageOptions2.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            editorButtonImageOptions2.ImageUri.Uri = "dashboards/convertto";
            buttonEdit_ReloadQF.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Glyph, "重载QF", -1, true, true, true, editorButtonImageOptions2, new KeyShortcut(Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default) });
            buttonEdit_ReloadQF.Properties.TextEditStyle = TextEditStyles.HideTextEditor;
            buttonEdit_ReloadQF.Size = new Size(100, 40);
            buttonEdit_ReloadQF.TabIndex = 5;
            buttonEdit_ReloadQF.Click += buttonEdit_ReloadQF_Click;
            // 
            // buttonEdit_ReloadNpcById
            // 
            buttonEdit_ReloadNpcById.EditValue = "";
            buttonEdit_ReloadNpcById.Location = new Point(13, 53);
            buttonEdit_ReloadNpcById.MenuManager = ribbonControl1;
            buttonEdit_ReloadNpcById.Name = "buttonEdit_ReloadNpcById";
            buttonEdit_ReloadNpcById.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Glyph, "重载NPC编号", -1, true, true, false, editorButtonImageOptions3, new KeyShortcut(Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default) });
            buttonEdit_ReloadNpcById.Size = new Size(220, 23);
            buttonEdit_ReloadNpcById.TabIndex = 4;
            buttonEdit_ReloadNpcById.ButtonClick += buttonEdit_ReloadNpcById_ButtonClick;
            // 
            // ReadLoadMapButton
            // 
            ReadLoadMapButton.Caption = "实例全重载";
            ReadLoadMapButton.Id = 10;
            ReadLoadMapButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("ReadLoadMapButton.ImageOptions.SvgImage");
            ReadLoadMapButton.LargeWidth = 70;
            ReadLoadMapButton.Name = "ReadLoadMapButton";
            ReadLoadMapButton.ItemClick += ReadLoadMapButton_ItemClick;
            // 
            // barButtonItem2
            // 
            barButtonItem2.Caption = "barButtonItem2";
            barButtonItem2.Id = 12;
            barButtonItem2.Name = "barButtonItem2";
            // 
            // barButtonItem3
            // 
            barButtonItem3.Caption = "barButtonItem3";
            barButtonItem3.Id = 13;
            barButtonItem3.Name = "barButtonItem3";
            // 
            // barButtonItem4
            // 
            barButtonItem4.Caption = "守卫";
            barButtonItem4.Id = 17;
            barButtonItem4.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem4.ImageOptions.SvgImage");
            barButtonItem4.Name = "barButtonItem4";
            barButtonItem4.Tag = "8";
            barButtonItem4.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem6
            // 
            barButtonItem6.Caption = "传送法阵";
            barButtonItem6.Id = 19;
            barButtonItem6.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem6.ImageOptions.SvgImage");
            barButtonItem6.Name = "barButtonItem6";
            barButtonItem6.Tag = "1";
            barButtonItem6.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem7
            // 
            barButtonItem7.Caption = "游戏技能";
            barButtonItem7.Id = 20;
            barButtonItem7.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem7.ImageOptions.SvgImage");
            barButtonItem7.Name = "barButtonItem7";
            barButtonItem7.Tag = "3";
            barButtonItem7.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem8
            // 
            barButtonItem8.Caption = "游戏物品";
            barButtonItem8.Id = 21;
            barButtonItem8.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem8.ImageOptions.SvgImage");
            barButtonItem8.Name = "barButtonItem8";
            barButtonItem8.Tag = "5";
            barButtonItem8.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem9
            // 
            barButtonItem9.Caption = "珍宝商品";
            barButtonItem9.Id = 22;
            barButtonItem9.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem9.ImageOptions.SvgImage");
            barButtonItem9.Name = "barButtonItem9";
            barButtonItem9.Tag = "6";
            barButtonItem9.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem10
            // 
            barButtonItem10.Caption = "龙卫模板";
            barButtonItem10.Id = 23;
            barButtonItem10.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem10.ImageOptions.SvgImage");
            barButtonItem10.Name = "barButtonItem10";
            barButtonItem10.Tag = "7";
            barButtonItem10.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem12
            // 
            barButtonItem12.Caption = "机器人";
            barButtonItem12.Id = 27;
            barButtonItem12.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem12.ImageOptions.SvgImage");
            barButtonItem12.Name = "barButtonItem12";
            barButtonItem12.Tag = "10";
            barButtonItem12.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem13
            // 
            barButtonItem13.Caption = "合区";
            barButtonItem13.Id = 28;
            barButtonItem13.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem13.ImageOptions.SvgImage");
            barButtonItem13.Name = "barButtonItem13";
            barButtonItem13.ItemClick += barButtonItem13_ItemClick;
            // 
            // barEditItem3
            // 
            barEditItem3.Caption = "请选择数据目录";
            barEditItem3.Edit = repositoryItemButtonEdit2;
            barEditItem3.EditWidth = 388;
            barEditItem3.Id = 29;
            barEditItem3.Name = "barEditItem3";
            barEditItem3.ShowingEditor += barEditItem_ShowingEditor;
            barEditItem3.HyperlinkClick += barEditItem3_HyperlinkClick;
            barEditItem3.ItemClick += barEditItem3_ItemClick;
            // 
            // repositoryItemButtonEdit2
            // 
            repositoryItemButtonEdit2.AutoHeight = false;
            repositoryItemButtonEdit2.Buttons.AddRange(new EditorButton[] { new EditorButton() });
            repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
            // 
            // barButtonItem14
            // 
            barButtonItem14.Caption = "barButtonItem14";
            barButtonItem14.Id = 30;
            barButtonItem14.Name = "barButtonItem14";
            // 
            // barButtonItem15
            // 
            barButtonItem15.Caption = "地图道具";
            barButtonItem15.Id = 32;
            barButtonItem15.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem15.ImageOptions.SvgImage");
            barButtonItem15.Name = "barButtonItem15";
            barButtonItem15.Tag = "12";
            barButtonItem15.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem16
            // 
            barButtonItem16.Caption = "装备神佑消耗";
            barButtonItem16.Id = 33;
            barButtonItem16.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem16.ImageOptions.SvgImage");
            barButtonItem16.Name = "barButtonItem16";
            barButtonItem16.Tag = "13";
            barButtonItem16.ItemClick += barButtonItem4_ItemClick;
            // 
            // barButtonItem18
            // 
            barButtonItem18.Caption = "公告表";
            barButtonItem18.Id = 34;
            barButtonItem18.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem18.ImageOptions.SvgImage");
            barButtonItem18.Name = "barButtonItem18";
            barButtonItem18.Tag = "15";
            barButtonItem18.ItemClick += barButtonItem4_ItemClick;
            // 
            // barEditItem4
            // 
            barEditItem4.Caption = "选主区账号目录";
            barEditItem4.Edit = repositoryItemButtonEdit3;
            barEditItem4.EditWidth = 388;
            barEditItem4.Id = 36;
            barEditItem4.Name = "barEditItem4";
            barEditItem4.ShowingEditor += barEditItem_ShowingEditor;
            // 
            // repositoryItemButtonEdit3
            // 
            repositoryItemButtonEdit3.AutoHeight = false;
            repositoryItemButtonEdit3.Buttons.AddRange(new EditorButton[] { new EditorButton() });
            repositoryItemButtonEdit3.Name = "repositoryItemButtonEdit3";
            // 
            // barButtonItem20
            // 
            barButtonItem20.Caption = "barButtonItem20";
            barButtonItem20.Id = 41;
            barButtonItem20.Name = "barButtonItem20";
            // 
            // barButtonItem_ResetRechargeHttp
            // 
            barButtonItem_ResetRechargeHttp.Caption = "重启充值服务";
            barButtonItem_ResetRechargeHttp.Enabled = false;
            barButtonItem_ResetRechargeHttp.Id = 43;
            barButtonItem_ResetRechargeHttp.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem_ResetRechargeHttp.ImageOptions.SvgImage");
            barButtonItem_ResetRechargeHttp.Name = "barButtonItem_ResetRechargeHttp";
            barButtonItem_ResetRechargeHttp.ItemClick += barButtonItem_ResetRechargeHttp_ItemClick;
            // 
            // barButtonItem_ResetClientFeeHttp
            // 
            barButtonItem_ResetClientFeeHttp.Caption = "重启门票服务";
            barButtonItem_ResetClientFeeHttp.Id = 44;
            barButtonItem_ResetClientFeeHttp.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem_ResetClientFeeHttp.ImageOptions.SvgImage");
            barButtonItem_ResetClientFeeHttp.Name = "barButtonItem_ResetClientFeeHttp";
            barButtonItem_ResetClientFeeHttp.ItemClick += barButtonItem_ResetClientFeeHttp_ItemClick;
            // 
            // skinPaletteRibbonGalleryBarItem1
            // 
            skinPaletteRibbonGalleryBarItem1.Caption = "skinPaletteRibbonGalleryBarItem1";
            skinPaletteRibbonGalleryBarItem1.Id = 47;
            skinPaletteRibbonGalleryBarItem1.Name = "skinPaletteRibbonGalleryBarItem1";
            // 
            // skinRibbonGalleryBarItem1
            // 
            skinRibbonGalleryBarItem1.Caption = "skinRibbonGalleryBarItem1";
            skinRibbonGalleryBarItem1.Id = 48;
            skinRibbonGalleryBarItem1.Name = "skinRibbonGalleryBarItem1";
            // 
            // skinRibbonGalleryBarItem2
            // 
            skinRibbonGalleryBarItem2.Caption = "skinRibbonGalleryBarItem2";
            skinRibbonGalleryBarItem2.Id = 59;
            skinRibbonGalleryBarItem2.Name = "skinRibbonGalleryBarItem2";
            // 
            // 重载怪物Button
            // 
            重载怪物Button.Caption = "怪物数据";
            重载怪物Button.Id = 60;
            重载怪物Button.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("重载怪物Button.ImageOptions.SvgImage");
            重载怪物Button.Name = "重载怪物Button";
            重载怪物Button.ItemClick += 重载怪物Button_ItemClick;
            // 
            // 游戏商店Button
            // 
            游戏商店Button.Caption = "游戏商店";
            游戏商店Button.Id = 61;
            游戏商店Button.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("游戏商店Button.ImageOptions.SvgImage");
            游戏商店Button.Name = "游戏商店Button";
            游戏商店Button.ItemClick += 游戏商店Button_ItemClick;
            // 
            // 游戏称号Button
            // 
            游戏称号Button.Caption = "游戏称号";
            游戏称号Button.Id = 62;
            游戏称号Button.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("游戏称号Button.ImageOptions.SvgImage");
            游戏称号Button.Name = "游戏称号Button";
            游戏称号Button.ItemClick += 游戏称号Button_ItemClick;
            // 
            // 坐骑数据Button
            // 
            坐骑数据Button.Caption = "坐骑数据";
            坐骑数据Button.Id = 63;
            坐骑数据Button.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("坐骑数据Button.ImageOptions.SvgImage");
            坐骑数据Button.Name = "坐骑数据Button";
            坐骑数据Button.ItemClick += 坐骑数据Button_ItemClick;
            // 
            // 怪物爆率Button
            // 
            怪物爆率Button.Caption = "怪物爆率";
            怪物爆率Button.Id = 64;
            怪物爆率Button.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("怪物爆率Button.ImageOptions.SvgImage");
            怪物爆率Button.Name = "怪物爆率Button";
            怪物爆率Button.ItemClick += 怪物爆率Button_ItemClick;
            // 
            // ribbonPageCategory1
            // 
            ribbonPageCategory1.Name = "ribbonPageCategory1";
            ribbonPageCategory1.Text = "ribbonPageCategory1";
            // 
            // ribbonPageCategory2
            // 
            ribbonPageCategory2.Name = "ribbonPageCategory2";
            ribbonPageCategory2.Text = "ribbonPageCategory2";
            // 
            // ribbonPageCategory3
            // 
            ribbonPageCategory3.Name = "ribbonPageCategory3";
            ribbonPageCategory3.Text = "ribbonPageCategory3";
            // 
            // ribbonPageCategory4
            // 
            ribbonPageCategory4.Name = "ribbonPageCategory4";
            ribbonPageCategory4.Text = "ribbonPageCategory4";
            // 
            // ribbonPageCategory5
            // 
            ribbonPageCategory5.Name = "ribbonPageCategory5";
            ribbonPageCategory5.Text = "ribbonPageCategory5";
            // 
            // ribbonPageCategory6
            // 
            ribbonPageCategory6.Name = "ribbonPageCategory6";
            ribbonPageCategory6.Text = "ribbonPageCategory6";
            // 
            // ribbonPageCategory7
            // 
            ribbonPageCategory7.Name = "ribbonPageCategory7";
            ribbonPageCategory7.Text = "ribbonPageCategory7";
            // 
            // ribbonPageCategory8
            // 
            ribbonPageCategory8.Name = "ribbonPageCategory8";
            ribbonPageCategory8.Text = "ribbonPageCategory8";
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "主页";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(StartServerButton);
            ribbonPageGroup1.ItemLinks.Add(StopServerButton);
            ribbonPageGroup1.ItemLinks.Add(ReadLoadMapButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "控制";
            // 
            // 重载页
            // 
            重载页.Groups.AddRange(new RibbonPageGroup[] { ribbonPageGroup3 });
            重载页.Name = "重载页";
            重载页.Text = "重载数据";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.CaptionButtonVisible = DefaultBoolean.False;
            ribbonPageGroup3.ItemLinks.Add(ReLoadLuaButton);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem4);
            ribbonPageGroup3.ItemLinks.Add(游戏称号Button);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem6);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem7);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem8);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem9);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem15);
            ribbonPageGroup3.ItemLinks.Add(游戏商店Button);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem10);
            ribbonPageGroup3.ItemLinks.Add(重载怪物Button);
            ribbonPageGroup3.ItemLinks.Add(怪物爆率Button);
            ribbonPageGroup3.ItemLinks.Add(坐骑数据Button);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem18);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem16);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem12, true);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem_ResetRechargeHttp);
            ribbonPageGroup3.ItemLinks.Add(barButtonItem_ResetClientFeeHttp);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            ribbonPageGroup3.Text = "重载";
            // 
            // ribbonPage3
            // 
            ribbonPage3.Groups.AddRange(new RibbonPageGroup[] { ribbonPageGroup4 });
            ribbonPage3.Name = "ribbonPage3";
            ribbonPage3.Text = "合区工具";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.ItemLinks.Add(barEditItem3);
            ribbonPageGroup4.ItemLinks.Add(barEditItem4);
            ribbonPageGroup4.ItemLinks.Add(barButtonItem13);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            ribbonPageGroup4.Text = "注: 合并客户数据为合区专用, 请谨慎使用(合区前请备份数据!!!)";
            // 
            // ribbonPage2
            // 
            ribbonPage2.Groups.AddRange(new RibbonPageGroup[] { ribbonPageGroup2 });
            ribbonPage2.Name = "ribbonPage2";
            ribbonPage2.Text = "皮肤";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(skinRibbonGalleryBarItem2);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // repositoryItemButtonEdit4
            // 
            repositoryItemButtonEdit4.AutoHeight = false;
            repositoryItemButtonEdit4.Buttons.AddRange(new EditorButton[] { new EditorButton() });
            repositoryItemButtonEdit4.Name = "repositoryItemButtonEdit4";
            // 
            // navBarControl1
            // 
            navBarControl1.ActiveGroup = navBarGroup1;
            navBarControl1.Dock = DockStyle.Left;
            navBarControl1.Groups.AddRange(new NavBarGroup[] { navBarGroup1, navBarGroup2, navBarGroup3 });
            navBarControl1.Items.AddRange(new NavBarItem[] { LogNavButton, navBarItem1, navBarItem2, navBarItem3, navBarItem4, navBarItem5, navBarItem7, navBarItem8, navBarItem10, navBarItem11, navBarItem12, navBarItem13, navBarItem14, navBarItem15, navBarItem16, navBarItem17, navBarItem18 });
            navBarControl1.Location = new Point(0, 160);
            navBarControl1.Margin = new Padding(4);
            navBarControl1.Name = "navBarControl1";
            navBarControl1.OptionsNavPane.ExpandedWidth = 168;
            navBarControl1.Size = new Size(168, 603);
            navBarControl1.TabIndex = 1;
            navBarControl1.Text = "navBarControl1";
            navBarControl1.Click += navBarControl1_Click;
            // 
            // navBarGroup1
            // 
            navBarGroup1.Caption = "操作";
            navBarGroup1.Expanded = true;
            navBarGroup1.ItemLinks.AddRange(new NavBarItemLink[] { new NavBarItemLink(LogNavButton), new NavBarItemLink(navBarItem1), new NavBarItemLink(navBarItem13), new NavBarItemLink(navBarItem15) });
            navBarGroup1.Name = "navBarGroup1";
            // 
            // LogNavButton
            // 
            LogNavButton.Caption = "系统日志";
            LogNavButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("LogNavButton.ImageOptions.SvgImage");
            LogNavButton.Name = "LogNavButton";
            LogNavButton.LinkClicked += LogNavButton_LinkClicked;
            // 
            // navBarItem1
            // 
            navBarItem1.Caption = "聊天日志";
            navBarItem1.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem1.ImageOptions.SvgImage");
            navBarItem1.Name = "navBarItem1";
            navBarItem1.LinkClicked += navBarItem1_LinkClicked;
            // 
            // navBarItem13
            // 
            navBarItem13.Caption = "命令日志";
            navBarItem13.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem13.ImageOptions.SvgImage");
            navBarItem13.Name = "navBarItem13";
            navBarItem13.LinkClicked += navBarItem13_LinkClicked;
            // 
            // navBarItem15
            // 
            navBarItem15.Caption = "游戏设置";
            navBarItem15.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem15.ImageOptions.SvgImage");
            navBarItem15.Name = "navBarItem15";
            navBarItem15.LinkClicked += navBarItem15_LinkClicked;
            // 
            // navBarGroup2
            // 
            navBarGroup2.Caption = "配置";
            navBarGroup2.Expanded = true;
            navBarGroup2.ItemLinks.AddRange(new NavBarItemLink[] { new NavBarItemLink(navBarItem2), new NavBarItemLink(navBarItem3), new NavBarItemLink(navBarItem4), new NavBarItemLink(navBarItem5), new NavBarItemLink(navBarItem7), new NavBarItemLink(navBarItem8), new NavBarItemLink(navBarItem10), new NavBarItemLink(navBarItem14), new NavBarItemLink(navBarItem16), new NavBarItemLink(navBarItem17), new NavBarItemLink(navBarItem18) });
            navBarGroup2.Name = "navBarGroup2";
            // 
            // navBarItem2
            // 
            navBarItem2.Caption = "技能配置";
            navBarItem2.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem2.ImageOptions.SvgImage");
            navBarItem2.Name = "navBarItem2";
            navBarItem2.LinkClicked += navBarItem2_LinkClicked;
            // 
            // navBarItem3
            // 
            navBarItem3.Caption = "铭文配置";
            navBarItem3.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem3.ImageOptions.SvgImage");
            navBarItem3.Name = "navBarItem3";
            navBarItem3.LinkClicked += navBarItem3_LinkClicked;
            // 
            // navBarItem4
            // 
            navBarItem4.Caption = "BUFF配置";
            navBarItem4.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem4.ImageOptions.SvgImage");
            navBarItem4.Name = "navBarItem4";
            navBarItem4.LinkClicked += navBarItem4_LinkClicked;
            // 
            // navBarItem5
            // 
            navBarItem5.Caption = "陷阱配置";
            navBarItem5.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem5.ImageOptions.SvgImage");
            navBarItem5.Name = "navBarItem5";
            navBarItem5.LinkClicked += navBarItem5_LinkClicked;
            // 
            // navBarItem7
            // 
            navBarItem7.Caption = "怪物配置";
            navBarItem7.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem7.ImageOptions.SvgImage");
            navBarItem7.Name = "navBarItem7";
            navBarItem7.LinkClicked += navBarItem7_LinkClicked;
            // 
            // navBarItem8
            // 
            navBarItem8.Caption = "物品配置";
            navBarItem8.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem8.ImageOptions.SvgImage");
            navBarItem8.Name = "navBarItem8";
            navBarItem8.LinkClicked += navBarItem8_LinkClicked;
            // 
            // navBarItem10
            // 
            navBarItem10.Caption = "地图配置";
            navBarItem10.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem10.ImageOptions.SvgImage");
            navBarItem10.Name = "navBarItem10";
            // 
            // navBarItem14
            // 
            navBarItem14.Caption = "公告配置";
            navBarItem14.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem14.ImageOptions.SvgImage");
            navBarItem14.Name = "navBarItem14";
            navBarItem14.LinkClicked += navBarItem14_LinkClicked;
            // 
            // navBarItem16
            // 
            navBarItem16.Caption = "守卫配置";
            navBarItem16.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem16.ImageOptions.SvgImage");
            navBarItem16.Name = "navBarItem16";
            navBarItem16.LinkClicked += navBarItem16_LinkClicked;
            // 
            // navBarItem17
            // 
            navBarItem17.Caption = "称号配置";
            navBarItem17.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem17.ImageOptions.SvgImage");
            navBarItem17.Name = "navBarItem17";
            navBarItem17.Visible = false;
            navBarItem17.LinkClicked += navBarItem17_LinkClicked;
            // 
            // navBarItem18
            // 
            navBarItem18.Caption = "刷怪配置";
            navBarItem18.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem18.ImageOptions.SvgImage");
            navBarItem18.Name = "navBarItem18";
            navBarItem18.LinkClicked += navBarItem18_LinkClicked;
            // 
            // navBarGroup3
            // 
            navBarGroup3.Caption = "数据";
            navBarGroup3.Expanded = true;
            navBarGroup3.ItemLinks.AddRange(new NavBarItemLink[] { new NavBarItemLink(navBarItem11), new NavBarItemLink(navBarItem12) });
            navBarGroup3.Name = "navBarGroup3";
            // 
            // navBarItem11
            // 
            navBarItem11.Caption = "角色数据";
            navBarItem11.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem11.ImageOptions.SvgImage");
            navBarItem11.Name = "navBarItem11";
            navBarItem11.LinkClicked += navBarItem11_LinkClicked;
            // 
            // navBarItem12
            // 
            navBarItem12.Caption = "地图信息";
            navBarItem12.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navBarItem12.ImageOptions.SvgImage");
            navBarItem12.Name = "navBarItem12";
            navBarItem12.LinkClicked += navBarItem12_LinkClicked;
            // 
            // DManager
            // 
            DManager.MdiParent = this;
            DManager.MenuManager = ribbonControl1;
            DManager.View = tabbedView1;
            DManager.ViewCollection.AddRange(new BaseView[] { tabbedView1 });
            // 
            // BManager
            // 
            BManager.Bars.AddRange(new Bar[] { bar3 });
            BManager.DockControls.Add(barDockControlTop);
            BManager.DockControls.Add(barDockControlBottom);
            BManager.DockControls.Add(barDockControlLeft);
            BManager.DockControls.Add(barDockControlRight);
            BManager.Form = this;
            BManager.Items.AddRange(new BarItem[] { ConnectionLabel, ObjectLabel, ProcessLabel, LoopLabel, TotalDownloadLabel, TotalUploadLabel, ConDelay, barEditItem1, barEditItem2, barButtonItem1, scriptMemory, barStaticItem2 });
            BManager.MaxItemId = 22;
            BManager.RepositoryItems.AddRange(new RepositoryItem[] { repositoryItemButtonEdit1, repositoryItemTextEdit1, repositoryItemTextEdit2 });
            BManager.StatusBar = bar3;
            // 
            // bar3
            // 
            bar3.BarName = "Status bar";
            bar3.CanDockStyle = BarCanDockStyle.Bottom;
            bar3.DockCol = 0;
            bar3.DockRow = 0;
            bar3.DockStyle = BarDockStyle.Bottom;
            bar3.LinksPersistInfo.AddRange(new LinkPersistInfo[] { new LinkPersistInfo(barEditItem2), new LinkPersistInfo(barButtonItem1), new LinkPersistInfo(ConnectionLabel), new LinkPersistInfo(ObjectLabel), new LinkPersistInfo(ProcessLabel), new LinkPersistInfo(LoopLabel), new LinkPersistInfo(ConDelay), new LinkPersistInfo(TotalDownloadLabel), new LinkPersistInfo(TotalUploadLabel), new LinkPersistInfo(scriptMemory), new LinkPersistInfo(barStaticItem2) });
            bar3.OptionsBar.AllowQuickCustomization = false;
            bar3.OptionsBar.DrawDragBorder = false;
            bar3.OptionsBar.UseWholeRow = true;
            bar3.Text = "Status bar";
            // 
            // barEditItem2
            // 
            barEditItem2.Caption = "barEditItem2";
            barEditItem2.Edit = repositoryItemTextEdit1;
            barEditItem2.EditValue = "";
            barEditItem2.EditWidth = 300;
            barEditItem2.Id = 13;
            barEditItem2.Name = "barEditItem2";
            barEditItem2.ShownEditor += barButtonItem1_ItemClick;
            // 
            // repositoryItemTextEdit1
            // 
            repositoryItemTextEdit1.AutoHeight = false;
            repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            repositoryItemTextEdit1.KeyPress += repositoryItemTextEdit1_KeyPress;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "< 执行命令";
            barButtonItem1.Id = 14;
            barButtonItem1.Name = "barButtonItem1";
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
            // 
            // ConnectionLabel
            // 
            ConnectionLabel.Caption = "连接: 0";
            ConnectionLabel.Id = 1;
            ConnectionLabel.Name = "ConnectionLabel";
            // 
            // ObjectLabel
            // 
            ObjectLabel.Caption = "对象: 0";
            ObjectLabel.Id = 2;
            ObjectLabel.Name = "ObjectLabel";
            // 
            // ProcessLabel
            // 
            ProcessLabel.Caption = "进程计数: 0";
            ProcessLabel.Id = 3;
            ProcessLabel.Name = "ProcessLabel";
            // 
            // LoopLabel
            // 
            LoopLabel.Caption = "循环计数: 0";
            LoopLabel.Id = 4;
            LoopLabel.Name = "LoopLabel";
            // 
            // ConDelay
            // 
            ConDelay.Caption = "Con 延时: 0";
            ConDelay.Id = 10;
            ConDelay.Name = "ConDelay";
            // 
            // TotalDownloadLabel
            // 
            TotalDownloadLabel.Caption = "已下载: 0B";
            TotalDownloadLabel.Id = 5;
            TotalDownloadLabel.Name = "TotalDownloadLabel";
            // 
            // TotalUploadLabel
            // 
            TotalUploadLabel.Caption = "已上传: 0B";
            TotalUploadLabel.Id = 6;
            TotalUploadLabel.Name = "TotalUploadLabel";
            // 
            // scriptMemory
            // 
            scriptMemory.Caption = "lua 内存";
            scriptMemory.Id = 15;
            scriptMemory.Name = "scriptMemory";
            scriptMemory.Size = new Size(120, 0);
            scriptMemory.Width = 120;
            // 
            // barStaticItem2
            // 
            barStaticItem2.Id = 20;
            barStaticItem2.Name = "barStaticItem2";
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = DockStyle.Top;
            barDockControlTop.Location = new Point(0, 0);
            barDockControlTop.Manager = BManager;
            barDockControlTop.Margin = new Padding(4);
            barDockControlTop.Size = new Size(1368, 0);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = DockStyle.Bottom;
            barDockControlBottom.Location = new Point(0, 763);
            barDockControlBottom.Manager = BManager;
            barDockControlBottom.Margin = new Padding(4);
            barDockControlBottom.Size = new Size(1368, 24);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = DockStyle.Left;
            barDockControlLeft.Location = new Point(0, 0);
            barDockControlLeft.Manager = BManager;
            barDockControlLeft.Margin = new Padding(4);
            barDockControlLeft.Size = new Size(0, 763);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = DockStyle.Right;
            barDockControlRight.Location = new Point(1368, 0);
            barDockControlRight.Manager = BManager;
            barDockControlRight.Margin = new Padding(4);
            barDockControlRight.Size = new Size(0, 763);
            // 
            // barEditItem1
            // 
            barEditItem1.Caption = "barEditItem1";
            barEditItem1.Edit = repositoryItemButtonEdit1;
            barEditItem1.EditWidth = 400;
            barEditItem1.Id = 12;
            barEditItem1.Name = "barEditItem1";
            // 
            // repositoryItemButtonEdit1
            // 
            repositoryItemButtonEdit1.AutoHeight = false;
            repositoryItemButtonEdit1.Buttons.AddRange(new EditorButton[] { new EditorButton() });
            repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            // 
            // repositoryItemTextEdit2
            // 
            repositoryItemTextEdit2.AutoHeight = false;
            repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // InterfaceTimer
            // 
            InterfaceTimer.Interval = 1000;
            InterfaceTimer.Tick += InterfaceTimer_Tick;
            // 
            // navBarItem6
            // 
            navBarItem6.Name = "navBarItem6";
            // 
            // barButtonItem5
            // 
            barButtonItem5.Caption = "守卫";
            barButtonItem5.Id = 17;
            barButtonItem5.Name = "barButtonItem5";
            barButtonItem5.Tag = "8";
            // 
            // ribbonPage4
            // 
            ribbonPage4.Name = "ribbonPage4";
            ribbonPage4.Text = "ribbonPage4";
            // 
            // barButtonItem17
            // 
            barButtonItem17.Caption = "龙卫模板";
            barButtonItem17.Id = 23;
            barButtonItem17.Name = "barButtonItem17";
            barButtonItem17.Tag = "7";
            // 
            // repositoryItemRibbonSearchEdit1
            // 
            repositoryItemRibbonSearchEdit1.AllowFocused = false;
            repositoryItemRibbonSearchEdit1.AutoHeight = false;
            repositoryItemRibbonSearchEdit1.BorderStyle = BorderStyles.NoBorder;
            editorButtonImageOptions4.AllowGlyphSkinning = DefaultBoolean.True;
            repositoryItemRibbonSearchEdit1.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new KeyShortcut(Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default), new EditorButton(ButtonPredefines.Clear, "", -1, true, false, false, editorButtonImageOptions5, new KeyShortcut(Keys.None), serializableAppearanceObject17, serializableAppearanceObject18, serializableAppearanceObject19, serializableAppearanceObject20, "", null, null, DevExpress.Utils.ToolTipAnchor.Default) });
            repositoryItemRibbonSearchEdit1.Name = "repositoryItemRibbonSearchEdit1";
            repositoryItemRibbonSearchEdit1.NullText = "Search";
            // 
            // galleryDropDown1
            // 
            galleryDropDown1.Name = "galleryDropDown1";
            galleryDropDown1.Ribbon = ribbonControl1;
            // 
            // barButtonItem19
            // 
            barButtonItem19.ActAsDropDown = true;
            barButtonItem19.ButtonStyle = BarButtonStyle.DropDown;
            barButtonItem19.Caption = "怪物爆率";
            barButtonItem19.Id = 40;
            barButtonItem19.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("barButtonItem19.ImageOptions.SvgImage");
            barButtonItem19.Name = "barButtonItem19";
            barButtonItem19.ItemClick += barButtonItem19_ItemClick;
            // 
            // SMain
            // 
            AllowFormGlass = DefaultBoolean.True;
            Appearance.Options.UseFont = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1368, 787);
            Controls.Add(navBarControl1);
            Controls.Add(popupControlContainer3);
            Controls.Add(ribbonControl1);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            Font = new Font("Microsoft YaHei UI", 9F);
            IsMdiContainer = true;
            Margin = new Padding(4);
            Name = "SMain";
            Ribbon = ribbonControl1;
            Text = "Elaina Engine - 游戏区名称：";
            Load += SMain_Load;
            ((ISupportInitialize)ribbonControl1).EndInit();
            ((ISupportInitialize)popupControlContainer3).EndInit();
            popupControlContainer3.ResumeLayout(false);
            ((ISupportInitialize)buttonEdit_ReloadAllNPC.Properties).EndInit();
            ((ISupportInitialize)buttonEdit_ReloadQF.Properties).EndInit();
            ((ISupportInitialize)buttonEdit_ReloadNpcById.Properties).EndInit();
            ((ISupportInitialize)repositoryItemButtonEdit2).EndInit();
            ((ISupportInitialize)repositoryItemButtonEdit3).EndInit();
            ((ISupportInitialize)repositoryItemButtonEdit4).EndInit();
            ((ISupportInitialize)navBarControl1).EndInit();
            ((ISupportInitialize)DManager).EndInit();
            ((ISupportInitialize)tabbedView1).EndInit();
            ((ISupportInitialize)BManager).EndInit();
            ((ISupportInitialize)repositoryItemTextEdit1).EndInit();
            ((ISupportInitialize)repositoryItemButtonEdit1).EndInit();
            ((ISupportInitialize)repositoryItemTextEdit2).EndInit();
            ((ISupportInitialize)repositoryItemRibbonSearchEdit1).EndInit();
            ((ISupportInitialize)galleryDropDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private RibbonPageCategory ribbonPageCategory7;
        private SkinPaletteRibbonGalleryBarItem skinPaletteRibbonGalleryBarItem1;
        private RibbonPageCategory ribbonPageCategory8;
        private SkinRibbonGalleryBarItem skinRibbonGalleryBarItem1;
        private RepositoryItemTextEdit repositoryItemTextEdit2;
        private SkinPaletteRibbonGalleryBarItem skinPaletteRibbonGalleryBarItem2;
        private SkinRibbonGalleryBarItem skinRibbonGalleryBarItem2;
        private BarButtonItem 重载怪物Button;
        private BarButtonItem 游戏商店Button;
        private BarButtonItem 游戏称号Button;
        private BarButtonItem 坐骑数据Button;
        private GalleryDropDown galleryDropDown1;
        private BarButtonItem barButtonItem19;
    }
}
