using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection.Emit;
using static Crackdown_Installer.InstallerManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Metadata.Ecma335;

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
		bool hasDonePopulateDependencies;

		//placeholders; should use the localization resource framework provided by microsoft
		const string DEPENDENCY_NEEDS_UPDATE = "Older version detected; an update is available.";
		const string DEPENDENCY_ALREADY_INSTALLED = "This mod has been installed and is up to date.";
		const string DEPENDENCY_NEEDS_INSTALL = "This mod is required but has not yet been installed.";

		public Form1()
		{
			InitializeComponent();

			hasDonePopulateDependencies = false;

			currentPage = 0;
			button_prevStage.Enabled = false;
			timesClickedTitle = 0;

			string detectedDirectory = InstallerWrapper.instMgr.GetPd2InstallDirectory();
			richTextBox1.Text = detectedDirectory;
			folderBrowserDialog1.InitialDirectory = detectedDirectory;


			panels = new List<Panel>();
			panels.Add(panel_stage1);
			panels.Add(panel_stage2);
			panels.Add(panel_stage3);//		panels.Add(panelNavigation)

			panel_stage3.VisibleChanged += new EventHandler(this.panel_stage3_OnVisibleChanged);

			//inherit selected properties from dummy
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

			AddMouseoverToolTip(label_installedModsList, "These are mods that you already have installed.");
			AddMouseoverToolTip(label_missingModsList, "These are required mods that are either not installed, or are outdated and need to be updated.");
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
					System.Diagnostics.Debug.WriteLine("Mousing over index " + index);
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
			if (panel_stage3.Visible && !hasDonePopulateDependencies)
			{
				InstallerWrapper.instMgr.CollectPd2Mods();

				List<ModDependencyEntry> dependencyEntries = InstallerWrapper.instMgr.GetDependencyEntries();
				checkedListBox_missingDependencyItems.Items.Clear();
				//AddMouseoverToolTip(checkedListBox_installedDependencyItems, "These items are already installed and will be skipped.");
				//AddMouseoverToolTip(checkedListBox_missingDependencyItems, "These items must be installed in order to use Crackdown.");

				foreach (ModDependencyEntry entry in dependencyEntries)
				{

					bool isOptional = entry.Optional;
					string modName = entry.Name;
					string modType = entry.DirectoryType;
					string modDesc = entry.Description;
					string modVersionType = entry.ModVersionType;
					string modVersionId = entry.ModVersionId;
					bool isInstalled = false;
					string? currentVersion = string.Empty;
					//optional field for mods; not guaranteed to exist

					if (!string.IsNullOrEmpty(modName) && !string.IsNullOrEmpty(modType))
					{
						Pd2ModFolder? modFolder = null;
						Pd2ModData? definitionFile = null;
						if (modType == "blt")
						{
							modFolder = InstallerWrapper.instMgr.GetInstalledBltMod(modName);
						}
						else if (modType == "beardlib")
						{
							modFolder = InstallerWrapper.instMgr.GetInstalledBeardlibMod(modName);
						}

						if (modFolder != null)
						{
							isInstalled = true;

							if (modVersionType == "xml")
							{

								definitionFile = modFolder.xmlModDefinition;
							}
							else if (modVersionType == "json")
							{
								definitionFile = modFolder.jsonModDefinition;
							}

							if (definitionFile != null)
							{
								currentVersion = definitionFile.GetVersion();
							}
						}

					}
					if (isInstalled)
					{
						//add to installed list
						int itemIndex = checkedListBox_installedDependencyItems.Items.Add(modName, true);
						if (itemIndex > -1)
						{
							//							AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex);
							AddMouseoverDescription(checkedListBox_installedDependencyItems, itemIndex, modDesc, label_modDependenciesItemMouseverDescription);
							checkedListBox_installedDependencyItems.CheckAndDisable(itemIndex);
						}


						//check manifest version against installed version
						//if version mismatch, also add to missing list as an optional update
						if (modVersionId != null && currentVersion != null && modVersionId != currentVersion)
						{
							int itemIndex2 = checkedListBox_missingDependencyItems.Items.Add(modName, true);
							if (itemIndex2 > -1)
							{
								AddMouseoverToolTip(checkedListBox_missingDependencyItems, itemIndex2, DEPENDENCY_NEEDS_UPDATE);
								AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex, DEPENDENCY_NEEDS_UPDATE);
							}
						}
						else
						{
							AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex, DEPENDENCY_ALREADY_INSTALLED);
						}
					}
					else
					{
						int itemIndex = checkedListBox_missingDependencyItems.Items.Add(modName, true);
						AddMouseoverDescription(checkedListBox_missingDependencyItems, itemIndex, modDesc, label_modDependenciesItemMouseverDescription);
						AddMouseoverToolTip(checkedListBox_missingDependencyItems, itemIndex, DEPENDENCY_NEEDS_INSTALL);
						if (itemIndex != -1)
						{
							//System.Diagnostics.Debug.WriteLine("Adding missing mod " + modName + " " + itemIndex);
							if (!isOptional)
							{
								checkedListBox_missingDependencyItems.CheckAndDisable(itemIndex);
							}
						}
					}

				}
				hasDonePopulateDependencies = true;
			}
		}

		//quit button
		private void button1_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void button_start_Click(object sender, EventArgs e)
		{

		}

		private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
		{

		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void linkLabelDiscord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://discord.gg/dak2zQ2");
			}
			catch (System.ComponentModel.Win32Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
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
				System.Diagnostics.Debug.WriteLine(ex.Message);
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
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button_browsePath_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog1.ShowDialog();
		}

		private void button_nextStage_Click(object sender, EventArgs e)
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

		private void button_prevStage_Click(object sender, EventArgs e)
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

		private void panel4_Paint(object sender, PaintEventArgs e)
		{

		}

		private void labelStage3Title_Click(object sender, EventArgs e)
		{

		}

		private void button_detectExistingMods_Click(object sender, EventArgs e)
		{

		}

		private void button_browseInstallPath_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				richTextBox1.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void button_resetInstallPath_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = folderBrowserDialog1.InitialDirectory;
			richTextBox1.Text = folderBrowserDialog1.InitialDirectory;
		}

		private void label_stage1Title_Click(object sender, EventArgs e)
		{
			if (++timesClickedTitle > 4)
			{
				System.Diagnostics.Debug.WriteLine("Stop clicking him, he's already dead!");
			}
		}

		private void label_stage3Title_Click(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
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
				string s = Items[e.Index].ToString();

				if (_checkedAndDisabledItems.Contains(s) || _checkedAndDisabledIndexes.Contains(e.Index))
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