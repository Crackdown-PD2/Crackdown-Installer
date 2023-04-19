using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection.Emit;
using static Crackdown_Installer.InstallerManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Metadata.Ecma335;
using System.Drawing.Text;
using System.Security.Policy;
using ZNix.SuperBLT;
using System.IO;
using System.CodeDom;

namespace Crackdown_Installer
{
	public partial class Form1 : Form
	{
		const int TOOLTIP_HOVER_DURATION = 10000; //10000ms -> 10s
		int currentPage;
		int timesClickedTitle;
		List<Panel> panels;
		CheckedListBoxDisabledItems checkedListBox_missingDependencyItems;
		CheckedListBoxDisabledItems checkedListBox_installedDependencyItems;

		List<ModDependencyEntry> allModsToInstall = new();
		List<ModDependencyEntry> selectedModsToInstall = new();

		private bool isDownloadDependenciesInProgress = false;
		private bool isQueryDependenciesInProgress = false;

		//placeholders; should use the localization resource framework provided by microsoft
		const string TOOLTIP_DEPENDENCY_NEEDS_UPDATE = "Older version detected; an update is available.";
		const string TOOLTIP_DEPENDENCY_ALREADY_INSTALLED = "These are mods that you already have installed.";
		const string TOOLTIP_DEPENDENCY_NEEDS_INSTALL = "This dependency has not yet been installed.";
		const string DEPENDENCY_ALREADY_INSTALLED = "This mod has been installed and is up to date.";
		const string INSTALL_STATUS_TEXT = "[$STATUS$] $NAME$";
		const string INSTALL_STATUS_PENDING = "Pending";
		const string INSTALL_STATUS_DONE = "Done";
		const string INSTALL_STATUS_FAILED = "Failed";

		//constructor method
		//
		public Form1()
		{
			InitializeComponent();

			//set initial value for pd2 install path
			string detectedDirectory = InstallerWrapper.GetPd2InstallPath();
			richTextBox_pd2InstallPath.Text = detectedDirectory;
			folderBrowserDialog1.InitialDirectory = detectedDirectory;

			currentPage = 0;
			button_prevStage.Enabled = false;
			timesClickedTitle = 0;

			panels = new List<Panel>();
			panels.Add(panel_stage1);
			panels.Add(panel_stage2);
			panels.Add(panel_stage3);
			panels.Add(panel_stage4);
			//		panels.Add(panelNavigation)

			//register visibility change (aka on stage change) event callbacks
			//panel_stage2.VisibleChanged += new EventHandler(this.panel_stage2_OnVisibleChanged);
			panel_stage3.VisibleChanged += new EventHandler(this.panel_stage3_OnVisibleChanged);
			panel_stage4.VisibleChanged += new EventHandler(this.panel_stage4_OnVisibleChanged);

			//inherit selected properties from dummy checkboxes
			//since the visual editor obviously can't handle custom extended control classes
			checkedListBox_missingDependencyItems = new()
			{
				CheckOnClick = checkedListBox_dummyMissingMods.CheckOnClick,
				Cursor = checkedListBox_dummyMissingMods.Cursor,
				Size = checkedListBox_dummyMissingMods.Size,
				MinimumSize = checkedListBox_dummyMissingMods.MinimumSize,
				MaximumSize = checkedListBox_dummyMissingMods.MaximumSize,
				Location = checkedListBox_dummyMissingMods.Location
			};
			panel_stage3.Controls.Add(checkedListBox_missingDependencyItems);

			checkedListBox_installedDependencyItems = new()
			{
				CheckOnClick = checkedListBox_dummyInstalledMods.CheckOnClick,
				Cursor = checkedListBox_dummyInstalledMods.Cursor,
				Size = checkedListBox_dummyInstalledMods.Size,
				MinimumSize = checkedListBox_dummyInstalledMods.MinimumSize,
				MaximumSize = checkedListBox_dummyInstalledMods.MaximumSize,
				Location = checkedListBox_dummyInstalledMods.Location
			};
			panel_stage3.Controls.Add(checkedListBox_installedDependencyItems);

			//AddMouseoverToolTip(label_installedModsList, TOOLTIP_DEPENDENCY_ALREADY_INSTALLED);
			//AddMouseoverToolTip(label_missingModsList, TOOLTIP_DEPENDENCY_NEEDS_INSTALL);
		}


		/// <summary>
		/// 
		/// </summary>
		private void CheckExistingMods()
		{

			allModsToInstall.Clear();
			checkedListBox_missingDependencyItems.Items.Clear();
			checkedListBox_installedDependencyItems.Items.Clear();
			InstallerWrapper.CollectExistingMods();

			List<ModDependencyEntry> dependencyEntries = InstallerWrapper.GetModDependencyList();
			//AddMouseoverToolTip(checkedListBox_installedDependencyItems, "These items are already installed and will be skipped.");
			//AddMouseoverToolTip(checkedListBox_missingDependencyItems, "These items must be installed in order to use Crackdown.");

			//remove any existing dependency options

			foreach (ModDependencyEntry entry in dependencyEntries)
			{

				bool dependencyIsOptional = entry.IsOptional();
				string dependencyName = entry.GetName();
				string dependencyType = entry.GetDefinitionType();
				string dependencyFileName = entry.GetFileName();
				string dependencyDesc = entry.GetDescription();
				string dependencyVersionType = entry.GetModVersionType();
				string dependencyVersionId = entry.GetModVersionId();
				string? dependencyHash = entry.GetHash();

				bool dependencyExistingNeedsUpdate = false;
				bool isDependencyInstalled = false;
				bool ignoreThisDependency = false;
				string? currentVersion = string.Empty;
				string? currentHash = null;


				if (!string.IsNullOrEmpty(dependencyName) && !string.IsNullOrEmpty(dependencyType))
				{
					
					//most dependencies will be mods, which are typically folders
					Pd2ModFolder? modFolder = null;
					Pd2ModData? definitionFile = null;

					//dependencyType only indicates which definition file we use to identify a dependency as being installed or not
					//(searching for an exact name inside a given definition file)
					if (dependencyType == "blt")
					{
						LogMessage("Looking for installed mod " + dependencyName);
						modFolder = InstallerWrapper.GetBltMod(dependencyName);
						isDependencyInstalled = modFolder != null;
					}
					else if (dependencyType == "beardlib")
					{
						modFolder = InstallerWrapper.GetBeardlibMod(dependencyName);
						isDependencyInstalled = modFolder != null;

					}
					else if (dependencyType == "file")
					{
						//this indicates that the dependency is detected as being installed only by the presence of the file itself
						//since users can generally rename mod folders without impacting the mod's functionality
						//(unless the mod is poorly written and does not account for this, eg. by hard-coding the folder name)
						//then this should really only be used for mods that are individual files,
						//which we can then also hash to compare version names

						string pd2InstallationPath = InstallerWrapper.GetPd2InstallPath();
						string modPath = Path.Combine(pd2InstallationPath, dependencyFileName);

						Pd2ModData? miscModData = InstallerWrapper.GetMiscMod(dependencyFileName);

						if (miscModData != null)
						{
							isDependencyInstalled = true;
							if (miscModData.GetName() == dependencyName)
							{
								isDependencyInstalled = true;
								currentVersion = miscModData.GetVersion();
							}
							else
							{
								ignoreThisDependency = true;
							}
						}
						else
						{
							isDependencyInstalled = false;
						}
					}
					else
					{
						throw new Exception("Unknown dependency type: " + dependencyType);
					}

					if (dependencyVersionType == "hash")
					{
						//determine whether to hash directory or file

						string pd2InstallationPath = InstallerWrapper.GetPd2InstallPath();
						string modPath = Path.Combine(pd2InstallationPath, dependencyFileName);

						bool isDirectory = true;

						if (modFolder != null)
						{
							isDirectory = true;
						}
						else
						{
							if (!string.IsNullOrEmpty(dependencyFileName))
							{
								string lastChar = dependencyFileName.Substring(dependencyFileName.Length - 1);

								isDirectory = lastChar == "\\" || lastChar == "/";
							}
						}
						
						if (isDirectory)
						{
							currentHash = Hasher.HashDirectory(modPath);
						}
						else
						{
							currentHash = Hasher.HashFile(modPath);
						}

						dependencyExistingNeedsUpdate = dependencyHash != currentHash;
					}
					else
					{ //standard mod folder
						if (dependencyVersionType == "xml")
						{
							if (modFolder != null)
							{
								definitionFile = modFolder.xmlModDefinition;
							}
						}
						else if (dependencyVersionType == "json")
						{
							if (modFolder != null)
							{
								definitionFile = modFolder.jsonModDefinition;
							}
						}

						if (definitionFile != null)
						{
							currentVersion = definitionFile.GetVersion();
							dependencyExistingNeedsUpdate = currentVersion != dependencyVersionId;
						}
					}
				}


				if (!ignoreThisDependency)
				{
					if (isDependencyInstalled)
					{
						//add to installed list
						int itemIndex = checkedListBox_installedDependencyItems.Items.Add(dependencyName, true);
						if (itemIndex > -1)
						{
							//AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex);
							AddMouseoverDescription(checkedListBox_installedDependencyItems, itemIndex, dependencyDesc, label_modDependenciesItemMouseverDescription);
							checkedListBox_installedDependencyItems.CheckAndDisable(itemIndex);
						}


						//check manifest version against installed version
						//if version mismatch, also add to missing list as an optional update
						if (dependencyExistingNeedsUpdate)
						{
							int itemIndex2 = checkedListBox_missingDependencyItems.Items.Add(dependencyName, true);
							if (itemIndex2 > -1)
							{
								AddMouseoverToolTip(checkedListBox_missingDependencyItems, itemIndex2, TOOLTIP_DEPENDENCY_NEEDS_UPDATE);
								AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex, TOOLTIP_DEPENDENCY_NEEDS_UPDATE);

								allModsToInstall.Add(entry);
							}
						}
						else
						{
							AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex, DEPENDENCY_ALREADY_INSTALLED);
						}
					}
					else
					{
						int itemIndex = checkedListBox_missingDependencyItems.Items.Add(dependencyName, true);
						allModsToInstall.Add(entry);
						if (itemIndex != -1)
						{
							AddMouseoverDescription(checkedListBox_missingDependencyItems, itemIndex, dependencyDesc, label_modDependenciesItemMouseverDescription);
							//System.Diagnostics.Debug.WriteLine("Adding missing mod " + modName + " " + itemIndex);
							AddMouseoverToolTip(checkedListBox_missingDependencyItems, itemIndex, TOOLTIP_DEPENDENCY_NEEDS_INSTALL);
							if (!dependencyIsOptional)
							{
								checkedListBox_missingDependencyItems.CheckAndDisable(itemIndex);
							}
						}
					}
				}
			}
		}



		private void CallbackDetectPd2InstallDirectory()
		{
		}

		private void CallbackPopulateDependencyInstallList()
		{
			selectedModsToInstall.Clear();
			foreach (int i in checkedListBox_missingDependencyItems.CheckedIndices)
			{
				ModDependencyEntry entry = allModsToInstall[i];
				if (entry != null)
				{
					selectedModsToInstall.Add(entry);
				}
			}

			listBox_downloadList.Items.Clear();
			foreach (ModDependencyEntry m in selectedModsToInstall)
			{
				string statusText = INSTALL_STATUS_TEXT.Replace("$NAME$", m.GetName())
						.Replace("$STATUS$", INSTALL_STATUS_PENDING);
				int i = listBox_downloadList.Items.Add(statusText);
			}
		}


		void SetDownloadProgressBar(System.Windows.Forms.ProgressBar p, int current, int total)
		{
			p.Value = current / total * 100;
		}

		void SetDownloadProgressBar(System.Windows.Forms.ProgressBar p, int progress)
		{
			p.Value = progress;
		}

		/// <summary>
		/// Adds an event handler to set the given label to the given text when hovering over the given element in the given CheckedListBox.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="checkboxIndex"></param>
		/// <param name="descriptionText"></param>
		/// <param name="descLabel"></param>
		void AddMouseoverDescription(CheckedListBox o, int checkboxIndex, string descriptionText, System.Windows.Forms.Label descLabel)
		{
			void OnMouseMove(object? sender, EventArgs e)
			{

				Point pos = o.PointToClient(MousePosition);

				int index = o.IndexFromPoint(pos);

				if (index == checkboxIndex)
				{
					pos = this.PointToClient(MousePosition);
					if (!descLabel.Visible)
					{
						descLabel.Show();
					}
					descLabel.Text = descriptionText;
				}
			}

			void OnMouseLeave(object? sender, EventArgs e)
			{
				descLabel.Hide();
			}
			//o.MouseHover+= new EventHandler(OnMouseHover);
			o.MouseMove += new MouseEventHandler(OnMouseMove);
			o.MouseLeave += new EventHandler(OnMouseLeave);
		}

		/// <summary>
		/// Adds an event handler to show the given tooltip text when hovering over the given element in the given CheckedListBox.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="checkboxIndex"></param>
		/// <param name="tooltipText"></param>
		void AddMouseoverToolTip(CheckedListBox o, int checkboxIndex, string tooltipText)
		{
			void OnMouseMove(object? sender, EventArgs e)
			{

				Point pos = o.PointToClient(MousePosition);

				int index = o.IndexFromPoint(pos);

				//TODO optimize by passing a linked list/array of descriptions for the whole CheckedListBox
				if (index == checkboxIndex)
				{
					pos = this.PointToClient(MousePosition);
					toolTip1.Show(tooltipText, this, pos.X, pos.Y, TOOLTIP_HOVER_DURATION);
					/*
					LogMessage.WriteLine("Mousing over index " + index);
					//string? s = null;
					string? s = descriptions[index + 1];
					if (s != null) { 
						toolTip1.Show(s, this, pos.X, pos.Y, 5000);
					}
					*/
				}
			}

			void OnMouseLeave(object? sender, EventArgs e)
			{
				//hide tooltip
				Point pos = o.PointToClient(MousePosition);
				toolTip1.Hide(this);
			}
			o.MouseMove += new MouseEventHandler(OnMouseMove);
			o.MouseLeave += new EventHandler(OnMouseLeave);
		}

		/// <summary>
		/// Adds an event handler to show the given tooltip text when hovering over the given Control object.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="tooltipText"></param>
		void AddMouseoverToolTip(Control o, string tooltipText)
		{
			void OnMouseHover(object? sender, EventArgs e)
			{
				Point pos = o.PointToClient(MousePosition);
				toolTip1.Show(tooltipText, this, pos.X, pos.Y, TOOLTIP_HOVER_DURATION);
			}
			void OnMouseLeave(object? sender, EventArgs e)
			{
				Point pos = o.PointToClient(MousePosition);
				toolTip1.Hide(this);
			}
			o.MouseHover += new EventHandler(OnMouseHover);
			o.MouseLeave += new EventHandler(OnMouseLeave);
		}

		/// <summary>
		/// Event handler for showing the installation page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_stage3_OnVisibleChanged(object? sender, EventArgs e)
		{
			if (panel_stage3.Visible)
			{
				CheckExistingMods();
			}
		}

		/// <summary>
		/// Event handler for showing the installation page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_stage4_OnVisibleChanged(object? sender, EventArgs e)
		{
			if (panel_stage4.Visible)
			{
				CallbackPopulateDependencyInstallList();
			}
		}

		//quit button
		private void button1_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		//start download button
		private async void button_start_Click(object sender, EventArgs e)
		{
			button_prevStage.Enabled = false;
			isDownloadDependenciesInProgress = true;
			button_startDownload.Enabled = false;
			InstallerWrapper.CreateTemporaryDirectory();
			foreach (ModDependencyEntry dependencyEntry in selectedModsToInstall)
			{
				LogMessage(dependencyEntry.GetName());
				bool success = await InstallerWrapper.DownloadDependency(dependencyEntry);
				LogMessage("Success: " + success);
			}
			InstallerWrapper.DisposeTemporaryDirectory();
			button_prevStage.Enabled = true;
			isDownloadDependenciesInProgress = false;
			button_startDownload.Enabled = true;

			//private bool isQueryDependenciesInProgress = false;

		}

		private void MoveToNextStage()
		{
			//			System.Diagnostics.Debug.WriteLine("+++");
			//			System.Diagnostics.Debug.WriteLine(currentPage);
			//			System.Diagnostics.Debug.WriteLine(panels.Count);
			if (currentPage < panels.Count - 1)
			{
				button_prevStage.Enabled = true;

				Panel currentPanel = panels[currentPage];
				currentPanel.Hide();
				//				System.Diagnostics.Debug.WriteLine("current page " + currentPage);
				Panel nextPanel = panels[++currentPage];
				//				System.Diagnostics.Debug.WriteLine("next page " + currentPage);
				//				System.Diagnostics.Debug.WriteLine("hide " + currentPanel.Name);
				nextPanel.Show();
				//				System.Diagnostics.Debug.WriteLine("show " + nextPanel.Name);
				if (currentPage == panels.Count - 1)
				{
					button_nextStage.Enabled = false;
				}
			}
		}

		private void MoveToPrevStage()
		{
			//			System.Diagnostics.Debug.WriteLine("---");
			//			System.Diagnostics.Debug.WriteLine(currentPage);
			//			System.Diagnostics.Debug.WriteLine(panels.Count);
			if (currentPage > 0)
			{
				button_nextStage.Enabled = true;
				Panel currentPanel = panels[currentPage];

				currentPanel.Hide();
				//				System.Diagnostics.Debug.WriteLine("hide " + currentPanel.Name);

				//				System.Diagnostics.Debug.WriteLine("current page " + currentPage);
				Panel nextPanel = panels[--currentPage];
				//				System.Diagnostics.Debug.WriteLine("next page " + currentPage);
				nextPanel.Show();
				//				System.Diagnostics.Debug.WriteLine("show " + nextPanel.Name);
				if (currentPage == 0)
				{
					button_prevStage.Enabled = false;
				}
			}
		}

		private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
		{

		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{}

		private void linkLabelDiscord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://discord.gg/dak2zQ2");
			}
			catch (System.ComponentModel.Win32Exception ex)
			{
				LogMessage(ex.Message);
			}
		}

		private void linkLabelHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("http://crackdownmod.com/");
			}
			catch (System.ComponentModel.Win32Exception ex)
			{
				LogMessage(ex.Message);
			}
		}

		private void linkLabelWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://totalcrackdown.wiki.gg/");
			}
			catch (System.ComponentModel.Win32Exception ex)
			{
				LogMessage(ex.Message);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button_browsePath_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog1.ShowDialog();
		}

		private void button_nextStage_Click(object sender, EventArgs e) {
			MoveToNextStage();
		}

		private void button_prevStage_Click(object sender, EventArgs e)
		{
			MoveToPrevStage();
		}

		private void panel4_Paint(object sender, PaintEventArgs e)
		{

		}

		private void labelStage3Title_Click(object sender, EventArgs e)
		{

		}

		private void button_detectExistingMods_Click(object sender, EventArgs e)
		{
			if (!isQueryDependenciesInProgress)
			{
				CheckExistingMods();
			}
		}

		private void button_browseInstallPath_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				richTextBox_pd2InstallPath.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void button_resetInstallPath_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = folderBrowserDialog1.InitialDirectory;
			richTextBox_pd2InstallPath.Text = folderBrowserDialog1.InitialDirectory;
		}

		private void label_stage1Title_Click(object sender, EventArgs e)
		{
			if (++timesClickedTitle > 4)
			{
				LogMessage("Stop clicking him, he's already dead!");
			}
		}

		private void label_stage3Title_Click(object sender, EventArgs e)
		{

		}

		private void pictureBox_cdLogo_Click(object sender, EventArgs e)
		{

		}

		private void label_modDependenciesItemMouseverDescription_Click(object sender, EventArgs e)
		{

		}
	}

	//extends CheckedListBox
	//to allow disabling individual items 
	//from:
	//https://stackoverflow.com/questions/4368618/how-to-disable-a-checkbox-in-a-checkedlistbox
	public class CheckedListBoxDisabledItems : CheckedListBox
	{
		private List<string> _checkedAndDisabledItems = new List<string>();
		private List<int> _checkedAndDisabledIndexes = new List<int>();

		public void CheckAndDisable(string item)
		{
			_checkedAndDisabledItems.Add(item);
			this.Refresh();
		}

		public void CheckAndDisable(int index)
		{
			_checkedAndDisabledIndexes.Add(index);
			this.Refresh();
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index != -1)
			{
				string s = Items[e.Index].ToString() ?? string.Empty;

				if (!string.IsNullOrEmpty(s) && (_checkedAndDisabledItems.Contains(s)) || _checkedAndDisabledIndexes.Contains(e.Index))
				{
					System.Windows.Forms.VisualStyles.CheckBoxState state = System.Windows.Forms.VisualStyles.CheckBoxState.CheckedDisabled;
					Size glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
					CheckBoxRenderer.DrawCheckBox(
						e.Graphics,
						new Point(e.Bounds.X + 1, e.Bounds.Y + 1), // add one pixel to align the check gliph properly
						new Rectangle(
							new Point(e.Bounds.X + glyphSize.Width + 3, e.Bounds.Y), // add three pixels to align text properly
							new Size(e.Bounds.Width - glyphSize.Width, e.Bounds.Height)),
						s,
						this.Font,
						TextFormatFlags.Left, // text is centered by default
						false,
						state);
				}
				else
				{
					base.OnDrawItem(e);
				}
			}
		}

		public void ClearDisabledItems()
		{
			_checkedAndDisabledIndexes.Clear();
			_checkedAndDisabledItems.Clear();
			this.Refresh();
		}
	}

}