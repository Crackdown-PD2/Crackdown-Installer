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
			panel_stage1 = new Panel();
			label_stage1Title = new Label();
			linkLabelWiki = new LinkLabel();
			linkLabelHomepage = new LinkLabel();
			button_start = new Button();
			linkLabelDiscord = new LinkLabel();
			button_quit = new Button();
			folderBrowserDialog1 = new FolderBrowserDialog();
			richTextBox1 = new RichTextBox();
			panel_stage2 = new Panel();
			label_stage2Title = new Label();
			label_browseInstallPathDesc = new Label();
			button_browseInstallPath = new Button();
			button_resetInstallPath = new Button();
			panel_stage3 = new Panel();
			label_installedModsList = new Label();
			label_missingModsList = new Label();
			label_modDependenciesItemMouseverDescription = new Label();
			checkedListBox_dummyMissingMods = new CheckedListBox();
			label_stage3Title = new Label();
			label_stage3Desc = new Label();
			button_detectExistingMods = new Button();
			button_prevStage = new Button();
			button_nextStage = new Button();
			panel_navigation = new Panel();
			panel_stage4 = new Panel();
			toolTip1 = new ToolTip(components);
			checkedListBox_dummyInstalledMods = new CheckedListBox();
			panel_stage1.SuspendLayout();
			panel_stage2.SuspendLayout();
			panel_stage3.SuspendLayout();
			panel_navigation.SuspendLayout();
			SuspendLayout();
			// 
			// panel_stage1
			// 
			panel_stage1.Controls.Add(label_stage1Title);
			panel_stage1.Controls.Add(linkLabelWiki);
			panel_stage1.Controls.Add(linkLabelHomepage);
			panel_stage1.Controls.Add(button_start);
			panel_stage1.Controls.Add(linkLabelDiscord);
			panel_stage1.Location = new Point(22, 51);
			panel_stage1.Name = "panel_stage1";
			panel_stage1.Size = new Size(201, 274);
			panel_stage1.TabIndex = 1;
			// 
			// label_stage1Title
			// 
			label_stage1Title.AutoSize = true;
			label_stage1Title.Location = new Point(11, 12);
			label_stage1Title.Name = "label_stage1Title";
			label_stage1Title.Size = new Size(229, 15);
			label_stage1Title.TabIndex = 4;
			label_stage1Title.Text = "Welcome to the Crackdown Mod Installer!";
			label_stage1Title.Click += label_stage1Title_Click;
			// 
			// linkLabelWiki
			// 
			linkLabelWiki.AutoSize = true;
			linkLabelWiki.Location = new Point(11, 85);
			linkLabelWiki.Name = "linkLabelWiki";
			linkLabelWiki.Size = new Size(121, 15);
			linkLabelWiki.TabIndex = 3;
			linkLabelWiki.TabStop = true;
			linkLabelWiki.Text = "Crackdown Mod Wiki";
			linkLabelWiki.LinkClicked += linkLabelWiki_LinkClicked;
			// 
			// linkLabelHomepage
			// 
			linkLabelHomepage.AutoSize = true;
			linkLabelHomepage.Location = new Point(6, 62);
			linkLabelHomepage.Name = "linkLabelHomepage";
			linkLabelHomepage.Size = new Size(140, 15);
			linkLabelHomepage.TabIndex = 2;
			linkLabelHomepage.TabStop = true;
			linkLabelHomepage.Text = "Crackdown Mod Website";
			linkLabelHomepage.LinkClicked += linkLabelHomepage_LinkClicked;
			// 
			// button_start
			// 
			button_start.Location = new Point(15, 229);
			button_start.Name = "button_start";
			button_start.Size = new Size(108, 27);
			button_start.TabIndex = 3;
			button_start.Text = "&Start";
			button_start.UseVisualStyleBackColor = true;
			button_start.Click += button_start_Click;
			// 
			// linkLabelDiscord
			// 
			linkLabelDiscord.AutoSize = true;
			linkLabelDiscord.Location = new Point(9, 39);
			linkLabelDiscord.Name = "linkLabelDiscord";
			linkLabelDiscord.Size = new Size(138, 15);
			linkLabelDiscord.TabIndex = 1;
			linkLabelDiscord.TabStop = true;
			linkLabelDiscord.Text = "Crackdown Mod Discord";
			linkLabelDiscord.LinkClicked += linkLabelDiscord_LinkClicked;
			// 
			// button_quit
			// 
			button_quit.Anchor = AnchorStyles.Bottom;
			button_quit.Location = new Point(1007, 24);
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
			richTextBox1.Location = new Point(16, 47);
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
			panel_stage2.Controls.Add(label_stage2Title);
			panel_stage2.Controls.Add(label_browseInstallPathDesc);
			panel_stage2.Controls.Add(button_browseInstallPath);
			panel_stage2.Controls.Add(button_resetInstallPath);
			panel_stage2.Controls.Add(richTextBox1);
			panel_stage2.Location = new Point(244, 51);
			panel_stage2.Name = "panel_stage2";
			panel_stage2.Size = new Size(484, 162);
			panel_stage2.TabIndex = 6;
			panel_stage2.Visible = false;
			// 
			// label_stage2Title
			// 
			label_stage2Title.AutoSize = true;
			label_stage2Title.Location = new Point(16, 27);
			label_stage2Title.Name = "label_stage2Title";
			label_stage2Title.Size = new Size(172, 15);
			label_stage2Title.TabIndex = 10;
			label_stage2Title.Text = "Auto-detected PAYDAY 2 folder";
			// 
			// label_browseInstallPathDesc
			// 
			label_browseInstallPathDesc.Location = new Point(16, 89);
			label_browseInstallPathDesc.Name = "label_browseInstallPathDesc";
			label_browseInstallPathDesc.Size = new Size(404, 36);
			label_browseInstallPathDesc.TabIndex = 9;
			label_browseInstallPathDesc.Text = "Before proceeding, please ensure that this path matches your PAYDAY 2 installation path.";
			// 
			// button_browseInstallPath
			// 
			button_browseInstallPath.Location = new Point(320, 47);
			button_browseInstallPath.Name = "button_browseInstallPath";
			button_browseInstallPath.Size = new Size(79, 30);
			button_browseInstallPath.TabIndex = 8;
			button_browseInstallPath.Text = "Browse...";
			button_browseInstallPath.UseVisualStyleBackColor = true;
			button_browseInstallPath.Click += button_browseInstallPath_Click;
			// 
			// button_resetInstallPath
			// 
			button_resetInstallPath.Location = new Point(405, 47);
			button_resetInstallPath.Name = "button_resetInstallPath";
			button_resetInstallPath.Size = new Size(66, 30);
			button_resetInstallPath.TabIndex = 7;
			button_resetInstallPath.Text = "Reset";
			button_resetInstallPath.UseVisualStyleBackColor = true;
			button_resetInstallPath.Click += button_resetInstallPath_Click;
			// 
			// panel_stage3
			// 
			panel_stage3.Controls.Add(label_installedModsList);
			panel_stage3.Controls.Add(label_missingModsList);
			panel_stage3.Controls.Add(checkedListBox_dummyInstalledMods);
			panel_stage3.Controls.Add(label_modDependenciesItemMouseverDescription);
			panel_stage3.Controls.Add(checkedListBox_dummyMissingMods);
			panel_stage3.Controls.Add(label_stage3Title);
			panel_stage3.Controls.Add(label_stage3Desc);
			panel_stage3.Controls.Add(button_detectExistingMods);
			panel_stage3.Location = new Point(244, 230);
			panel_stage3.Name = "panel_stage3";
			panel_stage3.Size = new Size(701, 334);
			panel_stage3.TabIndex = 7;
			panel_stage3.Visible = false;
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
			// label_modDependenciesItemMouseverDescription
			// 
			label_modDependenciesItemMouseverDescription.AutoSize = true;
			label_modDependenciesItemMouseverDescription.Location = new Point(195, 305);
			label_modDependenciesItemMouseverDescription.Name = "label_modDependenciesItemMouseverDescription";
			label_modDependenciesItemMouseverDescription.Size = new Size(258, 15);
			label_modDependenciesItemMouseverDescription.TabIndex = 13;
			label_modDependenciesItemMouseverDescription.Text = "Mouse over a dependency to see more about it.";
			// 
			// checkedListBox_dummyMissingMods
			// 
			checkedListBox_dummyMissingMods.CheckOnClick = true;
			checkedListBox_dummyMissingMods.Enabled = false;
			checkedListBox_dummyMissingMods.FormattingEnabled = true;
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
			button_detectExistingMods.Location = new Point(267, 52);
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
			button_prevStage.Anchor = AnchorStyles.Bottom;
			button_prevStage.Location = new Point(845, 24);
			button_prevStage.Name = "button_prevStage";
			button_prevStage.Size = new Size(74, 28);
			button_prevStage.TabIndex = 8;
			button_prevStage.Text = "Back";
			button_prevStage.UseVisualStyleBackColor = true;
			button_prevStage.Click += button_prevStage_Click;
			// 
			// button_nextStage
			// 
			button_nextStage.Anchor = AnchorStyles.Bottom;
			button_nextStage.Location = new Point(925, 24);
			button_nextStage.Name = "button_nextStage";
			button_nextStage.Size = new Size(76, 28);
			button_nextStage.TabIndex = 9;
			button_nextStage.Text = "Next";
			button_nextStage.UseVisualStyleBackColor = true;
			button_nextStage.Click += button_nextStage_Click;
			// 
			// panel_navigation
			// 
			panel_navigation.Controls.Add(button_prevStage);
			panel_navigation.Controls.Add(button_nextStage);
			panel_navigation.Controls.Add(button_quit);
			panel_navigation.Location = new Point(33, 589);
			panel_navigation.Name = "panel_navigation";
			panel_navigation.Size = new Size(1122, 65);
			panel_navigation.TabIndex = 10;
			panel_navigation.Paint += panel4_Paint;
			// 
			// panel_stage4
			// 
			panel_stage4.Location = new Point(28, 378);
			panel_stage4.Name = "panel_stage4";
			panel_stage4.Size = new Size(195, 172);
			panel_stage4.TabIndex = 15;
			// 
			// checkedListBox_dummyInstalledMods
			// 
			checkedListBox_dummyInstalledMods.CheckOnClick = true;
			checkedListBox_dummyInstalledMods.Enabled = false;
			checkedListBox_dummyInstalledMods.FormattingEnabled = true;
			checkedListBox_dummyInstalledMods.Location = new Point(366, 126);
			checkedListBox_dummyInstalledMods.Name = "checkedListBox_dummyInstalledMods";
			checkedListBox_dummyInstalledMods.Size = new Size(266, 166);
			checkedListBox_dummyInstalledMods.TabIndex = 17;
			checkedListBox_dummyInstalledMods.Visible = false;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1193, 666);
			Controls.Add(panel_stage4);
			Controls.Add(panel_navigation);
			Controls.Add(panel_stage3);
			Controls.Add(panel_stage2);
			Controls.Add(panel_stage1);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "Form1";
			Text = "Installer Wizard";
			Load += Form1_Load;
			panel_stage1.ResumeLayout(false);
			panel_stage1.PerformLayout();
			panel_stage2.ResumeLayout(false);
			panel_stage2.PerformLayout();
			panel_stage3.ResumeLayout(false);
			panel_stage3.PerformLayout();
			panel_navigation.ResumeLayout(false);
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
		private Panel panel_stage4;
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
	}
}