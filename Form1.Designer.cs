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
			label_communityTitle = new Label();
			pictureBox_cdLogo = new PictureBox();
			linkLabelDiscord = new LinkLabel();
			linkLabelHomepage = new LinkLabel();
			linkLabelWiki = new LinkLabel();
			label_installPathTitle = new Label();
			button_resetInstallPath = new Button();
			button_browseInstallPath = new Button();
			label_stage1Desc = new Label();
			label_browseInstallPathDesc = new Label();
			label_stage1Title = new Label();
			richTextBox_pd2InstallPath = new RichTextBox();
			label_stage2Title = new Label();
			button_quit = new Button();
			folderBrowserDialog1 = new FolderBrowserDialog();
			panel_stage2 = new Panel();
			label_stage3Desc_2 = new Label();
			label_stage3_arrow = new Label();
			label_stage3Desc = new Label();
			panel_stage3_mouseoverDesc = new Panel();
			label_modDependenciesItemMouseoverDescription = new Label();
			checkedListBox_dummyMissingMods = new CheckedListBox();
			label_installedModsList = new Label();
			button_detectExistingMods = new Button();
			label_missingModsList = new Label();
			checkedListBox_dummyInstalledMods = new CheckedListBox();
			label_stage3Title = new Label();
			progressBar_downloadIndividual = new ProgressBar();
			listBox_downloadList = new ListBox();
			label_downloadStatusDesc = new Label();
			label_downloadStatusTitle = new Label();
			panel_stage3 = new Panel();
			button_prevStage = new Button();
			button_nextStage = new Button();
			panel_navigation = new Panel();
			label_navigation_stage4 = new Label();
			label_navigation_stage3 = new Label();
			label_navigation_stage2 = new Label();
			label_navigation_stage1 = new Label();
			button_openTempFolder = new Button();
			label_navigation_stage5 = new Label();
			button_finishAndLaunch = new Button();
			toolTip1 = new ToolTip(components);
			panel_stage5 = new Panel();
			button_finalQuit = new Button();
			label_endDesc = new Label();
			label_endTitle = new Label();
			panel_stage4 = new Panel();
			linkLabelTroubleshooting = new LinkLabel();
			listBox_downloadFailedList = new ListBox();
			panel_stage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox_cdLogo).BeginInit();
			panel_stage2.SuspendLayout();
			panel_stage3_mouseoverDesc.SuspendLayout();
			panel_stage3.SuspendLayout();
			panel_navigation.SuspendLayout();
			panel_stage5.SuspendLayout();
			panel_stage4.SuspendLayout();
			SuspendLayout();
			// 
			// button_startDownload
			// 
			button_startDownload.Enabled = false;
			button_startDownload.Location = new Point(486, 256);
			button_startDownload.Name = "button_startDownload";
			button_startDownload.Size = new Size(128, 28);
			button_startDownload.TabIndex = 3;
			button_startDownload.Text = "Download";
			button_startDownload.UseVisualStyleBackColor = true;
			button_startDownload.Click += button_start_Click;
			// 
			// panel_stage1
			// 
			panel_stage1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel_stage1.BackColor = SystemColors.Control;
			panel_stage1.Controls.Add(splitContainer1);
			panel_stage1.Location = new Point(0, 0);
			panel_stage1.Name = "panel_stage1";
			panel_stage1.Size = new Size(632, 320);
			panel_stage1.TabIndex = 1;
			// 
			// splitContainer1
			// 
			splitContainer1.BorderStyle = BorderStyle.FixedSingle;
			splitContainer1.Location = new Point(16, 16);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.BackColor = SystemColors.Control;
			splitContainer1.Panel1.Controls.Add(label_communityTitle);
			splitContainer1.Panel1.Controls.Add(pictureBox_cdLogo);
			splitContainer1.Panel1.Controls.Add(linkLabelDiscord);
			splitContainer1.Panel1.Controls.Add(linkLabelHomepage);
			splitContainer1.Panel1.Controls.Add(linkLabelWiki);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.BackColor = SystemColors.Control;
			splitContainer1.Panel2.Controls.Add(label_installPathTitle);
			splitContainer1.Panel2.Controls.Add(button_resetInstallPath);
			splitContainer1.Panel2.Controls.Add(button_browseInstallPath);
			splitContainer1.Panel2.Controls.Add(label_stage1Desc);
			splitContainer1.Panel2.Controls.Add(label_browseInstallPathDesc);
			splitContainer1.Panel2.Controls.Add(label_stage1Title);
			splitContainer1.Panel2.Controls.Add(richTextBox_pd2InstallPath);
			splitContainer1.Panel2.Paint += splitContainer1_Panel2_Paint;
			splitContainer1.Size = new Size(600, 290);
			splitContainer1.SplitterDistance = 185;
			splitContainer1.TabIndex = 16;
			// 
			// label_communityTitle
			// 
			label_communityTitle.Anchor = AnchorStyles.None;
			label_communityTitle.AutoSize = true;
			label_communityTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label_communityTitle.Location = new Point(10, 184);
			label_communityTitle.Name = "label_communityTitle";
			label_communityTitle.Size = new Size(156, 21);
			label_communityTitle.TabIndex = 5;
			label_communityTitle.Text = "COMMUNITY LINKS";
			label_communityTitle.Click += label_communityTitle_Click;
			// 
			// pictureBox_cdLogo
			// 
			pictureBox_cdLogo.ImageLocation = "https://raw.githubusercontent.com/Crackdown-PD2/deathvox/autoupdate/PRIME.png";
			pictureBox_cdLogo.Location = new Point(10, 9);
			pictureBox_cdLogo.Name = "pictureBox_cdLogo";
			pictureBox_cdLogo.Size = new Size(164, 164);
			pictureBox_cdLogo.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox_cdLogo.TabIndex = 4;
			pictureBox_cdLogo.TabStop = false;
			pictureBox_cdLogo.Click += pictureBox_cdLogo_Click;
			// 
			// linkLabelDiscord
			// 
			linkLabelDiscord.AutoSize = true;
			linkLabelDiscord.Location = new Point(10, 262);
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
			linkLabelHomepage.Location = new Point(10, 214);
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
			linkLabelWiki.Location = new Point(10, 238);
			linkLabelWiki.Name = "linkLabelWiki";
			linkLabelWiki.Size = new Size(121, 15);
			linkLabelWiki.TabIndex = 3;
			linkLabelWiki.TabStop = true;
			linkLabelWiki.Text = "Crackdown Mod Wiki";
			linkLabelWiki.LinkClicked += linkLabelWiki_LinkClicked;
			// 
			// label_installPathTitle
			// 
			label_installPathTitle.AutoSize = true;
			label_installPathTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
			label_installPathTitle.Location = new Point(39, 158);
			label_installPathTitle.Name = "label_installPathTitle";
			label_installPathTitle.Size = new Size(174, 15);
			label_installPathTitle.TabIndex = 4;
			label_installPathTitle.Text = "PAYDAY 2 Installation Location";
			// 
			// button_resetInstallPath
			// 
			button_resetInstallPath.Anchor = AnchorStyles.None;
			button_resetInstallPath.Location = new Point(124, 225);
			button_resetInstallPath.Name = "button_resetInstallPath";
			button_resetInstallPath.Size = new Size(80, 28);
			button_resetInstallPath.TabIndex = 7;
			button_resetInstallPath.Text = "Reset";
			button_resetInstallPath.UseVisualStyleBackColor = true;
			button_resetInstallPath.Click += button_resetInstallPath_Click;
			// 
			// button_browseInstallPath
			// 
			button_browseInstallPath.Anchor = AnchorStyles.None;
			button_browseInstallPath.Location = new Point(38, 225);
			button_browseInstallPath.Name = "button_browseInstallPath";
			button_browseInstallPath.Size = new Size(80, 28);
			button_browseInstallPath.TabIndex = 8;
			button_browseInstallPath.Text = "Browse...";
			button_browseInstallPath.UseVisualStyleBackColor = true;
			button_browseInstallPath.Click += button_browseInstallPath_Click;
			// 
			// label_stage1Desc
			// 
			label_stage1Desc.Anchor = AnchorStyles.None;
			label_stage1Desc.Location = new Point(16, 57);
			label_stage1Desc.Name = "label_stage1Desc";
			label_stage1Desc.Size = new Size(366, 40);
			label_stage1Desc.TabIndex = 5;
			label_stage1Desc.Text = "This program will install the Crackdown Mod, as well as any dependency packages needed for it to run.";
			label_stage1Desc.Click += label_stage1Desc_Click;
			// 
			// label_browseInstallPathDesc
			// 
			label_browseInstallPathDesc.Anchor = AnchorStyles.None;
			label_browseInstallPathDesc.Location = new Point(16, 100);
			label_browseInstallPathDesc.Name = "label_browseInstallPathDesc";
			label_browseInstallPathDesc.Size = new Size(357, 50);
			label_browseInstallPathDesc.TabIndex = 9;
			label_browseInstallPathDesc.Text = "Before proceeding, please ensure that this path matches your PAYDAY 2 installation path. This should be the folder that contains the executable file called \"payday2_win32_release.exe\".";
			// 
			// label_stage1Title
			// 
			label_stage1Title.AutoSize = true;
			label_stage1Title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label_stage1Title.Location = new Point(39, 19);
			label_stage1Title.Name = "label_stage1Title";
			label_stage1Title.Size = new Size(330, 21);
			label_stage1Title.TabIndex = 4;
			label_stage1Title.Text = "Welcome to the Crackdown Mod Installer!";
			label_stage1Title.Click += label_stage1Title_Click;
			// 
			// richTextBox_pd2InstallPath
			// 
			richTextBox_pd2InstallPath.AccessibleDescription = "The path to your PAYDAY 2 installation.";
			richTextBox_pd2InstallPath.AccessibleName = "PAYDAY 2 folder path";
			richTextBox_pd2InstallPath.Anchor = AnchorStyles.None;
			richTextBox_pd2InstallPath.BackColor = Color.White;
			richTextBox_pd2InstallPath.DetectUrls = false;
			richTextBox_pd2InstallPath.HideSelection = false;
			richTextBox_pd2InstallPath.Location = new Point(39, 180);
			richTextBox_pd2InstallPath.Multiline = false;
			richTextBox_pd2InstallPath.Name = "richTextBox_pd2InstallPath";
			richTextBox_pd2InstallPath.ReadOnly = true;
			richTextBox_pd2InstallPath.ScrollBars = RichTextBoxScrollBars.ForcedHorizontal;
			richTextBox_pd2InstallPath.Size = new Size(334, 24);
			richTextBox_pd2InstallPath.TabIndex = 4;
			richTextBox_pd2InstallPath.TabStop = false;
			richTextBox_pd2InstallPath.Text = "";
			richTextBox_pd2InstallPath.WordWrap = false;
			richTextBox_pd2InstallPath.TextChanged += richTextBox1_TextChanged;
			// 
			// label_stage2Title
			// 
			label_stage2Title.Anchor = AnchorStyles.None;
			label_stage2Title.AutoSize = true;
			label_stage2Title.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
			label_stage2Title.Location = new Point(18, 12);
			label_stage2Title.Name = "label_stage2Title";
			label_stage2Title.Size = new Size(96, 15);
			label_stage2Title.TabIndex = 10;
			label_stage2Title.Text = "Select Packages";
			// 
			// button_quit
			// 
			button_quit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button_quit.Location = new Point(366, 5);
			button_quit.Name = "button_quit";
			button_quit.Size = new Size(80, 28);
			button_quit.TabIndex = 2;
			button_quit.Text = "Quit";
			button_quit.UseMnemonic = false;
			button_quit.UseVisualStyleBackColor = true;
			button_quit.Visible = false;
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
			// panel_stage2
			// 
			panel_stage2.BackColor = SystemColors.Control;
			panel_stage2.Controls.Add(label_stage3Desc_2);
			panel_stage2.Controls.Add(label_stage2Title);
			panel_stage2.Controls.Add(label_stage3_arrow);
			panel_stage2.Controls.Add(label_stage3Desc);
			panel_stage2.Controls.Add(panel_stage3_mouseoverDesc);
			panel_stage2.Controls.Add(checkedListBox_dummyMissingMods);
			panel_stage2.Controls.Add(label_installedModsList);
			panel_stage2.Controls.Add(button_detectExistingMods);
			panel_stage2.Controls.Add(label_missingModsList);
			panel_stage2.Controls.Add(checkedListBox_dummyInstalledMods);
			panel_stage2.Location = new Point(0, 380);
			panel_stage2.Name = "panel_stage2";
			panel_stage2.Size = new Size(632, 320);
			panel_stage2.TabIndex = 6;
			panel_stage2.Visible = false;
			// 
			// label_stage3Desc_2
			// 
			label_stage3Desc_2.Anchor = AnchorStyles.None;
			label_stage3Desc_2.Location = new Point(16, 56);
			label_stage3Desc_2.Name = "label_stage3Desc_2";
			label_stage3Desc_2.Size = new Size(600, 18);
			label_stage3Desc_2.TabIndex = 17;
			label_stage3Desc_2.Text = "Select which files to download and install, then click \"Next\".";
			// 
			// label_stage3_arrow
			// 
			label_stage3_arrow.AutoSize = true;
			label_stage3_arrow.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
			label_stage3_arrow.ForeColor = SystemColors.ControlDark;
			label_stage3_arrow.Location = new Point(285, 150);
			label_stage3_arrow.Name = "label_stage3_arrow";
			label_stage3_arrow.Size = new Size(48, 45);
			label_stage3_arrow.TabIndex = 21;
			label_stage3_arrow.Text = "→";
			label_stage3_arrow.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_stage3Desc
			// 
			label_stage3Desc.Location = new Point(16, 36);
			label_stage3Desc.Name = "label_stage3Desc";
			label_stage3Desc.Size = new Size(485, 19);
			label_stage3Desc.TabIndex = 2;
			label_stage3Desc.Text = "These are mods that Crackdown needs in order to work properly.";
			// 
			// panel_stage3_mouseoverDesc
			// 
			panel_stage3_mouseoverDesc.BorderStyle = BorderStyle.FixedSingle;
			panel_stage3_mouseoverDesc.Controls.Add(label_modDependenciesItemMouseoverDescription);
			panel_stage3_mouseoverDesc.Location = new Point(18, 275);
			panel_stage3_mouseoverDesc.Name = "panel_stage3_mouseoverDesc";
			panel_stage3_mouseoverDesc.Size = new Size(596, 28);
			panel_stage3_mouseoverDesc.TabIndex = 20;
			// 
			// label_modDependenciesItemMouseoverDescription
			// 
			label_modDependenciesItemMouseoverDescription.Anchor = AnchorStyles.None;
			label_modDependenciesItemMouseoverDescription.AutoSize = true;
			label_modDependenciesItemMouseoverDescription.Location = new Point(8, 6);
			label_modDependenciesItemMouseoverDescription.Name = "label_modDependenciesItemMouseoverDescription";
			label_modDependenciesItemMouseoverDescription.Size = new Size(258, 15);
			label_modDependenciesItemMouseoverDescription.TabIndex = 13;
			label_modDependenciesItemMouseoverDescription.Text = "Mouse over a dependency to see more about it.";
			label_modDependenciesItemMouseoverDescription.TextAlign = ContentAlignment.MiddleLeft;
			label_modDependenciesItemMouseoverDescription.Click += label_modDependenciesItemMouseverDescription_Click;
			// 
			// checkedListBox_dummyMissingMods
			// 
			checkedListBox_dummyMissingMods.CheckOnClick = true;
			checkedListBox_dummyMissingMods.Enabled = false;
			checkedListBox_dummyMissingMods.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
			checkedListBox_dummyMissingMods.FormattingEnabled = true;
			checkedListBox_dummyMissingMods.HorizontalScrollbar = true;
			checkedListBox_dummyMissingMods.Location = new Point(18, 105);
			checkedListBox_dummyMissingMods.Name = "checkedListBox_dummyMissingMods";
			checkedListBox_dummyMissingMods.Size = new Size(250, 157);
			checkedListBox_dummyMissingMods.TabIndex = 15;
			checkedListBox_dummyMissingMods.Visible = false;
			// 
			// label_installedModsList
			// 
			label_installedModsList.AutoSize = true;
			label_installedModsList.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
			label_installedModsList.Location = new Point(394, 84);
			label_installedModsList.Name = "label_installedModsList";
			label_installedModsList.Size = new Size(144, 15);
			label_installedModsList.TabIndex = 19;
			label_installedModsList.Text = "Packages already installed";
			// 
			// button_detectExistingMods
			// 
			button_detectExistingMods.Location = new Point(511, 12);
			button_detectExistingMods.Name = "button_detectExistingMods";
			button_detectExistingMods.Size = new Size(103, 28);
			button_detectExistingMods.TabIndex = 1;
			button_detectExistingMods.Text = "Re-detect Mods";
			button_detectExistingMods.UseVisualStyleBackColor = true;
			button_detectExistingMods.Visible = false;
			button_detectExistingMods.Click += button_detectExistingMods_Click;
			// 
			// label_missingModsList
			// 
			label_missingModsList.AutoSize = true;
			label_missingModsList.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
			label_missingModsList.Location = new Point(79, 84);
			label_missingModsList.Name = "label_missingModsList";
			label_missingModsList.Size = new Size(126, 15);
			label_missingModsList.TabIndex = 18;
			label_missingModsList.Text = "Packages to download";
			// 
			// checkedListBox_dummyInstalledMods
			// 
			checkedListBox_dummyInstalledMods.CheckOnClick = true;
			checkedListBox_dummyInstalledMods.Enabled = false;
			checkedListBox_dummyInstalledMods.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
			checkedListBox_dummyInstalledMods.FormattingEnabled = true;
			checkedListBox_dummyInstalledMods.HorizontalScrollbar = true;
			checkedListBox_dummyInstalledMods.Location = new Point(348, 105);
			checkedListBox_dummyInstalledMods.Name = "checkedListBox_dummyInstalledMods";
			checkedListBox_dummyInstalledMods.Size = new Size(266, 157);
			checkedListBox_dummyInstalledMods.TabIndex = 17;
			checkedListBox_dummyInstalledMods.Visible = false;
			// 
			// label_stage3Title
			// 
			label_stage3Title.AutoSize = true;
			label_stage3Title.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
			label_stage3Title.Location = new Point(14, 16);
			label_stage3Title.Name = "label_stage3Title";
			label_stage3Title.Size = new Size(117, 15);
			label_stage3Title.TabIndex = 16;
			label_stage3Title.Text = "Download Packages";
			label_stage3Title.Click += label_stage3Title_Click;
			// 
			// progressBar_downloadIndividual
			// 
			progressBar_downloadIndividual.Location = new Point(14, 263);
			progressBar_downloadIndividual.Name = "progressBar_downloadIndividual";
			progressBar_downloadIndividual.Size = new Size(241, 21);
			progressBar_downloadIndividual.TabIndex = 0;
			// 
			// listBox_downloadList
			// 
			listBox_downloadList.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
			listBox_downloadList.FormattingEnabled = true;
			listBox_downloadList.HorizontalScrollbar = true;
			listBox_downloadList.ItemHeight = 14;
			listBox_downloadList.Location = new Point(14, 40);
			listBox_downloadList.Name = "listBox_downloadList";
			listBox_downloadList.ScrollAlwaysVisible = true;
			listBox_downloadList.Size = new Size(600, 186);
			listBox_downloadList.TabIndex = 1;
			// 
			// label_downloadStatusDesc
			// 
			label_downloadStatusDesc.AutoSize = true;
			label_downloadStatusDesc.Location = new Point(14, 235);
			label_downloadStatusDesc.Name = "label_downloadStatusDesc";
			label_downloadStatusDesc.Size = new Size(224, 15);
			label_downloadStatusDesc.TabIndex = 3;
			label_downloadStatusDesc.Text = "Click \"Download\" to start the installation.";
			// 
			// label_downloadStatusTitle
			// 
			label_downloadStatusTitle.AutoSize = true;
			label_downloadStatusTitle.Location = new Point(262, 266);
			label_downloadStatusTitle.Name = "label_downloadStatusTitle";
			label_downloadStatusTitle.Size = new Size(62, 15);
			label_downloadStatusTitle.TabIndex = 2;
			label_downloadStatusTitle.Text = "- KB / - KB";
			// 
			// panel_stage3
			// 
			panel_stage3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel_stage3.BackColor = SystemColors.Control;
			panel_stage3.Controls.Add(label_stage3Title);
			panel_stage3.Controls.Add(progressBar_downloadIndividual);
			panel_stage3.Controls.Add(label_downloadStatusTitle);
			panel_stage3.Controls.Add(button_startDownload);
			panel_stage3.Controls.Add(listBox_downloadList);
			panel_stage3.Controls.Add(label_downloadStatusDesc);
			panel_stage3.Location = new Point(645, 0);
			panel_stage3.Name = "panel_stage3";
			panel_stage3.Size = new Size(632, 320);
			panel_stage3.TabIndex = 7;
			panel_stage3.Visible = false;
			// 
			// button_prevStage
			// 
			button_prevStage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button_prevStage.FlatStyle = FlatStyle.System;
			button_prevStage.Location = new Point(458, 5);
			button_prevStage.Name = "button_prevStage";
			button_prevStage.Size = new Size(74, 28);
			button_prevStage.TabIndex = 8;
			button_prevStage.Text = "&< Back";
			button_prevStage.UseVisualStyleBackColor = true;
			button_prevStage.Click += button_prevStage_Click;
			// 
			// button_nextStage
			// 
			button_nextStage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button_nextStage.Location = new Point(538, 5);
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
			panel_navigation.Location = new Point(0, 321);
			panel_navigation.Name = "panel_navigation";
			panel_navigation.Size = new Size(632, 40);
			panel_navigation.TabIndex = 10;
			panel_navigation.Paint += panel4_Paint;
			// 
			// label_navigation_stage4
			// 
			label_navigation_stage4.AutoSize = true;
			label_navigation_stage4.ForeColor = SystemColors.ControlDark;
			label_navigation_stage4.Location = new Point(305, 13);
			label_navigation_stage4.Name = "label_navigation_stage4";
			label_navigation_stage4.Size = new Size(38, 15);
			label_navigation_stage4.TabIndex = 13;
			label_navigation_stage4.Text = "Finish";
			// 
			// label_navigation_stage3
			// 
			label_navigation_stage3.AutoSize = true;
			label_navigation_stage3.ForeColor = SystemColors.ControlDark;
			label_navigation_stage3.Location = new Point(175, 13);
			label_navigation_stage3.Name = "label_navigation_stage3";
			label_navigation_stage3.Size = new Size(124, 15);
			label_navigation_stage3.TabIndex = 12;
			label_navigation_stage3.Text = "Download Packages >";
			// 
			// label_navigation_stage2
			// 
			label_navigation_stage2.AutoSize = true;
			label_navigation_stage2.ForeColor = SystemColors.ControlDark;
			label_navigation_stage2.Location = new Point(68, 13);
			label_navigation_stage2.Name = "label_navigation_stage2";
			label_navigation_stage2.Size = new Size(101, 15);
			label_navigation_stage2.TabIndex = 11;
			label_navigation_stage2.Text = "Select Packages >";
			// 
			// label_navigation_stage1
			// 
			label_navigation_stage1.AutoSize = true;
			label_navigation_stage1.Location = new Point(20, 13);
			label_navigation_stage1.Name = "label_navigation_stage1";
			label_navigation_stage1.Size = new Size(42, 15);
			label_navigation_stage1.TabIndex = 10;
			label_navigation_stage1.Text = "Start >";
			// 
			// button_openTempFolder
			// 
			button_openTempFolder.Anchor = AnchorStyles.None;
			button_openTempFolder.FlatStyle = FlatStyle.Popup;
			button_openTempFolder.Location = new Point(460, 140);
			button_openTempFolder.Name = "button_openTempFolder";
			button_openTempFolder.Size = new Size(154, 28);
			button_openTempFolder.TabIndex = 4;
			button_openTempFolder.Text = "Open Installer Logs Folder";
			button_openTempFolder.UseVisualStyleBackColor = true;
			button_openTempFolder.Click += button_openTempFolder_Click;
			// 
			// label_navigation_stage5
			// 
			label_navigation_stage5.AutoSize = true;
			label_navigation_stage5.ForeColor = SystemColors.ControlDark;
			label_navigation_stage5.Location = new Point(70, 267);
			label_navigation_stage5.Name = "label_navigation_stage5";
			label_navigation_stage5.Size = new Size(38, 15);
			label_navigation_stage5.TabIndex = 14;
			label_navigation_stage5.Text = "Finish";
			// 
			// button_finishAndLaunch
			// 
			button_finishAndLaunch.Location = new Point(162, 269);
			button_finishAndLaunch.Name = "button_finishAndLaunch";
			button_finishAndLaunch.Size = new Size(216, 28);
			button_finishAndLaunch.TabIndex = 6;
			button_finishAndLaunch.Text = "Close Installer and Launch PAYDAY 2";
			button_finishAndLaunch.UseVisualStyleBackColor = true;
			button_finishAndLaunch.Click += button_finishAndLaunch_Click;
			// 
			// panel_stage5
			// 
			panel_stage5.Anchor = AnchorStyles.None;
			panel_stage5.Controls.Add(label_navigation_stage5);
			panel_stage5.Location = new Point(1305, 342);
			panel_stage5.Name = "panel_stage5";
			panel_stage5.Size = new Size(632, 293);
			panel_stage5.TabIndex = 16;
			panel_stage5.Visible = false;
			panel_stage5.Paint += panel_stage5_Paint;
			// 
			// button_finalQuit
			// 
			button_finalQuit.Location = new Point(66, 269);
			button_finalQuit.Name = "button_finalQuit";
			button_finalQuit.Size = new Size(90, 28);
			button_finalQuit.TabIndex = 5;
			button_finalQuit.Text = "Close Installer";
			button_finalQuit.UseVisualStyleBackColor = true;
			button_finalQuit.Click += button_finalQuit_Click;
			// 
			// label_endDesc
			// 
			label_endDesc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			label_endDesc.Location = new Point(2, 56);
			label_endDesc.Name = "label_endDesc";
			label_endDesc.Size = new Size(628, 18);
			label_endDesc.TabIndex = 1;
			label_endDesc.Text = "You can now launch PAYDAY 2 and play the Crackdown Mod!";
			label_endDesc.TextAlign = ContentAlignment.TopCenter;
			// 
			// label_endTitle
			// 
			label_endTitle.Anchor = AnchorStyles.None;
			label_endTitle.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
			label_endTitle.Location = new Point(2, 19);
			label_endTitle.Name = "label_endTitle";
			label_endTitle.Size = new Size(628, 36);
			label_endTitle.TabIndex = 0;
			label_endTitle.Text = "Installation Complete";
			label_endTitle.TextAlign = ContentAlignment.TopCenter;
			label_endTitle.Click += label_endTitle_Click;
			// 
			// panel_stage4
			// 
			panel_stage4.BackColor = SystemColors.Control;
			panel_stage4.Controls.Add(linkLabelTroubleshooting);
			panel_stage4.Controls.Add(listBox_downloadFailedList);
			panel_stage4.Controls.Add(button_finalQuit);
			panel_stage4.Controls.Add(button_openTempFolder);
			panel_stage4.Controls.Add(label_endTitle);
			panel_stage4.Controls.Add(button_finishAndLaunch);
			panel_stage4.Controls.Add(label_endDesc);
			panel_stage4.Location = new Point(645, 380);
			panel_stage4.Name = "panel_stage4";
			panel_stage4.Size = new Size(632, 320);
			panel_stage4.TabIndex = 15;
			panel_stage4.Visible = false;
			// 
			// linkLabelTroubleshooting
			// 
			linkLabelTroubleshooting.AutoSize = true;
			linkLabelTroubleshooting.Location = new Point(14, 221);
			linkLabelTroubleshooting.Name = "linkLabelTroubleshooting";
			linkLabelTroubleshooting.Size = new Size(126, 15);
			linkLabelTroubleshooting.TabIndex = 17;
			linkLabelTroubleshooting.TabStop = true;
			linkLabelTroubleshooting.Text = "Troubleshooting guide";
			linkLabelTroubleshooting.LinkClicked += linkLabelTroubleshooting_LinkClicked;
			// 
			// listBox_downloadFailedList
			// 
			listBox_downloadFailedList.Anchor = AnchorStyles.None;
			listBox_downloadFailedList.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
			listBox_downloadFailedList.FormattingEnabled = true;
			listBox_downloadFailedList.HorizontalScrollbar = true;
			listBox_downloadFailedList.ItemHeight = 14;
			listBox_downloadFailedList.Items.AddRange(new object[] { "There were no installation errors." });
			listBox_downloadFailedList.Location = new Point(18, 105);
			listBox_downloadFailedList.Name = "listBox_downloadFailedList";
			listBox_downloadFailedList.Size = new Size(420, 102);
			listBox_downloadFailedList.TabIndex = 7;
			// 
			// Form1
			// 
			AcceptButton = button_nextStage;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Control;
			ClientSize = new Size(632, 361);
			Controls.Add(panel_stage5);
			Controls.Add(panel_stage3);
			Controls.Add(panel_stage4);
			Controls.Add(panel_stage2);
			Controls.Add(panel_navigation);
			Controls.Add(panel_stage1);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "Form1";
			SizeGripStyle = SizeGripStyle.Hide;
			Text = "Crackdown Mod Installer";
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
			panel_stage3_mouseoverDesc.ResumeLayout(false);
			panel_stage3_mouseoverDesc.PerformLayout();
			panel_stage3.ResumeLayout(false);
			panel_stage3.PerformLayout();
			panel_navigation.ResumeLayout(false);
			panel_navigation.PerformLayout();
			panel_stage5.ResumeLayout(false);
			panel_stage5.PerformLayout();
			panel_stage4.ResumeLayout(false);
			panel_stage4.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private Panel panel_stage1;
		private Button button_quit;
		private Button button_start;
		private FolderBrowserDialog folderBrowserDialog1;
		private RichTextBox richTextBox_pd2InstallPath;
		private Panel panel_stage2;
		private Panel panel_stage3;
		private LinkLabel linkLabelWiki;
		private LinkLabel linkLabelHomepage;
		private LinkLabel linkLabelDiscord;
		private Button button_prevStage;
		private Button button_nextStage;
		private Panel panel_navigation;
		private Label label_modDependenciesItemMouseoverDescription;
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
		private Label label_endTitle;
		private Label label_endDesc;
		private Button button_openTempFolder;
		private Button button_finalQuit;
		private Button button_finishAndLaunch;
		private Label label_navigation_stage5;
		private Label label_communityTitle;
		private Label label_stage1Desc;
		private Label label_stage3Desc_2;
		private Label label_installPathTitle;
		private ListBox listBox_downloadFailedList;
		private LinkLabel linkLabelTroubleshooting;
	}
}