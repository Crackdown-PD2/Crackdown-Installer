namespace Crackdown_Installer
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			button_startDownload = new Button();
			panel_stage1 = new Panel();
			splitContainer1 = new SplitContainer();
			pictureBox_cdLogo = new PictureBox();
			linkLabelDiscord = new LinkLabel();
			linkLabelHomepage = new LinkLabel();
			linkLabelWiki = new LinkLabel();
			label_stage1Title = new Label();
			button_quit = new Button();
			folderBrowserDialog1 = new FolderBrowserDialog();
			richTextBox1 = new RichTextBox();
			panel_stage2 = new Panel();
			label_stage2Title = new Label();
			label_browseInstallPathDesc = new Label();
			button_browseInstallPath = new Button();
			button_resetInstallPath = new Button();
			panel_stage3 = new Panel();
			label_stage3_arrow = new Label();
			panel_stage3_mouseoverDesc = new Panel();
			label_modDependenciesItemMouseverDescription = new Label();
			label_installedModsList = new Label();
			label_missingModsList = new Label();
			checkedListBox_dummyInstalledMods = new CheckedListBox();
			checkedListBox_dummyMissingMods = new CheckedListBox();
			label_stage3Title = new Label();
			label_stage3Desc = new Label();
			button_detectExistingMods = new Button();
			button_prevStage = new Button();
			button_nextStage = new Button();
			panel_navigation = new Panel();
			label_navigation_stage4 = new Label();
			label_navigation_stage3 = new Label();
			label_navigation_stage2 = new Label();
			label_navigation_stage1 = new Label();
			toolTip1 = new ToolTip(components);
			panel_stage5 = new Panel();
			label_downloadStatusDesc = new Label();
			label_downloadStatusTitle = new Label();
			listBox_downloadList = new ListBox();
			progressBar_downloadIndividual = new ProgressBar();
			panel_stage4 = new Panel();
			panel_stage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox_cdLogo).BeginInit();
			panel_stage2.SuspendLayout();
			panel_stage3.SuspendLayout();
			panel_stage3_mouseoverDesc.SuspendLayout();
			panel_navigation.SuspendLayout();
			panel_stage4.SuspendLayout();
			SuspendLayout();
			// 
			// button_startDownload
			// 
			button_startDownload.Location = new Point(16, 188);
			button_startDownload.Name = "button_startDownload";
			button_startDownload.Size = new Size(108, 27);
			button_startDownload.TabIndex = 3;
			button_startDownload.Text = "Download";
			button_startDownload.UseVisualStyleBackColor = true;
			button_startDownload.Click += button_start_Click;
			// 
			// panel_stage1
			// 
			panel_stage1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel_stage1.Controls.Add(splitContainer1);
			panel_stage1.Location = new Point(734, 172);
			panel_stage1.Name = "panel_stage1";
			panel_stage1.Size = new Size(479, 318);
			panel_stage1.TabIndex = 1;
			// 
			// splitContainer1
			// 
			splitContainer1.BorderStyle = BorderStyle.FixedSingle;
			splitContainer1.Location = new Point(29, 15);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(pictureBox_cdLogo);
			splitContainer1.Panel1.Controls.Add(linkLabelDiscord);
			splitContainer1.Panel1.Controls.Add(linkLabelHomepage);
			splitContainer1.Panel1.Controls.Add(linkLabelWiki);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(label_stage1Title);
			splitContainer1.Size = new Size(676, 300);
			splitContainer1.SplitterDistance = 170;
			splitContainer1.TabIndex = 16;
			// 
			// pictureBox_cdLogo
			// 
			pictureBox_cdLogo.ImageLocation = "https://i.imgur.com/jPwofLU.png";
			pictureBox_cdLogo.Location = new Point(69, 3);
			pictureBox_cdLogo.Name = "pictureBox_cdLogo";
			pictureBox_cdLogo.Size = new Size(96, 96);
			pictureBox_cdLogo.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox_cdLogo.TabIndex = 4;
			pictureBox_cdLogo.TabStop = false;
			pictureBox_cdLogo.Click += pictureBox_cdLogo_Click;
			// 
			// linkLabelDiscord
			// 
			linkLabelDiscord.AutoSize = true;
			linkLabelDiscord.Location = new Point(18, 178);
			linkLabelDiscord.Name = "linkLabelDiscord";
			linkLabelDiscord.Size = new Size(138, 15);
			linkLabelDiscord.TabIndex = 1;
			linkLabelDiscord.TabStop = true;
			linkLabelDiscord.Text = "Crackdown Mod Discord";
			linkLabelDiscord.LinkClicked += linkLabelDiscord_LinkClicked;
			// 
			// linkLabelHomepage
			// 
			linkLabelHomepage.AutoSize = true;
			linkLabelHomepage.Location = new Point(18, 234);
			linkLabelHomepage.Name = "linkLabelHomepage";
			linkLabelHomepage.Size = new Size(140, 15);
			linkLabelHomepage.TabIndex = 2;
			linkLabelHomepage.TabStop = true;
			linkLabelHomepage.Text = "Crackdown Mod Website";
			linkLabelHomepage.LinkClicked += linkLabelHomepage_LinkClicked;
			// 
			// linkLabelWiki
			// 
			linkLabelWiki.AutoSize = true;
			linkLabelWiki.Location = new Point(18, 203);
			linkLabelWiki.Name = "linkLabelWiki";
			linkLabelWiki.Size = new Size(121, 15);
			linkLabelWiki.TabIndex = 3;
			linkLabelWiki.TabStop = true;
			linkLabelWiki.Text = "Crackdown Mod Wiki";
			linkLabelWiki.LinkClicked += linkLabelWiki_LinkClicked;
			// 
			// label_stage1Title
			// 
			label_stage1Title.AutoSize = true;
			label_stage1Title.Location = new Point(27, 21);
			label_stage1Title.Name = "label_stage1Title";
			label_stage1Title.Size = new Size(229, 15);
			label_stage1Title.TabIndex = 4;
			label_stage1Title.Text = "Welcome to the Crackdown Mod Installer!";
			label_stage1Title.Click += label_stage1Title_Click;
			// 
			// button_quit
			// 
			button_quit.Anchor = AnchorStyles.None;
			button_quit.Location = new Point(904, 9);
			button_quit.Name = "button_quit";
			button_quit.Size = new Size(101, 28);
			button_quit.TabIndex = 2;
			button_quit.Text = "&Quit";
			button_quit.UseVisualStyleBackColor = true;
			button_quit.Click += button1_Click;
			// 
			// folderBrowserDialog1
			// 
			folderBrowserDialog1.Description = "Please select your PAYDAY 2 game install folder.";
			folderBrowserDialog1.InitialDirectory = "C:/Users/";
			folderBrowserDialog1.ShowHiddenFiles = true;
			folderBrowserDialog1.ShowNewFolderButton = false;
			folderBrowserDialog1.HelpRequest += folderBrowserDialog1_HelpRequest;
			// 
			// richTextBox1
			// 
			richTextBox1.AccessibleDescription = "The path to your PAYDAY 2 installation.";
			richTextBox1.AccessibleName = "PAYDAY 2 folder path";
			richTextBox1.DetectUrls = false;
			richTextBox1.HideSelection = false;
			richTextBox1.Location = new Point(16, 30);
			richTextBox1.Multiline = false;
			richTextBox1.Name = "richTextBox1";
			richTextBox1.ReadOnly = true;
			richTextBox1.ScrollBars = RichTextBoxScrollBars.Horizontal;
			richTextBox1.Size = new Size(298, 30);
			richTextBox1.TabIndex = 4;
			richTextBox1.Text = "";
			richTextBox1.WordWrap = false;
			richTextBox1.TextChanged += richTextBox1_TextChanged;
			// 
			// panel_stage2
			// 
			panel_stage2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			panel_stage2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel_stage2.Controls.Add(label_stage2Title);
			panel_stage2.Controls.Add(label_browseInstallPathDesc);
			panel_stage2.Controls.Add(button_browseInstallPath);
			panel_stage2.Controls.Add(button_resetInstallPath);
			panel_stage2.Controls.Add(richTextBox1);
			panel_stage2.Location = new Point(734, 12);
			panel_stage2.Name = "panel_stage2";
			panel_stage2.Size = new Size(479, 154);
			panel_stage2.TabIndex = 6;
			panel_stage2.Visible = false;
			// 
			// label_stage2Title
			// 
			label_stage2Title.AutoSize = true;
			label_stage2Title.Location = new Point(16, 12);
			label_stage2Title.Name = "label_stage2Title";
			label_stage2Title.Size = new Size(172, 15);
			label_stage2Title.TabIndex = 10;
			label_stage2Title.Text = "Auto-detected PAYDAY 2 folder";
			// 
			// label_browseInstallPathDesc
			// 
			label_browseInstallPathDesc.Location = new Point(16, 69);
			label_browseInstallPathDesc.Name = "label_browseInstallPathDesc";
			label_browseInstallPathDesc.Size = new Size(404, 31);
			label_browseInstallPathDesc.TabIndex = 9;
			label_browseInstallPathDesc.Text = "Before proceeding, please ensure that this path matches your PAYDAY 2 installation path.";
			// 
			// button_browseInstallPath
			// 
			button_browseInstallPath.Location = new Point(320, 31);
			button_browseInstallPath.Name = "button_browseInstallPath";
			button_browseInstallPath.Size = new Size(79, 30);
			button_browseInstallPath.TabIndex = 8;
			button_browseInstallPath.Text = "Browse...";
			button_browseInstallPath.UseVisualStyleBackColor = true;
			button_browseInstallPath.Click += button_browseInstallPath_Click;
			// 
			// button_resetInstallPath
			// 
			button_resetInstallPath.Location = new Point(405, 30);
			button_resetInstallPath.Name = "button_resetInstallPath";
			button_resetInstallPath.Size = new Size(66, 30);
			button_resetInstallPath.TabIndex = 7;
			button_resetInstallPath.Text = "Reset";
			button_resetInstallPath.UseVisualStyleBackColor = true;
			button_resetInstallPath.Click += button_resetInstallPath_Click;
			// 
			// panel_stage3
			// 
			panel_stage3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			panel_stage3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel_stage3.Controls.Add(label_stage3_arrow);
			panel_stage3.Controls.Add(panel_stage3_mouseoverDesc);
			panel_stage3.Controls.Add(label_installedModsList);
			panel_stage3.Controls.Add(label_missingModsList);
			panel_stage3.Controls.Add(checkedListBox_dummyInstalledMods);
			panel_stage3.Controls.Add(checkedListBox_dummyMissingMods);
			panel_stage3.Controls.Add(label_stage3Title);
			panel_stage3.Controls.Add(label_stage3Desc);
			panel_stage3.Controls.Add(button_detectExistingMods);
			panel_stage3.Location = new Point(45, 12);
			panel_stage3.Name = "panel_stage3";
			panel_stage3.Size = new Size(668, 332);
			panel_stage3.TabIndex = 7;
			panel_stage3.Visible = false;
			// 
			// label_stage3_arrow
			// 
			label_stage3_arrow.AutoSize = true;
			label_stage3_arrow.Location = new Point(304, 197);
			label_stage3_arrow.Name = "label_stage3_arrow";
			label_stage3_arrow.Size = new Size(25, 15);
			label_stage3_arrow.TabIndex = 21;
			label_stage3_arrow.Text = "-->";
			// 
			// panel_stage3_mouseoverDesc
			// 
			panel_stage3_mouseoverDesc.BorderStyle = BorderStyle.FixedSingle;
			panel_stage3_mouseoverDesc.Controls.Add(label_modDependenciesItemMouseverDescription);
			panel_stage3_mouseoverDesc.Location = new Point(24, 298);
			panel_stage3_mouseoverDesc.Name = "panel_stage3_mouseoverDesc";
			panel_stage3_mouseoverDesc.Size = new Size(608, 25);
			panel_stage3_mouseoverDesc.TabIndex = 20;
			// 
			// label_modDependenciesItemMouseverDescription
			// 
			label_modDependenciesItemMouseverDescription.AutoSize = true;
			label_modDependenciesItemMouseverDescription.Location = new Point(161, 4);
			label_modDependenciesItemMouseverDescription.Name = "label_modDependenciesItemMouseverDescription";
			label_modDependenciesItemMouseverDescription.Size = new Size(258, 15);
			label_modDependenciesItemMouseverDescription.TabIndex = 13;
			label_modDependenciesItemMouseverDescription.Text = "Mouse over a dependency to see more about it.";
			label_modDependenciesItemMouseverDescription.TextAlign = ContentAlignment.MiddleCenter;
			label_modDependenciesItemMouseverDescription.Click += label_modDependenciesItemMouseverDescription_Click;
			// 
			// label_installedModsList
			// 
			label_installedModsList.AutoSize = true;
			label_installedModsList.Location = new Point(444, 97);
			label_installedModsList.Name = "label_installedModsList";
			label_installedModsList.Size = new Size(81, 15);
			label_installedModsList.TabIndex = 19;
			label_installedModsList.Text = "Existing Mods";
			// 
			// label_missingModsList
			// 
			label_missingModsList.AutoSize = true;
			label_missingModsList.Location = new Point(75, 97);
			label_missingModsList.Name = "label_missingModsList";
			label_missingModsList.Size = new Size(113, 15);
			label_missingModsList.TabIndex = 18;
			label_missingModsList.Text = "Pending Downloads";
			// 
			// checkedListBox_dummyInstalledMods
			// 
			checkedListBox_dummyInstalledMods.CheckOnClick = true;
			checkedListBox_dummyInstalledMods.Enabled = false;
			checkedListBox_dummyInstalledMods.FormattingEnabled = true;
			checkedListBox_dummyInstalledMods.HorizontalScrollbar = true;
			checkedListBox_dummyInstalledMods.Location = new Point(366, 126);
			checkedListBox_dummyInstalledMods.Name = "checkedListBox_dummyInstalledMods";
			checkedListBox_dummyInstalledMods.Size = new Size(266, 166);
			checkedListBox_dummyInstalledMods.TabIndex = 17;
			checkedListBox_dummyInstalledMods.Visible = false;
			// 
			// checkedListBox_dummyMissingMods
			// 
			checkedListBox_dummyMissingMods.CheckOnClick = true;
			checkedListBox_dummyMissingMods.Enabled = false;
			checkedListBox_dummyMissingMods.FormattingEnabled = true;
			checkedListBox_dummyMissingMods.HorizontalScrollbar = true;
			checkedListBox_dummyMissingMods.Location = new Point(24, 126);
			checkedListBox_dummyMissingMods.Name = "checkedListBox_dummyMissingMods";
			checkedListBox_dummyMissingMods.Size = new Size(250, 166);
			checkedListBox_dummyMissingMods.TabIndex = 15;
			checkedListBox_dummyMissingMods.Visible = false;
			// 
			// label_stage3Title
			// 
			label_stage3Title.AutoSize = true;
			label_stage3Title.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
			label_stage3Title.Location = new Point(24, 12);
			label_stage3Title.Name = "label_stage3Title";
			label_stage3Title.Size = new Size(131, 15);
			label_stage3Title.TabIndex = 16;
			label_stage3Title.Text = "Dependency Packages";
			label_stage3Title.Click += label_stage3Title_Click;
			// 
			// label_stage3Desc
			// 
			label_stage3Desc.AutoSize = true;
			label_stage3Desc.Location = new Point(16, 27);
			label_stage3Desc.Name = "label_stage3Desc";
			label_stage3Desc.Size = new Size(570, 15);
			label_stage3Desc.TabIndex = 2;
			label_stage3Desc.Text = "These are mods that Crackdown needs in order to work properly. Select which files to download and install.";
			// 
			// button_detectExistingMods
			// 
			button_detectExistingMods.Location = new Point(270, 63);
			button_detectExistingMods.Name = "button_detectExistingMods";
			button_detectExistingMods.Size = new Size(103, 25);
			button_detectExistingMods.TabIndex = 1;
			button_detectExistingMods.Text = "Re-detect Mods";
			button_detectExistingMods.UseVisualStyleBackColor = true;
			button_detectExistingMods.Visible = false;
			button_detectExistingMods.Click += button_detectExistingMods_Click;
			// 
			// button_prevStage
			// 
			button_prevStage.Anchor = AnchorStyles.None;
			button_prevStage.FlatStyle = FlatStyle.System;
			button_prevStage.Location = new Point(729, 9);
			button_prevStage.Name = "button_prevStage";
			button_prevStage.Size = new Size(74, 28);
			button_prevStage.TabIndex = 8;
			button_prevStage.Text = "&< Back";
			button_prevStage.UseVisualStyleBackColor = true;
			button_prevStage.Click += button_prevStage_Click;
			// 
			// button_nextStage
			// 
			button_nextStage.Anchor = AnchorStyles.None;
			button_nextStage.Location = new Point(816, 9);
			button_nextStage.Name = "button_nextStage";
			button_nextStage.Size = new Size(76, 28);
			button_nextStage.TabIndex = 9;
			button_nextStage.Text = "Next &>";
			button_nextStage.UseVisualStyleBackColor = true;
			button_nextStage.Click += button_nextStage_Click;
			// 
			// panel_navigation
			// 
			panel_navigation.BorderStyle = BorderStyle.Fixed3D;
			panel_navigation.Controls.Add(label_navigation_stage4);
			panel_navigation.Controls.Add(label_navigation_stage3);
			panel_navigation.Controls.Add(label_navigation_stage2);
			panel_navigation.Controls.Add(label_navigation_stage1);
			panel_navigation.Controls.Add(button_quit);
			panel_navigation.Controls.Add(button_nextStage);
			panel_navigation.Controls.Add(button_prevStage);
			panel_navigation.Dock = DockStyle.Bottom;
			panel_navigation.Location = new Point(0, 641);
			panel_navigation.Name = "panel_navigation";
			panel_navigation.Size = new Size(1225, 44);
			panel_navigation.TabIndex = 10;
			panel_navigation.Paint += panel4_Paint;
			// 
			// label_navigation_stage4
			// 
			label_navigation_stage4.AutoSize = true;
			label_navigation_stage4.Location = new Point(374, 18);
			label_navigation_stage4.Name = "label_navigation_stage4";
			label_navigation_stage4.Size = new Size(113, 15);
			label_navigation_stage4.TabIndex = 13;
			label_navigation_stage4.Text = "Download Packages";
			label_navigation_stage4.Visible = false;
			// 
			// label_navigation_stage3
			// 
			label_navigation_stage3.AutoSize = true;
			label_navigation_stage3.Location = new Point(253, 16);
			label_navigation_stage3.Name = "label_navigation_stage3";
			label_navigation_stage3.Size = new Size(90, 15);
			label_navigation_stage3.TabIndex = 12;
			label_navigation_stage3.Text = "Select Packages";
			label_navigation_stage3.Visible = false;
			// 
			// label_navigation_stage2
			// 
			label_navigation_stage2.AutoSize = true;
			label_navigation_stage2.Location = new Point(84, 16);
			label_navigation_stage2.Name = "label_navigation_stage2";
			label_navigation_stage2.Size = new Size(139, 15);
			label_navigation_stage2.TabIndex = 11;
			label_navigation_stage2.Text = "Confirm Installation Path";
			label_navigation_stage2.Visible = false;
			// 
			// label_navigation_stage1
			// 
			label_navigation_stage1.AutoSize = true;
			label_navigation_stage1.Location = new Point(20, 16);
			label_navigation_stage1.Name = "label_navigation_stage1";
			label_navigation_stage1.Size = new Size(32, 15);
			label_navigation_stage1.TabIndex = 10;
			label_navigation_stage1.Text = "Intro";
			label_navigation_stage1.Visible = false;
			// 
			// panel_stage5
			// 
			panel_stage5.BorderStyle = BorderStyle.FixedSingle;
			panel_stage5.Location = new Point(734, 521);
			panel_stage5.Name = "panel_stage5";
			panel_stage5.Size = new Size(460, 77);
			panel_stage5.TabIndex = 16;
			// 
			// label_downloadStatusDesc
			// 
			label_downloadStatusDesc.AutoSize = true;
			label_downloadStatusDesc.Location = new Point(399, 221);
			label_downloadStatusDesc.Name = "label_downloadStatusDesc";
			label_downloadStatusDesc.Size = new Size(126, 15);
			label_downloadStatusDesc.TabIndex = 3;
			label_downloadStatusDesc.Text = "PlaceholderModName";
			// 
			// label_downloadStatusTitle
			// 
			label_downloadStatusTitle.AutoSize = true;
			label_downloadStatusTitle.Location = new Point(270, 221);
			label_downloadStatusTitle.Name = "label_downloadStatusTitle";
			label_downloadStatusTitle.Size = new Size(106, 15);
			label_downloadStatusTitle.TabIndex = 2;
			label_downloadStatusTitle.Text = "[1/6] Downloading";
			// 
			// listBox_downloadList
			// 
			listBox_downloadList.FormattingEnabled = true;
			listBox_downloadList.ItemHeight = 15;
			listBox_downloadList.Location = new Point(16, 13);
			listBox_downloadList.Name = "listBox_downloadList";
			listBox_downloadList.ScrollAlwaysVisible = true;
			listBox_downloadList.Size = new Size(635, 154);
			listBox_downloadList.TabIndex = 1;
			// 
			// progressBar_downloadIndividual
			// 
			progressBar_downloadIndividual.Location = new Point(16, 221);
			progressBar_downloadIndividual.Name = "progressBar_downloadIndividual";
			progressBar_downloadIndividual.Size = new Size(241, 21);
			progressBar_downloadIndividual.TabIndex = 0;
			// 
			// panel_stage4
			// 
			panel_stage4.Controls.Add(progressBar_downloadIndividual);
			panel_stage4.Controls.Add(label_downloadStatusDesc);
			panel_stage4.Controls.Add(listBox_downloadList);
			panel_stage4.Controls.Add(label_downloadStatusTitle);
			panel_stage4.Controls.Add(button_startDownload);
			panel_stage4.Location = new Point(45, 350);
			panel_stage4.Name = "panel_stage4";
			panel_stage4.Size = new Size(668, 260);
			panel_stage4.TabIndex = 15;
			panel_stage4.Visible = false;
			// 
			// Form1
			// 
			AcceptButton = button_nextStage;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1225, 685);
			Controls.Add(panel_stage5);
			Controls.Add(panel_stage3);
			Controls.Add(panel_stage4);
			Controls.Add(panel_stage2);
			Controls.Add(panel_navigation);
			Controls.Add(panel_stage1);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "Form1";
			Text = "Installer Wizard";
			Load += Form1_Load;
			panel_stage1.ResumeLayout(false);
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBox_cdLogo).EndInit();
			panel_stage2.ResumeLayout(false);
			panel_stage2.PerformLayout();
			panel_stage3.ResumeLayout(false);
			panel_stage3.PerformLayout();
			panel_stage3_mouseoverDesc.ResumeLayout(false);
			panel_stage3_mouseoverDesc.PerformLayout();
			panel_navigation.ResumeLayout(false);
			panel_navigation.PerformLayout();
			panel_stage4.ResumeLayout(false);
			panel_stage4.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private Panel panel_stage1;
		private Button button_quit;
		private Button button_start;
		private FolderBrowserDialog folderBrowserDialog1;
		private RichTextBox richTextBox1;
		private Panel panel_stage2;
		private Panel panel_stage3;
		private LinkLabel linkLabelWiki;
		private LinkLabel linkLabelHomepage;
		private LinkLabel linkLabelDiscord;
		private Button button_prevStage;
		private Button button_nextStage;
		private Panel panel_navigation;
		private Label label_modDependenciesItemMouseverDescription;
		private Panel panel_stage5;
		private CheckedListBox checkedListBox1;
		private CheckedListBox checkedListBox_dummyMissingMods;
		private Button button_detectExistingMods;
		private Button button_resetInstallPath;
		private Button button_browseInstallPath;
		private Label label_browseInstallPathDesc;
		private Label label_stage1Title;
		private Label label_stage2Title;
		private Label label_stage3Title;
		private Label label_stage3Desc;
		private ToolTip toolTip1;
		private Label label_installedModsList;
		private Label label_missingModsList;
		private CheckedListBox checkedListBox_dummyInstalledMods;
		private Panel panel_stage3_mouseoverDesc;
		private SplitContainer splitContainer1;
		private PictureBox pictureBox_cdLogo;
		private Label label_stage3_arrow;
		private Panel panel_stage4;
		private ProgressBar progressBar_downloadIndividual;
		private Label label_downloadStatusDesc;
		private Label label_downloadStatusTitle;
		private ListBox listBox_downloadList;
		private Button button_startDownload;
		private Label label_navigation_stage4;
		private Label label_navigation_stage3;
		private Label label_navigation_stage2;
		private Label label_navigation_stage1;
	}
}