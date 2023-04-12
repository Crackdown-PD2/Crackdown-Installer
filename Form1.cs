using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection.Emit;
using static Crackdown_Installer.InstallerManager;

namespace Crackdown_Installer
{
	public partial class Form1 : Form
	{
		int currentPage;
		int timesClickedTitle;
		List<Panel> panels;
		CheckedListBoxDisabledItems checkedListBox_modDependencyItems;
		bool hasDoneQueryUpdateServer;
		public Form1()
		{
			InitializeComponent();

			hasDoneQueryUpdateServer = false;

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

			//inherit properties from dummy
			checkedListBox_modDependencyItems = new();
			checkedListBox_modDependencyItems.CheckOnClick = checkedListBox_dummy.CheckOnClick;
			checkedListBox_modDependencyItems.Size = checkedListBox_dummy.Size;
			checkedListBox_modDependencyItems.AutoSize = checkedListBox_dummy.AutoSize;
			checkedListBox_modDependencyItems.MinimumSize = checkedListBox_dummy.MinimumSize;
			checkedListBox_modDependencyItems.MaximumSize = checkedListBox_dummy.MaximumSize;
			checkedListBox_modDependencyItems.Location = checkedListBox_dummy.Location;

			panel_stage4.Controls.Add(checkedListBox_modDependencyItems);

		}


		private void panel_stage3_OnVisibleChanged(object sender, EventArgs e)
		{
			if (panel_stage3.Visible && !hasDoneQueryUpdateServer)
			{
				/*
				foreach (ModDependencyEntry a in InstallerWrapper.instMgr.CollectDependencies())
				{
					LogMessage(a.Name);
				}
				List<ModDependencyEntry> missingMods = InstallerWrapper.instMgr.CollectMissingMods();
				foreach (ModDependencyEntry entry in missingMods)
				{
					checkedListBox_modDependencyItems.Items.Clear();
					int itemIndex = checkedListBox_modDependencyItems.Items.Add(entry.Name);
					System.Diagnostics.Debug.WriteLine("Adding mod " + entry.Name);

					if (itemIndex != -1 && !(bool)entry.Optional)
					{
						checkedListBox_modDependencyItems.CheckAndDisable(itemIndex);
					}
				}
				*/
				hasDoneQueryUpdateServer = true;
			}
		}

		//quit button
		private void button1_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void button_start_Click(object sender, EventArgs e)
		{
			//
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
			System.Diagnostics.Debug.WriteLine(folderBrowserDialog1.SelectedPath);
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