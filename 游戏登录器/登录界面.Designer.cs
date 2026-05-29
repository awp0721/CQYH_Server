namespace 游戏登录器
{

    public partial class 登录界面 : global::System.Windows.Forms.Form
    {

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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(登录界面));
            主选项卡 = new Sunny.UI.UITabControl();
            账号登录页面 = new System.Windows.Forms.TabPage();
            登录_登录账号按钮 = new Sunny.UI.UISymbolButton();
            登录_忘记密码选项 = new Sunny.UI.UILinkLabel();
            登录_账号密码输入框 = new Sunny.UI.UITextBox();
            登录_注册账号按钮 = new Sunny.UI.UISymbolButton();
            登录_错误提示标签 = new Sunny.UI.UILabel();
            登录_账号名字输入框 = new Sunny.UI.UITextBox();
            账号注册页面 = new System.Windows.Forms.TabPage();
            注册_返回登录按钮 = new Sunny.UI.UISymbolButton();
            注册_错误提示标签 = new Sunny.UI.UILabel();
            注册_注册账号按钮 = new Sunny.UI.UISymbolButton();
            注册_密保答案输入框 = new Sunny.UI.UITextBox();
            注册_账号密码输入框 = new Sunny.UI.UITextBox();
            注册_密保问题输入框 = new Sunny.UI.UITextBox();
            注册_账号名字输入框 = new Sunny.UI.UITextBox();
            密码修改页面 = new System.Windows.Forms.TabPage();
            修改_返回登录按钮 = new Sunny.UI.UISymbolButton();
            修改_错误提示标签 = new Sunny.UI.UILabel();
            修改_修改密码按钮 = new Sunny.UI.UISymbolButton();
            修改_密保答案输入框 = new Sunny.UI.UITextBox();
            修改_账号密码输入框 = new Sunny.UI.UITextBox();
            修改_密保问题输入框 = new Sunny.UI.UITextBox();
            修改_账号名字输入框 = new Sunny.UI.UITextBox();
            启动游戏页面 = new System.Windows.Forms.TabPage();
            启动_当前账号标签 = new Sunny.UI.UISymbolButton();
            启动_选中区服标签 = new Sunny.UI.UILinkLabel();
            启动_注销账号标签 = new Sunny.UI.UILinkLabel();
            启动_选择游戏区服 = new System.Windows.Forms.ListBox();
            启动_进入游戏按钮 = new Sunny.UI.UIButton();
            用户界面计时 = new System.Windows.Forms.Timer(components);
            数据处理计时 = new System.Windows.Forms.Timer(components);
            最小化到托盘 = new System.Windows.Forms.NotifyIcon(components);
            托盘右键菜单 = new System.Windows.Forms.ContextMenuStrip(components);
            打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            游戏进程监测 = new System.Windows.Forms.Timer(components);
            主选项卡.SuspendLayout();
            账号登录页面.SuspendLayout();
            账号注册页面.SuspendLayout();
            密码修改页面.SuspendLayout();
            启动游戏页面.SuspendLayout();
            托盘右键菜单.SuspendLayout();
            SuspendLayout();
            // 
            // 主选项卡
            // 
            主选项卡.Controls.Add(账号登录页面);
            主选项卡.Controls.Add(账号注册页面);
            主选项卡.Controls.Add(密码修改页面);
            主选项卡.Controls.Add(启动游戏页面);
            主选项卡.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            主选项卡.FillColor = System.Drawing.Color.FromArgb(255, 244, 240);
            主选项卡.Font = new System.Drawing.Font("微软雅黑", 12F);
            主选项卡.ItemSize = new System.Drawing.Size(260, 28);
            主选项卡.Location = new System.Drawing.Point(436, 21);
            主选项卡.MainPage = "";
            主选项卡.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            主选项卡.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            主选项卡.Name = "主选项卡";
            主选项卡.SelectedIndex = 0;
            主选项卡.Size = new System.Drawing.Size(386, 478);
            主选项卡.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            主选项卡.Style = Sunny.UI.UIStyle.Custom;
            主选项卡.StyleCustomMode = true;
            主选项卡.TabIndex = 9;
            主选项卡.TabSelectedColor = System.Drawing.Color.FromArgb(56, 56, 56);
            主选项卡.TabSelectedForeColor = System.Drawing.Color.FromArgb(255, 87, 34);
            主选项卡.TabSelectedHighColor = System.Drawing.Color.FromArgb(255, 87, 34);
            主选项卡.TabSelectedHighColorSize = 0;
            主选项卡.TabStop = false;
            主选项卡.TabUnSelectedForeColor = System.Drawing.Color.FromArgb(255, 87, 34);
            主选项卡.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            // 
            // 账号登录页面
            // 
            账号登录页面.BackColor = System.Drawing.Color.FromArgb(255, 244, 240);
            账号登录页面.Controls.Add(登录_登录账号按钮);
            账号登录页面.Controls.Add(登录_忘记密码选项);
            账号登录页面.Controls.Add(登录_账号密码输入框);
            账号登录页面.Controls.Add(登录_注册账号按钮);
            账号登录页面.Controls.Add(登录_错误提示标签);
            账号登录页面.Controls.Add(登录_账号名字输入框);
            账号登录页面.Location = new System.Drawing.Point(0, 28);
            账号登录页面.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            账号登录页面.Name = "账号登录页面";
            账号登录页面.Size = new System.Drawing.Size(386, 450);
            账号登录页面.TabIndex = 0;
            账号登录页面.Text = "账号登录";
            // 
            // 登录_登录账号按钮
            // 
            登录_登录账号按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            登录_登录账号按钮.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_登录账号按钮.FillColor2 = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_登录账号按钮.FillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_登录账号按钮.FillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_登录账号按钮.FillSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_登录账号按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            登录_登录账号按钮.LightColor = System.Drawing.Color.FromArgb(253, 243, 243);
            登录_登录账号按钮.Location = new System.Drawing.Point(67, 221);
            登录_登录账号按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            登录_登录账号按钮.MinimumSize = new System.Drawing.Size(1, 1);
            登录_登录账号按钮.Name = "登录_登录账号按钮";
            登录_登录账号按钮.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_登录账号按钮.RectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_登录账号按钮.RectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_登录账号按钮.RectSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_登录账号按钮.Size = new System.Drawing.Size(253, 41);
            登录_登录账号按钮.Style = Sunny.UI.UIStyle.Custom;
            登录_登录账号按钮.TabIndex = 13;
            登录_登录账号按钮.TabStop = false;
            登录_登录账号按钮.Text = "登录";
            登录_登录账号按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            登录_登录账号按钮.Click += 登录_登录账号按钮_Click;
            // 
            // 登录_忘记密码选项
            // 
            登录_忘记密码选项.ActiveLinkColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_忘记密码选项.AutoSize = true;
            登录_忘记密码选项.Font = new System.Drawing.Font("微软雅黑", 9F);
            登录_忘记密码选项.ForeColor = System.Drawing.Color.FromArgb(48, 48, 48);
            登录_忘记密码选项.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            登录_忘记密码选项.Location = new System.Drawing.Point(258, 143);
            登录_忘记密码选项.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            登录_忘记密码选项.Name = "登录_忘记密码选项";
            登录_忘记密码选项.Size = new System.Drawing.Size(62, 17);
            登录_忘记密码选项.Style = Sunny.UI.UIStyle.Custom;
            登录_忘记密码选项.TabIndex = 16;
            登录_忘记密码选项.TabStop = true;
            登录_忘记密码选项.Text = "忘记密码?";
            登录_忘记密码选项.VisitedLinkColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_忘记密码选项.Click += 登录_忘记密码选项_Click;
            // 
            // 登录_账号密码输入框
            // 
            登录_账号密码输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号密码输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_账号密码输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_账号密码输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号密码输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_账号密码输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_账号密码输入框.ButtonStyleInherited = false;
            登录_账号密码输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            登录_账号密码输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            登录_账号密码输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            登录_账号密码输入框.Location = new System.Drawing.Point(67, 82);
            登录_账号密码输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            登录_账号密码输入框.MinimumSize = new System.Drawing.Size(1, 16);
            登录_账号密码输入框.Name = "登录_账号密码输入框";
            登录_账号密码输入框.Padding = new System.Windows.Forms.Padding(5);
            登录_账号密码输入框.PasswordChar = '*';
            登录_账号密码输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号密码输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号密码输入框.ScrollBarStyleInherited = false;
            登录_账号密码输入框.ShowText = false;
            登录_账号密码输入框.Size = new System.Drawing.Size(253, 41);
            登录_账号密码输入框.Style = Sunny.UI.UIStyle.Custom;
            登录_账号密码输入框.Symbol = 61475;
            登录_账号密码输入框.SymbolSize = 22;
            登录_账号密码输入框.TabIndex = 2;
            登录_账号密码输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            登录_账号密码输入框.Watermark = "请输入密码";
            // 
            // 登录_注册账号按钮
            // 
            登录_注册账号按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            登录_注册账号按钮.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_注册账号按钮.FillColor2 = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_注册账号按钮.FillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_注册账号按钮.FillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_注册账号按钮.FillSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_注册账号按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            登录_注册账号按钮.LightColor = System.Drawing.Color.FromArgb(253, 243, 243);
            登录_注册账号按钮.Location = new System.Drawing.Point(67, 288);
            登录_注册账号按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            登录_注册账号按钮.MinimumSize = new System.Drawing.Size(1, 1);
            登录_注册账号按钮.Name = "登录_注册账号按钮";
            登录_注册账号按钮.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_注册账号按钮.RectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_注册账号按钮.RectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_注册账号按钮.RectSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_注册账号按钮.Size = new System.Drawing.Size(253, 41);
            登录_注册账号按钮.Style = Sunny.UI.UIStyle.Custom;
            登录_注册账号按钮.Symbol = 62004;
            登录_注册账号按钮.TabIndex = 14;
            登录_注册账号按钮.TabStop = false;
            登录_注册账号按钮.Text = "注册";
            登录_注册账号按钮.TipsColor = System.Drawing.Color.FromArgb(128, 255, 128);
            登录_注册账号按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            登录_注册账号按钮.Click += 登录_注册账号按钮_Click;
            // 
            // 登录_错误提示标签
            // 
            登录_错误提示标签.AutoSize = true;
            登录_错误提示标签.Font = new System.Drawing.Font("微软雅黑", 9F);
            登录_错误提示标签.ForeColor = System.Drawing.Color.Red;
            登录_错误提示标签.Location = new System.Drawing.Point(67, 179);
            登录_错误提示标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            登录_错误提示标签.Name = "登录_错误提示标签";
            登录_错误提示标签.Size = new System.Drawing.Size(56, 17);
            登录_错误提示标签.Style = Sunny.UI.UIStyle.Custom;
            登录_错误提示标签.TabIndex = 15;
            登录_错误提示标签.Text = "错误提示";
            登录_错误提示标签.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            登录_错误提示标签.Visible = false;
            // 
            // 登录_账号名字输入框
            // 
            登录_账号名字输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号名字输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_账号名字输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_账号名字输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号名字输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            登录_账号名字输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            登录_账号名字输入框.ButtonStyleInherited = false;
            登录_账号名字输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            登录_账号名字输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            登录_账号名字输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            登录_账号名字输入框.Location = new System.Drawing.Point(67, 30);
            登录_账号名字输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            登录_账号名字输入框.MinimumSize = new System.Drawing.Size(1, 16);
            登录_账号名字输入框.Name = "登录_账号名字输入框";
            登录_账号名字输入框.Padding = new System.Windows.Forms.Padding(5);
            登录_账号名字输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号名字输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            登录_账号名字输入框.ScrollBarStyleInherited = false;
            登录_账号名字输入框.ShowText = false;
            登录_账号名字输入框.Size = new System.Drawing.Size(253, 41);
            登录_账号名字输入框.Style = Sunny.UI.UIStyle.Custom;
            登录_账号名字输入框.Symbol = 61447;
            登录_账号名字输入框.SymbolSize = 22;
            登录_账号名字输入框.TabIndex = 1;
            登录_账号名字输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            登录_账号名字输入框.Watermark = "请输入账号";
            // 
            // 账号注册页面
            // 
            账号注册页面.BackColor = System.Drawing.Color.FromArgb(255, 244, 240);
            账号注册页面.Controls.Add(注册_返回登录按钮);
            账号注册页面.Controls.Add(注册_错误提示标签);
            账号注册页面.Controls.Add(注册_注册账号按钮);
            账号注册页面.Controls.Add(注册_密保答案输入框);
            账号注册页面.Controls.Add(注册_账号密码输入框);
            账号注册页面.Controls.Add(注册_密保问题输入框);
            账号注册页面.Controls.Add(注册_账号名字输入框);
            账号注册页面.Location = new System.Drawing.Point(0, 28);
            账号注册页面.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            账号注册页面.Name = "账号注册页面";
            账号注册页面.Size = new System.Drawing.Size(386, 450);
            账号注册页面.TabIndex = 1;
            账号注册页面.Text = "账号注册";
            // 
            // 注册_返回登录按钮
            // 
            注册_返回登录按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            注册_返回登录按钮.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_返回登录按钮.FillColor2 = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_返回登录按钮.FillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_返回登录按钮.FillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_返回登录按钮.FillSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_返回登录按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            注册_返回登录按钮.LightColor = System.Drawing.Color.FromArgb(253, 243, 243);
            注册_返回登录按钮.Location = new System.Drawing.Point(74, 313);
            注册_返回登录按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            注册_返回登录按钮.MinimumSize = new System.Drawing.Size(1, 1);
            注册_返回登录按钮.Name = "注册_返回登录按钮";
            注册_返回登录按钮.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_返回登录按钮.RectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_返回登录按钮.RectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_返回登录按钮.RectSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_返回登录按钮.Size = new System.Drawing.Size(248, 42);
            注册_返回登录按钮.Style = Sunny.UI.UIStyle.Custom;
            注册_返回登录按钮.Symbol = 61730;
            注册_返回登录按钮.TabIndex = 20;
            注册_返回登录按钮.TabStop = false;
            注册_返回登录按钮.Text = "返回登录";
            注册_返回登录按钮.TipsColor = System.Drawing.Color.FromArgb(128, 255, 128);
            注册_返回登录按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            注册_返回登录按钮.Click += 注册_返回登录按钮_Click;
            // 
            // 注册_错误提示标签
            // 
            注册_错误提示标签.AutoSize = true;
            注册_错误提示标签.Font = new System.Drawing.Font("微软雅黑", 9F);
            注册_错误提示标签.ForeColor = System.Drawing.Color.Red;
            注册_错误提示标签.Location = new System.Drawing.Point(74, 222);
            注册_错误提示标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            注册_错误提示标签.Name = "注册_错误提示标签";
            注册_错误提示标签.Size = new System.Drawing.Size(56, 17);
            注册_错误提示标签.Style = Sunny.UI.UIStyle.Custom;
            注册_错误提示标签.TabIndex = 17;
            注册_错误提示标签.Text = "错误提示";
            注册_错误提示标签.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            注册_错误提示标签.Visible = false;
            // 
            // 注册_注册账号按钮
            // 
            注册_注册账号按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            注册_注册账号按钮.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_注册账号按钮.FillColor2 = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_注册账号按钮.FillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_注册账号按钮.FillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_注册账号按钮.FillSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_注册账号按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            注册_注册账号按钮.LightColor = System.Drawing.Color.FromArgb(253, 243, 243);
            注册_注册账号按钮.Location = new System.Drawing.Point(74, 255);
            注册_注册账号按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            注册_注册账号按钮.MinimumSize = new System.Drawing.Size(1, 1);
            注册_注册账号按钮.Name = "注册_注册账号按钮";
            注册_注册账号按钮.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_注册账号按钮.RectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_注册账号按钮.RectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_注册账号按钮.RectSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_注册账号按钮.Size = new System.Drawing.Size(248, 42);
            注册_注册账号按钮.Style = Sunny.UI.UIStyle.Custom;
            注册_注册账号按钮.Symbol = 62004;
            注册_注册账号按钮.TabIndex = 16;
            注册_注册账号按钮.TabStop = false;
            注册_注册账号按钮.Text = "注册账号";
            注册_注册账号按钮.TipsColor = System.Drawing.Color.FromArgb(128, 255, 128);
            注册_注册账号按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            注册_注册账号按钮.Click += 注册_注册账号按钮_Click;
            // 
            // 注册_密保答案输入框
            // 
            注册_密保答案输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保答案输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_密保答案输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_密保答案输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保答案输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_密保答案输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_密保答案输入框.ButtonStyleInherited = false;
            注册_密保答案输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            注册_密保答案输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            注册_密保答案输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            注册_密保答案输入框.Location = new System.Drawing.Point(74, 174);
            注册_密保答案输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            注册_密保答案输入框.MinimumSize = new System.Drawing.Size(1, 16);
            注册_密保答案输入框.Name = "注册_密保答案输入框";
            注册_密保答案输入框.Padding = new System.Windows.Forms.Padding(5);
            注册_密保答案输入框.PasswordChar = '*';
            注册_密保答案输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保答案输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保答案输入框.ScrollBarStyleInherited = false;
            注册_密保答案输入框.ShowText = false;
            注册_密保答案输入框.Size = new System.Drawing.Size(248, 42);
            注册_密保答案输入框.Style = Sunny.UI.UIStyle.Custom;
            注册_密保答案输入框.Symbol = 61716;
            注册_密保答案输入框.SymbolSize = 22;
            注册_密保答案输入框.TabIndex = 4;
            注册_密保答案输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            注册_密保答案输入框.Watermark = "请输入密保答案";
            // 
            // 注册_账号密码输入框
            // 
            注册_账号密码输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号密码输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_账号密码输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_账号密码输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号密码输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_账号密码输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_账号密码输入框.ButtonStyleInherited = false;
            注册_账号密码输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            注册_账号密码输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            注册_账号密码输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            注册_账号密码输入框.Location = new System.Drawing.Point(74, 70);
            注册_账号密码输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            注册_账号密码输入框.MinimumSize = new System.Drawing.Size(1, 16);
            注册_账号密码输入框.Name = "注册_账号密码输入框";
            注册_账号密码输入框.Padding = new System.Windows.Forms.Padding(5);
            注册_账号密码输入框.PasswordChar = '*';
            注册_账号密码输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号密码输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号密码输入框.ScrollBarStyleInherited = false;
            注册_账号密码输入框.ShowText = false;
            注册_账号密码输入框.Size = new System.Drawing.Size(248, 42);
            注册_账号密码输入框.Style = Sunny.UI.UIStyle.Custom;
            注册_账号密码输入框.Symbol = 61475;
            注册_账号密码输入框.SymbolSize = 22;
            注册_账号密码输入框.TabIndex = 2;
            注册_账号密码输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            注册_账号密码输入框.Watermark = "请输入密码";
            // 
            // 注册_密保问题输入框
            // 
            注册_密保问题输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保问题输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_密保问题输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_密保问题输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保问题输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_密保问题输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_密保问题输入框.ButtonStyleInherited = false;
            注册_密保问题输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            注册_密保问题输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            注册_密保问题输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            注册_密保问题输入框.Location = new System.Drawing.Point(74, 122);
            注册_密保问题输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            注册_密保问题输入框.MinimumSize = new System.Drawing.Size(1, 16);
            注册_密保问题输入框.Name = "注册_密保问题输入框";
            注册_密保问题输入框.Padding = new System.Windows.Forms.Padding(5);
            注册_密保问题输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保问题输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_密保问题输入框.ScrollBarStyleInherited = false;
            注册_密保问题输入框.ShowText = false;
            注册_密保问题输入框.Size = new System.Drawing.Size(248, 42);
            注册_密保问题输入框.Style = Sunny.UI.UIStyle.Custom;
            注册_密保问题输入框.Symbol = 61563;
            注册_密保问题输入框.SymbolSize = 22;
            注册_密保问题输入框.TabIndex = 3;
            注册_密保问题输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            注册_密保问题输入框.Watermark = "请输入密保问题";
            // 
            // 注册_账号名字输入框
            // 
            注册_账号名字输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号名字输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_账号名字输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_账号名字输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号名字输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            注册_账号名字输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            注册_账号名字输入框.ButtonStyleInherited = false;
            注册_账号名字输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            注册_账号名字输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            注册_账号名字输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            注册_账号名字输入框.Location = new System.Drawing.Point(74, 17);
            注册_账号名字输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            注册_账号名字输入框.MinimumSize = new System.Drawing.Size(1, 16);
            注册_账号名字输入框.Name = "注册_账号名字输入框";
            注册_账号名字输入框.Padding = new System.Windows.Forms.Padding(5);
            注册_账号名字输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号名字输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            注册_账号名字输入框.ScrollBarStyleInherited = false;
            注册_账号名字输入框.ShowText = false;
            注册_账号名字输入框.Size = new System.Drawing.Size(248, 42);
            注册_账号名字输入框.Style = Sunny.UI.UIStyle.Custom;
            注册_账号名字输入框.Symbol = 61447;
            注册_账号名字输入框.SymbolSize = 22;
            注册_账号名字输入框.TabIndex = 1;
            注册_账号名字输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            注册_账号名字输入框.Watermark = "请输入账号";
            // 
            // 密码修改页面
            // 
            密码修改页面.BackColor = System.Drawing.Color.FromArgb(255, 244, 240);
            密码修改页面.Controls.Add(修改_返回登录按钮);
            密码修改页面.Controls.Add(修改_错误提示标签);
            密码修改页面.Controls.Add(修改_修改密码按钮);
            密码修改页面.Controls.Add(修改_密保答案输入框);
            密码修改页面.Controls.Add(修改_账号密码输入框);
            密码修改页面.Controls.Add(修改_密保问题输入框);
            密码修改页面.Controls.Add(修改_账号名字输入框);
            密码修改页面.Location = new System.Drawing.Point(0, 40);
            密码修改页面.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            密码修改页面.Name = "密码修改页面";
            密码修改页面.Size = new System.Drawing.Size(200, 60);
            密码修改页面.TabIndex = 2;
            密码修改页面.Text = "密码修改";
            // 
            // 修改_返回登录按钮
            // 
            修改_返回登录按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            修改_返回登录按钮.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_返回登录按钮.FillColor2 = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_返回登录按钮.FillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_返回登录按钮.FillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_返回登录按钮.FillSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_返回登录按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            修改_返回登录按钮.LightColor = System.Drawing.Color.FromArgb(253, 243, 243);
            修改_返回登录按钮.Location = new System.Drawing.Point(72, 312);
            修改_返回登录按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            修改_返回登录按钮.MinimumSize = new System.Drawing.Size(1, 1);
            修改_返回登录按钮.Name = "修改_返回登录按钮";
            修改_返回登录按钮.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_返回登录按钮.RectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_返回登录按钮.RectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_返回登录按钮.RectSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_返回登录按钮.Size = new System.Drawing.Size(248, 42);
            修改_返回登录按钮.Style = Sunny.UI.UIStyle.Custom;
            修改_返回登录按钮.Symbol = 61730;
            修改_返回登录按钮.TabIndex = 24;
            修改_返回登录按钮.TabStop = false;
            修改_返回登录按钮.Text = "返回登录";
            修改_返回登录按钮.TipsColor = System.Drawing.Color.FromArgb(128, 255, 128);
            修改_返回登录按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            修改_返回登录按钮.Click += 修改_返回登录按钮_Click;
            // 
            // 修改_错误提示标签
            // 
            修改_错误提示标签.AutoSize = true;
            修改_错误提示标签.Font = new System.Drawing.Font("微软雅黑", 9F);
            修改_错误提示标签.ForeColor = System.Drawing.Color.Red;
            修改_错误提示标签.Location = new System.Drawing.Point(72, 223);
            修改_错误提示标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            修改_错误提示标签.Name = "修改_错误提示标签";
            修改_错误提示标签.Size = new System.Drawing.Size(56, 17);
            修改_错误提示标签.Style = Sunny.UI.UIStyle.Custom;
            修改_错误提示标签.TabIndex = 22;
            修改_错误提示标签.Text = "错误提示";
            修改_错误提示标签.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            修改_错误提示标签.Visible = false;
            // 
            // 修改_修改密码按钮
            // 
            修改_修改密码按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            修改_修改密码按钮.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_修改密码按钮.FillColor2 = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_修改密码按钮.FillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_修改密码按钮.FillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_修改密码按钮.FillSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_修改密码按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            修改_修改密码按钮.LightColor = System.Drawing.Color.FromArgb(253, 243, 243);
            修改_修改密码按钮.Location = new System.Drawing.Point(72, 255);
            修改_修改密码按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            修改_修改密码按钮.MinimumSize = new System.Drawing.Size(1, 1);
            修改_修改密码按钮.Name = "修改_修改密码按钮";
            修改_修改密码按钮.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_修改密码按钮.RectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_修改密码按钮.RectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_修改密码按钮.RectSelectedColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_修改密码按钮.Size = new System.Drawing.Size(248, 42);
            修改_修改密码按钮.Style = Sunny.UI.UIStyle.Custom;
            修改_修改密码按钮.Symbol = 61573;
            修改_修改密码按钮.TabIndex = 21;
            修改_修改密码按钮.TabStop = false;
            修改_修改密码按钮.Text = "修改密码";
            修改_修改密码按钮.TipsColor = System.Drawing.Color.FromArgb(128, 255, 128);
            修改_修改密码按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            修改_修改密码按钮.Click += 修改_修改密码按钮_Click;
            // 
            // 修改_密保答案输入框
            // 
            修改_密保答案输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保答案输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_密保答案输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_密保答案输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保答案输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_密保答案输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_密保答案输入框.ButtonStyleInherited = false;
            修改_密保答案输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            修改_密保答案输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            修改_密保答案输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            修改_密保答案输入框.Location = new System.Drawing.Point(72, 175);
            修改_密保答案输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            修改_密保答案输入框.MinimumSize = new System.Drawing.Size(1, 16);
            修改_密保答案输入框.Name = "修改_密保答案输入框";
            修改_密保答案输入框.Padding = new System.Windows.Forms.Padding(5);
            修改_密保答案输入框.PasswordChar = '*';
            修改_密保答案输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保答案输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保答案输入框.ScrollBarStyleInherited = false;
            修改_密保答案输入框.ShowText = false;
            修改_密保答案输入框.Size = new System.Drawing.Size(248, 42);
            修改_密保答案输入框.Style = Sunny.UI.UIStyle.Custom;
            修改_密保答案输入框.Symbol = 61716;
            修改_密保答案输入框.SymbolSize = 22;
            修改_密保答案输入框.TabIndex = 20;
            修改_密保答案输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            修改_密保答案输入框.Watermark = "请输入密保答案";
            // 
            // 修改_账号密码输入框
            // 
            修改_账号密码输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号密码输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_账号密码输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_账号密码输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号密码输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_账号密码输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_账号密码输入框.ButtonStyleInherited = false;
            修改_账号密码输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            修改_账号密码输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            修改_账号密码输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            修改_账号密码输入框.Location = new System.Drawing.Point(72, 71);
            修改_账号密码输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            修改_账号密码输入框.MinimumSize = new System.Drawing.Size(1, 16);
            修改_账号密码输入框.Name = "修改_账号密码输入框";
            修改_账号密码输入框.Padding = new System.Windows.Forms.Padding(5);
            修改_账号密码输入框.PasswordChar = '*';
            修改_账号密码输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号密码输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号密码输入框.ScrollBarStyleInherited = false;
            修改_账号密码输入框.ShowText = false;
            修改_账号密码输入框.Size = new System.Drawing.Size(248, 42);
            修改_账号密码输入框.Style = Sunny.UI.UIStyle.Custom;
            修改_账号密码输入框.Symbol = 61475;
            修改_账号密码输入框.SymbolSize = 22;
            修改_账号密码输入框.TabIndex = 18;
            修改_账号密码输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            修改_账号密码输入框.Watermark = "请输入新的密码";
            // 
            // 修改_密保问题输入框
            // 
            修改_密保问题输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保问题输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_密保问题输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_密保问题输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保问题输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_密保问题输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_密保问题输入框.ButtonStyleInherited = false;
            修改_密保问题输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            修改_密保问题输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            修改_密保问题输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            修改_密保问题输入框.Location = new System.Drawing.Point(72, 123);
            修改_密保问题输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            修改_密保问题输入框.MinimumSize = new System.Drawing.Size(1, 16);
            修改_密保问题输入框.Name = "修改_密保问题输入框";
            修改_密保问题输入框.Padding = new System.Windows.Forms.Padding(5);
            修改_密保问题输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保问题输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_密保问题输入框.ScrollBarStyleInherited = false;
            修改_密保问题输入框.ShowText = false;
            修改_密保问题输入框.Size = new System.Drawing.Size(248, 42);
            修改_密保问题输入框.Style = Sunny.UI.UIStyle.Custom;
            修改_密保问题输入框.Symbol = 61563;
            修改_密保问题输入框.SymbolSize = 22;
            修改_密保问题输入框.TabIndex = 19;
            修改_密保问题输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            修改_密保问题输入框.Watermark = "请输入密保问题";
            // 
            // 修改_账号名字输入框
            // 
            修改_账号名字输入框.ButtonFillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号名字输入框.ButtonFillHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_账号名字输入框.ButtonFillPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_账号名字输入框.ButtonRectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号名字输入框.ButtonRectHoverColor = System.Drawing.Color.FromArgb(235, 115, 115);
            修改_账号名字输入框.ButtonRectPressColor = System.Drawing.Color.FromArgb(184, 64, 64);
            修改_账号名字输入框.ButtonStyleInherited = false;
            修改_账号名字输入框.Cursor = System.Windows.Forms.Cursors.IBeam;
            修改_账号名字输入框.FillColor2 = System.Drawing.Color.FromArgb(253, 243, 243);
            修改_账号名字输入框.Font = new System.Drawing.Font("微软雅黑", 12F);
            修改_账号名字输入框.Location = new System.Drawing.Point(72, 18);
            修改_账号名字输入框.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            修改_账号名字输入框.MinimumSize = new System.Drawing.Size(1, 16);
            修改_账号名字输入框.Name = "修改_账号名字输入框";
            修改_账号名字输入框.Padding = new System.Windows.Forms.Padding(5);
            修改_账号名字输入框.RectColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号名字输入框.ScrollBarColor = System.Drawing.Color.FromArgb(230, 80, 80);
            修改_账号名字输入框.ScrollBarStyleInherited = false;
            修改_账号名字输入框.ShowText = false;
            修改_账号名字输入框.Size = new System.Drawing.Size(248, 42);
            修改_账号名字输入框.Style = Sunny.UI.UIStyle.Custom;
            修改_账号名字输入框.Symbol = 61447;
            修改_账号名字输入框.SymbolSize = 22;
            修改_账号名字输入框.TabIndex = 17;
            修改_账号名字输入框.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            修改_账号名字输入框.Watermark = "请输入已有账号";
            // 
            // 启动游戏页面
            // 
            启动游戏页面.BackColor = System.Drawing.Color.FromArgb(255, 244, 240);
            启动游戏页面.Controls.Add(启动_当前账号标签);
            启动游戏页面.Controls.Add(启动_选中区服标签);
            启动游戏页面.Controls.Add(启动_注销账号标签);
            启动游戏页面.Controls.Add(启动_选择游戏区服);
            启动游戏页面.Controls.Add(启动_进入游戏按钮);
            启动游戏页面.Location = new System.Drawing.Point(0, 28);
            启动游戏页面.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            启动游戏页面.Name = "启动游戏页面";
            启动游戏页面.Size = new System.Drawing.Size(386, 450);
            启动游戏页面.TabIndex = 3;
            启动游戏页面.Text = "启动游戏";
            // 
            // 启动_当前账号标签
            // 
            启动_当前账号标签.Cursor = System.Windows.Forms.Cursors.Hand;
            启动_当前账号标签.Enabled = false;
            启动_当前账号标签.FillColor = System.Drawing.Color.FromArgb(110, 190, 40);
            启动_当前账号标签.FillColor2 = System.Drawing.Color.FromArgb(110, 190, 40);
            启动_当前账号标签.FillHoverColor = System.Drawing.Color.FromArgb(139, 203, 83);
            启动_当前账号标签.FillPressColor = System.Drawing.Color.FromArgb(88, 152, 32);
            启动_当前账号标签.FillSelectedColor = System.Drawing.Color.FromArgb(88, 152, 32);
            启动_当前账号标签.Font = new System.Drawing.Font("微软雅黑", 12F);
            启动_当前账号标签.LightColor = System.Drawing.Color.FromArgb(245, 251, 241);
            启动_当前账号标签.Location = new System.Drawing.Point(79, 3);
            启动_当前账号标签.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            启动_当前账号标签.MinimumSize = new System.Drawing.Size(1, 1);
            启动_当前账号标签.Name = "启动_当前账号标签";
            启动_当前账号标签.Radius = 15;
            启动_当前账号标签.RectColor = System.Drawing.Color.FromArgb(110, 190, 40);
            启动_当前账号标签.RectHoverColor = System.Drawing.Color.FromArgb(139, 203, 83);
            启动_当前账号标签.RectPressColor = System.Drawing.Color.FromArgb(88, 152, 32);
            启动_当前账号标签.RectSelectedColor = System.Drawing.Color.FromArgb(88, 152, 32);
            启动_当前账号标签.Size = new System.Drawing.Size(220, 27);
            启动_当前账号标签.Style = Sunny.UI.UIStyle.Custom;
            启动_当前账号标签.Symbol = 57607;
            启动_当前账号标签.TabIndex = 9;
            启动_当前账号标签.Text = "mistyes";
            启动_当前账号标签.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            // 
            // 启动_选中区服标签
            // 
            启动_选中区服标签.ActiveLinkColor = System.Drawing.Color.FromArgb(220, 155, 40);
            启动_选中区服标签.Font = new System.Drawing.Font("微软雅黑", 12F);
            启动_选中区服标签.ForeColor = System.Drawing.Color.FromArgb(48, 48, 48);
            启动_选中区服标签.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            启动_选中区服标签.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            启动_选中区服标签.LinkColor = System.Drawing.Color.FromArgb(192, 64, 0);
            启动_选中区服标签.Location = new System.Drawing.Point(275, 103);
            启动_选中区服标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            启动_选中区服标签.Name = "启动_选中区服标签";
            启动_选中区服标签.Size = new System.Drawing.Size(98, 26);
            启动_选中区服标签.Style = Sunny.UI.UIStyle.Custom;
            启动_选中区服标签.TabIndex = 7;
            启动_选中区服标签.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            启动_选中区服标签.VisitedLinkColor = System.Drawing.Color.FromArgb(230, 80, 80);
            // 
            // 启动_注销账号标签
            // 
            启动_注销账号标签.ActiveLinkColor = System.Drawing.Color.FromArgb(220, 155, 40);
            启动_注销账号标签.Font = new System.Drawing.Font("微软雅黑", 9F);
            启动_注销账号标签.ForeColor = System.Drawing.Color.FromArgb(48, 48, 48);
            启动_注销账号标签.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            启动_注销账号标签.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            启动_注销账号标签.LinkColor = System.Drawing.Color.Red;
            启动_注销账号标签.Location = new System.Drawing.Point(275, 36);
            启动_注销账号标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            启动_注销账号标签.Name = "启动_注销账号标签";
            启动_注销账号标签.Size = new System.Drawing.Size(47, 24);
            启动_注销账号标签.Style = Sunny.UI.UIStyle.Custom;
            启动_注销账号标签.TabIndex = 6;
            启动_注销账号标签.TabStop = true;
            启动_注销账号标签.Text = "退出";
            启动_注销账号标签.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            启动_注销账号标签.VisitedLinkColor = System.Drawing.Color.FromArgb(230, 80, 80);
            启动_注销账号标签.Click += 启动_注销账号标签_Click;
            // 
            // 启动_选择游戏区服
            // 
            启动_选择游戏区服.BackColor = System.Drawing.Color.Wheat;
            启动_选择游戏区服.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            启动_选择游戏区服.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            启动_选择游戏区服.ForeColor = System.Drawing.Color.Blue;
            启动_选择游戏区服.FormattingEnabled = true;
            启动_选择游戏区服.ItemHeight = 20;
            启动_选择游戏区服.Items.AddRange(new object[] { "魔龙谷", "伤心树" });
            启动_选择游戏区服.Location = new System.Drawing.Point(128, 36);
            启动_选择游戏区服.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            启动_选择游戏区服.Name = "启动_选择游戏区服";
            启动_选择游戏区服.Size = new System.Drawing.Size(139, 241);
            启动_选择游戏区服.TabIndex = 4;
            启动_选择游戏区服.TabStop = false;
            启动_选择游戏区服.DrawItem += 启动_选择游戏区服_DrawItem;
            启动_选择游戏区服.SelectedIndexChanged += 启动_选择游戏区服_SelectedIndexChanged;
            // 
            // 启动_进入游戏按钮
            // 
            启动_进入游戏按钮.Cursor = System.Windows.Forms.Cursors.Hand;
            启动_进入游戏按钮.FillColor = System.Drawing.Color.FromArgb(255, 87, 34);
            启动_进入游戏按钮.FillColor2 = System.Drawing.Color.FromArgb(255, 87, 34);
            启动_进入游戏按钮.FillHoverColor = System.Drawing.Color.FromArgb(255, 121, 78);
            启动_进入游戏按钮.FillPressColor = System.Drawing.Color.FromArgb(204, 70, 28);
            启动_进入游戏按钮.FillSelectedColor = System.Drawing.Color.FromArgb(204, 70, 28);
            启动_进入游戏按钮.Font = new System.Drawing.Font("微软雅黑", 12F);
            启动_进入游戏按钮.LightColor = System.Drawing.Color.FromArgb(255, 244, 240);
            启动_进入游戏按钮.Location = new System.Drawing.Point(79, 292);
            启动_进入游戏按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            启动_进入游戏按钮.MinimumSize = new System.Drawing.Size(1, 1);
            启动_进入游戏按钮.Name = "启动_进入游戏按钮";
            启动_进入游戏按钮.RectColor = System.Drawing.Color.FromArgb(255, 87, 34);
            启动_进入游戏按钮.RectHoverColor = System.Drawing.Color.FromArgb(255, 121, 78);
            启动_进入游戏按钮.RectPressColor = System.Drawing.Color.FromArgb(204, 70, 28);
            启动_进入游戏按钮.RectSelectedColor = System.Drawing.Color.FromArgb(204, 70, 28);
            启动_进入游戏按钮.Size = new System.Drawing.Size(220, 42);
            启动_进入游戏按钮.Style = Sunny.UI.UIStyle.Custom;
            启动_进入游戏按钮.TabIndex = 1;
            启动_进入游戏按钮.TabStop = false;
            启动_进入游戏按钮.Text = "进入游戏";
            启动_进入游戏按钮.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            启动_进入游戏按钮.Click += 启动_进入游戏按钮_Click;
            // 
            // 用户界面计时
            // 
            用户界面计时.Interval = 30000;
            用户界面计时.Tick += 登录请求超时;
            // 
            // 数据处理计时
            // 
            数据处理计时.Enabled = true;
            数据处理计时.Tick += 数据接收处理;
            // 
            // 最小化到托盘
            // 
            最小化到托盘.ContextMenuStrip = 托盘右键菜单;
            最小化到托盘.Icon = (System.Drawing.Icon)resources.GetObject("最小化到托盘.Icon");
            最小化到托盘.Text = "登录器";
            最小化到托盘.MouseClick += 托盘_恢复到任务栏;
            // 
            // 托盘右键菜单
            // 
            托盘右键菜单.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { 打开ToolStripMenuItem, 退出ToolStripMenuItem });
            托盘右键菜单.Name = "托盘右键菜单";
            托盘右键菜单.Size = new System.Drawing.Size(101, 48);
            // 
            // 打开ToolStripMenuItem
            // 
            打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            打开ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            打开ToolStripMenuItem.Text = "打开";
            打开ToolStripMenuItem.Click += 托盘_恢复到任务栏;
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 托盘_彻底关闭应用;
            // 
            // 游戏进程监测
            // 
            游戏进程监测.Enabled = true;
            游戏进程监测.Tick += 游戏进程检查;
            // 
            // 登录界面
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.登录器;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(824, 511);
            Controls.Add(主选项卡);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "登录界面";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Elaina Engine - 登录器";
            FormClosing += 托盘_隐藏到托盘区;
            Load += 登录界面_Load;
            主选项卡.ResumeLayout(false);
            账号登录页面.ResumeLayout(false);
            账号登录页面.PerformLayout();
            账号注册页面.ResumeLayout(false);
            账号注册页面.PerformLayout();
            密码修改页面.ResumeLayout(false);
            密码修改页面.PerformLayout();
            启动游戏页面.ResumeLayout(false);
            托盘右键菜单.ResumeLayout(false);
            ResumeLayout(false);
        }

        private global::System.ComponentModel.IContainer components;

        private global::System.Windows.Forms.TabPage 账号登录页面;

        private global::Sunny.UI.UILinkLabel 登录_忘记密码选项;

        private global::Sunny.UI.UISymbolButton 登录_注册账号按钮;

        private global::Sunny.UI.UISymbolButton 登录_登录账号按钮;

        private global::Sunny.UI.UITextBox 登录_账号密码输入框;

        private global::Sunny.UI.UITextBox 登录_账号名字输入框;

        private global::System.Windows.Forms.TabPage 账号注册页面;

        private global::System.Windows.Forms.TabPage 密码修改页面;

        private global::System.Windows.Forms.TabPage 启动游戏页面;

        private global::Sunny.UI.UISymbolButton 注册_注册账号按钮;

        private global::Sunny.UI.UITextBox 注册_密保答案输入框;

        private global::Sunny.UI.UITextBox 注册_账号密码输入框;

        private global::Sunny.UI.UITextBox 注册_密保问题输入框;

        private global::Sunny.UI.UITextBox 注册_账号名字输入框;

        private global::Sunny.UI.UISymbolButton 修改_修改密码按钮;

        private global::Sunny.UI.UITextBox 修改_密保答案输入框;

        private global::Sunny.UI.UITextBox 修改_账号密码输入框;

        private global::Sunny.UI.UITextBox 修改_密保问题输入框;

        private global::Sunny.UI.UITextBox 修改_账号名字输入框;

        private global::Sunny.UI.UIButton 启动_进入游戏按钮;

        private global::Sunny.UI.UILabel 注册_错误提示标签;

        private global::Sunny.UI.UILabel 修改_错误提示标签;

        private global::System.Windows.Forms.ListBox 启动_选择游戏区服;

        private global::Sunny.UI.UILinkLabel 启动_选中区服标签;

        private global::Sunny.UI.UILinkLabel 启动_注销账号标签;

        private global::Sunny.UI.UISymbolButton 注册_返回登录按钮;

        private global::Sunny.UI.UISymbolButton 修改_返回登录按钮;

        private global::System.Windows.Forms.Timer 用户界面计时;

        public global::Sunny.UI.UITabControl 主选项卡;

        public global::Sunny.UI.UILabel 登录_错误提示标签;

        private global::System.Windows.Forms.Timer 数据处理计时;

        private global::Sunny.UI.UISymbolButton 启动_当前账号标签;

        private global::System.Windows.Forms.NotifyIcon 最小化到托盘;

        private global::System.Windows.Forms.ContextMenuStrip 托盘右键菜单;

        private global::System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;

        private global::System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;

        private global::System.Windows.Forms.Timer 游戏进程监测;
    }
}
