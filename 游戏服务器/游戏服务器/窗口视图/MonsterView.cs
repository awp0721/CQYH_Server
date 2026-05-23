using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using 游戏服务器;
using 游戏服务器.地图类;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraVerticalGrid;

namespace 游戏服务器.窗口视图
{
	public class MonsterView : RibbonForm
	{
		private class 怪物视图
		{
			private 怪物实例 怪物;

			public string 怪物名字 => this.怪物?.对象名字;

			public int 怪物编号 => (this.怪物?.对象模板?.怪物编号).GetValueOrDefault();

			public int 实例编号 => this.怪物?.地图编号 ?? 0;

			public bool 已死亡 => this.怪物?.对象死亡 ?? false;

			public 怪物级别分类? 怪物级别 => this.怪物?.怪物级别;

			public string 当前目标 => this.怪物?.对象仇恨?.当前目标?.对象名字;

			public string 当前坐标 => this.PosToStr(this.怪物?.当前坐标 ?? default(Point));

			public string 游戏坐标 => this.PosToStr((this.怪物 == null) ? default(PointF) : 计算类.点阵坐标转游戏坐标(this.怪物.当前坐标));

			public int 当前体力 => this.怪物?.当前体力 ?? 0;

			public int 最大体力
			{
				get
				{
					if (this.怪物 != null)
					{
						return this.怪物[游戏对象属性.最大体力];
					}
					return 0;
				}
			}

			public int 最大攻击
			{
				get
				{
					if (this.怪物 != null)
					{
						return this.怪物[游戏对象属性.最大攻击];
					}
					return 0;
				}
			}

			public int 最大魔法
			{
				get
				{
					if (this.怪物 != null)
					{
						return this.怪物[游戏对象属性.最大魔法];
					}
					return 0;
				}
			}

			public 怪物视图(怪物实例 _怪物)
			{
				this.怪物 = _怪物;
			}

			public string PosToStr(Point p)
			{
				return $"{p.X},{p.Y}";
			}

			public string PosToStr(PointF p)
			{
				return $"{(int)p.X / 100},{(int)p.Y / 100}";
			}
		}

		private IContainer components;

		private RibbonControl ribbon;

		private BarButtonItem barButtonItem1;

		private BarButtonItem ReLoadDataBaseButton;

		private BarButtonItem NodeDeleteButton;

		private BarButtonItem NodeAddButton;

		private BarEditItem SkillNameEdit;

		private BarEditItem SkillIndexEdit;

		private BarEditItem SkillRuneEdit;

		private BarEditItem NodeTimeEdit;

		private BarEditItem NodeTypeEdit;

		private BarButtonItem SkillDeleteButton;

		private BarButtonItem SkillAddButton;

		private BarHeaderItem SkillHeader;

		private BarHeaderItem SkillAddHeader;

		private BarHeaderItem barHeaderItem1;

		private BarButtonItem SkillCopyButton;

		private RibbonPage ribbonPage1;

		private RibbonPageGroup ribbonPageGroup1;

		private TreeList treeList1;

		private TreeListColumn treeListColumn1;

		private GroupControl groupControl2;

		private GridControl gridControl1;

		private GridView gridView1;

		private RepositoryItemImageComboBox repositoryItemImageComboBox1;

		private SplitterControl splitterControl4;

		private GridControl gridControl2;

		private GridView gridView2;

		private GridColumn gridColumn4;

		private RepositoryItemImageComboBox repositoryItemImageComboBox2;

		private SplitterControl splitterControl5;

		private PropertyDescriptionControl propertyDescriptionControl2;

		public MonsterView()
		{
			this.InitializeComponent();
			this.ColumnWidth(this.gridView1);
			this.ColumnWidth(this.gridView2);
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.SelectionChanged += GridView1_SelectionChanged;
			this.Init();
		}

		private void GridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			BindingList<怪物指定掉落> dataSource;
			dataSource = new BindingList<怪物指定掉落>
			{
				new 怪物指定掉落
				{
					物品名字 = "物品",
					指定条件 = 指定掉落条件.指定玩家,
					指定条件值 = "玩家名",
					最低数量 = 1,
					最高数量 = 10
				}
			};
			int[] selectedRows;
			selectedRows = this.gridView1.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				this.gridControl2.DataSource = dataSource;
				this.gridView2.RefreshData();
			}
		}

		private void Init()
		{
			Dictionary<int, 地图实例> 地图实例表;
			地图实例表 = 地图处理网关.地图实例表;
			if (地图实例表 == null || !地图实例表.Any())
			{
				return;
			}
			this.treeList1.Nodes.Clear();
			foreach (KeyValuePair<int, 地图实例> item in from m in 地图处理网关.地图实例表.ToList()
				orderby m.Value.地图编号
				select m)
			{
				this.treeList1.Nodes.Add(item.Value.地图模板.地图名字).Tag = item.Value;
			}
			foreach (地图实例 item2 in from m in 地图处理网关.副本实例表.ToList()
				orderby m.地图编号
				select m)
			{
				this.treeList1.Nodes.Add(item2.地图模板.地图名字 + "(副本:" + ((item2.副本主人 <= 0 || !游戏数据网关.角色数据表.数据表.TryGetValue(item2.副本主人, out var value)) ? "" : value.ToString()) + ")").Tag = item2;
			}
			this.gridControl1.DataSource = null;
			this.gridView1.RefreshData();
		}

		private void treeList1_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
		{
			_ = e.Node;
			if (e.Node == null || !(e.Node.Tag is 地图实例 地图实例))
			{
				return;
			}
			List<怪物视图> list;
			list = new List<怪物视图>();
			foreach (地图对象 item in 地图实例.对象列表.ToList())
			{
				if (item.对象类型 == 游戏对象类型.怪物)
				{
					list.Add(new 怪物视图(item as 怪物实例));
				}
			}
			this.gridControl1.DataSource = list;
			this.gridView1.RefreshData();
		}

		private void treeList1_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void ColumnWidth(GridView gridView)
		{
			gridView.OptionsView.ColumnAutoWidth = false;
			gridView.OptionsView.BestFitMode = GridBestFitMode.Fast;
			gridView.BestFitColumns();
		}

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
			System.ComponentModel.ComponentResourceManager resources;
			resources = new System.ComponentModel.ComponentResourceManager(typeof(游戏服务器.窗口视图.MonsterView));
			this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.ReLoadDataBaseButton = new DevExpress.XtraBars.BarButtonItem();
			this.NodeDeleteButton = new DevExpress.XtraBars.BarButtonItem();
			this.NodeAddButton = new DevExpress.XtraBars.BarButtonItem();
			this.SkillNameEdit = new DevExpress.XtraBars.BarEditItem();
			this.SkillIndexEdit = new DevExpress.XtraBars.BarEditItem();
			this.SkillRuneEdit = new DevExpress.XtraBars.BarEditItem();
			this.NodeTimeEdit = new DevExpress.XtraBars.BarEditItem();
			this.NodeTypeEdit = new DevExpress.XtraBars.BarEditItem();
			this.SkillDeleteButton = new DevExpress.XtraBars.BarButtonItem();
			this.SkillAddButton = new DevExpress.XtraBars.BarButtonItem();
			this.SkillHeader = new DevExpress.XtraBars.BarHeaderItem();
			this.SkillAddHeader = new DevExpress.XtraBars.BarHeaderItem();
			this.barHeaderItem1 = new DevExpress.XtraBars.BarHeaderItem();
			this.SkillCopyButton = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.treeList1 = new DevExpress.XtraTreeList.TreeList();
			this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.repositoryItemImageComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
			this.splitterControl4 = new DevExpress.XtraEditors.SplitterControl();
			this.gridControl2 = new DevExpress.XtraGrid.GridControl();
			this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.repositoryItemImageComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
			this.splitterControl5 = new DevExpress.XtraEditors.SplitterControl();
			this.propertyDescriptionControl2 = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
			((System.ComponentModel.ISupportInitialize)this.ribbon).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.treeList1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.repositoryItemImageComboBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.gridControl2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.repositoryItemImageComboBox2).BeginInit();
			base.SuspendLayout();
			this.ribbon.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(35, 32, 35, 32);
			this.ribbon.ExpandCollapseItem.Id = 0;
			this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[16]
			{
				this.ribbon.ExpandCollapseItem,
				this.barButtonItem1,
				this.ReLoadDataBaseButton,
				this.NodeDeleteButton,
				this.NodeAddButton,
				this.SkillNameEdit,
				this.SkillIndexEdit,
				this.SkillRuneEdit,
				this.NodeTimeEdit,
				this.NodeTypeEdit,
				this.SkillDeleteButton,
				this.SkillAddButton,
				this.SkillHeader,
				this.SkillAddHeader,
				this.barHeaderItem1,
				this.SkillCopyButton
			});
			this.ribbon.Location = new System.Drawing.Point(0, 0);
			this.ribbon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ribbon.MaxItemId = 20;
			this.ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
			this.ribbon.Name = "ribbon";
			this.ribbon.OptionsMenuMinWidth = 385;
			this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage1 });
			this.ribbon.Size = new System.Drawing.Size(1237, 148);
			this.barButtonItem1.Caption = "重新加载";
			this.barButtonItem1.Id = 2;
			this.barButtonItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.Image");
			this.barButtonItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.LargeImage");
			this.barButtonItem1.Name = "barButtonItem1";
			this.ReLoadDataBaseButton.Caption = "刷新 数据";
			this.ReLoadDataBaseButton.Id = 4;
			this.ReLoadDataBaseButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ReLoadDataBaseButton.ImageOptions.Image");
			this.ReLoadDataBaseButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ReLoadDataBaseButton.ImageOptions.LargeImage");
			this.ReLoadDataBaseButton.LargeWidth = 50;
			this.ReLoadDataBaseButton.Name = "ReLoadDataBaseButton";
			this.NodeDeleteButton.Caption = "删除";
			this.NodeDeleteButton.Id = 5;
			this.NodeDeleteButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("NodeDeleteButton.ImageOptions.Image");
			this.NodeDeleteButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("NodeDeleteButton.ImageOptions.LargeImage");
			this.NodeDeleteButton.Name = "NodeDeleteButton";
			this.NodeAddButton.Caption = "添加节点";
			this.NodeAddButton.Id = 6;
			this.NodeAddButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("NodeAddButton.ImageOptions.Image");
			this.NodeAddButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("NodeAddButton.ImageOptions.LargeImage");
			this.NodeAddButton.Name = "NodeAddButton";
			this.SkillNameEdit.Caption = "名字";
			this.SkillNameEdit.Edit = null;
			this.SkillNameEdit.Id = 7;
			this.SkillNameEdit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SkillNameEdit.ImageOptions.Image");
			this.SkillNameEdit.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SkillNameEdit.ImageOptions.LargeImage");
			this.SkillNameEdit.Name = "SkillNameEdit";
			this.SkillIndexEdit.Caption = "编号";
			this.SkillIndexEdit.Edit = null;
			this.SkillIndexEdit.EditValue = 0;
			this.SkillIndexEdit.Id = 8;
			this.SkillIndexEdit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SkillIndexEdit.ImageOptions.Image");
			this.SkillIndexEdit.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SkillIndexEdit.ImageOptions.LargeImage");
			this.SkillIndexEdit.Name = "SkillIndexEdit";
			this.SkillRuneEdit.Caption = "铭文";
			this.SkillRuneEdit.Edit = null;
			this.SkillRuneEdit.EditValue = 0;
			this.SkillRuneEdit.Id = 9;
			this.SkillRuneEdit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SkillRuneEdit.ImageOptions.Image");
			this.SkillRuneEdit.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SkillRuneEdit.ImageOptions.LargeImage");
			this.SkillRuneEdit.Name = "SkillRuneEdit";
			this.NodeTimeEdit.Caption = "时间";
			this.NodeTimeEdit.Edit = null;
			this.NodeTimeEdit.EditValue = 0;
			this.NodeTimeEdit.Id = 10;
			this.NodeTimeEdit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("NodeTimeEdit.ImageOptions.Image");
			this.NodeTimeEdit.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("NodeTimeEdit.ImageOptions.LargeImage");
			this.NodeTimeEdit.Name = "NodeTimeEdit";
			this.NodeTypeEdit.Caption = "类型";
			this.NodeTypeEdit.Edit = null;
			this.NodeTypeEdit.Id = 12;
			this.NodeTypeEdit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("NodeTypeEdit.ImageOptions.Image");
			this.NodeTypeEdit.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("NodeTypeEdit.ImageOptions.LargeImage");
			this.NodeTypeEdit.Name = "NodeTypeEdit";
			this.SkillDeleteButton.Caption = "删除";
			this.SkillDeleteButton.Id = 13;
			this.SkillDeleteButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SkillDeleteButton.ImageOptions.Image");
			this.SkillDeleteButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SkillDeleteButton.ImageOptions.LargeImage");
			this.SkillDeleteButton.Name = "SkillDeleteButton";
			this.SkillAddButton.Caption = "添加技能";
			this.SkillAddButton.Id = 14;
			this.SkillAddButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SkillAddButton.ImageOptions.Image");
			this.SkillAddButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SkillAddButton.ImageOptions.LargeImage");
			this.SkillAddButton.Name = "SkillAddButton";
			this.SkillHeader.Caption = "未选中";
			this.SkillHeader.Id = 15;
			this.SkillHeader.Name = "SkillHeader";
			this.SkillAddHeader.Caption = "战士";
			this.SkillAddHeader.Id = 17;
			this.SkillAddHeader.Name = "SkillAddHeader";
			this.barHeaderItem1.Caption = "添加或删除技能节点";
			this.barHeaderItem1.Id = 18;
			this.barHeaderItem1.Name = "barHeaderItem1";
			this.SkillCopyButton.Caption = "复制技能";
			this.SkillCopyButton.Id = 19;
			this.SkillCopyButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SkillCopyButton.ImageOptions.Image");
			this.SkillCopyButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SkillCopyButton.ImageOptions.LargeImage");
			this.SkillCopyButton.Name = "SkillCopyButton";
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.ribbonPageGroup1 });
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "主页";
			this.ribbonPageGroup1.AllowTextClipping = false;
			this.ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonPageGroup1.ItemLinks.Add(this.ReLoadDataBaseButton);
			this.ribbonPageGroup1.Name = "ribbonPageGroup1";
			this.ribbonPageGroup1.Text = "动作";
			this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.treeListColumn1 });
			this.treeList1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeList1.Location = new System.Drawing.Point(0, 148);
			this.treeList1.MenuManager = this.ribbon;
			this.treeList1.Name = "treeList1";
			this.treeList1.OptionsMenu.EnableColumnMenu = false;
			this.treeList1.OptionsMenu.EnableNodeMenu = false;
			this.treeList1.OptionsMenu.ShowAddNodeItems = DevExpress.Utils.DefaultBoolean.False;
			this.treeList1.Size = new System.Drawing.Size(262, 566);
			this.treeList1.TabIndex = 17;
			this.treeList1.RowClick += new DevExpress.XtraTreeList.RowClickEventHandler(treeList1_RowClick);
			this.treeList1.MouseUp += new System.Windows.Forms.MouseEventHandler(treeList1_MouseUp);
			this.treeListColumn1.Caption = "地图列表";
			this.treeListColumn1.FieldName = "地图列表";
			this.treeListColumn1.Name = "treeListColumn1";
			this.treeListColumn1.OptionsColumn.AllowEdit = false;
			this.treeListColumn1.Visible = true;
			this.treeListColumn1.VisibleIndex = 0;
			this.groupControl2.Controls.Add(this.gridControl1);
			this.groupControl2.Controls.Add(this.splitterControl4);
			this.groupControl2.Controls.Add(this.gridControl2);
			this.groupControl2.Controls.Add(this.splitterControl5);
			this.groupControl2.Controls.Add(this.propertyDescriptionControl2);
			this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl2.Location = new System.Drawing.Point(262, 148);
			this.groupControl2.Name = "groupControl2";
			this.groupControl2.Size = new System.Drawing.Size(975, 566);
			this.groupControl2.TabIndex = 22;
			this.groupControl2.Text = "怪物信息";
			this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl1.Location = new System.Drawing.Point(2, 21);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.MenuManager = this.ribbon;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemImageComboBox1 });
			this.gridControl1.Size = new System.Drawing.Size(532, 487);
			this.gridControl1.TabIndex = 0;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.repositoryItemImageComboBox1.AutoHeight = false;
			this.repositoryItemImageComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
			});
			this.repositoryItemImageComboBox1.Name = "repositoryItemImageComboBox1";
			this.splitterControl4.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitterControl4.Location = new System.Drawing.Point(534, 21);
			this.splitterControl4.Name = "splitterControl4";
			this.splitterControl4.Size = new System.Drawing.Size(6, 487);
			this.splitterControl4.TabIndex = 23;
			this.splitterControl4.TabStop = false;
			this.gridControl2.Dock = System.Windows.Forms.DockStyle.Right;
			this.gridControl2.Location = new System.Drawing.Point(540, 21);
			this.gridControl2.MainView = this.gridView2;
			this.gridControl2.MenuManager = this.ribbon;
			this.gridControl2.Name = "gridControl2";
			this.gridControl2.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemImageComboBox2 });
			this.gridControl2.Size = new System.Drawing.Size(433, 487);
			this.gridControl2.TabIndex = 27;
			this.gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView2 });
			this.gridView2.GridControl = this.gridControl2;
			this.gridView2.Name = "gridView2";
			this.repositoryItemImageComboBox2.AutoHeight = false;
			this.repositoryItemImageComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
			});
			this.repositoryItemImageComboBox2.Name = "repositoryItemImageComboBox2";
			this.splitterControl5.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitterControl5.Location = new System.Drawing.Point(2, 508);
			this.splitterControl5.Name = "splitterControl5";
			this.splitterControl5.Size = new System.Drawing.Size(971, 6);
			this.splitterControl5.TabIndex = 26;
			this.splitterControl5.TabStop = false;
			this.propertyDescriptionControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyDescriptionControl2.Location = new System.Drawing.Point(2, 514);
			this.propertyDescriptionControl2.Name = "propertyDescriptionControl2";
			this.propertyDescriptionControl2.Size = new System.Drawing.Size(971, 50);
			this.propertyDescriptionControl2.TabIndex = 25;
			base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 14f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1237, 714);
			base.Controls.Add(this.groupControl2);
			base.Controls.Add(this.treeList1);
			base.Controls.Add(this.ribbon);
			base.Name = "MonsterView";
			this.Ribbon = this.ribbon;
			this.Text = "怪物数据";
			((System.ComponentModel.ISupportInitialize)this.ribbon).EndInit();
			((System.ComponentModel.ISupportInitialize)this.treeList1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
			this.groupControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.repositoryItemImageComboBox1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.gridControl2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.repositoryItemImageComboBox2).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
