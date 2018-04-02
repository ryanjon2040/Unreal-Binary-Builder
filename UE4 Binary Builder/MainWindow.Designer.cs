namespace UE4_Binary_Builder
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GameConfigurations = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bCleanBuild = new System.Windows.Forms.CheckBox();
            this.bEnableSymStore = new System.Windows.Forms.CheckBox();
            this.bSignExecutables = new System.Windows.Forms.CheckBox();
            this.bWithDDC = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bWithXboxOne = new System.Windows.Forms.CheckBox();
            this.bWithPS4 = new System.Windows.Forms.CheckBox();
            this.bWithSwitch = new System.Windows.Forms.CheckBox();
            this.bWithHTML5 = new System.Windows.Forms.CheckBox();
            this.bWithLinux = new System.Windows.Forms.CheckBox();
            this.bWithTVOS = new System.Windows.Forms.CheckBox();
            this.bWithIOS = new System.Windows.Forms.CheckBox();
            this.bWithAndroid = new System.Windows.Forms.CheckBox();
            this.bWithMac = new System.Windows.Forms.CheckBox();
            this.bWithWin32 = new System.Windows.Forms.CheckBox();
            this.bWithWin64 = new System.Windows.Forms.CheckBox();
            this.bHostPlatformOnly = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.BuildRocketUE = new System.Windows.Forms.Button();
            this.AutomationToolBrowse = new System.Windows.Forms.Button();
            this.AutomationToolPath = new System.Windows.Forms.TextBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.AboutMenu = new System.Windows.Forms.MenuItem();
            this.GetSourceCodeMenu = new System.Windows.Forms.MenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.LogWindow = new System.Windows.Forms.TextBox();
            this.bWithFullDebugInfo = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Automation Tool Launcher Path:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bWithFullDebugInfo);
            this.groupBox1.Controls.Add(this.GameConfigurations);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.bCleanBuild);
            this.groupBox1.Controls.Add(this.bEnableSymStore);
            this.groupBox1.Controls.Add(this.bSignExecutables);
            this.groupBox1.Controls.Add(this.bWithDDC);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(14, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 247);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // GameConfigurations
            // 
            this.GameConfigurations.Location = new System.Drawing.Point(12, 210);
            this.GameConfigurations.Name = "GameConfigurations";
            this.GameConfigurations.Size = new System.Drawing.Size(353, 23);
            this.GameConfigurations.TabIndex = 7;
            this.toolTip1.SetToolTip(this.GameConfigurations, "Which game configurations to include for packaged applications. Defaults to Devel" +
        "opment;Shipping");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Game Configurations";
            this.toolTip1.SetToolTip(this.label3, "Which game configurations to include for packaged applications");
            // 
            // bCleanBuild
            // 
            this.bCleanBuild.AutoSize = true;
            this.bCleanBuild.Location = new System.Drawing.Point(222, 109);
            this.bCleanBuild.Name = "bCleanBuild";
            this.bCleanBuild.Size = new System.Drawing.Size(90, 20);
            this.bCleanBuild.TabIndex = 5;
            this.bCleanBuild.Text = "Clean Build";
            this.toolTip1.SetToolTip(this.bCleanBuild, "Cleans any previous builds");
            this.bCleanBuild.UseVisualStyleBackColor = true;
            // 
            // bEnableSymStore
            // 
            this.bEnableSymStore.AutoSize = true;
            this.bEnableSymStore.Location = new System.Drawing.Point(222, 83);
            this.bEnableSymStore.Name = "bEnableSymStore";
            this.bEnableSymStore.Size = new System.Drawing.Size(146, 20);
            this.bEnableSymStore.TabIndex = 4;
            this.bEnableSymStore.Text = "Enable Symbol Store";
            this.toolTip1.SetToolTip(this.bEnableSymStore, "Whether to add Source indexing to Windows game apps so they can be added to a sym" +
        "bol server");
            this.bEnableSymStore.UseVisualStyleBackColor = true;
            // 
            // bSignExecutables
            // 
            this.bSignExecutables.AutoSize = true;
            this.bSignExecutables.Location = new System.Drawing.Point(222, 57);
            this.bSignExecutables.Name = "bSignExecutables";
            this.bSignExecutables.Size = new System.Drawing.Size(123, 20);
            this.bSignExecutables.TabIndex = 3;
            this.bSignExecutables.Text = "Sign Executables";
            this.toolTip1.SetToolTip(this.bSignExecutables, "Sign the executables produced where signing is available");
            this.bSignExecutables.UseVisualStyleBackColor = true;
            // 
            // bWithDDC
            // 
            this.bWithDDC.AutoSize = true;
            this.bWithDDC.Checked = true;
            this.bWithDDC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithDDC.Location = new System.Drawing.Point(222, 31);
            this.bWithDDC.Name = "bWithDDC";
            this.bWithDDC.Size = new System.Drawing.Size(96, 20);
            this.bWithDDC.TabIndex = 2;
            this.bWithDDC.Text = "Include DDC";
            this.toolTip1.SetToolTip(this.bWithDDC, resources.GetString("bWithDDC.ToolTip"));
            this.bWithDDC.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bWithXboxOne);
            this.groupBox2.Controls.Add(this.bWithPS4);
            this.groupBox2.Controls.Add(this.bWithSwitch);
            this.groupBox2.Controls.Add(this.bWithHTML5);
            this.groupBox2.Controls.Add(this.bWithLinux);
            this.groupBox2.Controls.Add(this.bWithTVOS);
            this.groupBox2.Controls.Add(this.bWithIOS);
            this.groupBox2.Controls.Add(this.bWithAndroid);
            this.groupBox2.Controls.Add(this.bWithMac);
            this.groupBox2.Controls.Add(this.bWithWin32);
            this.groupBox2.Controls.Add(this.bWithWin64);
            this.groupBox2.Controls.Add(this.bHostPlatformOnly);
            this.groupBox2.Location = new System.Drawing.Point(6, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 166);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Platforms";
            // 
            // bWithXboxOne
            // 
            this.bWithXboxOne.Location = new System.Drawing.Point(75, 136);
            this.bWithXboxOne.Name = "bWithXboxOne";
            this.bWithXboxOne.Size = new System.Drawing.Size(104, 24);
            this.bWithXboxOne.TabIndex = 11;
            this.bWithXboxOne.Text = "Xbox One";
            this.bWithXboxOne.UseVisualStyleBackColor = true;
            // 
            // bWithPS4
            // 
            this.bWithPS4.Location = new System.Drawing.Point(6, 136);
            this.bWithPS4.Name = "bWithPS4";
            this.bWithPS4.Size = new System.Drawing.Size(104, 24);
            this.bWithPS4.TabIndex = 10;
            this.bWithPS4.Text = "PS4";
            this.bWithPS4.UseVisualStyleBackColor = true;
            // 
            // bWithSwitch
            // 
            this.bWithSwitch.AutoSize = true;
            this.bWithSwitch.Location = new System.Drawing.Point(144, 110);
            this.bWithSwitch.Name = "bWithSwitch";
            this.bWithSwitch.Size = new System.Drawing.Size(65, 20);
            this.bWithSwitch.TabIndex = 9;
            this.bWithSwitch.Text = "Switch";
            this.bWithSwitch.UseVisualStyleBackColor = true;
            // 
            // bWithHTML5
            // 
            this.bWithHTML5.AutoSize = true;
            this.bWithHTML5.Checked = true;
            this.bWithHTML5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithHTML5.Location = new System.Drawing.Point(75, 110);
            this.bWithHTML5.Name = "bWithHTML5";
            this.bWithHTML5.Size = new System.Drawing.Size(66, 20);
            this.bWithHTML5.TabIndex = 8;
            this.bWithHTML5.Text = "HTML5";
            this.bWithHTML5.UseVisualStyleBackColor = true;
            // 
            // bWithLinux
            // 
            this.bWithLinux.AutoSize = true;
            this.bWithLinux.Checked = true;
            this.bWithLinux.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithLinux.Location = new System.Drawing.Point(6, 110);
            this.bWithLinux.Name = "bWithLinux";
            this.bWithLinux.Size = new System.Drawing.Size(56, 20);
            this.bWithLinux.TabIndex = 7;
            this.bWithLinux.Text = "Linux";
            this.bWithLinux.UseVisualStyleBackColor = true;
            // 
            // bWithTVOS
            // 
            this.bWithTVOS.AutoSize = true;
            this.bWithTVOS.Checked = true;
            this.bWithTVOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithTVOS.Location = new System.Drawing.Point(144, 84);
            this.bWithTVOS.Name = "bWithTVOS";
            this.bWithTVOS.Size = new System.Drawing.Size(60, 20);
            this.bWithTVOS.TabIndex = 6;
            this.bWithTVOS.Text = "TVOS";
            this.bWithTVOS.UseVisualStyleBackColor = true;
            // 
            // bWithIOS
            // 
            this.bWithIOS.AutoSize = true;
            this.bWithIOS.Checked = true;
            this.bWithIOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithIOS.Location = new System.Drawing.Point(75, 84);
            this.bWithIOS.Name = "bWithIOS";
            this.bWithIOS.Size = new System.Drawing.Size(47, 20);
            this.bWithIOS.TabIndex = 5;
            this.bWithIOS.Text = "iOS";
            this.bWithIOS.UseVisualStyleBackColor = true;
            // 
            // bWithAndroid
            // 
            this.bWithAndroid.AutoSize = true;
            this.bWithAndroid.Checked = true;
            this.bWithAndroid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithAndroid.Location = new System.Drawing.Point(6, 84);
            this.bWithAndroid.Name = "bWithAndroid";
            this.bWithAndroid.Size = new System.Drawing.Size(71, 20);
            this.bWithAndroid.TabIndex = 4;
            this.bWithAndroid.Text = "Android";
            this.bWithAndroid.UseVisualStyleBackColor = true;
            // 
            // bWithMac
            // 
            this.bWithMac.AutoSize = true;
            this.bWithMac.Checked = true;
            this.bWithMac.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithMac.Location = new System.Drawing.Point(144, 58);
            this.bWithMac.Name = "bWithMac";
            this.bWithMac.Size = new System.Drawing.Size(50, 20);
            this.bWithMac.TabIndex = 3;
            this.bWithMac.Text = "Mac";
            this.bWithMac.UseVisualStyleBackColor = true;
            // 
            // bWithWin32
            // 
            this.bWithWin32.AutoSize = true;
            this.bWithWin32.Checked = true;
            this.bWithWin32.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithWin32.Location = new System.Drawing.Point(75, 58);
            this.bWithWin32.Name = "bWithWin32";
            this.bWithWin32.Size = new System.Drawing.Size(63, 20);
            this.bWithWin32.TabIndex = 2;
            this.bWithWin32.Text = "Win32";
            this.bWithWin32.UseVisualStyleBackColor = true;
            // 
            // bWithWin64
            // 
            this.bWithWin64.AutoSize = true;
            this.bWithWin64.Checked = true;
            this.bWithWin64.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bWithWin64.Location = new System.Drawing.Point(6, 58);
            this.bWithWin64.Name = "bWithWin64";
            this.bWithWin64.Size = new System.Drawing.Size(63, 20);
            this.bWithWin64.TabIndex = 1;
            this.bWithWin64.Text = "Win64";
            this.bWithWin64.UseVisualStyleBackColor = true;
            // 
            // bHostPlatformOnly
            // 
            this.bHostPlatformOnly.AutoSize = true;
            this.bHostPlatformOnly.Location = new System.Drawing.Point(6, 22);
            this.bHostPlatformOnly.Name = "bHostPlatformOnly";
            this.bHostPlatformOnly.Size = new System.Drawing.Size(133, 20);
            this.bHostPlatformOnly.TabIndex = 0;
            this.bHostPlatformOnly.Text = "Host Platform Only";
            this.toolTip1.SetToolTip(this.bHostPlatformOnly, "A helper option to make an installed build for your host platform only, so that y" +
        "ou don\'t have to disable each platform individually");
            this.bHostPlatformOnly.UseVisualStyleBackColor = true;
            this.bHostPlatformOnly.CheckedChanged += new System.EventHandler(this.bHostPlatformOnly_CheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Information";
            // 
            // BuildRocketUE
            // 
            this.BuildRocketUE.Enabled = false;
            this.BuildRocketUE.Location = new System.Drawing.Point(14, 324);
            this.BuildRocketUE.Name = "BuildRocketUE";
            this.BuildRocketUE.Size = new System.Drawing.Size(388, 52);
            this.BuildRocketUE.TabIndex = 3;
            this.BuildRocketUE.Text = "Build";
            this.toolTip1.SetToolTip(this.BuildRocketUE, "Start build process.");
            this.BuildRocketUE.UseVisualStyleBackColor = true;
            this.BuildRocketUE.Click += new System.EventHandler(this.BuildRocketUE_Click);
            // 
            // AutomationToolBrowse
            // 
            this.AutomationToolBrowse.Location = new System.Drawing.Point(338, 41);
            this.AutomationToolBrowse.Name = "AutomationToolBrowse";
            this.AutomationToolBrowse.Size = new System.Drawing.Size(64, 23);
            this.AutomationToolBrowse.TabIndex = 4;
            this.AutomationToolBrowse.Text = "...";
            this.AutomationToolBrowse.UseVisualStyleBackColor = true;
            this.AutomationToolBrowse.Click += new System.EventHandler(this.AutomationToolBrowse_Click);
            // 
            // AutomationToolPath
            // 
            this.AutomationToolPath.Location = new System.Drawing.Point(14, 41);
            this.AutomationToolPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AutomationToolPath.Name = "AutomationToolPath";
            this.AutomationToolPath.Size = new System.Drawing.Size(318, 23);
            this.AutomationToolPath.TabIndex = 0;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.AboutMenu,
            this.GetSourceCodeMenu});
            // 
            // AboutMenu
            // 
            this.AboutMenu.Index = 0;
            this.AboutMenu.Text = "About";
            this.AboutMenu.Click += new System.EventHandler(this.AboutMenu_Click);
            // 
            // GetSourceCodeMenu
            // 
            this.GetSourceCodeMenu.Index = 1;
            this.GetSourceCodeMenu.Text = "Get Source Code";
            this.GetSourceCodeMenu.Click += new System.EventHandler(this.GetSourceCodeMenu_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 379);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(323, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Binary builds can be found in Your Engine Folder\\LocalBuilds folder";
            // 
            // LogWindow
            // 
            this.LogWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogWindow.BackColor = System.Drawing.SystemColors.WindowText;
            this.LogWindow.ForeColor = System.Drawing.Color.White;
            this.LogWindow.Location = new System.Drawing.Point(408, 12);
            this.LogWindow.Multiline = true;
            this.LogWindow.Name = "LogWindow";
            this.LogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogWindow.Size = new System.Drawing.Size(875, 483);
            this.LogWindow.TabIndex = 6;
            this.LogWindow.Text = "Welcome to UE4 Binary Builder\r\n------------------------------------\r\n";
            // 
            // bWithFullDebugInfo
            // 
            this.bWithFullDebugInfo.AutoSize = true;
            this.bWithFullDebugInfo.Location = new System.Drawing.Point(222, 135);
            this.bWithFullDebugInfo.Name = "bWithFullDebugInfo";
            this.bWithFullDebugInfo.Size = new System.Drawing.Size(158, 20);
            this.bWithFullDebugInfo.TabIndex = 8;
            this.bWithFullDebugInfo.Text = "Include Full Debug Info";
            this.toolTip1.SetToolTip(this.bWithFullDebugInfo, "Generate full debug info for binary editor and packaged application builds");
            this.bWithFullDebugInfo.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1295, 505);
            this.Controls.Add(this.LogWindow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AutomationToolBrowse);
            this.Controls.Add(this.BuildRocketUE);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AutomationToolPath);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Menu = this.mainMenu1;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UE4 Binary Build Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox AutomationToolPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox bHostPlatformOnly;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox bWithXboxOne;
        private System.Windows.Forms.CheckBox bWithPS4;
        private System.Windows.Forms.CheckBox bWithSwitch;
        private System.Windows.Forms.CheckBox bWithHTML5;
        private System.Windows.Forms.CheckBox bWithLinux;
        private System.Windows.Forms.CheckBox bWithTVOS;
        private System.Windows.Forms.CheckBox bWithIOS;
        private System.Windows.Forms.CheckBox bWithAndroid;
        private System.Windows.Forms.CheckBox bWithMac;
        private System.Windows.Forms.CheckBox bWithWin32;
        private System.Windows.Forms.CheckBox bWithWin64;
        private System.Windows.Forms.CheckBox bEnableSymStore;
        private System.Windows.Forms.CheckBox bSignExecutables;
        private System.Windows.Forms.CheckBox bWithDDC;
        private System.Windows.Forms.Button BuildRocketUE;
        private System.Windows.Forms.Button AutomationToolBrowse;
        private System.Windows.Forms.CheckBox bCleanBuild;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem AboutMenu;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox LogWindow;
        private System.Windows.Forms.MenuItem GetSourceCodeMenu;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox GameConfigurations;
        private System.Windows.Forms.CheckBox bWithFullDebugInfo;
    }
}

