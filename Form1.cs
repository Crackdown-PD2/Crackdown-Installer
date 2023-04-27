using static Crackdown_Installer.InstallerManager;
using ZNix.SuperBLT;

namespace Crackdown_Installer
{
	public partial class Form1 : Form
	{
		const int TOOLTIP_HOVER_DURATION = 10000; //10000ms -> 10s
		int currentPage;
		int? nextPageOverride = null;

		int timesClickedTitle;
		List<Panel> panels;
		List<System.Windows.Forms.Label> labels;
		CheckedListBoxDisabledItems checkedListBox_missingDependencyItems;
		CheckedListBoxDisabledItems checkedListBox_installedDependencyItems;

		List<ModDependencyEntry> allModsToInstall = new();
		List<ModDependencyEntry> selectedModsToInstall = new();

		private bool isQueryDependenciesInProgress = false;
		private bool hasDoneCollectDependencies = false;

		// placeholders; should use the localization resource framework provided by microsoft
		const string TOOLTIP_DEPENDENCY_NEEDS_UPDATE = "Older version detected; an update is available.";
		//const string TOOLTIP_DEPENDENCY_ALREADY_INSTALLED = "These are mods that you already have installed.";
		const string TOOLTIP_DEPENDENCY_NEEDS_INSTALL = "This dependency has not yet been installed.";
		const string DEPENDENCY_ALREADY_INSTALLED = "This mod has been installed and is up to date.";
		const string INSTALL_STATUS_PENDING = "Pending";
		const string INSTALL_STATUS_DONE = "Done";
		const string INSTALL_STATUS_SUCCESS = "Installed";
		const string INSTALL_STATUS_FAILED = "Failed ($reason$)";
		const string INSTALL_STATUS_INPROGRESS = "In progress";
		const string STAGE_DESC_ALL_ALREADY_INSTALLED = "You already have all Crackdown packages installed and up-to-date!";
		const string END_DOWNLOAD_ERRORS_TITLE = "Installation Failed";
		const string END_DOWNLOAD_ERRORS_DESC = "One or more errors were detected.";

		/*
		readonly string[] DUCKSOUNDS = {
			@"quack1.wav",
			@"quack2.wav"
		};
		*/

		int localMaxDependencyNameLength = 0;
		const int MIN_NUM_SPACERS = 12;

		// constructor method
		public Form1()
		{
			InitializeComponent();

			// set initial value for pd2 install path
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
			Point basePanelLocation = new Point(0, 0);
			panel_stage1.Location = basePanelLocation;
			panel_stage2.Location = basePanelLocation;
			panel_stage3.Location = basePanelLocation;
			panel_stage4.Location = basePanelLocation;

			labels = new();
			labels.Add(label_navigation_stage1);
			labels.Add(label_navigation_stage2);
			labels.Add(label_navigation_stage3);
			labels.Add(label_navigation_stage4);

			//register visibility change (aka on stage change) event callbacks
			//panel_stage2.VisibleChanged += new EventHandler(this.panel_stage2_OnVisibleChanged);
			panel_stage2.VisibleChanged += new EventHandler(this.panel_stage2_OnVisibleChanged);
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
				Location = checkedListBox_dummyMissingMods.Location,
				Font = checkedListBox_dummyMissingMods.Font
			};
			panel_stage2.Controls.Add(checkedListBox_missingDependencyItems);

			// disable "next stage" button when there are no items selected
			void OnItemCheckChanged(object? sender, ItemCheckEventArgs e)
			{
				CheckedListBoxDisabledItems? send = sender as CheckedListBoxDisabledItems;

				// evaluate true number of checked items
				// since this event is run before the checked items count is updated
				int count = send?.CheckedItems.Count ?? 0;
				if (e.NewValue == CheckState.Unchecked)
				{
					count--;
				}
				else if (e.NewValue == CheckState.Checked)
				{
					count++;
				}
				button_nextStage.Enabled = count > 0;

			}
			checkedListBox_missingDependencyItems.ItemCheck += new ItemCheckEventHandler(OnItemCheckChanged);


			checkedListBox_installedDependencyItems = new()
			{
				CheckOnClick = checkedListBox_dummyInstalledMods.CheckOnClick,
				Cursor = checkedListBox_dummyInstalledMods.Cursor,
				Size = checkedListBox_dummyInstalledMods.Size,
				MinimumSize = checkedListBox_dummyInstalledMods.MinimumSize,
				MaximumSize = checkedListBox_dummyInstalledMods.MaximumSize,
				Location = checkedListBox_dummyInstalledMods.Location,
				Font = checkedListBox_dummyInstalledMods.Font
			};
			panel_stage2.Controls.Add(checkedListBox_installedDependencyItems);

		}


		/// <summary>
		/// 
		/// </summary>
		private void CheckExistingMods(List<ModDependencyEntry> dependencyEntries)
		{

			// remove any existing dependency options
			allModsToInstall.Clear();
			checkedListBox_missingDependencyItems.Items.Clear();
			checkedListBox_installedDependencyItems.Items.Clear();

			// get list of detected mods that are installed
			InstallerWrapper.CollectExistingMods();

			if (dependencyEntries.Count == 0)
			{
				//todo dialog box error
				//error fetching dependencies, please quit and try again later
				//or download manually
			}

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

					// most dependencies will be mods, which are typically folders
					Pd2ModFolder? modFolder = null;
					Pd2ModData? definitionFile = null;


					// find out if this dependency is already installed or not;
					// dependencyType indicates which definition file we use to identify a dependency as being installed or not
					// (searching for an exact name inside a given definition file)
					LogMessage($"Looking for installed {dependencyType} mod {dependencyName}");
					if (dependencyType == "json")
					{
						// use json definition
						modFolder = InstallerWrapper.GetBltMod(dependencyName);
						isDependencyInstalled = modFolder != null;
					}
					else if (dependencyType == "xml")
					{
						// use xml definition
						modFolder = InstallerWrapper.GetBeardlibMod(dependencyName);
						isDependencyInstalled = modFolder != null;

					}
					else if (dependencyType == "file")
					{
						// detect the presence of the file itself;
						// since users can generally rename mod folders without impacting the mod's functionality
						// (unless the mod is poorly written and does not account for this, eg. by hard-coding the folder name)
						// then this should really only be used for mods that are individual files,
						// which we can then also hash to compare version names

						string pd2InstallationPath = InstallerWrapper.GetPd2InstallPath();
						string modPath = Path.Combine(pd2InstallationPath, dependencyFileName);

						// find "loose" mod data
						// (each one is added manually on a casewise basis, eg. the sblt dll)
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
								// previously only used to ignore
								// dll installation for either WSOCK32 or IPHLPAPI since only one was required
								// (not used)
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

					// check manifest version against installed version
					if (dependencyVersionType == "hash")
					{
						// determine whether to hash directory or file

						string pd2InstallationPath = InstallerWrapper.GetPd2InstallPath();
						string modPath = Path.Combine(pd2InstallationPath, dependencyFileName);

						bool isDirectory = true;

						if (modFolder != null)
						{
							//if it has a mod folder, hash the mod folder
							isDirectory = true;
							isDependencyInstalled = System.IO.Directory.Exists(modPath);
						}
						else
						{
							// determine if the dependency is supposed to be a folder or a file based on the name
							isDirectory = InstallerWrapper.IsDirectory(dependencyFileName);
							if (isDirectory)
							{
								isDependencyInstalled = System.IO.Directory.Exists(modPath);
							}
							else
							{
								isDependencyInstalled = System.IO.File.Exists(modPath);
							}
						}

						if (isDependencyInstalled)
						{
							if (isDirectory)
							{
								currentHash = Hasher.HashDirectory(modPath);
							}
							else
							{
								currentHash = Hasher.HashFile(modPath);
							}
						}

						dependencyExistingNeedsUpdate = dependencyHash != currentHash;
						LogMessage($"Version check (hash): {dependencyHash} ?=  {currentHash} ?");
					}
					else
					{
						// is standard mod folder; check version in definition file
						if (dependencyVersionType == "xml")
						{
							// use beardlib (xml) definition file
							if (modFolder != null)
							{
								definitionFile = modFolder.xmlModDefinition;
							}
						}
						else if (dependencyVersionType == "json")
						{
							// use sblt (json) definition file
							if (modFolder != null)
							{
								definitionFile = modFolder.jsonModDefinition;
							}
						}

						if (definitionFile != null)
						{
							// compare version (or hash) here
							currentVersion = definitionFile.GetVersion();
							dependencyExistingNeedsUpdate = currentVersion != dependencyVersionId;

							LogMessage($"Version check: {currentVersion} ?=  {dependencyVersionId} ?");
						}
					}
				}


				LogMessage($"Is installed: {isDependencyInstalled}");

				if (!ignoreThisDependency)
				{
					if (isDependencyInstalled)
					{
						// add to already-installed list
						int itemIndex = checkedListBox_installedDependencyItems.Items.Add(dependencyName, true);
						if (itemIndex > -1)
						{
							AddMouseoverDescription(checkedListBox_installedDependencyItems, itemIndex, dependencyDesc, label_modDependenciesItemMouseoverDescription);
							checkedListBox_installedDependencyItems.CheckAndDisable(itemIndex);
						}


						// if version mismatch, also add to missing list as an optional update
						if (dependencyExistingNeedsUpdate)
						{
							int itemIndex2 = checkedListBox_missingDependencyItems.Items.Add(dependencyName, true);
							if (itemIndex2 > -1)
							{
								// give tooltip "installed but needs update";
								//add dependency to "to-install" list
								AddMouseoverToolTip(checkedListBox_missingDependencyItems, itemIndex2, TOOLTIP_DEPENDENCY_NEEDS_UPDATE);
								AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex, TOOLTIP_DEPENDENCY_NEEDS_UPDATE);
								allModsToInstall.Add(entry);
							}
						}
						else
						{
							// give tooltip "already installed", does not need update
							AddMouseoverToolTip(checkedListBox_installedDependencyItems, itemIndex, DEPENDENCY_ALREADY_INSTALLED);
						}
					}
					else
					{
						// is not installed; add to "to-install" list
						int itemIndex = checkedListBox_missingDependencyItems.Items.Add(dependencyName, true);
						allModsToInstall.Add(entry);
						if (itemIndex != -1)
						{
							AddMouseoverDescription(checkedListBox_missingDependencyItems, itemIndex, dependencyDesc, label_modDependenciesItemMouseoverDescription);
							AddMouseoverToolTip(checkedListBox_missingDependencyItems, itemIndex, TOOLTIP_DEPENDENCY_NEEDS_INSTALL);
							if (!dependencyIsOptional)
							{
								checkedListBox_missingDependencyItems.CheckAndDisable(itemIndex);
							}
						}
					}
				}
			}

			/*
			if (checkedListBox_missingDependencyItems.Items.Count == 0)
			{
				// tell user that no downloads are required,
				// and that they may quit and play at any time
				label_stage3Desc_2.Text = STAGE_DESC_ALL_ALREADY_INSTALLED;
				LogMessage($"Done all 1, current page {currentPage}");
				if (currentPage == 2)
				{
					button_nextStage.Enabled = true;
					// go to finish
					nextPageOverride = 3;
				}
			}
			*/
		}


		private void CallbackOnQuitButtonPressed()
		{
			//pre-quit callback
			InstallerWrapper.OnApplicationClose();

			//quit application
			Application.Exit();
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
					string entryName = entry.GetName();
					localMaxDependencyNameLength = Math.Max(entryName.Length, localMaxDependencyNameLength);
				}
			}

			listBox_downloadList.Items.Clear();
			foreach (ModDependencyEntry entry in selectedModsToInstall)
			{
				int i = listBox_downloadList.Items.Add(GetDownloadSpacerString(entry.GetName(), INSTALL_STATUS_PENDING));
			}

			//only enable download button if there is at least one item to download
			button_startDownload.Enabled = listBox_downloadList.Items.Count > 0;

		}

		private string GetDownloadSpacerString(string dependencyName, string statusName)
		{
			int nameLen = dependencyName.Length;
			int numSpacers = (localMaxDependencyNameLength - nameLen) + MIN_NUM_SPACERS;
			string spacerString = new String('.', numSpacers);
			return dependencyName + spacerString + statusName;
		}

		void SetDownloadProgressBar(double? percent, long current, long? total)
		{

			string convertBytes(int i)
			{
				if (i > 1000000)
				{
					return String.Format($"{i / 1000000}MB");
				}
				else if (i > 1000)
				{
					return String.Format($"{i / 1000}KB");
				}
				else
				{
					return String.Format($"{i}B");
				}
			}

			if (percent != null)
			{
				//				progressBar_downloadIndividual.Value = (int) percent;
			}
			else
			{
				//				progressBar_downloadIndividual.Value = 0;
			}


			string totalStr;
			if (total != null)
			{
				totalStr = convertBytes((int)total);
			}
			else
			{ totalStr = "-"; }

			string currentStr = convertBytes((int)current);

			label_downloadStatusTitle.Text = $"{currentStr} / {totalStr}";
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

		private void CallbackOnGetDependencies(List<ModDependencyEntry> modDependencyEntries)
		{
			if (!hasDoneCollectDependencies)
			{
				CheckExistingMods(modDependencyEntries);

				//LogMessage("Callback complete");

				//foreach (ModDependencyEntry entry in modDependencyEntries)
				//{
				//	LogMessage("DEBUG:", entry.GetName());
				//}

				hasDoneCollectDependencies = true;
				if (currentPage == 1)
				{
					button_nextStage.Enabled = true;
				}

				if (checkedListBox_missingDependencyItems.Items.Count == 0)
				{
					// tell user that no downloads are required,
					// and that they may quit and play at any time
					label_stage3Desc_2.Text = STAGE_DESC_ALL_ALREADY_INSTALLED;

					// go to finish
					nextPageOverride = 3;
				}
			}
		}

		/// <summary>
		/// Event handler for showing the "select packages" page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_stage2_OnVisibleChanged(object? sender, EventArgs e)
		{
			if (panel_stage2.Visible)
			{
				if (hasDoneCollectDependencies)
				{
					button_nextStage.Enabled = true;
				}
				else
				{
					button_nextStage.Enabled = false;

					// get list of dependency mods used in crackdown
					// and then populate missing mods list
					Action<List<ModDependencyEntry>> clbk = CallbackOnGetDependencies;
					InstallerWrapper.GetModDependencyListAsync(true, clbk);
				}
			}
		}

		/// <summary>
		/// Event handler for showing the "download packages" page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_stage3_OnVisibleChanged(object? sender, EventArgs e)
		{
			if (panel_stage3.Visible)
			{
				CallbackPopulateDependencyInstallList();
				button_nextStage.Enabled = false;
			}
		}

		/// <summary>
		/// Event handler for showing the "installation complete" page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_stage4_OnVisibleChanged(object? sender, EventArgs e)
		{
			if (panel_stage4.Visible)
			{
				button_nextStage.Visible = false;
				button_nextStage.Enabled = false;
				button_prevStage.Visible = false;
				button_prevStage.Enabled = false;
				button_quit.Visible = false;
				button_quit.Enabled = false;
			}
		}

		//quit button
		private void button1_Click(object sender, EventArgs e)
		{
			CallbackOnQuitButtonPressed();
		}

		//start download button
		private async void button_start_Click(object sender, EventArgs e)
		{
			button_prevStage.Enabled = false;
			button_startDownload.Enabled = false;

			List<DependencyDownloadResult> downloadResults = await DownloadSelectedDependencies();

			//button_prevStage.Enabled = true; //don't enable backtracking
			//button_startDownload.Enabled = true; //should only be enabled on re-evaluate missing mods
			CallbackOnDownloadDependenciesComplete(downloadResults);
		}


		private void CallbackOnDownloadDependenciesComplete(List<DependencyDownloadResult> downloadResults)
		{
			LogMessage($"All downloads complete. Num results: {downloadResults?.Count ?? -1}");
			//listBox_downloadFailedList.Visible = true;
			if (downloadResults?.Count > 0)
			{
				bool failedAny = false;
				listBox_downloadFailedList.Items.Clear();
				foreach (DependencyDownloadResult downloadResult in downloadResults)
				{
					ModDependencyEntry entry = downloadResult.entry;
					string name = entry.GetName();
					string message = downloadResult.message;
					string result = GetDownloadSpacerString(name, message);
					int messageLen = message.Length;
					listBox_downloadFailedList.Items.Add(result);
				}

				if (failedAny)
				{
					label_downloadStatusDesc.Text = INSTALL_STATUS_DONE;
					label_endTitle.Text = END_DOWNLOAD_ERRORS_TITLE;
					label_endDesc.Text = END_DOWNLOAD_ERRORS_DESC;
				}
			}
			button_nextStage.Enabled = true;
		}

		private async Task<List<DependencyDownloadResult>> DownloadSelectedDependencies()
		{
			List<DependencyDownloadResult> downloadResults = new();

			// iterate through selected downloads list in order and download each item
			int i = 0;
			int numDependenciesQueued = selectedModsToInstall.Count;
			foreach (ModDependencyEntry dependencyEntry in selectedModsToInstall)
			{
				string entryName = dependencyEntry.GetName();
				LogMessage("Downloading", entryName);

				// set current download desc
				label_downloadStatusDesc.Text = $"[{i + 1}/{numDependenciesQueued}] Downloading \"{entryName}\"...";

				//reset download progress bar
				progressBar_downloadIndividual.Value = 0;

				Action<double?, long, long?> callbackSetDownloadProgress = SetDownloadProgressBar;

				// update status to "in progress"
				listBox_downloadList.Items.RemoveAt(i);
				listBox_downloadList.Items.Insert(i, GetDownloadSpacerString(entryName, INSTALL_STATUS_INPROGRESS));

				string? errorMsg = await InstallerWrapper.DownloadDependency(dependencyEntry, callbackSetDownloadProgress);

				if (!string.IsNullOrEmpty(errorMsg))
				{
					LogMessage("Download fail");

					// update status to "failed" (with reason)
					listBox_downloadList.Items.RemoveAt(i);
					listBox_downloadList.Items.Insert(i, GetDownloadSpacerString(entryName, INSTALL_STATUS_FAILED.Replace("$reason$", errorMsg)));

					downloadResults.Add(new DependencyDownloadResult(false, dependencyEntry, INSTALL_STATUS_FAILED.Replace("$reason$", errorMsg)));
				}
				else
				{
					// update status to "done"
					listBox_downloadList.Items.RemoveAt(i);
					listBox_downloadList.Items.Insert(i, GetDownloadSpacerString(entryName, INSTALL_STATUS_DONE));

					downloadResults.Add(new DependencyDownloadResult(true, dependencyEntry, INSTALL_STATUS_SUCCESS));
					LogMessage("Download success");
				}

				i++;
			}

			return downloadResults;
		}

		private void MoveToNextStage()
		{
			if (nextPageOverride != null)
			{
				MoveToStage((int)nextPageOverride);
				nextPageOverride = null;
				return;
			}
			if (currentPage < panels.Count - 1)
			{
				MoveToStage(currentPage + 1);
			}
		}

		private void MoveToStage(int nextStage)
		{
			int prevStage = currentPage;
			bool isWithinStageBounds(int i)
			{
				return (i <= panels.Count - 1 && i >= 0);
			}
			if (isWithinStageBounds(prevStage) && isWithinStageBounds(nextStage))
			{

				button_nextStage.Enabled = nextStage < panels.Count - 1;
				button_prevStage.Enabled = (nextStage > 0);

				Panel prevPanel = panels[prevStage];
				prevPanel.Hide();
				System.Windows.Forms.Label prevLabel = labels[prevStage];
				prevLabel.ForeColor = SystemColors.ControlDark;

				Panel nextPanel = panels[nextStage];
				nextPanel.Show();
				System.Windows.Forms.Label nextLabel = labels[nextStage];
				nextLabel.ForeColor = Control.DefaultForeColor;
				LogMessage($"paging from {currentPage} to {nextStage}");

				currentPage = nextStage;
			}
			else
			{
				LogMessage($"Couldn't navigate to next page- one or more indices was out of bounds! previous {prevStage}, next {nextStage}");
			}

		}

		private void MoveToPrevStage()
		{
			nextPageOverride = null;
			if (currentPage > 0)
			{
				MoveToStage(currentPage - 1);
			}
		}

		private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
		{ }

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{ }

		private void linkLabelDiscord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			InstallerWrapper.BrowserOpenDiscord();
		}

		private void linkLabelHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			InstallerWrapper.BrowserOpenHomepage();
		}

		private void linkLabelWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			InstallerWrapper.BrowserOpenWiki();
		}

		private void linkLabelTroubleshooting_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			InstallerWrapper.BrowserOpenInstructions();
		}

		private void Form1_Load(object sender, EventArgs e)
		{ }

		private void button_browsePath_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog1.ShowDialog();
		}

		private void button_nextStage_Click(object sender, EventArgs e)
		{
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
//				CheckExistingMods();
			}
		}

		private void button_browseInstallPath_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				richTextBox_pd2InstallPath.Text = folderBrowserDialog1.SelectedPath + @"\";
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
			/*
				int i = new Random().Next(DUCKSOUNDS.Length);
				string snd = DUCKSOUNDS[i];

				using (var player = new System.Media.SoundPlayer(snd))
				{
					try
					{
						player.Play();
					}
					catch (Exception e)
					{
						LogMessage($"Could not play sound: {e.Message}");
					}
				}
			*/
		}

		private void label_modDependenciesItemMouseverDescription_Click(object sender, EventArgs e)
		{ }

		private void button_finalQuit_Click(object sender, EventArgs e)
		{
			CallbackOnQuitButtonPressed();
		}

		private void button_finishAndLaunch_Click(object sender, EventArgs e)
		{
			string? steamDir = InstallerWrapper.GetSteamDirectory();
			if (steamDir != null)
			{
				try
				{
					InstallerWrapper.LaunchPD2();
				}
				catch (Exception er)
				{
					LogMessage($"Could not launch PAYDAY 2: {er}");
				}
			}
			CallbackOnQuitButtonPressed();
		}

		private void button_openTempFolder_Click(object sender, EventArgs e)
		{
			InstallerWrapper.OpenTempDirectory();
		}

		private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
		{ }

		private void panel_stage5_Paint(object sender, PaintEventArgs e)
		{ }

		private void label_stage1Desc_Click(object sender, EventArgs e)
		{ }

		private void label_communityTitle_Click(object sender, EventArgs e)
		{ }

		private void label_endTitle_Click(object sender, EventArgs e)
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